using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPostEffect : MonoBehaviour
{
    [SerializeField]
    private Material _afterEffectMaterial;

    private RenderTexture _bufferRenderTexture0;
    private RenderTexture _bufferRenderTexture1;


    [SerializeField]
    private Material _initializeMaterial;

    private void Start()
    {
        // RenderTextureを初期化する
        _bufferRenderTexture0 = new RenderTexture(1980, 1080, 0);
        _bufferRenderTexture1 = new RenderTexture(1980, 1080, 0);
        //_bufferRenderTexture.enableRandomWrite = true; // オプションでRandom Writeを有効にするなど
        _bufferRenderTexture0.Create();
        _bufferRenderTexture1.Create();

        Graphics.Blit(null, _bufferRenderTexture0, _initializeMaterial);
        Graphics.Blit(null, _bufferRenderTexture1, _initializeMaterial);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(Time.frameCount % 2 == 0)
        {
            _afterEffectMaterial.SetTexture("_AfterImageTex", _bufferRenderTexture0);

            Graphics.Blit(source, _bufferRenderTexture1, _afterEffectMaterial);
            Graphics.Blit(_bufferRenderTexture1, destination);
        }
        else
        {
            _afterEffectMaterial.SetTexture("_AfterImageTex", _bufferRenderTexture1);

            Graphics.Blit(source, _bufferRenderTexture0, _afterEffectMaterial);
            Graphics.Blit(_bufferRenderTexture0, destination);
        }
        //_afterEffectMaterial.SetTexture("_AfterImageTex", _bufferRenderTexture);
        //Graphics.Blit(source, destination, _afterEffectMaterial);
        //Graphics.Blit(destination, _bufferRenderTexture);
    }
}
