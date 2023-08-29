using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour {
    [SerializeField] private GameObject pauseUI;
    private InputActions actions;
    private int mainMenuSceneIndex = 0;

    private void Awake() {
        actions = new InputActions();

        actions.Menu.Enable();

        actions.Menu.Pause.performed += TogglePause;
    }

    private void OnEnable() {
        pauseUI.SetActive(false);
    }

    private void OnDisable() {
        
    }

    private void TogglePause(InputAction.CallbackContext context) {
        if (pauseUI.activeSelf)
            Resume();
        else
            Pause();
    }

    public void Resume() {
        Time.timeScale = 1; 
        pauseUI.SetActive(false);
        PlayerInfo.Instance.ToggleAction(true);
    }

    private void Pause() {
        Time.timeScale = 0; 
        pauseUI.SetActive(true);
        PlayerInfo.Instance.ToggleAction(false);
    }

    public void SaveGame()
    {
        SaveSystem.SaveGameData(GameManager.Instance.GetGameData());
    }

    public void Quit() {
        Time.timeScale = 1;
        actions.Menu.Pause.performed -= TogglePause;
        actions.Menu.Disable();
        SceneManager.LoadScene(mainMenuSceneIndex);
    }
}