using UnityEngine;

public class Tarea2problema3 : MonoBehaviour
{
    public Material lineMaterial;
    private float a = 3.3f;
    private float h;

    private Vector4 C = new Vector4(-1.812f, -6.824f, 5.247f, 1); // centro de la base

    private Vector4[] vertices = new Vector4[4];

    private Vector4 centroide;

    void Start()
    {
        h = Mathf.Sqrt(2f / 3f) * a;
        float R = a / Mathf.Sqrt(3f);

        Vector4 b1 = new Vector4(
            C.x + R,
            C.y,
            C.z,
            1
        );

        Vector4 b2 = new Vector4(
            C.x - R * 0.5f,
            C.y,
            C.z + (a * Mathf.Sqrt(3f) * 0.5f),
            1
        );

        Vector4 b3 = new Vector4(
            C.x - R * 0.5f,
            C.y,
            C.z - (a * Mathf.Sqrt(3f) * 0.5f),
            1
        );

        Vector4 top = new Vector4(
            C.x,
            C.y + h,
            C.z,
            1
        );

        vertices[0] = b1;
        vertices[1] = b2;
        vertices[2] = b3;
        vertices[3] = top;

        centroide = (b1 + b2 + b3 + top) / 4f;
        centroide.w = 1;
    }

    void OnRenderObject()
    {
        if (lineMaterial == null)
            return;

        lineMaterial.SetPass(0);
        GL.PushMatrix();
        GL.Begin(GL.LINES);
        GL.Color(Color.black);

        float angleOriginal = 0;
        DrawPyramid(angleOriginal);

        //float angle = -15f * Mathf.Deg2Rad;
        //DrawPyramid(angle);

        GL.End();
        GL.PopMatrix();
    }

    void DrawPyramid(float angle)
    {
        Vector4[] v = new Vector4[4];

        for (int i = 0; i < 4; i++)
        {
            v[i] = Transformar(vertices[i], angle);
        }

        DrawLine(v[0], v[1]);
        DrawLine(v[1], v[2]);
        DrawLine(v[2], v[0]);

        DrawLine(v[0], v[3]);
        DrawLine(v[1], v[3]);
        DrawLine(v[2], v[3]);
    }

    Vector4 Transformar(Vector4 p, float angle)
    {
        Matrix4x4 T1 = MatrizTraslacion(-centroide.x, -centroide.y, -centroide.z);
        Matrix4x4 R = MatrizRotY(angle);
        Matrix4x4 T2 = MatrizTraslacion(centroide.x, centroide.y, centroide.z);

        Matrix4x4 M = T2 * R * T1;
        return M * p;
    }

    Matrix4x4 MatrizTraslacion(float tx, float ty, float tz)
    {
        Matrix4x4 T = Matrix4x4.identity;
        T[0, 3] = tx;
        T[1, 3] = ty;
        T[2, 3] = tz;
        return T;
    }

    Matrix4x4 MatrizRotY(float angle)
    {
        float c = Mathf.Cos(angle);
        float s = Mathf.Sin(angle);

        Matrix4x4 R = Matrix4x4.identity;

        R[0, 0] = c; R[0, 2] = s;
        R[2, 0] = -s; R[2, 2] = c;

        return R;
    }

    void DrawLine(Vector4 a, Vector4 b)
    {
        GL.Vertex3(a.x, a.y, a.z);
        GL.Vertex3(b.x, b.y, b.z);
    }
}
