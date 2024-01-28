using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _tmpComment;
    [SerializeField] Canvas _mainCanvas;
    [SerializeField] Canvas _settingsCanvas;
    [SerializeField] Slider _bgmSilder;
    [SerializeField] Slider _seSlider;
    AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        GameData._remain = 5;
        GameData._stage = 1;
        _tmpComment.text = GameData._comment;


        _bgmSilder.maxValue = 1.0f;
        _bgmSilder.value = 0.5f;
        _seSlider.maxValue = 1.0f;
        _seSlider.value = 0.5f;

        _mainCanvas.enabled = true;
        _settingsCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(GameData._remain);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        Debug.Log(GameData._seVolume);
    }

    public void OnGameStartButtonClick()
    {
        GameData._remain = 5;
        GameData._stage = 1;
        GameData._comment = "";
        SceneManager.LoadScene("Stage1");
    }

    public void OnSettingsButtonClick()
    {
        _mainCanvas.enabled = false;
        _settingsCanvas.enabled = true;
    }

    public void OnBackButtonClick()
    {
        _mainCanvas.enabled = true;
        _settingsCanvas.enabled = false;
    }

    public void SetVolume()
    {
        GameData._bgmVolume = _bgmSilder.value;
        GameData._seVolume = _seSlider.value;
    }
}

public static class GameData
{
    public static int _remain = 5;
    public static int _stage = 1;
    public static string _comment = "";
    public static float _seVolume = 0.5f;
    public static float _bgmVolume = 0.5f;
}