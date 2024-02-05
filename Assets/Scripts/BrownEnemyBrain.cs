using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrownEnemyBrain : EnemyBrain
{
    [SerializeField]
    float _turretRotationVelo_Searching_deg;

    [SerializeField]
    bool _isClockwise = false;

    [SerializeField]
    float _turretRotationVelo_Attacking_deg;

    private MapManager _mm = null;

    [SerializeField]
    private int _randomMoveProbability = 5; // ランダムな方向へ移動する確率（未実装）

    [SerializeField]
    private float _continuousMovementTime = 0.2f;

    private float _movementTimer = 0f;

    private bool _canThinkNextMovement = true;

    private Vector2 _destinationPos;

    [SerializeField]
    private float _movementSpeed_Searching = 1f;

    [SerializeField]
    private float _movementSpeed_Attacking = 1.5f;

    protected override void Start()
    {
        base.Start();
        _mm = GameObject.Find("MapManager").GetComponent<MapManager>();
        if(_mm == null)
        {
            Debug.Log("MapManagerが見つかりませんでした。");
        }
    }

    protected override void ThinkNextMove_Attacking()
    {
        if (!_canThinkNextMovement)
        {

        }

        if (_canThinkNextMovement)
        {
            _destinationPos = LookAtMap();
            _canThinkNextMovement = false;

            _movementTimer = 0f;

            Vector2 hereToDestination = new Vector2(_destinationPos.x - transform.position.x, _destinationPos.y - transform.position.y);
            _continuousMovementTime = hereToDestination.magnitude / _movementSpeed_Attacking;
            _moveVec = hereToDestination.normalized * _movementSpeed_Attacking;
        }
        else
        {
            _movementTimer += Time.deltaTime;
            if (_continuousMovementTime <= _movementTimer)
            {
                _canThinkNextMovement = true;
            }
        }
    }

    protected override void ThinkNextMove_Searching()
    {
        if (!_canThinkNextMovement)
        {

        }

        if (_canThinkNextMovement)
        {
            _destinationPos = LookAtMap();
            _canThinkNextMovement = false;

            _movementTimer = 0f;

            Vector2 hereToDestination = new Vector2(_destinationPos.x - transform.position.x, _destinationPos.y - transform.position.y);
            _continuousMovementTime = hereToDestination.magnitude / _movementSpeed_Searching;

            _moveVec = hereToDestination.normalized * _movementSpeed_Searching;
        }
        else
        {
            _movementTimer += Time.deltaTime;
            if(_continuousMovementTime <= _movementTimer)
            {
                _canThinkNextMovement = true;
            }
        }


        /*
        if (hereToDestination.magnitude > _movementSpeed * Time.deltaTime)
        {
            _moveVec = hereToDestination.normalized * _movementSpeed;
        }
        else
        {
            _moveVec = hereToDestination;
            _canThinkNextMovement = true;
        }
        */
    }

    protected override void ThinkTurretRot_Searching()
    {
        float nextTurretRot_deg = _turret.transform.rotation.eulerAngles.z + _turretRotationVelo_Searching_deg * Time.deltaTime;
        nextTurretRot_deg = ClampAngle(nextTurretRot_deg);

        _turretRot_deg = nextTurretRot_deg;
    }

    protected override void ThinkTurretRot_Attacking_Scope()
    {
        float nextTurretRot_deg;
        float deltaAngle;

        if (_isAimingDetermined)
        {
            deltaAngle = _angleToDeterminedAiming_deg;
        }
        else
        {
            deltaAngle = _angleToPlayer_scope;
        }

        deltaAngle -= _turret.transform.eulerAngles.z;
        deltaAngle = ClampAngle(deltaAngle);

        if (Mathf.Abs(deltaAngle) <= _turretRotationVelo_Attacking_deg * Time.deltaTime)
        {
            if (_isAimingDetermined)
            {
                nextTurretRot_deg = _angleToDeterminedAiming_deg;
                _isAiming = true;
                _isAimingDetermined = false;
            }
            else
            {
                nextTurretRot_deg = _angleToPlayer_scope;
            }
        }
        else
        {
            nextTurretRot_deg = _turret.transform.rotation.eulerAngles.z + Mathf.Sign(deltaAngle) * _turretRotationVelo_Attacking_deg * Time.deltaTime;
        }

        _turretRot_deg = nextTurretRot_deg;
    }

    protected override void ThinkTurretRot_Attacking_Sonar()
    {
        float nextTurretRot_deg;
        float deltaAngle;

        if (_isAimingDetermined)
        {
            deltaAngle = _angleToDeterminedAiming_deg;
        }
        else
        {
            deltaAngle = _angleToPlayer_sonar;
        }

        deltaAngle -= _turret.transform.eulerAngles.z;
        deltaAngle = ClampAngle(deltaAngle);

        if (Mathf.Abs(deltaAngle) <= _turretRotationVelo_Attacking_deg * Time.deltaTime)
        {
            if (_isAimingDetermined)
            {
                nextTurretRot_deg = _angleToDeterminedAiming_deg;
                _isAiming = true;
                _isAimingDetermined = false;
            }
            else
            {
                nextTurretRot_deg = _angleToPlayer_sonar;
            }
        }
        else
        {
            nextTurretRot_deg = _turret.transform.rotation.eulerAngles.z + Mathf.Sign(deltaAngle) * _turretRotationVelo_Attacking_deg * Time.deltaTime;
        }

        _turretRot_deg = nextTurretRot_deg;
    }

    protected override void ThinkToShoot_Attacking()
    {
        base.ThinkToShoot_Attacking();
    }

    private Vector2 LookAtMap()
    {
        int[,] exceptMap = _mm.GetMap();
        int[,] objMap = _mm.GetOBJMap();
        int[] exceptedArrow = { -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        int[] dx = { 0, -1, -1, 0, 1, 1, 1, 0, -1 };
        int[] dy = { 0, 0, -1, -1, -1, 0, 1, 1, 1 };

        string debugcomment;

        int[] nowPos = { -Mathf.FloorToInt(transform.position.y) + exceptMap.GetLength(0) / 2 - 1, Mathf.FloorToInt(transform.position.x) + exceptMap.GetLength(1) / 2 };
        int x = 1, y = 0;

        int[] tmpPos = new int[2];
        for (int i = 0; i < 9; i++)
        {
            tmpPos[y] = nowPos[y] + dy[i];
            tmpPos[x] = nowPos[x] + dx[i];

            if (0 <= tmpPos[x] && tmpPos[x] <= exceptMap.GetLength(1) - 1 && 0 <= tmpPos[y] && tmpPos[y] <= exceptMap.GetLength(0) - 1)
            {
                if (exceptMap[tmpPos[y], tmpPos[x]] != -1 || objMap[tmpPos[y], tmpPos[x]] == 0)
                {
                    exceptedArrow[i] = exceptMap[tmpPos[y], tmpPos[x]];
                    //Debug.Log(i + ":" + tmpPos[y] + ", " + tmpPos[x] + ": " + map[tmpPos[y], tmpPos[x]]);
                }
            }
        }

        int exceptedId = 0;
        List<int> exceptedIdList = new List<int>();

        if(Random.Range(0, 100) <= _randomMoveProbability)
        {
            for(int i=0; i<9; i++)
            {
                if(exceptedArrow[i] != -1)
                {
                    exceptedIdList.Add(i);
                }
            }
        }
        else
        {
            for (int i = 0; i < 9; i++)
            {
                if (exceptedArrow[i] == 0)
                {
                    exceptedId = i;
                    exceptedIdList.Add(i);
                }
            }

            if (exceptedIdList.Count == 0)
            {
                exceptedId = 0;
                for (int i = 1; i < 9; i++)
                {
                    if (exceptedArrow[exceptedId] < exceptedArrow[i])
                    {
                        exceptedIdList.Clear();
                        exceptedId = i;
                        exceptedIdList.Add(i);
                    }
                    else if (exceptedArrow[exceptedId] == exceptedArrow[i])
                    {
                        exceptedIdList.Add(i);
                    }
                }
            }
        }

        exceptedId = exceptedIdList[Random.Range(0, exceptedIdList.Count)];
        //Vector2 hereToDestination = new Vector2((float)(nowPos[x] + dx[exceptedId]) - exceptMap.GetLength(1) / 2 + 0.5f, - (float)(nowPos[y] + dy[exceptedId]) + exceptMap.GetLength (0) / 2 - 0.5f) - new Vector2(transform.position.x, transform.position.y);
        Vector2 destinationPos = new Vector2((float)(nowPos[x] + dx[exceptedId]) - exceptMap.GetLength(1) / 2 + 0.5f, -(float)(nowPos[y] + dy[exceptedId]) + exceptMap.GetLength(0) / 2 - 0.5f);

        //_mm.PrintMap(exceptMap);
        //Debug.Log("");
        //_mm.PrintMap(objMap);
        return destinationPos;
    }

}
