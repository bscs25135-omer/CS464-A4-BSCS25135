using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] GameObject fxPrefab;
    [SerializeField] AudioClip clip;

    bool collected;

    void OnTriggerEnter(Collider other)
    {
        if (collected || !other.CompareTag("Player")) return;
        Collect();
    }

    public void Collect()
    {
        if (collected) return;
        collected = true;

        if (fxPrefab != null)
        {
            Instantiate(fxPrefab, transform.position, Quaternion.identity);
        }

        if (SfxPlayer.Instance != null && clip != null)
        {
            SfxPlayer.Instance.Play(clip);
        }

        Destroy(gameObject);
    }
}
