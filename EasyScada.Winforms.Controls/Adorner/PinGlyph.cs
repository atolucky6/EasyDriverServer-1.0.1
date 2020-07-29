using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design.Behavior;

namespace EasyScada.Winforms.Controls
{
    public sealed class PinGlyph : Glyph
    {
        public BehaviorService BehaviorService { get; private set; }
        public Control BaseControl { get; private set; }
        public ISelectionService SelectionService { get; private set; }
        public IDesigner Designer { get; private set; }
        public Adorner Adorner { get; private set; }

        public PinGlyph(
            BehaviorService behaviorService,
            Control baseControl,
            ISelectionService selectionService,
            IDesigner designer,
            Adorner adorner) : base(new PinBehavior(designer))
        {
            BehaviorService = behaviorService;
            SelectionService = selectionService;
            Designer = designer;
            Adorner = adorner;
            BaseControl = baseControl;
        }

        public override Rectangle Bounds
        {
            get
            {
                Point edge = BehaviorService.ControlToAdornerWindow(BaseControl);
                if (BaseControl is ISupportLinkable linkObj)
                {
                    if (linkObj.SelectedPinInfo != null)
                    {
                        Rectangle clientRect = new Rectangle(
                            edge.X + linkObj.SelectedPinInfo.Position.X, 
                            edge.Y + linkObj.SelectedPinInfo.Position.Y, 
                            10, 10);
                        return clientRect;
                    }
                }
                return Rectangle.Empty;
            }
        }

        public override Cursor GetHitTest(Point p)
        {
            if (Behavior is PinBehavior pinBehavior)
            {
                if (pinBehavior.IsDragging)
                    return Cursors.Hand;            }
            return null;
        }

        public override void Paint(PaintEventArgs pe)
        {
            if (BaseControl is ISupportLinkable linkObj)
            {
                if (linkObj.SelectedPinInfo != null)
                {
                    pe.Graphics.FillRectangle(Brushes.Red, Bounds);
                }
            }
        }
    }

    public class PinBehavior : Behavior
    {
        private readonly IDesigner designer;
        private readonly Control control;

        public bool IsDragging { get; private set; }
        public Point DragStartPoint { get; private set; }
        public PinBehavior(IDesigner designer)
        {
            this.designer = designer;
            control = designer.Component as Control;
        }

        public override bool OnMouseEnter(Glyph g)
        {
            return true;
        }

        public override bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc)
        {
            if (button == MouseButtons.Left)
            {
                IsDragging = true;
                DragStartPoint = mouseLoc;
            }
            return true;
        }

        public override bool OnMouseUp(Glyph g, MouseButtons button)
        {
            IsDragging = false;
            return true;
        }

        public override bool OnMouseMove(Glyph g, MouseButtons button, Point mouseLoc)
        {
            if (button != MouseButtons.Left)
                IsDragging = false;

            if (IsDragging)
            {
                PinGlyph pin = g as PinGlyph;
                Point edge = pin.BehaviorService.ControlToAdornerWindow(pin.BaseControl);
                Rectangle clientRect = new Rectangle(edge.X, edge.Y, pin.BaseControl.Width, pin.BaseControl.Height);
                int xDiff = mouseLoc.X - DragStartPoint.X;
                int yDiff = mouseLoc.Y - DragStartPoint.Y;
                if (pin.BaseControl is ISupportLinkable linkObj)
                {
                    if (linkObj.SelectedPinInfo != null)
                        linkObj.SelectedPinInfo.UpdatePosition(Point.Add(linkObj.SelectedPinInfo.Position, new Size(new Point(xDiff, yDiff))));
                }
                
                pin.Adorner.Invalidate();
            }

            return true;
        }
    }
}
