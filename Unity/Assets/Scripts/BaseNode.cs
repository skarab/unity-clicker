using UnityEngine;
using System.Collections;

public class BaseNode : MonoBehaviour
{
    public TreeNode Node;
    
    public virtual int GetChildCount() { return 0; }
    public virtual float GetCreationTime() { return 0.0f; }
    
    public bool IsCreating()
    {
        return _CreateTime<GetCreationTime();
    }

    public float GetCreateTime() { return _CreateTime; }
    
    void Start()
    {
        OnStart();
    }

    void Update()
    {
        if (IsCreating())
            _CreateTime += Time.deltaTime;
        
        OnUpdate();
        TreeManager.Get().UpdateNode(Node);
    }

    protected virtual void OnStart()
    {
    }

    protected virtual void OnUpdate()
    {
    }

    public virtual void OnClick()
    {
        TreeManager.Get().OnClickNode(Node);
    }

    public virtual Vector3 GenerateChildPosition(int id)
    {
        float angle = (360.0f*id/(float)GetChildCount())*Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(angle), 1.0f, Mathf.Cos(angle))*2.5f;
    }

    public virtual Quaternion GenerateChildRotation(int id)
    {
        return Quaternion.Euler(new Vector3(
            UnityEngine.Random.value*80.0f-40.0f,
            UnityEngine.Random.value*360.0f,
            UnityEngine.Random.value*80.0f-40.0f));
    }

    public void Create()
    {
        _CreateTime = 0.0f;
        OnCreate();
    }

    public virtual void OnCreate()
    {
    }

    public virtual void OnChildFull()
    {
    }

    private float _CreateTime = 99999.0f;
}
