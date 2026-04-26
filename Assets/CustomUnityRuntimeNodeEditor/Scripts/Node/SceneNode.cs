using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeNodeEditor;
using UnityEngine.UI;
using System;
using CustomNodeEditor;

public class SceneNode : Node
{
    [SerializeField] private RawImage preview;
    [SerializeField] private SocketInput input;
    [SerializeField] private SocketOutput output;
    private RectTransform rootRectTransform;
    private string isFadeIn;
    private string isFadeOut;
    private string fadeInDuration;
    private string fadeOutDuration;

    private IOutput _incomingOutputs;
    // Start is called before the first frame update
    public override void Setup()
    {
        Register(input);
        Register(output);

        SetHeader("BACKGROUND");

        OnConnectionEvent += OnConnection;
        OnDisconnectEvent += OnDisconnect;

        rootRectTransform = GetComponentInParent<Canvas>().rootCanvas.GetComponent<RectTransform>();

        output.SetValue(null);

        isFadeIn = "0";
        isFadeOut = "0";
        fadeInDuration = "1";
        fadeOutDuration = "1";

    }

    public void OpenBackgroundSelector()
    {
        SceneBGWindow sceneBGWindow = Utility.CreatePrefab<SceneBGWindow>("Interfaces/SceneBGWindow", rootRectTransform);
        if (sceneBGWindow)
        {
            sceneBGWindow.OnUpdateBG += UpdateBG;

            byte[] bgImage = ProjectsManager.Instance.LoadResourcesFile(ID + ".bg");
            sceneBGWindow.SetData(bgImage, isFadeIn == "1", isFadeOut == "1", fadeInDuration, fadeOutDuration);
        }
    }

    private void UpdateBG(byte[] bgImage, bool _isFadeIn, bool _isFadeOut, string _fadeInDuration, string _fadeOutDuration)
    {
        isFadeIn = _isFadeIn ? "1" : "0";
        isFadeOut = _isFadeOut ? "1" : "0";
        fadeInDuration = _fadeInDuration;
        fadeOutDuration = _fadeOutDuration;

        //Debugger.Log($"UpdateBG  {_isFadeIn}  {_isFadeOut} {_fadeInDuration}");

        if (bgImage != null)
        {
            Texture2D tex = new(2, 2, TextureFormat.RGBA32, false);
            tex.LoadImage(bgImage);
            preview.texture = tex;
            preview.color = Color.white;

            ProjectsManager.Instance.SaveResourcesFile(ID + ".bg", bgImage);
        }



    }

    private void OnConnection(SocketInput input, IOutput output)
    {
        output.ValueUpdated += OnConnectedValueUpdated;
        //_incomingOutputs.Add(output);
        _incomingOutputs = output;

        OnConnectedValueUpdated();
    }

    private void OnDisconnect(SocketInput input, IOutput output)
    {
        output.ValueUpdated -= OnConnectedValueUpdated;
        //_incomingOutputs.Remove(output);
        _incomingOutputs = null;

        OnConnectedValueUpdated();
    }

    private void OnConnectedValueUpdated()
    {
        if (_incomingOutputs == null)
        {
            output.SetValue(null);
            return;
        }

        List<SeqData> value = _incomingOutputs.GetValue<List<SeqData>>();

        if (value == null)
        {
            output.SetValue(null);
            return;
        }

        var lastSeqDataList = new List<SeqData>(value);

        //lastSeqDataList[^1].nextNodeID = ID;

        SeqData temp = lastSeqDataList[^1];
        temp.nextNodeID = ID;
        lastSeqDataList[^1] = temp;

        SeqData seqData = new()
        {
            currentNodeID = ID,
            nextNodeID = string.Empty
        };

        lastSeqDataList.Add(seqData);

        output.SetValue(lastSeqDataList);
    }

    public override void OnSerialize(Serializer serializer)
    {
        serializer.Add("bgPath", ID + ".bg");
        serializer.Add("fadeIn", isFadeIn);
        serializer.Add("fadeOut", isFadeOut);
        serializer.Add("fadeInDuration", fadeInDuration);
        serializer.Add("fadeOutDuration", fadeOutDuration);
    }

    public override void OnDeserialize(Serializer serializer)
    {
        byte[] bgImage = ProjectsManager.Instance.LoadResourcesFile(ID + ".bg");

        if (bgImage != null)
        {
            Texture2D tex = new(2, 2, TextureFormat.RGBA32, false);
            tex.LoadImage(bgImage);
            preview.texture = tex;
            preview.color = Color.white;
        }

        isFadeIn = string.IsNullOrEmpty(serializer.Get("fadeIn")) ? "0" : serializer.Get("fadeIn");
        isFadeOut = string.IsNullOrEmpty(serializer.Get("fadeOut")) ? "0" : serializer.Get("fadeOut");
        fadeInDuration = string.IsNullOrEmpty(serializer.Get("fadeInDuration")) ? "1" : serializer.Get("fadeInDuration");
        fadeOutDuration = string.IsNullOrEmpty(serializer.Get("fadeOutDuration")) ? "1" : serializer.Get("fadeOutDuration");

        //Debugger.Log($"OnDeserialize {isFadeIn} {fadeInDuration}");
    }

    public override void DeleteData()
    {
        ProjectsManager.Instance.DeleteResourcesFile(ID + ".bg");
    }
}
