using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBody : MonoBehaviour
{
    [SerializeField]
    protected float _movementSpeed = 1f;

    protected Rigidbody2D _rb;
    protected GameObject _turret = null;
    [SerializeField]
    protected float _turretLength = 1f;
    [SerializeField]
    protected GameObject _bulletPrefab;

    protected void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _turret = transform.Find("battery").gameObject;

        if(_turret == null)
        {
            Debug.Log("batteryÇ™å©Ç¬Ç©ÇËÇ‹ÇπÇÒÇ≈ÇµÇΩÅB");
        }

    }

    public void Move(Vector2 moveVec)
    {
        _rb.velocity = _movementSpeed * moveVec;
    }

    public void TurretRotate(float angle_deg)
    {
        _turret.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle_deg));
        //Debug.Log(_turret.transform.eulerAngles.z);
    }

    public void Shoot(bool isShoot)
    {
        if (isShoot)
        {
            Debug.Log("Shoot!");
            GameObject bullet = Instantiate(
                _bulletPrefab,
                transform.position + _turretLength * new Vector3(Mathf.Cos(_turret.transform.rotation.eulerAngles.z * Mathf.Deg2Rad),Mathf.Sin(_turret.transform.rotation.eulerAngles.z * Mathf.Deg2Rad), 0.0f),
                _turret.transform.rotation);
        }
    }

}
