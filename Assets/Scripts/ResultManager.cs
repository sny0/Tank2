using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _congraText;
    
    [SerializeField]
    private TextMeshProUGUI _resultText;

    // Start is called before the first frame update
    private void Start()
    {
        if (GameData._isClear)
        {
            _congraText.enabled = true;
            _resultText.text = "You Cleared This Game!";
        }
        else
        {
            _congraText.enabled = false;
            _resultText.text = "You Reached Stage" + GameData._stage + "!";
        }

        GameData._isClear = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("Title");
        }
    }
}
