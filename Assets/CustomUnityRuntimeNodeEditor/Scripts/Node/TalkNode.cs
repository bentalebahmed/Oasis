using System;
using System.Collections.Generic;
using CustomNodeEditor;
using RuntimeNodeEditor;
using UnityEngine;


public class TalkNode : Node
{
    [SerializeField] private SocketInput input;
    [SerializeField] private SocketOutput output;
    private RectTransform rootRectTransform;
    private string isClickToNxt;

    //private string speakerName;
    private List<string> talkLines = new();

    //private List<IOutput> _incomingOutputs;
    private IOutput _incomingOutputs;

    public override void Setup()
    {
        //speakerName = "Ahmed";

        Register(input);
        Register(output);

        SetHeader("TALK");

        OnConnectionEvent += OnConnection;
        OnDisconnectEvent += OnDisconnect;

        rootRectTransform = GetComponentInParent<Canvas>().rootCanvas.GetComponent<RectTransform>();

        output.SetValue(null);

        isClickToNxt = "0";
    }

    public void OpenTalkPoints()
    {
        TalkPointsWindow talkPointsWindow = Utility.CreatePrefab<TalkPointsWindow>("Interfaces/TalkPointsWindow", rootRectTransform);
        if (talkPointsWindow)
        {
            talkPointsWindow.OnUpdateTalkLines += UpdateTalkLines;

            talkPointsWindow.SetData(talkLines, isClickToNxt=="1");
        }
    }

    private void UpdateTalkLines(List<string> newLines, bool isClickNxt)
    {
        talkLines.Clear();

        talkLines = newLines;

        isClickToNxt = isClickNxt ? "1" : "0";
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
        string combined = string.Join("||", talkLines);
        serializer.Add("talkLines", combined);
        serializer.Add("isClick", isClickToNxt);
    }

    public override void OnDeserialize(Serializer serializer)
    {
        var value = serializer.Get("talkLines");
        if (!string.IsNullOrEmpty(value))
        {
            talkLines = null;
            talkLines = new List<string>(value.Split(new string[] { "||" }, StringSplitOptions.None));

        }

        value = serializer.Get("isClick");

        if (!string.IsNullOrEmpty(value))
        {
            isClickToNxt = value;
        }

    }


}