using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain : TankBrain
{
    protected GameObject _player = null;

    [SerializeField]
    protected float _rayMaxDirection = 5f;

    [SerializeField]
    protected float _angleToWatch;
    
    [SerializeField]
    protected int _rayNum = 5;

    public float SonarSearchRadius => _sonarSearchRadius;

    [SerializeField]
    protected float _sonarSearchRadius = 3f;

    protected float _timeToLoseSight;

    protected float _angleToPlayer_scope;
    protected float _angleToPlayer_sonar;

    protected bool _isAimingDetermined = false;
    protected bool _isAiming = false;
    protected float _angleToDeterminedAiming_deg;

    [SerializeField]
    protected float _bulletLockTime = 3f;
    protected float _timeSinceLastShot = 0f;
    protected bool _isBulletLock = false;

    [SerializeField]
    protected int _shootProbability = 20;

    protected bool[] _reflectOffWallsExpectation = { false, false, false, false }; // 四方の壁の反射を利用して自機を攻撃できるか index=左,上,右,下

    protected float[] _turretToWallRot = new float[4];
    protected Vector2[,] _wallsPos = new Vector2[4, 2];

    [SerializeField]
    private GameObject _bikkuriPrefab;

    private bool _isBikkuriActive = false;

    [SerializeField]
    private Vector3 _bikkuriOffset;

    private SpriteRenderer _sonarSpriteRenderer = null;

    [SerializeField]
    private Material _searchingMaterial;

    [SerializeField]
    private Material _attackingMaterial;

    protected enum DetectedSensor
    {
        None,
        Scope,
        Sonar
    }

    protected DetectedSensor _detectedSensor;

    protected enum EnemyState
    {
        Searching,
        Attacking
    }

    protected EnemyState _enemyState;

    [SerializeField]
    private float _timeUntilLossOfSight = 2f;

    private float _lossOfSightTimer = 0f;


    private bool _isFindPlayer = false;

    private float _findPlayerLockTime = 1f;

    private float _findPlayerTimer = 0f;

    private bool _canFindPlayer = true;

    protected override void Start()
    {
        _player = GameObject.Find("Player");
        if(_player == null)
        {
            Debug.Log("Playerが見つかりませんでした。");
        }

        _sonarSpriteRenderer = transform.Find("sonarEffect").gameObject.GetComponent<SpriteRenderer>();

        base.Start();
        _enemyState = EnemyState.Searching;
        _detectedSensor = DetectedSensor.None;

        _wallsPos[0, 0] = new Vector2(-10, 0);
        _wallsPos[0, 1] = new Vector2(-10, 1);
        _wallsPos[1, 0] = new Vector2(0, 4);
        _wallsPos[1, 1] = new Vector2(1, 4);
        _wallsPos[2, 0] = new Vector2(10, 0);
        _wallsPos[2, 1] = new Vector2(10, 1);
        _wallsPos[3, 0] = new Vector2(0, -4);
        _wallsPos[3, 1] = new Vector2(1, -4);
    }

    public override void Think()
    {
        UpdateState();
        UpdateEffect();
        base.Think();
    }

    protected virtual void UpdateState()
    {
        switch (_enemyState)
        {
            case EnemyState.Searching:
                Search();
                break;

            case EnemyState.Attacking:
                Attack();
                break;
        }
    }

    public void UpdateEffect()
    {
        switch (_enemyState)
        {
            case EnemyState.Searching:
                _sonarSpriteRenderer.material = _searchingMaterial;
                break;

            case EnemyState.Attacking:
                _sonarSpriteRenderer.material = _attackingMaterial;
                break;
        }
    }


    protected void Search()
    {
        if (IsPlayerInVision())
        {
            _enemyState = EnemyState.Attacking;
            _detectedSensor = DetectedSensor.Scope;
            FoundPlayer();
        }
        else if (IsPlayerWithinSearchRange())
        {
            _enemyState = EnemyState.Attacking;
            _detectedSensor = DetectedSensor.Sonar;
            FoundPlayer();
        }

    }
    protected void Attack()
    {
        if (IsPlayerInVision())
        {
            _detectedSensor = DetectedSensor.Scope;
            _lossOfSightTimer = 0f;
        }
        else if (IsPlayerWithinSearchRange())
        {
            _detectedSensor = DetectedSensor.Sonar;
            _lossOfSightTimer = 0f;
        }
        else
        {
            _lossOfSightTimer += Time.deltaTime;

            if(_lossOfSightTimer >= _timeUntilLossOfSight)
            {
                _enemyState = EnemyState.Searching;
                _detectedSensor = DetectedSensor.None;
            }
        }
    }


    protected override void ThinkNextMove()
    {
        switch (_enemyState)
        {
            case EnemyState.Searching:
                ThinkNextMove_Searching();
                break;

            case EnemyState.Attacking:
                ThinkNextMove_Attacking();
                break;
        }
    }

    protected virtual void ThinkNextMove_Searching()
    {

    }

    protected virtual void ThinkNextMove_Attacking()
    {

    }


    protected override void ThinkTurretRot()
    {
        switch (_enemyState)
        {
            case EnemyState.Searching:
                ThinkTurretRot_Searching();
                break;

            case EnemyState.Attacking:
                ThinkTurretRot_Attacking();
                break;
        }
    }

    protected virtual void ThinkTurretRot_Searching()
    {

    }

    protected void ThinkTurretRot_Attacking()
    {
        switch (_detectedSensor)
        {
            case DetectedSensor.Scope:
                ThinkTurretRot_Attacking_Scope();
                break;

            case DetectedSensor.Sonar:
                ThinkTurretRot_Attacking_Sonar();
                break;
        }
    }

    protected virtual void ThinkTurretRot_Attacking_Scope()
    {

    }

    protected virtual void ThinkTurretRot_Attacking_Sonar()
    {

    }


    protected override void ThinkToShoot()
    {
        switch (_enemyState)
        {
            case EnemyState.Searching:
                _isShoot = false;
                break;

            case EnemyState.Attacking:
                ThinkToShoot_Attacking();
                break;
        }
    }

    protected virtual void ThinkToShoot_Attacking()
    {
        _isShoot = false;
        if (_isBulletLock)
        {
            _timeSinceLastShot += Time.deltaTime;
            if (_timeSinceLastShot >= _bulletLockTime)
            {
                _isBulletLock = false;
            }
            else
            {
                return;
            }
        }
        else
        {
            if (_isAiming)
            {
                _isAiming = false;
                _isShoot = true;
                _isBulletLock = true;
                _timeSinceLastShot = 0f;
                return;
            }
        }


        if (!_isAimingDetermined)
        {
            int randomNum = Random.RandomRange(1, 100);
            if (randomNum <= _shootProbability)
            {
                _isAimingDetermined = true;

                switch (_detectedSensor)
                {
                    case DetectedSensor.Scope:
                        ThinkToShoot_Attacking_Scope();
                        break;

                    case DetectedSensor.Sonar:
                        ThinkToShoot_Attacking_Sonar();
                        break;
                }
            }
        }
    }

    protected virtual void ThinkToShoot_Attacking_Scope()
    {
        _angleToDeterminedAiming_deg = _angleToPlayer_scope;
    }

    protected virtual void ThinkToShoot_Attacking_Sonar()
    {
        if (IsPathClearBetweenPlayerAndSelf())
        {
            _angleToDeterminedAiming_deg = _angleToPlayer_sonar;
        }
        else
        {
            canAimToPlayerUsingReflectionWalls();
            List<int> exceptedWallIndent = new List<int>();
            for(int i=0; i<4; i++)
            {
                if (_reflectOffWallsExpectation[i])
                {
                    exceptedWallIndent.Add(i);
                }
            }

            if(exceptedWallIndent.Count == 0)
            {
                _isAimingDetermined = false;
            }
            else
            {
                int random = Random.Range(0, exceptedWallIndent.Count);
                _angleToDeterminedAiming_deg = _turretToWallRot[exceptedWallIndent[random]];
            }
        }
    }
    

    protected bool IsPlayerInVision()
    {
        for(int i=0; i<_rayNum; i++)
        {
            Vector2 rayDir = _turret.transform.right;
            float angle_deg = i * _angleToWatch / (_rayNum - 1) - _angleToWatch / 2;
            Quaternion rotaionMatrix = Quaternion.Euler(0f, 0f, angle_deg);
            rayDir = rotaionMatrix * rayDir;
            Ray2D ray = new Ray2D(_turret.transform.position, rayDir);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, _rayMaxDirection, 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Obstacle"));

            Debug.DrawRay(_turret.transform.position, rayDir * _rayMaxDirection, Color.cyan, 0.01f, false);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    _angleToPlayer_scope = Mathf.Atan2(rayDir.y, rayDir.x) * Mathf.Rad2Deg;
                    return true;
                }
            }
        }
        return false;
    }

    protected bool IsPlayerWithinSearchRange()
    {
        Vector2 toPlayer = _player.transform.position - transform.position;
        float distanceToPlayer = Mathf.Abs(toPlayer.magnitude);

        if (distanceToPlayer <= _sonarSearchRadius)
        {
            _angleToPlayer_sonar = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
            return true;
        }

        return false;
    }

    protected bool IsPathClearBetweenPlayerAndSelf()
    {
        Vector2 rayDir = _player.transform.position - transform.position;
        Ray2D ray = new Ray2D(_turret.transform.position, rayDir);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, rayDir.magnitude, 1 << LayerMask.NameToLayer("Obstacle") | 1 << LayerMask.NameToLayer("Tank") | 1 << LayerMask.NameToLayer("Player"));

        Debug.DrawRay(_turret.transform.position, rayDir, Color.green, 0.1f, false);

        if (hit.collider != null)
        {
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                return true;
            }
        }

        return false;
    }

    protected void canAimToPlayerUsingReflectionWalls()
    {
        for(int i=0; i<4; i++)
        {
            _reflectOffWallsExpectation[i] = false;
        }

        for(int i=0; i<4; i++)
        {
            Vector2 reflectedPos = ReflectPoint(_wallsPos[i, 0], _wallsPos[i, 1], new Vector2(_player.transform.position.x, _player.transform.position.y));
            float angleToWall = Mathf.Atan2(reflectedPos.y - _turret.transform.position.y, reflectedPos.x - _turret.transform.position.x) * Mathf.Rad2Deg;
            _turretToWallRot[i] = angleToWall;

            Vector2 rayDir0 = new Vector2(reflectedPos.x - _turret.transform.position.x, reflectedPos.y - _turret.transform.position.y);

            RaycastHit2D hit0 = Physics2D.Raycast(new Vector2(_turret.transform.position.x, _turret.transform.position.y) + rayDir0.normalized, rayDir0.normalized, float.MaxValue, 1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("Tank") | 1 << LayerMask.NameToLayer("Obstacle"));
            Debug.DrawRay(transform.position, rayDir0 * 100, Color.blue, 5f, false);
            
            if(hit0.collider != null)
            {
                if(hit0.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                {
                    Vector2 rayPos1 = hit0.point + hit0.normal * 0.1f;
                    Vector2 rayDir1 = Vector2.Reflect(reflectedPos - new Vector2(transform.position.x, transform.position.y), hit0.normal).normalized;

                    RaycastHit2D hit1 = Physics2D.Raycast(rayPos1, rayDir1, float.MaxValue, 1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("Tank") | 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Obstacle"));
                    Debug.DrawRay(rayPos1, rayDir1 * 100, Color.yellow, 5f, false);
                    if (hit1.collider != null)
                    {
                        if (hit1.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                        {
                            _reflectOffWallsExpectation[i] = true;
                        }
                    }
                }
            }
        }
    }


    public float ClampAngle(float angle)
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

        return angle;
    }

    protected Vector2 ReflectPoint(Vector2 linePoint0, Vector2 linePoint1, Vector2 point)
    {
        Vector2 lineNormal = new Vector2(linePoint1.y - linePoint0.y, linePoint0.x - linePoint1.x);

        Vector2 lineToPoint = new Vector2(point.x - linePoint0.x, point.y - linePoint0.y);

        float dotProduct = Vector2.Dot(lineNormal, lineToPoint);

        Vector2 reflectedPoint = point - 2 * dotProduct * lineNormal / lineNormal.sqrMagnitude;

        return reflectedPoint;
    }

    private void FoundPlayer()
    {
        Instantiate(_bikkuriPrefab, transform.position + _bikkuriOffset, Quaternion.identity);
        _isFindPlayer = true;
        _findPlayerTimer = 0f;
        _canFindPlayer = false;
    }

    private void Update()
    {
        if (!_canFindPlayer)
        {
            _findPlayerTimer += Time.deltaTime;
            
            if(_findPlayerTimer >= _findPlayerLockTime)
            {
                _canFindPlayer = true;
            }
        }
    }
}
