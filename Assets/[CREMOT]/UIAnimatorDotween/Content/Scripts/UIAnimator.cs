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
        }

        [System.Serializable]
        public class AnimationSettings
        {
            [SerializeField] private EAnimationType _animationType;
            [SerializeField] private float _duration = 1f;
            [SerializeField] private Ease _ease = Ease.OutQuad;
            [SerializeField] private Transform _targetMove;
            [SerializeField] private Vector3 _targetScale;
            [SerializeField] private bool _playOnStart;

            public UnityEvent OnAnimationFinished;

            public EAnimationType AnimationType { get => _animationType; set => _animationType = value; }
            public float Duration { get => _duration; set => _duration = value; }
            public Ease Ease { get => _ease; set => _ease = value; }
            public Transform TargetMove{ get => _targetMove; set => _targetMove = value; }
            public bool PlayOnStart { get => _playOnStart; set => _playOnStart = value; }
            public Vector3 TargetScale { get => _targetScale; set => _targetScale = value; }
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
            else if (TryGetComponent<Image>(out Image image))
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

            }
        }
        private void AnimateFade(float targetAlpha, float duration, Ease ease, AnimationSettings settings)
        {
            if (_canvasGroup == null && _image == null)
            {
                Debug.LogError("CanvasGroup or Image is required for Fade animations. Please add a CanvasGroup component.");
                return;
            }

            if (_canvasGroup != null)
            {
                TweenerCore<float, float, FloatOptions> t = DOTween.To(() => _canvasGroup.alpha, x => _canvasGroup.alpha = x, targetAlpha, duration);
                t.SetTarget(_canvasGroup).OnComplete(() => NotifyAnimationFinished(settings));
            }
            else if (_image != null)
            {
                Color currentColor = _image.color;
                TweenerCore<Color, Color, ColorOptions> t = DOTween.ToAlpha(() => _image.color, x => _image.color = x, targetAlpha, duration);
                t.SetTarget(_image).OnComplete(() => NotifyAnimationFinished(settings));
            }
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
