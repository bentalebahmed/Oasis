using System.Collections.Generic;
using UnityEngine;
using RuntimeNodeEditor;
using CustomNodeEditor;

public class StartNode : Node
{
    [SerializeField] private SocketOutput output;

    public override void Setup()
    {
        Register(output);

        SetHeader("START");


        SeqData newSeqData = new()
        {
            currentNodeID = "start",
            nextNodeID = string.Empty
        };

        output.SetValue(new List<SeqData> { newSeqData });
    }
}
