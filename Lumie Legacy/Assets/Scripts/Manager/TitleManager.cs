using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TitleManager : MonoBehaviour {
    private readonly int firstScene = 1;

    [SerializeField] private Button loadButton;

    private void Awake() {
        loadButton.interactable = SaveSystem.SaveFileExists();
    }

    public void StartGame(){
        EnterGame(new GameData(), firstScene);
    }

    public void LoadGame()
    {
        GameData gameData = SaveSystem.LoadGameData(); // Load saved game data
        if(gameData != null)
        {
            EnterGame(gameData, gameData.worldState.currentScene);
        }
    }

    private void EnterGame(GameData gameData, int scene)
    {
        GameManager.Instance.SetGameData(gameData);

        SceneManager.LoadScene(scene);
    }

    public void DeleteGame()
    {
        SaveSystem.DeleteSaveData();

        loadButton.interactable = false;
    }

    public void QuitGame(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}