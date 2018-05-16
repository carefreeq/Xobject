using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Lighting;
using Autodesk.Revit.Attributes;
using System.Runtime.Serialization;
using Autodesk.Revit.Utility;
using System.Text.RegularExpressions;
using System.IO;
using XObject;
namespace RevitToXObject
{

    public class XRContext : IExportContext
    {
        public View3D View { get; private set; }
        public string FilePath { get; private set; }
        public Options Options { get; private set; }
        public Stack<Document> Documents { get; private set; }
        public Stack<Transform> Transforms { get; private set; }
        public XData XData { get; private set; }

        private ElementId id;
        private int matid = 0;
        private string localMap = @"C:\Program Files (x86)\Common Files\Autodesk Shared\Materials\Textures";
        private Dictionary<int, int> matLink = new Dictionary<int, int>();
        private Dictionary<string, int> texLink = new Dictionary<string, int>();
        public static bool EnableLog { get; set; }

        List<string> meshLog = new List<string>();
        List<string> matLog = new List<string>();
        public XRContext(View3D view, string path)
        {
            View = view;
            FilePath = path;
            Documents = new Stack<Document>();
            Transforms = new Stack<Transform>();
            XData = new XData(Path.GetFileNameWithoutExtension(path), FileType.Revit);
            EnableLog = false;
        }

        public bool Start()
        {
            if (View != null)
            {
                Options = View.Document.Application.Create.NewGeometryOptions();
                Options.ComputeReferences = true;
                Options.DetailLevel = View.DetailLevel;

                Documents.Push(View.Document);
                Transforms.Push(Transform.Identity);

                return true;
            }
            return false;
        }
        public void Finish()
        {
            XData.Write(FilePath);
            if (EnableLog)
            {
                StringBuilder s = new StringBuilder();
                foreach (string l in meshLog)
                    s.Append(l + "\n");
                foreach (string l in matLog)
                    s.Append(l + "\n");
                File.WriteAllText("D://revitlog.txt", s.ToString());
            }
            TaskDialog.Show("保存成功", "成功导出为XR文件\n元素:" + XData.Objects.Count + "个\n多边形:" + meshLog.Count + " 个\n材质:" + matLog.Count + "个\n贴图:" + XData.Textures.Count + "个");
        }

        public bool IsCanceled()
        {
            return false;
        }

        public RenderNodeAction OnElementBegin(ElementId elementId)
        {
            id = elementId;
            Element ele = Documents.Peek().GetElement(elementId);
            GeometryElement ge = ele.get_Geometry(Options);
            if (ge != null && !ge.Any<GeometryObject>())
            {
                return RenderNodeAction.Skip;
            }

            XElement obj = new XElement();
            obj.Name = ele.Name;
            XData.Objects.Add(obj);
            return RenderNodeAction.Proceed;
        }

        public void OnElementEnd(ElementId elementId)
        {

        }

        public RenderNodeAction OnFaceBegin(FaceNode node)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnFaceEnd(FaceNode node)
        {

        }

        public RenderNodeAction OnInstanceBegin(InstanceNode node)
        {
            Transforms.Push(Transforms.Peek().Multiply(node.GetTransform()));
            return RenderNodeAction.Proceed;
        }

        public void OnInstanceEnd(InstanceNode node)
        {
            Transforms.Pop();
        }

        public void OnLight(LightNode node)
        {
            //LightType l = LightType.GetLightTypeFromInstance(Documents.Peek(), id);

            //Asset a = node.GetAsset();
        }

        public RenderNodeAction OnLinkBegin(LinkNode node)
        {
            Documents.Push(node.GetDocument());
            Transforms.Push(Transforms.Peek().Multiply(node.GetTransform()));
            return RenderNodeAction.Proceed;
        }

        public void OnLinkEnd(LinkNode node)
        {
            Documents.Pop();
            Transforms.Pop();
        }

