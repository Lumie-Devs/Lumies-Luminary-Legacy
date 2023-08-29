using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TitleManager : MonoBehaviour {
    private readonly int firstScene = 1;
    private InputActions actions;

    [SerializeField] private Button loadButton;

    private void Awake() {
        loadButton.interactable = SaveSystem.SaveFileExists();

        actions = new InputActions();

        actions.Menu.Enable();

        // actions.Menu.Navigate.performed += ;
        // actions.Menu.Select.performed += ;
    }

    public void StartGame(){
        SceneManager.LoadScene(firstScene);
    }

    public void LoadGame()
    {
        GameData gameData = SaveSystem.LoadGameData(); // Load saved game data
        if(gameData != null)
        {
            GameManager.Instance.SetGameData(gameData);

            SceneManager.LoadScene(gameData.worldState.currentScene);
        }
    }

    public void QuitGame(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}