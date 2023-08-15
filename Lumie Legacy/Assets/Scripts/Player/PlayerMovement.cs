using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    public float speed = 5f;

    private InputActions actions;
    private Rigidbody2D rb;
    // private Animator anim;
    private SpriteRenderer sr;
    private Vector2 moveInput;
    public float jumpSpeed = 25f;
    public float jumpHeight = 5f;

    public Transform leftCheck;
    public Transform middleCheck;
    public Transform rightCheck;
    public float groundCheckDistance = 0.1f;
    public LayerMask surfaceLayer;
    private float initialJumpPosition;
    private bool isGrounded = false;
    private bool isJumping = false;

    // private AudioSource audioSource;
    // [SerializeField] private AudioClip walkSound;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        // audioSource = GetComponent<AudioSource>();

        actions = new InputActions();
    }

    private void OnEnable()
    {
        actions.Player.Enable();

        actions.Player.Direction.performed += MoveCharacter;
        actions.Player.Direction.canceled += StopCharacter;
        actions.Player.Action.performed += DoAction;
        actions.Player.Jump.performed += Jump;
    }

    private void OnDisable() {
        actions.Player.Direction.performed -= MoveCharacter;
        actions.Player.Direction.canceled -= StopCharacter;
        actions.Player.Action.performed -= DoAction;
        actions.Player.Jump.performed -= Jump;

        actions.Player.Disable();

        CancelMovement();
        
        rb.velocity = Vector3.zero;
    }

    private void MoveCharacter(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        // anim.SetBool("Moving", true);

        if (moveInput.x > 0) sr.flipX = false;

        if (moveInput.x < 0) sr.flipX = true;
    }
    private void StopCharacter(InputAction.CallbackContext context)
    {
        CancelMovement();
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            initialJumpPosition = transform.position.y;
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            isJumping = true;
        }
    }

    private void DoAction(InputAction.CallbackContext context)
    {

    }

    private void CancelMovement()
    {
        moveInput = Vector2.zero;
        // anim.SetBool("Moving", false);
    }

    private void FixedUpdate()
    {
        Movement();
        GroundCheck();
        ControlJump();
    }

    private void Movement()
    {


        float moveX = moveInput.x * speed;
        rb.velocity = new Vector2(moveX, rb.velocity.y);
    }

    private void GroundCheck()
    {
        bool leftGrounded = Physics2D.Raycast(leftCheck.position, Vector2.down, groundCheckDistance, surfaceLayer);
        bool middleGrounded = Physics2D.Raycast(middleCheck.position, Vector2.down, groundCheckDistance, surfaceLayer);
        bool rightGrounded = Physics2D.Raycast(rightCheck.position, Vector2.down, groundCheckDistance, surfaceLayer);

        isGrounded = leftGrounded || middleGrounded || rightGrounded;

    }

    private void ControlJump()
    {
        if (isJumping && transform.position.y >= initialJumpPosition + jumpHeight)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            isJumping = false; // Reset jumping flag
        }
    }

    // public void PlayMoveSound()
    // {
    //     audioSource.clip = walkSound;
    //     audioSource.time = 1.88f;
    //     audioSource.Play();
    // }
}