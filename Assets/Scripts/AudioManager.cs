using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSourceBGM;

    [SerializeField]
    private AudioSource _audioSourceSE;

    [SerializeField]
    private AudioClip _normalTitleBGM;

    [SerializeField]
    private AudioClip _extraTitleBGM;

    [SerializeField]
    private AudioClip _normalGameBGM;

    [SerializeField]
    private AudioClip _extraGameBGM;

    [SerializeField]
    private float _normalTitleBgmGain = 1f;

    [SerializeField]
    private float _extraTitleBgmGain = 1f;

    [SerializeField]
    private float _normalGameBgmGain = 1f;

    [SerializeField]
    private float _extraGameBgmGain = 1f;

    [SerializeField]
    private AudioClip _reflectSE;

    [SerializeField]
    private float _reflectSEGain = 0.6f;

    [SerializeField]
    private AudioClip _destroySE;

    [SerializeField]
    private float _destroySEGain = 0.0f;

    [SerializeField]
    private AudioClip _explosionSE;

    [SerializeField]
    private float _explosionSEGain = 1.0f;

    [SerializeField]
    private AudioClip _stageClearSE;

    [SerializeField]
    private float _stageClearSEGain = 0.0f;

    [SerializeField]
    private AudioClip _failureSE;

    [SerializeField]
    private float _failureSEGain = 0.8f;

    [SerializeField]
    private AudioClip _shotSE;

    [SerializeField]
    private float _shotSEGain = 1.0f;

    [SerializeField]
    private AudioClip _findSE;

    [SerializeField]
    private float _findSEGain = 1.0f;


    public void ReflectSE()
    {
        _audioSourceSE.volume = GameData._seVolume * _reflectSEGain;
        _audioSourceSE.PlayOneShot(_reflectSE);
    }

    public void DestroySE()
    {
        _audioSourceSE.volume = GameData._seVolume * _destroySEGain;
        //Debug.Log("destroySe:" + _audioSource.volume);
        _audioSourceSE.PlayOneShot(_destroySE);
    }

    public void ExplosionSE()
    {
        _audioSourceSE.volume = GameData._seVolume * _explosionSEGain;
        _audioSourceSE.PlayOneShot(_explosionSE);
    }

    public void FailureSE()
    {
        _audioSourceSE.volume = GameData._seVolume * _failureSEGain;
        _audioSourceSE.PlayOneShot(_failureSE);
    }

    public void StageClearSE()
    {
        _audioSourceSE.volume = GameData._seVolume * _stageClearSEGain;
        _audioSourceSE.PlayOneShot(_stageClearSE);
    }

    public void ShotSE()
    {
        _audioSourceSE.volume = GameData._seVolume * _shotSEGain;
        _audioSourceSE.PlayOneShot(_shotSE);
    }

    public void FindSE()
    {
        _audioSourceSE.volume = GameData._seVolume * _findSEGain;
        _audioSourceSE.PlayOneShot(_findSE);
    }

    public void UpdateBGM(GameManager.Scene scene, TitleManager.Mode mode)
    {
        switch (scene)
        {
            case GameManager.Scene.Title:
                switch (mode)
                {
                    case TitleManager.Mode.Normal:
                        _audioSourceBGM.volume = GameData._bgmVolume * _normalTitleBgmGain;
                        break;

                    case TitleManager.Mode.Extra:
                        _audioSourceBGM.volume = GameData._bgmVolume * _extraTitleBgmGain;
                        break;
                }
                break;

            case GameManager.Scene.MainGame:
                switch (mode)
                {
                    case TitleManager.Mode.Normal:
                        _audioSourceBGM.volume = GameData._bgmVolume * _normalGameBgmGain;
                        break;

                    case TitleManager.Mode.Extra:
                        _audioSourceBGM.volume = GameData._bgmVolume * _extraGameBgmGain;
                        break;
                }
                break;
        }
    }

    public void StopBGM()
    {
        _audioSourceBGM.Stop();
    }

    public void PlayBGM(GameManager.Scene scene, TitleManager.Mode mode)
    {
        switch (scene)
        {
            case GameManager.Scene.Title:
                switch (mode)
                {
                    case TitleManager.Mode.Normal:
                        _audioSourceBGM.clip = _normalTitleBGM;
                        _audioSourceBGM.volume = GameData._bgmVolume * _normalTitleBgmGain;
                        break;
                    case TitleManager.Mode.Extra:
                        _audioSourceBGM.clip = _extraTitleBGM;
                        _audioSourceBGM.volume = GameData._bgmVolume * _extraTitleBgmGain;
                        break;
                }
                break;

            case GameManager.Scene.MainGame:
                switch (mode)
                {
                    case TitleManager.Mode.Normal:
                        _audioSourceBGM.clip = _normalGameBGM;
                        _audioSourceBGM.volume = GameData._bgmVolume * _normalGameBgmGain;
                        break;
                    case TitleManager.Mode.Extra:
                        _audioSourceBGM.clip = _extraGameBGM;
                        _audioSourceBGM.volume = GameData._bgmVolume * _extraGameBgmGain;
                        break;
                }
                break;
        }
        _audioSourceBGM.Play();
    }
}
