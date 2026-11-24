using UnityEngine;

// Timed speed pickup. The pickup only calls BoostSpeed; the timer runs on the player,
// so it outlives Destroy(gameObject).
public class PowerUp : MonoBehaviour
{
    [SerializeField] float multiplier = 1.6f;
    [SerializeField] float duration = 5f;
    [SerializeField] GameObject fxPrefab;
    [SerializeField] AudioClip clip;
    bool collected;

    void OnTriggerEnter(Collider other)
    {
        if (collected || !other.CompareTag("Player")) return;
        collected = true;

        if (fxPrefab != null)
        {
            Instantiate(fxPrefab, transform.position, Quaternion.identity);
        }

        if (SfxPlayer.Instance != null && clip != null)
        {
            SfxPlayer.Instance.Play(clip);
        }

        if (other.TryGetComponent(out PlayerController p)) p.BoostSpeed(multiplier, duration);
        Destroy(gameObject);
    }
}
