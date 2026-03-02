using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 2;
    [SerializeField] private float mouseSensitivity = 2;

    [Header("Interaction")]
    [SerializeField] private float interactRange = 2;
    [SerializeField] private LayerMask interactableLayer;

    private CharacterController characterController;
    private Camera playerCamera;
    private float verticalRotation = 0;
    private bool canMove = true;

    public Vector3 movement;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (canMove)
        {
            HandleMovement();
            HandleMouseLook();
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        movement = transform.right * horizontal + transform.forward * vertical;
        characterController.Move(movement * movementSpeed * Time.deltaTime);
        characterController.Move(Vector3.down * 9.81f * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90, 90);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }
}