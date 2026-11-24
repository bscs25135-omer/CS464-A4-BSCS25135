using UnityEngine;

public class Health : MonoBehaviour
{
    public event System.Action<int, int> Damaged;

    [SerializeField] int max = 3;
    [SerializeField] float iFrames = 1f;
    int current;
    float invulnerableUntil;

    public int Current => current;
    public int Max => max;

    void Awake() => current = max;

    void Start()
    {
        Damaged?.Invoke(current, max);
    }

    public bool TakeDamage(int amount)
    {
        if (current <= 0 || Time.time < invulnerableUntil) return false;   // i-frames
        invulnerableUntil = Time.time + iFrames;
        current -= amount;
        Damaged?.Invoke(current, max);
        if (GameManager.Instance != null) GameManager.Instance.UpdateUI();
        if (current <= 0 && GameManager.Instance != null) GameManager.Instance.PlayerDied();               // <= 0, never == 0
        return true;
    }

    public void Restore(float protection)
    {
        current = max;
        invulnerableUntil = Time.time + protection;
        Damaged?.Invoke(current, max);
        if (GameManager.Instance != null) GameManager.Instance.UpdateUI();
    }
}
