using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeNodeEditor;
using System.Linq;
using UnityEngine.UI;
using CustomNodeEditor;

public class EndNode : Node
{
    [SerializeField] private SocketInput input;

    List<IOutput> _incomingOutputs = new();
    //private IOutput _incomingOutputs;

    public override void Setup()
    {
        Register(input);

        SetHeader("END");

        OnConnectionEvent += OnConnection;
        OnDisconnectEvent += OnDisconnect;

        ProjectsManager.Instance.OnSaveProject += OnConnectedValueUpdated;
    }


    private void OnConnection(SocketInput input, IOutput output)
    {
        //output.ValueUpdated += OnConnectedValueUpdated;
        if(!_incomingOutputs.Contains(output))
            _incomingOutputs.Add(output);

        //_incomingOutputs = output;

        //OnConnectedValueUpdated();
    }

    private void OnDisconnect(SocketInput input, IOutput output)
    {
        //output.ValueUpdated -= OnConnectedValueUpdated;
        _incomingOutputs.Remove(output);
        //_incomingOutputs = null;
        //OnConnectedValueUpdated();
    }

    private void OnConnectedValueUpdated()
    {
        if (_incomingOutputs == null || _incomingOutputs.Count == 0)
        {
            ProjectsManager.Instance.SaveResourcesFile("seq.json", JsonUtility.ToJson("", true));
            return;
        }

        List<SeqData> mergedData = new();

        foreach (var output in _incomingOutputs) 
        {
            if(output==null) continue;

            var value = output.GetValue<List<SeqData>>();
            if(value == null || value.Count == 0) continue;

            var copy = new List<SeqData>(value);

            //copy[^1].nextNodeID = ID;

            SeqData temp = copy[^1];
            temp.nextNodeID = ID;
            copy[^1] = temp;

            SeqData endSeq = new SeqData
            {
                currentNodeID = ID,
                nextNodeID = "end"
            };

            copy.Add(endSeq);

            mergedData.AddRange(copy);
        }

        if(mergedData.Count == 0)
        {
            ProjectsManager.Instance.SaveResourcesFile("seq.json", JsonUtility.ToJson("", true));
            return;
        }

        HashSet<string> seen = new();
        List<SeqData> uniqueList = new();

        foreach (var entry in mergedData)
        {
            string key = $"{entry.currentNodeID}-{entry.nextNodeID}";
            if (seen.Add(key)) // only adds if not already seen
            {
                uniqueList.Add(entry);
            }
        }

        Seq seq = new()
        {
            seqDatas = uniqueList.ToArray()
        };

        ProjectsManager.Instance.SaveResourcesFile("seq.json", JsonUtility.ToJson(seq, true));

    }


}

//List<SeqData> value = _incomingOutputs.GetValue<List<SeqData>>();

//if (value == null)
//{
//    ProjectsManager.Instance.SaveResourcesFile("seq.json", JsonUtility.ToJson("", true));
//    return;
//}

//var lastSeqDataList = new List<SeqData>(value);

//lastSeqDataList[^1].nextNodeID = ID;

//SeqData seqData = new()
//{
//    currentNodeID = "end",
//    nextNodeID = string.Empty
//};

//lastSeqDataList.Add(seqData);

//Seq seq = new()
//{
//    seqDatas = lastSeqDataList.ToArray()
//};

//ProjectsManager.Instance.SaveResourcesFile("seq.json", JsonUtility.ToJson(seq, true));


//if (_incomingOutputs == null)
//{
//    return;
//}

//object inputValue = _incomingOutputs.GetValue<object>();

//List<string> jsonList;

//if (inputValue is List<string> list)
//{
//    if (list.Count == 1 && list[0] == "\"start\"")
//    {
//        Debugger.Warning("Do not connect a start node to an end node");
//        return;
//    }

//    // Copy the list
//    jsonList = new List<string>(list);
//}
//else
//{
//    jsonList = new List<string>();
//    Debugger.Warning($"EndNode received unexpected input type: {inputValue?.GetType()}", "EndNode");
//}

//// Add the end marker
//jsonList.Add("\"end\"");


//string combined = string.Join("\n---\n", jsonList);
//Debugger.Log(combined, "EndNode");
