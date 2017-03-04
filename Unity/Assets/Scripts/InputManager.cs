using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    private const float _ClickTimeThreshold = 0.2f;
 
    public Camera Cam;

	void Start()
    {
	}

    void Update()
    {
        bool clicked = false;
        Vector2 mouse_delta = Vector2.zero;
        if (Input.GetMouseButton(0))
        {
            if (_ClickTime>0.0f)
            {
                Vector2 pos = Input.mousePosition;
                mouse_delta = (pos-_MousePosition)/Time.deltaTime;
            }

            if (_ClickTime<_ClickTimeThreshold)
                _ClickTime += Time.deltaTime;

            _MousePosition = Input.mousePosition;
        }
        else if (_ClickTime>0.0f)
        {
            if (_ClickTime<_ClickTimeThreshold)
                clicked = true;
            _ClickTime = 0.0f;
        }

        Vector3 center = TreeManager.Get().GetCenter();
        float size = TreeManager.Get().GetSize()+5.0f;

        _Distance = Mathf.Clamp(_Distance-Input.mouseScrollDelta.y/10.0f, 0.1f, 1.5f);

        mouse_delta *= 0.01f;
        Quaternion rotation = _Rotation*Quaternion.Euler(new Vector3(-mouse_delta.y, mouse_delta.x, 0.0f));

        _RotationSpeed = Quaternion.Lerp(_RotationSpeed, Quaternion.Inverse(_Rotation)*rotation, Time.deltaTime*2.0f);

        Cam.transform.rotation *= _RotationSpeed;
        Cam.transform.position = Vector3.Lerp(Cam.transform.position, center-Cam.transform.forward*size*_Distance, Time.deltaTime*8.0f);

        if (clicked)
        {
            Ray ray = Cam.ScreenPointToRay(_MousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider!=null)
                {
                    BaseNode node = hit.collider.GetComponentInParent<BaseNode>();
                    if (node!=null)
                        node.OnClick();
                }
            }
        }
    }

    private float _Distance = 10.0f;
    private Quaternion _Rotation = Quaternion.identity;
    private Quaternion _RotationSpeed = Quaternion.identity;
    private float _ClickTime = 0.0f;
    private Vector2 _MousePosition = Vector2.zero;
}
