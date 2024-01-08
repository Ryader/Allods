using UnityEngine;
using UnityEngine.UI;

public class FileListButton : MonoBehaviour {

    public RectTransform rect;
    public Text title, date;
    public string meta { get; set; }
    public string path { get; set; }
    public System.DateTime  dateTime { get; set; }

    public void Click()
    {
        MenuSaveLoad.LoadMeta(this);
    }

    public void SetButton(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
