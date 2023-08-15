using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    public float speed = 5f;

    private InputActions actions;
    private Rigidbody2D rb;
    // private Animator anim;
    private SpriteRenderer sr;
    private Vector2 moveInput;

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
    }

    private void Movement()
    {
        float moveX = moveInput.x * speed;
        rb.velocity = new Vector2(moveX, rb.velocity.y);
    }

    // public void PlayMoveSound()
    // {
    //     audioSource.clip = walkSound;
    //     audioSource.time = 1.88f;
    //     audioSource.Play();
    // }
}