        private void LogMaterial(AssetProperty asset, StringBuilder log, string t = null)
        {
            if (asset == null)
                return;
            if (EnableLog)
                log.Append(t + "Asset.Type:" + asset.Type.ToString() + "::" + asset.Name + "=");
            switch (asset.Type)
            {
                case AssetPropertyType.APT_Asset:
                    Asset a = asset as Asset;
                    if (EnableLog)
                        log.Append("Asset,Size:" + a.Size + "\n");
                    for (int i = 0; i < a.Size; i++)
                        LogMaterial(a[i], log, t + "\t");
                    break;
                case AssetPropertyType.APT_Boolean:
                    AssetPropertyBoolean ab = asset as AssetPropertyBoolean;
                    if (EnableLog)
                        log.Append(ab.Value + "\n");
                    break;
                case AssetPropertyType.APT_Distance:
                    AssetPropertyDistance ad = asset as AssetPropertyDistance;
                    if (EnableLog)
                        log.Append(ad.Value + "\n");
                    break;
                case AssetPropertyType.APT_Double:
                    AssetPropertyDouble ado = asset as AssetPropertyDouble;
                    if (EnableLog)
                        log.Append(ado.Value + "\n");
                    break;
                case AssetPropertyType.APT_Double44:
                    break;
                case AssetPropertyType.APT_DoubleArray2d:
                    AssetPropertyDoubleArray2d ado2 = asset as AssetPropertyDoubleArray2d;
                    if (EnableLog)
                        log.Append(ado2.Value.get_Item(0) + "," + ado2.Value.get_Item(1) + "\n");
                    break;
                case AssetPropertyType.APT_DoubleArray3d:
                    AssetPropertyDoubleArray3d ado3 = asset as AssetPropertyDoubleArray3d;
                    if (EnableLog)
                        log.Append(ado3.Value.get_Item(0) + "," + ado3.Value.get_Item(1) + "," + ado3.Value.get_Item(2) + "\n");
                    break;
                case AssetPropertyType.APT_DoubleArray4d:
                    AssetPropertyDoubleArray4d ado4 = asset as AssetPropertyDoubleArray4d;
                    if (EnableLog)
                        log.Append(ado4.Value.get_Item(0) + "," + ado4.Value.get_Item(1) + "," + ado4.Value.get_Item(2) + "," + ado4.Value.get_Item(3) + "\n");
                    break;
                case AssetPropertyType.APT_Enum:
                    AssetPropertyEnum ae = asset as AssetPropertyEnum;
                    if (EnableLog)
                        log.Append(ae.Value + "\n");
                    break;
                case AssetPropertyType.APT_Float:
                    AssetPropertyFloat af = asset as AssetPropertyFloat;
                    if (EnableLog)
                        log.Append(af.Value + "\n");
                    break;
                case AssetPropertyType.APT_FloatArray:
                    IList<float> lf = (asset as AssetPropertyFloatArray).GetValue();
                    if (EnableLog)
                    {
                        foreach (float f in lf)
                            log.Append(f + ",");
                        log.Append("\n");
                    }
                    break;
                case AssetPropertyType.APT_Int64:
                    AssetPropertyInt64 ai6 = asset as AssetPropertyInt64;
                    if (EnableLog)
                        log.Append(ai6.Value + "\n");
                    break;
                case AssetPropertyType.APT_Integer:
                    AssetPropertyInteger ai = asset as AssetPropertyInteger;
                    if (EnableLog)
                        log.Append(ai.Value + "\n");
                    break;
                case AssetPropertyType.APT_List:
                    break;
                case AssetPropertyType.APT_Properties:
                    AssetProperties ap = asset as AssetProperties;
                    if (EnableLog)
                        log.Append("AssetProperties,Count:" + ap.Size + "\n");
                    for (int i = 0; i < ap.Size; i++)
                        LogMaterial(ap[i], log, t + "\t");
                    break;
                case AssetPropertyType.APT_Reference:
                    break;
                case AssetPropertyType.APT_String:
                    AssetPropertyString _as = asset as AssetPropertyString;
                    if (EnableLog)
                        log.Append(_as.Value + "\n");
                    break;
                case AssetPropertyType.APT_Time:
                    AssetPropertyTime at = asset as AssetPropertyTime;
                    if (EnableLog)
                        log.Append(at.Value + "\n");
                    break;
                case AssetPropertyType.APT_UInt64:
                    AssetPropertyUInt64 aiu6 = asset as AssetPropertyUInt64;
                    if (EnableLog)
                        log.Append(aiu6.Value + "\n");
                    break;
                case AssetPropertyType.APT_Unknown:
                    if (EnableLog)
                        log.Append("\n");
                    break;
                default:
                    if (EnableLog)
                        log.Append("\n");
                    break;
            }
            foreach (Asset _a in asset.GetAllConnectedProperties())
            {
                if (EnableLog)
                    log.Append(t + "GetAllConnectedProperties:\n");
                LogMaterial(_a, log, t + "\t");
            }
        }

