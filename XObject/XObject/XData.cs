using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace XObject
{
    public enum FileType
    {
        D3Max,
        Maya,
        Revit,
        Unity,
    }
    /// <summary>
    /// XObject文件类
    /// </summary>
    [Serializable]
    public class XData
    {
        /// <summary>
        /// 文件扩展名
        /// </summary>
        public const string Extension = ".xo";
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public FileType Type { get; private set; }
        /// <summary>
        /// 元素们
        /// </summary>
        public List<XElement> Objects { get; set; }
        /// <summary>
        /// 材质们
        /// </summary>
        public List<XMaterial> Materials { get; set; }
        /// <summary>
        /// 贴图们
        /// </summary>
        public List<XTexture> Textures { get; set; }
        public XData(string name, FileType type)
        {
            Name = name;
            Type = type;
            Objects = new List<XElement>();
            Materials = new List<XMaterial>();
            Textures = new List<XTexture>();
        }
        /// <summary>
        /// 保存到指定路径
        /// </summary>
        /// <param name="path">路径</param>
        public void Write(string path)
        {
            using (FileStream fs = File.Open(path, FileMode.OpenOrCreate))
            {
                byte[] data = ObjectToByte(this);
                fs.Write(data, 0, data.Length);
            }
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns>数据</returns>
        public byte[] GetBytes()
        {
            return ObjectToByte(this);
        }
        /// <summary>
        /// 读取指定路径的文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static XData Read(string path)
        {
            if (File.Exists(path))
            {
                byte[] data = new byte[0];
                using (FileStream fs = File.Open(path, FileMode.Open))
                {
                    data = new byte[fs.Length];
                    fs.Read(data, 0, data.Length);
                }
                return (XData)ByteToObject(data);
            }
            throw new Exception("路径错误");
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static XData Read(byte[] data)
        {
            return (XData)ByteToObject(data);
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static byte[] ObjectToByte(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                return ms.GetBuffer();
            }
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static object ByteToObject(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return bf.Deserialize(ms);
            }
        }

    }
}
