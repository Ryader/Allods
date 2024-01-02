using UnityEngine;
using UnityEngine.UI;

public class AlertMenu : MonoBehaviour {

    public Text alertText;
    public Button aplly, cancel;

    public void Cancel()
    {
        gameObject.SetActive(false);
    }

    public void Aplly()
    {
        MenuSaveLoad.CurrentAction();
    }
}
