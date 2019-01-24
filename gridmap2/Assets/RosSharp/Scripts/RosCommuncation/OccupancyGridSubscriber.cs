using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class OccupancyGridSubscriber : Subscriber<Messages.Navigation.OccupancyGrid>
    {
        //public Transform PublishedTransform;
        public GameObject block;
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
        private Vector3[] GridPosition;

        //障害物の数
        private int count = 0;
        //一つ目前障害物の数
        private int old_count = 0;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            //base_grid = GameObject.CreatePrimitive(PrimitiveType.Cube);
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
            if (receive_flag == 1)
            {
                var clones = GameObject.FindGameObjectsWithTag("Grid");
                if(clones != null) {
                    foreach (var clone in clones)
                    {
                        Destroy(clone);
                    }
                }

                createmap();

            }
            


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
            //オブジェクトの初期化
            for (int i = 0; i < count; i++)
            {
                
                //Cubeのオブジェクトを生成
                GridPoint[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //衝突判定をなくすためにColliderを削除
                DestroyImmediate(GridPoint[i].GetComponent<Collider>());
                GridPoint[i].name = "GridPoint"+i;
                GridPoint[i].tag = "Grid";
                //このスクリプトが張られたオブジェクトを親にして，今回生成したオブジェクトが子になる
                GridPoint[i].transform.parent = transform;
                //マテリアルをParticles/Additiveにする
                //GridPoint[i].GetComponent<Renderer>().material = new Material(Shader.Find("Particles/Additive"));
                
                /*
                //オブジェクト生成の高速化
                GridPoint[i] = Instantiate(block,transform);
                //衝突判定をなくすためにColliderを削除
                DestroyImmediate(GridPoint[i].GetComponent<Collider>());
                GridPoint[i].name = "GridPoint" + i;
                GridPoint[i].tag = "Grid";
                */
            }

            //オブジェクトの設定
            for (int i = 0; i < count; i++)
            {
                //GridPoint[i].SetActive(ranges[i] != 0);
                //GridPoint[i].GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                GridPoint[i].GetComponent<Renderer>().material.color = new Color((float)(i % Random.Range(1.0f, 100.0f) * 0.01), (float)(i % Random.Range(1.0f, 100.0f) * 0.01), (float)(i % Random.Range(1.0f, 100.0f) * 0.01));
                //GridPoint[i].transform.localScale = objectWidth * Vector3.one;
                GridPoint[i].transform.localScale = new Vector3((float)0.1, (float)0.1, (float)0.1);
                GridPoint[i].transform.localPosition = GridPosition[i];
            }
            /*
            for (int i = 0; i < count; i++)
            {
                GridPoint[i] = Instantiate(base_grid, GridPosition[i], Quaternion.identity);
                GridPoint[i].name = "GridPoint" + i;
                GridPoint[i].transform.parent = transform;
            }
            */


            ready_flag = 0;
            receive_flag = 0;
        }


        protected override void ReceiveMessage(Messages.Navigation.OccupancyGrid gridmap)
        {
            Debug.Log(gridmap.data.Length);

            //一つ前の障害物の数を削除用に保存
            old_count = count;
            //障害物の数をリセット
            count = 0;

            //地図のパラメータを取得
            resolution = gridmap.info.resolution;
            //Debug.Log(resolution);
            //height方向のcellの数
            height = (int)gridmap.info.height;
            //Debug.Log(height);
            //width方向のセルの数
            width = (int)gridmap.info.width;
            //Debug.Log(width);
            //dataの取得
            grids = new float[gridmap.data.Length];
            //grids[i]が100以上なら壁なのでその数を計算
            for (int i = 0; i < gridmap.data.Length; i++)
            {
                //Debug.Log(i);
                if (gridmap.data[i] > 99)
                {
                    count++;
                }
                //grids[i] = gridmap.data[i];
            }
            //Debug.Log(grids[1]);

            //データの数だけオブジェクトを生成
            GridPoint = new GameObject[count];
            GridPosition = new Vector3[count];


            //障害物のポジションを取得
            int j = 0;
            //int height_number = (int)(height / resolution);
            //int width_number = (int)(width / resolution);
            //Debug.Log(height_number);
            //Debug.Log(width_number);

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

            receive_flag = 1;








        }
    }
}
