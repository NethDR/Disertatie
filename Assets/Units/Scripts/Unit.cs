using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Unit : MonoBehaviour
{
    [SerializeReference]
    public Player owner;
    
    public abstract class GenericTarget
    {
        public abstract Vector3 Position();
    }

    public class PointTarget : GenericTarget
    {
        private Vector3 _targetPoint;

        public PointTarget(Vector3 targetPoint)
        {
            _targetPoint = targetPoint;
        }

        public override Vector3 Position()
        {
            return _targetPoint;
        }
    }

    public class UnitTarget : GenericTarget
    {
        private Unit _targetUnit;
        
        public override Vector3 Position()
        {
            return _targetUnit.transform.position;
        }
        
    }

    
    [CanBeNull] public GenericTarget Target;
    
    public static bool SameTeam(Unit a, Unit b)
    {
        return a.owner.Team == b.owner.Team;
    }
    
    public bool SameTeam(Unit other)
    {
        return owner.Team == other.owner.Team;
    }
    
    
    [SerializeField] private GameObject selectionRing;
    [SerializeField] private InputActionAsset inputActionAsset;
    [SerializeField] private LayerMask ground;
    
    private InputActionMap _actionMap;
    
    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            selectionRing.SetActive(value);
            _isSelected = value;
        }
    }
    
    private InputAction _moveCommand;
    

    [CanBeNull] public AttackController AttackController { get; private set; }
    [CanBeNull] public MoveController MoveController { get; private set; }
    [CanBeNull] public NavMeshAgent Agent { get; private set; }
    
    
    private void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        IsSelected = false;
        UnitSelectionManager.Instance.RegisterUnit(this);
            
        // GetComponent<>()
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
