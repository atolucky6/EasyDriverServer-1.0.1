using DevExpress.Mvvm;
using EasyDriver.MenuPlugin;
using EasyDriver.Reversible;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using IServiceContainer = EasyDriver.ServiceContainer.IServiceContainer;

namespace EasyDriver.ReversibleMenu
{
    public class EasyReverseMenuPlugin : EasyMenuPlugin
    {
        readonly IReverseService reverseService;
        readonly IBarComponent editMenu;
        readonly IBarComponent editToolbar;
        readonly IBarComponent undoSplitButtonItem;
        readonly IBarComponent redoSplitButtonItem;
        readonly List<IBarComponent> undoItems;
        readonly List<IBarComponent> redoItems;

        public EasyReverseMenuPlugin(IServiceContainer serviceContainer) : base(serviceContainer)
        {
            string path = "pack://application:,,,/EasyDriver.ReversibleMenu;component/Images/"; ;
            ImageSource undoImageSource = new BitmapImage(new Uri(path + "undo-48px.png", UriKind.Absolute));
            ImageSource redoImageSource = new BitmapImage(new Uri(path + "redo-48px.png", UriKind.Absolute));

            editMenu = BarFactory.Default.CreateSubItem("Edit");
            editToolbar = BarFactory.Default.CreateToolBar("Edit");

            undoSplitButtonItem = BarFactory.Default.CreateButtonSplitItem(
                displayName: "Undo",
                keyGesture: new KeyGesture(Key.Z, ModifierKeys.Control),
                imageSource: undoImageSource,
                command: new DelegateCommand(() =>
                {
                    reverseService.Undo();
                }, () => reverseService.CanUndo()));

            redoSplitButtonItem = BarFactory.Default.CreateButtonSplitItem(
                displayName: "Redo",
                keyGesture: new KeyGesture(Key.Y, ModifierKeys.Control),
                imageSource: redoImageSource,
                command: new DelegateCommand(() =>
                {
                    reverseService.Redo();
                }, () => reverseService.CanRedo()));

            undoItems = new List<IBarComponent>();
            redoItems = new List<IBarComponent>();
            this.reverseService = serviceContainer.Get<IReverseService>();
            this.reverseService.HistoryChanged += ReverseService_HistoryChanged;

            editMenu.Add(BarFactory.Default.CreateSeparator()).Add(undoSplitButtonItem).Add(redoSplitButtonItem);
            editToolbar.Add(BarFactory.Default.CreateSeparator()).Add(undoSplitButtonItem).Add(redoSplitButtonItem);
        }

        private void InitializeUndoRedoItems()
        {
            var undoTextList = reverseService.Session.GetUndoTextList().ToList();
            undoSplitButtonItem.Clear();
            undoItems.Clear();
            for (int i = 0; i < undoTextList.Count(); i++)
            {
                int count = i + 1;
                IBarComponent undoItem = BarFactory.Default.CreateButton(
                    displayName: undoTextList[i],
                    command: new DelegateCommand(() => reverseService.Undo(count)));
                undoItems.Add(undoItem);
                undoSplitButtonItem.Add(undoItem);
            };

            var redoTextList = reverseService.Session.GetRedoTextList().ToList();
            redoSplitButtonItem.Clear();
            redoItems.Clear();
            for (int i = 0; i < redoTextList.Count(); i++)
            {
                int count = i + 1;
                IBarComponent redoItem = BarFactory.Default.CreateButton(
                    displayName: redoTextList[i],
                    command: new DelegateCommand(() => reverseService.Redo(count)));
                redoItems.Add(redoItem);
                redoSplitButtonItem.Add(redoItem);
            }
        }

        private void ReverseService_HistoryChanged(object sender, EventArgs e)
        {
            InitializeUndoRedoItems();
        }

        public override IEnumerable<IBarComponent> GetExtendApplicationMenuItems(IBarComponent container, object context)
        {
            return new List<IBarComponent>() { editMenu };
        }

        public override IEnumerable<IBarComponent> GetExtendApplicationToolBarItems(IBarComponent container, object context)
        {
            return new List<IBarComponent>() { editToolbar };
        }
    }
}
