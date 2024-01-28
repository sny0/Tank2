using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] bool _isMove = false; // 動くか否か
    [SerializeField] bool _isSmart = false; // 賢いか否か

    GameObject _mmObject; // MapManager GameObject
    MapManager _mm; // MapManager

    GameObject _playerObject; // Player GameObject
    [SerializeField] GameObject _explosion;

    public float _speed = 5.0f; // 機体の移動スピード

    private Rigidbody2D rb;
    [SerializeField] GameObject bulletPrefab; // 発射する弾
    [SerializeField] GameObject _Tri;
    [SerializeField] float _bulletLockTime = 2.0f;
    float _lastShotTime = 0.0f;

    bool _isHitExpected = false; // ヒットが期待できるか否か

    private GameObject _battery; // 砲台
    [SerializeField] float _batteryLength = 1f; // 砲台の長さ

    int[] dx = { 0, -1, -1, 0, 1, 1, 1, 0, -1 };
    int[] dy = { 0, 0, -1, -1, -1, 0, 1, 1, 1 };
    int[,] map;
    Vector2 _velocity;

    Vector2[,] _wallLine;
    bool _isPaused = false;
    bool _startPaused = false;
    float _time = 0.0f;

    bool[] _isHitReflectedBullet = { false, false, false, false };
    float[] _batteryToWallRot = new float[4];

    bool _isAlive = true;

    GameManager _gm = null;

    [SerializeField] float _batteryRotVeo = 90.0f;
    float _toTargetAngle;
    bool _isAming = false;
    bool _isAmingDetermined = false;

    // Start is called before the first frame update
    void Start()
    {
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gm == null)
        {
            Debug.Log("GameManagerが見つかりませんでした。");
        }
        _mmObject = GameObject.Find("MapManager");
        if(_mmObject != null)
        {
            _mm = _mmObject.GetComponent<MapManager>();
            if(_mm == null)
            {
                Debug.Log("MapManagerコンポーネントがみつかりませんでした。");
            }
        }
        else
        {
            Debug.Log("MapManagerオブジェクトが見つかりませんでした。");
        }

        _playerObject = GameObject.Find("Player");
        if(_playerObject == null)
        {
            Debug.Log("Playerが見つかりませんでした。");
        }

        rb = GetComponent<Rigidbody2D>();
        _battery = transform.Find("battery").gameObject;
        _velocity = new Vector2(0.0f, 0.0f);
        map = new int[10,22];

        _wallLine = new Vector2[4, 2];
        _wallLine[0, 0] = new Vector2(-10, 0);
        _wallLine[0, 1] = new Vector2(-10, 1);
        _wallLine[1, 0] = new Vector2(0, 4);
        _wallLine[1, 1] = new Vector2(1, 4);
        _wallLine[2, 0] = new Vector2(10, 0);
        _wallLine[2, 1] = new Vector2(10, 1);
        _wallLine[3, 0] = new Vector2(0, -4);
        _wallLine[3, 1] = new Vector2(1, -4);

        _startPaused = true;
        _isAming = false;
        _isAmingDetermined = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_startPaused)
        {
            _startPaused = false;
            _time = 0.0f;
            _isPaused = true;
            return;
        }
        else if (_isPaused)
        {
            _time += Time.deltaTime;
            if (_time >= 3.0f)
            {
                _isPaused = false;
            }
            else
            {
                return;
            }
        }
        if (_isMove)
        {
            map = _mm.GetMap();
            int[] exceptedArrow = { -1, -1, -1, -1, -1, -1, -1, -1, -1 };

            int[] nowPos = { -Mathf.FloorToInt(transform.position.y) + map.GetLength(0) / 2 - 1, Mathf.FloorToInt(transform.position.x) + map.GetLength(1) / 2 };
            int x = 1, y = 0;
            //Debug.Log("nowPos:" + nowPos[0] + ", " + nowPos[1]);
            int[] tmpPos = new int[2];
            for (int i = 0; i < 9; i++)
            {
                tmpPos[y] = nowPos[y] + dy[i];
                tmpPos[x] = nowPos[x] + dx[i];

                if (0 <= tmpPos[x] && tmpPos[x] <= map.GetLength(1) - 1 && 0 <= tmpPos[y] && tmpPos[y] <= map.GetLength(0) - 1)
                {
                    if (map[tmpPos[y], tmpPos[x]] != -1)
                    {
                        exceptedArrow[i] = map[tmpPos[y], tmpPos[x]];
                        //Debug.Log(i + ":" + tmpPos[y] + ", " + tmpPos[x] + ": " + map[tmpPos[y], tmpPos[x]]);
                    }
                }
            }

            int exceptedId = 0;
            List<int> exceptedIdList = new List<int>();

            for (int i = 0; i < 9; i++)
            {
                if (exceptedArrow[i] == 0)
                {
                    exceptedId = i;
                    exceptedIdList.Add(i);
                }
                else
                {
                    if (exceptedArrow[exceptedId] < exceptedArrow[i])
                    {
                        exceptedIdList.Clear();
                        exceptedId = i;
                        exceptedIdList.Add(i);
                    }
                }
            }


            exceptedId = exceptedIdList[Random.Range(0, exceptedIdList.Count)];
            Vector2 hereToDestination = new Vector2((float)(nowPos[x] + dx[exceptedId]) - map.GetLength(1) / 2 + 0.5f, -(float)(nowPos[y] + dy[exceptedId]) + map.GetLength(0) / 2 - 0.5f) - new Vector2(transform.position.x, transform.position.y);
            //Debug.Log(exceptedId + ", " + hereToDestination+ ", "+ new Vector2((float)(nowPos[x] + dx[exceptedId]) - map.GetLength(1) / 2 + 0.5f, -(float)(nowPos[y] + dy[exceptedId]) + map.GetLength(0) / 2 - 0.5f) + ", (" + nowPos[0] + "," + nowPos[1]+")");
            //Debug.Log(exceptedId + ": " + exceptedArrow[0] + ", " + exceptedArrow[1] + ", " + exceptedArrow[2] + ", " + exceptedArrow[3] + ", " + exceptedArrow[4]);
            if (hereToDestination.magnitude <= _speed)
            {
                _velocity = hereToDestination;
                //Debug.Log("a:"+_velocity);
            }
            else
            {
                _velocity = hereToDestination.normalized * _speed;
                //Debug.Log("b:"+_velocity);
            }
            rb.velocity = _velocity;
            //Debug.Log(rb.velocity +", " +_velocity + ", exceID:" + exceptedId + ": " + exceptedArrow[0] + ", " + exceptedArrow[1] + ", " + exceptedArrow[2] + ", " + exceptedArrow[3] + ", " + exceptedArrow[4]);

        }
        if (!_isAmingDetermined)
        {
            _isHitExpected = false;

            Vector2 myPos = transform.position;
            Vector2 playerPos = _playerObject.transform.position;

            RaycastHit2D hit = Physics2D.Raycast(myPos + _batteryLength * (playerPos - myPos).normalized, (playerPos - myPos).normalized, float.MaxValue, 1 << LayerMask.NameToLayer("Obstacle") | 1 << LayerMask.NameToLayer("Tank") | 1 << LayerMask.NameToLayer("Player"));
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    _isHitExpected = true;
                    _isAmingDetermined = true;
                }
            }

            Vector2 batteryDirection = new Vector2(_playerObject.transform.position.x - transform.position.x, _playerObject.transform.position.y - transform.position.y);
            float angle = Mathf.Atan2(batteryDirection.y, batteryDirection.x) * Mathf.Rad2Deg;
            if (_isAmingDetermined)
            {
                _toTargetAngle = angle;
                //_battery.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            }


            if (_isSmart && !_isHitExpected)
            {
                for (int i = 0; i < 4; i++)
                {
                    _isHitReflectedBullet[i] = false;
                }

                for (int i = 0; i < 4; i++)
                {
                    Vector2 reflectedPos = ReflectPoint(_wallLine[i, 0], _wallLine[i, 1], new Vector2(_playerObject.transform.position.x, _playerObject.transform.position.y));
                    //Instantiate(_Tri, new Vector3(reflectedPos.x, reflectedPos.y, 0.0f), transform.rotation);
                    float angleToWall = Mathf.Atan2(reflectedPos.y - myPos.y, reflectedPos.x - myPos.x) * Mathf.Rad2Deg;
                    _batteryToWallRot[i] = angleToWall;
                    RaycastHit2D hit0 = Physics2D.Raycast(myPos + _batteryLength * (reflectedPos - myPos).normalized, (reflectedPos - myPos).normalized, float.MaxValue, 1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("Tank") | 1 << LayerMask.NameToLayer("Obstacle"));
                    Debug.DrawRay(myPos + _batteryLength * (reflectedPos - myPos).normalized, (reflectedPos - myPos) * 1.5f, Color.blue, 5f, false);
                    //Instantiate(_Tri, myPos + _batteryLength * (reflectedPos - myPos).normalized, transform.rotation);
                    if (hit0.collider != null)
                    {
                        //Debug.Log(i + " " + "layer:" + hit0.collider.gameObject.layer + " normal:" + hit0.normal);
                        if (hit0.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                        {
                            Vector2 rayPos = hit0.point + hit0.normal * 0.1f;
                            Vector2 rayDir = Vector2.Reflect(reflectedPos - myPos, hit0.normal).normalized;
                            RaycastHit2D hit1 = Physics2D.Raycast(rayPos, rayDir, float.MaxValue, 1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("Tank") | 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Obstacle"));
                            Debug.DrawRay(rayPos, rayDir * 100, Color.yellow, 5f, false);
                            if (hit1.collider != null)
                            {
                                //Debug.Log(i+ "" + "layer2:" + hit1.collider.gameObject.layer);
                                if (hit1.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                                {
                                    _isHitReflectedBullet[i] = true;
                                    _isHitExpected = true;
                                }
                                else
                                {
                                    //Instantiate(_Tri, hit1.point, transform.rotation);
                                }
                            }
                        }
                    }
                }

                List<int> playerHitWallIndent = new List<int>();
                for (int i = 0; i < 4; i++)
                {
                    if (_isHitReflectedBullet[i])
                    {
                        playerHitWallIndent.Add(i);
                    }
                }

                Debug.Log("isHitWall:" + _isHitReflectedBullet[0] + ", " + _isHitReflectedBullet[1] + ", " + _isHitReflectedBullet[2] + ", " + _isHitReflectedBullet[3]);

                if (playerHitWallIndent.Count > 0)
                {
                    _isAmingDetermined = true;

                    int rnd = Random.Range(0, playerHitWallIndent.Count);
                    _toTargetAngle = _batteryToWallRot[playerHitWallIndent[rnd]];

                    /*
                    if(deltaAngle >= 0f) // 時計回りに回転
                    {
                        if(Mathf.Abs(deltaAngle) >= _batteryRotVeo * Time.deltaTime)
                        {
                            newAngleZ = _battery.transform.rotation.eulerAngles.z + _batteryRotVeo * Time.deltaTime;
                            newAngleZ = ((newAngleZ);
                        }
                        else
                        {
                            newAngleZ = _battery.transform.rotation.eulerAngles.z + deltaAngle;
                            newAngleZ = ClampAngle(newAngleZ);
                        }
                    }
                    else // 反時計回りに回転
                    {
                        if (Mathf.Abs(deltaAngle) >= _batteryRotVeo * Time.deltaTime)
                        {
                            newAngleZ = _battery.transform.rotation.eulerAngles.z - _batteryRotVeo * Time.deltaTime;
                            newAngleZ = ClampAngle(newAngleZ);
                        }
                        else
                        {
                            newAngleZ = _battery.transform.rotation.eulerAngles.z - deltaAngle;
                            newAngleZ = ClampAngle(newAngleZ);
                        }
                    }
                    */

                    //Debug.Log("AngleToWall:" + _batteryToWallRot[playerHitWallIndent[rnd]] + ", deltaAngle:" + deltaAngle + ", newAngle:" + newAngleZ + "nowAngle:" + _battery.transform.rotation.eulerAngles.z);
                    //_battery.transform.rotation = Quaternion.Euler(new Vector3(0, 0, _batteryToWallRot[playerHitWallIndent[rnd]]));
                }
            }
        }
        if (_isAmingDetermined)
        {
            float deltaAngle = _toTargetAngle - _battery.transform.rotation.eulerAngles.z;

            deltaAngle = ClampAngle(deltaAngle);
            Debug.Log("AngleToWall:" + _toTargetAngle + ", deltaAngle:" + deltaAngle + ", nowAngle:" + _battery.transform.rotation.eulerAngles.z);

            float newAngleZ;

            if (Mathf.Abs(deltaAngle) <= _batteryRotVeo * Time.deltaTime)
            {
                Debug.Log("tttt");
                newAngleZ = _toTargetAngle;
                _isAming = true;
            }
            else
            {
                Debug.Log("sssss" + _batteryRotVeo * Time.deltaTime);
                newAngleZ = _battery.transform.rotation.eulerAngles.z + Mathf.Sign(deltaAngle) * _batteryRotVeo * Time.deltaTime;
                Debug.Log("newAngle:" + newAngleZ);
            }

            _battery.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, newAngleZ));
        }

    }

    private void Update()
    {
        if (_isPaused) return;
        _lastShotTime += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.C))
        {
            _mm.PrintMap(map);
        }

        if (_isHitExpected)
        {
            if (_lastShotTime >= _bulletLockTime && _isAming == true)
            {

                Shot();
                _lastShotTime = 0.0f;
                _isAming = false;
                _isAmingDetermined = false;
            }
        }
        Debug.Log("AimedAngle:" + _toTargetAngle + ", nowAngle:" + _battery.transform.rotation.eulerAngles.z);

    }

    void Shot()
    {
        Debug.Log("Enemy's Shot!!");
        GameObject bullet = Instantiate(bulletPrefab, transform.position + _batteryLength * new Vector3(Mathf.Cos(_battery.transform.rotation.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(_battery.transform.rotation.eulerAngles.z * Mathf.Deg2Rad), 0.0f), _battery.transform.rotation);
        _gm.ShotSE();
    }

    Vector2 ReflectPoint(Vector2 linePoint0, Vector2 linePoint1, Vector2 point)
    {
        Vector2 lineNormal = new Vector2(linePoint1.y - linePoint0.y, linePoint0.x - linePoint1.x);

        Vector2 lineToPoint = new Vector2(point.x - linePoint0.x, point.y - linePoint0.y);

        float dotProduct = Vector2.Dot(lineNormal, lineToPoint);

        Vector2 reflectedPoint = point - 2 * dotProduct * lineNormal / lineNormal.sqrMagnitude;

        return reflectedPoint;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Die();
        }
    }

    public void Die()
    {
        _isAlive = false;
        gameObject.SetActive(false);
        Instantiate(_explosion, transform.position, transform.rotation);
    }

    IEnumerator PauseForSeconds(float time)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(time);

        Time.timeScale = 1f;
    }

    public bool isAlive()
    {
        return _isAlive;
    }

    private float ClampAngle(float angle)
    {
        while(angle < 0.0f)
        {
            angle += 360.0f;
        }

        angle %= 360;

        if(angle > 180.0f)
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
}
