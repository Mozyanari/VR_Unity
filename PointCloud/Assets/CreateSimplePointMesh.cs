using UnityEngine;

public class CreateSimplePointMesh : MonoBehaviour
{

    int numPoints = 60000; // 点の個数
    float r = 1.0f; // 半径

    void Start()
    {
    }

    /// <summary>
    /// 球の表面にランダムに点を生成
    /// </summary>
    /// <param name="numPoints">点の数</param>
    /// <param name="radius">球の半径</param>
    /// <returns></returns>
    Mesh CreateSimpleSurfacePointMesh(int numPoints, float radius)
    {
        Vector3[] points = new Vector3[numPoints];
        int[] indecies = new int[numPoints];
        Color[] colors = new Color[numPoints]; // 追加

        for (int i = 0; i < numPoints; i++)
        {
            float z = Random.Range(-1.0f, 1.0f);
            float th = Mathf.Deg2Rad * Random.Range(0.0f, 360.0f);
            float x = Mathf.Sqrt(1.0f - z * z) * Mathf.Cos(th);
            float y = Mathf.Sqrt(1.0f - z * z) * Mathf.Sin(th);

            points[i] = new Vector3(x, y, z) * radius; // 頂点座標
            indecies[i] = i; // 配列番号をそのままインデックス番号に流用
            colors[i] = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)); // 追加
        }

        Mesh mesh = new Mesh
        {
            vertices = points
        };
        mesh.SetIndices(indecies, MeshTopology.Points, 0); // 1頂点が1インデックスの関係
        mesh.colors = colors; // 追加

        return mesh;
    }

    private void Update()
    {
        GetComponent<Renderer>().material.EnableKeyword("_PointSize");
        GetComponent<Renderer>().material.SetFloat("_PointSize", 1.0f);
        Mesh meshSurface = CreateSimpleSurfacePointMesh(numPoints, r);
        GetComponent<MeshFilter>().mesh = meshSurface;
    }
}