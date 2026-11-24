using UnityEngine;
using UnityEngine.Audio;

public class PersistentMusic : MonoBehaviour
{
    public static PersistentMusic Instance { get; private set; }

    [SerializeField] AudioMixer mixer;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (mixer != null)
        {
            mixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(SaveData.MusicVolume, 0.0001f)) * 20f);
        }
    }
}
