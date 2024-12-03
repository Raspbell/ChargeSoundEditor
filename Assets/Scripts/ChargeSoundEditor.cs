using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeSoundEditor : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Slider soundSpeedSlider;

    void Update()
    {
        audioSource.pitch = soundSpeedSlider.value;
    }
}
