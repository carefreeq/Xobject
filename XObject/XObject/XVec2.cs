using System;
using System.Collections.Generic;
using System.Text;

namespace XObject
{
    /// <summary>
    /// 二维向量
    /// </summary>
    [Serializable]
    public struct XVec2
    {
        public float x { get; set; }
        public float y { get; set; }
        public static XVec2 zero { get { return new XVec2(0f, 0f); } }
        public static XVec2 one { get { return new XVec2(1f, 1f); } }
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
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
        public XVec2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public XVec2(double x, double y)
        {
            this.x = (float)x;
            this.y = (float)y;
        }
        /// <summary>
        /// 点乘
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Dot(XVec2 a, XVec2 b)
        {
            return a.x * b.x + a.y * b.y;
        }
        public static XVec2 operator +(XVec2 a, XVec2 b)
        {
            return new XVec2(a.x + b.y, a.y + b.y);
        }
        public static XVec2 operator -(XVec2 a, XVec2 b)
        {
            return new XVec2(a.x - b.y, a.y - b.y);
        }
        public static XVec2 operator *(XVec2 v, float f)
        {
            return new XVec2(v.x * f, v.y * f);
        }
        public static XVec2 operator *(XVec2 a, XVec2 b)
        {
            return new XVec2(a.x * b.y, a.y * b.y);
        }
        public static XVec2 operator /(XVec2 v, float f)
        {
            return new XVec2(v.x / f, v.y / f);
        }
        public static XVec2 operator /(XVec2 a, XVec2 b)
        {
            return new XVec2(a.x / b.y, a.y / b.y);
        }
        public static implicit operator XVec2(XVec3 v)
        {
            return new XVec2(v.x, v.y);
        }
    }
}
