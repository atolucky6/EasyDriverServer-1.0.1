using EasyScada.Core;
using EasyScada.Core.Animate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyScada.Winforms.Controls
{
    public partial class AnimateDesignerForm : EasyForm
    {
        #region Inners
        public enum EditState
        {
            None,
            Add,
            Edit
        }

        public enum TriggerTab
        {
            Analog,
            Discrete,
            Quality
        }
        #endregion

        public AnimateDesignerForm(TriggerCollection triggersSource, IServiceProvider serviceProvide, string defaultTriggerTag = "")
        {
            InitializeComponent();
            this.serviceProvider = serviceProvide;
            this.defaultTriggerTag = defaultTriggerTag;
            qualityGridView.AutoGenerateColumns = false;
            discreteGridView.AutoGenerateColumns = false;
            analogGridView.AutoGenerateColumns = false;

            this.triggersSource = triggersSource;
            DiscreteTriggers = new BindingList<DiscreteTrigger>();
            AnalogTriggers = new BindingList<AnalogTrigger>();
            QualityTriggers = new BindingList<QualityTrigger>();

            foreach (var item in triggersSource)
            {
                if (item is DiscreteTrigger discreteTrigger)
                    DiscreteTriggers.Add(discreteTrigger);
                else if (item is AnalogTrigger analogTrigger)
                    AnalogTriggers.Add(analogTrigger);
                else if (item is QualityTrigger qualityTrigger)
                    QualityTriggers.Add(qualityTrigger);
            }

            discreteGridView.DataSource = DiscreteTriggers;
            analogGridView.DataSource = AnalogTriggers;
            qualityGridView.DataSource = QualityTriggers;

            btnAddQuality.Click += BtnAddQuality_Click;
            btnEditQuality.Click += BtnEditQuality_Click;
            btnSaveQuality.Click += BtnSaveQuality_Click;
            btnCancelQuality.Click += BtnCancelQuality_Click;
            btnDeleteQuality.Click += BtnDeleteQuality_Click;
            btnBrowseTriggerTagQuality.Click += BtnBrowseTagQuality_Click;

            foreach (var item in Enum.GetValues(typeof(Quality)))
                cobQuality.Items.Add(item.ToString());
            cobCompareModeQuality.Items.Add(CompareMode.Equal.ToString());
            cobCompareModeQuality.Items.Add(CompareMode.NotEqual.ToString());
            qualityGridView.SelectionChanged += QualityGridView_SelectionChanged;

            btnAddDiscrete.Click += BtnAddDiscrete_Click;
            btnEditDiscrete.Click += BtnEditDiscrete_Click;
            btnDeleteDiscrete.Click += BtnDeleteDiscrete_Click;
            btnSaveDiscrete.Click += BtnSaveDiscrete_Click;
            btnCancelDiscrete.Click += BtnCancelDiscrete_Click;
            btnBrowseTriggerTagDiscrete.Click += BtnBrowseTriggerTagDiscrete_Click;
            btnBrowseTriggerValueDiscrete.Click += BtnBrowseTriggerValueDiscrete_Click;
            discreteGridView.SelectionChanged += DiscreteGridView_SelectionChanged;
            cobCompareModeDiscrete.Items.Add(CompareMode.Equal.ToString());
            cobCompareModeDiscrete.Items.Add(CompareMode.NotEqual.ToString());

            btnAddAnalog.Click += BtnAddAnalog_Click;
            btnEditAnalog.Click += BtnEditAnalog_Click;
            btnDeleteAnalog.Click += BtnDeleteAnalog_Click;
            btnSaveAnalog.Click += BtnSaveAnalog_Click;
            btnCancelAnalog.Click += BtnCancelAnalog_Click;
            btnBrowseMaxValueAnalog.Click += BtnBrowseMaxValueAnalog_Click;
            btnBrowseMinValueAnalog.Click += BtnBrowseMinValueAnalog_Click;
            btnBrowseTriggerTagAnalog.Click += BtnBrowseTriggerTagAnalog_Click;
            analogGridView.SelectionChanged += AnalogGridView_SelectionChanged;

            cobCompareModeAnalog.Items.Add(CompareMode.InRange.ToString());
            cobCompareModeAnalog.Items.Add(CompareMode.OutRange.ToString());
            cobCompareModeAnalog.Items.Add(CompareMode.Larger.ToString());
            cobCompareModeAnalog.Items.Add(CompareMode.Smaller.ToString());

            easyNavigator1.TabClicked += EasyNavigator1_TabClicked;

            UpdateQualityButtons();
            UpdateDiscreteButtons();
            UpdateAnalogButtons();
            EasyNavigator1_TabClicked(easyNavigator1, new Navigator.EasyPageEventArgs(easyNavigator1.SelectedPage, easyNavigator1.SelectedIndex));
            btnResetProperty.Click += BtnResetProperty_Click;
            resetToDefaultToolStripMenuItem.Click += ResetToDefaultToolStripMenuItem_Click;
        }

        #region Fields
        private string defaultTriggerTag;
        private IServiceProvider serviceProvider;
        private TriggerCollection triggersSource;
        public BindingList<DiscreteTrigger> DiscreteTriggers { get; set; }
        public BindingList<AnalogTrigger> AnalogTriggers { get; set; }
        public BindingList<QualityTrigger> QualityTriggers { get; set; }
        private DiscreteTrigger selectedDiscreteTrigger;
        public DiscreteTrigger SelectedDiscreteTrigger
        {
            get => selectedDiscreteTrigger;
            set
            {
                selectedDiscreteTrigger = value;
                DisplayAnimateProperties();
                UpdateDiscreteButtons();
                DisplayPropertiesDiscreteTrigger(value);
            }
        }

        private AnalogTrigger selectedAnalogTrigger;
        public AnalogTrigger SelectedAnalogTrigger
        {
            get => selectedAnalogTrigger;
            set
            {
                selectedAnalogTrigger = value;
                DisplayAnimateProperties();
                UpdateAnalogButtons();
                DisplayPropertiesAnalogTrigger(value);
            }
        }

        private QualityTrigger selectedQualityTrigger;
        public QualityTrigger SelectedQualityTrigger
        {
            get => selectedQualityTrigger;
            set
            {
                selectedQualityTrigger = value;
                DisplayAnimateProperties();
                UpdateQualityButtons();
                DisplayPropertiesQualityTrigger(value);
            }
        }

        private EditState qualittyEditState;
        public EditState QualityEditState
        {
            get => qualittyEditState;
            set
            {
                if (qualittyEditState != value)
                {
                    qualittyEditState = value;
                    UpdateQualityButtons();
                }
            }
        }

        private EditState discreteEditState;
        public EditState DiscreteEditState
        {
            get => discreteEditState;
            set
            {
                if (discreteEditState != value)
                {
                    discreteEditState = value;
                    UpdateDiscreteButtons();
                }
            }
        }

        private EditState analogEditState;
        public EditState AnalogEditState
        {
            get => analogEditState;
            set
            {
                if (analogEditState != value)
                {
                    analogEditState = value;
                    UpdateAnalogButtons();
                }
            }
        }

        private TriggerTab selectedTriggerTab;
        public TriggerTab SelectedTriggerTab
        {
            get => selectedTriggerTab;
            set
            {
                selectedTriggerTab = value;
                DisplayAnimateProperties();

            }
        }

        private QualityTrigger editItemQualityTrigger;
        private AnalogTrigger editItemAnalogTrigger;
        private DiscreteTrigger editItemDiscreteTrigger;
        #endregion

        #region Event handlers

        private void ResetToDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (propertyGrid.SelectedGridItem != null)
            {
                if (propertyGrid.SelectedGridItem.Value is AnimatePropertyBase property)
                {
                    property.ResetToDefault();
                    propertyGrid.Refresh();
                }
            }
        }

        private void BtnResetProperty_Click(object sender, EventArgs e)
        {
            var mbr = EasyMessageBox.Show(this, "Do you want to reset all animate properties to default ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (mbr == DialogResult.Yes)
            {
                if (propertyGrid.SelectedObject is AnimatePropertyWrapper propertyWrapper)
                {
                    foreach (AnimatePropertyBase animateProperty in propertyWrapper.GetAnimateProperties())
                    {
                        animateProperty.ResetToDefault();
                    }
                }
                propertyGrid.Refresh();
            }
        }

        private void EasyNavigator1_TabClicked(object sender, Navigator.EasyPageEventArgs e)
        {
            SelectedTriggerTab = (TriggerTab)e.Index;
        }

        #region Quality
        private void BtnBrowseTagQuality_Click(object sender, EventArgs e)
        {
            if (QualityEditState != EditState.None)
            {
                if (ShowSelectTag(out string selectedTag))
                {
                    txbTriggerTagQuality.Text = selectedTag;
                }
            }
        }

        private void BtnCancelQuality_Click(object sender, System.EventArgs e)
        {
            QualityEditState = EditState.None;
            DisplayPropertiesQualityTrigger(SelectedQualityTrigger);
        }

        private void BtnDeleteQuality_Click(object sender, System.EventArgs e)
        {
            if (SelectedQualityTrigger != null)
            {
                var mbr = MessageBox.Show("Do you want to delete selected object ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (mbr == DialogResult.Yes)
                {
                    QualityTriggers.Remove(SelectedQualityTrigger);
                    triggersSource.Remove(SelectedQualityTrigger);
                }
            }
            UpdateQualityButtons();
        }

        private void BtnSaveQuality_Click(object sender, System.EventArgs e)
        {
            Enum.TryParse(cobQuality.Text, out Quality quality);
            Enum.TryParse(cobCompareModeQuality.Text, out CompareMode compareMode);
            editItemQualityTrigger.Enabled = ckbEnabledQuality.Checked;
            editItemQualityTrigger.TriggerTagPath = txbTriggerTagQuality.Text?.Trim();
            editItemQualityTrigger.TriggerQuality = quality;
            editItemQualityTrigger.Delay = decimal.ToInt32(numDelayQuality.Value);
            editItemQualityTrigger.Description = txbDescriptionQuality.Text?.Trim();
            if (compareMode != CompareMode.Equal && compareMode != CompareMode.NotEqual)
                compareMode = CompareMode.Equal;
            editItemQualityTrigger.CompareMode = compareMode;

            if (QualityEditState == EditState.Add)
            {
                QualityTriggers.Add(editItemQualityTrigger);
                triggersSource.Add(editItemQualityTrigger);
            }

            qualityGridView.ClearSelection();
            QualityEditState = EditState.None;
            foreach (DataGridViewRow row in qualityGridView.Rows)
            {
                if (row.DataBoundItem == editItemQualityTrigger)
                    row.Selected = true;
            }
            DisplayAnimateProperties();
            editItemQualityTrigger = null;
        }

        private void BtnEditQuality_Click(object sender, System.EventArgs e)
        {
            QualityEditState = EditState.Edit;
            editItemQualityTrigger = SelectedQualityTrigger;
            txbTriggerTagQuality.Focus();
        }

        private void BtnAddQuality_Click(object sender, System.EventArgs e)
        {
            QualityEditState = EditState.Add;
            editItemQualityTrigger = new QualityTrigger() { Enabled = true };
            DisplayPropertiesQualityTrigger(editItemQualityTrigger);
            txbTriggerTagQuality.Text = defaultTriggerTag;
            txbTriggerTagQuality.Focus();
        }

        private void QualityGridView_SelectionChanged(object sender, System.EventArgs e)
        {
            if (qualityGridView.SelectedRows.Count > 0)
            {
                SelectedQualityTrigger = qualityGridView.SelectedRows[0].DataBoundItem as QualityTrigger;
            }
        }
        #endregion

        #region Discrete
        private void BtnBrowseTriggerValueDiscrete_Click(object sender, EventArgs e)
        {
        }

        private void BtnBrowseTriggerTagDiscrete_Click(object sender, EventArgs e)
        {
            if (DiscreteEditState != EditState.None)
            {
                if (ShowSelectTag(out string selectedTag))
                {
                    txbTriggerTagDiscrete.Text = selectedTag;
                }
            }
        }

        private void BtnCancelDiscrete_Click(object sender, EventArgs e)
        {
            DiscreteEditState = EditState.None;
            DisplayPropertiesDiscreteTrigger(SelectedDiscreteTrigger);
        }

        private void BtnSaveDiscrete_Click(object sender, EventArgs e)
        {
            Enum.TryParse(cobCompareModeDiscrete.Text, out CompareMode compareMode);
            editItemDiscreteTrigger.Enabled = ckbEnabledDiscrete.Checked;
            editItemDiscreteTrigger.TriggerTagPath = txbTriggerTagDiscrete.Text?.Trim();
            editItemDiscreteTrigger.TriggerValue = txbTriggerValueDiscrete.Text?.Trim();
            editItemDiscreteTrigger.Delay = decimal.ToInt32(numDelayDiscrete.Value);
            editItemDiscreteTrigger.Description = txbDescriptionDiscrete.Text?.Trim();
            if (compareMode == CompareMode.InRange || compareMode == CompareMode.OutRange)
                compareMode = CompareMode.Equal;
            editItemDiscreteTrigger.CompareMode = compareMode;

            if (DiscreteEditState == EditState.Add)
            {
                DiscreteTriggers.Add(editItemDiscreteTrigger);
                triggersSource.Add(editItemDiscreteTrigger);
            }

            discreteGridView.ClearSelection();
            DiscreteEditState = EditState.None;
            foreach (DataGridViewRow row in discreteGridView.Rows)
            {
                if (row.DataBoundItem == editItemDiscreteTrigger)
                    row.Selected = true;
            }
            DisplayAnimateProperties();
            editItemDiscreteTrigger = null;
        }

        private void BtnDeleteDiscrete_Click(object sender, EventArgs e)
        {
            if (SelectedDiscreteTrigger != null)
            {
                var mbr = MessageBox.Show("Do you want to delete selected object ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (mbr == DialogResult.Yes)
                {
                    DiscreteTriggers.Remove(SelectedDiscreteTrigger);
                    triggersSource.Remove(SelectedDiscreteTrigger);
                }
            }
            UpdateDiscreteButtons();
        }

        private void BtnEditDiscrete_Click(object sender, EventArgs e)
        {
            DiscreteEditState = EditState.Edit;
            editItemDiscreteTrigger = SelectedDiscreteTrigger; ;
            txbTriggerTagDiscrete.Focus();
        }

        private void BtnAddDiscrete_Click(object sender, EventArgs e)
        {
            DiscreteEditState = EditState.Add;
            editItemDiscreteTrigger = new DiscreteTrigger() { Enabled = true };
            DisplayPropertiesDiscreteTrigger(editItemDiscreteTrigger);
            txbTriggerTagDiscrete.Text = defaultTriggerTag;
            txbTriggerTagDiscrete.Focus();
        }

        private void DiscreteGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (discreteGridView.SelectedRows.Count > 0)
            {
                SelectedDiscreteTrigger = discreteGridView.SelectedRows[0].DataBoundItem as DiscreteTrigger;
            }
        }
        #endregion

        #region Analog
        private void AnalogGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (analogGridView.SelectedRows.Count > 0)
            {
                SelectedAnalogTrigger = analogGridView.SelectedRows[0].DataBoundItem as AnalogTrigger;
            }
        }

        private void BtnBrowseTriggerTagAnalog_Click(object sender, EventArgs e)
        {
            if (AnalogEditState != EditState.None)
            {
                if (ShowSelectTag(out string selectedTag))
                {
                    txbTriggerTagAnalog.Text = selectedTag;
                }
            }
        }

        private void BtnBrowseMinValueAnalog_Click(object sender, EventArgs e)
        {
        }

        private void BtnBrowseMaxValueAnalog_Click(object sender, EventArgs e)
        {
        }

        private void BtnCancelAnalog_Click(object sender, EventArgs e)
        {
            AnalogEditState = EditState.None;
            DisplayPropertiesAnalogTrigger(SelectedAnalogTrigger);
        }

        private void BtnSaveAnalog_Click(object sender, EventArgs e)
        {
            Enum.TryParse(cobCompareModeAnalog.Text, out CompareMode compareMode);
            editItemAnalogTrigger.Enabled = ckbEnabledAnalog.Checked;
            editItemAnalogTrigger.TriggerTagPath = txbTriggerTagAnalog.Text?.Trim();
            editItemAnalogTrigger.MaxValue = txbMaxValueAnalog.Text?.Trim();
            editItemAnalogTrigger.MinValue = txbMinValueAnalog.Text?.Trim();
            editItemAnalogTrigger.Delay = decimal.ToInt32(numDelayAnalog.Value);
            editItemAnalogTrigger.Description = txbDescriptionAnalog.Text?.Trim();
            if (compareMode == CompareMode.Equal || compareMode == CompareMode.NotEqual)
                compareMode = CompareMode.InRange;
            editItemAnalogTrigger.CompareMode = compareMode;

            if (AnalogEditState == EditState.Add)
            {
                AnalogTriggers.Add(editItemAnalogTrigger);
                triggersSource.Add(editItemAnalogTrigger);
            }

            analogGridView.ClearSelection();
            AnalogEditState = EditState.None;
            foreach (DataGridViewRow row in analogGridView.Rows)
            {
                if (row.DataBoundItem == editItemAnalogTrigger)
                    row.Selected = true;
            }
            DisplayAnimateProperties();
            editItemAnalogTrigger = null;
        }

        private void BtnDeleteAnalog_Click(object sender, EventArgs e)
        {
            if (SelectedAnalogTrigger != null)
            {
                var mbr = MessageBox.Show("Do you want to delete selected object ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (mbr == DialogResult.Yes)
                {
                    AnalogTriggers.Remove(SelectedAnalogTrigger);
                    triggersSource.Remove(SelectedAnalogTrigger);
                }
            }
            UpdateAnalogButtons();
        }

        private void BtnEditAnalog_Click(object sender, EventArgs e)
        {
            AnalogEditState = EditState.Edit;
            editItemAnalogTrigger = SelectedAnalogTrigger; 
            txbTriggerTagAnalog.Focus();
        }

        private void BtnAddAnalog_Click(object sender, EventArgs e)
        {
            AnalogEditState = EditState.Add;
            editItemAnalogTrigger = new AnalogTrigger() { Enabled = true };
            DisplayPropertiesAnalogTrigger(editItemAnalogTrigger);
            txbTriggerTagAnalog.Text = defaultTriggerTag;
            txbTriggerTagAnalog.Focus();
        }
        #endregion

        #endregion

        #region Methods

        public bool ShowSelectTag(out string selectedTag)
        {
            selectedTag = "";
            SelectTagPathDesignerForm form = new SelectTagPathDesignerForm(serviceProvider);
            if (form.ShowDialog() == DialogResult.OK)
            {
                selectedTag = form.SelectedTagPath;
                return true;
            }
            return false;
        }

        private void DisplayPropertiesQualityTrigger(QualityTrigger qualityTrigger)
        {
            if (qualityTrigger != null && QualityEditState == EditState.None)
            {
                ckbEnabledQuality.Checked = qualityTrigger.Enabled;
                txbTriggerTagQuality.Text = qualityTrigger.TriggerTagPath;
                cobQuality.Text = qualityTrigger.TriggerQuality.ToString();
                numDelayQuality.Value = qualityTrigger.Delay;
                cobCompareModeQuality.Text = qualityTrigger.CompareMode.ToString();
                txbDescriptionQuality.Text = qualityTrigger.Description;
            }
        }

        private void DisplayPropertiesDiscreteTrigger(DiscreteTrigger discreteTrigger)
        {
            if (discreteTrigger != null && DiscreteEditState == EditState.None)
            {
                ckbEnabledDiscrete.Checked = discreteTrigger.Enabled;
                txbTriggerTagDiscrete.Text = discreteTrigger.TriggerTagPath;
                txbTriggerValueDiscrete.Text = discreteTrigger.TriggerValue;
                numDelayDiscrete.Value = discreteTrigger.Delay;
                cobCompareModeDiscrete.Text = discreteTrigger.CompareMode.ToString();
                txbDescriptionDiscrete.Text = discreteTrigger.Description;
            }
        }

        private void DisplayPropertiesAnalogTrigger(AnalogTrigger analogTrigger)
        {
            if (analogTrigger != null && AnalogEditState == EditState.None)
            {
                ckbEnabledAnalog.Checked = analogTrigger.Enabled;
                txbTriggerTagQuality.Text = analogTrigger.TriggerTagPath;
                txbMinValueAnalog.Text = analogTrigger.MinValue;
                txbMaxValueAnalog.Text = analogTrigger.MaxValue;
                numDelayAnalog.Value = analogTrigger.Delay;
                cobCompareModeAnalog.Text = analogTrigger.CompareMode.ToString();
                txbDescriptionAnalog.Text = analogTrigger.Description;
            }
        }

        private void DisplayAnimateProperties()
        {
            switch (SelectedTriggerTab)
            {
                case TriggerTab.Analog:
                    propertyGrid.SelectedObject = SelectedAnalogTrigger?.AnimatePropertyWrapper;
                    break;
                case TriggerTab.Discrete:
                    propertyGrid.SelectedObject = SelectedDiscreteTrigger?.AnimatePropertyWrapper;
                    break;
                case TriggerTab.Quality:
                    propertyGrid.SelectedObject = SelectedQualityTrigger?.AnimatePropertyWrapper;
                    break;
                default:
                    break;
            }
            propertyGrid.ExpandAllGridItems();
        }

        private void UpdateQualityButtons()
        {
            switch (QualityEditState)
            {
                case EditState.None:
                    btnAddQuality.Enabled = true;
                    btnEditQuality.Enabled = SelectedQualityTrigger != null;
                    btnDeleteQuality.Enabled = SelectedQualityTrigger != null;
                    btnSaveQuality.Enabled = false;
                    btnCancelQuality.Enabled = false;
                    break;
                case EditState.Add:
                case EditState.Edit:
                    btnAddQuality.Enabled = false;
                    btnEditQuality.Enabled = false;
                    btnDeleteQuality.Enabled = false;
                    btnSaveQuality.Enabled = true;
                    btnCancelQuality.Enabled = true;
                    break;
                default:
                    break;
            }
        }

        private void UpdateDiscreteButtons()
        {
            switch (DiscreteEditState)
            {
                case EditState.None:
                    btnAddDiscrete.Enabled = true;
                    btnEditDiscrete.Enabled = SelectedDiscreteTrigger != null;
                    btnDeleteDiscrete.Enabled = SelectedDiscreteTrigger != null;
                    btnSaveDiscrete.Enabled = false;
                    btnCancelDiscrete.Enabled = false;
                    break;
                case EditState.Add:
                case EditState.Edit:
                    btnAddDiscrete.Enabled = false;
                    btnEditDiscrete.Enabled = false;
                    btnDeleteDiscrete.Enabled = false;
                    btnSaveDiscrete.Enabled = true;
                    btnCancelDiscrete.Enabled = true;
                    break;
                default:
                    break;
            }
        }

        private void UpdateAnalogButtons()
        {
            switch (AnalogEditState)
            {
                case EditState.None:
                    btnAddAnalog.Enabled = true;
                    btnEditAnalog.Enabled = SelectedAnalogTrigger != null;
                    btnDeleteAnalog.Enabled = SelectedAnalogTrigger != null;
                    btnSaveAnalog.Enabled = false;
                    btnCancelAnalog.Enabled = false;
                    break;
                case EditState.Add:
                case EditState.Edit:
                    btnAddAnalog.Enabled = false;
                    btnEditAnalog.Enabled = false;
                    btnDeleteAnalog.Enabled = false;
                    btnSaveAnalog.Enabled = true;
                    btnCancelAnalog.Enabled = true;
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}
