using UnityEngine;

public class Menu : MonoBehaviour
{

    [SerializeField] internal GameObject _menuPanel;

    [SerializeField] private CameraMove _cameraMove;
    private GameObject _currentPanel;

    private void Awake()
    {
        Time.timeScale = 0f;
    }

    public void NewGame()
    {
        Time.timeScale = 1f;
        _cameraMove._offset = new Vector3(0, 0, -2);
        _menuPanel.SetActive(false);
    }

    public void OpenPanel(GameObject panel)
    {
        if (_currentPanel != null)
        {
            _currentPanel.SetActive(false);
        }

        panel.SetActive(true);
        _currentPanel = panel;
        _menuPanel.SetActive(false);
    }

    public void CloseCurrentPanel()
    {
        if (_currentPanel != null)
        {
            _currentPanel.SetActive(false);
            _currentPanel = null;
        }

        _menuPanel.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}