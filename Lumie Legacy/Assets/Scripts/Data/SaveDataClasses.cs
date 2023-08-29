using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class PlayerData
{

}

[System.Serializable]
public class WorldState
{
    public int currentScene = 1;
}

[System.Serializable]
public class GameData
{
    public PlayerData playerData;
    public WorldState worldState;
}
