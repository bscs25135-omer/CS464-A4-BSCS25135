using UnityEngine;

public class HitSound : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] AudioClip clip;

    int lastHealth = -1;

    void OnEnable()
    {
        if (health != null)
        {
            lastHealth = health.Current;
            health.Damaged += OnDamaged;
        }
    }

    void OnDisable()
    {
        if (health != null)
        {
            health.Damaged -= OnDamaged;
        }
    }

    void OnDamaged(int current, int max)
    {
        if (lastHealth >= 0 && current < lastHealth)
        {
            if (SfxPlayer.Instance != null && clip != null)
            {
                SfxPlayer.Instance.Play(clip);
            }
        }
        lastHealth = current;
    }
}
