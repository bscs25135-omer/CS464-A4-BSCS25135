using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] int creditsIndex = 6;
    [SerializeField] GameObject loadingPanel;
    [SerializeField] Image bar;

    bool loading;

    public void LoadNext()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        Load(currentIndex >= creditsIndex ? 0 : currentIndex + 1);
    }

    public void Reload()
    {
        Load(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMenu()
    {
        Load(0);
    }

    public void LoadCredits()
    {
        Load(creditsIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Load(int index)
    {
        if (loading) return;
        loading = true;
        Time.timeScale = 1f;
        StartCoroutine(Run(index));
    }

    IEnumerator Run(int index)
    {
        if (loadingPanel != null) loadingPanel.SetActive(true);
        var op = SceneManager.LoadSceneAsync(index);
        while (!op.isDone)
        {
            if (bar != null) bar.fillAmount = Mathf.Clamp01(op.progress / 0.9f);
            yield return null;
        }
    }
}
