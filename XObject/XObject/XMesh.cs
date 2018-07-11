using System;
using System.Collections.Generic;
using System.Text;

namespace XObject
{
    /// <summary>
    /// 网格信息类
    /// </summary>
    [Serializable]
    public class XMesh : XObject
    {
        /// <summary>
        /// 三角索引
        /// </summary>
        public List<int> Triangles { get; set; }
        /// <summary>
        /// 顶点
        /// </summary>
        public List<XVec3> Vertexs { get; set; }
        /// <summary>
        /// 法线
        /// </summary>
        public List<XVec3> Normals { get; set; }
        /// <summary>
        /// UV
        /// </summary>
        public List<XVec2> UVs { get; set; }
        /// <summary>
        /// 材质索引ID
        /// </summary>
        public int MaterialID { get; set; }
        public XMesh()
        {
            Triangles = new List<int>();
            Vertexs = new List<XVec3>();
            Normals = new List<XVec3>();
            UVs = new List<XVec2>();
            MaterialID = 0;
        }
    }
}
