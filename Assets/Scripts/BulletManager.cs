using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    private List<Bullet> _bulletsList;
    private GameObject _owner;

    public BulletManager()
    {
        _bulletsList = new List<Bullet>();
    }

    public void RegisterBullet(Bullet bullet)
    {
        _bulletsList.Add(bullet);
    }

    public void UnRegisterBullet(Bullet bullet)
    {
        bool isRegistered = _bulletsList.Contains(bullet);
        if (isRegistered)
        {
            _bulletsList.Remove(bullet);
        }
        else
        {
            Debug.Log(bullet + "ÇÕìoò^Ç≥ÇÍÇƒÇ¢Ç‹ÇπÇÒÅB");
        }
    }

    public void Clear()
    {
        _bulletsList.Clear();
    }

    public int GetBulletsNum()
    {
        return _bulletsList.Count;
    }
}
