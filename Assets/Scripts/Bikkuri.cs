using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bikkuri : MonoBehaviour
{
    private GameManager _gm = null;

    [SerializeField]
    private float _livingTime = 0.5f;

    private float _timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gm == null)
        {
            Debug.Log("GameManager‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ‚Å‚µ‚½B");
        }

        _gm.FindSE();
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;

        if(_timer >= _livingTime)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
