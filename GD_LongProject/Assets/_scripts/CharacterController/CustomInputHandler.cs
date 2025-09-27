using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CustomInputHandler : InputAxisControllerBase<CustomInputHandler.Reader>
{
    [SerializeField] private PlayerInput playerInput;

    private void Awake()
    {
        if (playerInput == null)
            TryGetComponent(out playerInput);

        if (playerInput == null)
        {
            Debug.LogError("No PlayerInput found for CustomInputHandler");
            return;
        }

        playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
        playerInput.onActionTriggered += action =>
        {
            foreach (var controller in Controllers)
                controller.Input.ProcessInput(action.action);
        };
    }
    private void Update()
    {
        if (Application.isPlaying)
            UpdateControllers(); 
    }

    [Serializable]
    public class Reader : IInputAxisReader
    {
        public InputActionReference input;
        public PlayerValues playerValues;
        private Vector2 _mValue;
        [SerializeField] private float mouseSensitivity = 5.0f;
        [SerializeField] private float controllerSensitivity = 5000.0f;

        void Start()
        {
            mouseSensitivity = playerValues.mouseSensitivity;
            controllerSensitivity = playerValues.controllerSensitivity;
        }
        public void ProcessInput(InputAction action)
        {
            if (input != null && input.action.id == action.id)
            {
                if (action.expectedControlType == "Vector2")
                {
                    Vector2 raw = action.ReadValue<Vector2>();

                    // Detect mouse vs gamepad
                    if (action.activeControl.device is Mouse)
                    {
                        _mValue = raw * mouseSensitivity; 
                    }
                    else
                    {
                        _mValue = raw * controllerSensitivity * Time.deltaTime;
                    }
                }
                else
                {
                    _mValue = new Vector2(action.ReadValue<float>(), 0);
                }
            }
        }

        public float GetValue(UnityEngine.Object context, IInputAxisOwner.AxisDescriptor.Hints hint)
        {
            return hint == IInputAxisOwner.AxisDescriptor.Hints.Y ? _mValue.y : _mValue.x;
        }
    }
}