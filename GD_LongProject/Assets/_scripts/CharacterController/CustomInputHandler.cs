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
        public InputActionReference Input;
        private Vector2 m_Value;

        public void ProcessInput(InputAction action)
        {
            if (Input != null && Input.action.id == action.id)
            {
                if (action.expectedControlType == "Vector2")
                    m_Value = action.ReadValue<Vector2>();
                else
                    m_Value = new Vector2(action.ReadValue<float>(), 0);
            }
        }

        public float GetValue(UnityEngine.Object context, IInputAxisOwner.AxisDescriptor.Hints hint)
        {
            return hint == IInputAxisOwner.AxisDescriptor.Hints.Y ? m_Value.y : m_Value.x;
        }
    }
}