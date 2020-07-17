// *****************************************************************************
// 
//  © Component Factory Pty Ltd 2017. All rights reserved.
//  The software and associated documentation supplied hereunder are the 
//  proprietary information of Component Factory Pty Ltd, 13 Swallows Close, 
//  Mornington, Vic 3931, Australia and are supplied subject to licence terms.
// 
//  Version 4.6.0.0 	www.ComponentFactory.com
// *****************************************************************************

using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using System.Collections;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls
{
    #region Delegates
    /// <summary>
    /// Signature of a bare method.
    /// </summary>
    public delegate void SimpleCall();

    /// <summary>
    /// Signature of a method that performs an operation.
    /// </summary>
    /// <param name="parameter">Operation parameter.</param>
    /// <returns>Operation result.</returns>
    public delegate object Operation(object parameter);

    /// <summary>
    /// Signature of a method that returns a ToolStripRenderer instance.
    /// </summary>
    public delegate ToolStripRenderer GetToolStripRenderer();
    #endregion

    /// <summary>
	/// Set of common helper routines for the Toolkit
	/// </summary>
    public static class CommonHelper
    {
        #region Private static members

        private static readonly ColorMatrix _matrixDisabled = new ColorMatrix(new float[][]{new float[]{0.3f,0.3f,0.3f,0,0},
                                                                                            new float[]{0.59f,0.59f,0.59f,0,0},
                                                                                            new float[]{0.11f,0.11f,0.11f,0,0},
                                                                                            new float[]{0,0,0,0.5f,0},
                                                                                            new float[]{0,0,0,0,1}});

        private static PropertyInfo _cachedDesignModePI;
        private static readonly Padding _inheritPadding = new Padding(-1);
        private static Rectangle _nullRectangle = new Rectangle(Int32.MaxValue, Int32.MaxValue, 0, 0);
        private static Point _nullPoint = new Point(Int32.MaxValue, Int32.MaxValue);
        private static int nextId = 1000;

        #endregion

        #region Public static members

        public static void SetInvoke<T>(this T control, Action<T> setAction)
            where T : Control
        {
            if (control.InvokeRequired)
            {
                MethodInvoker methodInvoker = delegate
                {
                    setAction(control);
                };
                control.Invoke(methodInvoker);
            }
            else
            {
                setAction(control);
            }
        }

        public static async void SetInvokeAsync<T>(this T control, Action<T> setAction)
            where T : Control
        {
            await Task.Run(() =>
            {
                SetInvoke(control, setAction);
            });
        }

        /// <summary>
        /// Gets the next global identifier in sequence.
        /// </summary>
        public static int NextId
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return nextId++; }
        }

        /// <summary>
        /// Gets the padding value used when inheritance is needed.
        /// </summary>
        public static Padding InheritPadding
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _inheritPadding; }
        }

        /// <summary>
        /// Gets a value indicating if the provided value is an override state.
        /// </summary>
        /// <param name="state">Specific state.</param>
        /// <returns>True if an override state; otherwise false.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static bool IsOverrideState(PaletteState state)
        {
            return (state & PaletteState.Override) == PaletteState.Override;
        }

        /// <summary>
        /// Gets a value indicating if the provided value is an override state but excludes one value.
        /// </summary>
        /// <param name="state">Specific state.</param>
        /// <param name="exclude">State that should be excluded from test.</param>
        /// <returns>True if an override state; otherwise false.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static bool IsOverrideStateExclude(PaletteState state, PaletteState exclude)
        {
            return (state != exclude) && IsOverrideState(state);
        }

        /// <summary>
        /// Color matrix used to adjust colors to look disabled.
        /// </summary>
        public static ColorMatrix MatrixDisabled
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _matrixDisabled; }
        }

        /// <summary>
        /// Gets access to the global null rectangle value.
        /// </summary>
        public static Rectangle NullRectangle
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _nullRectangle; }
        }

        /// <summary>
        /// Gets access to the global null point value.
        /// </summary>
        public static Point NullPoint
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _nullPoint; }
        }

        #endregion

        #region Public static methods

        /// <summary>
        /// Check the provided string is a number or not
        /// </summary>
        /// <param name="str"></param>
        /// <returns>True is a number; False is not a number</returns>
        public static bool IsNumber(this string str)
        {
            return decimal.TryParse(str, out decimal value);
        }

        /// <summary>
        /// The method to get a <see cref="PropertyDescriptor"/> of the control by property name
        /// </summary>
        /// <param name="control"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static PropertyDescriptor GetPropertyByName(this object control, [CallerMemberName]string propName = null)
        {
            PropertyDescriptor prop;
            prop = TypeDescriptor.GetProperties(control)[propName];
            if (null == prop)
                throw new ArgumentException("Matching ColorLabel property not found!", propName);
            else
                return prop;
        }

        /// <summary>
        /// Set the value for property of the control
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value"></param>
        /// <param name="propName"></param>
        public static void SetValue(this object control, object value = null, [CallerMemberName]string propName = null)
        {
            control.GetPropertyByName(propName).SetValue(control, value);
        }

        /// <summary>
        /// Blacken a provided color by applying per channel percentages.
        /// </summary>
        /// <param name="color1">Color.</param>
        /// <param name="percentR">Percentage of red to keep.</param>
        /// <param name="percentG">Percentage of green to keep.</param>
        /// <param name="percentB">Percentage of blue to keep.</param>
        /// <returns>Modified color.</returns>
        public static Color BlackenColor(Color color1,
                                         float percentR,
                                         float percentG,
                                         float percentB)
        {
            // Find how much to use of each primary color
            int red = (int)(color1.R * percentR);
            int green = (int)(color1.G * percentG);
            int blue = (int)(color1.B * percentB);

            // Limit check against individual component
            if (red < 0) red = 0;
            if (red > 255) red = 255;
            if (green < 0) green = 0;
            if (green > 255) green = 255;
            if (blue < 0) blue = 0;
            if (blue > 255) blue = 255;

            // Return the whitened color
            return Color.FromArgb(color1.A, red, green, blue);
        }

        /// <summary>
        /// Find the appropriate content style to match the incoming label style.
        /// </summary>
        /// <param name="style">LabelStyle enumeration.</param>
        /// <returns>Matching PaletteContentStyle enumeration value.</returns>
        public static PaletteContentStyle ContentStyleFromLabelStyle(LabelStyle style)
        {
            switch (style)
            {
                case LabelStyle.NormalControl:
                    return PaletteContentStyle.LabelNormalControl;
                case LabelStyle.BoldControl:
                    return PaletteContentStyle.LabelBoldControl;
                case LabelStyle.ItalicControl:
                    return PaletteContentStyle.LabelItalicControl;
                case LabelStyle.TitleControl:
                    return PaletteContentStyle.LabelTitleControl;
                case LabelStyle.NormalPanel:
                    return PaletteContentStyle.LabelNormalPanel;
                case LabelStyle.BoldPanel:
                    return PaletteContentStyle.LabelBoldPanel;
                case LabelStyle.ItalicPanel:
                    return PaletteContentStyle.LabelItalicPanel;
                case LabelStyle.TitlePanel:
                    return PaletteContentStyle.LabelTitlePanel;
                case LabelStyle.GroupBoxCaption:
                    return PaletteContentStyle.LabelGroupBoxCaption;
                case LabelStyle.ToolTip:
                    return PaletteContentStyle.LabelToolTip;
                case LabelStyle.SuperTip:
                    return PaletteContentStyle.LabelSuperTip;
                case LabelStyle.KeyTip:
                    return PaletteContentStyle.LabelKeyTip;
                case LabelStyle.Custom1:
                    return PaletteContentStyle.LabelCustom1;
                case LabelStyle.Custom2:
                    return PaletteContentStyle.LabelCustom2;
                case LabelStyle.Custom3:
                    return PaletteContentStyle.LabelCustom3;
                default:
                    // Should never happen!
                    Debug.Assert(false);
                    return PaletteContentStyle.LabelNormalPanel;
            }
        }

        /// <summary>
        /// Perform operation in a worker thread with wait dialog in main thread.
        /// </summary>
        /// <param name="op">Delegate of operation to be performed.</param>
        /// <param name="parameter">Parameter to be passed into the operation.</param>
        /// <returns>Result of performing the operation.</returns>
        public static object PerformOperation(Operation op, object parameter)
        {
            // Create a modal window for showing feedback
            using (ModalWaitDialog wait = new ModalWaitDialog())
            {
                // Create the object that runs the operation in a separate thread
                OperationThread opThread = new OperationThread(op, parameter);

                // Create the actual thread and provide thread entry point
                Thread thread = new Thread(new ThreadStart(opThread.Run));

                // Kick off the thread action
                thread.Start();

                // Keep looping until the thread is finished
                while (opThread.State == 0)
                {
                    // Sleep to allow thread to perform more work
                    Thread.Sleep(25);

                    // Give the feedback dialog a chance to update
                    wait.UpdateDialog();
                }

                // Process operation result
                switch (opThread.State)
                {
                    case 1:
                        return opThread.Result;
                    case 2:
                        throw opThread.Exception;
                    default:
                        // Should never happen!
                        Debug.Assert(false);
                        return null;
                }
            }
        }

        /// <summary>
        /// Convert from palette rendering hint to actual rendering hint.
        /// </summary>
        /// <param name="hint">Palette rendering hint.</param>
        /// <returns>Converted value for use with a Graphics instance.</returns>
        public static TextRenderingHint PaletteTextHintToRenderingHint(PaletteTextHint hint)
        {
            switch (hint)
            {
                case PaletteTextHint.AntiAlias:
                    return TextRenderingHint.AntiAlias;
                case PaletteTextHint.AntiAliasGridFit:
                    return TextRenderingHint.AntiAliasGridFit;
                case PaletteTextHint.ClearTypeGridFit:
                    return TextRenderingHint.ClearTypeGridFit;
                case PaletteTextHint.SingleBitPerPixel:
                    return TextRenderingHint.SingleBitPerPixel;
                case PaletteTextHint.SingleBitPerPixelGridFit:
                    return TextRenderingHint.SingleBitPerPixelGridFit;
                case PaletteTextHint.SystemDefault:
                    return TextRenderingHint.SystemDefault;
                default:
                    // Should never happen!
                    Debug.Assert(false);
                    return TextRenderingHint.SystemDefault;
            }
        }

        /// <summary>
        /// Discover if the component is in design mode.
        /// </summary>
        /// <param name="c">Component to test.</param>
        /// <returns>True if in design mode; otherwise false.</returns>
        public static bool DesignMode(Component c)
        {
            // Cache the info needed to sneak access to the component protected property
            if (_cachedDesignModePI == null)
            {
                _cachedDesignModePI = typeof(ToolStrip).GetProperty("DesignMode",
                                                                    BindingFlags.Instance |
                                                                    BindingFlags.GetProperty |
                                                                    BindingFlags.NonPublic);
            }

            return (bool)_cachedDesignModePI.GetValue(c, null);
        }

        /// <summary>
        /// Convert from VisualOrientation to Orientation.
        /// </summary>
        /// <param name="orientation">VisualOrientation value.</param>
        /// <returns>Orientation value.</returns>
        public static Orientation VisualToOrientation(VisualOrientation orientation)
        {
            switch (orientation)
            {
                case VisualOrientation.Top:
                case VisualOrientation.Bottom:
                    return Orientation.Vertical;
                case VisualOrientation.Left:
                case VisualOrientation.Right:
                    return Orientation.Horizontal;
                default:
                    // Should never happen!
                    Debug.Assert(false);
                    return Orientation.Vertical;
            }
        }

        /// <summary>
        /// Gets a string that is guaranteed to be unique.
        /// </summary>
        public static string UniqueString
        {
            get
            {
                // Generate a GUID that is guaranteed to be unique
                PI.GUIDSTRUCT newGUID = new PI.GUIDSTRUCT();
                PI.CoCreateGuid(ref newGUID);

                // Return as a hex formatted string.
                return string.Format("{0:X4}{1:X4}{2:X4}{3:X4}{4:X4}{5:X4}{6:X4}{7:X4}",
                                     newGUID.Data1, newGUID.Data2, newGUID.Data3, newGUID.Data4,
                                     newGUID.Data5, newGUID.Data6, newGUID.Data7, newGUID.Data8);

            }
        }

        /// <summary>
        /// Gets the size of the borders requested by the real window.
        /// </summary>
        /// <param name="cp">Window style parameters.</param>
        /// <returns>Border sizing.</returns>
        public static Padding GetWindowBorders(CreateParams cp)
        {
            PI.RECT rect = new PI.RECT();

            // Start with a zero sized rectangle
            rect.left = 0;
            rect.right = 0;
            rect.top = 0;
            rect.bottom = 0;

            // Adjust rectangle to add on the borders required
            PI.AdjustWindowRectEx(ref rect, cp.Style, false, cp.ExStyle);

            // Return the per side border values
            return new Padding(-rect.left, -rect.top, rect.right, rect.bottom);
        }

        /// <summary>
        /// Discover if the provided Form is currently maximized.
        /// </summary>
        /// <param name="f">Form reference.</param>
        /// <returns>True if maximized; otherwise false.</returns>
        public static bool IsFormMaximized(Form f)
        {
            // Get the current window style (cannot use the 
            // WindowState property as it can be slightly out of date)
            uint style = PI.GetWindowLong(f.Handle, PI.GWL_STYLE);

            return ((style &= PI.WS_MAXIMIZE) != 0);
        }

        /// <summary>
        /// Discover if the provided Form is currently minimized.
        /// </summary>
        /// <param name="f">Form reference.</param>
        /// <returns>True if minimized; otherwise false.</returns>
        public static bool IsFormMinimized(Form f)
        {
            // Get the current window style (cannot use the 
            // WindowState property as it can be slightly out of date)
            uint style = PI.GetWindowLong(f.Handle, PI.GWL_STYLE);

            return ((style &= PI.WS_MINIMIZE) != 0);
        }

        /// <summary>
        /// Convert the color to a black and white color.
        /// </summary>
        /// <param name="color">Base color.</param>
        /// <returns>Black and White version of color.</returns>
        public static Color ColorToBlackAndWhite(Color color)
        {
            // Use the standard percentages of RGB for the human eye bias
            int gray = (int)(((float)color.R * 0.3f) +
                             ((float)color.G * 0.59f) +
                             ((float)color.B * 0.11f));

            return Color.FromArgb(gray, gray, gray);
        }

        /// <summary>
        /// Whiten a provided color by applying per channel percentages.
        /// </summary>
        /// <param name="color1">Color.</param>
        /// <param name="percentR">Percentage of red to keep.</param>
        /// <param name="percentG">Percentage of green to keep.</param>
        /// <param name="percentB">Percentage of blue to keep.</param>
        /// <returns>Modified color.</returns>
        public static Color WhitenColor(Color color1,
                                        float percentR,
                                        float percentG,
                                        float percentB)
        {
            // Find how much to use of each primary color
            int red = (int)(color1.R / percentR);
            int green = (int)(color1.G / percentG);
            int blue = (int)(color1.B / percentB);

            // Limit check against individual component
            if (red < 0) red = 0;
            if (red > 255) red = 255;
            if (green < 0) green = 0;
            if (green > 255) green = 255;
            if (blue < 0) blue = 0;
            if (blue > 255) blue = 255;

            // Return the whitened color
            return Color.FromArgb(color1.A, red, green, blue);
        }

        /// <summary>
        /// Apply an orientation to the draw border edges to get a correct value.
        /// </summary>
        /// <param name="borders">Border edges to be drawn.</param>
        /// <param name="orientation">How to adjsut the border edges.</param>
        /// <returns>Border edges adjusted for orientation.</returns>
        public static PaletteDrawBorders OrientateDrawBorders(PaletteDrawBorders borders,
                                                              VisualOrientation orientation)
        {
            // No need to perform an change for top orientation
            if (orientation == VisualOrientation.Top)
                return borders;

            // No need to change the All or None values
            if ((borders == PaletteDrawBorders.All) || (borders == PaletteDrawBorders.None))
                return borders;

            PaletteDrawBorders ret = PaletteDrawBorders.None;

            // Apply orientation change to each side in turn
            switch (orientation)
            {
                case VisualOrientation.Bottom:
                    // Invert sides
                    if (CommonHelper.HasTopBorder(borders)) ret |= PaletteDrawBorders.Bottom;
                    if (CommonHelper.HasBottomBorder(borders)) ret |= PaletteDrawBorders.Top;
                    if (CommonHelper.HasLeftBorder(borders)) ret |= PaletteDrawBorders.Right;
                    if (CommonHelper.HasRightBorder(borders)) ret |= PaletteDrawBorders.Left;
                    break;
                case VisualOrientation.Left:
                    // Rotate one anti-clockwise
                    if (CommonHelper.HasTopBorder(borders)) ret |= PaletteDrawBorders.Left;
                    if (CommonHelper.HasBottomBorder(borders)) ret |= PaletteDrawBorders.Right;
                    if (CommonHelper.HasLeftBorder(borders)) ret |= PaletteDrawBorders.Bottom;
                    if (CommonHelper.HasRightBorder(borders)) ret |= PaletteDrawBorders.Top;
                    break;
                case VisualOrientation.Right:
                    // Rotate sides one clockwise
                    if (CommonHelper.HasTopBorder(borders)) ret |= PaletteDrawBorders.Right;
                    if (CommonHelper.HasBottomBorder(borders)) ret |= PaletteDrawBorders.Left;
                    if (CommonHelper.HasLeftBorder(borders)) ret |= PaletteDrawBorders.Top;
                    if (CommonHelper.HasRightBorder(borders)) ret |= PaletteDrawBorders.Bottom;
                    break;
                default:
                    // Should never happen!
                    Debug.Assert(false);
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Create a graphics path that describes a rounded rectangle.
        /// </summary>
        /// <param name="rect">Rectangle to become rounded.</param>
        /// <param name="rounding">The rounding factor to apply.</param>
        /// <returns>GraphicsPath instance.</returns>
        public static GraphicsPath RoundedRectanglePath(Rectangle rect,
                                                        int rounding)
        {
            GraphicsPath roundedPath = new GraphicsPath();

            // Only use a rounding that will fit inside the rect
            rounding = Math.Min(rounding, Math.Min(rect.Width / 2, rect.Height / 2) - rounding);

            // If there is no room for any rounding effect...
            if (rounding <= 0)
            {
                // Just add a simple rectangle as a quick way of adding four lines
                roundedPath.AddRectangle(rect);
            }
            else
            {
                // We create the path using a floating point rectangle
                RectangleF rectF = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);

                // The border is made of up a quarter of a circle arc, in each corner
                int arcLength = rounding * 2;
                roundedPath.AddArc(rectF.Left, rectF.Top, arcLength, arcLength, 180f, 90f);
                roundedPath.AddArc(rectF.Right - arcLength, rectF.Top, arcLength, arcLength, 270f, 90f);
                roundedPath.AddArc(rectF.Right - arcLength, rectF.Bottom - arcLength, arcLength, arcLength, 0f, 90f);
                roundedPath.AddArc(rectF.Left, rectF.Bottom - arcLength, arcLength, arcLength, 90f, 90f);

                // Make the last and first arc join up
                roundedPath.CloseFigure();
            }

            return roundedPath;
        }

        /// <summary>
        /// Convert a client mouse position inside a windows message into a screen position.
        /// </summary>
        /// <param name="m">Window message.</param>
        /// <returns>Screen point.</returns>
        public static Point ClientMouseMessageToScreenPt(Message m)
        {
            // Extract the x and y mouse position from message
            PI.POINTC clientPt = new PI.POINTC();
            clientPt.x = PI.LOWORD((int)m.LParam);
            clientPt.y = PI.HIWORD((int)m.LParam);

            // Negative positions are in the range 32767 -> 65535, 
            // so convert to actual int values for the negative positions
            if (clientPt.x >= 32767) clientPt.x = (clientPt.x - 65536);
            if (clientPt.y >= 32767) clientPt.y = (clientPt.y - 65536);

            // Convert a 0,0 point from client to screen to find offsetting
            PI.POINTC zeroPIPt = new PI.POINTC();
            zeroPIPt.x = 0;
            zeroPIPt.y = 0;
            PI.MapWindowPoints(m.HWnd, IntPtr.Zero, zeroPIPt, 1);

            // Adjust the client coordinate by the offset to get to screen
            clientPt.x += zeroPIPt.x;
            clientPt.y += zeroPIPt.y;

            // Return as a managed point type
            return new Point(clientPt.x, clientPt.y);
        }

        /// <summary>
        /// Gets a value indicating if the enumeration specifies all four borders.
        /// </summary>
        /// <param name="borders">Enumeration for borders.</param>
        /// <returns>True if all four borders specified; otherwise false.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static bool HasAllBorders(PaletteDrawBorders borders)
        {
            return ((borders & PaletteDrawBorders.All) == PaletteDrawBorders.All);
        }

        /// <summary>
        /// Gets a value indicating if the enumeration specifies at least one border.
        /// </summary>
        /// <param name="borders">Enumeration for borders.</param>
        /// <returns>True if at least one border specified; otherwise false.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static bool HasABorder(PaletteDrawBorders borders)
        {
            return ((borders & PaletteDrawBorders.All) != PaletteDrawBorders.None);
        }

        /// <summary>
        /// Gets a value indicating if the enumeration includes the top border.
        /// </summary>
        /// <param name="borders">Enumeration for borders.</param>
        /// <returns>True if includes the top border; otherwise false.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static bool HasTopBorder(PaletteDrawBorders borders)
        {
            return ((borders & PaletteDrawBorders.Top) == PaletteDrawBorders.Top);
        }

        /// <summary>
        /// Gets a value indicating if the enumeration includes the bottom border.
        /// </summary>
        /// <param name="borders">Enumeration for borders.</param>
        /// <returns>True if includes the bottom border; otherwise false.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static bool HasBottomBorder(PaletteDrawBorders borders)
        {
            return ((borders & PaletteDrawBorders.Bottom) == PaletteDrawBorders.Bottom);
        }

        /// <summary>
        /// Gets a value indicating if the enumeration includes the left border.
        /// </summary>
        /// <param name="borders">Enumeration for borders.</param>
        /// <returns>True if includes the left border; otherwise false.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static bool HasLeftBorder(PaletteDrawBorders borders)
        {
            return ((borders & PaletteDrawBorders.Left) == PaletteDrawBorders.Left);
        }

        /// <summary>
        /// Gets a value indicating if the enumeration includes the right border.
        /// </summary>
        /// <param name="borders">Enumeration for borders.</param>
        /// <returns>True if includes the right border; otherwise false.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static bool HasRightBorder(PaletteDrawBorders borders)
        {
            return ((borders & PaletteDrawBorders.Right) == PaletteDrawBorders.Right);
        }

        /// <summary>
        /// Gets the form level right to left setting.
        /// </summary>
        /// <param name="control">Control for which the setting is needed.</param>
        /// <returns>RightToLeft setting.</returns>
        public static bool GetRightToLeftLayout(Control control)
        {
            // Default to left-to-right layout
            bool rtl = false;

            // We need a valid control to find a top level form
            if (control != null)
            {
                // Search for a top level form associated with the control
                Form topForm = control.FindForm();

                // If can find an owning form
                if (topForm != null)
                {
                    // Use the form setting instead
                    rtl = topForm.RightToLeftLayout;
                }
            }

            return rtl;
        }

        /// <summary>
        /// Apply a reversed orientation so that when orientated again it comes out with the original value.
        /// </summary>
        /// <param name="borders">Border edges to be drawn.</param>
        /// <param name="orientation">How to adjsut the border edges.</param>
        /// <returns>Border edges adjusted for orientation.</returns>
        public static PaletteDrawBorders ReverseOrientateDrawBorders(PaletteDrawBorders borders,
                                                                     VisualOrientation orientation)
        {
            // No need to perform an change for top orientation
            if (orientation == VisualOrientation.Top)
                return borders;

            // No need to change the All or None values
            if ((borders == PaletteDrawBorders.All) || (borders == PaletteDrawBorders.None))
                return borders;

            PaletteDrawBorders ret = PaletteDrawBorders.None;

            // Apply orientation change to each side in turn
            switch (orientation)
            {
                case VisualOrientation.Bottom:
                    // Invert sides
                    if (CommonHelper.HasTopBorder(borders)) ret |= PaletteDrawBorders.Bottom;
                    if (CommonHelper.HasBottomBorder(borders)) ret |= PaletteDrawBorders.Top;
                    if (CommonHelper.HasLeftBorder(borders)) ret |= PaletteDrawBorders.Right;
                    if (CommonHelper.HasRightBorder(borders)) ret |= PaletteDrawBorders.Left;
                    break;
                case VisualOrientation.Right:
                    // Rotate one anti-clockwise
                    if (CommonHelper.HasTopBorder(borders)) ret |= PaletteDrawBorders.Left;
                    if (CommonHelper.HasBottomBorder(borders)) ret |= PaletteDrawBorders.Right;
                    if (CommonHelper.HasLeftBorder(borders)) ret |= PaletteDrawBorders.Bottom;
                    if (CommonHelper.HasRightBorder(borders)) ret |= PaletteDrawBorders.Top;
                    break;
                case VisualOrientation.Left:
                    // Rotate sides one clockwise
                    if (CommonHelper.HasTopBorder(borders)) ret |= PaletteDrawBorders.Right;
                    if (CommonHelper.HasBottomBorder(borders)) ret |= PaletteDrawBorders.Left;
                    if (CommonHelper.HasLeftBorder(borders)) ret |= PaletteDrawBorders.Top;
                    if (CommonHelper.HasRightBorder(borders)) ret |= PaletteDrawBorders.Bottom;
                    break;
                default:
                    // Should never happen!
                    Debug.Assert(false);
                    break;
            }

            return ret;
        }


        /// <summary>
        /// Return the provided size with orientation specific padding applied.
        /// </summary>
        /// <param name="orientation">Orientation to apply padding with.</param>
        /// <param name="size">Starting size.</param>
        /// <param name="padding">Padding to be applied.</param>
        /// <returns>Updated size.</returns>
        public static Size ApplyPadding(Orientation orientation, Size size, Padding padding)
        {
            // Ignore an empty padding value
            if (!padding.Equals(CommonHelper.InheritPadding))
            {
                // The orientation determines how the border padding is 
                // applied to the preferred size of the children
                switch (orientation)
                {
                    case Orientation.Vertical:
                        size.Width += padding.Vertical;
                        size.Height += padding.Horizontal;
                        break;
                    case Orientation.Horizontal:
                        size.Width += padding.Horizontal;
                        size.Height += padding.Vertical;
                        break;
                    default:
                        // Should never happen!
                        Debug.Assert(false);
                        break;
                }
            }

            return size;
        }

        /// <summary>
        /// Return the provided size with visual orientation specific padding applied.
        /// </summary>
        /// <param name="orientation">Orientation to apply padding with.</param>
        /// <param name="size">Starting size.</param>
        /// <param name="padding">Padding to be applied.</param>
        /// <returns>Updated size.</returns>
        public static Size ApplyPadding(VisualOrientation orientation,
                                        Size size,
                                        Padding padding)
        {
            // Ignore an empty padding value
            if (!padding.Equals(CommonHelper.InheritPadding))
            {
                // The orientation determines how the border padding is 
                // applied to the preferred size of the children
                switch (orientation)
                {
                    case VisualOrientation.Top:
                    case VisualOrientation.Bottom:
                        size.Width += padding.Horizontal;
                        size.Height += padding.Vertical;
                        break;
                    case VisualOrientation.Left:
                    case VisualOrientation.Right:
                        size.Width += padding.Vertical;
                        size.Height += padding.Horizontal;
                        break;
                    default:
                        // Should never happen!
                        Debug.Assert(false);
                        break;
                }
            }

            return size;
        }

        /// <summary>
        /// Return the provided rectangle with orientation specific padding applied.
        /// </summary>
        /// <param name="orientation">Orientation to apply padding with.</param>
        /// <param name="rect">Starting rectangle.</param>
        /// <param name="padding">Padding to be applied.</param>
        /// <returns>Updated rectangle.</returns>
        public static Rectangle ApplyPadding(Orientation orientation,
                                             Rectangle rect,
                                             Padding padding)
        {
            // Ignore an empty padding value
            if (!padding.Equals(CommonHelper.InheritPadding))
            {
                // The orientation determines how the border padding is 
                // applied to the preferred size of the children
                switch (orientation)
                {
                    case Orientation.Horizontal:
                        rect.X += padding.Left;
                        rect.Width -= padding.Horizontal;
                        rect.Y += padding.Top;
                        rect.Height -= padding.Vertical;
                        break;
                    case Orientation.Vertical:
                        rect.X += padding.Top;
                        rect.Width -= padding.Vertical;
                        rect.Y += padding.Right;
                        rect.Height -= padding.Horizontal;
                        break;
                    default:
                        // Should never happen!
                        Debug.Assert(false);
                        break;
                }
            }

            return rect;
        }

        /// <summary>
        /// Return the provided rectangle with visual orientation specific padding applied.
        /// </summary>
        /// <param name="orientation">Orientation to apply padding with.</param>
        /// <param name="rect">Starting rectangle.</param>
        /// <param name="padding">Padding to be applied.</param>
        /// <returns>Updated rectangle.</returns>
        public static Rectangle ApplyPadding(VisualOrientation orientation,
                                             Rectangle rect,
                                             Padding padding)
        {
            // Ignore an empty padding value
            if (!padding.Equals(CommonHelper.InheritPadding))
            {
                // The orientation determines how the border padding is 
                // used to reduce the space available for children
                switch (orientation)
                {
                    case VisualOrientation.Top:
                        rect = new Rectangle(rect.X + padding.Left, rect.Y + padding.Top,
                                             rect.Width - padding.Horizontal, rect.Height - padding.Vertical);
                        break;
                    case VisualOrientation.Bottom:
                        rect = new Rectangle(rect.X + padding.Right, rect.Y + padding.Bottom,
                                             rect.Width - padding.Horizontal, rect.Height - padding.Vertical);
                        break;
                    case VisualOrientation.Left:
                        rect = new Rectangle(rect.X + padding.Top, rect.Y + padding.Right,
                                             rect.Width - padding.Vertical, rect.Height - padding.Horizontal);
                        break;
                    case VisualOrientation.Right:
                        rect = new Rectangle(rect.X + padding.Bottom, rect.Y + padding.Left,
                                             rect.Width - padding.Vertical, rect.Height - padding.Horizontal);
                        break;
                    default:
                        // Should never happen!
                        Debug.Assert(false);
                        break;
                }
            }

            return rect;
        }

        /// <summary>
        /// Modify the incoming padding to reflect the visual orientation.
        /// </summary>
        /// <param name="orientation">Orientation to apply to padding.</param>
        /// <param name="padding">Padding to be modified.</param>
        /// <returns>Updated padding.</returns>
        public static Padding OrientatePadding(VisualOrientation orientation,
                                               Padding padding)
        {
            switch (orientation)
            {
                case VisualOrientation.Top:
                    return padding;
                case VisualOrientation.Bottom:
                    return new Padding(padding.Right, padding.Bottom, padding.Left, padding.Top);
                case VisualOrientation.Left:
                    return new Padding(padding.Top, padding.Right, padding.Bottom, padding.Left);
                case VisualOrientation.Right:
                    return new Padding(padding.Bottom, padding.Left, padding.Top, padding.Right);
                default:
                    // Should never happen!
                    Debug.Assert(false);
                    return padding;
            }
        }

        /// <summary>
        /// Get the number of bits used to define the color depth of the display.
        /// </summary>
        /// <returns>Number of bits in color depth.</returns>
        public static int ColorDepth()
        {
            // Get access to the desktop DC
            IntPtr desktopDC = PI.GetDC(IntPtr.Zero);

            // Find raw values that define the color depth
            int planes = PI.GetDeviceCaps(desktopDC, (int)PI.PLANES);
            int bitsPerPixel = PI.GetDeviceCaps(desktopDC, (int)PI.BITSPIXEL);

            // Must remember to release it!
            PI.ReleaseDC(IntPtr.Zero, desktopDC);

            return planes * bitsPerPixel;
        }

        /// <summary>
        /// Merge two colors together using relative percentages.
        /// </summary>
        /// <param name="color1">First color.</param>
        /// <param name="percent1">Percentage of first color to use.</param>
        /// <param name="color2">Second color.</param>
        /// <param name="percent2">Percentage of second color to use.</param>
        /// <returns>Merged color.</returns>
        public static Color MergeColors(Color color1, float percent1,
                                        Color color2, float percent2)
        {
            // Use existing three color merge
            return MergeColors(color1, percent1, color2, percent2, Color.Empty, 0f);
        }

        /// <summary>
        /// Merge three colors together using relative percentages.
        /// </summary>
        /// <param name="color1">First color.</param>
        /// <param name="percent1">Percentage of first color to use.</param>
        /// <param name="color2">Second color.</param>
        /// <param name="percent2">Percentage of second color to use.</param>
        /// <param name="color3">Third color.</param>
        /// <param name="percent3">Percentage of third color to use.</param>
        /// <returns>Merged color.</returns>
        public static Color MergeColors(Color color1, float percent1,
                                        Color color2, float percent2,
                                        Color color3, float percent3)
        {
            // Find how much to use of each primary color
            int red = (int)((color1.R * percent1) + (color2.R * percent2) + (color3.R * percent3));
            int green = (int)((color1.G * percent1) + (color2.G * percent2) + (color3.G * percent3));
            int blue = (int)((color1.B * percent1) + (color2.B * percent2) + (color3.B * percent3));

            // Limit check against individual component
            if (red < 0) red = 0;
            if (red > 255) red = 255;
            if (green < 0) green = 0;
            if (green > 255) green = 255;
            if (blue < 0) blue = 0;
            if (blue > 255) blue = 255;

            // Return the merged color
            return Color.FromArgb(red, green, blue);
        }

        #endregion
    }
}
