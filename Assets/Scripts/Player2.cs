using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : Tank
{
    protected override void Start()
    {
        _tankBrain = GetComponent<TankBrain>();
        _tankBody = GetComponent<TankBody>();

        base.Start();
    }

    protected override void Update()
    {
        /*
        if (_isStart)
        {
            _tankBrain.Think();
            _tankBody.Move(_tankBrain.MoveVec);
            _tankBody.TurretRotate(_tankBrain.TurretRot_deg);
            _tankBody.Shoot(_tankBrain.IsShoot);
        }
        else
        {
            _timeSinceStart += Time.deltaTime;
            if (_timeSinceStart >= _startTime)
            {
                _isStart = true;
            }

            _tankBody.Move(_tankBrain.MoveVec);
        }
        */
        _tankBrain.Think();
        _tankBody.Move(_tankBrain.MoveVec);
        _tankBody.TurretRotate(_tankBrain.TurretRot_deg);
        _tankBody.Shoot(_tankBrain.IsShoot);

    }
}
