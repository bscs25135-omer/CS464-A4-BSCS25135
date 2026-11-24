using UnityEngine;
using UnityEngine.EventSystems;

public class GameOver : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] GameObject panel;
    [SerializeField] GameObject firstButton;
    [SerializeField] AudioClip loseClip;

    bool triggered;

    void OnEnable()
    {
        triggered = false;
        if (health != null)
        {
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
        if (current <= 0 && !triggered)
        {
            if (GameManager.Instance != null && GameManager.Instance.Lives > 1)
            {
                return;
            }
            TriggerGameOver();
        }
    }

    public void TriggerGameOver()
    {
        if (triggered) return;
        triggered = true;
        Time.timeScale = 0f;

        if (SfxPlayer.Instance != null && loseClip != null)
        {
            SfxPlayer.Instance.Play(loseClip);
        }

        if (panel != null)
        {
            panel.SetActive(true);
        }

        if (EventSystem.current != null && firstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(firstButton);
        }
    }
}
