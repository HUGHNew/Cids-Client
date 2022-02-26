using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Feedback : Form
    {
        #region Feedback Data and methods
        public const string FeedbackPlaceholder = "填写您的意见";
        public const int FeedbackLenghtLimit = 512;
        public const string FeedbackSubmitMsg = "谢谢您的宝贵意见";
        #endregion
        public Feedback()
        {
            InitializeComponent();
            FdTBIssue.Text = FeedbackPlaceholder; // set placeholder
            FdTBIssue.MaxLength = FeedbackLenghtLimit; // set maxlength
        }
        #region Button Enable Controls
        private void EnableSubmit(bool enable=true) {
            FdBtnSubmit.Enabled = enable;
        }
        #endregion
        #region [Obosolete] Focus Control
        private void FdIssueGotFocus(object sender, EventArgs e)
        {
            if(FdTBIssue.Text == FeedbackPlaceholder){
                FdTBIssue.Text = "";
            }
        }

        private void FdIssueLostFocus(object sender, EventArgs e) {
            if(String.IsNullOrWhiteSpace(FdTBIssue.Text)) {
                FdTBIssue.Text = FeedbackPlaceholder;
            }
        }
        #endregion
        private void FdTextChange(object sender, EventArgs e) {
            if (String.IsNullOrWhiteSpace(FdTBIssue.Text)) {
                FdTBIssue.Text= FeedbackPlaceholder;
                EnableSubmit(false);
            }else if (FdTBIssue.Text == FeedbackPlaceholder) {
                FdTBIssue.SelectionStart = FdTBIssue.TextLength;
                EnableSubmit(false);
            } else if(FdTBIssue.Text.StartsWith(FeedbackPlaceholder)){
                FdTBIssue.Text = FdTBIssue.Text.Substring(FeedbackPlaceholder.Length);
                FdTBIssue.SelectionStart = FdTBIssue.TextLength;
            } else { EnableSubmit(true); }
        }

        private void FdBtnCancelClick(object sender, EventArgs e) {
            Close();
        }
        private void IssueSubmit() {
            AutoClosingMessageBox.Show(FeedbackSubmitMsg);
            // get from ShowDialog
            // info from Button Result
        }
        private void FdBtnSubmitClick(object sender, EventArgs e) {
            Hide();
            IssueSubmit();
            Close();
        }
        public String FeedbackMessage => FdTBIssue.Text;
    }
}
