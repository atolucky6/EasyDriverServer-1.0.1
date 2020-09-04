using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WpfApp1
{
    [Localizability(LocalizationCategory.ComboBox)]
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ComboBoxItem))]
    [TemplatePart(Name = "PART_EditableTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    public class AutoCompleteComboBox : ComboBox
    {
        #region Constructors

        static AutoCompleteComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoCompleteComboBox), new FrameworkPropertyMetadata(typeof(AutoCompleteComboBox)));

        }

        public AutoCompleteComboBox() : base()
        {
        }

        #endregion

        #region Private members

        private bool canUpdate = true;
        private bool needUpdate = false;
        private Timer timer;
        IEnumerable oldDataSource;

        #endregion

        #region Public members

        public int SearchDelay
        {
            get { return (int)GetValue(SearchDelayProperty); }
            set { SetValue(SearchDelayProperty, value); }
        }
        public static readonly DependencyProperty SearchDelayProperty =
            DependencyProperty.Register("SearchDelay", typeof(int), typeof(AutoCompleteComboBox), new PropertyMetadata(100));

        public IEnumerable AutoCompleteCustomSource
        {
            get { return (IEnumerable)GetValue(AutoCompleteCustomSourceProperty); }
            set { SetValue(AutoCompleteCustomSourceProperty, value); }
        }
        public static readonly DependencyProperty AutoCompleteCustomSourceProperty =
            DependencyProperty.Register("AutoCompleteCustomSource", typeof(int), typeof(AutoCompleteComboBox), new PropertyMetadata(null));

        #endregion

        #region Override methods

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Enter)
            {
                if (IsDropDownOpen)
                {
                    if (SelectedIndex == -1)
                    {
                        string oldText = Text;
                        IsDropDownOpen = false;
                        Text = oldText;
                    }
                }
            }
            else if (e.Key != Key.Up &&
                    e.Key != Key.Down &&
                    e.Key != Key.LeftCtrl &&
                    e.Key != Key.LeftShift &&
                    e.Key != Key.LeftAlt &&
                    e.Key != Key.Escape)
            {
                if (needUpdate)
                {
                    if (canUpdate)
                    {
                        canUpdate = false;
                        UpdateSuggestionData();
                    }
                    else
                    {
                        RestartTimer();
                    }
                }
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            needUpdate = true;
            base.OnTextInput(e);
        }

        #endregion

        #region Private methods

        private void RestartTimer()
        {
            if (timer == null)
            {
                timer = new Timer();
                timer.Elapsed += OnDelay;
                timer.Interval = SearchDelay;
            }
            timer.Stop();
            canUpdate = false;
            timer.Start();
        }

        private void OnDelay(object sender, ElapsedEventArgs e)
        {
            canUpdate = true;
            timer.Stop();
            UpdateSuggestionData();
        }

        private void UpdateSuggestionData()
        {
            if (Text.Length >= 1 && AutoCompleteCustomSource != null)
            {
                List<string> searchData = Find(Text);
                HandleTextChanged(searchData);
            }
            else
            {
                HandleTextChanged(null);
            }
        }

        private List<string> Find(string text)
        {
            List<string> searchData = new List<string>();
            foreach (string item in AutoCompleteCustomSource)
            {
                if (item.ToLower().Contains(text.ToLower()))
                    searchData.Add(item);
            }
            return searchData;
        }

        private void HandleTextChanged(List<string> dataSource)
        {
            if (oldDataSource == null)
                oldDataSource = ItemsSource;

            string text = Text;

            if (dataSource != null && dataSource.Count > 0)
            {
                SelectedIndex = -1;

                ItemsSource = dataSource;
                //string selectText = Items[0].ToString();
                //SelectionLength = selectText.Length - Text.Length;
                IsDropDownOpen = true;
                Text = text;
            }
            else
            {
                SelectedIndex = -1;
                ItemsSource = oldDataSource;
                Text = text;
                IsDropDownOpen = true;
            }
        }

        #endregion
    }
}
