using System;
using UnityEngine;

public class BuildGhost : MonoBehaviour
{
    private Collider _collider;

    private Terrain t;

    public bool ok = false;

    [SerializeField]
    private Material mat1;
    [SerializeField]
    private Material mat2;

    private void Start()
    {
        _collider = GetComponent<Collider>();
        t = Terrain.activeTerrain;
    }

    private void OnTriggerStay(Collider other) // happens before update
    {
        if (other.GetComponent<Terrain>() == t)
        {
            ok = false;
        }
    }

    private void FixedUpdate() // happens before ontriggerstay
    {
        ok = true;
    }

    public void Update()
    {
        if (ok)
        {
            GetComponentInChildren<Renderer>().material = mat1;
        }
        else
        {
            GetComponentInChildren<Renderer>().material = mat2;
        }

        
    }
}
