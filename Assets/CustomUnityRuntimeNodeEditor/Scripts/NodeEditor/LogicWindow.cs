using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogicWindow : MonoBehaviour
{
    [HideInInspector] public Action<string, string, string> OnSaveData;

    [SerializeField] private TMP_InputField question;
    [SerializeField] private TMP_InputField trueWord;
    [SerializeField] private TMP_InputField falseWord;
    [SerializeField] private Button saveBt;
    [SerializeField] private Button closeBt;

    private void OnEnable()
    {
        saveBt.onClick.AddListener(SaveData);
        closeBt.onClick.AddListener(Close);
    }

    private void OnDisable()
    {
        saveBt.onClick.RemoveAllListeners();
        closeBt.onClick.RemoveAllListeners();
        OnSaveData = null;
    }

    public void SetData(string q, string tW, string fW)
    {
        question.text = q;
        trueWord.text = tW;
        falseWord.text = fW;
    }

    public void SaveData()
    {
        OnSaveData?.Invoke(question.text, trueWord.text, falseWord.text);
    }

    private void Close()
    {
        Destroy(gameObject);
    }
}
