using System;
using System.Drawing;

namespace EasyScada.Winforms.Controls.Charts
{
    public class AuxiliaryLine : IDisposable
    {
        private bool isDisposed;

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    Pen penDash = PenDash;
                    if (penDash != null)
                        penDash.Dispose();
                    Pen penSolid = PenSolid;
                    if (penSolid != null)
                        penSolid.Dispose();
                    Brush lineTextBrush = LineTextBrush;
                    if (lineTextBrush != null)
                        lineTextBrush.Dispose();
                }
                isDisposed = true;
            }
        }

        public Pen GetPen()
        {
            return IsDashStyle ? PenDash : PenSolid;
        }

        public float Value { get; set; }
        public float PaintValue { get; set; }
        public float PaintValueBackup { get; set; }
        public Color LineColor { get; set; }
        public Pen PenDash { get; set; }
        public Pen PenSolid { get; set; }
        public float LineThickness { get; set; }
        public Brush LineTextBrush { get; set; }
        public bool IsLeftFrame { get; set; }
        public bool IsDashStyle { get; set; }
    }
}
