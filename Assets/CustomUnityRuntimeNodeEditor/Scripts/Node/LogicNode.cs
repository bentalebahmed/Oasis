using System;
using System.Collections;
using System.Collections.Generic;
using CustomNodeEditor;
using RuntimeNodeEditor;
using UnityEngine;

public class LogicNode : Node
{
    [SerializeField] private SocketInput input;
    [SerializeField] private SocketOutput trueOutput;
    [SerializeField] private SocketOutput falseOutput;
    private RectTransform rootRectTransform;

    private string question = string.Empty;
    private string trueWord = string.Empty;
    private string falseWord = string.Empty;

    //private List<IOutput> _incomingOutputs;
    private IOutput _incomingOutputs;

    public override void Setup()
    {
        Register(input);
        Register(trueOutput);
        Register(falseOutput);

        SetHeader("LOGIC");

        OnConnectionEvent += OnConnection;
        OnDisconnectEvent += OnDisconnect;

        rootRectTransform = GetComponentInParent<Canvas>().rootCanvas.GetComponent<RectTransform>();

        trueOutput.SetValue(null);
        falseOutput.SetValue(null);
    }

    public void OpenLogicWindow()
    {
        LogicWindow logicWindow = Utility.CreatePrefab<LogicWindow>("Interfaces/LogicWindow", rootRectTransform);
        if (logicWindow)
        {
            logicWindow.OnSaveData += UpdateData;
            logicWindow.SetData(question, trueWord, falseWord);
        }
    }

    private void UpdateData(string q, string tW, string fW)
    {
        question = q;
        trueWord = tW;
        falseWord = fW;
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
            trueOutput.SetValue(null);
            falseOutput.SetValue(null);
            return;
        }

        List<SeqData> value = _incomingOutputs.GetValue<List<SeqData>>();

        if (value == null)
        {
            trueOutput.SetValue(null);
            falseOutput.SetValue(null);
            return;
        }

        var trueOuputDatSeq = new List<SeqData>(value);
        var falseOuputDatSeq = new List<SeqData>(value);

        //trueOuputDatSeq[^1].nextNodeID = ID;
        //falseOuputDatSeq[^1].nextNodeID = ID;

        SeqData temp = trueOuputDatSeq[^1];
        temp.nextNodeID = ID;
        trueOuputDatSeq[^1] = temp;

        temp = falseOuputDatSeq[^1];
        temp.nextNodeID = ID;
        falseOuputDatSeq[^1] = temp;

        SeqData trueSeqData = new()
        {
            currentNodeID = ID,
            nextNodeID = string.Empty,
            index = "true"
        };

        SeqData falseSeqData = new()
        {
            currentNodeID = ID,
            nextNodeID = string.Empty,
            index = "false"
        };


        trueOuputDatSeq.Add(trueSeqData);
        falseOuputDatSeq.Add(falseSeqData);

        trueOutput.SetValue(trueOuputDatSeq);
        falseOutput.SetValue(falseOuputDatSeq);
    }

    public override void OnSerialize(Serializer serializer)
    {
        serializer.Add("question", question);
        serializer.Add("trueWord", trueWord);
        serializer.Add("falseWord", falseWord);
    }

    public override void OnDeserialize(Serializer serializer)
    {
        question = serializer.Get("question");
        trueWord = serializer.Get("trueWord");
        falseWord = serializer.Get("falseWord");
    }
}
