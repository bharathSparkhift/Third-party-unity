using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    public string name;
    public int level;
    public float health;
}

[System.Serializable]
public class PlayerListWrapper
{
    public List<Player> players = new List<Player>();
}
