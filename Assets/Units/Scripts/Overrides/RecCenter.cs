using UnityEngine;

public class RecCenter : Unit
{
    private static bool StaticsInit = false;
    public static NoTargetCommand buildWorker = new NoTargetCommand() {perform = unit =>
    {
        if(unit is RecCenter center)
            Instantiate(Resources.Load<GameObject>("Worker"), center.SpawnPoint.position, Quaternion.identity);
    }};
    public static NoTargetCommand buildBaseUnit = new NoTargetCommand() {perform = unit =>
    {
        if(unit is RecCenter center)
            Instantiate(Resources.Load<GameObject>("BaseUnit"), center.SpawnPoint.position, Quaternion.identity);
    }};

    [SerializeField]
    private Transform SpawnPoint;


    public CommandCard commCard = new CommandCard()
    {
        Q = buildWorker,
        W = buildBaseUnit
    };

    public override CommandCard CommandCard
    {
        get { return commCard; }
    }
    public override void InitResources()
    {
        if (!StaticsInit)
        {
            buildWorker.Icon = Resources.Load<Texture2D>("CommandIcons/Worker/Build");
            buildBaseUnit.Icon = Resources.Load<Texture2D>("CommandIcons/General/Attack");

            StaticsInit = true;
        }
    }
}
