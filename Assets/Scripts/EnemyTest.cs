using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    public GameObject _player = null;
    protected float _rayMaxDirection = 5f;
    GameObject _turret;
    private float _angleToPlayer_deg;
    private float _turretRotationVeo_deg = 90;

    private bool _isAimingDetermined = false;
    private bool _isAiming = false;
    private float _batteryLength = 1f;
    private float _angleToAimingDetermined_deg;

    private float _bulletLockTime = 0.5f;
    private float _timeSinceLastShot = 0f;
    private bool _isBulletLock = false;

    public GameObject _bulletPrefab;
    private enum EmemyState
    {
        Watching,
        Attacking,
        Tracking,
        Escaping,
        Dead
    }

    private EmemyState _ememyState;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player");
        if (_player == null)
        {
            Debug.Log("Playerが見つかりませんでした。");
        }

        _turret = transform.Find("battery").gameObject;
        if (_turret == null)
        {
            Debug.Log("Turret(battery)が見つかりませんでした。");
        }

        _ememyState = EmemyState.Watching;
    }

    // Update is called once per frame
    void Update()
    {
        if (_ememyState == EmemyState.Watching)
        {
            Watch();
        }else if(_ememyState == EmemyState.Attacking)
        {
            Attack();
        }

        Debug.Log(_ememyState);
    }

    public void Watch()
    {
        if (hasFoundPlayer())
        {
            _ememyState = EmemyState.Attacking;
        }
        else
        {
            //Debug.Log("turret before" + _turret.transform.rotation.eulerAngles.z);
            float rot = _turret.transform.rotation.eulerAngles.z + 60f * Time.deltaTime;
            //rot = ClampAngle(rot);
            //Debug.Log("turret:" + rot);
            _turret.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, rot));
            //_battery.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, newAngleZ));
        }
    }

    public void Attack()
    {
        if (_isBulletLock)
        {
            _timeSinceLastShot += Time.deltaTime;
            if(_timeSinceLastShot >= _bulletLockTime)
            {
                _isBulletLock = false;
            }
            else
            {
                return;
            }
        }

        if (_isAimingDetermined)
        {
            float deltaAngle = _angleToAimingDetermined_deg - _turret.transform.eulerAngles.z;
            deltaAngle = ClampAngle(deltaAngle);

            float newAngle;

            if(Mathf.Abs(deltaAngle) <= _turretRotationVeo_deg * Time.deltaTime)
            {
                newAngle = _angleToAimingDetermined_deg;
                _isAiming = true;
            }
            else
            {
                newAngle = _turret.transform.rotation.eulerAngles.z + Mathf.Sign(deltaAngle) * _turretRotationVeo_deg * Time.deltaTime;

            }

            _turret.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, newAngle));
        }
        else
        {
            if (hasFoundPlayer())
            {
                int randomNum = Random.Range(1, 100);
                if (randomNum <= 20)
                {
                    _isAimingDetermined = true;
                    _angleToAimingDetermined_deg = _angleToPlayer_deg;
                }

                float deltaAngle = _angleToPlayer_deg - _turret.transform.eulerAngles.z;
                deltaAngle = ClampAngle(deltaAngle);

                float newAngle;

                if (Mathf.Abs(deltaAngle) <= _turretRotationVeo_deg * Time.deltaTime)
                {
                    newAngle = _angleToAimingDetermined_deg;
                    _isAiming = true;
                }
                else
                {
                    newAngle = _turret.transform.rotation.eulerAngles.z + Mathf.Sign(deltaAngle) * _turretRotationVeo_deg * Time.deltaTime;

                }

                _turret.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, newAngle));

            }
            else
            {
                _ememyState = EmemyState.Watching;
                return;
            }
        }

        if (_isAiming)
        {
            Shoot();
        }

    }

    public bool hasFoundPlayer()
    {
        for(int i=-5; i<=5; i++)
        {
            Vector2 rayDir = _turret.transform.right;
            float angle_deg = i*5;
            Quaternion rotaionMatrix = Quaternion.Euler(0f, 0f, angle_deg);
            rayDir = rotaionMatrix * rayDir;
            Ray2D ray = new Ray2D(_turret.transform.position, rayDir);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, _rayMaxDirection, 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Obstacle"));
           
            Debug.DrawRay(_turret.transform.position, rayDir * _rayMaxDirection, Color.cyan, 0.01f, false);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    _angleToPlayer_deg = Mathf.Atan2(rayDir.y, rayDir.x) * Mathf.Rad2Deg;
                    return true;
                }
            }
        }
        

        return false;
    }

    private float ClampAngle(float angle)
    {
        while (angle < 0.0f)
        {
            angle += 360.0f;
        }

        angle %= 360;

        if (angle > 180.0f)
        {
            angle -= 360.0f;
        }
        /*
        angle %= 360; // 角度を360度以内に収める
        if (angle > 180) // -180度から180度に変換
            angle -= 360;
        */
        return angle;
    }

    public void Shoot()
    {
        Instantiate(_bulletPrefab, _turret.transform.position + _batteryLength * new Vector3(Mathf.Cos(_turret.transform.rotation.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(_turret.transform.rotation.eulerAngles.z * Mathf.Deg2Rad), 0.0f), _turret.transform.rotation);

        _isAimingDetermined = false;
        _isAiming = false;

        _isBulletLock = true;
        _timeSinceLastShot = 0f;
    }
}
