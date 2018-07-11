using System;
using System.Collections.Generic;
using System.Text;

namespace XObject
{
    [Serializable]
    public class XColor
    {
        public float r { get; set; }
        public float g { get; set; }
        public float b { get; set; }
        public float a { get; set; }
        public static XColor white { get { return new XColor(1, 1, 1, 1); } }
        public float this[int i]
        {
            set
            {
                switch (i)
                {
                    case 0:
                        r = value;
                        break;
                    case 1:
                        g = value;
                        break;
                    case 2:
                        b = value;
                        break;
                    case 3:
                        a = value;
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
                        return r;
                    case 1:
                        return b;
                    case 2:
                        return g;
                    case 3:
                        return a;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
        public XColor()
        {
            r = 1f;
            g = 1f;
            b = 1f;
            a = 1f;
        }
        public XColor(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
        public XColor(double r, double g, double b, double a)
        {
            this.r = (float)r;
            this.g = (float)g;
            this.b = (float)b;
            this.a = (float)a;
        }
        public static XColor operator *(XColor c, float f)
        {
            return new XColor(c.r * f, c.g * f, c.b * f, c.a * f);
        }
        public static XColor operator *(XColor a, XColor b)
        {
            return new XColor(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);
        }
        public static XColor operator /(XColor c, float f)
        {
            return new XColor(c.r / f, c.g / f, c.b / f, c.a / f);
        }
        public static XColor operator /(XColor a, XColor b)
        {
            return new XColor(a.r / b.r, a.g / b.g, a.b / b.b, a.a / b.a);
        }
    }
}
