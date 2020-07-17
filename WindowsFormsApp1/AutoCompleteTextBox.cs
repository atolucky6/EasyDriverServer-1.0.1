using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class AutoCompleteTextBox : TextBox
    {
        #region Constructors

        public AutoCompleteTextBox() : base()
        {

        }

        #endregion

        #region Private members

        ListBox listBox;

        string oldText;

        Panel panel;

        List<string> currentAutoCompleteList;

        Form parentForm
        {
            get { return this.FindForm(); }
        }

        #endregion

        #region Public members

        public List<string> AutoCompleteList { get; set; }

        public int SelectedIndex { get; set; }


        #endregion

        #region Methods

        private void HideSuggestionListBox()
        {
            if (parentForm != null)
            {
                panel.Hide();
                if (parentForm.Controls.Contains(panel))
                    parentForm.Controls.Remove(panel);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        #endregion
    }
}
