using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CREMOT.UIAnimatorDotween
{
    [CustomEditor(typeof(UIAnimator))]
    public class UIAnimatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            UIAnimator animator = (UIAnimator)target;

            if (animator.Animations == null || animator.Animations.Length == 0)
            {
                if (GUILayout.Button("Add Animation"))
                {
                    animator.Animations = new UIAnimator.AnimationSettings[1];
                    animator.Animations[0] = new UIAnimator.AnimationSettings();
                }
            }

            if (animator.Animations != null)
            {
                for (int i = 0; i < animator.Animations.Length; i++)
                {
                    EditorGUILayout.LabelField($"Animation {i + 1}", EditorStyles.boldLabel);

                    animator.Animations[i].AnimationType = (UIAnimator.EAnimationType)EditorGUILayout.EnumPopup("Type", animator.Animations[i].AnimationType);
                    animator.Animations[i].Duration = EditorGUILayout.FloatField("Duration", animator.Animations[i].Duration);
                    animator.Animations[i].Ease = (Ease)EditorGUILayout.EnumPopup("Ease", animator.Animations[i].Ease);

                    if (animator.Animations[i].AnimationType == UIAnimator.EAnimationType.MOVETO)
                    {
                        animator.Animations[i].TargetMove = (Transform)EditorGUILayout.ObjectField("Target Move", animator.Animations[i].TargetMove, typeof(Transform), true);
                    }

                    if (animator.Animations[i].AnimationType == UIAnimator.EAnimationType.SCALETO)
                    {
                        animator.Animations[i].TargetScale = EditorGUILayout.Vector3Field("Target Scale", animator.Animations[i].TargetScale);
                    }

                    animator.Animations[i].PlayOnStart = EditorGUILayout.Toggle("Play On Start", animator.Animations[i].PlayOnStart);

                    if (animator.Animations[i].AnimationType == UIAnimator.EAnimationType.COLORTO)
                    {
                        animator.Animations[i].TargetColor = EditorGUILayout.ColorField("Target Color", animator.Animations[i].TargetColor);
                    }

                    // ---------------- Event field ---------------------
                    SerializedObject serializedObject = new SerializedObject(animator);
                    SerializedProperty animationsArray = serializedObject.FindProperty("_animations");
                    SerializedProperty currentAnimation = animationsArray.GetArrayElementAtIndex(i);
                    SerializedProperty onAnimationFinished = currentAnimation.FindPropertyRelative("OnAnimationFinished");
                    EditorGUILayout.PropertyField(onAnimationFinished, new GUIContent("OnAnimationFinished"));
                    // ------------------------------------------------------


                    serializedObject.ApplyModifiedProperties();

                    if (GUILayout.Button("Remove Animation"))
                    {
                        var tempList = new System.Collections.Generic.List<UIAnimator.AnimationSettings>(animator.Animations);
                        tempList.RemoveAt(i);
                        animator.Animations = tempList.ToArray();
                    }

                    EditorGUILayout.Space();
                }

                if (GUILayout.Button("Add Animation"))
                {
                    var tempList = new System.Collections.Generic.List<UIAnimator.AnimationSettings>(animator.Animations);
                    tempList.Add(new UIAnimator.AnimationSettings());
                    animator.Animations = tempList.ToArray();
                }
            }

            if (GUILayout.Button("Play All"))
            {
                animator.PlayAllAnimations();
            }

            EditorUtility.SetDirty(target);
        }
    }
}
