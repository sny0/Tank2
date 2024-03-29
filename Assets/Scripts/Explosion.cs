using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] 
    private float _livingTime = 1.5f;

    float _timer = 0.0f;
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        _timer += Time.deltaTime;
        if(_timer >= _livingTime)
        {
            Destroy(gameObject);
        }
    }
}
