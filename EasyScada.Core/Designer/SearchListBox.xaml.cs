using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace EasyScada.Core
{
    /// <summary>
    /// Interaction logic for SearchListBox.xaml
    /// </summary>
    public partial class SearchListBox : UserControl
    {
        #region Constructors

        public SearchListBox()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                txbSearch.KeyUp += OnSearchTextBoxKeyUp;
                txbSearch.TextChanged += OnSearchTextChanged;
                lbResult.SelectionChanged += OnListBoxSelectionChanged;

                timer = new Timer();
                timer.Interval = SearchDelay;
                timer.Elapsed += OnDelay;
            }
        }

        #endregion

        #region Private members
        private readonly Timer timer;
        #endregion

        #region Public members
        public int SearchDelay { get; set; } = 100;
        IEnumerable itemsSource;
        public IEnumerable ItemsSource
        {
            get { return itemsSource; }
            set
            {
                if (itemsSource != value)
                {
                    itemsSource = value;
                    lbResult.ItemsSource = value;
                }
            }
        }
        #endregion

        #region Event handlers

        private void OnListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void OnSearchTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

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

        #endregion

        #region Private methods

        private void OnDelay(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateSuggestionData();
            }), DispatcherPriority.Background);
        }

        private void UpdateSuggestionData()
        {
            if (txbSearch.Text.Length >= 1 && ItemsSource != null)
            {
                IEnumerable<string> searchData = Find(txbSearch.Text);
                HandleTextChanged(searchData);
            }
            else
            {
                HandleTextChanged(null);
            }
        }

        private IEnumerable<string> Find(string text)
        {
            foreach (string item in ItemsSource)
            {
                if (item.ToLower().Contains(text.ToLower()))
                    yield return item;
            }
        }

        private void HandleTextChanged(IEnumerable<string> dataSource)
        {
            if (dataSource != null && dataSource.Any())
            {
                lbResult.ItemsSource = dataSource;
            }
            else
            {
                lbResult.ItemsSource = ItemsSource;
            }
        }

        #endregion
    }
}
