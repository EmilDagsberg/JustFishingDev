using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public float speed = 3f;
    public float turnSpeed = 2f;
    public float changeDirectionTime = 3f;

    public Vector3 swimAreaCenter;
    public Vector3 swimAreaSize = new Vector3(20f, 10f, 20f);

    private float timer;
    private Vector3 targetDirection;

    void Start()
    {
        timer = changeDirectionTime;
        SetNewDirection();
    }

    void Update()
    {
        MoveForward();
        HandleDirectionChange();
        KeepInsideBounds();
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

    void KeepInsideBounds()
    {
        Vector3 min = swimAreaCenter - swimAreaSize / 2;
        Vector3 max = swimAreaCenter + swimAreaSize / 2;

        if (transform.position.x < min.x || transform.position.x > max.x ||
            transform.position.y < min.y || transform.position.y > max.y ||
            transform.position.z < min.z || transform.position.z > max.z)
        {
            Vector3 directionToCenter = (swimAreaCenter - transform.position).normalized;
            targetDirection = directionToCenter;
        }
    }

    
}