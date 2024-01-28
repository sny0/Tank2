using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursol : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0.0f);
        transform.position = mousePosition;
    }
}
