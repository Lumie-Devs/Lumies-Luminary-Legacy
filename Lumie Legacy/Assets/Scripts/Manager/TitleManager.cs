using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour {
    private readonly int firstScene = 1;

    [SerializeField] private Button loadButton;

    private void Awake() {
        loadButton.interactable = SaveSystem.SaveFileExists();
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

    private void QuitGame(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}