using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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
    private Canvas _rankingCanvas;

    [SerializeField]
    private AudioManager _audioManager;

    [SerializeField]
    private float _silderSELockTime = 1f;

    private float _seTimer = 0f;

    [SerializeField]
    private TextMeshProUGUI[] _rankingScores = new TextMeshProUGUI[5];

    [SerializeField]
    private GameObject _deletePanel;

    [SerializeField]
    private Canvas _extraCanvas;

    [SerializeField]
    private Canvas _extraRankingCanvas;

    [SerializeField]
    private TextMeshProUGUI[] _extraRankingScores = new TextMeshProUGUI[5];

    private Camera _camera;

    [SerializeField]
    private Button _extraQuitButton;

    [SerializeField]
    private Button _extraRankingButton;

    [SerializeField]
    private Button _extraStartButton;

    [SerializeField]
    private float _notPressButtonTimeInExtra = 6f;

    private float _extraTimer = 0f;

    [SerializeField]
    private Toggle[] _bulletTypeToggles = new Toggle[2];

    public Toggle[] _maxRemainToggles = new Toggle[3];

    [SerializeField]
    private int[] _maxRemainToggleValue = new int[] { 5, 10, 20 };

    [SerializeField]
    private Toggle[] _maxBulletRemainToggles = new Toggle[3];

    [SerializeField]
    private int[] _maxBulletRemainValue = new int[] { 5, 8, 99 };
    public enum Mode
    {
        Normal,
        Extra
    }

    public Mode GameMode => _mode;

    private Mode _mode = Mode.Normal;

    private float _modeChangeTimer = 0f;
    private const float _modeChangeLockTime = 2f;

    // Start is called before the first frame update
    private void Start()
    {
        _camera = Camera.main;
        if(_camera == null)
        {
            Debug.Log("Main Camera‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ‚Å‚µ‚½B");
        }

        _camera.backgroundColor = new Color(0.58f, 0.75f, 0.82f);

        GameData._remain = GameData._maxRemain;
        GameData._stage = 1;
        _tmpComment.text = GameData._comment;


        _bgmSilder.maxValue = 1.0f;
        _bgmSilder.value = GameData._bgmVolume;
        _seSlider.maxValue = 1.0f;
        _seSlider.value = GameData._seVolume;

        UpdateRanking();

        _mainCanvas.enabled = true;
        _settingsCanvas.enabled = false;
        _howToPlayCanvas.enabled = false;
        _rankingCanvas.enabled = false;
        _deletePanel.SetActive(false);
        _extraCanvas.enabled = false;
        _extraRankingCanvas.enabled = false;

        _mode = Mode.Normal;
        GameData._isExtra = false;
        _audioManager.PlayBGM(GameManager.Scene.Title, _mode);

        InitializeToggle();

        UpdateBulletTypeToggle();
        UpdateMaxRemainToggle();
        UpdateMaxBulletRemainToggle();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }

        _seTimer += Time.deltaTime;
        _modeChangeTimer += Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            _audioManager.ShotSE();
        }

        if(_mode == Mode.Extra)
        {
            _extraTimer += Time.deltaTime;
            if(_extraTimer >= _notPressButtonTimeInExtra)
            {
                _extraStartButton.interactable = true;
                _extraQuitButton.interactable = true;
                _extraRankingButton.interactable = true;
            }
        }

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.N) && Input.GetKey(KeyCode.Y) && Input.GetKey(KeyCode.Alpha0))
        {
            if (_modeChangeTimer >= _modeChangeLockTime)
            {

                switch (_mode)
                {
                    case Mode.Normal:
                        _camera.backgroundColor = new Color(0.05f, 0.05f, 0.05f);
                        _mainCanvas.enabled = false;
                        _settingsCanvas.enabled = false;
                        _howToPlayCanvas.enabled = false;
                        _rankingCanvas.enabled = false;
                        _deletePanel.SetActive(false);
                        _extraCanvas.enabled = true;
                        _extraRankingCanvas.enabled = false;
                        _mode = Mode.Extra;
                        GameData._isExtra = true;

                        _extraStartButton.interactable = false;
                        _extraQuitButton.interactable = false;
                        _extraRankingButton.interactable = false;

                        break;

                    case Mode.Extra:
                        _camera.backgroundColor = new Color(0.58f, 0.75f, 0.82f);
                        _mainCanvas.enabled = true;
                        _settingsCanvas.enabled = false;
                        _howToPlayCanvas.enabled = false;
                        _rankingCanvas.enabled = false;
                        _deletePanel.SetActive(false);
                        _extraCanvas.enabled = false;
                        _extraRankingCanvas.enabled = false;
                        _mode = Mode.Normal;
                        GameData._isExtra = false;
                        break;
                }

                _audioManager.PlayBGM(GameManager.Scene.Title, _mode);
                _modeChangeTimer = 0f;
            }
        }

    }

    public void OnGameStartButtonClick()
    {
        GameData._remain = GameData._maxRemain;
        GameData._stage = 1;
        GameData._comment = "";
        if (!GameData._isExtra)
        {
            SceneManager.LoadScene("Stage1");
        }
        else
        {
            SceneManager.LoadScene("ExtraStage1");
        }
    }

    public void OnSettingsButtonClick()
    {
        _mainCanvas.enabled = false;
        _settingsCanvas.enabled = true;
        _howToPlayCanvas.enabled = false;
        _rankingCanvas.enabled = false;
        _extraRankingCanvas.enabled = false;
    }

    public void OnHowToPlayButtonClick()
    {
        _mainCanvas.enabled = false;
        _settingsCanvas.enabled = false;
        _howToPlayCanvas.enabled = true;
        _rankingCanvas.enabled = false;
        _extraRankingCanvas.enabled = false;
    }

    public void OnRankingButtonClick()
    {
        switch (_mode)
        {
            case Mode.Normal:
                _mainCanvas.enabled = false;
                _settingsCanvas.enabled = false;
                _howToPlayCanvas.enabled = false;
                _rankingCanvas.enabled = true;
                _extraCanvas.enabled = false;
                _extraRankingCanvas.enabled = false;
                break;
            case Mode.Extra:
                _mainCanvas.enabled = false;
                _settingsCanvas.enabled = false;
                _howToPlayCanvas.enabled = false;
                _rankingCanvas.enabled = false;
                _extraCanvas.enabled = false;
                _extraRankingCanvas.enabled = true;
                break;
        }
    }

    public void OnBackButtonClick()
    {
        switch (_mode)
        {
            case Mode.Normal:
                _mainCanvas.enabled = true;
                _settingsCanvas.enabled = false;
                _howToPlayCanvas.enabled = false;
                _rankingCanvas.enabled = false;
                _deletePanel.SetActive(false);
                _extraCanvas.enabled = false;
                _extraRankingCanvas.enabled = false;
                break;
            case Mode.Extra:
                _mainCanvas.enabled = false;
                _settingsCanvas.enabled = false;
                _howToPlayCanvas.enabled = false;
                _rankingCanvas.enabled = false;
                _deletePanel.SetActive(false);
                _extraCanvas.enabled = true;
                _extraRankingCanvas.enabled = false;
                break;
        }
    }

    public void OnDeleteDataButtonClick()
    {
        _deletePanel.SetActive(true);
    }

    public void OnDeletePanelOkButtonClick()
    {
        DeleteData();
        UpdateRanking();
        _deletePanel.SetActive(false);
    }

    public void OnDeletePanelNoButtonClick()
    {
        _deletePanel.SetActive(false);
    }

    public void UpdateBGM()
    {
        GameData._bgmVolume = _bgmSilder.value;
        _audioManager.UpdateBGM(GameManager.Scene.Title, _mode);
    }

    public void UpdateSE()
    {
        GameData._seVolume = _seSlider.value;
        
        if(_seTimer >= _silderSELockTime)
        {
            _audioManager.ShotSE();
            _seTimer = 0f;
        }
    }

    public void DeleteData()
    {
        PlayerPrefs.DeleteAll();
    }

    public void Quit()
    {
         Application.Quit();
    }

    public void UpdateRanking()
    {
        for (int i = 0; i < 5; i++)
        {
            int score = PlayerPrefs.GetInt(i.ToString() + "Score", GameData._NoRecordNumber);
            if (score == GameData._clearNumber)
            {
                _rankingScores[i].text = "CLEAR!";
            }
            else if (score == GameData._NoRecordNumber)
            {
                _rankingScores[i].text = "---";
            }
            else
            {
                _rankingScores[i].text = "Reached Stage " + score.ToString();
            }
        }

        for (int i = 0; i < 5; i++)
        {
            int score = PlayerPrefs.GetInt(i.ToString() + "Extra", GameData._NoRecordNumber);
            if (score == GameData._clearNumber)
            {
                _extraRankingScores[i].text = "CLEAR!!";
            }
            else if (score == GameData._NoRecordNumber)
            {
                _extraRankingScores[i].text = "---";
            }
            else
            {
                _extraRankingScores[i].text = "Reached Stage " + score.ToString();
            }
        }

    }

    public void UpdateBulletTypeToggle()
    {
        if (_bulletTypeToggles[0].isOn)
        {
            GameData._isNormalBullet = true;
            GameData._selectedBulletTypeToggle = 0;
        }
        else
        {
            GameData._isNormalBullet = false;
            GameData._selectedBulletTypeToggle = 1;
        }
    }

    public void UpdateMaxRemainToggle()
    {
        for(int i=0; i<3; i++)
        {
            if (_maxRemainToggles[i].isOn)
            {
                GameData._maxRemain = _maxRemainToggleValue[i];
                GameData._selectedMaxRemainToggle = i;
                break;
            }
        }
    }

    public void UpdateMaxBulletRemainToggle()
    {
        for(int i=0; i<3; i++)
        {
            if (_maxBulletRemainToggles[i].isOn)
            {
                GameData._maxBulletRemain = _maxBulletRemainValue[i];
                GameData._selectedMaxBulletRemainToggle = i;
                break;
            }
        }
    }

    public void InitializeToggle()
    {
        _bulletTypeToggles[GameData._selectedBulletTypeToggle].isOn = true;
        _maxRemainToggles[GameData._selectedMaxRemainToggle].isOn = true;
        _maxBulletRemainToggles[GameData._selectedMaxBulletRemainToggle].isOn = true;
    }
}

public static class GameData
{
    public static int _remain = 10;
    public static int _maxRemain = 10;
    public static int _stage = 1;
    public static int _maxStage = 10;
    public static int _maxExtraStage = 10;
    public static string _comment = "";
    public static float _seVolume = 0.5f;
    public static float _bgmVolume = 0.5f;
    public static bool _isClear = false;
    public static int _clearNumber = 101;
    public static int _NoRecordNumber = -1;
    public static bool _isExtra = false;
    public static bool _isNormalBullet = true;
    public static int _maxBulletRemain = 5;
    public static int _selectedBulletTypeToggle = 0;
    public static int _selectedMaxRemainToggle = 0;
    public static int _selectedMaxBulletRemainToggle = 0;
}
