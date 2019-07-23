using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubemaker : MonoBehaviour {

    private GameObject Point;

    public Shader cubeshader;

    // Use this for initialization
    void Start () {
        
        //Objectの生成
        for(int i = 0; i<100; i++)
        {
            for(int j = 0; j<10; j++)
            {
                //オブジェクトを生成
                Point = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //オブジェクトの位置設定
                Point.transform.position = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), 10);

                //Point.gameObject.
                //Colliderの当たり判定の削除
                //DestroyImmediate(Point.GetComponent<Collider>());
                Point.name = i.ToString() + j.ToString();

                //

            }
            
        }
        /*
        Point = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Point.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        GameObject.Destroy(Point.GetComponent<BoxCollider>());

        Mesh cubeMesh = Point.GetComponent<MeshFilter>().sharedMesh;
        CombineInstance[] combineInstanceAry = new CombineInstance[1000];

        for(int i = 0; i < 1000; i++)
        {
            combineInstanceAry[i].mesh = cubeMesh;
            combineInstanceAry[i].transform = Matrix4x4.Translate(new Vector3(Random.Range(-50,50), Random.Range(-50, 50), 5));
        }

        var combinedMesh = new Mesh();
        combinedMesh.name = "Cubes";
        combinedMesh.CombineMeshes(combineInstanceAry);
        // 上書きする
        Point.GetComponent<MeshFilter>().mesh = combinedMesh;
        */
        //meshは結合して，一方別にmesh無しのcolliderを用意する？
        //選択された場所にはそれぞれにboxを置く

        

        

    }

    // Update is called once per frame
    void Update () {
        /*
        for (int i = 0; i < 1000; i++)
        {
            Point = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Point.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Point.transform.position = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0);
        }
        */
    }
}
