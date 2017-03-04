using UnityEngine;
using System.Collections;

public class QG : BaseNode
{
    public Material MatEmpty;
    public Material MatFull;

    public override int GetChildCount() { return 4; }

    void Awake()
    {
        GetComponent<Renderer>().sharedMaterial = MatEmpty;
    }
    
    public override void OnChildFull()
    {
        GetComponent<Renderer>().sharedMaterial = MatFull;
    }
}
