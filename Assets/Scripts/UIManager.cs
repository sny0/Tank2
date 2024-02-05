using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    private GameObject[] _countDown;

    private float _time;

    private void Start()
    {

        _countDown = new GameObject[4];
        _countDown[0] = transform.Find("CountDown_0").gameObject;
        _countDown[1] = transform.Find("CountDown_1").gameObject;
        _countDown[2] = transform.Find("CountDown_2").gameObject;
        _countDown[3] = transform.Find("CountDown_3").gameObject;
        SetAllCountDownNotActive();
    }

    private void Update()
    {
        _time += Time.deltaTime;
        SetAllCountDownNotActive();
        if(0f <= _time && _time < 1f)
        {
            SetCountDownActive(3);
        }else if(1f <= _time && _time < 2f)
        {
            SetCountDownActive(2);
        }else if(2f <= _time && _time < 3f)
        {
            SetCountDownActive(1);
        }else if(3f <= _time && _time < 4f)
        {
            SetCountDownActive(0);
        }
    }

    private void SetAllCountDownNotActive()
    {
        for(int i=0; i<_countDown.Length; i++)
        {
            if(_countDown[i] != null)
            {
                _countDown[i].SetActive(false);
            }
        }
    }

    private void SetCountDownActive(int i)
    {
        if(0 <= i && i < _countDown.Length)
        {
            if(_countDown[i] != null)
            {
                _countDown[i].SetActive(true);
            }
        }
    }
}
