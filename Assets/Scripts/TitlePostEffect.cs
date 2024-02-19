using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitlePostEffect : MonoBehaviour
{
    private TitleManager _titleManager = null;

    [SerializeField]
    private Material _noiseMaterial;

    [SerializeField]
    private Material _mixMaterial;

    [SerializeField]
    private Material _distortMaterial;

    [SerializeField]
    private Material _scanlinesAndStripesMaterial;

    [SerializeField]
    private Material _brownTubeMaterial;

    private RenderTexture _noiseTexture;

    private RenderTexture _mixedTexture;

    private RenderTexture _distortTexture;

    private RenderTexture _intermediateTexture;

    [SerializeField]
    private float _noiseTime = 3f;

    [SerializeField]
    private float _noiseToCleanTime = 3f;

    private float _noiseTimer = 0f;

    [SerializeField]
    private float _minNoiseRate = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        _titleManager = GameObject.Find("GameManager").GetComponent<TitleManager>();
        if (_titleManager == null)
        {
            Debug.Log("TitleManagerÇ™Ç›Ç¬Ç©ÇËÇ‹ÇπÇÒÇ≈ÇµÇΩÅB");
        }

        _noiseTexture = new RenderTexture(1920, 1080, 0);
        _noiseTexture.Create();

        _mixedTexture = new RenderTexture(1920, 1080, 0);
        _mixedTexture.Create();

        _distortTexture = new RenderTexture(1920, 1080, 0);
        _distortTexture.Create();

        _intermediateTexture = new RenderTexture(1920, 1080, 0);
        _intermediateTexture.Create();
    }

    private void Update()
    {
        switch (_titleManager.GameMode)
        {
            case TitleManager.Mode.Normal:
                _noiseTimer = 0;
                break;

            case TitleManager.Mode.Extra:
                _noiseTimer += Time.deltaTime;
                break;
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        switch (_titleManager.GameMode)
        {
            case TitleManager.Mode.Normal:
                //Graphics.Blit(source, _distortTexture, _distortMaterial);

                Graphics.Blit(source, _intermediateTexture, _scanlinesAndStripesMaterial);
                //Graphics.Blit(_distortTexture, _intermediateTexture, _distanceFeildMaterial);

                Graphics.Blit(_intermediateTexture, destination, _brownTubeMaterial);
                break;

            case TitleManager.Mode.Extra:
                Graphics.Blit(source, _noiseTexture, _noiseMaterial);

                float rate = _minNoiseRate;

                if(_noiseTimer < _noiseTime)
                {
                    rate = 1f;
                }else if(_noiseTimer <= _noiseTime + _noiseToCleanTime)
                {
                    rate = 1.0f - (_noiseTimer - _noiseTime) / _noiseToCleanTime;
                    rate = (1.0f - _minNoiseRate) * rate + _minNoiseRate;
                }

                _mixMaterial.SetFloat("_MixRate", rate);
                _mixMaterial.SetTexture("_MixedSideTex", _noiseTexture);

                Graphics.Blit(source, _mixedTexture, _mixMaterial);

                Graphics.Blit(_mixedTexture, _distortTexture, _distortMaterial);

                Graphics.Blit(_distortTexture, _intermediateTexture, _scanlinesAndStripesMaterial);

                Graphics.Blit(_intermediateTexture, destination, _brownTubeMaterial);
                break;
        }
    }
}
