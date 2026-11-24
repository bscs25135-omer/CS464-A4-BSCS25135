using UnityEngine;

public class MovingHazard : MonoBehaviour
{
    [SerializeField] Vector3 direction = Vector3.right;
    [SerializeField] float speed = 1.5f, distance = 6f;
    Rigidbody rb;
    Vector3 start;

    void Awake() { rb = GetComponent<Rigidbody>(); start = rb.position; }

    void FixedUpdate()
    {
        float x = Mathf.PingPong(Time.time * speed, distance);
        rb.MovePosition(start + direction.normalized * x);
    }
}
