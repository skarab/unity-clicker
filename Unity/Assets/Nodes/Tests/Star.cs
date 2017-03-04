using UnityEngine;
using System.Collections;

public class Star : BaseNode
{
    protected override void OnUpdate()
    {
        transform.GetChild(0).LookAt(transform.position+TreeManager.Get().Cam.transform.forward);
    }
}
