﻿using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;
using System.Windows;
using System.Windows.Input;

namespace EasyScada.ServerApplication
{
    public class CustomCopyPasteGridControl : Behavior<GridControl>
    {
        public ICommand CopyCommand
        {
            get { return (ICommand)GetValue(CopyCommandProperty); }
            set { SetValue(CopyCommandProperty, value); }
        }

        public static readonly DependencyProperty CopyCommandProperty =
            DependencyProperty.Register("CopyCommand", typeof(ICommand), typeof(CustomCopyPasteGridControl), new PropertyMetadata(null));

        public ICommand PasteCommand
        {
            get { return (ICommand)GetValue(PasteCommandProperty); }
            set { SetValue(PasteCommandProperty, value); }
        }

        public static readonly DependencyProperty PasteCommandProperty =
            DependencyProperty.Register("PasteCommand", typeof(ICommand), typeof(CustomCopyPasteGridControl), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.CopyingToClipboard += OnCopyingToClipboard;
            AssociatedObject.PastingFromClipboard += OnPastingFromClipboard;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.CopyingToClipboard -= OnCopyingToClipboard;
            AssociatedObject.PastingFromClipboard -= OnPastingFromClipboard;
        }

        private void OnPastingFromClipboard(object sender, PastingFromClipboardEventArgs e)
        {
            e.Handled = true;
            if (PasteCommand != null)
            {
                if (PasteCommand.CanExecute(null))
                    PasteCommand.Execute(null);
            }
        }

        private void OnCopyingToClipboard(object sender, CopyingToClipboardEventArgs e)
        {
            e.Handled = true;
            if (CopyCommand != null)
            {
                if (CopyCommand.CanExecute(null))
                    CopyCommand.Execute(null);
            }
        }
    }
}
