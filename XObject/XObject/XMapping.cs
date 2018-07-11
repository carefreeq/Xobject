using System;
using System.Collections.Generic;
using System.Text;

namespace XObject
{
    /// <summary>
    /// 贴图类型
    /// </summary>
    public enum TextureType
    {
        None,
        Diffuse,
        Normal,
        Reflection,
    }
    [Serializable]
    public class XMapping : XObject
    {
        /// <summary>
        /// 贴图映射ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 贴图比例
        /// </summary>
        public XVec2 Scale { get; set; }
        /// <summary>
        /// 贴图偏移
        /// </summary>
        public XVec2 Offset { get; set; }
        /// <summary>
        /// 贴图类型
        /// </summary>
        public TextureType Type { get; set; }
        public XMapping()
        {
            ID = -1;
            Scale = new XVec2();
            Offset = new XVec2();
            Type = TextureType.Diffuse;
        }
    }
}
