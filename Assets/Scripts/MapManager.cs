using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    int[,] _objMap; // �I�u�W�F�N�g�̃}�b�v�i-1:��, 1:��Q��, 0:�����Ȃ�)
    int[,] _expectedMap; // ���Ғl�}�b�v�i-1:��, 0:���S, n(>0):n�t���[�����Bullet�����ł���j
    List<Bullet> _bullets; // ��ʏ�ɑ��݂���Bullet��ێ�

    List<GameObject> _obstacles;
    // Start is called before the first frame update
    void Start()
    {
        _objMap = new int[10, 22];
        _expectedMap = new int[10, 22];
        for (int i = 0; i < 22; i++)
        {
            _objMap[0, i] = -1;
            _expectedMap[0, i] = -1;
            _objMap[9, i] = -1;
            _expectedMap[9, i] = -1;
        }
        for (int i = 0; i < 10; i++)
        {
            _objMap[i, 0] = -1;
            _expectedMap[i, 0] = -1;
            _objMap[i, 21] = -1;
            _expectedMap[i, 21] = -1;
        }
        _bullets = new List<Bullet>();

        _obstacles = new List<GameObject>();

        GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (var go in allGameObjects)
        {
            if (go.layer == LayerMask.NameToLayer("Obstacle"))
            {
                _obstacles.Add(go);
            }
        }

        UpdateObjMap();
        Debug.Log("objMap");
        PrintMap(_objMap);
        //Debug.Log(expectedMap.GetLength(0) + ", " + expectedMap.GetLength(1));
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0; i<10; i++)
        {
            for(int j=0; j<22; j++)
            {
                _expectedMap[i, j] = 0;
            }
        }
        for (int i = 0; i < 22; i++)
        {
            _objMap[0, i] = -1;
            _expectedMap[0, i] = -1;
            _objMap[9, i] = -1;
            _expectedMap[9, i] = -1;
        }
        for (int i = 0; i < 10; i++)
        {
            _objMap[i, 0] = -1;
            _expectedMap[i, 0] = -1;
            _objMap[i, 21] = -1;
            _expectedMap[i, 21] = -1;
        }


        foreach (Bullet b in _bullets){
            for(int i=0; i<10; i++)
            {
                for(int j=0; j<22; j++)
                {
                    if(_expectedMap[i,j] != -1)
                    {
                        if(b._bulletMap[i,j] > 0)
                        {
                            if(_expectedMap[i,j] > 0)
                            {
                                _expectedMap[i, j] = Mathf.Min(_expectedMap[i, j], b._bulletMap[i, j]);
                            }
                            else
                            {
                                _expectedMap[i, j] = b._bulletMap[i, j];
                            }
                            
                        }
                        else
                        {
                            
                        }
                    }
                }
            }
        }

        /*
        if (Input.GetKeyDown(KeyCode.A))
        {
            PrintMap(_expectedMap);
        }
        */
    }

    public void PrintMap(int[,] map)
    {
        int rowNum = map.GetLength(0);
        int columnNum = map.GetLength(1);
        for (int i = 0; i < rowNum; i++)
        {
            string tmp = i.ToString() + ": ";
            for (int j = 0; j < columnNum; j++)
            {
                tmp += map[i, j].ToString();
                if (j < columnNum - 1)
                {
                    tmp += ", ";
                }
            }
            Debug.Log(tmp);
        }
    }

    public int[,] GetMap()
    {
        return _expectedMap;
    }

    public int[,] GetOBJMap()
    {
        return _objMap;
    }

    public void AddMap(int x, int y, int n)
    {
        _expectedMap[y+_expectedMap.GetLength(0)/2, x+_expectedMap.GetLength(1)/2] += n;
    }

    public int[,] MakeMap()
    {
        int[,] tmpMap = new int[10, 22];
        for (int i = 0; i < 22; i++)
        {
            tmpMap[0, i] = -1;
            tmpMap[9, i] = -1;
        }
        for (int i = 0; i < 10; i++)
        {
            tmpMap[i, 0] = -1;
            tmpMap[i, 21] = -1;
        }
        return tmpMap;
    }

    // _bullets��Bullet��o�^
    public void AddBullet(Bullet bullet)
    {
        _bullets.Add(bullet);
    }

    // _bullets����Bullet������
    public void UnRegister(Bullet bullet)
    {
        if (_bullets.Contains(bullet))
        {
            _bullets.Remove(bullet);
        }
        else
        {
            Debug.Log("�w�肵��Bullet�C���X�^���X��������܂���ł����B");
        }
    }

    private void UpdateObjMap()
    {
        foreach (var go in _obstacles)
        {
            int obstaclePosY = Mathf.FloorToInt(go.transform.position.y);
            int obstaclePosX = Mathf.FloorToInt(go.transform.position.x);

            if (obstaclePosX >= -_objMap.GetLength(1) / 2 && obstaclePosX < _objMap.GetLength(1) / 2 && obstaclePosY >= -_objMap.GetLength(0) / 2 && obstaclePosY < _objMap.GetLength(0) / 2)
            {
                _objMap[-obstaclePosY + _objMap.GetLength(0) / 2 - 1, obstaclePosX + _objMap.GetLength(1) / 2] = 1;
            }
        }
    }
}
