using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultPostEffect : MonoBehaviour
{
    [SerializeField]
    private Material _distanceFieldMaterial;

    [SerializeField]
    private Material _postEffectMaterial;

    private RenderTexture _intermediateTexture;

    private void Start()
    {
        // RenderTextureを初期化する
        _intermediateTexture = new RenderTexture(1920, 1080, 0);
        //_bufferRenderTexture.enableRandomWrite = true; // オプションでRandom Writeを有効にするなど
        _intermediateTexture.Create();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, _intermediateTexture, _distanceFieldMaterial);
        //Graphics.Blit(source, _intermediateTexture[1], _distanceFieldMaterial);

        Graphics.Blit(_intermediateTexture, destination, _postEffectMaterial);
    }
}
