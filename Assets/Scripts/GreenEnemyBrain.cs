using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenEnemyBrain : EnemyBrain
{
    [SerializeField]
    float _turretRotationVelo_Searching_deg;

    [SerializeField]
    bool _isClockwise = false;

    [SerializeField]
    float _turretRotationVelo_Attacking_deg;

    protected override void ThinkNextMove_Attacking()
    {
        _moveVec = new Vector2(0f, 0f);
    }

    protected override void ThinkNextMove_Searching()
    {
        _moveVec = new Vector2(0f, 0f);
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
}
