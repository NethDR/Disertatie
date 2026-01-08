using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandManager : MonoBehaviour
{
    public static UnitCommandManager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private Command ChosenCommand = null;
    
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask units;
    
    [SerializeField]
    private UnityEngine.Camera _camera;

    public GameObject ghost;

    public void SendCommand(Command c)
    {
        foreach (Unit unit in UnitSelectionManager.Instance.Selection)
        {
            c.perform(unit);
        }
    }
    private void Start()
    {
        var cmdmap = ActionAssetHolder.Instance.Actions.CommandHotkeys;

        cmdmap.Q.performed += (ctx) => {
            ChosenCommand = UnitSelectionManager.Instance.getCommandCard()?.Q;
            if(ChosenCommand is NoTargetCommand)
            {
                SendCommand(ChosenCommand);
                ChosenCommand = null;
            } else if (ChosenCommand is BuildCommand buildCommand && buildCommand.buildingGhostPrefab)
            {
                ghost = Instantiate(buildCommand.buildingGhostPrefab);
            }};
        cmdmap.W.performed += (ctx) => { ChosenCommand = UnitSelectionManager.Instance.getCommandCard()?.W;
            if(ChosenCommand is NoTargetCommand)
            {
                SendCommand(ChosenCommand);
                ChosenCommand = null;} else if (ChosenCommand is BuildCommand buildCommand && buildCommand.buildingGhostPrefab)
            {
                ghost = Instantiate(buildCommand.buildingGhostPrefab);
            }};
        cmdmap.E.performed += (ctx) => { ChosenCommand = UnitSelectionManager.Instance.getCommandCard()?.E;
            if(ChosenCommand is NoTargetCommand)
            {
                SendCommand(ChosenCommand);
                ChosenCommand = null;} else if (ChosenCommand is BuildCommand buildCommand && buildCommand.buildingGhostPrefab)
            {
                ghost = Instantiate(buildCommand.buildingGhostPrefab);
            }};
        cmdmap.R.performed += (ctx) => { ChosenCommand = UnitSelectionManager.Instance.getCommandCard()?.R;
            if(ChosenCommand is NoTargetCommand)
            {
                SendCommand(ChosenCommand);
                ChosenCommand = null;} else if (ChosenCommand is BuildCommand buildCommand && buildCommand.buildingGhostPrefab)
            {
                ghost = Instantiate(buildCommand.buildingGhostPrefab);
            }};
        cmdmap.T.performed += (ctx) => { ChosenCommand = UnitSelectionManager.Instance.getCommandCard()?.T;
            if(ChosenCommand is NoTargetCommand)
            {
                SendCommand(ChosenCommand);
                ChosenCommand = null;} else if (ChosenCommand is BuildCommand buildCommand && buildCommand.buildingGhostPrefab)
            {
                ghost = Instantiate(buildCommand.buildingGhostPrefab);
            }};
        
        
        cmdmap.A.performed += (ctx) => { ChosenCommand = UnitSelectionManager.Instance.getCommandCard()?.A;
            if(ChosenCommand is NoTargetCommand)
            {
                SendCommand(ChosenCommand);
                ChosenCommand = null;} else if (ChosenCommand is BuildCommand buildCommand && buildCommand.buildingGhostPrefab)
            {
                ghost = Instantiate(buildCommand.buildingGhostPrefab);
            }};
        cmdmap.S.performed += (ctx) => { ChosenCommand = UnitSelectionManager.Instance.getCommandCard()?.S;
            if(ChosenCommand is NoTargetCommand)
            {
                SendCommand(ChosenCommand);
                ChosenCommand = null;} else if (ChosenCommand is BuildCommand buildCommand && buildCommand.buildingGhostPrefab)
            {
                ghost = Instantiate(buildCommand.buildingGhostPrefab);
            }};
        cmdmap.D.performed += (ctx) => { ChosenCommand = UnitSelectionManager.Instance.getCommandCard()?.D;
            if(ChosenCommand is NoTargetCommand)
            {
                SendCommand(ChosenCommand);
                ChosenCommand = null;} else if (ChosenCommand is BuildCommand buildCommand && buildCommand.buildingGhostPrefab)
            {
                ghost = Instantiate(buildCommand.buildingGhostPrefab);
            }};
        cmdmap.F.performed += (ctx) => { ChosenCommand = UnitSelectionManager.Instance.getCommandCard()?.F;
            if(ChosenCommand is NoTargetCommand)
            {
                SendCommand(ChosenCommand);
                ChosenCommand = null;} else if (ChosenCommand is BuildCommand buildCommand && buildCommand.buildingGhostPrefab)
            {
                ghost = Instantiate(buildCommand.buildingGhostPrefab);
            }};
        cmdmap.G.performed += (ctx) => { ChosenCommand = UnitSelectionManager.Instance.getCommandCard()?.G;
            if(ChosenCommand is NoTargetCommand)
            {
                SendCommand(ChosenCommand);
                ChosenCommand = null;} else if (ChosenCommand is BuildCommand buildCommand && buildCommand.buildingGhostPrefab)
            {
                ghost = Instantiate(buildCommand.buildingGhostPrefab);
            }};
        
        
        cmdmap.Z.performed += (ctx) => { ChosenCommand = UnitSelectionManager.Instance.getCommandCard()?.Z;
            if(ChosenCommand is NoTargetCommand)
            {
                SendCommand(ChosenCommand);
                ChosenCommand = null;} else if (ChosenCommand is BuildCommand buildCommand && buildCommand.buildingGhostPrefab)
            {
                ghost = Instantiate(buildCommand.buildingGhostPrefab);
            }};
        cmdmap.X.performed += (ctx) => { ChosenCommand = UnitSelectionManager.Instance.getCommandCard()?.X;
            if(ChosenCommand is NoTargetCommand)
            {
                SendCommand(ChosenCommand);
                ChosenCommand = null;} else if (ChosenCommand is BuildCommand buildCommand && buildCommand.buildingGhostPrefab)
            {
                ghost = Instantiate(buildCommand.buildingGhostPrefab);
            }};
        cmdmap.C.performed += (ctx) => { ChosenCommand = UnitSelectionManager.Instance.getCommandCard()?.C;
            if(ChosenCommand is NoTargetCommand)
            {
                SendCommand(ChosenCommand);
                ChosenCommand = null;} else if (ChosenCommand is BuildCommand buildCommand && buildCommand.buildingGhostPrefab)
            {
                ghost = Instantiate(buildCommand.buildingGhostPrefab);
            }};
        cmdmap.V.performed += (ctx) => { ChosenCommand = UnitSelectionManager.Instance.getCommandCard()?.V;
            if(ChosenCommand is NoTargetCommand)
            {
                SendCommand(ChosenCommand);
                ChosenCommand = null;} else if (ChosenCommand is BuildCommand buildCommand && buildCommand.buildingGhostPrefab)
            {
                ghost = Instantiate(buildCommand.buildingGhostPrefab);
            }};
        cmdmap.B.performed += (ctx) => { ChosenCommand = UnitSelectionManager.Instance.getCommandCard()?.B;
            if(ChosenCommand is NoTargetCommand)
            {
                SendCommand(ChosenCommand);
                ChosenCommand = null;} else if (ChosenCommand is BuildCommand buildCommand && buildCommand.buildingGhostPrefab)
            {
                ghost = Instantiate(buildCommand.buildingGhostPrefab);
            }};

        var select = ActionAssetHolder.Instance.Actions.UnitControl.Move;
        select.performed += ctx =>
        {
            if (ChosenCommand is PointTargetCommand pointTargetCommand)
            {
                var ray = _camera.ScreenPointToRay(Mouse.current.position.value);
                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, ground))
                {
                    pointTargetCommand.target = hit.point;
                    SendCommand(pointTargetCommand);
                }
            }
            else if (ChosenCommand is UnitTargetCommand unitTargetCommand)
            {
                var ray = _camera.ScreenPointToRay(Mouse.current.position.value);
                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, units))
                {
                    unitTargetCommand.target = hit.collider.gameObject.GetComponentInParent<Unit>();
                    SendCommand(unitTargetCommand);
                }
            }
            else if (ChosenCommand is AdaptableTargetCommand adaptableTargetCommand)
            {
                var ray = _camera.ScreenPointToRay(Mouse.current.position.value);
                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, units))
                {
                    adaptableTargetCommand.unitTarget = hit.collider.gameObject.GetComponentInParent<Unit>();
                    SendCommand(adaptableTargetCommand);
                }
                else if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
                {
                    adaptableTargetCommand.pointTarget = hit.point;
                    SendCommand(adaptableTargetCommand);
                }

            }


            ChosenCommand = null;
        };
    }

    private void Update()
    {
        Cursor.SetCursor(ChosenCommand?.Icon, Vector2.zero, CursorMode.Auto);
        if (ghost)
        {
            if (ChosenCommand is BuildCommand b)
            {
                var ray = _camera.ScreenPointToRay(Mouse.current.position.value);
                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, ground))
                {
                    ghost.transform.position = hit.point;
                }
            }
            else
            {
                Destroy(ghost);
            }
        }
    }
}
