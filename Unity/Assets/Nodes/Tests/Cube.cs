using UnityEngine;
using System.Collections;

public class Cube : BaseNode
{
    void Awake()
    {
        GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }
    
    protected override void OnStart()
    {
        _Roll = new Vector4(Random.value-0.5f, Random.value-0.5f, Random.value-0.5f);
	}
	
    void OnDestroy()
    {
        DestroyImmediate(GetComponent<Renderer>().material);
    }

	protected override void OnUpdate()
    {
        transform.localRotation = Quaternion.Euler(_Roll*Time.deltaTime*4.0f)*transform.localRotation;
        Node.Rotation = transform.localRotation;

        if ((transform.parent.position-transform.position).sqrMagnitude>0.0f)
        {
            Transform branch = transform.GetChild(0);
            float dist = (transform.parent.position-transform.position).magnitude;
            branch.localScale = new Vector3(0.1f, 0.1f, dist);
            branch.rotation = Quaternion.LookRotation((transform.parent.position-transform.position)/dist);
            branch.position = transform.position+branch.rotation*new Vector3(0.0f, 0.0f, dist/2.0f);
        }
    }

    public override void OnChildFull()
    {
        GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
    }

    private Vector3 _Roll = Vector3.zero;
}
