using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XObject
{
    /// <summary>
    /// 元素类
    /// </summary>
    [Serializable]
    public class XElement : XObject
    {
        /// <summary>
        /// 坐标
        /// </summary>
        public XVec3 Position { get; set; }
        /// <summary>
        /// 角度
        /// </summary>
        public XVec3 Euler { get; set; }
        /// <summary>
        /// 比例
        /// </summary>
        public XVec3 Scale { get; set; }
        /// <summary>
        /// 网格们
        /// </summary>
        public List<XMesh> Meshs { get; set; }
        /// <summary>
        /// 孩子们！
        /// </summary>
        public List<XElement> Children { get; set; }
        public XElement()
        {
            Name = "XRobject";
            Meshs = new List<XMesh>();
            Position = XVec3.zero;
            Euler = XVec3.zero;
            Scale = XVec3.one;
        }
        public XElement(string name)
        {
            Name = name;
            Meshs = new List<XMesh>();
            Children = new List<XElement>();
            Position = XVec3.zero;
            Euler = XVec3.zero;
            Scale = XVec3.one;
        }
    }
}
