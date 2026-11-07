using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public static PlayerInputHandler Instance { get; private set; }

    // Ganti 'PlayerInputActions' jadi nama class yang di-generate Unity (misal: PlayerInput)
    private InputSystem_Actions inputActions;

    public event Action OnInteractPressed;
    public float MoveInput { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Inisialisasi sesuai nama C# class dari Input Action Asset-mu
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Interact.performed += Interact_performed;
    }

    private void OnDisable()
    {
        inputActions.Player.Interact.performed -= Interact_performed;
        inputActions.Player.Disable();
    }

    private void Update()
    {
        Vector2 moveVector = inputActions.Player.Move.ReadValue<Vector2>();
        MoveInput = moveVector.x;
    }

    private void Interact_performed(InputAction.CallbackContext context)
    {
        OnInteractPressed?.Invoke();
    }

    public void SetInputState(bool enable)
    {
        if (enable)
            inputActions.Player.Enable();
        else
            inputActions.Player.Disable();
    }
}