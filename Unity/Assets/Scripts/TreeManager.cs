using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Numerics;

[Serializable]
public class TreeNode
{
    [NonSerialized] public GameObject Node;

    public int ID;
    public int Type;
    public Vector3 Position;
    public Quaternion Rotation;
    public int[] Children;
    
    public static TreeNode Create(Tree root, int type, Vector3 position, Quaternion rotation)
    {
        TreeNode node = new TreeNode();
        node.ID = root.Nodes.Count;
        node.Type = type;
        root.Nodes.Add(node);

        node.Position = position;
        node.Rotation = rotation;
        
        node.Children = new int[0];

        return node;
    }

    public void CreateChildren(int count)
    {
        Children = new int[count];
        
        for (int i=0 ; i<count ; ++i)        
            Children[i] = -1;
    }

    public bool IsSlotFree(int id)
    {
        return Children[id]==-1;
    }

    public List<int> GetFreeSlots()
    {
        List<int> slots = new List<int>();
        for (int i=0 ; i<Children.Length ; ++i)
        {
            if (IsSlotFree(i))
                slots.Add(i);
        }
        return slots;
    }
}

[Serializable]
public class Tree
{
    public List<TreeNode> Nodes = new List<TreeNode>();
    public BigInteger Bank;
    public DateTime LatestTime;
}

[Serializable]
public class NodeSetting
{
    public GameObject Prefab;
    public int MPS;
    public int Price;
    public Image UITex;
}

public class TreeManager : MonoBehaviour
{
    public NodeSetting[] NodeSettings;
    public Text TextBank;
    public Text TextWeight;
    public Text TextSize;
    public Camera Cam;

    void Awake()
    {
        _Instance = this;
        
        _TreePath = Helpers.GetDataPath()+"tree.bin";

        _Tree = (Tree)Helpers.Load(_TreePath);

        if (_Tree==null)
        {
            _Tree = new Tree();
            _Tree.Bank = 0;
            TreeNode root = TreeNode.Create(_Tree, 0, Vector3.zero, Quaternion.identity);
            root.CreateChildren(4);
        }
        else
        {
            _Loaded = true;
        }
    }

    void OnDestroy()
    {
        _Tree.LatestTime = DateTime.UtcNow;
        Helpers.Save(_Tree, _TreePath);
    }

    void Start()
    {
        CreateTreeNode(_Tree.Nodes[0], transform);
	}

    private float _Time = 0.0f;

    private bool _Update = false;

    void Update()
    {
        if (_Loaded && _Count>0)
        {
            TimeSpan diff = DateTime.UtcNow.Subtract(_Tree.LatestTime);
            if (diff.TotalSeconds>0.0)
                _Tree.Bank += (_MPS/8)*(int)(diff.TotalSeconds/0.2f);
            _Loaded = false;
        }

        _Update = false;
        _Time += Time.deltaTime;
        while (_Time>=0.2f)
        {
            _Update = true;
            _Time -= 0.2f;
            _Tree.Bank += _MPS/8;
        }

        if (_Update)
            _MPS = 0;

        if (_Count>0)
        {
            _Center = _Centers/(float)_Count;
            _Size = _Sizes;
        }

        _Sizes = 0.0f;
        _Count = 0;
        _Centers = Vector3.zero;
        
        TextWeight.text = _Tree.Nodes.Count.ToString();
        TextSize.text = _Size.ToString("0.000000");
        TextBank.text = _Tree.Bank.ToString();

        for (int i=0 ; i<NodeSettings.Length ; ++i)
        {
            Color color;
            float c = NodeSettings[i].Price<=_Tree.Bank?1.0f:0.6f;

            if (i==_NodeType) color = new Color(c, c, c, 1.0f);
            else color = new Color(c*0.5f, c*0.5f, c*0.5f, 1.0f);

            if (NodeSettings[i].UITex!=null)
                NodeSettings[i].UITex.color = color;
        }
    }

    public static TreeManager Get() { return _Instance; }
    public Vector3 GetCenter() { return _Center; }
    public float GetSize() { return _Size; }    

    public void UpdateNode(TreeNode node)
    {
        if (_Update)
        {
            ++_Count;
            Vector3 pos = node.Node.transform.position;
            _Centers += pos;
            _Sizes = Mathf.Max(_Sizes, Vector3.Distance(_Center, pos));
            _MPS += NodeSettings[node.Type].MPS;
        }
    }

    public void OnClickNode(TreeNode node)
    {
        _Tree.Bank += 1;

        List<int> slots = node.GetFreeSlots();
        if (slots.Count==0)
            return;

        BaseNode parent_node = node.Node.GetComponent<BaseNode>();
        if (parent_node.IsCreating())
            return;

        if (_Tree.Bank<NodeSettings[_NodeType].Price)
            return;
        _Tree.Bank -= NodeSettings[_NodeType].Price;
        
        Vector3 position = parent_node.GenerateChildPosition(slots[0]);
        Quaternion rotation = parent_node.GenerateChildRotation(slots[0]);

        TreeNode child = TreeNode.Create(_Tree, _NodeType, position, rotation);
        node.Children[slots[0]] = child.ID;
        CreateTreeNode(child, node.Node.transform);
        
        BaseNode base_node = child.Node.GetComponent<BaseNode>();
        child.CreateChildren(base_node.GetChildCount());

        base_node.Create();

        if (slots.Count==1)
            parent_node.OnChildFull();
    }

    public void SelectNodeType(int i)
    {
        _NodeType = i;
    }

    private void CreateTreeNode(TreeNode node, Transform parent)
    {
        GameObject obj = GameObject.Instantiate<GameObject>(NodeSettings[node.Type].Prefab);
        obj.transform.SetParent(parent);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = node.Position;
        obj.transform.localRotation = node.Rotation;

        BaseNode base_node = obj.GetComponent<BaseNode>();
        base_node.Node = node;        
        node.Node = obj;

        bool full = node.Children.Length>0;
        for (int i=0 ; i<node.Children.Length ; ++i)
        {
            if (node.Children[i]==-1)
            {
                full = false;
            }
            else
            {
                CreateTreeNode(_Tree.Nodes[node.Children[i]], obj.transform);
            }
        }

        if (full)
            base_node.OnChildFull();
    }
    
    private static TreeManager _Instance;
    private Tree _Tree = null;
    private Vector3 _Center = Vector3.zero;
    private float _Size = 0.0f;
    private Vector3 _Centers = Vector3.zero;
    private float _Sizes = 0.0f;
    private int _Count = 0;
    private string _TreePath;
    private BigInteger _MPS = 0;
    private bool _Loaded = false;
    private int _NodeType = 1;
}
