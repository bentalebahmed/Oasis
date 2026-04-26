using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomNodeEditor;
using RuntimeNodeEditor;
using SFB;
using TMPro;
using UnityEngine;

[Serializable]
public class ProjectInfo
{
    public string name;
    public string path;
    public string createdDate;
    public string lastModified;
}


public class ProjectsManager : MonoBehaviour
{
    public static ProjectsManager Instance { get; private set; }

    [SerializeField] private NodeEditorManager nodeEditorManager;
    [SerializeField] private RectTransform mainWindow;
    [SerializeField] private RectTransform nodeEditorWindow;
    [SerializeField] private RectTransform projectFoldersHolder;
    [SerializeField] private TMP_Text projectTitle;
    [HideInInspector] public Action OnSaveProject;
    private string appDataPath;
    private List<ProjectInfo> Projects;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        mainWindow.gameObject.SetActive(true);
        nodeEditorWindow.gameObject.SetActive(false);

        appDataPath = Path.Combine(Application.persistentDataPath, "projects_list.json");
        Debugger.Log($"Projects list path: {appDataPath}", "ProjectsManager");
        LoadProjects();
    }

    public void LoadExistingProject()
    {
        StandaloneFileBrowser.OpenFolderPanelAsync("Select Existing Project Folder", "", false, CheckExistingProject);

    }


    private void CheckExistingProject(string[] paths)
    {
        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            string graphPath = Path.Combine(paths[0], "graph.json");
            if (!File.Exists(graphPath))
            {
                // TODO: add notif that the project is not valid, no graph data
                Debugger.Warning("project is not valid, no graph data", name);
                return;
            }

            bool alreadyExists = Projects.Any(p => p.path == paths[0]);

            if (alreadyExists)
            {
                // TODO: add notif that the project is already added
                Debugger.Warning("project already exists", name);
                return;
            }

            var projectInfo = new ProjectInfo
            {
                name = Path.GetFileName(paths[0]),
                path = paths[0],
                createdDate = DateTime.Now.ToString(),
                lastModified = DateTime.Now.ToString(),
            };

            AddProject(projectInfo);
            //OpenEditProjectGraph(projectInfo);
        }
    }


    public void ShowCreateNewProjectInterface()
    {
        CreateNewProjectInterface newProjectInterface = Utility.CreatePrefab<CreateNewProjectInterface>("Interfaces/CreateNewProjectInterface", mainWindow);

        if (newProjectInterface)
        {
            newProjectInterface.OnConfirmCreateProject += CreateNewProject;
        }
    }


    private void CreateNewProject(string projectRootFolder, string projectName)
    {
        string fullPath = Path.Combine(projectRootFolder, projectName);

        if (Directory.Exists(fullPath))
        {
            // TODO: add notif that the project already exists, and if you want to override it
            Debugger.Warning("project already exists", name);
            return;
        }

        Directory.CreateDirectory(fullPath);

        var projectInfo = new ProjectInfo
        {
            name = projectName,
            path = fullPath,
            createdDate = DateTime.Now.ToString(),
            lastModified = DateTime.Now.ToString(),
        };

        File.Create(Path.Combine(fullPath, "graph.json"));
        Directory.CreateDirectory(Path.Combine(fullPath, "Resources"));

        AddProject(projectInfo);
        OpenEditProjectGraph(projectInfo);
    }

    private void LoadProjects()
    {
        if (!File.Exists(appDataPath))
        {
            Projects = new List<ProjectInfo>();
            return;
        }

        string json = File.ReadAllText(appDataPath);
        var wrapper = JsonUtility.FromJson<Wrapper<ProjectInfo>>(json);
        Projects = wrapper?.list ?? new List<ProjectInfo>();

        Debugger.Log($"Projects {Projects.Count}", name);

        foreach (Transform pFolder in projectFoldersHolder.transform)
        {
            Destroy(pFolder.gameObject);
        }

        List<ProjectInfo> toRemove = new();
        foreach (var pInfo in Projects)
        {
            string graphPath = Path.Combine(pInfo.path, "graph.json");
            if (File.Exists(graphPath))
            {
                ProjectFolder projectFolder = Utility.CreatePrefab<ProjectFolder>("Interfaces/ProjectFolder", projectFoldersHolder);
                if (projectFolder)
                {
                    projectFolder.SetProjectInfo(pInfo);
                }
            }
            else
            {
                toRemove.Add(pInfo);
            }
        }

        if (toRemove.Count > 0)
            foreach (var pInfo in toRemove)
                RemoveProject(pInfo);

    }

    private void AddProject(ProjectInfo project)
    {
        Projects.Add(project);
        SaveProjects();
    }

    public void RemoveProject(ProjectInfo project)
    {
        Projects.Remove(project);
        SaveProjects();
    }

    private void SaveProjects()
    {
        Projects = Projects.OrderByDescending(p =>
         {
             DateTime.TryParse(p.lastModified, out DateTime parsedDate);
             return parsedDate;
         }).ToList();

        string json = JsonUtility.ToJson(new Wrapper<ProjectInfo> { list = Projects }, true);
        File.WriteAllText(appDataPath, json);
        Debugger.Log($"Project list saved at: {appDataPath}", name);
        LoadProjects();
    }

    public void OpenEditProjectGraph(ProjectInfo projectInfo)
    {
        projectTitle.text = projectInfo.name;
        mainWindow.gameObject.SetActive(false);
        nodeEditorWindow.gameObject.SetActive(true);
        nodeEditorManager.InitGraph2Edit(projectInfo.path);
    }

    public void SaveEditedProjectGraph()
    {
        string projectPath = nodeEditorManager.SaveGraph();

        OnSaveProject?.Invoke();

        if (string.IsNullOrEmpty(projectPath))
        {
            return;
        }

        ProjectInfo currentProject = Projects.FirstOrDefault(p => p.path == projectPath);

        if (currentProject == null)
        {
            //TODO: notif could not save project
            Debugger.Warning("Could not save project", name);
            return;
        }

        currentProject.lastModified = DateTime.Now.ToString();
        SaveProjects();

    }

    public void ExitEditor()
    {
        SaveEditedProjectGraph();
        mainWindow.gameObject.SetActive(true);
        nodeEditorWindow.gameObject.SetActive(false);
        nodeEditorManager.ExitEditor();
    }

    public void SaveResourcesFile(string fileName, byte[] file)
    {
        string pPath = nodeEditorManager.GetCurrentResourcesPath();
        if (!string.IsNullOrEmpty(pPath))
        {
            File.WriteAllBytes(Path.Combine(pPath, fileName), file);
        }
    }

    public void SaveResourcesFile(string fileName, string file)
    {
        string pPath = nodeEditorManager.GetCurrentResourcesPath();
        if (!string.IsNullOrEmpty(pPath))
        {
            File.WriteAllText(Path.Combine(pPath, fileName), file);
        }
    }

    public byte[] LoadResourcesFile(string fileName)
    {
        string pPath = nodeEditorManager.GetCurrentResourcesPath();
        if (File.Exists(Path.Combine(pPath, fileName)))
        {
            return File.ReadAllBytes(Path.Combine(pPath, fileName));
        }

        return null;
    }

    public string LoadSeqFile()
    {
        string pPath = nodeEditorManager.GetCurrentResourcesPath();
        if (File.Exists(Path.Combine(pPath, "seq.json")))
        {
            return File.ReadAllText(Path.Combine(pPath, "seq.json"));
        }

        return null;
    }

    public string LoadGraphFile()
    {
        string pPath = nodeEditorManager.GetCurrentPath();
        if (File.Exists(Path.Combine(pPath, "graph.json")))
        {
            return File.ReadAllText(Path.Combine(pPath, "graph.json"));
        }

        return null;
    }

    public void DeleteResourcesFile(string fileName)
    {
        string pPath = nodeEditorManager.GetCurrentResourcesPath();
        if (File.Exists(Path.Combine(pPath, fileName)))
        {
            File.Delete(Path.Combine(pPath, fileName));
        }
    }

    [Serializable]
    private class Wrapper<T>
    {
        public List<T> list;
    }
}
