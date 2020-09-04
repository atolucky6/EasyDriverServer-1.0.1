using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp7
{
    public class RenderPanel : Panel
    {
        #region Constructors

        public RenderPanel() : base()
        {
            if (!DesignMode)
            {
                #region Initialize

                var desc = new SwapChainDescription()
                {
                    BufferCount = 1,
                    ModeDescription = new ModeDescription(Width, Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                    IsWindowed = false,
                    OutputHandle = Handle,
                    SampleDescription = new SampleDescription(1, 0),
                    SwapEffect = SwapEffect.Discard,
                    Usage = Usage.RenderTargetOutput
                };

                SharpDX.Direct3D11.Device.CreateWithSwapChain(
                    DriverType.Hardware,
                    SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport,
                    new[] { SharpDX.Direct3D.FeatureLevel.Level_10_0 },
                    desc,
                    out _device, out _swapChain);

                _backBuffer = SharpDX.Direct3D11.Texture2D.FromSwapChain<SharpDX.Direct3D11.Texture2D>(_swapChain, 0);
                _backBufferView = new SharpDX.Direct3D11.RenderTargetView(_device, _backBuffer);

                factory2D = new SharpDX.Direct2D1.Factory();
                using (var surface = _backBuffer.QueryInterface<Surface>())
                {
                    RenderTarget2D = new RenderTarget(
                        factory2D,
                        surface,
                        new RenderTargetProperties(
                            new SharpDX.Direct2D1.PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)));
                }

                RenderTarget2D.AntialiasMode = AntialiasMode.PerPrimitive;
                factoryDWrite = new SharpDX.DirectWrite.Factory();
                SceneColorBrush = new SolidColorBrush(RenderTarget2D, SharpDX.Color.White);
                #endregion

                try
                {
                    _bitmap = LoadFromFile(RenderTarget2D, "Motor 14.png");
                    _bitmap2 = LoadFromFile(RenderTarget2D, "Motor 14Red.png");
                    Render();
                }
                catch { }
            }
        }

        static RenderPanel()
        {
   
        }

        #endregion

        #region Fields

        Timer timer;

        SharpDX.Direct3D11.Device _device;
        SwapChain _swapChain;
        SharpDX.Direct3D11.Texture2D _backBuffer;
        SharpDX.Direct3D11.RenderTargetView _backBufferView;

        private SharpDX.Direct2D1.Factory factory2D;
        private SharpDX.DirectWrite.Factory factoryDWrite;

        public RenderTarget RenderTarget2D { get; private set; }
        public SolidColorBrush SceneColorBrush { get; private set; }

        bool waitVerticalBlanking;

        SharpDX.Direct2D1.Bitmap _bitmap;
        SharpDX.Direct2D1.Bitmap _bitmap2;

        #endregion

        #region Methods

        int count = 0;
        public void Render()
        {
            _device.ImmediateContext.Rasterizer.SetViewport(new Viewport(0, 0, Width, Height));
            _device.ImmediateContext.OutputMerger.SetTargets(_backBufferView);
            RenderTarget2D.BeginDraw();
            if (count == 0)
            {
                RenderTarget2D.DrawBitmap(_bitmap, 1.0f, BitmapInterpolationMode.Linear);
                count = 1;
            }
            else
            {
                count = 0;
                RenderTarget2D.DrawBitmap(_bitmap2, 1.0f, BitmapInterpolationMode.Linear);
            }
            RenderTarget2D.EndDraw();
            _swapChain.Present(waitVerticalBlanking ? 1 : 0, PresentFlags.None);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
        }

        /// <summary>
        /// Loads a Direct2D Bitmap from a file using System.Drawing.Image.FromFile(...)
        /// </summary>
        /// <param name="renderTarget">The render target.</param>
        /// <param name="file">The file.</param>
        /// <returns>A D2D1 Bitmap</returns>
        public SharpDX.Direct2D1.Bitmap LoadFromFile(RenderTarget renderTarget, string file)
        {
            // Loads from file using System.Drawing.Image
            using (var unscaleBitMap = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(file))
            {
                using (var bitmap = ResizeBitmap(unscaleBitMap, Width, Height))
                {
                    var sourceArea = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
                    var bitmapProperties = new BitmapProperties(new SharpDX.Direct2D1.PixelFormat(Format.R8G8B8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied));
                    var size = new Size2(bitmap.Width, bitmap.Height);

                    // Transform pixels from BGRA to RGBA
                    int stride = bitmap.Width * sizeof(int);
                    using (var tempStream = new DataStream(bitmap.Height * stride, true, true))
                    {
                        // Lock System.Drawing.Bitmap
                        var bitmapData = bitmap.LockBits(sourceArea, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                        // Convert all pixels 
                        for (int y = 0; y < bitmap.Height; y++)
                        {
                            int offset = bitmapData.Stride * y;
                            for (int x = 0; x < bitmap.Width; x++)
                            {
                                // Not optimized 
                                byte B = Marshal.ReadByte(bitmapData.Scan0, offset++);
                                byte G = Marshal.ReadByte(bitmapData.Scan0, offset++);
                                byte R = Marshal.ReadByte(bitmapData.Scan0, offset++);
                                byte A = Marshal.ReadByte(bitmapData.Scan0, offset++);
                                if (A == 0)
                                {
                                    R = BackColor.R;
                                    B = BackColor.B;
                                    G = BackColor.G;
                                    
                                }
                                int rgba = R | (G << 8) | (B << 16) | (A << 24);
                                tempStream.Write(rgba);
                            }

                        }
                        bitmap.UnlockBits(bitmapData);
                        tempStream.Position = 0;

                        return new SharpDX.Direct2D1.Bitmap(renderTarget, size, tempStream, stride, bitmapProperties);
                    }
                }
            }
        }

        public System.Drawing.Bitmap ResizeBitmap(System.Drawing.Bitmap bmp, int width, int height)
        {
            System.Drawing.Bitmap result = new System.Drawing.Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, width, height);
            }

            return result;
        }

        #endregion
    }
}
