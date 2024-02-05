using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBrain : TankBrain
{
    [SerializeField]
    private float _maxMovementVelo = 1.0f;

    [SerializeField]
    private float _bulletLockTime = 3f;

    private float _timeSinceLastShot = 0f;
    private bool _isBulletLock = false;

    [SerializeField]
    private int _maxBulletNumOnScreen = 5;

    private int _bulletNumOnScreen = 0;

    private enum DemoPlayerState
    {
        MoveRight,
        MoveLeft,
        MoveUp,
        MoveDown,
        Rotate,
        Shoot
    }

    private DemoPlayerState _demoPlayerState = DemoPlayerState.MoveRight;

    protected override void Start()
    {
        base.Start();
    }

    public override void Think()
    {
        base.Think();
    }

    public void ThinkOnlyTurretRot()
    {
        ThinkTurretRot();
    }

    protected override void ThinkNextMove()
    {
        Vector2 inputVector = new Vector2(0f, 0f);

        switch (_demoPlayerState)
        {
            case DemoPlayerState.MoveRight:
                inputVector = new Vector2(1.0f, 0f);
                break;

            case DemoPlayerState.MoveLeft:
                inputVector = new Vector2(-1.0f, 0f);
                break;

            case DemoPlayerState.MoveUp:
                inputVector = new Vector2(0f, 1.0f);
                break;

            case DemoPlayerState.MoveDown:
                inputVector = new Vector2(0f, -1.0f);
                break;
        }
        //Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _moveVec = inputVector * _maxMovementVelo;
    }

    protected override void ThinkTurretRot()
    {
        if(_demoPlayerState == DemoPlayerState.Rotate)
        {

        }
        else
        {

        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 lookDirection = mousePosition - transform.position;

        float angle_deg = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        _turretRot_deg = angle_deg;
    }

    protected override void ThinkToShoot()
    {
        _isShoot = false;
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

        if(!_isBulletLock)
        {
            _isShoot = true;
        }
    }

}
