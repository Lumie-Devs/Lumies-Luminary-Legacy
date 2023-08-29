using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    private GameData gameData;

    private void Awake() {
        if (Instance == null) Setup();
          
        else Destroy(gameObject);
    }

    private void Setup()
    {
        Instance = this;
        gameData = new GameData();
        DontDestroyOnLoad(gameObject);
    }

    public void SetGameData(GameData gameData)
    {
        this.gameData = gameData;

        StopAllCoroutines();
        StartCoroutine(CountPlayTime());
    }

    public PlayerData GetPlayerData()
    {
        return gameData.playerData;
    }

    public GameData GetGameData()
    {
        return gameData;
    }

    public IEnumerator CountPlayTime()
    {
        float previousTime = Time.time;

        while (true)
        {
            float deltaTime = Time.time - previousTime;
            gameData.systemData.playTime += deltaTime;

            previousTime = Time.time;

            Debug.Log(FormatTimeToString(gameData.systemData.playTime));

            yield return null;
        }
    }
    private string FormatTimeToString(float time)
    {
        int hours = (int)time / 3600;
        int minutes = ((int)time % 3600) / 60;
        int seconds = ((int)time % 3600) % 60;

        return hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}