using System.Xml;
using UnityEngine;

public class DialogueGenerator : MonoBehaviour
{

    public string fileName = "Example";
    public string folder = "Russian";
    public DialogueNode[] node;

    public void Generate()
    {
        if (node.Length == 0) return;

        string path = Application.dataPath + "/Resources/" + folder + "/" + fileName + ".xml";

        XmlNode userNode;
        XmlElement element;

        XmlDocument xmlDoc = new XmlDocument();
        XmlNode rootNode = xmlDoc.CreateElement("dialogue");
        XmlAttribute attribute = xmlDoc.CreateAttribute("name");
        attribute.Value = fileName;
        rootNode.Attributes.Append(attribute);
        xmlDoc.AppendChild(rootNode);

        for (int j = 0; j < node.Length; j++)
        {
            userNode = xmlDoc.CreateElement("node");
            attribute = xmlDoc.CreateAttribute("id");
            attribute.Value = j.ToString();
            userNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute("npcText");
            attribute.Value = node[j].npcText;
            userNode.Attributes.Append(attribute);

            for (int i = 0; i < node[j].playerAnswer.Length; i++)
            {
                element = xmlDoc.CreateElement("answer");
                element.SetAttribute("text", node[j].playerAnswer[i].text);

                if (node[j].playerAnswer[i].exit)
                {
                    element.SetAttribute("exit", node[j].playerAnswer[i].exit.ToString());

                    if (node[j].playerAnswer[i].questStatus > 0 && node[j].playerAnswer[i].questName.Trim().Length > 0)
                    {
                        element.SetAttribute("questStatus", node[j].playerAnswer[i].questStatus.ToString());
                        element.SetAttribute("questName", node[j].playerAnswer[i].questName);
                    }
                }
                else
                {
                    element.SetAttribute("toNode", node[j].playerAnswer[i].toNode.ToString());

                    if (node[j].playerAnswer[i].questStatus > 0 && node[j].playerAnswer[i].questName.Trim().Length > 0)
                    {
                        element.SetAttribute("questStatus", node[j].playerAnswer[i].questStatus.ToString());
                    }

                    if (node[j].playerAnswer[i].questValueGreater >= 1 && node[j].playerAnswer[i].questName.Trim().Length > 0)
                    {
                        element.SetAttribute("questValueGreater", node[j].playerAnswer[i].questValueGreater.ToString());
                        element.SetAttribute("questName", node[j].playerAnswer[i].questName);
                    }
                    else if (node[j].playerAnswer[i].questValue >= 0 && node[j].playerAnswer[i].questName.Trim().Length > 0)
                    {
                        element.SetAttribute("questValue", node[j].playerAnswer[i].questValue.ToString());
                        element.SetAttribute("questName", node[j].playerAnswer[i].questName);
                    }
                }

                userNode.AppendChild(element);
            }

            rootNode.AppendChild(userNode);
        }

        xmlDoc.Save(path);
        Debug.Log(this + " создан XML файл диалога [ " + fileName + " ] по адресу: " + path);
    }
}

[System.Serializable]
public class DialogueNode
{
    public string npcText;
    public PlayerAnswer[] playerAnswer;
}


[System.Serializable]
public class PlayerAnswer
{
    public string text;

    [Tooltip("Этот ответ закрывает окно диалога?")]
    public bool exit;

    [Tooltip("Переход на другой узел диалога.")]
    public int toNode;

    [Tooltip("Строка появится в диалоге, если прогресс квеста соответствует данному значению. [значение от '0']")]
    public int questValue;

    [Tooltip("Строка появится в диалоге, если прогресс квеста больше или равно этому значению. [значение от '1']")]
    public int questValueGreater;

    [Tooltip("Имя квеста с которым взаимодействует данный ответ.")]
    public string questName;

    [Tooltip("Взять квест = '1', Отказаться от квеста = '2', Сдать квест = '3', Неактивно = '0'")]
    [Range(0, 3)] public int questStatus;
}