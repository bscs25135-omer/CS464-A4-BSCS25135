using UnityEngine;

public class HitSparks : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] ParticleSystem sparks;

    int lastHealth = -1;

    void Awake()
    {
        if (sparks == null) sparks = GetComponent<ParticleSystem>();
    }

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
            if (sparks != null)
            {
                sparks.Play();
            }
        }
        lastHealth = current;
    }
}
