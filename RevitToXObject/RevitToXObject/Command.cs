using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.IO;
using System.Windows;
using Microsoft.Win32;
namespace RevitToXObject
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        public ExternalCommandData CommandData { get; private set; }
        public Document Document { get; private set; }
        public CustomExporter Exporter { get; private set; }
        public XRContext XRContext { get; private set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "场景导出到";
            sfd.Filter = "xr file(*.xrobj)|*.xrobj";
            sfd.FilterIndex = 2;
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() == true)
            {
                string p = sfd.FileName.ToString();
                CommandData = commandData;
                Document = commandData.Application.ActiveUIDocument.Document;
                View3D v = Get3DView(Document.ActiveView);
                if (v != null)
                {
                    XRContext = new XRContext(v, p);
                    Exporter = new CustomExporter(Document, XRContext);
                    Exporter.Export(v);
                }
                else
                {
                    TaskDialog.Show("错误", "没找到View3D");
                }
                return Result.Succeeded;
            }
            return Result.Failed;
        }
        private bool IsView(View v)
        {
            return v != null && v.ViewType == ViewType.ThreeD && v.CanBePrinted && !v.IsTemplate;
        }
        private View3D Get3DView(View v)
        {
            if (!IsView(v))
            {
                v = new FilteredElementCollector(Document).OfClass(typeof(View3D)).Cast<View3D>().Where((__v) => { return __v.CanBePrinted && !__v.IsTemplate && !__v.IsPerspective; }).FirstOrDefault();
                if (!IsView(v))
                    throw new Exception("找不到3d视图");
                CommandData.Application.ActiveUIDocument.ActiveView = v;
            }
            View3D _v = v as View3D;
            if (_v != null)
                return _v;
            return null;
        }
        //其他方法
        private void GeometryData(Element e)
        {
            if (e == null)
                return;
            Options op = CommandData.Application.Application.Create.NewGeometryOptions();
            //opt.View = currentView;
            op.ComputeReferences = false;
            GeometryElement ge = e.get_Geometry(op);

            IEnumerator<GeometryObject> os = ge.GetEnumerator();
            while (os.MoveNext())
            {
                GeometryObject o = os.Current;
                string geoType = o.GetType().Name;
                switch (geoType)
                {
                    case "Solid":
                        SolidData(o);
                        break;
                    case "Face":
                        break;
                    case "Mesh":
                        MeshData(o);
                        break;
                    case "Curve":
                    case "Line":
                    case "Arc":
                        break;
                    case "Profile":
                        break;
                    case "Element":
                        break;
                    case "Instance":
                        break;
                    case "Edge":
                        break;
                    default:
                        break;
                }
            }
        }
        private void SolidData(GeometryObject obj)
        {
            Solid solid = obj as Solid;
            if (null == solid)
            {
                return;
            }

            FaceArray faces = solid.Faces;
            if (faces.Size == 0)
            {
                return;
            }

            foreach (Face f in faces)
            {
                Mesh mesh = f.Triangulate();
                if (null == mesh)
                {
                    return;
                }
                MeshData(mesh);
            }
        }
        private void MeshData(GeometryObject obj)
        {
            Mesh mesh = obj as Mesh;
            if (null == mesh)
            {
                return;
            }
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < mesh.NumTriangles; i++)
            {

                MeshTriangle triangular = mesh.get_Triangle(i);
                List<XYZ> points = new List<XYZ>();
                for (int n = 0; n < 3; n++)
                {
                    XYZ point = triangular.get_Vertex(n);
                    points.Add(point);
                    str.Append(point.ToString() + "\n");
                }
                XYZ iniPoint = points[0];
                points.Add(iniPoint);
            }
            TaskDialog td = new TaskDialog("vertex");
            td.MainContent = str.ToString();
            td.Show();
        }
    }
}
