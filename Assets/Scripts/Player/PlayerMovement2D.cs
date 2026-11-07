
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool canMove = true;
    [SerializeField] private Transform character;
    

    private Rigidbody2D rb;
    private Animator animator;
    private float horizontalInput;
    private bool isFacingRight = false;

    public bool CanMove
    {
        get => canMove;
        set => canMove = value;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();

        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (!canMove ||
            Dialogue.IsAnyDialogueActive ||
            PlayerInputHandler.Instance == null)
        {
            horizontalInput = 0f;
            UpdateAnimator();
            return;
        }

        horizontalInput = PlayerInputHandler.Instance.MoveInput;

        HandleFacing();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (!canMove || Dialogue.IsAnyDialogueActive)
        {
            horizontalInput = 0f;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(
            horizontalInput * moveSpeed,
            rb.linearVelocity.y
        );
    }

    private void HandleFacing()
    {
        if (character == null)
        {
            return;
        }

        if (horizontalInput > 0f && !isFacingRight)
        {
            Flip();
        }
        else if (horizontalInput < 0f && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 scale = character.localScale;
        scale.x *= -1f;
        character.localScale = scale;
    }


    private void UpdateAnimator()
    {
        if (animator != null)
        {
            animator.SetBool("IsWalking", Mathf.Abs(horizontalInput) > 0.01f);
        }
    }

    
}