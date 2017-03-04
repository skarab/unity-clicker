using UnityEngine;
using System.Collections;

public class Platform : BaseNode
{
    public Material MatEmpty;
    public Material MatFull;

    public override int GetChildCount() { return 4; }
    public override float GetCreationTime() { return 1.0f; }

    void Awake()
    {
        GetComponent<Renderer>().sharedMaterial = MatEmpty;
    }
    
    protected override void OnUpdate()
    {
        if (IsCreating())
        {
            transform.localScale = Vector3.Slerp(_StartScale, Vector3.one, GetCreateTime());
            transform.localPosition = Vector3.Slerp(_StartPosition, _TargetPosition, GetCreateTime());
            transform.hasChanged = true;
        }

        if (transform.hasChanged)
        {
            if ((transform.parent.position-transform.position).sqrMagnitude>0.0f)
            {
                Transform branch = transform.GetChild(0);
                float dist = (transform.parent.position-transform.position).magnitude;
                branch.localScale = new Vector3(0.1f, 0.1f, dist);
                branch.rotation = Quaternion.LookRotation((transform.parent.position-transform.position)/dist);
                branch.position = transform.position+branch.rotation*new Vector3(0.0f, 0.0f, dist/2.0f);
            }
            transform.hasChanged = false;
        }
    }

    public override void OnCreate()
    {
        _TargetPosition = transform.localPosition;
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one*0.1f;

        _StartPosition = transform.localPosition;
        _StartScale = transform.localScale;
    }

    public override void OnChildFull()
    {
        GetComponent<Renderer>().sharedMaterial = MatFull;
    }

    private Vector3 _TargetPosition;
    private Vector3 _StartPosition;
    private Vector3 _StartScale;
}
