using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TankBrain : MonoBehaviour
{
    public Vector2 MoveVec => _moveVec;
    protected Vector2 _moveVec = new Vector2(0f, 0f);

    public float TurretRot_deg => _turretRot_deg;
    protected float _turretRot_deg = 0f;

    public bool IsShoot => _isShoot;
    protected bool _isShoot = false;

    protected GameObject _turret = null;

    protected virtual void Start()
    {
        _turret = transform.Find("battery").gameObject;
        if(_turret == null)
        {
            Debug.Log("batteryÇ™å©Ç¬Ç©ÇËÇ‹ÇπÇÒÇ≈ÇµÇΩÅB");
        }
    }

    public virtual void Think()
    {
        ThinkNextMove();

        ThinkTurretRot();

        ThinkToShoot();

    }


    protected virtual void ThinkNextMove()
    {

    }

    protected virtual void ThinkTurretRot()
    {

    }

    protected virtual void ThinkToShoot()
    {

    }

}