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
        public enum EAnimationType
        {
            FADEIN,
            FADEOUT,
            MOVETO,
            SCALETO,
            COLORTO
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

            [SerializeField] private bool _playOnStart;

            public UnityEvent OnAnimationFinished;

            public EAnimationType AnimationType { get => _animationType; set => _animationType = value; }
            public float Duration { get => _duration; set => _duration = value; }
            public Ease Ease { get => _ease; set => _ease = value; }
            public Transform TargetMove{ get => _targetMove; set => _targetMove = value; }
            public bool PlayOnStart { get => _playOnStart; set => _playOnStart = value; }
            public Vector3 TargetScale { get => _targetScale; set => _targetScale = value; }
            public Color TargetColor { get => _targetColor; set => _targetColor = value; }
        }
        #endregion


        #region Fields

        [SerializeField] private AnimationSettings[] _animations;

        private CanvasGroup _canvasGroup;
        private Image _image;


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
                case EAnimationType.FADEIN:
                    AnimateFade(1, settings.Duration, settings.Ease, settings);
                    break;
                case EAnimationType.FADEOUT:
                    AnimateFade(0, settings.Duration, settings.Ease, settings);
                    break;
                case EAnimationType.MOVETO:
                    transform.DOMove(settings.TargetMove.position, settings.Duration).SetEase(settings.Ease).OnComplete(()=> NotifyAnimationFinished(settings));
                    break;
                case EAnimationType.SCALETO:
                    transform.DOScale(settings.TargetScale, settings.Duration).SetEase(settings.Ease).OnComplete(() => NotifyAnimationFinished(settings));
                    break;
                case EAnimationType.COLORTO:
                    AnimColorTo(settings.TargetColor, settings.Duration, settings.Ease, settings);
                    break;

            }
        }
        private void AnimateFade(float targetAlpha, float duration, Ease ease, AnimationSettings settings)
        {
            if (_canvasGroup == null && _image == null)
            {
                Debug.LogError("CanvasGroup or Image is required for Fade animations. Please add a CanvasGroup or Image component.");
                return;
            }

            if (_canvasGroup != null)
            {
                _canvasGroup.DOFade(targetAlpha, duration).OnComplete(() => NotifyAnimationFinished(settings));
            }
            else if (_image != null)
            {
                _image.DOFade(targetAlpha, duration).OnComplete(() => NotifyAnimationFinished(settings));
            }
        }
        private void AnimColorTo(Color targetColor, float duration, Ease ease, AnimationSettings settings)
        {
            if (_image == null)
            {
                Debug.LogError("Image is required for Color animations. Please add an Image component.");
                return;
            }

            _image.DOColor(targetColor, duration).OnComplete(() => NotifyAnimationFinished(settings));
        }

        private void NotifyAnimationFinished(AnimationSettings settings)
        {
            settings.OnAnimationFinished?.Invoke();
        }

        public void PlayAllAnimations()
        {
            foreach (var animation in _animations)
            {
                PlayAnimation(animation);
            }
        }

        #endregion

    }
}
