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
        //
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
        private GameObject Point_1;
        private GameObject Point_2;
        private Vector3[] GridPosition;
        private Vector3 Position;
        private bool[] GridFlag;
        private bool[] old_GridFlag;
        private Dictionary<uint, GameObject> GridList = new Dictionary<uint, GameObject>();
        private List<uint> change_number = new List<uint>();

        //デバック変数
        //障害物の数
        private int count = 0;
        //一つ目前障害物の数
        private int old_count = 0;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            GridFlag = new bool[0];
            old_GridFlag = new bool[0];
        }

        private void FixedUpdate()
        {
            if (receive_flag == 1)
            {
                createmap();

                //一つ前の障害物のデータを比較用に保存
                old_GridFlag = GridFlag;
            }

        }

        private void Update()
        {

        }

        //得られたデータから地図の位置にCubeを置く
        //old_GridFlagとGridFlagの長さが違うときは，最初の地図の更新または異なる地図の受信と考えてすべてのcubeを破壊して再生成
        //また，(数字,gameobject)のリストですべてを保持して変更する際には数字を参照する
        //同じ場合は変更がある場所だけ，破壊・生成を行う
        private void createmap()
        {
            //GridFlagからCubeを作成
            Point_1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Point_2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //Point.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            //Point.transform.localScale = new Vector3(resolution, resolution, resolution);
            //GameObject.Destroy(Point.GetComponent<BoxCollider>());

            GameObject Point_MESH = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //Point_MESH.transform.localScale = new Vector3((float)resolution, (float)resolution, (float)resolution);
            Mesh cubeMesh = Point_MESH.GetComponent<MeshFilter>().sharedMesh;
            Point_MESH.SetActive(false);
            Debug.Log(cubeMesh.vertexCount);
            //cubeMesh.bounds.Expand(0.01f);

            CombineInstance[] combineInstanceAry_1 = new CombineInstance[2000];
            CombineInstance[] combineInstanceAry_2 = new CombineInstance[1725];

            int j = 0;
            for (uint i = 0; i < data_length; i++)
            {
                if (GridFlag[i] == true)
                {
                    //位置の計算
                    //行の番号
                    int line = (int)i / width;
                    //列の番号
                    int raw = (int)i % width;
                    //行の位置の計算
                    //GridPosition[j].x = (line * resolution) - ((resolution * width)/2);
                    //Position.z = ((resolution / 2) + (line) * resolution)-20;
                    Position.y = ((resolution / 2) + (line) * resolution) + offset_y;
                    //Position.x = ((resolution / 2) + (line) * resolution);
                    //列の位置の計算
                    //GridPosition[j].z = (raw * resolution) - ((resolution * height)/2);
                    Position.x = ((resolution / 2) + (raw) * resolution) + offset_x;
                    //Position.y = ((resolution / 2) + (raw) * resolution);
                    Position.z = 0;
                    //ROSの座標系からUnityの座標系に変換
                    Position = Position.Ros2Unity();
                    if (j < 2000)
                    {
                        combineInstanceAry_1[j].mesh = cubeMesh;
                        combineInstanceAry_1[j].transform = Matrix4x4.TRS(new Vector3(Position.x, 0, Position.z), Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f)), new Vector3(resolution, resolution, resolution));
                    }
                    else
                    {
                        combineInstanceAry_2[j-2000].mesh = cubeMesh;
                        combineInstanceAry_2[j-2000].transform = Matrix4x4.TRS(new Vector3(Position.x, 0, Position.z), Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f)), new Vector3(resolution, resolution, resolution));
                    }
                    

                    j++;

                    /*
                    //Objectの生成
                    Point = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //Colliderの当たり判定の削除
                    //DestroyImmediate(Point.GetComponent<Collider>());
                    //nameはiを参照して設定
                    Point.name = "GridPoint" + i;
                    //tagはGridで設定
                    Point.tag = "Grid";
                    //このスクリプトが張られたオブジェクトを親にして，今回生成したオブジェクトが子になる
                    Point.transform.parent = transform;
                    //色をランダムに
                    //Point.GetComponent<Renderer>().material.color = new Color((float)(i % Random.Range(1.0f, 100.0f) * 0.01), (float)(i % Random.Range(1.0f, 100.0f) * 0.01), (float)(i % Random.Range(1.0f, 100.0f) * 0.01));
                    //Point.GetComponent<Renderer>().material.color = new Color((float)0, (float)0, (float)0);
                    Point.transform.localScale = new Vector3((float)resolution, (float)resolution, (float)resolution);
                    //位置は上で計算済みのものを使用
                    Point.transform.localPosition = Position;

                    //Listに追加
                    GridList.Add(i, Point);
                    */
                    
                }
                
            }
            Debug.Log(j);
            var combinedMesh_1 = new Mesh();
            var combinedMesh_2 = new Mesh();
            combinedMesh_1.name = "Cubes_1";
            combinedMesh_2.name = "Cubes_2";
            combinedMesh_1.CombineMeshes(combineInstanceAry_1);
            combinedMesh_2.CombineMeshes(combineInstanceAry_2);
            // 上書きする
            Point_1.GetComponent<MeshFilter>().mesh = combinedMesh_1;
            Point_2.GetComponent<MeshFilter>().mesh = combinedMesh_2;

            Debug.Log(combinedMesh_1.vertexCount);
            Debug.Log(combinedMesh_1.vertexBufferCount);

            receive_flag = 0;
            /*
            if (old_GridFlag.Length != GridFlag.Length)
            {
                //Gridタグのオブジェクトをすべて破壊
                var clones = GameObject.FindGameObjectsWithTag("Grid");
                if (clones != null)
                {
                    foreach (var clone in clones)
                    {
                        Destroy(clone);
                    }
                }
                //Listも初期化
                GridList.Clear();

                //GridFlagからCubeを作成
                for (uint i = 0; i < data_length; i++)
                {
                    if (GridFlag[i] == true)
                    {
                        //位置の計算
                        //行の番号
                        int line = (int)i / width;
                        //列の番号
                        int raw = (int)i % width;
                        //行の位置の計算
                        //GridPosition[j].x = (line * resolution) - ((resolution * width)/2);
                        //Position.z = ((resolution / 2) + (line) * resolution)-20;
                        Position.y = ((resolution / 2) + (line) * resolution) + offset_y;
                        //Position.x = ((resolution / 2) + (line) * resolution);
                        //列の位置の計算
                        //GridPosition[j].z = (raw * resolution) - ((resolution * height)/2);
                        Position.x = ((resolution / 2) + (raw) * resolution) + offset_x;
                        //Position.y = ((resolution / 2) + (raw) * resolution);
                        Position.z = 0;
                        Position = Position.Ros2Unity();

                        //Objectの生成
                        Point = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        //Colliderの当たり判定の削除
                        //DestroyImmediate(Point.GetComponent<Collider>());
                        //nameはiを参照して設定
                        Point.name = "GridPoint" + i;
                        //tagはGridで設定
                        Point.tag = "Grid";
                        //このスクリプトが張られたオブジェクトを親にして，今回生成したオブジェクトが子になる
                        Point.transform.parent = transform;
                        //色をランダムに
                        //Point.GetComponent<Renderer>().material.color = new Color((float)(i % Random.Range(1.0f, 100.0f) * 0.01), (float)(i % Random.Range(1.0f, 100.0f) * 0.01), (float)(i % Random.Range(1.0f, 100.0f) * 0.01));
                        //Point.GetComponent<Renderer>().material.color = new Color((float)0, (float)0, (float)0);
                        Point.transform.localScale = new Vector3((float)resolution, (float)resolution, (float)resolution);
                        //位置は上で計算済みのものを使用
                        Point.transform.localPosition = Position;

                        //Listに追加
                        GridList.Add(i, Point);
                    }
                }
            }
            else
            {
                for (uint i = 0; i < data_length; i++)
                {
                    if (old_GridFlag[i] != GridFlag[i])
                    {
                        if (GridFlag[i] == true)
                        {
                            //位置の計算
                            //行の番号
                            int line = (int)i / width;
                            //列の番号
                            int raw = (int)i % width;
                            //行の位置の計算
                            //GridPosition[j].x = (line * resolution) - ((resolution * width)/2);
                            //Position.z = ((resolution / 2) + (line) * resolution)-20;
                            Position.y = ((resolution / 2) + (line) * resolution) + offset_y;
                            //Position.x = ((resolution / 2) + (line) * resolution);
                            //列の位置の計算
                            //GridPosition[j].z = (raw * resolution) - ((resolution * height)/2);
                            Position.x = ((resolution / 2) + (raw) * resolution) + offset_x;
                            //Position.y = ((resolution / 2) + (raw) * resolution);
                            Position.z = 0;
                            Position = Position.Ros2Unity();

                            //Objectの生成
                            Point = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            //Colliderの当たり判定の削除
                            DestroyImmediate(Point.GetComponent<Collider>());
                            //nameはiを参照して設定
                            Point.name = "GridPoint" + i;
                            //tagはGridで設定
                            Point.tag = "Grid";
                            //このスクリプトが張られたオブジェクトを親にして，今回生成したオブジェクトが子になる
                            Point.transform.parent = transform;
                            //色をランダムに
                            Point.GetComponent<Renderer>().material.color = new Color((float)(i % Random.Range(1.0f, 100.0f) * 0.01), (float)(i % Random.Range(1.0f, 100.0f) * 0.01), (float)(i % Random.Range(1.0f, 100.0f) * 0.01));
                            Point.transform.localScale = new Vector3((float)resolution, (float)resolution, (float)resolution);
                            //位置は上で計算済みのものを使用
                            Point.transform.localPosition = Position;

                            //Listに追加
                            GridList.Add(i, Point);
                        }
                        else
                        {

                            //Debug.Log("single destroy");
                            //cubeを破壊
                            Destroy(GameObject.Find("GridPoint" + i));
                            //List上でも破壊
                            GridList.Remove(i);

                        }

                    }
                    else
                    {
                        //何もしない
                    }

                }
            }
            receive_flag = 0;
            */
        }


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
            //GridPoint = new GameObject[gridmap.data.Length];
            GridFlag = new bool[gridmap.data.Length];
            //grids[i]が100以上なら壁なのでGridFlagにtrueを入れる
            count = 0;
            for (uint i = 0; i < data_length; i++)
            {
                //Debug.Log(i);
                if (gridmap.data[i] > 99)
                {
                    GridFlag[i] = true;
                    count++;
                }
                else
                {
                    GridFlag[i] = false;
                }
            }
            receive_flag = 1;
            Debug.Log(count);
        }
    }
}
