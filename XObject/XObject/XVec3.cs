using System;
using System.Collections.Generic;
using System.Text;

namespace XObject
{
    /// <summary>
    /// 三维向量
    /// </summary>
    [Serializable]
    public struct XVec3
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public static XVec3 zero { get { return new XVec3(0f, 0f, 0f); } }
        public static XVec3 one { get { return new XVec3(1f, 1f, 1f); } }
        public float this[int i]
        {
            set
            {
                switch (i)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            get
            {
                switch (i)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
        public XVec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public XVec3(double x, double y, double z)
        {
            this.x = (float)x;
            this.y = (float)y;
            this.z = (float)z;
        }
        public static float Dot(XVec3 a, XVec3 b)
        {
            return a.x * b.x + a.y * b.y + a.z + b.z;
        }
        public static XVec3 Cross(XVec3 a, XVec3 b)
        {
            return new XVec3(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
        }
        public static XVec3 operator +(XVec3 a, XVec3 b)
        {
            return new XVec3(a.x + b.y, a.y + b.y, a.z + b.z);
        }
        public static XVec3 operator -(XVec3 a, XVec3 b)
        {
            return new XVec3(a.x - b.y, a.y - b.y, a.z - b.z);
        }
        public static XVec3 operator *(XVec3 v, float f)
        {
            return new XVec3(v.x * f, v.y * f, v.z * f);
        }
        public static XVec3 operator *(XVec3 a, XVec3 b)
        {
            return new XVec3(a.x * b.y, a.y * b.y, a.z * b.z);
        }
        public static XVec3 operator /(XVec3 v, float f)
        {
            return new XVec3(v.x / f, v.y / f, v.z / f);
        }
        public static XVec3 operator /(XVec3 a, XVec3 b)
        {
            return new XVec3(a.x / b.y, a.y / b.y, a.z / b.z);
        }
    }
}
