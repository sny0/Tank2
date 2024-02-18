using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject _bulletPrefab; // 発射する弾
    
    [SerializeField]
    private GameObject _superBulletPrefab; 
    
    [SerializeField]
    private float _speed = 5.0f; // 機体の移動スピード

    private Rigidbody2D _rb;
    private GameObject _battery; // 砲台
    
    [SerializeField]
    private float _bulletLockTime = 1.0f;
    
    [SerializeField]
    private float _batteryLength = 0.5f; // 砲台の長さ
    private float _lastShotTime = 0.0f;
    private float _time = 0.0f;
    private int _bulletNum = 0;

    public int BulletNum => _bulletNum;

    private bool _isPaused = false;
    private bool _startPaused = false;
    private bool _isAlive = true;

    [SerializeField]
    private GameObject _explosion;
    
    GameManager _gm = null;
    
    [SerializeField]
    private GameObject _cursol;
    
    [SerializeField]
    private GameObject _circle;
    
    GameObject[] _circles;
    
    [SerializeField]
    private int _circleNum = 5;
    
    [SerializeField]
    private GameObject _youUIPre;
    
    private GameObject _youUI;

 
    private void Start()
    {
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gm == null)
        {
            Debug.Log("GameManagerが見つかりませんでした。");
        }
        _rb = GetComponent<Rigidbody2D>();
        _battery = transform.Find("battery").gameObject;
        _startPaused = true;
        _circles = new GameObject[_circleNum];
        for(int i=0; i<_circleNum; i++)
        {
            _circles[i] = Instantiate(_circle);
        }
        Instantiate(_cursol, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.Euler(Vector3.zero));

        _youUI = Instantiate(_youUIPre, transform.position + new Vector3(0.0f, 1.5f, -2.0f), Quaternion.Euler(Vector3.zero));
    }


    private void FixedUpdate()
    {
        // マウスのカーソル位置をスクリーン座標からワールド座標に変換
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0.0f);
        // オブジェクトの位置からマウスの位置へのベクトルを計算
        Vector3 lookDirection = mousePosition - transform.position;
        for (int i = 0; i < _circleNum; i++)
        {
            Vector3 circlePos = transform.position + lookDirection * (i + 1) / (_circleNum + 1);
            _circles[i].transform.position = circlePos;
        }

        //Debug.Log(mousePosition);
        // ベクトルを角度に変換してオブジェクトを回転
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        _battery.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if (_startPaused)
        {
            _startPaused = false;
            _time = 0.0f;
            _isPaused = true;
            return;
        }else if (_isPaused)
        {
            _time += Time.deltaTime;
            if(_time >= 3.0f)
            {
                _isPaused = false;
                _youUI.SetActive(false);
            }
            else
            {
                return;
            }
        }

        Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

    }

    private void Update()
    {
        if (_isPaused) return;
        _lastShotTime += Time.deltaTime;
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot(){
        if(_lastShotTime >= _bulletLockTime && _bulletNum < GameData._maxBulletRemain)
        {
            Debug.Log("Shot!!");

            GameObject bullet;

            if (GameData._isNormalBullet)
            {
                bullet = Instantiate(_bulletPrefab, transform.position + _batteryLength * new Vector3(Mathf.Cos(_battery.transform.rotation.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(_battery.transform.rotation.eulerAngles.z * Mathf.Deg2Rad), 0.0f), _battery.transform.rotation);
            }
            else
            {
                bullet = Instantiate(_superBulletPrefab, transform.position + _batteryLength * new Vector3(Mathf.Cos(_battery.transform.rotation.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(_battery.transform.rotation.eulerAngles.z * Mathf.Deg2Rad), 0.0f), _battery.transform.rotation);
            }

            _bulletNum++;
            _lastShotTime = 0.0f;
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if(bulletScript != null)
            {
                bulletScript.SetParent(gameObject);
            }
        }
    }

    void Move(float x, float y){
        //Vector3 moveDirection = new Vector3(x, y, 0);
        Vector2 moveDirection = new Vector3(x, y);
        //transform.position += moveDirection * speed * Time.deltaTime;
        _rb.velocity = moveDirection * _speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Die();
            Debug.Log("Damaged!");
        }
    }

    public void Die()
    {
        _isAlive = false;
        gameObject.SetActive(false);
        
        for(int i=0; i<_circleNum; i++)
        {
            _circles[i].SetActive(false);
        }
        
        Instantiate(_explosion, transform.position, transform.rotation);
    }

    public void ReduceBullet()
    {
        _bulletNum--;
    }

    public bool isAlive()
    {
        return _isAlive;
    }
}
