using System;
using UnityEngine;

[ExecuteInEditMode]
public class MeshText : MonoBehaviour
{
    [SerializeField] private string _text = "";

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;


    public Material material;

    public MeshFontTable meshFontTable;

    [SerializeField] private float _scale = 1f;

    public float scale
    {
        get { return _scale; }
        set { _scale = value; }
    }

    [SerializeField] private Color _color1 = Color.white;

    public Color color1
    {
        get { return _color1; }
        set { _color1 = value; }
    }

    [SerializeField] private Color _color2 = Color.white;

    public Color color2
    {
        get { return _color2; }
        set { _color2 = value; }
    }

    public enum HorizontalAlignType
    {
        Left,
        Center,
        Right
    }

    //当text中存在宽度不一致的字体时，计算Center和Right会有误差。不过对于战斗HUD，够用了。
    public HorizontalAlignType HAlignType;

    public string Text
    {
        get { return _text; }
        set
        {
            _text = value;
            GenerateFilter();
        }
    }

    private void OnValidate()
    {
        Text = _text;
    }

    void Awake()
    {
        meshRenderer.sharedMaterial = material;
        if (!string.IsNullOrEmpty(_text))
        {
            GenerateFilter();
        }
    }

    public void GenerateFilter()
    {
        Mesh mesh = new Mesh();

        int length = Text.Length;
        Vector3[] vertices = new Vector3[length << 2];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[(length << 1) * 3];
        Color[] colors = new Color[vertices.Length];
        float tempX = 0;
        float[] rates = new float[length];
        Sprite[] sprites = new Sprite[length];
        for (int i = 0; i < length; i++)
        {
            uint c = Text[i];
            var mSprite = meshFontTable[c];
            sprites[i] = mSprite;
            rates[i] = mSprite.rect.width / mSprite.rect.height;
        }

        if (HAlignType == HorizontalAlignType.Left)
        {
            tempX = 0f;
        }
        else
        {
            var width = 0f;
            foreach (var rate in rates)
            {
                width += rate;
            }
            if (HAlignType == HorizontalAlignType.Center)
            {
                tempX = -width / 2f;
            }
            else if (HAlignType == HorizontalAlignType.Right)
            {
                tempX = -width;
            }
        }

        for (int i = 0; i < vertices.Length; i += 4)
        {
            var mSprite = sprites[i / 4]; 
            var r = rates[i / 4];
            //setting vertices

            vertices[i] = new Vector3(tempX, 0.5f) * _scale;
            vertices[i + 1] = new Vector3(tempX, -0.5f) * _scale;
            tempX += r;
            vertices[i + 2] = new Vector3(tempX, 0.5f) * _scale;
            vertices[i + 3] = new Vector3(tempX, -0.5f) * _scale;


            colors[i] = color1;
            colors[i + 1] = color2;
            colors[i + 2] = color1;
            colors[i + 3] = color2;


            Texture tex = mSprite.texture;
            //setting uvs

            Rect inner = new Rect(mSprite.rect.x + mSprite.border.x, mSprite.rect.y + mSprite.border.w,
                mSprite.rect.width - mSprite.border.x - mSprite.border.z,
                mSprite.rect.height - mSprite.border.y - mSprite.border.w);
            inner = ConvertToTexCoords(inner, tex.width, tex.height);

            uvs[i] = new Vector2(inner.xMin, inner.yMax);
            uvs[i + 1] = new Vector2(inner.xMin, inner.yMin);
            uvs[i + 2] = new Vector2(inner.xMax, inner.yMax);
            uvs[i + 3] = new Vector2(inner.xMax, inner.yMin);
        }

        for (int i = 0; i < triangles.Length; i += 6)
        {
            var tmp = (i / 3) << 1;
            triangles[i] = triangles[i + 3] = tmp;
            triangles[i + 1] = triangles[i + 5] = tmp + 3;
            triangles[i + 2] = tmp + 1;
            triangles[i + 4] = tmp + 2;
        }
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        meshFilter.mesh = mesh;
    }

    Rect ConvertToTexCoords(Rect rect, int width, int height)
    {
        float uMin = rect.x / (float)width;
        float vMin = 1f - (rect.y + rect.height) / (float)height;
        float uMax = (rect.x + rect.width) / (float)width;
        float vMax = 1f - rect.y / (float)height;

        return new Rect(uMin, vMin, uMax - uMin, vMax - vMin);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        DrawMesh();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        DrawMesh();
    }

    private void DrawMesh()
    {
        if (meshFilter == null)
        {
            return;
        }
        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            return;
        }
        int[] tris = mesh.triangles;
        for (int i = 0; i < tris.Length; i += 3)
        {
            Gizmos.DrawLine(convert2World(mesh.vertices[tris[i]]), convert2World(mesh.vertices[tris[i + 1]]));
            Gizmos.DrawLine(convert2World(mesh.vertices[tris[i]]), convert2World(mesh.vertices[tris[i + 2]]));
            Gizmos.DrawLine(convert2World(mesh.vertices[tris[i + 1]]), convert2World(mesh.vertices[tris[i + 2]]));
        }
    }

    private Vector3 convert2World(Vector3 src)
    {
        return transform.TransformPoint(src);
    }
}