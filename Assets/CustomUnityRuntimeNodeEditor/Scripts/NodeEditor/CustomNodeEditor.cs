using UnityEngine;
using RuntimeNodeEditor;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

namespace CustomNodeEditor
{
    [Serializable]
    public struct SeqData
    {
        public string currentNodeID;
        public string nextNodeID;
        public string index;
    }

    public class Seq
    {
        public SeqData[] seqDatas;
    }

    public class CustomNodeEditor : NodeEditor
    {

        [SerializeField] private Color bgColor;
        public TGraphComponent CreateGraph<TGraphComponent>(RectTransform holder, Color connectionColor, Sprite bgImage = null) where TGraphComponent : NodeGraph
        {
            var nodeGraph = base.CreateGraph<TGraphComponent>(holder, Color.red, connectionColor);

            holder.GetChild(0).GetComponent<RectTransform>().localScale = Vector3.one;

            Image bg = nodeGraph.background.GetComponent<Image>();

            if (bgImage != null)
            {
                bg.color = Color.white;
                bg.sprite = bgImage;
            }
            else
            {
                bg.color = bgColor;
            }
            return nodeGraph;
        }

        public override void StartEditor(NodeGraph graph)
        {
            base.StartEditor(graph);

            Graph.SetSize(Vector2.one * 20000);

            //Events.OnGraphPointerClickEvent += OnGraphPointerClick;
            Events.OnNodePointerClickEvent += OnNodePointerClick;
            Events.OnConnectionPointerClickEvent += OnNodeConnectionPointerClick;
            //Events.OnSocketConnect += OnConnect;
        }

        public void InitStartEndNodes()
        {
            Graph.Create("Nodes/start_node", new Vector2(-500, 0));
            Graph.Create("Nodes/end_node", new Vector2(500, 0));
        }

        public void AddTalkNode()
        {
            Graph.Create("Nodes/talk_node", Vector2.zero);
        }

        public void AddLogicNode()
        {
            Graph.Create("Nodes/logic_node", Vector2.zero);
        }

        public void AddBackgroundNode()
        {
            Graph.Create("Nodes/scene_node", Vector2.zero);
        }

        public void SaveGraph(string savePath)
        {
            Graph.SaveFile(savePath);
        }

        public void LoadGraph(string savePath)
        {
            Graph.Clear();
            Graph.LoadFile(savePath);
        }

        public void Clear()
        {
            Graph.Clear();
        }

        private void OnNodePointerClick(Node node, PointerEventData eventData)
        {
            if (eventData.clickCount == 2 && eventData.button.Equals(PointerEventData.InputButton.Left))
            {
                switch (node)
                {
                    case TalkNode talkNode:
                        talkNode.OpenTalkPoints();
                        break;
                    case SceneNode sceneNode:
                        sceneNode.OpenBackgroundSelector();
                        break;
                    case LogicNode logicNode:
                        logicNode.OpenLogicWindow();
                        break;
                }
            }
            else if (eventData.clickCount == 1 && eventData.button.Equals(PointerEventData.InputButton.Right))
            {
                ClearConnections(node);

            }
            else if (eventData.clickCount == 2 && eventData.button.Equals(PointerEventData.InputButton.Right))
            {
                if (node is not StartNode && node is not EndNode)
                    DeleteNode(node);
            }
        }

        private void OnNodeConnectionPointerClick(string connId, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                Disconnect(connId);
            }
        }

        //private void OnConnect(SocketInput arg1, SocketOutput arg2)
        //{
        //    //Graph.drawer.SetConnectionColor(arg2.connection.connId, Color.green);
        //}

        private void DeleteNode(Node node)
        {
            node.DeleteData();
            Graph.Delete(node);
        }

        private void DuplicateNode(Node node)
        {
            Graph.Duplicate(node);// Not used
        }

        private void Disconnect(string line_id)
        {
            Graph.Disconnect(line_id);
        }

        private void ClearConnections(Node node)
        {
            Graph.ClearConnectionsOf(node);
        }
    }
}

