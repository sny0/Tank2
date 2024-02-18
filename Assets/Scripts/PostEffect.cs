using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffect : MonoBehaviour
{
    [SerializeField]
    private Material _mixMaterial;

    [SerializeField]
    private Material _distanceFieldMaterial;

    [SerializeField] 
    private Material _postEffectMaterial;

    [SerializeField]
    private RenderTexture _bulletAfterImageTexture;

    private RenderTexture[] _intermediateTexture = new RenderTexture[2];

    private void Start()
    {
        // RenderTexture������������
        _intermediateTexture[0] = new RenderTexture(1920, 1080, 0);
        _intermediateTexture[1] = new RenderTexture(1920, 1080, 0);
        //_bufferRenderTexture.enableRandomWrite = true; // �I�v�V������Random Write��L���ɂ���Ȃ�
        _intermediateTexture[0].Create();
        _intermediateTexture[1].Create();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        _mixMaterial.SetTexture("_BulletAfterImageTex", _bulletAfterImageTexture);
        Graphics.Blit(source, _intermediateTexture[0], _mixMaterial);

        Graphics.Blit(_intermediateTexture[0], _intermediateTexture[1], _distanceFieldMaterial);
        //Graphics.Blit(source, _intermediateTexture[1], _distanceFieldMaterial);

        Graphics.Blit(_intermediateTexture[1], destination, _postEffectMaterial);
    }
}