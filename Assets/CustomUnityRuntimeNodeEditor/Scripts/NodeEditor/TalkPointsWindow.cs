using System;
using System.Collections;
using System.Collections.Generic;
using RuntimeNodeEditor;
using UnityEngine;
using UnityEngine.UI;

public class TalkPointsWindow : MonoBehaviour
{
    [HideInInspector] public Action<List<string>, bool> OnUpdateTalkLines;
    [SerializeField] private RectTransform scrollContent;
    [SerializeField] private Button addLineBt;
    [SerializeField] private Button saveBt;
    [SerializeField] private Button closeBt;
    [SerializeField] private Toggle clockToNxtTgl;

    private void OnEnable()
    {
        addLineBt.onClick.AddListener(AddNewLine);
        saveBt.onClick.AddListener(SaveLines);
        closeBt.onClick.AddListener(Close);
    }

    private void OnDisable()
    {
        addLineBt.onClick.RemoveAllListeners();
        saveBt.onClick.RemoveAllListeners();
        closeBt.onClick.RemoveAllListeners();
        OnUpdateTalkLines = null;

    }

    public void SetData(List<string> talkLines, bool isOn)
    {
        clockToNxtTgl.SetIsOnWithoutNotify(isOn);

        foreach (string line in talkLines)
            AddNewLine(line);
    }

    private void Close()
    {
        Destroy(gameObject);
    }

    private void AddNewLine()
    {
        Utility.CreatePrefab<GameObject>("Interfaces/TalkLine", scrollContent);
    }

    private void AddNewLine(string text)
    {
        TalkLineHandler tlh = Utility.CreatePrefab<TalkLineHandler>("Interfaces/TalkLine", scrollContent);
        if (tlh)
        {
            tlh.Text = text;
        }
    }

    private void SaveLines()
    {
        List<string> lines = new List<string>();
        foreach (Transform child in scrollContent.transform)
        {
            if (child.TryGetComponent(out TalkLineHandler lineHandler))
                if (!string.IsNullOrEmpty(lineHandler.Text))
                    lines.Add(lineHandler.Text);


        }
        // Action so the talk node gets the list of talk lines 
        OnUpdateTalkLines?.Invoke(lines, clockToNxtTgl.isOn);
    }
}
