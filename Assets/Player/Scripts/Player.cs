using System;
using System.Collections.Generic;

public class Player
{
    public static Player Player1 = new("Human", 1);
    public static Player Player2 = new("Bot", 2);
    
    
    public static List<Player> Players = new List<Player>() {Player1, Player2};

    
    
    public string Name;
    public int Team;

    public Player(string name, int team)
    {
        Name = name;
        Team = team;
    }

    public int Resource1Amount = 0;
}
