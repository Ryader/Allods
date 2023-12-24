using UnityEngine;

internal class Menu : MonoBehaviour
{

    [SerializeField] private GameObject _menuPanel;  // Главное меню
    [SerializeField] private CameraMove _cameraMove;
    private GameObject _currentPanel;

    public void NewGame()
    {
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

    // Выход из игры
    public void Quit()
    {
        Application.Quit();
    }
}