using System.Collections.Generic;
using UnityEngine;
using RuntimeNodeEditor;
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
        if (!_incomingOutputs.Contains(output))
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
            if (output == null) continue;

            var value = output.GetValue<List<SeqData>>();
            if (value == null || value.Count == 0) continue;

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

        if (mergedData.Count == 0)
        {
            ProjectsManager.Instance.SaveResourcesFile("seq.json", JsonUtility.ToJson("", true));
            return;
        }

        HashSet<string> seen = new();
        List<SeqData> uniqueList = new();

        foreach (var entry in mergedData)
        {
            string key = $"{entry.currentNodeID}-{entry.nextNodeID}";
            if (seen.Add(key)) // only adds it if not already seen
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
