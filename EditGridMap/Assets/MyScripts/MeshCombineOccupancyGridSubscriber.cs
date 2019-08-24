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

        //目標FPS
        [SerializeField]
        public int target_FPS;

        //mapのデータ
        private float[] grids;
        private GameObject base_grid;
        private GameObject[] GridPoint;
        
        
        private Vector3[] GridPosition;
        private sbyte[] GridFlag;
        private sbyte[] old_GridFlag;

        //wallに関するデータ
        private GameObject[] wall_mesh;
        public GameObject wall_object;

        //floorに関するデータ
        private GameObject[] floor_mesh;
        public GameObject floor_object;

        //障害物の数，床の数
        private int wall_quantity = 0;
        private int floor_quantity = 0;

        //表示する結合したメッシュ
        private Mesh[] wall_combineMesh;
        private Mesh[] floor_combineMesh;

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
                StartCoroutine(createmap());
                //一つ前の障害物のデータを比較用に保存
                //old_GridFlag = GridFlag;
                //createcollder();
            }

        }

        private IEnumerator createmap()
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

            //使用する壁のmeshを取得
            //Mesh cubeMesh = GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<MeshFilter>().sharedMesh;
            Mesh wall_object_mesh = wall_object.GetComponent<MeshFilter>().sharedMesh;
            //meshの頂点の数から一つのCombineInstanceの中に入るmeshの数wall_numberを設定
            int wall_number = (65536 / wall_object_mesh.vertexCount);
            //メッシュを何個に分割すればいいか計算
            int wall_mesh_count = (wall_quantity / wall_number) + 1;

            //使用する床のmeshを取得
            Mesh floor_object_mesh = floor_object.GetComponent<MeshFilter>().sharedMesh;
            //meshの頂点の数から一つのCombineInstanceの中に入るmeshの数wall_numberを設定
            int floor_number = (65536 / floor_object_mesh.vertexCount);
            //メッシュを何個に分割すればいいか計算
            int floor_mesh_count = (floor_quantity / floor_number) + 1;

            //wall_mesh_countからCombineInstanceを動的生成
            CombineInstance[][] wall_combineInstanceAry = new CombineInstance[wall_mesh_count][];
            for(int i=0;i<wall_mesh_count;i++)
            {
                wall_combineInstanceAry[i] = new CombineInstance[wall_number];
            }
            //floor_mesh_countからCombineInstanceを動的生成
            CombineInstance[][] floor_combineInstanceAry = new CombineInstance[floor_mesh_count][];
            for (int i = 0; i < floor_mesh_count; i++)
            {
                floor_combineInstanceAry[i] = new CombineInstance[floor_number];
            }

            int wall_count = 0;
            int floor_count = 0;
            //地図生成時に全データをすべて探索するので，コルーチンを使いながら分割して処理する
            //目標FPSから1フレームにかけることが出来る時間を計算
            //float target_time = (1.0f / (float)target_FPS);
            float goNextFrameTime = Time.realtimeSinceStartup + 0.01f;

            for (int i = 0; i < data_length; i++)
            {
                // 10msec以上経過したら次フレームへ
                if (Time.realtimeSinceStartup >= goNextFrameTime)
                {
                    yield return null;
                    goNextFrameTime = Time.realtimeSinceStartup + 0.01f;
                }

                //壁の生成
                if (GridFlag[i] == 1)
                {
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
                    wall_combineInstanceAry[wall_count / wall_number][wall_count % wall_number].mesh = wall_object_mesh;
                    wall_combineInstanceAry[wall_count / wall_number][wall_count % wall_number].transform = Matrix4x4.TRS(Position, Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f)), new Vector3(resolution, resolution, resolution));

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
                //床の生成
                else if (GridFlag[i] == 0)
                {
                    //位置の計算
                    //行の番号
                    int line = i % width;
                    //列の番号
                    int raw = i / width;
                    //行列の位置の計算
                    Vector3 Position;
                    Position.x = ((resolution / 2) + (line) * resolution) + offset_x;
                    Position.y = ((resolution / 2) + (raw) * resolution) + offset_y;
                    Position.z = -resolution;
                    //ROSの座標系からUnityの座標系に変換
                    Position = Position.Ros2Unity();

                    //CombineInstanceに情報を入力
                    floor_combineInstanceAry[floor_count / floor_number][floor_count % floor_number].mesh = floor_object_mesh;
                    floor_combineInstanceAry[floor_count / floor_number][floor_count % floor_number].transform = Matrix4x4.TRS(Position, Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f)), new Vector3(resolution, resolution, resolution));

                    //Colliderを生成
                    GameObject Point;
                    //Point = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Point = GameObject.Instantiate(floor_object) as GameObject;
                    Point.name = i.ToString();
                    Point.tag = "Grid";
                    Point.transform.position = Position;
                    Point.transform.localScale = new Vector3((float)resolution, (float)resolution, (float)resolution);
                    //meshはCombineMeshesでまとめて作成するため削除
                    DestroyImmediate(Point.GetComponent<MeshFilter>());

                    floor_count++;
                }
            }
            //壁の結合したmeshの生成
            Mesh[] wall_combineMesh = new Mesh[wall_mesh_count];
            wall_mesh = new GameObject[wall_mesh_count];
            //Instantiate(wall_mesh[1]);
            for (int i = 0; i< wall_mesh_count; i++)
            {
                wall_combineMesh[i] = new Mesh();
                wall_combineMesh[i].CombineMeshes(wall_combineInstanceAry[i]);
                wall_mesh[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                DestroyImmediate(wall_mesh[i].GetComponent<Collider>());
                wall_mesh[i].GetComponent<MeshRenderer>().material = wall_object.GetComponent<Renderer>().sharedMaterial;
                wall_mesh[i].GetComponent<MeshFilter>().mesh = wall_combineMesh[i];
                wall_mesh[i].tag = "Grid";
                wall_mesh[i].name = "Wall" + i;
            }

            //床の結合したmeshの生成
            Mesh[] floor_combineMesh = new Mesh[floor_mesh_count];
            floor_mesh = new GameObject[floor_mesh_count];
            //Instantiate(floor_mesh[1]);
            for (int i = 0; i < floor_mesh_count; i++)
            {
                floor_combineMesh[i] = new Mesh();
                floor_combineMesh[i].CombineMeshes(floor_combineInstanceAry[i]);
                floor_mesh[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                DestroyImmediate(floor_mesh[i].GetComponent<Collider>());
                floor_mesh[i].GetComponent<MeshRenderer>().material = floor_object.GetComponent<Renderer>().sharedMaterial;
                floor_mesh[i].GetComponent<MeshFilter>().mesh = floor_combineMesh[i];
                floor_mesh[i].tag = "Grid";
                floor_mesh[i].name = "Floor" + i;
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
