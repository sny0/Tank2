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
    private float _bgmGain = 1f;


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

    private void Start()
    {
        UpdateBGM();
    }

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

    public void UpdateBGM()
    {
        _audioSourceBGM.volume = GameData._bgmVolume * _bgmGain;
    }
}