        private XMapping ReadTexture(AssetProperty asset)
        {
            switch (asset.Type)
            {
                case AssetPropertyType.APT_DoubleArray4d:
                    TextureType t = TextureType.None;
                    switch (asset.Name)
                    {
                        case "generic_diffuse":
                            t = TextureType.Diffuse;
                            break;
                        case "generic_bump_map":
                            t = TextureType.Normal;
                            break;
                    }
                    if (t != TextureType.None)
                    {
                        IList<AssetProperty> la = asset.GetAllConnectedProperties();
                        if (la.Count > 0)
                        {
                            Asset a = la[0] as Asset;
                            XMapping m = new XMapping();
                            m.Type = t;
                            XVec2 s = new XVec2();
                            XVec2 o = new XVec2();
                            for (int i = 0; i < a.Size; i++)
                            {
                                AssetProperty _a = a[i];
                                switch (_a.Name)
                                {
                                    case "unifiedbitmap_Bitmap":
                                        AssetPropertyString aps = _a as AssetPropertyString;
                                        string p = aps.Value.Split('|')[0];
                                        if (!p.Contains(":"))
                                        {
                                            p = Path.Combine(localMap, p);
                                        }
                                        if (texLink.ContainsKey(p))
                                        {
                                            m.ID = texLink[p];
                                        }
                                        else
                                        {
                                            if (File.Exists(p))
                                                using (FileStream fs = new FileStream(p, FileMode.Open, FileAccess.Read))
                                                {
                                                    XTexture tex = new XTexture();
                                                    tex.Name = fs.Name;
                                                    tex.Data = new byte[fs.Length];
                                                    fs.Read(tex.Data, 0, (int)fs.Length);
                                                    m.ID = XData.Textures.Count;
                                                    texLink.Add(p, m.ID);
                                                    XData.Textures.Add(tex);
                                                }
                                            else
                                                return null;
                                        }
                                        break;
                                    case "texture_RealWorldScaleX":
                                        AssetPropertyDistance asx = _a as AssetPropertyDistance;
                                        s.x = 1 / (float)UnitUtils.Convert(asx.Value, asx.DisplayUnitType, DisplayUnitType.DUT_DECIMAL_FEET);
                                        break;
                                    case "texture_RealWorldScaleY":
                                        AssetPropertyDistance asy = _a as AssetPropertyDistance;
                                        s.y = 1 / (float)UnitUtils.Convert(asy.Value, asy.DisplayUnitType, DisplayUnitType.DUT_DECIMAL_FEET);
                                        break;
                                    case "texture_RealWorldOffsetX":
                                        AssetPropertyDistance aox = _a as AssetPropertyDistance;
                                        o.x = s.x * (float)UnitUtils.Convert(aox.Value, aox.DisplayUnitType, DisplayUnitType.DUT_DECIMAL_FEET);
                                        break;
                                    case "texture_RealWorldOffsetY":
                                        AssetPropertyDistance aoy = _a as AssetPropertyDistance;
                                        o.y = s.y * (float)UnitUtils.Convert(aoy.Value, aoy.DisplayUnitType, DisplayUnitType.DUT_DECIMAL_FEET);
                                        break;
                                }
                            }
                            m.Scale = s;
                            m.Offset = o;
                            return m;
                        }
                    }
                    break;
            }
            return null;
        }
        public void OnMaterial(MaterialNode node)
        {
            matid = node.MaterialId.IntegerValue;
            if (!matLink.ContainsKey(matid))
            {
                StringBuilder log = new StringBuilder();
                XMaterial mat = new XMaterial();
                Element e = Documents.Peek().GetElement(node.MaterialId);
                if (e is Material)
                {
                    Material m = e as Material;
                    mat.Name = m.Name;
                    if (m.UseRenderAppearanceForShading)
                        mat.Color = new XColor(1.0f, 1.0f, 1.0f, 1 - (float)node.Transparency);
                    else
                        mat.Color = new XColor(m.Color.Red / 255f, m.Color.Green / 255f, m.Color.Blue / 255f, 1 - (float)node.Transparency);
                    mat.Specular = XColor.white * (m.Smoothness / 100f);
                    mat.Shininess = m.Shininess / 100f;
                    Element _e = Documents.Peek().GetElement(m.AppearanceAssetId);
                    if (EnableLog)
                    {
                        log.Append("Material:" + m.Name + "\n");
                        log.Append("AppearanceAssetElement:" + (_e is AppearanceAssetElement).ToString() + "\n");
                    }
                    if (_e is AppearanceAssetElement)
                    {
                        AppearanceAssetElement aae = _e as AppearanceAssetElement;
                        Asset a = aae.GetRenderingAsset();
                        if (EnableLog)
                            log.Append("Asset.Size:" + a.Size.ToString() + "\n");
                        if (a.Size > 0)
                        {
                            for (int i = 0; i < a.Size; i++)
                            {
                                if (EnableLog)
                                    LogMaterial(a[i], log);
                                XMapping t = ReadTexture(a[i]);
                                if (t != null)
                                {
                                    mat.Maps.Add(t);
                                }
                            }
                        }
                    }
                }
                else
                {
                    mat.Name = node.NodeName;
                    mat.Color = new XColor(node.Color.Red / 255f, node.Color.Green / 255f, node.Color.Blue / 255f, 1 - (float)node.Transparency);
                    mat.Specular = new XColor(0, 0, 0, 1);
                    mat.Shininess = 0f;
                }
                matLog.Add(log.ToString());
                matLink.Add(matid, XData.Materials.Count);
                XData.Materials.Add(mat);
            }
        }

