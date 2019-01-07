using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour {
    //右手
    [SerializeField]
    private Transform _RightHandAnchor;

    //左手
    [SerializeField]
    private Transform _LeftHandAnchor;

    //目の中心
    [SerializeField]
    private Transform _CenterEyeAnchor;

    //最大距離
    [SerializeField]
    private float _MaxDistance = 100.0f;

    //LineRenderer
    [SerializeField]
    LineRenderer _LaserPointerRenderer;

    private Transform Pointer
    {
        get
        {
            //今使ってるコントローラを代入
            var controller = OVRInput.GetActiveController();
            //今使ってるコントローラをリターンする．何もなかったら目の間をリターン
            if(controller == OVRInput.Controller.RTrackedRemote)
            {
                return _RightHandAnchor;
            }else if(controller == OVRInput.Controller.LTrackedRemote)
            {
                return _LeftHandAnchor;
            }

            return _CenterEyeAnchor;
        }
    }




    // Use this for initialization
    void Start () {

		
	}
	
	// Update is called once per frame
	void Update () {
        var pointer = Pointer;

        if (pointer == null || _LaserPointerRenderer == null)
        {
            return;
        }

        Ray pointerRay = new Ray(pointer.position, pointer.forward);

        //レーザの起点
        _LaserPointerRenderer.SetPosition(0, pointerRay.origin);

        RaycastHit hitInfo;

        if(Physics.Raycast(pointerRay, out hitInfo, _MaxDistance))
        {
            //Rayが当たったらそこまで
            _LaserPointerRenderer.SetPosition(1, hitInfo.point);
        }
        else
        {
            //当たらんかったらMaxDistanece伸ばす
            _LaserPointerRenderer.SetPosition(1, pointerRay.origin + pointerRay.direction * _MaxDistance);
        }

        

	}
}
