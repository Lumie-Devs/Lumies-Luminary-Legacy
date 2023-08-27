using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] private PlayerActions playerActions;
    private InputActions actions;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private Vector2 moveInput;

    public static float maxSpeed = 9f;
    public float speed;
    public float jumpSpeed = 35f;
    public float jumpHeight = 5f;
    public float catapultSpeed = 50f;
    public float catapultHeight = 8f;
    
    public float dashSpeed = 20f;
    public float dashDuration = 0.07f;
    public float dashCooldown = .5f;

    public float stunDuration = .3f;

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
    public bool isHanging = false;
    private float initialJumpPosition;
    private float originalGravityScale;

    // private AudioSource audioSource;
    // [SerializeField] private AudioClip walkSound;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
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

        bool isMoving = !isHanging && moveInput.x != 0;
        anim.SetBool("Moving", isMoving);

        if (isMoving) {
            DetermineCharacterDirection();
        }
    }

    private void StopCharacter(InputAction.CallbackContext context)
    {
        CancelMovement();
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (!canJump || isDashing || isSmashing) return;

        if (isComboing) StopComboing();

        float upSpeed = jumpSpeed;

        if (isHanging)
        {
            Unhang();
            upSpeed = catapultSpeed;
        } else {
            anim.SetTrigger("Jump");
        }
        
        ApplyJump(upSpeed);
    }

    private void Dash(InputAction.CallbackContext context)
    {
        if (!canDash || moveInput.x == 0 || isHanging) return;
        
        StartCoroutine(DashCoroutine());
        BreakJump();
    }

    private void DoAction(InputAction.CallbackContext context)
    {
        if (isDashing || isSmashing) return;

        if (isHanging)
        {
            transform.localScale *= new Vector2(-1,1);
            Unhang();
            playerActions.wallInRange = false;
        }
            
        BreakJump();
        playerActions.DoAction(moveInput.y, isGrounded);
    }

    private void DoMoveAction(InputAction.CallbackContext context)
    {
        if (isDashing || isSmashing || isHanging) return;

        BreakJump();
        playerActions.ThrowHammer();
    }

    private void DetermineCharacterDirection()
    {
        if (moveInput.x == 0) return;

        Vector3 newScale = transform.localScale;
        newScale.x = moveInput.x > 0 ? 1 : -1;

        transform.localScale = newScale;
    }

    private void CancelMovement()
    {
        moveInput = Vector2.zero;
        anim.SetBool("Moving", false);
    }

    public void ApplyJump(float force)
    {
        isJumping = true;
        canJump = false;

        initialJumpPosition = transform.position.y;
        rb.velocity = new Vector2(rb.velocity.x, force);
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
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.down * smashSpeed, ForceMode2D.Impulse);
        anim.SetTrigger("Smash");
    }

    private void FixedUpdate()
    {
        Movement();
        GroundCheck();
        ControlJump();
    }

    private void Movement()
    {
        if (isDashing || isHanging) return;

        float moveX = moveInput.x * speed;
        float moveY = isComboing ? 0 : rb.velocity.y;
        rb.velocity = new Vector2(moveX, moveY);
        anim.SetBool("Grounded", isGrounded);
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
        if (isJumping && transform.position.y >= initialJumpPosition + jumpHeight)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            BreakJump();
        }
    }

    public void HammerThrow()
    {
        rb.velocity = Vector2.zero;
    }

    public void Hang()
    {
        isHanging = true;
        rb.gravityScale = 0;
        canJump = true;
    }

    public void Unhang()
    {
        isHanging = false;
        rb.gravityScale = originalGravityScale;
        DetermineCharacterDirection();
    }

    private IEnumerator DashCoroutine()
    {
        // Set canDash to false to prevent dashing again until cooldown
        isDashing = true;
        canDash = false;

        // Disable gravity for the Rigidbody2D component
        rb.gravityScale = 0f;

        // Determine dash direction based on the sprite's orientation
        Vector2 dashDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

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

    public void BreakJump()
    {
        anim.ResetTrigger("Jump");
        isJumping = false;

    }

    public void Hit()
    {
        StartCoroutine(HitStun());
    }

    private IEnumerator HitStun()
    {
        actions.Player.Disable();

        float knockbackDistance = 2f;
        
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos - new Vector3(transform.localScale.x, 0, 0).normalized * knockbackDistance;

        float elapsed = 0f;

        while (elapsed < stunDuration)
        {
            float t = elapsed / stunDuration;
            // Apply Quadratic Ease-Out function to t
            float easedT = -(t * (t - 2));
            transform.position = Vector3.Lerp(startPos, endPos, easedT);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Make sure the position ends up at endPos.
        transform.position = endPos;

        actions.Player.Enable();
    }
}