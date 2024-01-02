using UnityEngine;

public class TestTest : MonoBehaviour
{
    public string value;

    public void SSystem()
    {
        SaveSystem.SetString("key", "value"); // пишем какие-то данные для сохранения
        MenuSaveLoad.metaInfo = value; // пишем метаданные о текущем сохранении
        MenuSaveLoad.Show(!MenuSaveLoad.IsActive); // открываем меню
    }

    void Example()
    {
        // инициализация системы, необходимо выполнять до запуска других скриптов сцены
        SaveSystem.Initialize();
        
        // чтение данных из массива системы "ключ" и "значение по умолчанию"
        bool kBool = SaveSystem.GetBool("key", false);
        Color kColor = SaveSystem.GetColor("key", Color.black);
        int kint = SaveSystem.GetInt("key", 0);
        float kfloat = SaveSystem.GetFloat("key", 0);
        string kstring = SaveSystem.GetString("key", "value");
        Vector2 kVector2 = SaveSystem.GetVector2("key", Vector2.zero);
        Vector3 kVector3 = SaveSystem.GetVector3("key", Vector3.zero);

        // запись данных в массив системы "ключ" и "значение"
        SaveSystem.SetBool("key", false);
        SaveSystem.SetColor("key", Color.black);
        SaveSystem.SetInt("key", 0);
        SaveSystem.SetFloat("key", 0);
        SaveSystem.SetString("key", "value");
        SaveSystem.SetVector2("key", Vector2.zero);
        SaveSystem.SetVector3("key", Vector3.zero);

        // удалить данные массива по "ключу"
        SaveSystem.RemoveKey("key");

        // сохранить данные массива SaveSystem
        MenuSaveLoad.SaveGame();

        // загрузить последнее сохранение, продолжить игру
        MenuSaveLoad.Resume();
    }
}
