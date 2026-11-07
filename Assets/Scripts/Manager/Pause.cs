
using UnityEngine;

public class Pause : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _pauseUI;
    [SerializeField] private GameObject _settingsUI;

    private bool _isPaused;

    private void Start()
    {
        Time.timeScale = 1f;
        _isPaused = false;

        _pauseMenu.SetActive(false);
        _pauseUI.SetActive(false);
        _settingsUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (!_isPaused)
        {
            PauseGame();
            return;
        }

        if (_settingsUI.activeSelf)
        {
            OpenPauseUI();
            return;
        }

        ResumeGame();
    }

    public void PauseGame()
    {
        _isPaused = true;
        Time.timeScale = 0f;

        _pauseMenu.SetActive(true);
        OpenPauseUI();
    }

    public void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1f;

        _pauseMenu.SetActive(false);
    }

    public void OpenPauseUI()
    {
        _pauseUI.SetActive(true);
        _settingsUI.SetActive(false);
    }

    public void OpenSettingsUI()
    {
        _pauseUI.SetActive(false);
        _settingsUI.SetActive(true);
    }

    private void OnApplicationQuit()
    {
        Time.timeScale = 1f;
    }
}
