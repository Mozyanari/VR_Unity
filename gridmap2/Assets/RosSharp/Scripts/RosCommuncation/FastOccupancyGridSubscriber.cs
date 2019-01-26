using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

namespace RosSharp.RosBridgeClient
{
    public class FastOccupancyGridSubscriber : Subscriber<Messages.Navigation.OccupancyGrid>
    {
        //public Transform PublishedTransform;
        //public GameObject block;
        private int data_length;
        private float resolution;
        private int height;
        private int width;

        int receive_flag = 0;
        int create_flag = 0;
        int ready_flag = 0;

        //private float yaw;

        private float[] grids;
        private GameObject base_grid;
        private GameObject[] GridPoint;
        private GameObject Point;
        private Vector3[] GridPosition;
        private Vector3 Position;
        private bool[] GridFlag;
        private bool[] old_GridFlag;
        private Dictionary<uint,GameObject> GridList = new Dictionary<uint, GameObject>();
        private List<uint> change_number = new List<uint>();

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


                /*
                var clones = GameObject.FindGameObjectsWithTag("Grid");
                if (clones != null)
                {
                    foreach (var clone in clones)
                    {
                        Destroy(clone);
                    }
                }
                */

                createmap();

                //一つ前の障害物のデータを比較用に保存
                old_GridFlag = GridFlag;
            }

        }

        private void Update()
        {
            /*
            if (ready_flag > 500)
            {
                //地図の作成
                createmap();
            }
            */

            /*
            //Debug.Log(old_count);
            if (receive_flag == 1)
            {
                ready_flag++;
                //今表示してるcubeをすべて削除
                if (GridPoint.Length != 0)
                {
                    Debug.Log("Destroy");
                    Debug.Log(GridPoint.Length);
                    for (int i = 0; i < GridPoint.Length; i++)
                    {
                        //Debug.Log(i);
                        //GameObject.DestroyImmediate(GridPoint[i].transform);
                        //DestroyObject(GridPoint[i].gameObject);
                        DestroyImmediate(GridPoint[i], true);
                        //GridPoint[i].transform.DetachChildren();
                    }
                }
            }
            */
            


        }
        /*
        protected void OnDisable()
        {
            if (GridPoint.Length != 0)
            {
                Debug.Log("Destroy");
                Debug.Log(GridPoint.Length);
                for (int i = 0; i < GridPoint.Length; i++)
                {
                    //Component component = GridPoint[i].GetComponent();
                    Destroy(GridPoint[i]);
                    //GridPoint[i].transform.DetachChildren();
                }
            }
        }
        */

        private void createmap()
        {
            //得られたデータから地図の位置にCubeを置く
            //old_GridFlagとGridFlagの長さが違うときは，最初の地図の更新または異なる地図の受信と考えてすべてのcubeを破壊して再生成
            //また，(数字,gameobject)のリストですべてを保持して変更する際には数字を参照する
            //同じ場合は変更がある場所だけ，破壊・生成を行う
            if (old_GridFlag.Length != GridFlag.Length)
            {
                //Debug.Log("all destroy");
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
                        Position.x = ((resolution / 2) + (line) * resolution) - 20;
                        //列の位置の計算
                        //GridPosition[j].z = (raw * resolution) - ((resolution * height)/2);
                        Position.z = ((resolution / 2) + (raw) * resolution) - 20;

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
                        Point.transform.localScale = new Vector3((float)0.05, (float)0.05, (float)0.05);
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
                        if(GridFlag[i] == true)
                        {
                            //位置の計算
                            //行の番号
                            int line = (int)i / width;
                            //列の番号
                            int raw = (int)i % width;
                            //行の位置の計算
                            //GridPosition[j].x = ((line - 1) * resolution) - ((resolution * width)/2);
                            Position.x = ((resolution / 2) + (line) * resolution) - 20;
                            //列の位置の計算
                            //GridPosition[j].z = ((raw - 1) * resolution) - ((resolution * height)/2);
                            Position.z = ((resolution / 2) + (raw) * resolution) - 20;

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
                            Point.transform.localScale = new Vector3((float)0.05, (float)0.05, (float)0.05);
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
        }


        protected override void ReceiveMessage(Messages.Navigation.OccupancyGrid gridmap)
        {
            Debug.Log(gridmap.data.Length);
            

            //地図のパラメータを取得
            resolution = gridmap.info.resolution;
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
            for (uint i = 0; i < data_length; i++)
            {
                //Debug.Log(i);
                if (gridmap.data[i] > 99)
                {
                    GridFlag[i] = true;
                }
                else
                {
                    GridFlag[i] = false;
                }
                /*
                if(old_GridFlag.Length != GridFlag.Length)
                {
                    if
                }
                */
                //cubeの個数もカウント
                //count++;
            }

            //old_GridFlagと比較して番号を保存しておく

            //Debug.Log(grids[1]);

            //データの数だけオブジェクトを生成
            //GridPoint = new GameObject[count];
            //GridPosition = new Vector3[count];


            //障害物のポジションを取得
            //int j = 0;
            //int height_number = (int)(height / resolution);
            //int width_number = (int)(width / resolution);
            //Debug.Log(height_number);
            //Debug.Log(width_number);
            /*
            for (int i = 0; i < gridmap.data.Length; i++)
            {
                if (gridmap.data[i] > 99)
                {
                    //行の番号
                    int line = i / width;
                    //列の番号
                    int raw = i % width;
                    //行の位置の計算
                    //GridPosition[j].x = ((line - 1) * resolution) - ((resolution * width)/2);
                    GridPosition[j].x = ((resolution / 2) + (line) * resolution) - 20;
                    //列の位置の計算
                    //GridPosition[j].z = ((raw - 1) * resolution) - ((resolution * height)/2);
                    GridPosition[j].z = ((resolution / 2) + (raw) * resolution) - 20;

                    j++;
                    if (j > count)
                    {
                        Debug.Log("countERR");
                    }
                }
                //grids[i] = gridmap.data[i];
            }
            */


            receive_flag = 1;








        }
    }
}