        public void OnPolymesh(PolymeshTopology node)
        {
            XMesh m = new XMesh();
            m.Name = "Mesh" + XData.Objects.Last().Meshs.Count;
            Transform t = Transforms.Peek();
            for (int i = 0; i < node.NumberOfFacets; i++)
            {
                PolymeshFacet f = node.GetFacet(i);
                m.Triangles.Add(f.V1);
                m.Triangles.Add(f.V2);
                m.Triangles.Add(f.V3);
            }
            for (int i = 0; i < node.NumberOfPoints; i++)
            {
                XYZ p = t.OfPoint(node.GetPoint(i));
                m.Vertexs.Add(new XVec3(p.X, p.Y, p.Z));
            }
            for (int i = 0; i < node.NumberOfNormals; i++)
            {
                XYZ n = t.OfVector(node.GetNormal(i));
                m.Normals.Add(new XVec3(n.X, n.Y, n.Z));
            }
            for (int i = 0; i < node.NumberOfUVs; i++)
            {
                UV u = node.GetUV(i);
                m.UVs.Add(new XVec2(u.U, u.V));
            }
            m.MaterialID = matLink[matid];
            XData.Objects.Last().Meshs.Add(m);
            string log = null;
            if (EnableLog)
                log = "Mesh\n,Name:" + m.Name + "\nVertex:" + m.Vertexs.Count;
            meshLog.Add(log);
        }

        public void OnRPC(RPCNode node)
        {

        }

        public RenderNodeAction OnViewBegin(ViewNode node)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnViewEnd(ElementId elementId)
        {

        }
        private void ExportLight()
        {

        }
    }
}
