using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateNewProjectInterface : MonoBehaviour
{
    [SerializeField] private TMP_InputField projectRootFolderPath;
    [SerializeField] private TMP_InputField projectName;
    [SerializeField] private Button confirm;
    [SerializeField] private Button selectFolder;
    [SerializeField] private Button cancel;
    [HideInInspector] public Action<string, string> OnConfirmCreateProject;


    private void OnEnable()
    {
        projectRootFolderPath.onValueChanged.AddListener(_ => ValidateInputs());
        projectName.onValueChanged.AddListener(_ => ValidateInputs());
        confirm.onClick.AddListener(CreateProject);
        selectFolder.onClick.AddListener(SelectFolder);
        cancel.onClick.AddListener(Close);
        confirm.interactable = false;
    }

    private void OnDisable()
    {
        projectRootFolderPath.onValueChanged.RemoveAllListeners();
        projectName.onValueChanged.RemoveAllListeners();
        confirm.onClick.RemoveAllListeners();
        selectFolder.onClick.RemoveAllListeners();
        cancel.onClick.RemoveAllListeners();
        confirm.interactable = false;
        OnConfirmCreateProject = null;
    }


    private void ValidateInputs()
    {
        bool valid = !string.IsNullOrEmpty(projectName.text) && !string.IsNullOrEmpty(projectRootFolderPath.text);
        confirm.interactable = valid;
    }

    private void SelectFolder()
    {
        StandaloneFileBrowser.OpenFolderPanelAsync("Select Project Folder", "", false, SetNewProjectFolderPath);
    }

    private void SetNewProjectFolderPath(string[] paths)
    {
        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            projectRootFolderPath.text = paths[0];
            ValidateInputs();
        }
    }

    private void CreateProject()
    {
        if (string.IsNullOrEmpty(projectRootFolderPath.text) || string.IsNullOrEmpty(projectName.text))
        {
            Debugger.Log("CreateProject: folder path or name not valid");
            return;// TODO: Notif as folder path or name not valid
        }

        OnConfirmCreateProject?.Invoke(projectRootFolderPath.text, projectName.text);

        Close();
    }

    private void Close()
    {
        Destroy(gameObject);
    }

}

