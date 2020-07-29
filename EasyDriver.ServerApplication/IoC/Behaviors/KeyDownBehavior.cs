using DevExpress.Mvvm.UI.Interactivity;
using System;
using System.Windows;
using System.Windows.Input;

namespace EasyScada.ServerApplication
{
    public class KeyDownBehavior : Behavior<FrameworkElement>
    {
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public Key Key
        {
            get { return (Key)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(KeyDownBehavior), new PropertyMetadata(null));

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(Key), typeof(KeyDownBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            AssociatedObject.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key)
            {
                if (Command != null)
                {
                    if (Command.CanExecute(null))
                        Command.Execute(null);
                }
            }
        }
    }
}
