using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    //public partial class DX9Viewer : Control
    //{
    //    public Device device = null;
    //    PresentParameters presentParams = new PresentParameters();
    //    public bool initFailure = true; // Shows if it has been a problem during the initialization
    //    private bool deviceLost = false; // if the device is lost
    //    private bool rendering = false; // Flag to avoid call the Render method while is already rendering

    //    public DX9Viewer()
    //    {
    //        this.BackColor = Color.Black;
    //    }

    //    /// <summary>
    //    /// Init the device
    //    /// </summary>
    //    public void InitDevice()
    //    {
    //        AdapterInformation adapterInfo = Manager.Adapters[0];

    //        presentParams.BackBufferCount = 1;
    //        presentParams.BackBufferFormat = adapterInfo.CurrentDisplayMode.Format;
    //        presentParams.BackBufferWidth = this.Width;
    //        presentParams.BackBufferHeight = this.Height;
    //        presentParams.SwapEffect = SwapEffect.Discard;
    //        presentParams.AutoDepthStencilFormat = DepthFormat.D24S8;
    //        presentParams.EnableAutoDepthStencil = true;
    //        presentParams.Windowed = true;

    //        device = new Device(0, DeviceType.Hardware, this.Handle, CreateFlags.SoftwareVertexProcessing, presentParams);

    //        device.DeviceResizing += new System.ComponentModel.CancelEventHandler(OnResize);
    //        initFailure = false;
    //    }

    //    /// <summary>
    //    /// Call this method when you wamt to render the scene.
    //    /// </summary>
    //    public void Render()
    //    {
    //        if (device == null) return;
    //        if (rendering) return;

    //        if (deviceLost) AttemptRecovery();
    //        if (deviceLost) return;

    //        rendering = true;

    //        device.BeginScene();

    //        // Render the scene here
    //        device.EndScene();

    //        try
    //        {
    //            // Copy the back buffer to the display
    //            device.Present();
    //        }
    //        catch (DeviceLostException)
    //        {
    //            // Indicate that the device has been lost
    //            deviceLost = true;
    //        }
    //        finally
    //        {
    //            rendering = false;
    //        }
    //    }


    //    /// <summary>
    //    /// Attempt to recover the device if it is lost.
    //    /// </summary>
    //    protected void AttemptRecovery()
    //    {
    //        if (device == null) return;
    //        try
    //        {
    //            device.TestCooperativeLevel();
    //        }
    //        catch (DeviceLostException)
    //        {
    //        }
    //        catch (DeviceNotResetException)
    //        {
    //            try
    //            {
    //                device.Reset(presentParams);
    //                deviceLost = false;
    //            }
    //            catch (DeviceLostException)
    //            {
    //                // If it's still lost or lost again, just do nothing
    //            }
    //        }
    //    }

    //    protected override void OnPaint(PaintEventArgs pe)
    //    {
    //        // Calling the base class OnPaint
    //        base.OnPaint(pe);

    //        if (device == null) return;
    //        this.Render();
    //    }

    //    /// <summary>
    //    /// Cancel the resize
    //    /// </summary>
    //    private void OnResize(object sender, System.ComponentModel.CancelEventArgs e)
    //    {
    //        e.Cancel = true;
    //    }
    //}

}
