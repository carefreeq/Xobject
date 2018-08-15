using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XObject;
namespace QQ
{
    public static class XOFormat
    {
        private static int _ColorId;
        private static int _MainTexId;
        //private static int _BumpMapId;
        private static Material mat_diffuse = Resources.Load<Material>("xoformat/diffuse");
        private static Material mat_transparent = Resources.Load<Material>("xoformat/transparent");
        static XOFormat()
        {
            _ColorId = Shader.PropertyToID("_Color");
            _MainTexId = Shader.PropertyToID("_MainTex");
            //_BumpMapId = Shader.PropertyToID("_BumpMap");
        }
        /// <summary>
        /// 导出.xobject
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="go">GameObject</param>
        public static void Export(string path, params GameObject[] go)
        {
            _Export(System.IO.Path.GetFileNameWithoutExtension(path), go).Write(path);
        }
        private static XData _Export(string n, params GameObject[] go)
        {
            XData d = new XData(n, FileType.Unity);
            List<Material> mats = new List<Material>();
            List<Texture2D> texs = new List<Texture2D>();
            for (int i = 0; i < go.Length; i++)
            {
                d.Objects.Add(CreateElement(go[i], mats));
            }
            for (int i = 0; i < mats.Count; i++)
            {
                Material m = mats[i];
                XMaterial xm = new XMaterial();
                xm.Color = m.GetColor(_ColorId).ToXColor();
                Texture2D t = m.GetTexture(_MainTexId) as Texture2D;
                if (t != null)
                {
                    XMapping xp = new XMapping();
                    xp.ID = texs.IndexOf(t);
                    if (xp.ID < 0)
                    {
                        xp.ID = texs.Count;
                        texs.Add(t);
                    }
                    xp.Type = TextureType.Diffuse;
                    xp.Offset = m.GetTextureOffset(_MainTexId).ToXVec2();
                    xp.Scale = m.GetTextureScale(_MainTexId).ToXVec2();
                    xm.Maps.Add(xp);
                }
                d.Materials.Add(xm);
            }
            for (int i = 0; i < texs.Count; i++)
            {
                Texture2D t = texs[i];
                Texture2D _t = new Texture2D(t.width, t.height);
                _t.SetPixels(t.GetPixels());
                XTexture xt = new XTexture();
                xt.Data = _t.EncodeToPNG();
                d.Textures.Add(xt);
            }
            return d;
        }
        private static XElement CreateElement(GameObject go, List<Material> mats)
        {
            XElement xe = new XElement(go.name);
            xe.Position = go.transform.localPosition.ToVec3();
            xe.Euler = go.transform.localEulerAngles.ToVec3();
            xe.Scale = go.transform.localScale.ToVec3();
            MeshFilter mf = go.GetComponent<MeshFilter>();
            if (mf != null)
            {
                Mesh m = mf.sharedMesh;
                Material[] ms = go.GetComponent<MeshRenderer>().sharedMaterials;
                for (int i = 0; i < m.subMeshCount; i++)
                {
                    XMesh xm = new XMesh();
                    xm.Name = go.name + i;
                    xm.Triangles = new List<int>(m.GetTriangles(i));

                    List<int> ct = xm.Triangles.Distinct().ToList();
                    xm.Vertexs = (from v in ct select m.vertices[v].ToVec3()).ToList();
                    xm.Normals = (from v in ct select m.normals[v].ToVec3()).ToList();
                    xm.UVs = (from v in ct select m.uv[v].ToXVec2()).ToList();
                    xm.Triangles = (from v in xm.Triangles select ct.IndexOf(v)).ToList();
                    xm.MaterialID = mats.IndexOf(ms[i]);
                    if (xm.MaterialID < 0)
                    {
                        xm.MaterialID = mats.Count;
                        mats.Add(ms[i]);
                    }
                    xe.Meshs.Add(xm);
                }
            }
            for (int i = 0; i < go.transform.childCount; i++)
            {
                xe.Children.Add(CreateElement(go.transform.GetChild(i).gameObject, mats));
            }
            return xe;
        }
        /// <summary>
        /// 加载.xobject
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>GameObject</returns>
        public static GameObject Import(string path)
        {
            return _Import(XData.Read(path));
        }
        /// <summary>
        /// 加载.xobject
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>GameObject</returns>
        public static GameObject Import(byte[] data)
        {
            return _Import(XData.Read(data));
        }
        private static GameObject _Import(XData d)
        {
            GameObject g = new GameObject(d.Name);

            List<Texture2D> t = new List<Texture2D>();
            for (int i = 0; i < d.Textures.Count; i++)
            {
                XTexture t0 = d.Textures[i];
                Texture2D t1 = new Texture2D(0, 0);
                t1.name = t0.Name;
                if (t1.LoadImage(t0.Data))
                    t.Add(t1);
                else
                    Debug.LogError("XRobject Texture Load Error!");
            }
            List<Material> m = new List<Material>();
            for (int i = 0; i < d.Materials.Count; i++)
            {
                XMaterial xm = d.Materials[i];
                Material _m;
                if (xm.Color.a > 0.0 && xm.Color.a < 1.0)
                {
                    _m = new Material(mat_transparent);
                }
                else
                {
                    _m = new Material(mat_diffuse);
                }
                _m.name = xm.Name;
                _m.color = new Color(xm.Color.r, xm.Color.g, xm.Color.b, xm.Color.a);
                //mat.SetColor("_SpecColor", m.Specular.ToColor());
                //mat.SetFloat("_Shininess", m.Shininess);
                foreach (XMapping xp in xm.Maps)
                {
                    string ts = null;
                    switch (xp.Type)
                    {
                        case TextureType.Diffuse:
                            ts = "_MainTex";
                            break;
                        case TextureType.Normal:
                            ts = "_BumpMap";
                            break;
                        case TextureType.Reflection:
                            break;
                    }
                    _m.SetTexture(ts, t[xp.ID]);
                    _m.SetTextureScale(ts, xp.Scale.ToVector2());
                    _m.SetTextureOffset(ts, xp.Offset.ToVector2());
                }
                m.Add(_m);
            }
            Vector3 min = Vector3.one * float.MaxValue;
            Vector3 max = Vector3.one * float.MinValue;
            CreateGameObject(d.Type, d.Objects, g.transform, m.ToArray(),ref min,ref max);

            switch (d.Type)
            {
                case FileType.Maya:
                case FileType.Unity:
                    break;
                case FileType.D3Max:
                case FileType.Revit:
                    g.transform.localScale = new Vector3(-1f, 1f, 1f);
                    g.transform.eulerAngles = new Vector3(-90f, 0.0f, 0.0f);
                    break;
            }
            BoxCollider b = g.AddComponent<BoxCollider>();
            b.size = max - min;
            b.center = (max + min) * 0.5f;
            return g;
        }
        private static void CreateGameObject(FileType t, List<XElement> ele, Transform parent, Material[] mats, ref Vector3 min, ref Vector3 max)
        {
            for (int i = 0; i < ele.Count; i++)
            {
                XElement xo = ele[i];
                GameObject g = new GameObject(xo.Name);
                g.transform.SetParent(parent.transform);
                g.transform.localPosition = new Vector3(xo.Position.x, xo.Position.y, xo.Position.z);
                g.transform.localRotation = Quaternion.Euler(xo.Euler.x, xo.Euler.y, xo.Euler.z);
                g.transform.localScale = new Vector3(xo.Scale.x, xo.Scale.y, xo.Scale.z);
                List<XMesh> xms = xo.Meshs;
                for (int _i = 0; _i < xms.Count; _i++)
                {
                    XMesh xm = xms[_i];
                    if (xm.Vertexs.Count > 60000)
                    {
                        Debug.Log("顶点数:" + xm.Vertexs.Count);
                    }
                    else
                    {
                        List<Vector3> vers = new List<Vector3>();
                        for (int __i = 0; __i < xm.Vertexs.Count; __i++)
                        {
                            vers.Add(xm.Vertexs[__i].ToVector3());
                        }
                        List<Vector2> uvs = new List<Vector2>();
                        for (int __i = 0; __i < xm.UVs.Count; __i++)
                        {
                            uvs.Add(xm.UVs[__i].ToVector2());
                        }
                        Mesh m = new Mesh();
                        m.SetVertices(vers);
                        m.SetUVs(0, uvs);
                        m.SetTriangles(xm.Triangles, 0);
                        m.RecalculateNormals();
                        min = Vector3.Min(min, m.bounds.min + g.transform.localPosition);
                        max = Vector3.Max(max, m.bounds.max + g.transform.localPosition);

                        GameObject _g = g;
                        if (xms.Count > 1)
                        {
                            _g = new GameObject(xo.Name + "_" + _i);
                            _g.transform.SetParent(g.transform, false);
                        }
                        switch (t)
                        {
                            case FileType.D3Max:
                            case FileType.Maya:
                            case FileType.Unity:
                            case FileType.Revit:
                                MeshFilter mf = _g.GetComponent<MeshFilter>();
                                if (!mf) mf = _g.AddComponent<MeshFilter>();
                                mf.mesh = m;
                                MeshRenderer mr = _g.GetComponent<MeshRenderer>();
                                if (!mr) mr = _g.AddComponent<MeshRenderer>();
                                mr.material = mats[xm.MaterialID];
                                break;
                        }
                    }
                }
                CreateGameObject(t, xo.Children, g.transform, mats, ref min, ref max);
            }
        }
    }

    public static class XObjectExtensions
    {
        public static Vector2 ToVector2(this XVec2 v)
        {
            return new Vector2(v.x, v.y);
        }
        public static XVec2 ToXVec2(this Vector2 v)
        {
            return new XVec2(v.x, v.y);
        }
        public static Vector3 ToVector3(this XVec3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }
        public static XVec3 ToVec3(this Vector3 v)
        {
            return new XVec3(v.x, v.y, v.z);
        }
        public static Color ToColor(this XColor c)
        {
            return new Color(c.r, c.g, c.b, c.a);
        }
        public static XColor ToXColor(this Color c)
        {
            return new XColor(c.r, c.g, c.b, c.a);
        }
    }
}