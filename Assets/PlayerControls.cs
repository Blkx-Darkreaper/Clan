// GENERATED AUTOMATICALLY FROM 'Assets/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""83fd148c-b7a0-4bf3-8b9d-4ef5689b8cb1"",
            ""actions"": [
                {
                    ""name"": ""Toggle Item"",
                    ""type"": ""Button"",
                    ""id"": ""44ca9590-9331-4cdf-89db-79580a7da43c"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Button"",
                    ""id"": ""5a7c90f8-c210-4397-a4a9-6992b4573af7"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Item Primary"",
                    ""type"": ""Button"",
                    ""id"": ""aee726df-7a14-42b5-a0df-9477062c0f61"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Item Secondary"",
                    ""type"": ""Button"",
                    ""id"": ""70561482-4cda-48e4-b75d-6a903d225114"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Manipulate"",
                    ""type"": ""Button"",
                    ""id"": ""7ab9c724-5945-4abd-b207-fc064813cc84"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""a7c56d20-f233-4763-be42-c6773700c0fc"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Toggle Item"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""95c023f4-af21-44ec-bd34-c033b230eb4a"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0552f283-4341-47a6-8cbd-f99c08321be9"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Item Primary"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dcdda52c-9dbf-4e21-b172-512634bdf798"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Item Secondary"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e1a04bc7-ba46-4d3c-ab3d-da4b27e7c5b3"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Manipulate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_ToggleItem = m_Gameplay.FindAction("Toggle Item", throwIfNotFound: true);
        m_Gameplay_Move = m_Gameplay.FindAction("Move", throwIfNotFound: true);
        m_Gameplay_ItemPrimary = m_Gameplay.FindAction("Item Primary", throwIfNotFound: true);
        m_Gameplay_ItemSecondary = m_Gameplay.FindAction("Item Secondary", throwIfNotFound: true);
        m_Gameplay_Manipulate = m_Gameplay.FindAction("Manipulate", throwIfNotFound: true);
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

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_ToggleItem;
    private readonly InputAction m_Gameplay_Move;
    private readonly InputAction m_Gameplay_ItemPrimary;
    private readonly InputAction m_Gameplay_ItemSecondary;
    private readonly InputAction m_Gameplay_Manipulate;
    public struct GameplayActions
    {
        private @PlayerControls m_Wrapper;
        public GameplayActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @ToggleItem => m_Wrapper.m_Gameplay_ToggleItem;
        public InputAction @Move => m_Wrapper.m_Gameplay_Move;
        public InputAction @ItemPrimary => m_Wrapper.m_Gameplay_ItemPrimary;
        public InputAction @ItemSecondary => m_Wrapper.m_Gameplay_ItemSecondary;
        public InputAction @Manipulate => m_Wrapper.m_Gameplay_Manipulate;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @ToggleItem.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnToggleItem;
                @ToggleItem.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnToggleItem;
                @ToggleItem.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnToggleItem;
                @Move.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @ItemPrimary.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnItemPrimary;
                @ItemPrimary.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnItemPrimary;
                @ItemPrimary.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnItemPrimary;
                @ItemSecondary.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnItemSecondary;
                @ItemSecondary.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnItemSecondary;
                @ItemSecondary.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnItemSecondary;
                @Manipulate.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnManipulate;
                @Manipulate.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnManipulate;
                @Manipulate.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnManipulate;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ToggleItem.started += instance.OnToggleItem;
                @ToggleItem.performed += instance.OnToggleItem;
                @ToggleItem.canceled += instance.OnToggleItem;
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @ItemPrimary.started += instance.OnItemPrimary;
                @ItemPrimary.performed += instance.OnItemPrimary;
                @ItemPrimary.canceled += instance.OnItemPrimary;
                @ItemSecondary.started += instance.OnItemSecondary;
                @ItemSecondary.performed += instance.OnItemSecondary;
                @ItemSecondary.canceled += instance.OnItemSecondary;
                @Manipulate.started += instance.OnManipulate;
                @Manipulate.performed += instance.OnManipulate;
                @Manipulate.canceled += instance.OnManipulate;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);
    public interface IGameplayActions
    {
        void OnToggleItem(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
        void OnItemPrimary(InputAction.CallbackContext context);
        void OnItemSecondary(InputAction.CallbackContext context);
        void OnManipulate(InputAction.CallbackContext context);
    }
}
