using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravity = -30f;
    [SerializeField] private float riseMultiplier = 1.0f;  // affects jump-up only
    [SerializeField] private float fallMultiplier = 3.0f;  // affects falling only

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.3f;
    [SerializeField] private LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private Vector3 move;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        CheckGround();
        HandleMovement();
        HandleJump();
        ApplyGravity();
    }

    void CheckGround()
    {
        // Find what colliders are inside the ground-check sphere
        Collider[] hits = Physics.OverlapSphere(
            groundCheck.position,
            groundDistance,
            groundMask,
            QueryTriggerInteraction.Ignore
        );

        isGrounded = hits.Length > 0;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        float sprintSpeed = speed * sprintMultiplier; // calculated value
        float moveSpeed = isSprinting ? sprintSpeed : speed;

        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void ApplyGravity()
    {
        float g = gravity;

        // Going up (velocity.y > 0): usually keep normal gravity or slightly stronger
        if (velocity.y > 0f)
            g *= riseMultiplier;

        // Falling (velocity.y < 0): make it stronger for snappy fall
        if (velocity.y < 0f)
            g *= fallMultiplier;

        velocity.y += g * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}