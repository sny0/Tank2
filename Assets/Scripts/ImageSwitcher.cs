using UnityEngine;
using UnityEngine.UI;

public class ImageSwitcher : MonoBehaviour
{
    [SerializeField]
    private Sprite[] _images;

    [SerializeField]
    private float _switchInterval = 3f;

    private Image _image = null;
    private int _currentIndex = 0;
    private float _timer = 0f;

    // Start is called before the first frame update
    private void Start()
    {
        _image = GetComponent<Image>();
        if(_image == null)
        {
            Debug.Log("Imageコンポーネントが見つかりませんでした。");
        }

        if(_images.Length > 0)
        {
            _image.sprite = _images[0];
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if(_images.Length <= 1)
        {
            return;
        }

        _timer += Time.deltaTime;

        if(_timer >= _switchInterval)
        {
            _timer = 0f;
            _currentIndex = (_currentIndex + 1) % _images.Length;

            _image.sprite = _images[_currentIndex];
        }
    }
}
