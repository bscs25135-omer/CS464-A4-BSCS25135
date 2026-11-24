using UnityEngine;

// Part A force: ForceMode.VelocityChange = one kick, mass ignored,
// applied once from OnTriggerEnter, so every ball launches the same.
public class BouncePad : MonoBehaviour
{
    [SerializeField] float bounceSpeed = 11f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Rigidbody body = other.attachedRigidbody;
        if (body == null) return;
        Vector3 v = body.linearVelocity;
        body.linearVelocity = new Vector3(v.x, 0f, v.z);
        body.AddForce(Vector3.up * bounceSpeed, ForceMode.VelocityChange);
    }
}
