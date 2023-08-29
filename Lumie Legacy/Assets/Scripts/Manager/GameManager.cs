using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void SetGameData(GameData gameData)
    {
        
    }
}