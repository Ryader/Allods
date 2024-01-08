using UnityEngine;
using System.Collections;

public class QuestManager : MonoBehaviour {

	public enum Status {Disable, Active, Complete};

	public static int GetCurrentValue(string questName) // запрос статуса по имени квеста
	{
		int j = 0;

		switch(questName)
		{
		case "TestQuest":
			// здесь имеет смысл использовать условие и запрос из сохранения, чтобы проверить завершен квест или нет, если да то, передать значение: -1
			j = TestQuest.questValue;
			break;
		}

		return j;
	}
	
	public static void SetQuestStatus(string questName, Status status) // изменения статуса, указанного квеста
	{
		switch(questName)
		{
		case "TestQuest":
			TestQuest.Internal.QuestStatus(status);
			break;
		}
	}
}
