using System;
using Unity.VisualScripting;
using UnityEngine;



[Serializable]
public class Command
{
    public Texture2D Icon;

    public Action<Unit> perform;
}

[Serializable]
// command that targets a point such as single-target abilities
public class UnitTargetCommand : Command
{
    public Unit target;
}

[Serializable]
// command that targets a point such as movement, building placement or area attacks
public class PointTargetCommand : Command
{
    public Vector3 target;
}

[Serializable]
public class BuildCommand : Command
{
    public GameObject buildingPrefab;
    public GameObject buildingGhostPrefab;
}

[Serializable]
// command that requires no targets
public class NoTargetCommand : Command
{
    
}

[Serializable]
// command that can target either a unit or a point (choosing units if available) such as attacking (unit: unit attack, point: attack-move)
public class AdaptableTargetCommand : Command
{
    public Vector3? pointTarget;
    public Unit unitTarget;
}

abstract class GeneralCommands
{
    private static bool init = false;
    public static PointTargetCommand Move = new PointTargetCommand() {perform = (unit => unit.MoveTo(Move.target))};
    public static AdaptableTargetCommand Attack = new AdaptableTargetCommand() { perform = (unit =>
    {
        if (Attack.unitTarget != null) unit.Attack(Attack.unitTarget);
        else if (Attack.pointTarget != null) unit.AttackMove(Attack.pointTarget);
    }) };
    public static NoTargetCommand Stop = new NoTargetCommand() {perform = (unit => unit.Stop())};

    public static void InitIcons()
    {
        if(init) return;
        Move.Icon = Resources.Load<Texture2D>("CommandIcons/General/Move");
        Attack.Icon = Resources.Load<Texture2D>("CommandIcons/General/Attack");
        Stop.Icon = Resources.Load<Texture2D>("CommandIcons/General/Stop");
    }
}

public class CommandCard
{
    public Command
        Q,W,E,R,T,
        A,S,D,F,G,
        Z,X,C,V,B;
}