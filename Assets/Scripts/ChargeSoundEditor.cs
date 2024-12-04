using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeSoundEditor : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private KeyCode decelerationKey;
    [SerializeField] private KeyCode chargingKey;
    [SerializeField] private float chargeInterval = 0.5f;

    private float chargeTime = 4 / 60f;
    private float secondChargeTime = -1f;
    private float speedFactor = 20f;
    private float chargeIntervalCounter = 0f;

    private float soundTimeCounter = -1f;

    // 時間伸縮用の変数
    private AudioClip originalClip;
    private float[] clipData;
    private float readPos = 0f;
    private float resampleFactor = 1.0f;

    // 再生終了フラグ
    private bool isPlaybackFinished = false;

    public enum Weapon
    {
        GrizzcoCharger,
        Bamboozler14Mk1,
        ClassicSquiffer,
        SplatCharger,
        Eliter4K,
        GooTuber,
        Snipewriter5H
    }

    void Start()
    {
        originalClip = audioSource.clip;
        int samples = originalClip.samples * originalClip.channels;
        clipData = new float[samples];
        originalClip.GetData(clipData, 0);

        // audioSource.clip を null に設定して、再生時間を無制限にする
        audioSource.clip = null;
    }

    void Update()
    {
        float currentChargeTime = Input.GetKey(decelerationKey) ? chargeTime * speedFactor : chargeTime;

        // resampleFactor を調整して、音声が指定の時間内に最後まで再生されるようにする
        resampleFactor = (float)clipData.Length / (currentChargeTime * AudioSettings.outputSampleRate * originalClip.channels);

        if (Input.GetKey(chargingKey))
        {
            if (!audioSource.isPlaying)
            {
                chargeIntervalCounter += Time.deltaTime;
                if (chargeIntervalCounter >= chargeInterval)
                {
                    chargeIntervalCounter = 0f;
                    readPos = 0f; // 読み取り位置をリセット
                    isPlaybackFinished = false; // 再生終了フラグをリセット
                    audioSource.Play();
                    soundTimeCounter = 0f;
                }
            }
        }
        else
        {
            audioSource.Stop();
            soundTimeCounter = -1f;
        }

        if (soundTimeCounter >= 0f)
        {
            soundTimeCounter += Time.deltaTime;
            if (!audioSource.isPlaying)
            {
                Debug.Log(soundTimeCounter);
                soundTimeCounter = -1f;
            }
        }

        if (Input.GetKeyUp(chargingKey))
        {
            chargeIntervalCounter = chargeInterval;
        }

        // 再生終了フラグが立っていたら、audioSource を停止
        if (isPlaybackFinished)
        {
            audioSource.Stop();
            isPlaybackFinished = false;
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        int dataLen = data.Length;
        int clipLen = clipData.Length;

        for (int i = 0; i < dataLen; i += channels)
        {
            // オリジナルクリップから読み取る位置を計算
            int p = (int)(readPos) * channels;
            if (p >= clipLen)
            {
                // クリップデータの終端に達した場合
                for (int c = 0; c < channels; c++)
                {
                    data[i + c] = 0;
                }
                // 再生終了フラグを設定
                isPlaybackFinished = true;
                break;
            }
            else
            {
                for (int c = 0; c < channels; c++)
                {
                    data[i + c] = clipData[p + c];
                }
            }

            readPos += resampleFactor;
        }
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
                chargeTime = 50f / 60f;
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
    }
}
