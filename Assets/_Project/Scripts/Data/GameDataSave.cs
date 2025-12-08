using System.Collections.Generic;

[System.Serializable]
public class GameDataSave
{
    public int level;
    public int money;
    public int life;

    public Dictionary<string, int> boosters = new Dictionary<string, int>();

    public GameDataSave() 
    { 
        level = 1;
        money = 0;
        life = 5;

        boosters["timeFrozen"] = 0; 
        boosters["bomb"] = 0; 
        boosters["hammer"] = 0;
    }
}