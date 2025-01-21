using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CREMOT.JoystickMobile
{
    public class PlayerController : MonoBehaviour
    {
        #region Fields

        private Rigidbody _rb;

        [Header("Input Setup")]
        [SerializeField] private CREMOT.JoystickMobile.JoystickMobile _joystick;


        [Header("Movements parameters")]
        [SerializeField] private float _speed;

        private Vector2 _moveInputVector;

        #endregion


        private void Awake()
        {
            _moveInputVector = Vector2.zero;

            _rb = GetComponent<Rigidbody>();
        }
        private void Start()
        {
            if (_joystick != null)
            {
                _joystick.OnInputVectorUpdated += SetMoveInputVector;
            }
        }
        private void OnDestroy()
        {
            if (_joystick != null)
            {
                _joystick.OnInputVectorUpdated -= SetMoveInputVector;
            }
        }


        #region Movements
        public void SetMoveInputVector(Vector2 moveInputVector)
        {
            _moveInputVector = moveInputVector;
        }
        private void MovePlayer(Vector2 moveInputVector)
        {
            if (_rb == null)    return;

            Vector3 moveVector = new Vector3();

            moveVector.x = moveInputVector.x;

            moveVector.z = moveInputVector.y;

            _rb.MovePosition(_rb.position + moveVector * _speed * Time.fixedDeltaTime);
        }

        #endregion

        private void FixedUpdate()
        {
            MovePlayer(_moveInputVector);
        }
    }
}
