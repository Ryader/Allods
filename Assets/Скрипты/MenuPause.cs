using UnityEngine;

public class MenuPause : MonoBehaviour
{
    private static bool _gameIsPaused = false;

    [SerializeField] private GameObject _pauseMenuUI;
    [SerializeField] private CameraMove _cameraMove;
    [SerializeField] private Menu _menu;
    [SerializeField] private GameObject _hBar;

    void Update()
    {
        if (!_menu._menuPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _gameIsPaused = !_gameIsPaused;
                _pauseMenuUI.SetActive(_gameIsPaused);
                Time.timeScale = _gameIsPaused ? 0f : 1f;
                _hBar.SetActive(!_gameIsPaused);
            }
        }
    }

    public void ExitMenu()
    {
        Time.timeScale = 1f;
        _cameraMove._offset = new Vector3(0, 0, -11);
        _pauseMenuUI.SetActive(false);
        _menu._menuPanel.SetActive(true);
        _hBar.SetActive(false);
    }
}