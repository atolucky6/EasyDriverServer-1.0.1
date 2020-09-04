using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EasyScada.Core.Designer
{
    [TemplatePart(Name = "TransformRoot", Type = typeof(Grid)), TemplatePart(Name = "Presenter", Type = typeof(ContentPresenter))]
    public sealed class TabPositionElement : ContentControl
    {
        // Fields
        private const string TransformRootName = "TransformRoot";
        private const string PresenterName = "Presenter";
        private const double AcceptableDelta = 0.0001;
        private const int DecimalsAfterRound = 4;
        public static readonly DependencyProperty LayoutTransformProperty =
            DependencyProperty.Register("LayoutTransform", typeof(Transform), typeof(TabPositionElement), new PropertyMetadata(null, LayoutTransformChanged));
        private Panel _transformRoot;
        private ContentPresenter _contentPresenter;
        private MatrixTransform _matrixTransform;
        private Matrix _transformation;
        private Size _childActualSize = Size.Empty;

        // Methods
        public TabPositionElement()
        {
            base.DefaultStyleKey = typeof(TabPositionElement);
            base.IsTabStop = false;
            base.UseLayoutRounding = false;
        }

        public void ApplyLayoutTransform()
        {
            this.ProcessTransform(this.LayoutTransform);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            FrameworkElement child = this.Child;
            if ((this._transformRoot != null) && (child != null))
            {
                Size a = this.ComputeLargestTransformedSize(finalSize);
                if (IsSizeSmaller(a, this._transformRoot.DesiredSize))
                {
                    a = this._transformRoot.DesiredSize;
                }
                double introduced4 = a.Width;
                Rect rect = RectTransform(new Rect(0.0, 0.0, introduced4, a.Height), this._transformation);
                double introduced5 = a.Width;
                Rect finalRect = new Rect(-rect.Left + ((finalSize.Width - rect.Width) / 2.0), -rect.Top + ((finalSize.Height - rect.Height) / 2.0), introduced5, a.Height);
                this._transformRoot.Arrange(finalRect);
                if (IsSizeSmaller(a, child.RenderSize) && (Size.Empty == this._childActualSize))
                {
                    this._childActualSize = new Size(child.ActualWidth, child.ActualHeight);
                    base.InvalidateMeasure();
                    return finalSize;
                }
                this._childActualSize = Size.Empty;
            }
            return finalSize;
        }

        private Size ComputeLargestTransformedSize(Size arrangeBounds)
        {
            Size size = Size.Empty;
            bool flag = double.IsInfinity(arrangeBounds.Width);
            if (flag)
            {
                arrangeBounds.Width  = arrangeBounds.Height;
            }
            bool flag2 = double.IsInfinity(arrangeBounds.Height);
            if (flag2)
            {
                arrangeBounds.Height = arrangeBounds.Width;
            }
            double num = this._transformation.M11;
            double num2 = this._transformation.M12;
            double num3 = this._transformation.M21;
            double num4 = this._transformation.M22;
            double num5 = Math.Abs((double)(arrangeBounds.Width / num));
            double num6 = Math.Abs((double)(arrangeBounds.Width / num3));
            double num7 = Math.Abs((double)(arrangeBounds.Height / num2));
            double num8 = Math.Abs((double)(arrangeBounds.Height / num4));
            double num9 = num5 / 2.0;
            double num10 = num6 / 2.0;
            double num11 = num7 / 2.0;
            double num12 = num8 / 2.0;
            double num13 = -(num6 / num5);
            double num14 = -(num8 / num7);
            if ((0.0 == arrangeBounds.Width) || (0.0 == arrangeBounds.Height))
            {
                return new Size(arrangeBounds.Width, arrangeBounds.Height);
            }
            if (flag && flag2)
            {
                return new Size(double.PositiveInfinity, double.PositiveInfinity);
            }
            if (!MatrixHasInverse(this._transformation))
            {
                return new Size(0.0, 0.0);
            }
            if ((0.0 == num2) || (0.0 == num3))
            {
                double num15 = flag2 ? double.PositiveInfinity : num8;
                double num16 = flag ? double.PositiveInfinity : num5;
                if ((0.0 == num2) && (0.0 == num3))
                {
                    return new Size(num16, num15);
                }
                if (0.0 == num2)
                {
                    double num17 = Math.Min(num10, num15);
                    return new Size(num16 - Math.Abs((double)((num3 * num17) / num)), num17);
                }
                if (0.0 == num3)
                {
                    double num18 = Math.Min(num11, num16);
                    size = new Size(num18, num15 - Math.Abs((double)((num2 * num18) / num4)));
                }
                return size;
            }
            if ((0.0 == num) || (0.0 == num4))
            {
                double num19 = flag2 ? double.PositiveInfinity : num7;
                double num20 = flag ? double.PositiveInfinity : num6;
                if ((0.0 == num) && (0.0 == num4))
                {
                    return new Size(num19, num20);
                }
                if (0.0 == num)
                {
                    double num21 = Math.Min(num12, num20);
                    return new Size(num19 - Math.Abs((double)((num4 * num21) / num2)), num21);
                }
                if (0.0 == num4)
                {
                    double num22 = Math.Min(num9, num19);
                    size = new Size(num22, num20 - Math.Abs((double)((num * num22) / num3)));
                }
                return size;
            }
            if (num10 <= ((num14 * num9) + num8))
            {
                return new Size(num9, num10);
            }
            if (num12 <= ((num13 * num11) + num6))
            {
                return new Size(num11, num12);
            }
            double num23 = (num8 - num6) / (num13 - num14);
            return new Size(num23, (num13 * num23) + num6);
        }

        [Conditional("DIAGNOSTICWRITELINE")]
        private static void DiagnosticWriteLine(string message)
        {
        }

        private Matrix GetTransformMatrix(Transform transform)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return new Matrix();
            }
            if (transform != null)
            {
                TransformGroup group = transform as TransformGroup;
                if (group != null)
                {
                    Matrix matrix = Matrix.Identity;
                    foreach (Transform transform2 in group.Children)
                    {
                        matrix = MatrixMultiply(matrix, this.GetTransformMatrix(transform2));
                    }
                    return matrix;
                }
                RotateTransform transform3 = transform as RotateTransform;
                if (transform3 != null)
                {
                    double angle = transform3.Angle;
                    double a = (6.2831853071795862 * angle) / 360.0;
                    double num3 = Math.Sin(a);
                    double num4 = Math.Cos(a);
                    return new Matrix(num4, num3, -num3, num4, 0.0, 0.0);
                }
                ScaleTransform transform4 = transform as ScaleTransform;
                if (transform4 != null)
                {
                    double scaleX = transform4.ScaleX;
                    return new Matrix(scaleX, 0.0, 0.0, transform4.ScaleY, 0.0, 0.0);
                }
                SkewTransform transform5 = transform as SkewTransform;
                if (transform5 != null)
                {
                    double angleX = transform5.AngleX;
                    double angleY = transform5.AngleY;
                    double num9 = (6.2831853071795862 * angleX) / 360.0;
                    return new Matrix(1.0, (6.2831853071795862 * angleY) / 360.0, num9, 1.0, 0.0, 0.0);
                }
                MatrixTransform transform6 = transform as MatrixTransform;
                if (transform6 != null)
                {
                    return transform6.Matrix;
                }
            }
            return Matrix.Identity;
        }

        private static bool IsSizeSmaller(Size a, Size b)
        {
            if ((a.Width + 0.0001) >= b.Width)
            {
                return ((a.Height + 0.0001) < b.Height);
            }
            return true;
        }

        private static void LayoutTransformChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((TabPositionElement)o).ProcessTransform((Transform)e.NewValue);
        }

        private static bool MatrixHasInverse(Matrix matrix) =>
            !(0.0 == ((matrix.M11 * matrix.M22) - (matrix.M12 * matrix.M21)));

        private static Matrix MatrixMultiply(Matrix matrix1, Matrix matrix2) =>
            new Matrix((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21), (matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22), (matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21), (matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22), ((matrix1.OffsetX * matrix2.M11) + (matrix1.OffsetY * matrix2.M21)) + matrix2.OffsetX, ((matrix1.OffsetX * matrix2.M12) + (matrix1.OffsetY * matrix2.M22)) + matrix2.OffsetY);

        protected override Size MeasureOverride(Size availableSize)
        {
            Size size;
            FrameworkElement child = this.Child;
            if ((this._transformRoot == null) || (child == null))
            {
                return Size.Empty;
            }
            if (this._childActualSize == Size.Empty)
            {
                size = this.ComputeLargestTransformedSize(availableSize);
            }
            else
            {
                size = this._childActualSize;
            }
            this._transformRoot.Measure(size);
            Rect rect = RectTransform(new Rect(0.0, 0.0, this._transformRoot.DesiredSize.Width, this._transformRoot.DesiredSize.Height), this._transformation);
            double introduced6 = rect.Width;
            return new Size(introduced6, rect.Height);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this._transformRoot = base.GetTemplateChild("TransformRoot") as Grid;
            this._contentPresenter = base.GetTemplateChild("Presenter") as ContentPresenter;
            this._matrixTransform = new MatrixTransform();
            if (this._transformRoot != null)
            {
                this._transformRoot.RenderTransform = this._matrixTransform;
            }
            this.ApplyLayoutTransform();
        }

        private void ProcessTransform(Transform transform)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this._transformation = RoundMatrix(this.GetTransformMatrix(transform), 4);
                if (this._matrixTransform != null)
                {
                    this._matrixTransform.Matrix = this._transformation;
                }
                base.InvalidateMeasure();
            }
        }

        private static Rect RectTransform(Rect rect, Matrix matrix)
        {
            Point point = matrix.Transform(new Point(rect.Left, rect.Top));
            Point point2 = matrix.Transform(new Point(rect.Right, rect.Top));
            Point point3 = matrix.Transform(new Point(rect.Left, rect.Bottom));
            Point point4 = matrix.Transform(new Point(rect.Right, rect.Bottom));
            double num = Math.Min(Math.Min(point.X, point2.X), Math.Min(point3.X, point4.X));
            double num2 = Math.Min(Math.Min(point.Y, point2.Y), Math.Min(point3.Y, point4.Y));
            double num3 = Math.Max(Math.Max(point.X, point2.X), Math.Max(point3.X, point4.X));
            double num4 = Math.Max(Math.Max(point.Y, point2.Y), Math.Max(point3.Y, point4.Y));
            return new Rect(num, num2, num3 - num, num4 - num2);
        }

        private static Matrix RoundMatrix(Matrix matrix, int decimals) =>
            new Matrix(Math.Round(matrix.M11, decimals), Math.Round(matrix.M12, decimals), Math.Round(matrix.M21, decimals), Math.Round(matrix.M22, decimals), matrix.OffsetX, matrix.OffsetY);

        // Properties
        public Transform LayoutTransform
        {
            get =>
                ((Transform)base.GetValue(LayoutTransformProperty));
            set =>
                base.SetValue(LayoutTransformProperty, value);
        }

        private FrameworkElement Child
        {
            get
            {
                if (this._contentPresenter == null)
                {
                    return null;
                }
                return ((this._contentPresenter.Content as FrameworkElement) ?? this._contentPresenter);
            }
        }
    }
}
