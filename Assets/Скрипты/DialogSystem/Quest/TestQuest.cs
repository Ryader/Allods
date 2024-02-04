using UnityEngine;
using System.Collections;

public class TestQuest : MonoBehaviour { // простой тестовый квест

	public GameObject[] target;
	private static int value, tmp;
    public int goal;
	private static TestQuest _internal;

	public static TestQuest Internal
	{
		get{ return _internal; }
	}

	void Awake()
	{
        goal = target.Length;
        ResetQuest();

		value = 0; // начальное значение статуса

		_internal = this;
		enabled = false;
	}

	void LateUpdate()
	{
		tmp = 0;
		foreach(GameObject obj in target)
		{
			if(!obj.activeSelf)
			{
				tmp++;
			}

			if(tmp == goal)
			{
				value = 2; // цель достигнута
				enabled = false;
			}
		}
	}

	public static int questValue
	{
		get{ return value; }
	}

	public void QuestStatus(QuestManager.Status status)
	{
		switch(status)
		{
		case QuestManager.Status.Active:
			SetActiveQuest();
			break;
		case QuestManager.Status.Complete:
			SetCompleteQuest();
			break;
		case QuestManager.Status.Disable:
			ResetQuest();
			break;
		}
	}

	void SetActiveQuest()
	{
		value = 1; // квест активен
		enabled = true;
		foreach(GameObject obj in target)
		{
			obj.SetActive(true);
		}
	}

	void SetCompleteQuest()
	{
		enabled = false;
		value = -1; // квест сдан
	}

	void ResetQuest()
	{
		enabled = false;
		value = 0;
		foreach(GameObject obj in target)
		{
			obj.SetActive(false);
		}
	}
}
