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

    [SerializeField]
    private TextMeshProUGUI _secretText;

    [SerializeField]
    private TextMeshProUGUI _clickText;

    [SerializeField]
    private float _lockTime = 3.0f;

    private float _lockTimer = 0f;

    [SerializeField]
    private float _displayTime = 1.0f;

    [SerializeField]
    private float _notDisplayTime = 0.3f;

    private float _blinkingTimer = 0.0f;

    // Start is called before the first frame update
    private void Start()
    {
        if (GameData._isClear)
        {
            _congraText.enabled = true;
            _secretText.enabled = true;
            _resultText.text = "You Cleared This Game!";
            UpdateRanking(GameData._clearNumber);
        }
        else
        {
            _congraText.enabled = false;
            _secretText.enabled = false;
            _resultText.text = "You Reached Stage" + GameData._stage + "!";
            UpdateRanking(GameData._stage);
        }

        GameData._isClear = false;
    }

    // Update is called once per frame
    private void Update()
    {
        _lockTimer += Time.deltaTime;

        if(_lockTimer >= _lockTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene("Title");
            }


            _blinkingTimer += Time.deltaTime;
            if(_blinkingTimer <= _displayTime)
            {
                _clickText.enabled = true;
            }else if(_blinkingTimer <= _displayTime + _notDisplayTime)
            {
                _clickText.enabled = false;
            }
            else
            {
                _blinkingTimer = 0f;
            }
        }
        else
        {
            _clickText.enabled = false;
        }
    }

    private void UpdateRanking(int newScore)
    {
        int[] scores = new int[5];

        if (!GameData._isExtra)
        {
            for (int i = 0; i < 5; i++)
            {
                scores[i] = PlayerPrefs.GetInt(i.ToString() + "Score", GameData._NoRecordNumber);
            }

            for (int i = 0; i < 5; i++)
            {
                if (scores[i] < newScore)
                {
                    for (int j = 4; j > i; j--)
                    {
                        scores[j] = scores[j - 1];
                    }
                    scores[i] = newScore;
                    break;
                }
            }

            for (int i = 0; i < 5; i++)
            {
                PlayerPrefs.SetInt(i.ToString() + "Score", scores[i]);
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                scores[i] = PlayerPrefs.GetInt(i.ToString() + "Extra", GameData._NoRecordNumber);
            }

            for (int i = 0; i < 5; i++)
            {
                if (scores[i] < newScore)
                {
                    for (int j = 4; j > i; j--)
                    {
                        scores[j] = scores[j - 1];
                    }
                    scores[i] = newScore;
                    break;
                }
            }

            for (int i = 0; i < 5; i++)
            {
                PlayerPrefs.SetInt(i.ToString() + "Extra", scores[i]);
            }
        }

    }
}
