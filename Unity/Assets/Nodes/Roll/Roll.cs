﻿using UnityEngine;
using System.Collections;

public class Roll : BaseNode
{
    public Material MatEmpty;
    public Material MatFull;

    public override int GetChildCount() { return 1; }
    public override float GetCreationTime() { return 1.0f; }

    void Awake()
    {
        GetComponent<Renderer>().sharedMaterial = MatEmpty;
        _Roll = new Vector4(Random.value-0.5f, Random.value-0.5f, Random.value-0.5f);
    }

    protected override void OnUpdate()
    {
        if (IsCreating())
        {
            transform.localScale = Vector3.Slerp(_StartScale, Vector3.one, GetCreateTime());
            transform.localPosition = Vector3.Slerp(_StartPosition, _TargetPosition, GetCreateTime());
            transform.hasChanged = true;
        }
        else
        {
            transform.localRotation = Quaternion.Euler(_Roll*Time.deltaTime*4.0f)*transform.localRotation;
            Node.Rotation = transform.localRotation;
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

    public override Vector3 GenerateChildPosition(int id)
    {
        return new Vector3(0.0f, 1.0f, 0.0f)*2.5f;
    }

    public override Quaternion GenerateChildRotation(int id)
    {
        return Quaternion.Euler(new Vector3(
            UnityEngine.Random.value*70.0f,
            UnityEngine.Random.value*360.0f,
            UnityEngine.Random.value*70.0f));
    }

    private Vector3 _TargetPosition;
    private Vector3 _StartPosition;
    private Vector3 _StartScale;
    private Vector3 _Roll = Vector3.zero;
}
