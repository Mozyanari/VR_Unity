using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

namespace RosSharp.RosBridgeClient
{
    public class MeshCombineOccupancyGridSubscriber : Subscriber<Messages.Navigation.OccupancyGrid>
    {
        //mapトピックのデータ
        //dataの長さ
        private int data_length;
        //mapの分解能
        private float resolution;
        //高さ方向のcell数
        private int height;
        //横方向のcell数
        private int width;
        //mapの原点に対するオフセット
        private float offset_x;
        private float offset_y;

        //mapトピックのフラグ
        int receive_flag = 0;
        int create_flag = 0;
        int ready_flag = 0;

        //private float yaw;

        //mapのデータ
        private float[] grids;
        private GameObject base_grid;
        private GameObject[] GridPoint;
        
        
        private Vector3[] GridPosition;
        private sbyte[] GridFlag;
        private sbyte[] old_GridFlag;

        //wallに関するデータ
        private GameObject[] wall_mesh;
        private int wall_number;
        private int cube_count;
        public GameObject wall_object;

        //障害物の数，床の数
        private int wall_quantity = 0;
        private int floor_quantity = 0;

        //表示する結合したメッシュ
        private Mesh[] combineMesh;

        //robot位置
        //public GameObject RobotObject;

        

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            GridFlag = new sbyte[0];
            old_GridFlag = new sbyte[0];
        }

        private void FixedUpdate()
        {
            if (receive_flag == 1)
            {
                createmap();
                //一つ前の障害物のデータを比較用に保存
                //old_GridFlag = GridFlag;
                //createcollder();
            }

        }

        private void createmap()
        {
            //前回作られた物があれば削除する(メッシュとコライダー)
            var clones = GameObject.FindGameObjectsWithTag("Grid");
            if (clones != null)
            {
                foreach (var clone in clones)
                {
                    Destroy(clone);
                }
            }

            //受信フラグを折る
            receive_flag = 0;
            //使用するmeshを取得
            //Mesh cubeMesh = GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<MeshFilter>().sharedMesh;
            Mesh wallobject_mesh = wall_object.GetComponent<MeshFilter>().sharedMesh;
            //meshの頂点の数から一つのCombineInstanceの中に入るmeshの数wall_numberを設定
            wall_number = (65536 / wallobject_mesh.vertexCount);
            //メッシュを何個に分割すればいいか計算
            int mesh_count_wall = (wall_quantity / wall_number) + 1;

            //mesh_count_wallからCombineInstanceを動的生成
            CombineInstance[][] combineInstanceAry = new CombineInstance[mesh_count_wall][];
            for(int i=0;i<mesh_count_wall;i++)
            {
                combineInstanceAry[i] = new CombineInstance[wall_number];
            }

            int wall_count = 0;
            int floor_count = 0;
            for (int i = 0; i < data_length; i++)
            {
                if (GridFlag[i] == 1)
                {
                    //Debug.Log(wall_count);
                    //位置の計算
                    //行の番号
                    int line = i % width;
                    //列の番号
                    int raw = i / width;
                    //行列の位置の計算
                    Vector3 Position;
                    Position.x = ((resolution / 2) + (line) * resolution) + offset_x;
                    Position.y = ((resolution / 2) + (raw) * resolution) + offset_y;
                    Position.z = 0;
                    //ROSの座標系からUnityの座標系に変換
                    Position = Position.Ros2Unity();

                    //CombineInstanceに情報を入力
                    combineInstanceAry[wall_count / wall_number][wall_count % wall_number].mesh = wallobject_mesh;
                    combineInstanceAry[wall_count / wall_number][wall_count % wall_number].transform = Matrix4x4.TRS(new Vector3(Position.x, 0, Position.z), Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f)), new Vector3(resolution, resolution, resolution));

                    //Colliderを生成
                    GameObject Point;
                    //Point = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Point = GameObject.Instantiate(wall_object) as GameObject;
                    Point.name = i.ToString();
                    Point.tag = "Grid";
                    Point.transform.position = Position;
                    Point.transform.localScale = new Vector3((float)resolution, (float)resolution, (float)resolution);
                    //meshはCombineMeshesでまとめて作成するため削除
                    DestroyImmediate(Point.GetComponent<MeshFilter>());

                    wall_count++;

                }
                else if (GridFlag[i] == 0)
                {
                    //todo:メッシュが決定次第実装
                }
            }

            Mesh[] combineMesh = new Mesh[mesh_count_wall];
            wall_mesh = new GameObject[mesh_count_wall];
            //Instantiate(wall_mesh[1]);
            for (int i = 0; i< mesh_count_wall; i++)
            {
                combineMesh[i] = new Mesh();
                combineMesh[i].CombineMeshes(combineInstanceAry[i]);
                wall_mesh[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall_mesh[i].GetComponent<MeshRenderer>().material = wall_object.GetComponent<Renderer>().sharedMaterial;
                wall_mesh[i].GetComponent<MeshFilter>().mesh = combineMesh[i];
                wall_mesh[i].tag = "Grid";
                wall_mesh[i].name = "Mesh" + i;
            }
        }
        /*
        void createcollder()
        {
            //ロボットの位置を取得
            Vector3 robotposition = RobotObject.transform.position;

            //地図におけるロボットの位置を計算

        }
        */
        protected override void ReceiveMessage(Messages.Navigation.OccupancyGrid gridmap)
        {
            Debug.Log(gridmap.data.Length);

            //地図のパラメータを取得
            resolution = gridmap.info.resolution;
            offset_x = gridmap.info.origin.position.x;
            offset_y = gridmap.info.origin.position.y;
            //Debug.Log(resolution);
            //height方向のcellの数
            height = (int)gridmap.info.height;
            //Debug.Log(height);
            //width方向のセルの数
            width = (int)gridmap.info.width;
            //Debug.Log(width);
            //data数の取得
            data_length = gridmap.data.Length;

            //データ数に応じて，地図情報を保存する変数を動的生成
            GridFlag = new sbyte[data_length];

            //grids[i]が100以上なら壁，0なら床，それ以外は何もなし
            wall_quantity = 0;
            floor_quantity = 0;
            for (uint i = 0; i < data_length; i++)
            {
                //Debug.Log(i);
                if (gridmap.data[i] > 99)
                {
                    GridFlag[i] = 1;
                    wall_quantity++;
                }else if(gridmap.data[i] == 0)
                {
                    GridFlag[i] = 0;
                    floor_quantity++;
                }
                else
                {
                    GridFlag[i] = -1;
                }
            }
            receive_flag = 1;
            //Debug.Log(count);
        }

        public Vector3 GetMapData()
        {
            Vector3 map_data;
            map_data.x = width;
            map_data.y = height;
            map_data.z = resolution;
            return map_data;
        }
    }
}
