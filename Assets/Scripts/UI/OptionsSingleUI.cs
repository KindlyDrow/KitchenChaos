using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsSingleUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _value;

    [SerializeField] private bool _selected;

    private enum SliderMean
    {
        Volume,
        Sound,
    }

    [SerializeField] private SliderMean sliderMean;


    private void OnEnable()
    {
        if (_selected) _slider.Select();
    }

    private void Start()
    {
        
        _slider.onValueChanged.AddListener(delegate { OnValueChanged(); });
        float curentVolume;
        switch (sliderMean)
        {
            case SliderMean.Volume:
                curentVolume = SoundManager.Instance.globalMusicVolume * _slider.maxValue;
                _value.text = curentVolume.ToString();
                _slider.value = curentVolume;
                break;
            case SliderMean.Sound:
                curentVolume = SoundManager.Instance.globalSoundVolume * _slider.maxValue;
                _value.text = curentVolume.ToString();
                _slider.value = curentVolume;
                break;
        }
    }

    private void OnValueChanged()
    {
        _value.text = _slider.value.ToString();
        float volume;
        volume = _slider.value / _slider.maxValue;
        switch (sliderMean)
        {
            case SliderMean.Volume:
                SoundManager.Instance.SetMusicVolume(volume);
                break;
            case SliderMean.Sound:
                SoundManager.Instance.SetSoundVolume(volume);
                break;
        }
    }
}
