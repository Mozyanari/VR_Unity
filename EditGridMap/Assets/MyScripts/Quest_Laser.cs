using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Quest_Laser : MonoBehaviour
{
    [SerializeField]
    private Transform _RightHandAnchor; // 右手

    [SerializeField]
    private Transform _LeftHandAnchor;  // 左手

    [SerializeField]
    private float _MaxDistance = 100.0f; // 距離

    [SerializeField]
    private LineRenderer _LaserPointerRenderer; // LineRenderer

    [SerializeField]
    public Text text;   //デバック用表示text

    [SerializeField]
    public RosSharp.RosBridgeClient.MeshCombineOccupancyGridSubscriber map_data;

    // コントローラー
    private Transform Pointer_R;
    private Transform Pointer_L;

    //選択したオブジェクトを保持する変数
    private List<RosSharp.RosBridgeClient.Messages.Geometry.Point32> select_cubes = new List<RosSharp.RosBridgeClient.Messages.Geometry.Point32>();


    void Start()
    {
        Pointer_R = _RightHandAnchor;
        Pointer_L = _LeftHandAnchor;
    }

    // Update is called once per frame
    void Update()
    {

        var pointer = Pointer_R; // コントローラーを取得

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
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger,OVRInput.Controller.RTouch))
            {
                //完全に消える
                //hitInfo.collider.gameObject.SetActive(false);
                //hitInfo.transform.gameObject.GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 1.0f, 1.0f);
                
                //select_cubes.Add()

                //polygonに選択したcubeの情報を代入
                //mapの縦と横のセル数を取得
                int map_width = (int)map_data.GetMapData().x;
                int map_height = (int)map_data.GetMapData().y;
                //cubeの番号からmapのどこのブロックか計算して代入
                //ただ，pgm形式のデータに対応するために(x,y)=(height-y,x)に変換
                RosSharp.RosBridgeClient.Messages.Geometry.Point32 point = new RosSharp.RosBridgeClient.Messages.Geometry.Point32();
                //point.x = int.Parse(hitInfo.collider.gameObject.name) % map_width;
                //point.y = int.Parse(hitInfo.collider.gameObject.name) / map_width;
                point.x = (map_height-1) - (int.Parse(hitInfo.collider.gameObject.name) / map_width);
                point.y = int.Parse(hitInfo.collider.gameObject.name) % map_width;
                point.z = 254.0f;
                OVRDebugConsole.Log("x_pix" + map_width + "y_pix" + map_height);
                OVRDebugConsole.Log("x="+point.x.ToString() + " y="+point.y.ToString() + " z="+point.z.ToString());
                //OVRDebugConsole.Log("y="+point.y.ToString());
                //OVRDebugConsole.Log("z="+point.z.ToString());

                //List選択したcubeの情報を追加
                select_cubes.Add(point);
                OVRDebugConsole.Log("selectcubeadd");
                
            }
            else if (OVRInput.Get(OVRInput.RawButton.Back))
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
        //OVRDebugConsole.Log("" + select_cubes);
    }

    public List<RosSharp.RosBridgeClient.Messages.Geometry.Point32> GetSelectCubeList()
    {
        /*
        //Listのpointをpolygonに変換して送る
        RosSharp.RosBridgeClient.Messages.Geometry.Polygon polygon = new RosSharp.RosBridgeClient.Messages.Geometry.Polygon();
        polygon.points = new RosSharp.RosBridgeClient.Messages.Geometry.Point32[select_cubes.Count];
        polygon.points = select_cubes.ToArray();
        for(int i = 0; i < polygon.points.Length; i++)
        {
            OVRDebugConsole.Log("x=" + polygon.points[i].x.ToString());
            OVRDebugConsole.Log("y=" + polygon.points[i].y.ToString());
            OVRDebugConsole.Log("z=" + polygon.points[i].z.ToString());
        }
        OVRDebugConsole.Log("GetSelectCubePolygon");
        */
        return select_cubes;
    }

    public void ResetSelectCubeList()
    {
        select_cubes.Clear();
        OVRDebugConsole.Log("ResetSelectCubePolygon");
    }
}
