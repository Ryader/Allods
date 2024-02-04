using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSaveLoad : MonoBehaviour {

    public Canvas[] ignoreCanvas; // игнорировать UI элементы, при создании скриншота
    public string fileName = "data"; // имя файла сохранения, если оно не было задано вручную
    public float thumbnailSize = 250; // размер миниатюры скриншота по высоте
    public Image thumbnail; // вывод миниатюры скриншота
    public GameObject menuParent; // родительский объект меню вкл/выкл
    public InputField inputField;
    public AlertMenu alert;
    public Text slotText;
    public Button load, save, slot, del;
    public ScrollRect scrollRect;
    public int offset = 20; // расстояние между UI элементами массива сохранений
    public int limit = 50; // лимит сохранений
    public Text metaData; // вывод данных о текущем выбранном сохранении
    public FileListButton sample; // образец UI элемента
    public ScreenFader fader; // затемнение экрана
    private float curY;
    private static MenuSaveLoad _internal;
    private FileListButton current;
    private List<FileListButton> fileLists = new List<FileListButton>();
    private enum Act { none, delete, rewrite }
    private Act act;
    private string currentFileName;
    public static string metaInfo { get; set; } // здесь можно задать некоторые данные, перед сохранением уровня, например, описание локации или уровня героя/ев и т.п.
    public static bool IsActive { get; private set; }

    void Awake()
    {
        SaveSystem.Initialize(); // первым делом инициализация системы сохранения
        fader.Init(); // инициализация экрана перехода
        metaData.text = "--- --- ---";
        menuParent.SetActive(false);
        IsActive = false;
        _internal = this;
        Build();
        LoadButton();
        alert.Cancel();
        load.onClick.AddListener(()=> { Load(); });
        save.onClick.AddListener(() => { Save(); });
        slot.onClick.AddListener(() => { Slot(); });
        del.onClick.AddListener(() => { Delete(); });
    }

    public static void Resume()
    {
        _internal.Resume_internal();
    }

    void Resume_internal()
    {
        string val = SaveSystem.GameResumeScene();

        if (val != null && Application.CanStreamedLevelBeLoaded(val))
        {
            StartCoroutine(WaitLoad(val));
        }
    }

    string RemoveSpecialCharacters(string input)
    {
        Regex r = new Regex("[^\\w\x20]", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        return r.Replace(input, string.Empty);
    }

    string SaveFile(System.DateTime timeNow, string file)
    {
        return string.Format
            (
            "{0}/{1}-{2}",
            SaveSystem.SavePath(),
            file,
            timeNow.ToFileTime()
            );
    }

    public static void Show(bool value) // метод вкл/выкл меню
    {
        _internal.Show_internal(value);
    }

    void Show_internal(bool value)
    {
        if (alert.gameObject.activeSelf)
        {
            alert.Cancel();
            return;
        }

        menuParent.SetActive(value);
        IsActive = value;
        save.interactable = false;
        load.interactable = false;
        del.interactable = false;
    }

    public static void SaveGame() // сохранить игру и сделать миниатюру скриншота
    {
        _internal.SaveGame_internal();
    }

    void SaveGame_internal()
    {
        if (!SaveSystem.IsData()) return;
        act = Act.none;
        CurrentAction();
        System.DateTime timeNow = System.DateTime.Now;
        string fname = currentFileName.Replace(' ', '_');
        StartCoroutine(Thumbnail(SaveFile(timeNow, fname), ignoreCanvas, thumbnailSize));
        SaveSystem.SaveToDisk(fname, metaInfo, timeNow);
    }

    IEnumerator Thumbnail(string path, Canvas[] ignoreCanvas, float scale)
    {
        scale = Screen.height < scale ? Screen.height : scale / Screen.height;

        yield return null;

        if (ignoreCanvas != null)
            for (int i = 0; i < ignoreCanvas.Length; i++) ignoreCanvas[i].enabled = false;

        yield return new WaitForEndOfFrame();

        Texture2D result = null;
        Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        tex.Apply();

        if (ignoreCanvas != null)
            for (int i = 0; i < ignoreCanvas.Length; i++) ignoreCanvas[i].enabled = true;

        int width = (int)(Screen.width * scale);
        int height = (int)(Screen.height * scale);

        if (scale < 1)
        {
            result = new Texture2D(width, height, tex.format, true);
            Color[] pixels = result.GetPixels(0);

            float x = ((float)1 / tex.width) * ((float)tex.width / width);
            float y = ((float)1 / tex.height) * ((float)tex.height / height);

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = tex.GetPixelBilinear(x * ((float)i % width), y * ((float)Mathf.Floor(i / width)));
            }

            result.SetPixels(pixels, 0);
            result.Apply();
        }
        else
        {
            result = tex;
        }

        byte[] bytes = result.EncodeToPNG();
        File.WriteAllBytes(path + ".png", bytes);
        if (tex != null) Destroy(tex);
        if (result != null) Destroy(result);
    }

    Sprite LoadSprite(string path)
    {
        if (!File.Exists(path)) return null;
        byte[] data = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(data);
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
    }

    void Build()
    {
        fileLists.Add(sample);

        for (int i = 0; i < limit - 1; i++)
        {
            FileListButton clone = Instantiate(sample, scrollRect.content);
            clone.rect.localScale = Vector3.one;
            fileLists.Add(clone);
        }
    }

    public static void LoadMeta(FileListButton fileListButton)
    {
        _internal.LoadMeta_internal(fileListButton);
    }

    void LoadMeta_internal(FileListButton  fileListButton)
    {
        if (thumbnail.sprite != null)
        {
            Destroy(thumbnail.sprite.texture);
            Destroy(thumbnail.sprite);
        }

        current = fileListButton;
        metaData.text = SaveSystem.LoadMetaData(current.meta);
        load.interactable = true;
        del.interactable = true;
        save.interactable = true;
        thumbnail.sprite = LoadSprite(current.path.Replace(".bin", ".png"));
        thumbnail.preserveAspect = true;
    }

    void Slot()
    {
        if (thumbnail.sprite != null)
        {
            Destroy(thumbnail.sprite.texture);
            Destroy(thumbnail.sprite);
        }

        load.interactable = false;
        del.interactable = false;
        current = null;
        save.interactable = (SaveSystem.fileLists.Count < limit) ? true : false;
        metaData.text = "--- --- ---";
    }

    void Delete()
    {
        if (current != null)
        {
            act = Act.delete;
            alert.alertText.text = "Вы уверены, что хотите удалить это сохранение?";
            alert.gameObject.SetActive(true);
        }
    }

    void Save()
    {
        if (current != null)
        {
            act = Act.rewrite;
            alert.alertText.text = "Перезаписать текущее сохранение?";
            alert.gameObject.SetActive(true);
            return;
        }

        SaveGame();
    }

    void Load()
    {
        load.interactable = false;

        if (SaveSystem.currentScene != null && current != null && Application.CanStreamedLevelBeLoaded(SaveSystem.currentScene))
        {
            StartCoroutine(WaitLoad(SaveSystem.currentScene));
        }
    }

    IEnumerator WaitLoad(string level)
    {
        ScreenFader.Fader(3, Color.black);

        while (true)
        {
            yield return null;

            if (ScreenFader.isColor)
            {
                // если загрузка длиться несколько секунд, то тут можно выводить надпись, например " - ЗАГРУЗКА - "
                SceneManager.LoadScene(level);
                yield break;
            }
        }
    }

    public static void CurrentAction()
    {
        _internal.CurrentAction_internal();
    }

    void CurrentAction_internal()
    {
        currentFileName = RemoveSpecialCharacters(string.IsNullOrEmpty(inputField.text) ? fileName : inputField.text).Trim();
        if (string.IsNullOrEmpty(currentFileName)) currentFileName = fileName;

        switch (act)
        {
            case Act.delete:
                SaveSystem.RemoveSave(current.dateTime);
                break;

            case Act.rewrite:
                SaveSystem.RemoveSave(current.dateTime);
                SaveGame();
                break;
        }

        if (thumbnail.sprite != null)
        {
            Destroy(thumbnail.sprite.texture);
            Destroy(thumbnail.sprite);
        }

        metaData.text = "--- --- ---";
        inputField.text = string.Empty;
        load.interactable = false;
        del.interactable = false;
        alert.gameObject.SetActive(false);
    }

    public static void ReBuildList()
    {
        _internal.ReBuildList_internal();
    }

    void ReBuildList_internal()
    {
        float delta = scrollRect.verticalNormalizedPosition;
        LoadButton();
        scrollRect.verticalNormalizedPosition = delta;
    }

    void ResetButton()
    {
        for (int i = 0; i < fileLists.Count; i++)
        {
            fileLists[i].SetButton(false);
        }
    }

    string Date(int ix)
    {
        return SaveSystem.fileLists[ix].date.ToString("dd/MM/yyyy    HH:mm:ss");
    }

    void LoadButton()
    {
        ResetButton();

        slot.targetGraphic.rectTransform.anchoredPosition = new Vector2(0, -slot.targetGraphic.rectTransform.sizeDelta.y / 2f + -offset);
        curY = slot.targetGraphic.rectTransform.sizeDelta.y + offset * 2f;

        int j = 1;

        for (int i = 0; i < SaveSystem.fileLists.Count; i++)
        {
            fileLists[i].rect.anchoredPosition = new Vector2(0, -curY - fileLists[i].rect.sizeDelta.y / 2);
            curY += fileLists[i].rect.sizeDelta.y + offset;

            fileLists[i].dateTime = SaveSystem.fileLists[i].date;
            fileLists[i].path = SaveSystem.fileLists[i].path;
            fileLists[i].meta = SaveSystem.fileLists[i].meta;
            fileLists[i].title.text = SaveSystem.fileLists[i].title.Replace('_', ' ');
            fileLists[i].date.text = (j < 10) ? "0" + j + ":   " + Date(i) : j + ":   " + Date(i);
            fileLists[i].meta = SaveSystem.fileLists[i].meta;
            fileLists[i].SetButton(true);

            j++;
        }

        RectContent();

        save.interactable = false;
        load.interactable = false;
        del.interactable = false;

        slotText.text = (SaveSystem.fileLists.Count < limit) ? SlotText(true) : SlotText(false);
    }

    void RectContent()
    {
        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, curY);
        scrollRect.content.anchoredPosition = Vector2.zero;
    }

    string SlotText(bool isSlot)
    {
        if (isSlot)
        {
            return "-- СВОБОДНЫЙ СЛОТ --\n(доступно слотов для сохранения " + (limit - SaveSystem.fileLists.Count) + " из " + limit + ")";
        }

        return "-- НЕТ СВОБОДНЫХ СЛОТОВ --\n(удалите или перезапишите одно из старых сохранений)";
    }
}
