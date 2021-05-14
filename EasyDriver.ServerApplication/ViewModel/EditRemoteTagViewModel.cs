using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using EasyDriverPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyScada.ServerApplication
{
    public class EditRemoteTagViewModel : ISupportParentViewModel, ISupportParameter
    {
        public virtual double Gain { get; set; }
        public virtual double Offset { get; set; }
        public virtual ITagCore Tag { get => Parameter as ITagCore; }
        public virtual object Parameter { get; set; }
        public virtual object ParentViewModel { get; set; }
        public string Title { get; set; }
        public SizeToContent SizeToContent { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public ICurrentWindowService CurrentWindowService { get => this.GetService<ICurrentWindowService>(); }

        public EditRemoteTagViewModel()
        {
            SizeToContent = SizeToContent.Height;
            Title = "Edit Tag";
            Width = 600;
            Height = 500;
        }

        public virtual void OnLoaded()
        {
            Gain = Tag.Gain;
            Offset = Tag.Offset;
            this.RaisePropertiesChanged();
        }

        public void Save()
        {
            Tag.Gain = Gain;
            Tag.Offset = Offset;
            Cancel();
        }

        public void Cancel()
        {
            CurrentWindowService.Close();
        }
    }
}
