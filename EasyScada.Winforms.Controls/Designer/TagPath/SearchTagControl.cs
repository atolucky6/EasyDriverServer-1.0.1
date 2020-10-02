using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using EasyScada.Core;

namespace EasyScada.Winforms.Controls
{
    public partial class SearchTagControl : UserControl
    {
        #region Constructors
        public SearchTagControl()
        {
            InitializeComponent();

            gridView.CellDoubleClick += GridView_CellDoubleClick;
            txbSearch.TextChanged += TxbSearch_TextChanged;
            btnClearSearch.Click += BtnClearSearch_Click;

            gridView.VirtualMode = true;
            gridView.CellValueNeeded += GridView_CellValueNeeded;
            gridView.SelectionChanged += GridView_SelectionChanged;
            colAddress.Visible = false;
            colDataType.Visible = false;
            timerDelay.Interval = 300;
            timerDelay.Tick += TimerDelay_Tick;
        }
        #endregion

        #region Properties

        public object SelectedItem { get; set; }

        public bool IsInSearchMode { get; set; }
        public bool UseTagPath { get; set; }

        private List<string> tagPathsSearchResult = new List<string>();
        private List<string> tagPathSource;
        public List<string> TagPathSource
        {
            get => tagPathSource;
            set
            {
                UseTagPath = true;
                colDataType.Visible = false;
                colAddress.Visible = false;
                colName.HeaderText = "Tag Path";
                if (value != tagPathSource)
                {
                    tagPathSource = value;
                    gridView.RowCount = value.Count;
                }
                else
                {
                    if (value == null)
                    {
                        gridView.RowCount = 0;
                    }
                }
                RefreshView();
            }
        }

        private List<ICoreItem> coreItemsSearchResult = new List<ICoreItem>();
        private List<ICoreItem> coreItemSource;
        public List<ICoreItem> CoreItemSource
        {
            get => coreItemSource;
            set
            {
                UseTagPath = false;
                colDataType.Visible = true;
                colAddress.Visible = true;
                colName.HeaderText = "Name";
                if (value != coreItemSource)
                {
                    coreItemSource = value;
                    gridView.RowCount = value.Count;
                }
                else
                {
                    if (value == null)
                    {
                        gridView.RowCount = 0;
                    }
                }
                RefreshView();
            }
        }
        #endregion

        #region Events
        public event Action<object> SelectedItemDoubleClick;
        public event Action<object> SelectedItemChanged;
        #endregion

        #region Event handlers
        private void TimerDelay_Tick(object sender, EventArgs e)
        {
            try
            {
                IsInSearchMode = true;
                timerDelay.Stop();
                RefreshView();
            }
            catch { }
        }

        private void GridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (UseTagPath)
            {
                if (TagPathSource != null && e.RowIndex >= 0 && e.RowIndex < TagPathSource.Count)
                {
                    string currentTagPath = null;
                    if (IsInSearchMode)
                    {
                        if (tagPathsSearchResult != null && e.RowIndex < tagPathsSearchResult.Count)
                        {
                            currentTagPath = tagPathsSearchResult[e.RowIndex];
                        }
                    }
                    else
                    {
                        currentTagPath = TagPathSource[e.RowIndex];
                    }
                    if (currentTagPath != null)
                    {
                        switch (gridView.Columns[e.ColumnIndex].HeaderText)
                        {
                            case "Tag Path":
                                e.Value = currentTagPath;
                                break;
                            case "":
                                e.Value = (e.RowIndex + 1).ToString();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            else
            {
                if (CoreItemSource != null && e.RowIndex >= 0 && e.RowIndex < CoreItemSource.Count)
                {
                    ICoreItem currentCoreItem = null;
                    if (IsInSearchMode)
                    {
                        if (coreItemsSearchResult != null && e.RowIndex < coreItemsSearchResult.Count)
                        {
                            currentCoreItem = coreItemsSearchResult[e.RowIndex];
                        }
                    }
                    else
                    {
                        currentCoreItem = CoreItemSource[e.RowIndex];
                    }

                    if (currentCoreItem != null)
                    {
                        if (currentCoreItem is ITag tag)
                        {
                            switch (gridView.Columns[e.ColumnIndex].HeaderText)
                            {
                                case "Name":
                                    e.Value = tag.Name;
                                    break;
                                case "Address":
                                    e.Value = tag.Address;
                                    break;
                                case "DataType":
                                    e.Value = tag.DataType;
                                    break;
                                case "":
                                    e.Value = (e.RowIndex + 1).ToString();
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            switch (gridView.Columns[e.ColumnIndex].HeaderText)
                            {
                                case "Name":
                                    e.Value = currentCoreItem.Name;
                                    break;
                                case "":
                                    e.Value = (e.RowIndex + 1).ToString();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void BtnClearSearch_Click(object sender, EventArgs e)
        {
            txbSearch.Clear();
        }

        private void TxbSearch_TextChanged(object sender, EventArgs e)
        {
            IsInSearchMode = false;
            if (!string.IsNullOrEmpty(txbSearch.Text))
            {
                timerDelay.Stop();
                timerDelay.Start();
                return;
            }
            else
            {
                RefreshView();
            }
        }

        private void GridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (gridView.SelectedRows.Count > 0)
            {
                SelectedItemDoubleClick?.Invoke(gridView.SelectedRows[0].Cells["colName"].Value.ToString());
            }
        }

        private void GridView_SelectionChanged(object sender, EventArgs e)
        {
            if (gridView.SelectedRows.Count > 0)
            {
                SelectedItem = gridView.SelectedRows[0].Cells["colName"].Value;
                SelectedItemChanged?.Invoke(SelectedItem?.ToString());
            }
        }
        #endregion

        #region Methods
        private void RefreshView()
        {
            if (UseTagPath)
            {
                if (IsInSearchMode)
                {
                    tagPathsSearchResult.Clear();
                    string searchText = txbSearch.Text?.ToLower();
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        if (TagPathSource != null)
                            tagPathsSearchResult.AddRange(TagPathSource.Where(x => x.ToLower().Contains(searchText)));
                        gridView.RowCount = tagPathsSearchResult.Count;
                    }
                    else
                    {
                        IsInSearchMode = false;
                    }
                }

                if (!IsInSearchMode)
                {
                    gridView.RowCount = TagPathSource == null ? 0 : TagPathSource.Count;
                }
            }
            else
            {
                if (IsInSearchMode)
                {
                    coreItemsSearchResult.Clear();
                    string searchText = txbSearch.Text?.ToLower();
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        if (CoreItemSource != null)
                            coreItemsSearchResult.AddRange(CoreItemSource.Where(x => x.Name.ToLower().Contains(searchText)));
                        gridView.RowCount = coreItemsSearchResult.Count;
                    }
                    else
                    {
                        IsInSearchMode = false;
                    }
                }

                if (!IsInSearchMode)
                {
                    gridView.RowCount = CoreItemSource == null ? 0 : CoreItemSource.Count;
                }
            }
            gridView.Refresh();
        }
        #endregion
    }
}
