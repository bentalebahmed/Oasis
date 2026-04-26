using System.IO;
using RuntimeNodeEditor;
using UnityEngine;

namespace CustomNodeEditor
{
    public class NodeEditorManager : MonoBehaviour
    {
        [Header("Editor params")]
        [SerializeField] private RectTransform editorHolder;
        [SerializeField] private CustomNodeEditor editor;
        [SerializeField] private GameObject nodesPanel;
        private string currentProjectPath;

        private void Awake()
        {
            currentProjectPath = string.Empty;

            // Set to windowed mode
            Screen.fullScreenMode = FullScreenMode.Windowed;

            // Get display resolution
            int width = Display.main.systemWidth;
            int height = Display.main.systemHeight;

            // Set window to screen size (simulate "maximized")
            Screen.SetResolution(width, height, FullScreenMode.Windowed);

            nodesPanel.SetActive(false);

            Color lineColor = ColorUtility.TryParseHtmlString("#939393", out var c) ? c : new Color(147 / 255, 147 / 255, 147 / 255);
            var graph = editor.CreateGraph<NodeGraph>(editorHolder, lineColor);
            editor.StartEditor(graph);

        }
       

        public void InitGraph2Edit(string projectPath)
        {
            currentProjectPath = projectPath;
            string graphPath = Path.Combine(currentProjectPath, "graph.json");
            string file;
            
            try
            {
                file = File.ReadAllText(graphPath);
            }
            catch (System.Exception)
            {
                file = string.Empty;
            }
            

            if(file.Length == 0)
            {
                editor.InitStartEndNodes();
                return;
            }

            editor.LoadGraph(graphPath);
        }       

        public string GetCurrentResourcesPath()
        {
            if (string.IsNullOrEmpty(currentProjectPath))
            {
                ///TODO: notif could not save project
                Debugger.Warning("Could not save project", name);
                return string.Empty;
            }

            return Path.Combine(currentProjectPath, "Resources");
        }

        public string GetCurrentPath()
        {
            if (string.IsNullOrEmpty(currentProjectPath))
            {
                ///TODO: notif could not save project
                Debugger.Warning("Could not save project", name);
                return string.Empty;
            }

            return currentProjectPath;
        }

        public string SaveGraph()
        {
            if (string.IsNullOrEmpty(currentProjectPath))
            {
                //TODO: notif could not save project
                Debugger.Warning("Could not save project", name);
                return string.Empty;
            }

            string graphPath = Path.Combine(currentProjectPath, "graph.json");
            editor.SaveGraph(graphPath);

            return currentProjectPath;
        }

        public void ExitEditor()
        {
            currentProjectPath = string.Empty;
            editor.Clear();
        }

        public void ToggleNodesPanel()
        {
            nodesPanel.SetActive(!nodesPanel.activeSelf);
        }

        public void AddBGNode()
        {
            editor.AddBackgroundNode();
        }

        public void AddTalkNode()
        {
            editor.AddTalkNode();
        }

        public void AddLogicNode()
        {
            editor.AddLogicNode();
        }



    }
}
