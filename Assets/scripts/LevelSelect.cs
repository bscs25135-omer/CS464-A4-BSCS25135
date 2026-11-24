using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] Button[] buttons;
    [SerializeField] TMP_Text[] labels;
    [SerializeField] LevelLoader loader;

    void Start()
    {
        if (loader == null) loader = FindFirstObjectByType<LevelLoader>();

        int count = Mathf.Min(buttons != null ? buttons.Length : 0, labels != null ? labels.Length : 0);
        for (int i = 0; i < count; i++)
        {
            int n = i + 1;
            bool unlocked = SaveData.IsUnlocked(n);
            if (buttons[i] != null)
            {
                buttons[i].interactable = unlocked;
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() =>
                {
                    if (loader != null) loader.Load(n);
                });
            }

            if (labels[i] != null)
            {
                if (!unlocked)
                {
                    labels[i].text = "Locked";
                }
                else
                {
                    string txt = "Level " + n;
                    if (SaveData.HasBest(n))
                    {
                        txt += "\n" + SaveData.GetBest(n).ToString("0.0") + " s";
                    }
                    labels[i].text = txt;
                }
            }
        }
    }
}
