using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player
{
    public static Player Player1 = new("Human", 1);
    public static Player Player2 = new("Bot", 2);
    
    
    public static List<Player> Players = new List<Player>() {Player1, Player2};

    
    [SerializeField]
    public string Name;
    [SerializeField]
    public int Team;

    public Player(string name, int team)
    {
        Name = name;
        Team = team;
    }

    [SerializeField]
    public int Resource1Amount = 0;
}
