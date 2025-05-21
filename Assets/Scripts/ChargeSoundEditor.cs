using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CriWare;
using TMPro;

public class ChargeSoundEditor : MonoBehaviour
{
    [SerializeField] private CriAtomSource atomSource;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip chargeSound;
    [SerializeField] private KeyCode decelerationKey;
    [SerializeField] private KeyCode chargingKey;
    [SerializeField] private TextMeshProUGUI chargeTimeText;
    [SerializeField] private Image chargeProgressImage;
    [SerializeField] private Image chargeProgressImageCenter;
    [SerializeField] private string chargeSoundName;
    [SerializeField] private AudioClip chargeCompleteSound;
    [SerializeField] private Color inkLuckingColor;

    private float chargeTime = 4 / 60f;
    private float secondChargeTime = -1f;
    private float speedFactor = 20f;
    private float currentSpeedFactor = 1f;
    private float soundTimeCounter = -1f;
    private float chargeProgress = 0f;
    private CriAtomExVoicePool voicePool;
    private bool completeSoundFlag;

    public enum Weapon
    {
        GrizzcoCharger,
        Bamboozler14Mk1,
        ClassicSquiffer,
        SplatCharger,
        GooTuber,
        Snipewriter5H,
        Eliter4K
    }

    void Start()
    {
        voicePool = new CriAtomExStandardVoicePool(1, 2, 96000, false, 100);
        voicePool.AttachDspTimeStretch();
        atomSource.player.SetVoicePoolIdentifier(voicePool.identifier);
        atomSource.player.SetDspTimeStretchRatio(1f);
        atomSource.cueName = chargeSoundName;
    }

    void Update()
    {
        if (Input.GetKeyDown(chargingKey))
        {
            if (atomSource.player.GetStatus() == CriAtomExPlayer.Status.Stop
            || atomSource.player.GetStatus() == CriAtomExPlayer.Status.PlayEnd
            || atomSource.player.GetStatus() == CriAtomExPlayer.Status.Prep)
            {
                atomSource.cueName = chargeSoundName;
                completeSoundFlag = false;
                atomSource.Play();
            }
        }

        if (Input.GetKeyUp(chargingKey))
        {
            atomSource.Stop();
            chargeProgress = 0f;
        }

        if (Input.GetKey(chargingKey))
        {
            chargeProgress += Time.deltaTime / currentSpeedFactor;
        }

        if (Input.GetKey(decelerationKey))
        {
            currentSpeedFactor = speedFactor;
            chargeProgressImage.color = inkLuckingColor;
            chargeProgressImageCenter.color = inkLuckingColor;
        }
        else
        {
            currentSpeedFactor = 1f;
            chargeProgressImage.color = Color.white;
            chargeProgressImageCenter.color = Color.white;
        }

        if (atomSource.cueName == chargeSoundName)
        {
            atomSource.player.SetDspTimeStretchRatio(chargeTime * currentSpeedFactor);
            atomSource.player.UpdateAll();
        }

        float actualChargeTime = Mathf.Clamp(chargeTime * currentSpeedFactor, 0.25f, 3f);
        chargeTimeText.text = "音声クリップの長さ : " + (actualChargeTime).ToString("F2") + "s";
        chargeProgressImage.fillAmount = chargeProgress / chargeTime;

        if (chargeProgress >= chargeTime && !completeSoundFlag)
        {
            audioSource.PlayOneShot(chargeCompleteSound);
            completeSoundFlag = true;
            Debug.Log("Charge Complete");
        }
    }

    private void OnDestroy()
    {
        voicePool.Dispose();
    }

    [EnumAction(typeof(Weapon))]
    public void RadioButtonChanged(int weapon)
    {
        Weapon enumType = (Weapon)weapon;
        switch (enumType)
        {
            case Weapon.GrizzcoCharger:
                chargeTime = 4f / 60f;
                speedFactor = 20f;
                break;
            case Weapon.Bamboozler14Mk1:
                chargeTime = 20f / 60f;
                speedFactor = 5f;
                break;
            case Weapon.ClassicSquiffer:
                chargeTime = 45f / 60f;
                speedFactor = 4f;
                break;
            case Weapon.SplatCharger:
                chargeTime = 60f / 60f;
                speedFactor = 3f;
                break;
            case Weapon.GooTuber:
                chargeTime = 71f / 60f;
                secondChargeTime = 71f / 60f;
                speedFactor = 3f;
                break;
            case Weapon.Snipewriter5H:
                chargeTime = 72f / 60f;
                speedFactor = 3f;
                break;
            case Weapon.Eliter4K:
                chargeTime = 92f / 60f;
                speedFactor = 3f;
                break;
        }
        atomSource.player.SetDspTimeStretchRatio(chargeTime);
        atomSource.player.UpdateAll();
    }
}
