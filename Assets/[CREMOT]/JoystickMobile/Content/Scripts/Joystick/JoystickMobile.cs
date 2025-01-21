using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CREMOT.JoystickMobile
{
    public class JoystickMobile : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        #region Fields
        private RectTransform _background;

        [SerializeField] private RectTransform _handle;

        private Vector2 _inputVector;

        #endregion

        #region Properties
        public Vector2 InputVector { get => _inputVector; set => _inputVector = value; }

        public float Horizontal { get => _inputVector.x; }
        public float Vertical { get => _inputVector.y; }

        #endregion

        #region Delegates

        public event Action<Vector2> OnInputVectorUpdated;
        public UnityEvent<Vector2> OnInputVectorUpdatedUnity;

        #endregion


        private void Awake()
        {
            _background = GetComponent<RectTransform>();
        }

        #region Interfaces Drag Joystick
        public void OnDrag(PointerEventData eventData)
        {
            if (_handle == null || _background == null) return;

            Vector2 position;

            bool isValid = RectTransformUtility.ScreenPointToLocalPointInRectangle(_background, eventData.position, eventData.pressEventCamera, out position);

            if (!isValid)
            {
                Debug.LogWarning("Invalid drag position. Make sure the RectTransform is set up correctly.");
                return;
            }

            // Normalize relative to the size of the joystick
            Vector2 backgroundSize = _background.rect.size;

            // Normalize position relative to the background size and to match circle radius
            position.x /= backgroundSize.x / 2; // Divide by half width
            position.y /= backgroundSize.y / 2; // Divide by half height


            // Clamp the vector to a magnitude of 1
            _inputVector = (position.magnitude > 1.0f) ? position.normalized : position;

            _handle.anchoredPosition = new Vector2(_inputVector.x * (backgroundSize.x / 2), _inputVector.y * (backgroundSize.y / 2));

            OnInputVectorUpdated?.Invoke(_inputVector);
            OnInputVectorUpdatedUnity?.Invoke(_inputVector);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _inputVector = Vector2.zero;

            if (_handle != null)
            {
                _handle.anchoredPosition = Vector2.zero;
            }

            OnInputVectorUpdated?.Invoke(_inputVector);
            OnInputVectorUpdatedUnity?.Invoke(_inputVector);
        }

        #endregion
    }
}
