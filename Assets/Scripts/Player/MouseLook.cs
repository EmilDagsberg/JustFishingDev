using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Range(50f, 400f)]
    [SerializeField] private float mouseSensitivity = 150f;

    [SerializeField] private Transform controller;

    private float xRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Pitch (camera up/down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Yaw (player left/right)
        if (controller != null)
            controller.Rotate(Vector3.up * mouseX);
    }
}