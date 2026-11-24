using UnityEngine;
using UnityEngine.Audio;

public class SfxPlayer : MonoBehaviour
{
    public static SfxPlayer Instance { get; private set; }

    [SerializeField] AudioSource[] sources;
    [SerializeField] AudioMixerGroup sfxGroup;

    int next;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (sources == null || sources.Length == 0)
        {
            sources = new AudioSource[4];
            for (int i = 0; i < 4; i++)
            {
                AudioSource src = gameObject.AddComponent<AudioSource>();
                src.playOnAwake = false;
                src.outputAudioMixerGroup = sfxGroup;
                sources[i] = src;
            }
        }

        for (int i = 0; i < sources.Length; i++)
        {
            if (sources[i] != null)
            {
                sources[i].ignoreListenerPause = true;
                if (sources[i].outputAudioMixerGroup == null && sfxGroup != null)
                {
                    sources[i].outputAudioMixerGroup = sfxGroup;
                }
            }
        }
    }

    public void Play(AudioClip c)
    {
        if (c == null || sources == null || sources.Length == 0) return;
        AudioSource s = sources[next];
        next = (next + 1) % sources.Length;
        if (s != null)
        {
            s.pitch = Mathf.Pow(2f, Random.Range(-2f, 2f) / 12f);
            s.PlayOneShot(c);
        }
    }
}
