using System;
using UnityEngine;
using UnityEngine.UI;

public class SliderAudio : MonoBehaviour
{

    [Header("Sound")]
    [SerializeField] private Slider Slider;

    private void Start()
    {
        if (AudioManager.Instance.MainMusicSource != null) Slider.value = AudioManager.Instance.MainMusicSource.volume;
        Slider.onValueChanged.AddListener(delegate {UpdateAudioManager ();});
    }

    private void Update()
    {
        if (AudioManager.Instance.MainMusicSource != null) Slider.value = AudioManager.Instance.MainMusicSource.volume;
    }

    private void UpdateAudioManager()
    {
        if (AudioManager.Instance.MainMusicSource != null) AudioManager.Instance.MainMusicSource.volume = Slider.value;
    }
}
