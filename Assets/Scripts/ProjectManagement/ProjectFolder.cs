using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProjectFolder : Selectable, IPointerClickHandler
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text path;
    [SerializeField] private TMP_Text lastModified;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Button optionsBtn;
    [SerializeField] private Button showFolderBtn;
    [SerializeField] private Button removeProjectBtn;
    private ProjectInfo projectInfo;

    public void SetProjectInfo(ProjectInfo info)
    {
        projectInfo = info;
        title.text = projectInfo.name;
        path.text = projectInfo.path;
        lastModified.text = GetTimeAgo(projectInfo.lastModified);
        optionsPanel.SetActive(false);
        optionsBtn.onClick.AddListener(ToggleOptionsPanel);
        showFolderBtn.onClick.AddListener(ShowProjectFolder);
        removeProjectBtn.onClick.AddListener(RemoveProjectFromList);
    }


#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();

        if (!TryGetComponent<Image>(out var imageComponent))
        {
            imageComponent = gameObject.AddComponent<Image>();
        }

        targetGraphic = imageComponent;
    }
#endif

    protected override void OnDestroy()
    {
        optionsBtn.onClick.RemoveAllListeners();
        showFolderBtn.onClick.RemoveAllListeners();
        removeProjectBtn.onClick.RemoveAllListeners();
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            optionsPanel.SetActive(false);
            ProjectsManager.Instance.OpenEditProjectGraph(projectInfo);
            //Debugger.Log($"Open project {projectInfo.name}", name);
        }
        //else if (eventData.clickCount == 1 && eventData.button == PointerEventData.InputButton.Right)
        //{
        //    optionsPanel.SetActive(!optionsPanel.activeSelf);
        //}
    }

    //public override void OnDeselect(BaseEventData eventData)
    //{
    //    base.OnDeselect(eventData);
    //    Debugger.Log("OnDeselect");
    //    removePanel.SetActive(false);
    //}

    private void ToggleOptionsPanel()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

    private void ShowProjectFolder()
    {

    }

    private void RemoveProjectFromList()
    {
        ProjectsManager.Instance.RemoveProject(projectInfo);
        Destroy(gameObject);
    }

    private string GetTimeAgo(string dateTimeStr)
    {
        // Try to parse your input string into a DateTime
        if (!DateTime.TryParse(dateTimeStr, out DateTime modifiedTime))
            return "Invalid date";

        // If your input is in UTC and you want to compare to local time, adjust if needed
        DateTime now = DateTime.Now;
        TimeSpan diff = now - modifiedTime;

        double seconds = diff.TotalSeconds;
        double minutes = diff.TotalMinutes;
        double hours = diff.TotalHours;
        double days = diff.TotalDays;

        if (seconds < 60)
            return $"few seconds ago";
            //return $"{Math.Floor(seconds)} seconds ago";
        else if (minutes < 60)
            return $"few minutes ago";
            //return $"{Math.Floor(minutes)} minutes ago";
        else if (hours < 24)
            return $"{Math.Floor(hours)} hour{(hours >= 2 ? "s" : "")} ago";
        else if (days < 30)
            return $"{Math.Floor(days)} day{(days >= 2 ? "s" : "")} ago";
        else if (days < 365)
        {
            int months = (int)Math.Floor(days / 30);
            return $"{months} month{(months > 1 ? "s" : "")} ago";
        }
        else
        {
            int years = (int)Math.Floor(days / 365);
            return $"{years} year{(years > 1 ? "s" : "")} ago";
        }
    }


}
