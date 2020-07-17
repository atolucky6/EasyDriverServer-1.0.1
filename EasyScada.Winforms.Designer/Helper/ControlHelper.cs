using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Winforms.Designer
{
    public static class ControlHelper
    {
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
    }
}
