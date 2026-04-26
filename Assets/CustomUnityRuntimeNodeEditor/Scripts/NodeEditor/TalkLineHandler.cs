using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalkLineHandler : MonoBehaviour
{
    [SerializeField] private Button deleteBt;
    [SerializeField] private TMP_InputField text;

    public string Text { get => string.IsNullOrEmpty(text.text) ? string.Empty : text.text; 
                            set => text.text = value; }

    private void OnEnable()
    {
        deleteBt.onClick.AddListener(Delete);
    }

    private void OnDisable()
    {
        deleteBt.onClick.RemoveAllListeners();
    }

    private void Delete()
    {
        Destroy(gameObject);
    }   
}
