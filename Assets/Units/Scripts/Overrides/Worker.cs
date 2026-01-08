using Unity.VisualScripting;
using UnityEngine;

public class Worker : Unit
{
    private enum CommandCardState
    {
        Base, Build
    }

    private CommandCardState state;

    private static bool StaticsInit = false;
    
    private static NoTargetCommand EnterBuildMode = new NoTargetCommand() {perform = unit =>
    {
        if (unit is Worker worker)
        {
            worker.state = CommandCardState.Build;
        }
    }};
    private static NoTargetCommand ExitBuildMode = new NoTargetCommand() {perform = unit =>
    {
        if (unit is Worker worker)
        {
            worker.state = CommandCardState.Base;
        }
    }};
    
    private static BuildCommand BuildRecCenter = new BuildCommand();
    private static BuildCommand BuildResCenter = new BuildCommand();
    

    private CommandCard WorkerCommandCard = new CommandCard()
    {
        S = GeneralCommands.Stop,
        D = GeneralCommands.Move,
        Z = EnterBuildMode
    };
    
    private CommandCard BuildCommandCard = new CommandCard()
    {
        Q = BuildRecCenter,
        W = BuildResCenter,
        C = ExitBuildMode,
    };

    public override CommandCard CommandCard {
        get
        {
            if (state == CommandCardState.Base)
            {
                return WorkerCommandCard;
            }

            return BuildCommandCard;
        }
    }
    public override void InitResources()
    {
        if (!StaticsInit)
        {
            EnterBuildMode.Icon = Resources.Load<Texture2D>("CommandIcons/Worker/Build");
            ExitBuildMode.Icon = Resources.Load<Texture2D>("CommandIcons/Worker/Cancel");

            BuildRecCenter.Icon = Resources.Load<Texture2D>("CommandIcons/Worker/Build");
            BuildResCenter.Icon = Resources.Load<Texture2D>("Resource1");

            BuildRecCenter.buildingPrefab = Resources.Load<GameObject>("RecCenter");
            BuildResCenter.buildingPrefab = Resources.Load<GameObject>("ResCenter");

            BuildRecCenter.buildingGhostPrefab = Resources.Load<GameObject>("BuidGhost");
            BuildResCenter.buildingGhostPrefab = Resources.Load<GameObject>("BuidGhost");

            StaticsInit = true;
        }
    }
}
