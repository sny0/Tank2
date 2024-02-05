using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// �w�肳�ꂽ�J�����̓��e���L���v�`������T���v��
/// </summary>
public class CameraScreenShotCapturer : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private string _fileName = "CameraScreenShot";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            CaptureScreenShot( _fileName + ".png");
        }
    }

    // �J�����̃X�N���[���V���b�g��ۑ�����
    private void CaptureScreenShot(string filePath)
    {
        var rt = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 24);
        var prev = _camera.targetTexture;
        _camera.targetTexture = rt;
        _camera.Render();
        _camera.targetTexture = prev;
        RenderTexture.active = rt;

        var screenShot = new Texture2D(
            _camera.pixelWidth,
            _camera.pixelHeight,
            TextureFormat.RGB24,
            false);
        screenShot.ReadPixels(new Rect(0, 0, screenShot.width, screenShot.height), 0, 0);
        screenShot.Apply();

        var bytes = screenShot.EncodeToPNG();
        Destroy(screenShot);

        File.WriteAllBytes(filePath, bytes);
    }
}