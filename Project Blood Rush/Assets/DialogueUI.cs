using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    //public GameObject panel;
    public TMP_Text text;

    [SerializeField] private float speed = 0.03f;

    private Coroutine typing;
    private bool isTyping;
    private string currentLine;

    void Awake()
    {
        Instance = this;
        //panel.SetActive(false);
    }

    public void ShowLine(string line)
    {
       // panel.SetActive(true);
        currentLine = line;

        if (typing != null) StopCoroutine(typing);
        typing = StartCoroutine(Type(line));
    }

    IEnumerator Type(string line)
    {
        text.text = "";
        isTyping = true;

        foreach (char c in line)
        {
            text.text += c;
            yield return new WaitForSeconds(speed);
        }

        isTyping = false;
    }

    public void Skip()
    {
        if (!isTyping) return;

        StopCoroutine(typing);
        text.text = currentLine;
        isTyping = false;
    }

    public void Hide()
    {
       // panel.SetActive(false);
    }

    public bool IsTyping() => isTyping;
}