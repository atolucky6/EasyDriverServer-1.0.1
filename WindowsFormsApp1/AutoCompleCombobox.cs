using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class AutoCompleComboBox : ComboBox
    {
        #region Constructors

        public AutoCompleComboBox() : base()
        {
            
        }

        #endregion

        #region Private members

        private bool canUpdate = true;
        private bool needUpdate = false;
        private Timer timer;
        object oldDataSource;

        #endregion

        #region Public members

        #endregion

        #region Override methods

        protected override void OnDropDown(EventArgs e)
        {
            base.OnDropDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (DroppedDown)
                {
                    if (SelectedIndex == -1)
                    {
                        string oldText = Text;
                        DroppedDown = false;
                        Text = oldText;
                    }
                }

            }
            else if (e.KeyCode != Keys.Up && 
                e.KeyCode != Keys.Down && 
                e.KeyCode != Keys.Control && 
                e.KeyCode != Keys.Shift && 
                e.KeyCode != Keys.Alt && 
                e.KeyCode != Keys.Escape)
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
            base.OnKeyUp(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {

            base.OnKeyPress(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {

            base.OnTextChanged(e);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            needUpdate = true;
            base.OnSelectedIndexChanged(e);
        }

        protected override void OnTextUpdate(EventArgs e)
        {
            needUpdate = true;
            base.OnTextUpdate(e);
        }

        protected override void Dispose(bool disposing)
        {
            timer?.Stop();
            timer?.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #region Private methods

        private void RestartTimer()
        {
            if (timer == null)
            {
                timer = new Timer();
                timer.Tick += Timer_Tick;
                timer.Interval = 100;
            }
            timer.Stop();
            canUpdate = false;
            timer.Start();
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
                oldDataSource = DataSource;

            string text = Text;

            if (dataSource != null && dataSource.Count > 0)
            {
                SelectedIndex = -1;

                DataSource = dataSource;
                //string selectText = Items[0].ToString();
                //SelectionLength = selectText.Length - Text.Length;
                DroppedDown = true;
                Cursor.Current = Cursors.Default;
                Text = text;
                SelectionStart = text.Length;
            }
            else
            {
                SelectedIndex = -1;
                DataSource = oldDataSource;
                Text = text;
                DroppedDown = true;
                Cursor.Current = Cursors.Default;
                SelectionStart = text.Length;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            canUpdate = true;
            timer.Stop();
            UpdateSuggestionData();
        }

        #endregion
    }
}
