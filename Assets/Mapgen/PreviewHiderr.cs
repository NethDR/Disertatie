using UnityEngine;

public class PreviewHiderr : MonoBehaviour
{
    public Canvas c;

    public void Hider()
    {
        Debug.Log("hi");
        Destroy(c);
    }
}
