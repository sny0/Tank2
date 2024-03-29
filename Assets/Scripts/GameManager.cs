using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private TextMeshProUGUI _tmpRemain = null;
    private TextMeshProUGUI _tmpStage = null;

    private int _remain;
    private int _stage;

    private GameObject _player = null;
    private Player _playerPlayer = null;

    [SerializeField]
    private List<GameObject> _enemies;
    
    [SerializeField] 
    private GameObject[] allGameObjects;
    
    [SerializeField]
    private float _afterglowTime = 1.5f;
    
    [SerializeField]
    private float _startSCSETime = 0.8f;
   
    [SerializeField]
    private GameObject _bulletUI;

    [SerializeField]
    private GameObject _superBulletUI;
    
    private GameObject[] _bulletUIs;

    private TextMeshProUGUI _tmpBulletNum= null;

    private bool _isStageClear = false;
    private bool _isFailure = false;
    private bool _isRingSCSE = false;
    private bool _isRingFSE = false;
    private float _stageClearTimer = 0.0f;
    private float _failureTimer = 0.0f;
    
    private AudioManager _audioManager = null;
    
    public enum Scene
    {
        Title,
        MainGame
    }

    // Start is called before the first frame update
    private void Start()
    {
        _remain = GameData._remain;
        _stage = GameData._stage;

        _tmpRemain =  GameObject.Find("Remain").GetComponent<TextMeshProUGUI>();
        _tmpStage =  GameObject.Find("Stage").GetComponent<TextMeshProUGUI>();
        _tmpBulletNum = GameObject.Find("BulletNum").GetComponent<TextMeshProUGUI>();

        if(_tmpRemain == null)
        {
            Debug.Log("Remain UIが見つかりませんでした。");
        }

        if(_tmpStage == null)
        {
            Debug.Log("Stage UIが見つかりませんでした。");
        }

        if(_tmpBulletNum == null)
        {
            Debug.Log("Bullet Num が見つかりませんでした。");
        }

        _tmpRemain.text = "REMAIN:" + _remain.ToString();
        _tmpStage.text = "STAGE:" + _stage.ToString();

        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        if(_audioManager == null)
        {
            Debug.Log("AudioManagerが見つかりませんでした。");
        }

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

        _bulletUIs = new GameObject[5];
        GameObject bulletPrefab;
        if (GameData._isNormalBullet)
        {
            bulletPrefab = _bulletUI;
        }
        else
        {
            bulletPrefab = _superBulletUI;
        }

        for (int i = 0; i < 5; i++)
        {
            _bulletUIs[i] = Instantiate(bulletPrefab, new Vector3(0f + 1.5f * i, -5.7f, 0.0f), Quaternion.EulerAngles(Vector3.zero));
        }

        if (!GameData._isExtra)
        {
            _audioManager.PlayBGM(GameManager.Scene.MainGame, TitleManager.Mode.Normal);
        }
        else
        {
            _audioManager.PlayBGM(GameManager.Scene.MainGame, TitleManager.Mode.Extra);
        }
    }

    // Update is called once per frame
    private void Update()
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
            if (!GameData._isExtra)
            {
                SceneManager.LoadScene("Stage" + _stage.ToString());
            }
            else
            {
                SceneManager.LoadScene("ExtraStage" + _stage.ToString());
            }
        }
        else
        {
            Debug.Log("GameOver");
            GameData._comment = "Game Over! You Have Been Defeated at Stage" + _stage.ToString() + ".";
            SceneManager.LoadScene("Result");
        }
    }

    public void ClearStage()
    {
        if (!GameData._isExtra)
        {
            if (GameData._maxStage > _stage)
            {

                GameData._stage += 1;
                SceneManager.LoadScene("Stage" + (_stage + 1).ToString());
            }
            else
            {
                ClearGame();
            }
        }
        else
        {
            if(GameData._maxExtraStage > _stage)
            {
                GameData._stage += 1;
                SceneManager.LoadScene("ExtraStage" + (_stage + 1).ToString());
            }
            else
            {
                ClearGame();
            }
        }
    }

    private void ClearGame()
    {
        Debug.Log("Clear!");
        GameData._comment = "Congratulations! You Have Cleared This Game!";
        GameData._isClear = true;
        SceneManager.LoadScene("Result");
    }

    public void ReflectSE()
    {
        _audioManager.ReflectSE();
    }

    public void DestroySE()
    {
        _audioManager.DestroySE();
    }
    
    public void ExplosionSE()
    {
        _audioManager.ExplosionSE();
    }

    public void FailureSE()
    {
        _audioManager.FailureSE();
    }

    public void StageClearSE()
    {
        _audioManager.StageClearSE();
    }

    public void ShotSE()
    {
        _audioManager.ShotSE();
    }

    public void FindSE()
    {
        _audioManager.FindSE();
    }

    private void ChangeBulletUI()
    {
        for(int i=0; i<5; i++)
        {
            _bulletUIs[i].SetActive(false);
            _tmpBulletNum.text = "";
        }

        int remainBullet = GameData._maxBulletRemain - _playerPlayer.BulletNum;

        if (remainBullet > 5)
        {
            _bulletUIs[0].SetActive(true);
            _tmpBulletNum.text = "X " + remainBullet.ToString();
        }
        else
        {
            for (int i = 0; i < remainBullet; i++)
            {
                _bulletUIs[i].SetActive(true);
            }
        }
    }
}
