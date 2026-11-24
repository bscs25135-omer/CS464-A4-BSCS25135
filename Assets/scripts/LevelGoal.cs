using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class LevelGoal : MonoBehaviour
{
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject nextButton;
    [SerializeField] TMP_Text timeText;
    [SerializeField] AudioClip winClip;

    bool reached;

    void OnTriggerEnter(Collider other)
    {
        if (reached || !other.CompareTag("Player")) return;
        TriggerWin();
    }

    public void TriggerWin()
    {
        if (reached) return;
        reached = true;

        int level = gameObject.scene.buildIndex;
        float t = Time.timeSinceLevelLoad;
        bool best = SaveData.Record(level, t);
        SaveData.Unlock(level + 1);

        if (SfxPlayer.Instance != null && winClip != null)
        {
            SfxPlayer.Instance.Play(winClip);
        }

        if (timeText != null)
        {
            timeText.text = t.ToString("0.0") + " s" + (best ? " NEW BEST" : "");
        }

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        if (nextButton != null)
        {
            // If on the 5th level (buildIndex 5) or last level, we can hide or keep next button
            // If next button is active, select it
            if (EventSystem.current != null && nextButton.activeInHierarchy)
            {
                EventSystem.current.SetSelectedGameObject(nextButton);
            }
        }

        Time.timeScale = 0f;
    }
}
