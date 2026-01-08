using System;
using UnityEngine;

public class ActionAssetHolder : MonoBehaviour
{
    public static ActionAssetHolder Instance;
    
    public RtsActions Actions { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
        {
            Instance = this;
            Actions = new RtsActions();
            Actions.Enable();
        }
    }
}
