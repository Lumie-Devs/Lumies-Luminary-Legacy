using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public Vector2 spawnLocation = new(0,0);
}

[System.Serializable]
public class WorldState
{
    public int currentScene = 1;
}

[System.Serializable]
public class SystemData
{
    public float playTime = 0;
}

[System.Serializable]
public class GameData
{
    public PlayerData playerData = new();
    public WorldState worldState = new();
    public SystemData systemData = new();
}
