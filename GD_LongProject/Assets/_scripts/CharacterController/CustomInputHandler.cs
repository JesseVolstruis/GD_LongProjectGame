using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CustomInputHandler : InputAxisControllerBase<CustomInputHandler.Reader>
{
    [Header("Input Source Override")]
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

        // Subscribe only to *this* player’s actions
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
            UpdateControllers(); // pushes cached values into Cinemachine
    }

    [Serializable]
    public class Reader : IInputAxisReader
    {
        public InputActionReference input;
        private Vector2 _mValue;

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
                        // Mouse delta: scale directly
                        _mValue = raw * mouseSensitivity; 
                    }
                    else
                    {
                        // Controller stick: scale with deltaTime
                        _mValue = raw * controllerSensitivity * Time.deltaTime;
                    }
                }
                else
                {
                    _mValue = new Vector2(action.ReadValue<float>(), 0);
                }
            }
        }

        [SerializeField] private float mouseSensitivity = 5.0f;
        [SerializeField] private float controllerSensitivity = 50.0f;


        public float GetValue(UnityEngine.Object context, IInputAxisOwner.AxisDescriptor.Hints hint)
        {
            return hint == IInputAxisOwner.AxisDescriptor.Hints.Y ? _mValue.y : _mValue.x;
        }
    }
}