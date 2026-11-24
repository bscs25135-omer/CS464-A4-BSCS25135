using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveForce = 10f;
    [SerializeField] ForceMode mode = ForceMode.Force;
    [SerializeField] float fallDeathY = -5f;

    Rigidbody rb;
    Transform cam;
    Vector3 input;
    float speedMultiplier = 1f;
    float noControlUntil;
    Coroutine boost;
    bool isFalling;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (Camera.main != null) cam = Camera.main.transform;
    }

    public void OnRespawn()
    {
        isFalling = false;
        noControlUntil = 0f;
        input = Vector3.zero;
    }

    void Update()
    {
        if (PauseMenu.IsPaused) return;

        if (!isFalling && transform.position.y < fallDeathY)
        {
            isFalling = true;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerDied();
            }
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Camera-relative movement
        if (cam == null && Camera.main != null) cam = Camera.main.transform;
        Vector3 forward = cam != null ? cam.forward : Vector3.forward; forward.y = 0f;
        Vector3 right = cam != null ? cam.right : Vector3.right; right.y = 0f;
        if (forward.sqrMagnitude < 0.01f && cam != null) { forward = cam.up; forward.y = 0f; }
        forward.Normalize();
        right.Normalize();

        input = Vector3.ClampMagnitude(forward * v + right * h, 1f);
    }

    void FixedUpdate()
    {
        // steering waits while a knockback is playing out
        if (Time.time >= noControlUntil && rb != null)
            rb.AddForce(input * moveForce * speedMultiplier, mode);
    }

    // A push that steering can't wipe (called by Hazard when a hit lands)
    public void Knockback(Vector3 push, float seconds)
    {
        if (rb == null) return;
        Vector3 v = rb.linearVelocity;
        rb.linearVelocity = new Vector3(0f, v.y, 0f);
        rb.AddForce(push, ForceMode.Impulse);
        noControlUntil = Time.time + seconds;
    }

    // Timed power-up: the timer runs HERE, so it outlives the pickup.
    // Rule for a second pickup: REFRESH (stop the running timer, start a new one).
    public void BoostSpeed(float multiplier, float seconds)
    {
        if (boost != null) StopCoroutine(boost);
        boost = StartCoroutine(Boost(multiplier, seconds));
    }

    IEnumerator Boost(float multiplier, float seconds)
    {
        speedMultiplier = multiplier;
        yield return new WaitForSeconds(seconds);
        speedMultiplier = 1f;
        boost = null;
    }
}
