using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultPostEffect : MonoBehaviour
{
    [SerializeField]
    private Material _scanlinesAndStripesMaterial;

    [SerializeField]
    private Material _brownTubeMaterial;

    [SerializeField]
    private Material _distortMaterial;

    private RenderTexture[] _intermediateTexture = new RenderTexture[2];

    private void Start()
    {
        // RenderTextureを初期化する
        _intermediateTexture[0] = new RenderTexture(1920, 1080, 0);
        _intermediateTexture[1] = new RenderTexture(1920, 1080, 0);

        _intermediateTexture[0].Create();
        _intermediateTexture[1].Create();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, _intermediateTexture[0], _distortMaterial);
        //Graphics.Blit(source, destination, _distortMaterial);

        Graphics.Blit(_intermediateTexture[0], _intermediateTexture[1], _scanlinesAndStripesMaterial);

        Graphics.Blit(_intermediateTexture[1], destination, _brownTubeMaterial);
    }
}
