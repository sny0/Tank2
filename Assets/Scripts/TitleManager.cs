using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _tmpComment;
    
    [SerializeField]
    private Canvas _mainCanvas;
    
    [SerializeField]
    private Canvas _settingsCanvas;

    [SerializeField]
    private Canvas _howToPlayCanvas;

    [SerializeField]
    private Slider _bgmSilder;
    
    [SerializeField]
    private Slider _seSlider;

    [SerializeField]
    private AudioManager _audioManager;

    [SerializeField]
    private float _silderSELockTime = 1f;

    private float _timer = 0f;


    // Start is called before the first frame update
    private void Start()
    {
        GameData._remain = 5;
        GameData._stage = 1;
        _tmpComment.text = GameData._comment;


        _bgmSilder.maxValue = 1.0f;
        _bgmSilder.value = GameData._bgmVolume;
        _seSlider.maxValue = 1.0f;
        _seSlider.value = GameData._seVolume;

        _mainCanvas.enabled = true;
        _settingsCanvas.enabled = false;
        _howToPlayCanvas.enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }

        _timer += Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            _audioManager.ShotSE();
        }

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
        _howToPlayCanvas.enabled = false;
    }

    public void OnHowToPlayButtonClick()
    {
        _mainCanvas.enabled = false;
        _settingsCanvas.enabled = false;
        _howToPlayCanvas.enabled = true;
    }

    public void OnBackButtonClick()
    {
        _mainCanvas.enabled = true;
        _settingsCanvas.enabled = false;
        _howToPlayCanvas.enabled = false;
    }


    public void UpdateBGM()
    {
        GameData._bgmVolume = _bgmSilder.value;
        _audioManager.UpdateBGM();
    }

    public void UpdateSE()
    {
        GameData._seVolume = _seSlider.value;
        
        if(_timer >= _silderSELockTime)
        {
            _audioManager.ShotSE();
            _timer = 0f;
        }
    }

    public void Quit()
    {
         Application.Quit();
    }
}

public static class GameData
{
    public static int _remain = 5;
    public static int _stage = 1;
    public static string _comment = "";
    public static float _seVolume = 0.5f;
    public static float _bgmVolume = 0.5f;
    public static bool _isClear = false;
}
