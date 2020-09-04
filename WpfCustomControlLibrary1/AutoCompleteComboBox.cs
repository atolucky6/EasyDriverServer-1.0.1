using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfCustomControlLibrary1
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
            this.VisualTextRenderingMode = System.Windows.Media.TextRenderingMode.Auto;
            this.CacheMode = new BitmapCache();
            timer = new Timer();
            timer.Elapsed += OnDelay;
        }

        #endregion

        #region Private members

        private Timer timer;
        IEnumerable oldDataSource;
        List<string> searchResult;

        #endregion

        #region Public members

        public int SearchDelay
        {
            get { return (int)GetValue(SearchDelayProperty); }
            set { SetValue(SearchDelayProperty, value); }
        }
        public static readonly DependencyProperty SearchDelayProperty =
            DependencyProperty.Register("SearchDelay", typeof(int), typeof(AutoCompleteComboBox), new PropertyMetadata(500));

        public IEnumerable AutoCompleteCustomSource
        {
            get { return (IEnumerable)GetValue(AutoCompleteCustomSourceProperty); }
            set { SetValue(AutoCompleteCustomSourceProperty, value); }
        }
        public static readonly DependencyProperty AutoCompleteCustomSourceProperty =
            DependencyProperty.Register("AutoCompleteCustomSource", typeof(IEnumerable), typeof(AutoCompleteComboBox), new PropertyMetadata(null));

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

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

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
                timer.Interval = SearchDelay;
                timer.Stop();
                timer.Start();
            }
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);
        }

        #endregion

        #region Private methods

        private void OnDelay(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            UpdateSuggestionData();
        }

        private void UpdateSuggestionData()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Debug.WriteLine($"{DateTime.Now.ToLongTimeString()} - Searh text: {Text}");
                if (!string.IsNullOrEmpty(Text) && Text.Length >= 1 && AutoCompleteCustomSource != null)
                {
                    UpdateDataSource(Find(Text));
                }
                else
                {
                    UpdateDataSource(null);
                }
            }));
        }

        private List<string> Find(string text)
        {
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            searchResult = new List<string>();
            foreach (string item in AutoCompleteCustomSource)
            {
                if (item.ToLower().Contains(text.ToLower()))
                    searchResult.Add(item);
            }
            sw.Stop();
            Debug.WriteLine($"Search time: {sw.ElapsedMilliseconds}");
            return searchResult;
        }

        private void UpdateDataSource(List<string> dataSource)
        {
            if (oldDataSource == null)
                oldDataSource = ItemsSource;

            string text = Text;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (dataSource != null && dataSource.Count > 0)
                {
                    ItemsSource = dataSource;
                    IsDropDownOpen = true;
                }
                else
                {
                    SelectedIndex = -1;
                    ItemsSource = oldDataSource;
                    Text = text;
                    IsDropDownOpen = true;
                }
            }));
        }

        #endregion
    }
}
