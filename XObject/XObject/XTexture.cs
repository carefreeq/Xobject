using System;
using System.Collections.Generic;
using System.Text;

namespace XObject
{
    /// <summary>
    /// 贴图纹理数据
    /// </summary>
    [Serializable]
    public class XTexture : XObject
    {
        public byte[] Data { get; set; }
    }
}
