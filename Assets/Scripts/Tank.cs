using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tank : MonoBehaviour
{
    [SerializeField]
    private GameObject _explosionPrefab;

    private int _health = 1;

    protected TankBrain _tankBrain;
    protected TankBody _tankBody;
    protected GameManager _gm = null;
    protected Rigidbody2D _rb = null;
    protected GameObject _turret = null;

    protected bool _isStart = false;
    protected float _timeSinceStart = 0.0f;
    protected float _startTime = 3.0f;

    private bool _isAlive = true;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        if(_gm == null)
        {
            Debug.Log("GameManagerÇ™å©Ç¬Ç©ÇËÇ‹ÇπÇÒÇ≈ÇµÇΩÅB");
        }


        _rb = GetComponent<Rigidbody2D>();
        if(_rb == null)
        {
            Debug.Log("Rigidbody2DÇ™å©Ç¬Ç©ÇËÇ‹ÇπÇÒÇ≈ÇµÇΩÅB");
        }

        _turret = transform.Find("battery").gameObject;
        if(_turret == null)
        {
            Debug.Log("Turret(battery)Ç™å©Ç¬Ç©ÇËÇ‹ÇπÇÒÇ≈ÇµÇΩÅB");
        }

        //_tankBrain = new TankBrain(_tankBody);
        //_tankBody = new TankBody(_rb, _turret);
        Debug.Log("Tank Start");
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (_isStart)
        {
           
        }
        else
        {
            _timeSinceStart += Time.deltaTime;
            if(_timeSinceStart >= _startTime)
            {
                _isStart = true;
            }
            return;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damageValue)
    {
        _health -= damageValue;
        if(_health <= 0)
        {
            Die();
        }
    }

    protected void Die()
    {
        _isAlive = false;
        gameObject.SetActive(false);
        Instantiate(_explosionPrefab, transform.position, transform.rotation);
    }
}
