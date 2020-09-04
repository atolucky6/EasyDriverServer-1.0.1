using EasyScada.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EasyScada.Wpf.Controls
{
    public class EasyTextBox : TextBox, ISupportTag, ISupportInitialize, ISupportWriteSingleTag
    {
        #region Constructors

        static EasyTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EasyTextBox), new FrameworkPropertyMetadata(typeof(EasyTextBox)));
        }

        public EasyTextBox() : base()
        {
        }

        #endregion

        #region Public members

        [TypeConverter(typeof(TagPathConverter))]
        public string PathToTag
        {
            get { return (string)GetValue(PathToTagProperty); }
            set
            {
                if (value != PathToTag)
                {
                    SetValue(PathToTagProperty, value);
                    OnPathToTagPropertyChanged();
                }
            }
        }

        public static readonly DependencyProperty PathToTagProperty =
            DependencyProperty.Register("PathToTag", typeof(string), typeof(EasyTextBox), new PropertyMetadata(null));

        [Browsable(false)]
        public ITag LinkedTag { get; private set; }

        [Browsable(false)]
        public IEasyDriverConnector Connector { get => EasyDriverConnectorProvider.GetEasyDriverConnector(); }

        #region ISupportWriteTag

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public WriteTrigger WriteTrigger
        {
            get { return (WriteTrigger)GetValue(WriteTriggerProperty); }
            set { SetValue(WriteTriggerProperty, value); }
        }
        public static readonly DependencyProperty WriteTriggerProperty =
            DependencyProperty.Register("WriteTrigger", typeof(WriteTrigger), typeof(EasyTextBox), new PropertyMetadata(WriteTrigger.OnEnter));


        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public int WriteDelay
        {
            get { return (int)GetValue(WriteDelayProperty); }
            set { SetValue(WriteDelayProperty, value); }
        }
        public static readonly DependencyProperty WriteDelayProperty =
            DependencyProperty.Register("WriteDelay", typeof(int), typeof(EasyTextBox), new PropertyMetadata(0));

        public event EventHandler<TagWritingEventArgs> TagWriting;
        public event EventHandler<TagWritedEventArgs> TagWrited;

        #endregion

        #endregion

        #region Methods

        public override void EndInit()
        {
            base.EndInit();
            OnPathToTagPropertyChanged();
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            if (WriteTrigger == WriteTrigger.ValueChanged)
            {
                WriteTag(Text);
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            if (WriteTrigger == WriteTrigger.LostFocus)
            {
                WriteTag(Text);
                if (LinkedTag != null)
                    RefreshValue(LinkedTag.Value);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == Key.Enter)
            {
                if (WriteTrigger == WriteTrigger.OnEnter)
                    WriteTag(Text);
            }
            else if (e.Key == Key.Escape)
            {
                if (LinkedTag != null)
                    RefreshValue(LinkedTag.Value);
            }
        }

        private async void WriteTag(string writeValue)
        {
            if (Connector != null && 
                Connector.IsStarted && 
                LinkedTag != null && 
                IsNumber(writeValue))
            {
                await Task.Delay(WriteDelay);

                TagWriting?.Invoke(this, new TagWritingEventArgs(LinkedTag, writeValue));
                Quality writeQuality = Quality.Uncertain;
                //writeQuality = await LinkedTag.WriteAsync(writeValue);
                TagWrited?.Invoke(this, new TagWritedEventArgs(LinkedTag, writeQuality, writeValue));
                if (writeQuality == Quality.Bad)
                    RefreshValue(LinkedTag.Value);
            }
        }

        private bool IsNumber(string writeValue)
        {
            return decimal.TryParse(writeValue, out decimal value);
        }

        private void OnPathToTagPropertyChanged()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (LinkedTag != null)
                {
                    LinkedTag.ValueChanged -= OnValueChanged;
                    LinkedTag.QualityChanged -= OnQualityChanged;
                    LinkedTag = null;
                }

                if (Connector.IsStarted)
                {
                    OnConnectorStarted(Connector, null);
                }
                else
                {
                    Connector.Started += OnConnectorStarted;
                }
            }
        }

        private void OnConnectorStarted(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e != null)
                {
                    Connector.Started -= OnConnectorStarted;
                }

                if (LinkedTag == null)
                    LinkedTag = Connector.GetTag(PathToTag);

                if (LinkedTag != null)
                {
                    OnValueChanged(LinkedTag, new TagValueChangedEventArgs("", LinkedTag.Value));
                    LinkedTag.ValueChanged += OnValueChanged;
                    LinkedTag.QualityChanged += OnQualityChanged;
                }
            });
        }

        private void OnQualityChanged(object sender, TagQualityChangedEventArgs e)
        {

        }

        private void OnValueChanged(object sender, TagValueChangedEventArgs e)
        {
            DispatcherService.Instance.AddToDispatcherQueue(new Action(() =>
            {
                if (Text != e.NewValue && !IsFocused)
                {
                    Text = e.NewValue;
                }
            }));  
        }

        private void RefreshValue(string value)
        {
            DispatcherService.Instance.AddToDispatcherQueue(new Action(() =>
            {
                Text = value;
            }));
        }

        #endregion
    }
}
