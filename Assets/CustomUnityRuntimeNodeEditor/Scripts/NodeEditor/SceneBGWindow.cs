using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using System.Security.Cryptography;
using UnityEngine.XR;
using TMPro;

public class SceneBGWindow : MonoBehaviour
{
    [HideInInspector] public Action<byte[], bool, bool, string, string> OnUpdateBG;
    [SerializeField] private RawImage bgPreview;
    private byte[] bgImage;
    [SerializeField] private Button addBt;
    [SerializeField] private Button saveBt;
    [SerializeField] private Button closeBt;
    [SerializeField] private Toggle fadeInTgl;
    [SerializeField] private Toggle fadeOutTgl;
    [SerializeField] private TMP_InputField fadeInDuration;
    [SerializeField] private TMP_InputField fadeOutDuration;
    // Start is called before the first frame update
    private void OnEnable()
    {
        addBt.onClick.AddListener(OpenImagePicker);
        saveBt.onClick.AddListener(SaveBG);
        closeBt.onClick.AddListener(Close);
        bgImage = null;
    }


    private void OnDisable()
    {
        addBt.onClick.RemoveAllListeners();
        saveBt.onClick.RemoveAllListeners();
        closeBt.onClick.RemoveAllListeners();
        OnUpdateBG = null;
        bgImage = null;
    }

    public void SetData(byte[] image, bool fadeIn, bool fadeOut, string fadeInDur, string fadeOutDur)
    {
        if(image != null)
        {
            Texture2D tex = new(2, 2, TextureFormat.RGBA32, false);
            tex.LoadImage(image);
            bgPreview.texture = tex;
        }

        fadeInTgl.SetIsOnWithoutNotify(fadeIn);
        fadeOutTgl.SetIsOnWithoutNotify(fadeOut);
        fadeInDuration.text = fadeInDur;
        fadeOutDuration.text = fadeOutDur;
    }

    private void Close()
    {
        Destroy(gameObject);
    }

    private void SaveBG()
    {
        OnUpdateBG?.Invoke(bgImage, fadeInTgl.isOn, fadeOutTgl.isOn, fadeInDuration.text, fadeOutDuration.text);
        bgImage = null;
    }

    private void OpenImagePicker()
    {
        var extensions = new[] {
            new ExtensionFilter("Image Files")
        };

        // opens native dialog, returns paths
        var paths = StandaloneFileBrowser.OpenFilePanel("Open Image", "", extensions, false);
        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            StartCoroutine(LoadImage(paths[0]));
        }
    }

    IEnumerator LoadImage(string path)
    {
        if (!File.Exists(path))
            yield break;

        byte[] bytes = File.ReadAllBytes(path);
        bgImage = null;
        bgImage = bytes;
        Texture2D tex = new(2, 2, TextureFormat.RGBA32, false);
        tex.LoadImage(bytes);
        bgPreview.texture = tex;
        yield return null;
    }
}
