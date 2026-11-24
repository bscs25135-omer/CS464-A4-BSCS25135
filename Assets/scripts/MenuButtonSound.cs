using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MenuButtonSound : MonoBehaviour
{
    [SerializeField] AudioClip clip;

    void Awake()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(PlaySound);
        }
    }

    public void PlaySound()
    {
        if (SfxPlayer.Instance != null && clip != null)
        {
            SfxPlayer.Instance.Play(clip);
        }
    }
}
