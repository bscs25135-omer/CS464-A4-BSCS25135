using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider slider;

    void Start()
    {
        if (slider == null) slider = GetComponentInChildren<Slider>(true);
        if (slider != null)
        {
            slider.minValue = 0.0001f;
            slider.maxValue = 1f;
            float vol = SaveData.MusicVolume;
            slider.SetValueWithoutNotify(vol);
            Apply(vol);
            slider.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    void OnSliderChanged(float val)
    {
        Apply(val);
        SaveData.MusicVolume = val;
    }

    void Apply(float v)
    {
        if (mixer != null)
        {
            mixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(v, 0.0001f)) * 20f);
        }
    }

    void OnDestroy()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(OnSliderChanged);
        }
    }
}
