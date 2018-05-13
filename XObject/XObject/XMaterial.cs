using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XObject
{
    /// <summary>
    /// 材质类
    /// </summary>
    [Serializable]
    public class XMaterial : XObject
    {
        /// <summary>
        /// 材质颜色
        /// </summary>
        public XColor Color { get; set; }
        /// <summary>
        /// 材质高光颜色
        /// </summary>
        public XColor Specular { get; set; }
        /// <summary>
        /// 材质光泽度
        /// </summary>
        public float Shininess { get; set; }
        /// <summary>
        /// 材质贴图
        /// </summary>
        public List<XMapping> Maps { get; set; }
        public XMaterial()
        {
            Color = new XColor();
            Specular = new XColor();
            Shininess = 0f;
            Maps = new List<XMapping>();
        }
    }
}
