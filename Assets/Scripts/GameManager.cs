using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    TextMeshProUGUI _tmpRemain = null;
    TextMeshProUGUI _tmpStage = null;

    [SerializeField] int _stageNum = 5;

    int _remain;
    int _stage;

    GameObject _player = null;
    Player _playerPlayer = null;

    [SerializeField] List<GameObject> _enemies;
    [SerializeField] GameObject[] allGameObjects;
    [SerializeField] float _afterglowTime = 1.5f;
    [SerializeField] float _startSCSETime = 0.8f;
    [SerializeField] AudioClip _reflectSE;
    [SerializeField] float _reflectSEGain = 0.6f;
    [SerializeField] AudioClip _destroySE;
    [SerializeField] float _destroySEGain = 0.0f;
    [SerializeField] AudioClip _explosionSE;
    [SerializeField] float _explosionSEGain = 1.0f;
    [SerializeField] AudioClip _stageClearSE;
    [SerializeField] float _stageClearSEGain = 0.0f;
    [SerializeField] AudioClip _failureSE;
    [SerializeField] float _failureSEGain = 0.8f;
    [SerializeField] AudioClip _shotSE;
    [SerializeField] float _shotSEGain = 1.0f;
    [SerializeField] GameObject _bulletUI;
    GameObject[] _bulletUIs;

    bool _isStageClear = false;
    bool _isFailure = false;
    bool _isRingSCSE = false;
    bool _isRingFSE = false;
    float _stageClearTimer = 0.0f;
    float _failureTimer = 0.0f;
    AudioSource _audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _remain = GameData._remain;
        _stage = GameData._stage;

        _tmpRemain =  GameObject.Find("Remain").GetComponent<TextMeshProUGUI>();
        _tmpStage =  GameObject.Find("Stage").GetComponent<TextMeshProUGUI>();

        if(_tmpRemain == null)
        {
            Debug.Log("Remain UIが見つかりませんでした。");
        }

        if(_tmpStage == null)
        {
            Debug.Log("Stage UIが見つかりませんでした。");
        }

        _tmpRemain.text = "REMAIN:" + _remain.ToString();
        _tmpStage.text = "STAGE:" + _stage.ToString();

        int tankLayerMask = LayerMask.GetMask("Tank");
        int playerLayerMask = LayerMask.GetMask("Player");

        // Tankレイヤーに属するすべてのGameObjectを取得
        allGameObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach(var go in allGameObjects)
        {
            if(go.layer == LayerMask.NameToLayer("Player"))
            {
                _player = go;
                _playerPlayer = _player.GetComponent<Player>();
            }
            if(go.layer == LayerMask.NameToLayer("Tank"))
            {
                _enemies.Add(go);
            }
        }

        _bulletUIs = new GameObject[_playerPlayer._maxBulletsOnScreen];
        for(int i=0; i<_playerPlayer._maxBulletsOnScreen; i++)
        {
            _bulletUIs[i] = Instantiate(_bulletUI, new Vector3(-10f + 1.0f * i, -5.7f, 0.0f), Quaternion.EulerAngles(Vector3.zero));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(GameData._remain);
        _remain = GameData._remain;
        _stage = GameData._stage;
        //Debug.Log("remain:" + _remain);
        _tmpRemain.text = "REMAIN:" + _remain.ToString();
        _tmpStage.text = "STAGE:" + _stage.ToString();
        //Debug.Log("player:" + _player.activeSelf);
        if (_isStageClear)
        {
            _stageClearTimer += Time.deltaTime;
            if (!_isRingSCSE)
            {
                StageClearSE();
                _isRingSCSE = true;
            }
            if(_stageClearTimer >= _afterglowTime)
            {
                ClearStage();
            }
        }
        if (_isFailure)
        {
            _failureTimer += Time.deltaTime;
            if (!_isRingFSE)
            {
                FailureSE();
                _isRingFSE = true;
            }
            if(_failureTimer >= _afterglowTime)
            {
                Retry();
            }
        }
        if (!_player.activeSelf)
        {
            //Retry();
            _isFailure = true;
        }
        else
        {
            bool isDefeatAllEnemies = true;
            foreach(var e in _enemies)
            {
                if (e.activeSelf)
                {
                    isDefeatAllEnemies = false;
                }
            }
            if (isDefeatAllEnemies)
            {
                _isStageClear = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        ChangeBulletUI();
    }

    public void Retry()
    {
        _isStageClear = false;
        _isRingSCSE = false;
        _stageClearTimer = 0.0f;
        _failureTimer = 0.0f;
        if(_remain > 1)
        {
            GameData._remain -= 1;
            SceneManager.LoadScene("Stage" + _stage.ToString());
        }
        else
        {
            Debug.Log("GameOver");
            GameData._comment = "Game Over! You Have Been Defeated at Stage" + _stage.ToString() + ".";
            SceneManager.LoadScene("Title");
        }
    }

    public void ClearStage()
    {
        if(_stageNum > _stage)
        {

            GameData._stage += 1;
            SceneManager.LoadScene("Stage" + (_stage + 1).ToString());
        }
        else
        {
            ClearGame();
        }
    }

    void ClearGame()
    {
        Debug.Log("Clear!");
        GameData._comment = "Congratulations! You Have Cleared This Game!";
        SceneManager.LoadScene("Title");
    }

    public void ReflectSE()
    {
        _audioSource.volume = GameData._seVolume * _reflectSEGain;
        _audioSource.PlayOneShot(_reflectSE);
    }

    public void DestroySE()
    {
        _audioSource.volume = GameData._seVolume * _destroySEGain;
        Debug.Log("destroySe:" + _audioSource.volume);
        _audioSource.PlayOneShot(_destroySE);
    }
    
    public void ExplosionSE()
    {
        _audioSource.volume = GameData._seVolume * _explosionSEGain;
        _audioSource.PlayOneShot(_explosionSE);
    }

    public void FailureSE()
    {
        _audioSource.volume = GameData._seVolume * _failureSEGain;
        _audioSource.PlayOneShot(_failureSE);
    }

    public void StageClearSE()
    {
        _audioSource.volume = GameData._seVolume * _stageClearSEGain;
        _audioSource.PlayOneShot(_stageClearSE);
    }

    public void ShotSE()
    {
        _audioSource.volume = GameData._seVolume * _shotSEGain;
        _audioSource.PlayOneShot(_shotSE);
    }

    private void ChangeBulletUI()
    {
        for(int i=0; i<_playerPlayer._maxBulletsOnScreen; i++)
        {
            _bulletUIs[i].SetActive(false);
        }

        for(int i=0; i<_playerPlayer._maxBulletsOnScreen - _playerPlayer._bulletNum; i++)
        {
            _bulletUIs[i].SetActive(true);
        }
    }
}
