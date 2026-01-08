using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Unit : MonoBehaviour
{
    private int _hp;
    [SerializeField]
    private int _maxHp;


    public float moveSpeed = 20;
    public float acceleration = 10;


    public int HP
    {
        get { return _hp; }
        set {
            _hp = value;
            if (_hp > _maxHp) _hp = _maxHp;
            if (_hp <= 0) Destroy(gameObject);
        }
    }
    
    
    public int MaxHp => _maxHp;

    [SerializeField]
    private string _typename;
    public string Typename => _typename;

    // [SerializeReference]
    // public Player owner;

    private CommandCard _baseCommandCard = new CommandCard()
    {
        A = GeneralCommands.Attack,
        S = GeneralCommands.Stop,
        D = GeneralCommands.Move
    };

    public virtual CommandCard CommandCard => _baseCommandCard;

    [SerializeField] private GameObject selectionRing;
    
    private bool _isSelected;
    public bool IsSelected
    {
        get { return _isSelected; }
        set
        {
            _isSelected = value;
            selectionRing.SetActive(value);
        }
    }
    

    private void Start()
    {
        _hp = _maxHp;
            GetComponent<NavMeshAgent>().speed = moveSpeed;
            GetComponent<NavMeshAgent>().acceleration = acceleration;
        
        InitResources();
        
        UnitSelectionManager.Instance.RegisterUnit(this);

    }

    public void MoveTo(Vector3 point)
    {
        GetComponent<NavMeshAgent>()?.SetDestination(point);
    }
    
    public void Stop()
    {
        GetComponent<NavMeshAgent>()?.SetDestination(transform.position);
    }

    private void OnDestroy()
    {
        UnitSelectionManager.Instance.DeregisterUnit(this);
    }


    public virtual void InitResources(){}

    public void Attack(Unit attackUnitTarget)
    {
    }

    public void AttackMove(Vector3? attackPointTarget)
    {
    }
}
