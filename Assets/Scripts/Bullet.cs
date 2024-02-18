using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float _bulletSpeed = 1.0f; // �e�̃X�s�[�h
    
    private int _collisionNum = 0; // �Փˉ�
    
    [SerializeField]
    private int _maxReflectNum = 2; // �ő�Փˁi���ˁj��
    
    private Rigidbody2D rb;

    private Vector2 m_velocity; // �����x�N�g��

    [SerializeField]
    private GameObject _tri;
    
    private MapManager _mm; // MapManager
    public int[,] _bulletMap; // ���Ғl�}�b�v
    private bool flag = true;

    private GameObject _parent = null; // �e���o�����I�u�W�F�N�g
    private GameManager _gm = null;

    [SerializeField] 
    private GameObject  _hibana;
    // Start is called before the first frame update
    void Start()
    {
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        if(_gm == null)
        {
            Debug.Log("GameManager��������܂���ł����B");
        }

        rb = GetComponent<Rigidbody2D>();
        //Debug.Log(transform.rotation.eulerAngles.z);
        Vector3 vector = new Vector3(Mathf.Cos(transform.rotation.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(transform.rotation.eulerAngles.z * Mathf.Deg2Rad), 0.0f);
        vector = vector.normalized;
        //Debug.Log(Mathf.Cos(transform.rotation.eulerAngles.z * Mathf.Deg2Rad) + ", " +  Mathf.Sin(transform.rotation.eulerAngles.z * Mathf.Deg2Rad));
        //rb.velocity = transform.rotation.eulerAngles * _bulletSpeed;
        _mm = FindObjectOfType<MapManager>();
    
        m_velocity = vector * _bulletSpeed;
        rb.velocity = m_velocity;
        _mm.AddBullet(this);
        _bulletMap = _mm.MakeMap();
        _gm.ShotSE();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float angleInDegrees = transform.rotation.eulerAngles.z;

        _bulletMap = _mm.MakeMap();
        UpdatePotentialBulletMap(Mathf.FloorToInt(rb.position.x), Mathf.FloorToInt(rb.position.y), 1);


        Vector2 rayPos = transform.position;
        Vector2 rayDir = m_velocity.normalized;

        int frame = 0;
        for (int i=_collisionNum; i<=_maxReflectNum; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(rayPos, rayDir, float.MaxValue, 1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("Obstacle"));
            if (hit.collider != null)
            {
                if(flag) Debug.DrawRay(rayPos, rayDir*100f, Color.red, 5f, false);
                //Debug.Log("name: " + hit.collider.name + "rayPos: " + rayPos + ", rayDir: " + rayDir);
                float distance = Vector2.Distance(rayPos, hit.point);
                for (int j = 1; j * 0.2f < distance; j++)
                {
                    Vector2 p = j * rayDir * 0.2f;

                    UpdatePotentialBulletMap(Mathf.FloorToInt(p.x+rayPos.x), Mathf.FloorToInt(p.y+rayPos.y), Mathf.FloorToInt(p.magnitude/_bulletSpeed*60) + frame);
                    //if (flag) Instantiate(_tri, new Vector3(p.x+rayPos.x, p.y+rayPos.y, 0.0f), transform.rotation);
                }
                UpdatePotentialBulletMap(Mathf.FloorToInt(hit.point.x), Mathf.FloorToInt(hit.point.y), Mathf.FloorToInt(distance/_bulletSpeed*60) + frame);
                frame += Mathf.FloorToInt(distance / _bulletSpeed * 60);
                rayPos = hit.point+hit.normal*0.1f;
                rayDir = Vector2.Reflect(rayDir, hit.normal).normalized;
            }
        }


        flag = false;

    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.B))
        {
            _mm.PrintMap(_bulletMap);
        }
        */
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            ContactPoint2D contact = collision.contacts[0];

            Vector2 contactPoint = contact.point;
            Vector2 contactNormal = contact.normal;

            _collisionNum++;
            if(_collisionNum > _maxReflectNum)
            {
                _gm.DestroySE();
                DeleteBullet();
                return;
            }

            // �x�N�g���̊p�x�i���W�A���j���v�Z
            float angleRadians = Mathf.Atan2(contactNormal.y, contactNormal.x);

            // ���W�A����x���ɕϊ�
            float angleDegreeZ = Mathf.Rad2Deg * angleRadians;

            Vector3 angleDegrees = new Vector3(0.0f, 0.0f, angleDegreeZ);

            Vector2 reflectedVec = Vector2.Reflect(m_velocity, contactNormal).normalized;
            m_velocity = reflectedVec * _bulletSpeed;
            rb.velocity = m_velocity;
            float angle = Mathf.Atan2(reflectedVec.y, reflectedVec.x) * Mathf.Rad2Deg;
            rb.rotation = angle;

            Instantiate(_hibana, new Vector3(contactPoint.x, contactPoint.y, 0.0f), Quaternion.Euler(angleDegrees));

            _gm.ReflectSE();
        }
        else if (collision.gameObject.CompareTag("Bullet"))
        {
            _gm.DestroySE();    
            DeleteBullet();
            Debug.Log(collision.gameObject.name + "�ƏՓ˂��܂����B");
        }else if (collision.gameObject.CompareTag("Tank"))
        {
            _gm.ExplosionSE();   
            DeleteBullet();
            
        }
    }

    void AddMap(int x, int y, int n)
    {
        _bulletMap[-y + _bulletMap.GetLength(0) / 2-1, x + _bulletMap.GetLength(1) / 2] += n;
    }

    void UpdatePotentialBulletMap(int x, int y, int n)
    {
        if(x >= -_bulletMap.GetLength(1)/2 && x < _bulletMap.GetLength(1)/2 && y>=-_bulletMap.GetLength(0)/2 && y < _bulletMap.GetLength(0) / 2)
        {
            if (_bulletMap[-y + _bulletMap.GetLength(0) / 2 - 1, x + _bulletMap.GetLength(1) / 2] == 0)
            {
                AddMap(x, y, n);
            }
        }
        //Debug.Log(x + ", " + y + ", " + n);
        //_mm.PrintMap(_bulletMap);
     
    }


    public void DeleteBullet()
    {
        if (_parent != null && _parent.layer == LayerMask.NameToLayer("Player")){
            Player player = _parent.GetComponent<Player>();
            if(player != null)
            {
                player.ReduceBullet();
            }
        }
        _mm.UnRegister(this);
        Destroy(gameObject);
    }

    public void SetParent(GameObject go)
    {
        _parent = go;
    }
}
