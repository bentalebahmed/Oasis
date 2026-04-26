using System.Collections;
using System.Collections.Generic;
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
            //Color color = ColorUtility.TryParseHtmlString("#2E333F", out var c) ? c : Color.gray;
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
                //bg.color = color;
            }

            return nodeGraph;
        }

        public override void StartEditor(NodeGraph graph)
        {
            base.StartEditor(graph);
            // TODO: add initialization

            Graph.SetSize(Vector2.one * 20000);

            Events.OnGraphPointerClickEvent += OnGraphPointerClick;
            Events.OnNodePointerClickEvent += OnNodePointerClick;
            Events.OnConnectionPointerClickEvent += OnNodeConnectionPointerClick;
            Events.OnSocketConnect += OnConnect;
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

        private void OnGraphPointerClick(PointerEventData eventdata)
        {
            //switch (eventdata.button)
            //{
            //    case PointerEventData.InputButton.Right:
            //        var ctx = new ContextMenuBuilder()
            //            .Add("Node/START", () => Graph.Create("Nodes/start_node"))
            //            .Add("Node/END", () => Graph.Create("Nodes/end_node"))
            //            .Add("Node/TALK", () => Graph.Create("Nodes/talk_node"))
            //            .Add("Node/SCENE", () => Graph.Create("Nodes/scene_node"))
            //            .Build();
            //        SetContextMenu(ctx);
            //        DisplayContextMenu();
            //        break;
            //    case PointerEventData.InputButton.Left:
            //        CloseContextMenu();
            //        break;
            //}
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
                //var ctx = new ContextMenuBuilder()
                //.Add("duplicate", () => DuplicateNode(node))
                //.Add("clear connections", () => ClearConnections(node))
                //.Add("delete", () => DeleteNode(node))
                //.Build();

                //SetContextMenu(ctx);
                //DisplayContextMenu();

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
                //var ctx = new ContextMenuBuilder()
                //.Add("clear connection", () => DisconnectConnection(connId))
                //.Build();

                //SetContextMenu(ctx);
                //DisplayContextMenu();

                DisconnectConnection(connId);

            }
        }

        private void OnConnect(SocketInput arg1, SocketOutput arg2)
        {
            //Graph.drawer.SetConnectionColor(arg2.connection.connId, Color.green);
        }

        private void DeleteNode(Node node)
        {
            node.DeleteData();
            Graph.Delete(node);
        }

        private void DuplicateNode(Node node)
        {
            Graph.Duplicate(node);
        }

        private void DisconnectConnection(string line_id)
        {
            Graph.Disconnect(line_id);
        }

        private void ClearConnections(Node node)
        {
            Graph.ClearConnectionsOf(node);
        }
    }
}

