using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XObject
{
    /// <summary>
    /// 贴图纹理数据
    /// </summary>
    [Serializable]
    public class XRTexture : XObject
    {
        public byte[] Data { get; set; }
    }
}
