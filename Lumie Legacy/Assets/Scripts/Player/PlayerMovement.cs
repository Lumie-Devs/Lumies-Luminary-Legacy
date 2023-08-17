using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] private PlayerActions playerActions;
    [SerializeField] private PlayerMoveActions playerMoveActions;
    private InputActions actions;
    private Rigidbody2D rb;
    // private Animator anim;
    private SpriteRenderer sr;
    private Vector2 moveInput;

    public static float maxSpeed = 9f;
    public float speed;
    public float jumpSpeed = 35f;
    public float jumpHeight = 5f;
    
    public float dashSpeed = 20f;
    public float dashDuration = 0.07f;
    public float dashCooldown = .5f;

    public float groundCheckDistance = 0.1f;
    public Transform leftCheck;
    public Transform middleCheck;
    public Transform rightCheck;
    public LayerMask surfaceLayer;

    public bool canDash = true;
    public bool canJump = true;
    public bool isDashing = false;
    public bool isGrounded = false;
    public bool isJumping = false;
    public bool isComboing = false;
    public bool isSmashing = false;
    private float initialJumpPosition;
    private float originalGravityScale;

    // private AudioSource audioSource;
    // [SerializeField] private AudioClip walkSound;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        // audioSource = GetComponent<AudioSource>();

        originalGravityScale = rb.gravityScale;

        actions = new InputActions();

        speed = maxSpeed;
    }

    private void OnEnable()
    {
        actions.Player.Enable();

        actions.Player.Direction.performed += MoveCharacter;
        actions.Player.Direction.canceled += StopCharacter;
        actions.Player.Action.performed += DoAction;
        actions.Player.MoveAction.performed += DoMoveAction;
        actions.Player.Jump.performed += Jump;
        actions.Player.Dash.performed += Dash;
    }

    private void OnDisable() {
        actions.Player.Direction.performed -= MoveCharacter;
        actions.Player.Direction.canceled -= StopCharacter;
        actions.Player.Action.performed -= DoAction;
        actions.Player.MoveAction.performed -= DoMoveAction;
        actions.Player.Jump.performed -= Jump;
        actions.Player.Dash.performed -= Dash;

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
        if (!canJump || isDashing || isSmashing) return;

        if (isComboing) StopComboing();
        
        ApplyJump(jumpSpeed);
    }

    public void ApplyJump(float force)
    {
        isJumping = true;
        canJump = false;

        initialJumpPosition = transform.position.y;
        rb.velocity = new Vector2(rb.velocity.x, force);
    }

    private void Dash(InputAction.CallbackContext context)
    {
        if (canDash && moveInput.x != 0)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private void DoAction(InputAction.CallbackContext context)
    {
        if (isDashing || isSmashing) return;
            
        
        playerActions.DoAction(moveInput.y, isGrounded);
    }

    private void DoMoveAction(InputAction.CallbackContext context)
    {
        if (isDashing || isSmashing) return;

        playerMoveActions.DoAction();
    }

    private void CancelMovement()
    {
        moveInput = Vector2.zero;
        // anim.SetBool("Moving", false);
    }

    public void Comboing(float moveSpeedDecrease)
    {
        isComboing = true;
        isJumping = false;
        speed = moveSpeedDecrease;
    }

    public void StopComboing()
    {
        isComboing = false;
        speed = maxSpeed;
    }

    public void Smashing(float smashSpeed)
    {
        isSmashing = true;
        isJumping = false;
        rb.velocity = new Vector2(0, -smashSpeed);
    }

    private void FixedUpdate()
    {
        Movement();
        GroundCheck();
        ControlJump();
    }

    private void Movement()
    {
        if (isDashing || isSmashing) return;

        float moveX = moveInput.x * speed;
        float moveY = isComboing ? 0 : rb.velocity.y;
        rb.velocity = new Vector2(moveX, moveY);
    }

    private void GroundCheck()
    {
        bool leftGrounded = Physics2D.Raycast(leftCheck.position, Vector2.down, groundCheckDistance, surfaceLayer);
        bool middleGrounded = Physics2D.Raycast(middleCheck.position, Vector2.down, groundCheckDistance, surfaceLayer);
        bool rightGrounded = Physics2D.Raycast(rightCheck.position, Vector2.down, groundCheckDistance, surfaceLayer);

        isGrounded = leftGrounded || middleGrounded || rightGrounded;

        if (isGrounded) 
        {
            playerActions.ResetAirAttacks();
            isSmashing = false;

            if (!isJumping) canJump = true;
        }

    }

    private void ControlJump()
    {
        if (isJumping && transform.position.y >= initialJumpPosition + jumpHeight || canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            isJumping = false; // Reset jumping flag
        } else if (canJump && isJumping)
        {
            isJumping = false;
        }
    }

    private IEnumerator DashCoroutine()
    {
        // Set canDash to false to prevent dashing again until cooldown
        isDashing = true;
        canDash = false;

        // Disable gravity for the Rigidbody2D component
        rb.gravityScale = 0f;

        // Determine dash direction based on the sprite's orientation
        Vector2 dashDirection = sr.flipX ? Vector2.left : Vector2.right;

        // Apply dash force, maintaining the current y velocity
        rb.velocity = new Vector2(dashDirection.x * dashSpeed, 0f);
        
        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravityScale;

        // Reset velocity to what it was before the dash
        rb.velocity = Vector2.zero;

        isDashing = false;

        while (!isGrounded)
        {
            yield return null;
        }

        // Wait for the cooldown
        yield return new WaitForSeconds(dashCooldown);

        // Allow dashing again
        canDash = true;
    }

    // public void PlayMoveSound()
    // {
    //     audioSource.clip = walkSound;
    //     audioSource.time = 1.88f;
    //     audioSource.Play();
    // }
}