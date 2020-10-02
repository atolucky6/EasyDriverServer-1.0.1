using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls.Charts
{
    public static class ChartHelper
    {
        public static SizeF ConvertSize(SizeF size, float angle)
        {
            Matrix matrix = new Matrix();
            matrix.Rotate(angle);
            PointF[] pts = new PointF[4];
            pts[0].X = -size.Width / 2f;
            pts[0].Y = -size.Height / 2f;
            pts[1].X = -size.Width / 2f;
            pts[1].Y = size.Height / 2f;
            pts[2].X = size.Width / 2f;
            pts[2].Y = size.Height / 2f;
            pts[3].X = size.Width / 2f;
            pts[3].Y = -size.Height / 2f;
            matrix.TransformPoints(pts);
            float maxValue = float.MaxValue;
            float minValue = float.MinValue;
            float yMax = float.MaxValue;
            float yMin = float.MinValue;
            foreach (PointF tf in pts)
            {
                if (tf.X < maxValue)
                {
                    maxValue = tf.X;
                }
                if (tf.X > minValue)
                {
                    minValue = tf.X;
                }
                if (tf.Y < yMax)
                {
                    yMax = tf.Y;
                }
                if (tf.Y > yMin)
                {
                    yMin = tf.Y;
                }
            }
            return new SizeF(minValue - maxValue, yMax - yMin);
        }

        public static void DrawString(Graphics g, string s, Font font, Brush brush, PointF point, StringFormat format, float angle)
        {
            Matrix transform = g.Transform;
            Matrix matrix2 = g.Transform;
            matrix2.RotateAt(angle, point);
            g.Transform = matrix2;
            g.DrawString(s, font, brush, point, format);
            g.Transform = transform;
        }

        public static void AddArrayData<T>(ref T[] array, T[] data, int max)
        {
            if (array.Length == max)
            {
                Array.Copy(array, data.Length, array, 0, array.Length - data.Length);
                Array.Copy(data, 0, array, array.Length - data.Length, data.Length);
            }
            else if (array.Length + data.Length > max)
            {
                T[] localArray = new T[max];
                int index = 0;
                while (true)
                {
                    if (index >= max - data.Length)
                    {
                        int i = 0;
                        while(true)
                        {
                            if (i >= data.Length)
                            {
                                array = localArray;
                                break;
                            }
                            localArray[localArray.Length - data.Length + i] = data[i];
                            i++;
                        }
                        break;
                    }
                    localArray[index] = array[index + array.Length - max + data.Length];
                    index++;
                }
            }
            else
            {
                T[] localArray = new T[array.Length + data.Length];
                int index = 0;
                while(true)
                {
                    if (index >= array.Length)
                    {
                        int i = 0;
                        while(true)
                        {
                            if (i >= data.Length)
                            {
                                array = localArray;
                                break;
                            }
                            localArray[localArray.Length - data.Length + i] = data[i];
                            i++;
                        }
                        break;
                    }
                    localArray[index] = array[index];
                    index++;
                }
            }
        }

        public static int CalculateMaxSectionFrom(int[] values)
        {
            int num4;
            int num = values.Max();
            if (num <= 5)
            {
                num4 = 5;
            }
            else if (num <= 10)
            {
                num4 = 10;
            }
            else
            {
                int digit = num.ToString().Length - 2;
                int num3 = int.Parse(num.ToString().Substring(0, 2));
                if (num3 < 12)
                {
                    num4 = 12 * GetPow(digit);
                }
                else if (num3 < 14)
                {
                    num4 = 14 * GetPow(digit);
                }
                else if (num3 < 16)
                {
                    num4 = 16 * GetPow(digit);
                }
                else if (num3 < 18)
                {
                    num4 = 18 * GetPow(digit);
                }
                else if (num3 < 20)
                {
                    num4 = 20 * GetPow(digit);
                }
                else if (num3 < 22)
                {
                    num4 = 22 * GetPow(digit);
                }
                else if (num3 < 24)
                {
                    num4 = 24 * GetPow(digit);
                }
                else if (num3 < 26)
                {
                    num4 = 26 * GetPow(digit);
                }
                else if (num3 < 28)
                {
                    num4 = 28 * GetPow(digit);
                }
                else if (num3 < 30)
                {
                    num4 = 30 * GetPow(digit);
                }
                else if (num3 < 40)
                {
                    num4 = 40 * GetPow(digit);
                }
                else if (num3 < 50)
                {
                    num4 = 50 * GetPow(digit);
                }
                else if (num3 < 60)
                {
                    num4 = 60 * GetPow(digit);
                }
                else if (num3 < 80)
                {
                    num4 = 80 * GetPow(digit);
                }
                else
                {
                    num4 = 100 * GetPow(digit);
                }
            }
            return num4;
        }

        public static float ComputePaintLocationY(int max, int min, int height, int value)
        {
            float num;
            if ((max - min) == 0f)
            {
                num = height;
            }
            else
            {
                num = height - ((((value - min) * 1f) / ((float)(max - min))) * height);
            }
            return num;
        }

        public static float ComputePaintLocationY(float max, float min, float height, float value)
        {
            float num;
            if ((max - min) == 0f)
            {
                num = height;
            }
            else
            {
                num = height - (((value - min) / (max - min)) * height);
            }
            return num;
        }

        public static Color GetColorLight(Color color)
        {
            return Color.FromArgb(color.R + (((255 - color.R) * 40) / 100), color.G + (((255 - color.G) * 40) / 100), color.B + (((255 - color.B) * 40) / 100));
        }

        public static Color GetColorLightFive(Color color)
        {
            return Color.FromArgb(color.R + (((255 - color.R) * 50) / 100), color.G + (((255 - color.G) * 50) / 100), color.B + (((255 - color.B) * 50) / 100));
        }

        public static PointF[] GetPointsFrom(string points, float soureWidth, float sourceHeight, float width, float height, float dx = 0f, float dy = 0f)
        {
            char[] separator = new char[] { ' ' };
            string[] strArray = points.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            PointF[] tfArray = new PointF[strArray.Length];
            for (int i = 0; i < strArray.Length; i++)
            {
                int index = strArray[i].IndexOf(',');
                float num3 = Convert.ToSingle(strArray[i].Substring(0, index));
                float num4 = Convert.ToSingle(strArray[i].Substring(index + 1));
                tfArray[i] = new PointF((width * (num3 + dx)) / soureWidth, (height * (num4 + dy)) / sourceHeight);
            }
            return tfArray;
        }

        private static int GetPow(int digit)
        {
            int num = 1;
            for (int i = 0; i < digit; i++)
            {
                num *= 10;
            }
            return num;
        }

        public static Point[] GetRhombusFromRectangle(Rectangle rect)
        {
            return new Point[] { new Point(rect.X, rect.Y + (rect.Height / 2)), new Point(rect.X + (rect.Width / 2), (rect.Y + rect.Height) - 1), new Point((rect.X + rect.Width) - 1, rect.Y + (rect.Height / 2)), new Point(rect.X + (rect.Width / 2), rect.Y), new Point(rect.X, rect.Y + (rect.Height / 2)) };
        }

        public static void PaintCoordinateDivide(Graphics g, Pen penLine, Pen penDash, Font font, Brush brush, StringFormat sf, int degree, int max, int min, int width, int height, int left = 60, int right = 8, int up = 8, int down = 8)
        {
            for (int i = 0; i <= degree; i++)
            {
                int num2 = (((max - min) * i) / degree) + min;
                int num3 = (((int)ComputePaintLocationY(max, min, (height - up) - down, num2)) + up) + 1;
                g.DrawLine(penLine, left - 1, num3, left - 4, num3);
                if (i != 0)
                {
                    g.DrawLine(penDash, left, num3, width - right, num3);
                }
                g.DrawString(num2.ToString(), font, brush, new Rectangle(-5, num3 - (font.Height / 2), left, font.Height), sf);
            }
        }

        public static void PaintTriangle(Graphics g, Brush brush, Point point, int size, GraphDirection direction)
        {
            Point[] points = new Point[4];
            if (direction == GraphDirection.Leftward)
            {
                points[0] = new Point(point.X, point.Y - size);
                points[1] = new Point(point.X, point.Y + size);
                points[2] = new Point(point.X - (2 * size), point.Y);
            }
            else if (direction == GraphDirection.Rightward)
            {
                points[0] = new Point(point.X, point.Y - size);
                points[1] = new Point(point.X, point.Y + size);
                points[2] = new Point(point.X + (2 * size), point.Y);
            }
            else if (direction == GraphDirection.Upward)
            {
                points[0] = new Point(point.X - size, point.Y);
                points[1] = new Point(point.X + size, point.Y);
                points[2] = new Point(point.X, point.Y - (2 * size));
            }
            else
            {
                points[0] = new Point(point.X - size, point.Y);
                points[1] = new Point(point.X + size, point.Y);
                points[2] = new Point(point.X, point.Y + (2 * size));
            }
            points[3] = points[0];
            g.FillPolygon(brush, points);
        }

        public static void PaintTriangle(Graphics g, Brush brush, PointF point, int size, GraphDirection direction)
        {
            PointF[] points = new PointF[4];
            if (direction == GraphDirection.Leftward)
            {
                points[0] = new PointF(point.X, point.Y - size);
                points[1] = new PointF(point.X, point.Y + size);
                points[2] = new PointF(point.X - (2 * size), point.Y);
            }
            else if (direction == GraphDirection.Rightward)
            {
                points[0] = new PointF(point.X, point.Y - size);
                points[1] = new PointF(point.X, point.Y + size);
                points[2] = new PointF(point.X + (2 * size), point.Y);
            }
            else if (direction == GraphDirection.Upward)
            {
                points[0] = new PointF(point.X - size, point.Y);
                points[1] = new PointF(point.X + size, point.Y);
                points[2] = new PointF(point.X, point.Y - (2 * size));
            }
            else
            {
                points[0] = new PointF(point.X - size, point.Y);
                points[1] = new PointF(point.X + size, point.Y);
                points[2] = new PointF(point.X, point.Y + (2 * size));
            }
            points[3] = points[0];
            g.FillPolygon(brush, points);
        }

    }
}
