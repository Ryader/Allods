using UnityEngine;

public class MenuPause : MonoBehaviour
{
    private static bool _gameIsPaused = false;

    [SerializeField] private GameObject _pauseMenuUI;
    [SerializeField] private CameraMove _cameraMove;
    [SerializeField] private Menu _menu;
   

    void Update()
    {

        if (!_menu._menuPanel.activeSelf)
        {
            // Если главное меню не открыто, открываем панель меню паузы
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Resume();
            }
        }

    }

    public void Resume()
    {
        _pauseMenuUI.SetActive(!_gameIsPaused);
        Time.timeScale = _gameIsPaused ? 1f : 0f;
        _gameIsPaused = !_gameIsPaused;
    }

    public void ExitMenu()
    {
        Time.timeScale = 1f;
        _cameraMove._offset = new Vector3(0, 0, -11);
        _pauseMenuUI.SetActive(false);
        _menu._menuPanel.SetActive(true);
    }
}