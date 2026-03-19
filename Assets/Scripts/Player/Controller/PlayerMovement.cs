using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float jumpHeight = 1.2f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -30f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    private CharacterController controller;

    private Vector3 velocity;
    private Vector3 moveInput;

    private bool isGrounded;

    public Vector3 CurrentMovement { get; private set; }

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        CheckGround();
        HandleInput();
        HandleJump();
        ApplyGravity();
        MovePlayer();
    }
    
    void CheckGround()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }
    }
    void HandleInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        moveInput = (transform.right * x + transform.forward * z).normalized;
        CurrentMovement = moveInput;
    }
    
    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    
    void ApplyGravity()
    {
        if (velocity.y < 0f)
        {
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        }
        else if (velocity.y > 0f && !Input.GetButton("Jump"))
        {
            velocity.y += gravity * lowJumpMultiplier * Time.deltaTime;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }
    
    void MovePlayer()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting ? walkSpeed * sprintMultiplier : walkSpeed;

        Vector3 finalMove = moveInput * currentSpeed;
        finalMove.y = velocity.y;

        controller.Move(finalMove * Time.deltaTime);
    }
}