using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] Image fill;
    [SerializeField] Image lag;

    float targetFill = 1f;
    float lagTimer;

    void OnEnable()
    {
        if (health != null)
        {
            health.Damaged += OnHealthChanged;
            Set(health.Current, health.Max);
        }
    }

    void OnDisable()
    {
        if (health != null)
        {
            health.Damaged -= OnHealthChanged;
        }
    }

    void Start()
    {
        if (health != null)
        {
            Set(health.Current, health.Max);
        }
    }

    void Update()
    {
        if (lag != null)
        {
            if (lagTimer > 0f)
            {
                lagTimer -= Time.unscaledDeltaTime;
            }
            else if (lag.fillAmount > targetFill)
            {
                lag.fillAmount = Mathf.MoveTowards(lag.fillAmount, targetFill, 0.8f * Time.unscaledDeltaTime);
            }
        }
    }

    public void Set(int current, int max)
    {
        targetFill = Mathf.Clamp01((float)current / Mathf.Max(max, 1));
        if (fill != null) fill.fillAmount = targetFill;

        if (lag != null)
        {
            if (targetFill >= lag.fillAmount)
            {
                lag.fillAmount = targetFill;
                lagTimer = 0f;
            }
            else
            {
                lagTimer = 0.4f;
            }
        }
    }

    void OnHealthChanged(int current, int max)
    {
        Set(current, max);
    }
}
