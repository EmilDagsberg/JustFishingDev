using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public float speed = 3f;
    public float turnSpeed = 2f;
    public float changeDirectionTime = 3f;
    public float swimRadius = 1f; // small area around starting position

    private float timer;
    private Vector3 targetDirection;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position; // center of small swim area
        timer = changeDirectionTime;
        SetNewDirection();
    }

    void Update()
    {
        MoveForward();
        HandleDirectionChange();
        KeepWithinSmallArea();
    }

    void MoveForward()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(targetDirection),
            turnSpeed * Time.deltaTime
        );
    }

    void HandleDirectionChange()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SetNewDirection();
            timer = Random.Range(2f, changeDirectionTime);
        }
    }

    void SetNewDirection()
    {
        targetDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-0.5f, 0.5f),
            Random.Range(-1f, 1f)
        ).normalized;
    }

    void KeepWithinSmallArea()
    {
        Vector3 offset = transform.position - startPosition;

        // If outside the small radius, steer back towards the center
        if (offset.magnitude > swimRadius)
        {
            targetDirection = (-offset).normalized;
        }
    }
}