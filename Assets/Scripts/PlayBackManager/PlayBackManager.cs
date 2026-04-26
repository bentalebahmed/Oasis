using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomNodeEditor;
using RuntimeNodeEditor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayBackManager : MonoBehaviour
{
    [Header("Playback param")]
    [SerializeField] private RectTransform playBackWindow;
    [Header("talk param")]
    [SerializeField] private GameObject talkPreview;
    [SerializeField] private Button nextTalkBt;
    [SerializeField] private TMP_Text talkText;
    [SerializeField, Range(0.05f, 0.1f)] private float talkDelay = 0.08f;
    [Header("BG param")]
    [SerializeField] private RawImage bg;
    [SerializeField] private CanvasGroup fadePanel;
    [Header("Logic param")]
    [SerializeField] private GameObject logicPreview;
    [SerializeField] private TMP_Text logicQuestionText;
    [SerializeField] private Button logicTrueBt;
    [SerializeField] private Button logicFalseBt;
    private bool isLogicSet;
    private string logicVal;
    [SerializeField]
    private IEnumerator PlaybackCoroutine;
    private NodeData[] nodes;
    private SeqData[] seqNodes;
    private bool isNxtTalk;

    private void Awake()
    {
        HideAll();
    }

    public void Play()
    {
        Debugger.Log("Play experience");

        var graph = GetGraphData();
        var seq = ReadSeq();

        if (graph.nodes == null || seq.seqDatas == null)
        {
            // TODO: notif could not load experience data
            Debugger.Error("Couldn't load experience data");
            return;
        }


        nodes = graph.nodes;
        seqNodes = seq.seqDatas;

        SeqData endSeq = seqNodes.FirstOrDefault(data => data.nextNodeID == "end");
        if (seqNodes[0].currentNodeID != "start" || endSeq.Equals(default(SeqData)))
        {
            Debugger.Error("No start or end nodes");
            return;
        }

        if (PlaybackCoroutine != null)
        {
            StopCoroutine(PlaybackCoroutine);
            PlaybackCoroutine = null;
        }

        nextTalkBt.onClick.AddListener(OnNextTalk);

        logicTrueBt.onClick.AddListener(OnTrue);
        logicFalseBt.onClick.AddListener(OnFalse);

        PlaybackCoroutine = PlayExperiance();
        StartCoroutine(PlaybackCoroutine);
    }

    private void HideAll()
    {
        CanvasGroupOn(fadePanel);
        bg.texture = null;
        bg.color = Color.black;
        playBackWindow.gameObject.SetActive(false);
        talkPreview.SetActive(false);
        nextTalkBt.gameObject.SetActive(false);
        talkText.text = "";
        nextTalkBt.onClick.RemoveAllListeners();
        isNxtTalk = false;

        logicPreview.SetActive(false);
        logicTrueBt.onClick.RemoveAllListeners();
        logicFalseBt.onClick.RemoveAllListeners();
        isLogicSet = false;
        logicVal = string.Empty;

        if (PlaybackCoroutine != null)
        {
            isNxtTalk = true;
            StopCoroutine(PlaybackCoroutine);
            PlaybackCoroutine = null;
        }
    }

    private void CanvasGroupOn(CanvasGroup cg)
    {
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    private void CanvasGroupOff(CanvasGroup cg)
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    public void Close()
    {
        HideAll();
    }

    private GraphData GetGraphData()
    {
        var file = ProjectsManager.Instance.LoadGraphFile();

        if (file == null)
            return null;

        return JsonUtility.FromJson<GraphData>(file);
    }

    private Seq ReadSeq()
    {
        var file = ProjectsManager.Instance.LoadSeqFile();

        if (file == null)
            return null;

        return JsonUtility.FromJson<Seq>(file);
    }

    IEnumerator SetScene(string path, bool fadeIn, bool fadeOut, float fadeInDuration, float fadeOutDuration)
    {
        if (fadeIn)
        {
            CanvasGroupOff(fadePanel);
            float t = 0f;
            while (t < fadeInDuration)
            {
                t += Time.deltaTime;
                fadePanel.alpha = Mathf.Lerp(0f, 1f, t / fadeInDuration);
                yield return null;
            }

        }

        CanvasGroupOn(fadePanel);

        byte[] bgImage = ProjectsManager.Instance.LoadResourcesFile(path);

        if (bgImage != null)
        {
            Texture2D tex = new(2, 2, TextureFormat.RGBA32, false);
            tex.LoadImage(bgImage);
            bg.texture = tex;
            bg.color = Color.white;
        }

        if (fadeOut)
        {
            float t = 0f;
            while (t < fadeOutDuration)
            {
                t += Time.deltaTime;
                fadePanel.alpha = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
                yield return null;
            }
        }
        CanvasGroupOff(fadePanel);
    }

    private void DisplayTalk(string line)
    {
        Debugger.Log($"display talk : {line}");
        talkText.text = $"{line}";
    }

    private string GetNodeData(NodeData node, string key)
    {
        Serializer serializer = new();
        serializer.Deserialize(node.values);
        return serializer.Get(key);
    }

    private void OnNextTalk()
    {
        isNxtTalk = true;
    }

    private void OnTrue()
    {
        logicVal = "true";
        isLogicSet = true;
    }

    private void OnFalse()
    {
        logicVal = "false";
        isLogicSet = true;
    }

    IEnumerator PlayExperiance()
    {
        Debugger.Log($"Starting playback");
        playBackWindow.gameObject.SetActive(true);

        var currentSeqNode = seqNodes[0];
        NodeData currentNode = nodes.FirstOrDefault(node => node.id == currentSeqNode.nextNodeID);
        if (currentNode.id == null) currentSeqNode.nextNodeID = "end";
        bool isBranching = false;
        while (currentSeqNode.nextNodeID != "end")
        {
            //Debugger.Log($"Current Node: {currentNode.path.Replace("Nodes/", "")}");

            if (!isBranching) currentSeqNode = seqNodes.FirstOrDefault(node => node.currentNodeID == currentSeqNode.nextNodeID);
            isBranching = false;

            currentNode = nodes.FirstOrDefault(node => node.id == currentSeqNode.currentNodeID);
            if (currentNode.id == null)
            {
                currentSeqNode.nextNodeID = "end";
                yield break;
            }

            string type = currentNode.path.Replace("Nodes/", "");

            Debugger.Log($"current node: {type}");

            switch (type)
            {
                case "scene_node":
                    string bgPath = GetNodeData(currentNode, "bgPath");
                    bool fadeIn = GetNodeData(currentNode, "fadeIn") == "1";
                    bool fadeOut = GetNodeData(currentNode, "fadeOut") == "1";
                    float fadeInDuration = float.TryParse(GetNodeData(currentNode, "fadeInDuration"), out float t1) ? t1 : 1f;
                    float fadeOutDuration = float.TryParse(GetNodeData(currentNode, "fadeOutDuration"), out float t2) ? t2 : 1f;

                    if (!string.IsNullOrEmpty(bgPath))
                    {
                        yield return StartCoroutine(SetScene(bgPath, fadeIn, fadeOut, fadeInDuration, fadeOutDuration));
                    }
                    break;
                case "talk_node":
                    string rawLines = GetNodeData(currentNode, "talkLines");

                    if (!string.IsNullOrEmpty(rawLines))
                    {
                        string[] talkLines = rawLines.Split(new string[] { "||" }, StringSplitOptions.None);
                        bool isClick = GetNodeData(currentNode, "isClick") == "1";

                        nextTalkBt.gameObject.SetActive(isClick);

                        talkPreview.SetActive(talkLines.Length > 0);

                        foreach (string line in talkLines)
                        {
                            isNxtTalk = false;
                            DisplayTalk(line);

                            if (isClick) yield return new WaitUntil(() => isNxtTalk);
                            else yield return new WaitForSeconds(talkDelay * line.Length);

                            isNxtTalk = false;
                        }

                        talkPreview.SetActive(false);
                        talkText.text = "";
                    }
                    break;
                case "logic_node":

                    string question = GetNodeData(currentNode, "question");
                    logicQuestionText.text = question;
                    logicPreview.SetActive(true);

                    SeqData[] currentSeqNodes = seqNodes.Where(node => node.currentNodeID == currentSeqNode.currentNodeID).ToArray();
                    Dictionary<string, string> branches = null;

                    branches = currentSeqNodes
                                .Where(node => !string.IsNullOrEmpty(node.index))
                                .ToDictionary(node => node.index, node => node.nextNodeID);

                    //currentSeqNode = currentSeqNodes.FirstOrDefault(node => node.index == "false");
                    //foreach (var branch in branches)
                    //a Debugger.Log($"Branch {branch.Key}: {branch.Value}");

                    // Here create UI from the options and wait for user input to select the currentSeqNode
                    yield return new WaitUntil(() => isLogicSet);
                    currentSeqNode = seqNodes.FirstOrDefault(node => node.currentNodeID == branches[logicVal]);
                    isBranching = true;
                    isLogicSet = false;
                    logicPreview.SetActive(false);
                    break;

            }
        }
        yield return null;
        Close();
    }
}
