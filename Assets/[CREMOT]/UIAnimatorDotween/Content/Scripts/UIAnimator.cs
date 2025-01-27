using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CREMOT.UIAnimatorDotween
{
    public class UIAnimator : MonoBehaviour
    {
        #region Type / Settings
        [System.Serializable]
        public enum EAnimationType
        {
            FADEIN_0 = 0,
            FADEOUT_1 = 1,
            MOVETO_2 = 2,
            SCALETO_3 = 3,
            COLORTO_4 = 4,
            IDLE_INFINITE_5 = 5
        }

        [System.Serializable]
        public class AnimationSettings
        {
            [SerializeField] private EAnimationType _animationType;
            [SerializeField] private float _duration = 1f;
            [SerializeField] private Ease _ease = Ease.OutQuad;

            [SerializeField] private Transform _targetMove;
            [SerializeField] private Vector3 _targetScale;

            [SerializeField] private Color _targetColor;

            [SerializeField] private float _idleAmplitude;

            [SerializeField] private bool _playOnStart;

            private Tween _animationTween;

            public UnityEvent OnAnimationStarted;
            public UnityEvent OnAnimationFinished;

            public EAnimationType AnimationType { get => _animationType; set => _animationType = value; }
            public float Duration { get => _duration; set => _duration = value; }
            public Ease Ease { get => _ease; set => _ease = value; }
            public Transform TargetMove { get => _targetMove; set => _targetMove = value; }
            public bool PlayOnStart { get => _playOnStart; set => _playOnStart = value; }
            public Vector3 TargetScale { get => _targetScale; set => _targetScale = value; }
            public Color TargetColor { get => _targetColor; set => _targetColor = value; }
            public float IdleAmplitude { get => _idleAmplitude; set => _idleAmplitude = value; }
            public Tween AnimationTween { get => _animationTween; set => _animationTween = value; }
        }
        #endregion


        #region Fields

        [SerializeField] private AnimationSettings[] _animations;

        private CanvasGroup _canvasGroup;
        private Image _image;
        private SpriteRenderer _spriteRenderer;


        #endregion

        #region Properties
        public AnimationSettings[] Animations { get => _animations; set => _animations = value; }

        #endregion


        #region Handle Animations

        private void Awake()
        {
            if (TryGetComponent<CanvasGroup>(out CanvasGroup canvasGroup))
            {
                _canvasGroup = canvasGroup;
            }
            if (TryGetComponent<Image>(out Image image))
            {
                _image = image;
            }
            if (TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
            {
                _spriteRenderer = spriteRenderer;
            }
        }
        private void Start()
        {
            foreach (var animation in _animations)
            {
                if (animation.PlayOnStart)
                    PlayAnimation(animation);
            }
        }

        public void PlayAnimation(AnimationSettings settings)
        {
            switch (settings.AnimationType)
            {
                case EAnimationType.FADEIN_0:
                    settings.AnimationTween = AnimateFade(1, settings.Duration, settings.Ease, settings);
                    break;
                case EAnimationType.FADEOUT_1:
                    settings.AnimationTween = AnimateFade(0, settings.Duration, settings.Ease, settings);
                    break;
                case EAnimationType.MOVETO_2:
                    settings.AnimationTween = transform.DOMove(settings.TargetMove.position, settings.Duration).SetEase(settings.Ease).OnComplete(() => NotifyAnimationFinished(settings));
                    break;
                case EAnimationType.SCALETO_3:
                    settings.AnimationTween = transform.DOScale(settings.TargetScale, settings.Duration).SetEase(settings.Ease).OnComplete(() => NotifyAnimationFinished(settings));
                    break;
                case EAnimationType.COLORTO_4:
                    settings.AnimationTween = AnimColorTo(settings.TargetColor, settings.Duration, settings.Ease, settings);
                    break;
                case EAnimationType.IDLE_INFINITE_5:
                    settings.AnimationTween = AnimateIdleInfinite(1f, settings.Duration, settings.Ease, settings);
                    break;

            }

            if (settings != null)
                settings.OnAnimationStarted?.Invoke();
        }
        private Tween AnimateFade(float targetAlpha, float duration, Ease ease, AnimationSettings settings)
        {
            if (_canvasGroup == null && _image == null && _spriteRenderer == null)
            {
                Debug.LogError("CanvasGroup or Image is required for Fade animations. Please add a CanvasGroup or Image component.");
                return null;
            }

            if (_canvasGroup != null)
            {
                return _canvasGroup.DOFade(targetAlpha, duration).OnComplete(() => NotifyAnimationFinished(settings));
            }
            else if (_image != null)
            {
                return _image.DOFade(targetAlpha, duration).OnComplete(() => NotifyAnimationFinished(settings));
            }
            else if (_spriteRenderer != null)
            {
                return _spriteRenderer.DOFade(targetAlpha, duration).OnComplete(() => NotifyAnimationFinished(settings));
            }

            return null;
        }
        private Tween AnimColorTo(Color targetColor, float duration, Ease ease, AnimationSettings settings)
        {
            if (_image == null && _spriteRenderer == null)
            {
                Debug.LogError("Image is required for Color animations. Please add an Image component.");
                return null;
            }
            if (_image != null)
            {
                return _image.DOColor(targetColor, duration).OnComplete(() => NotifyAnimationFinished(settings));
            }
            else if (_spriteRenderer != null)
            {
                return _spriteRenderer.DOColor(targetColor, duration).OnComplete(() => NotifyAnimationFinished(settings));
            }

            return null;
        }

        private Tween AnimateIdleInfinite(float direction, float duration, Ease ease, AnimationSettings settings)
        {
            return transform.DOLocalMoveY(settings.IdleAmplitude * direction, duration).OnComplete(() => AnimateIdleInfinite(direction * -1f, duration, ease, settings));
        }

        private void NotifyAnimationFinished(AnimationSettings settings)
        {
            settings.OnAnimationFinished?.Invoke();
        }

        public void PlayAllAnimations()
        {
            foreach (var animation in _animations)
            {
                if (animation.AnimationTween != null)
                {
                    animation.AnimationTween.Kill();
                }
                PlayAnimation(animation);
            }
        }
        public void PlayAnimationAtIndex(int index)
        {
            if (index >= _animations.Length) return;

            AnimationSettings animation = _animations[index];

            if (animation == null) return;

            PlayAnimation(animation);
        }


        public void KillAllAnimations()
        {
            foreach (var animation in _animations)
            {
                if (animation.AnimationTween == null) continue;

                animation.AnimationTween.Kill();
            }
        }
        public void KillAllAnimationOfType(int typeIndex)
        {
            foreach (var animation in _animations)
            {
                if (animation.AnimationType != (EAnimationType)typeIndex) continue;

                if (animation.AnimationTween == null) continue;

                animation.AnimationTween.Kill();
            }
        }
        public void KillAnimationAtIndex(int index)
        {
            if (index >= _animations.Length) return;

            AnimationSettings animation = _animations[index];

            if (animation == null) return;

            if (animation.AnimationTween == null) return;

            animation.AnimationTween.Kill();
        }

        #endregion

    }
}
