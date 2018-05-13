using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace XObject
{
    /// <summary>
    /// 对象基类
    /// </summary>
    [Serializable]
    public class XObject
    {
        public string Name { get; set; }
    }
}
