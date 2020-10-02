using EasyScada.Core;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace EasyScada.Wpf.Controls
{
    public class EasyLabel : Label, ISupportTag, ISupportInitialize
    {
        #region Constructors

        static EasyLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EasyLabel), new FrameworkPropertyMetadata(typeof(EasyLabel)));
        }

        public EasyLabel() : base()
        {
        }

        #endregion

        #region Public members

        [TypeConverter(typeof(TagPathConverter))]
        public string TagPath
        {
            get { return (string)GetValue(PathToTagProperty); }
            set
            {
                if (value != TagPath)
                {
                    SetValue(PathToTagProperty, value);
                    OnPathToTagPropertyChanged();
                }
            }
        }

        public static readonly DependencyProperty PathToTagProperty =
            DependencyProperty.Register("PathToTag", typeof(string), typeof(EasyLabel), new PropertyMetadata(null));

        [Browsable(false)]
        public ITag LinkedTag { get; private set; }

        [Browsable(false)]
        public IEasyDriverConnector Connector { get => EasyDriverConnectorProvider.GetEasyDriverConnector(); }

        #endregion

        #region Methods

        public override void EndInit()
        {
            base.EndInit();
            OnPathToTagPropertyChanged();
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
                    LinkedTag = Connector.GetTag(TagPath);

                if (LinkedTag != null)
                {
                    OnValueChanged(LinkedTag, new TagValueChangedEventArgs(LinkedTag, "", LinkedTag.Value));
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
                Content = e.NewValue;
            }));
        }

        #endregion
    }
}
