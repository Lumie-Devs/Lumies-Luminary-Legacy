using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    private GameData gameData;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void SetGameData(GameData gameData)
    {
        this.gameData = gameData;
    }

    public GameData GetGameData()
    {
        return gameData;
    }
}