using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class MyLaserPointer : MonoBehaviour {
    [SerializeField]
    private Transform _RightHandAnchor; // 右手

    [SerializeField]
    private Transform _LeftHandAnchor;  // 左手

    [SerializeField]
    private Transform _CenterEyeAnchor; // 目の中心

    [SerializeField]
    private float _MaxDistance = 100.0f; // 距離

    [SerializeField]
    private LineRenderer _LaserPointerRenderer; // LineRenderer

    [SerializeField]
    public Text text;   //デバック用表示text

    // コントローラー
    private Transform Pointer
    {
        get
        {
            // 現在アクティブなコントローラーを取得
            var controller = OVRInput.GetActiveController();
            if (controller == OVRInput.Controller.RTrackedRemote)
            {
                return _RightHandAnchor;
            }
            else if (controller == OVRInput.Controller.LTrackedRemote)
            {
                return _LeftHandAnchor;
            }
            // どちらも取れなければ目の間からビームが出る
            return _CenterEyeAnchor;
        }
    }

    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        var pointer = Pointer; // コントローラーを取得
                               // コントローラーがない or LineRendererがなければ何もしない

        if (pointer == null || _LaserPointerRenderer == null)
        {
            return;
        }
        // コントローラー位置からRayを飛ばす
        Ray pointerRay = new Ray(pointer.position, pointer.forward);

        // レーザーの起点
        _LaserPointerRenderer.SetPosition(0, pointerRay.origin);

        RaycastHit hitInfo;
        if (Physics.Raycast(pointerRay, out hitInfo, _MaxDistance))
        {
            // Rayがヒットしたらそこまで
            _LaserPointerRenderer.SetPosition(1, hitInfo.point);
            //ヒットした名前を表示
            text.text = hitInfo.collider.gameObject.name;

            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
            {
                //完全に消える
                //hitInfo.collider.gameObject.SetActive(false);
                hitInfo.transform.gameObject.GetComponent<Renderer>().material.color = new Color(1.0f,0.0f,1.0f,1.0f);
            }else if(OVRInput.Get(OVRInput.RawButton.Back))
            {
                //透明度0に
                hitInfo.transform.gameObject.GetComponent<Renderer>().material.color = new Color(0.0f, 1.0f, 0.0f, 0.1f);
            }
        }
        else
        {
            // Rayがヒットしなかったら向いている方向にMaxDistance伸ばす
            _LaserPointerRenderer.SetPosition(1, pointerRay.origin + pointerRay.direction * _MaxDistance);
            //名前をNULLにする
            text.text = "Miss";
        }
    }
}
