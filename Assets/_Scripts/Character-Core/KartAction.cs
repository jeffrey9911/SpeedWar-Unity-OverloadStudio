//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.3
//     from Assets/_Scripts/Character-Core/KartAction.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @KartAction : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @KartAction()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""KartAction"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""f5a3e0fc-affb-4c09-94d1-4856c61e3b91"",
            ""actions"": [
                {
                    ""name"": ""Handbrake"",
                    ""type"": ""Button"",
                    ""id"": ""8bc5a38e-7d6f-4c95-aa71-253ee62d020b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""UseItem"",
                    ""type"": ""Button"",
                    ""id"": ""a9ad2f71-8f89-4b3b-9e9d-e069d8c4de28"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SwitchItem"",
                    ""type"": ""Button"",
                    ""id"": ""1dc30de7-e0fb-4a02-b526-1302818751f6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""677f4df0-5461-4eab-a7c4-6dc80fefebe0"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Handbrake"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""66d94b12-eccd-4626-bf19-54d54ca6d759"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UseItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4fa23188-a7ce-40bf-b7d2-7bda6df4f0e3"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Handbrake = m_Player.FindAction("Handbrake", throwIfNotFound: true);
        m_Player_UseItem = m_Player.FindAction("UseItem", throwIfNotFound: true);
        m_Player_SwitchItem = m_Player.FindAction("SwitchItem", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Handbrake;
    private readonly InputAction m_Player_UseItem;
    private readonly InputAction m_Player_SwitchItem;
    public struct PlayerActions
    {
        private @KartAction m_Wrapper;
        public PlayerActions(@KartAction wrapper) { m_Wrapper = wrapper; }
        public InputAction @Handbrake => m_Wrapper.m_Player_Handbrake;
        public InputAction @UseItem => m_Wrapper.m_Player_UseItem;
        public InputAction @SwitchItem => m_Wrapper.m_Player_SwitchItem;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Handbrake.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHandbrake;
                @Handbrake.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHandbrake;
                @Handbrake.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHandbrake;
                @UseItem.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUseItem;
                @UseItem.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUseItem;
                @UseItem.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUseItem;
                @SwitchItem.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwitchItem;
                @SwitchItem.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwitchItem;
                @SwitchItem.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwitchItem;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Handbrake.started += instance.OnHandbrake;
                @Handbrake.performed += instance.OnHandbrake;
                @Handbrake.canceled += instance.OnHandbrake;
                @UseItem.started += instance.OnUseItem;
                @UseItem.performed += instance.OnUseItem;
                @UseItem.canceled += instance.OnUseItem;
                @SwitchItem.started += instance.OnSwitchItem;
                @SwitchItem.performed += instance.OnSwitchItem;
                @SwitchItem.canceled += instance.OnSwitchItem;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    public interface IPlayerActions
    {
        void OnHandbrake(InputAction.CallbackContext context);
        void OnUseItem(InputAction.CallbackContext context);
        void OnSwitchItem(InputAction.CallbackContext context);
    }
}