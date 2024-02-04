using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml;
using System.IO;

public class DialogueManager : MonoBehaviour
{

    public ScrollRect scrollRect;
    public ButtonComponent[] buttonText;
    public string folder = "Russian";
    public int offset = 20;

    private string fileName, lastName;
    private List<Dialogue> node;
    private Dialogue dialogue;
    private Answer answer;
    private float curY, height;
    private static DialogueManager _internal;
    private int id;
    private static bool _active;

    public void DialogueStart(string name)
    {
        if (name == string.Empty) return;
        fileName = name;
        Load();
    }

    public static DialogueManager Internal
    {
        get { return _internal; }
    }

    public static bool isActive
    {
        get { return _active; }
    }

    void Awake()
    {
        _internal = this;
        CloseWindow();
    }

    void Load()
    {
        if (lastName == fileName)
        {
            BuildDialogue(0);
            return;
        }

        node = new List<Dialogue>();

        try
        {
            TextAsset binary = Resources.Load<TextAsset>(folder + "/" + fileName);
            XmlTextReader reader = new(new StringReader(binary.text));

            int index = 0;
            while (reader.Read())
            {
                if (reader.IsStartElement("node"))
                {
                    dialogue = new Dialogue();
                    dialogue.answer = new List<Answer>();
                    dialogue.npcText = reader.GetAttribute("npcText");
                    dialogue.id = GetINT(reader.GetAttribute("id"));
                    node.Add(dialogue);

                    XmlReader inner = reader.ReadSubtree();
                    while (inner.ReadToFollowing("answer"))
                    {
                        answer = new Answer();
                        answer.text = reader.GetAttribute("text");
                        answer.toNode = GetINT(reader.GetAttribute("toNode"));
                        answer.exit = GetBOOL(reader.GetAttribute("exit"));
                        answer.questStatus = GetINT(reader.GetAttribute("questStatus"));
                        answer.questValue = GetINT(reader.GetAttribute("questValue"));
                        answer.questValueGreater = GetINT(reader.GetAttribute("questValueGreater"));
                        answer.questName = reader.GetAttribute("questName");
                        node[index].answer.Add(answer);
                    }
                    inner.Close();

                    index++;
                }
            }

            lastName = fileName;
            reader.Close();
        }
        catch (System.Exception error)
        {
            Debug.Log(this + " ошибка чтения файла диалога: " + fileName + ".xml | Error: " + error.Message);
            CloseWindow();
            lastName = string.Empty;
        }

        BuildDialogue(0);
    }

    void AddToList(bool exit, int toNode, string text, int questStatus, string questName, bool isActive)
    {
        buttonText[id].text.text = text;
        buttonText[id].rect.sizeDelta = new Vector2(buttonText[id].rect.sizeDelta.x, buttonText[id].text.preferredHeight + offset);
        buttonText[id].button.interactable = isActive;
        height = buttonText[id].rect.sizeDelta.y;
        buttonText[id].rect.anchoredPosition = new Vector2(0, -height / 2 - curY);

        if (exit)
        {
            SetExitDialogue(buttonText[id].button);
            if (questStatus != 0) SetQuestStatus(buttonText[id].button, questStatus, questName);
        }
        else
        {
            SetNextNode(buttonText[id].button, toNode);
            if (questStatus != 0) SetQuestStatus(buttonText[id].button, questStatus, questName);
        }

        id++;

        curY += height + offset;
        RectContent();
    }

    void RectContent()
    {
        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, curY);
        scrollRect.content.anchoredPosition = Vector2.zero;
    }

    void ClearDialogue()
    {
        id = 0;
        curY = offset;
        foreach (ButtonComponent b in buttonText)
        {
            b.text.text = string.Empty;
            b.rect.sizeDelta = new Vector2(b.rect.sizeDelta.x, 0);
            b.rect.anchoredPosition = new Vector2(b.rect.anchoredPosition.x, 0);
            b.button.onClick.RemoveAllListeners();
        }
        RectContent();
    }

    void SetQuestStatus(Button button, int i, string name)
    {
        string t = name + "|" + i;
        button.onClick.AddListener(() => QuestStatus(t));
    }

    void SetNextNode(Button button, int i)
    {
        button.onClick.AddListener(() => BuildDialogue(i));
    }

    void SetExitDialogue(Button button)
    {
        button.onClick.AddListener(() => CloseWindow());
    }

    void QuestStatus(string s)
    {
        string[] t = s.Split(new char[] { '|' });

        if (t[1] == "1")
        {
            QuestManager.SetQuestStatus(t[0], QuestManager.Status.Active);
        }
        else if (t[1] == "2")
        {
            QuestManager.SetQuestStatus(t[0], QuestManager.Status.Disable);
        }
        else if (t[1] == "3")
        {
            QuestManager.SetQuestStatus(t[0], QuestManager.Status.Complete);
        }
    }

    void CloseWindow()
    {
        _active = false;
        scrollRect.gameObject.SetActive(false);
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    void ShowWindow()
    {
        scrollRect.gameObject.SetActive(true);
        _active = true;
    }

    int FindNodeByID(int i)
    {
        int j = 0;
        foreach (Dialogue d in node)
        {
            if (d.id == i) return j;
            j++;
        }

        return -1;
    }

    void BuildDialogue(int current)
    {
        ClearDialogue();

        int j = FindNodeByID(current);

        if (j < 0)
        {
            Debug.LogError(this + " в диалоге [" + fileName + ".xml] отсутствует или указан неверно идентификатор узла.");
            return;
        }

        AddToList(false, 0, node[j].npcText, 0, string.Empty, false);

        for (int i = 0; i < node[j].answer.Count; i++)
        {
            int value = QuestManager.GetCurrentValue(node[j].answer[i].questName);


            if (value >= node[j].answer[i].questValueGreater && node[j].answer[i].questValueGreater != 0 ||
                node[j].answer[i].questValue == value && node[j].answer[i].questValueGreater == 0 ||
                node[j].answer[i].questName == null)
            {
                AddToList(node[j].answer[i].exit, node[j].answer[i].toNode, node[j].answer[i].text, node[j].answer[i].questStatus, node[j].answer[i].questName, true);
            }
        }

        EventSystem.current.SetSelectedGameObject(scrollRect.gameObject);
        ShowWindow();
    }

    int GetINT(string text)
    {
        int value;
        if (int.TryParse(text, out value))
        {
            return value;
        }
        return 0;
    }

    bool GetBOOL(string text)
    {
        bool value;
        if (bool.TryParse(text, out value))
        {
            return value;
        }
        return false;
    }
}

class Dialogue
{
    public int id;
    public string npcText;
    public List<Answer> answer;
}

class Answer
{
    public string text, questName;
    public int toNode, questValue, questValueGreater, questStatus;
    public bool exit;
}