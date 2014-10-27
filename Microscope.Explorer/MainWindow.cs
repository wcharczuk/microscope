using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microscope.Explorer
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext.Default.Changed += DataContext_Changed;
        }

        void DataContext_Changed(object sender, DataContextChangedEventArgs e)
        {
            RebindListView();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            testCaseView.ContextMenuStrip = listBoxContextMenu;
            RebindListView();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowAddWindow();
        }

        private void RebindListView()
        {
            this.testCaseView.Items.Clear();
            foreach (var testCase in DataContext.Default)
            {
                if (DataContext.Default.LastRun != null && testCase.Passed != null)
                {
                    string label = "";
                    if (testCase.Errored != null && testCase.Errored.Value)
                    {
                        label = String.Format("{0} (Errored: {1})", testCase.Contents, testCase.Exception.Message);
                    }
                    else
                    {
                        label = String.Format("{0} ({1})", testCase.Contents, testCase.Passed.Value ? "Passed" : "Failed");
                    }
                    this.testCaseView.Items.Add(label);
                }
                else
                {
                    this.testCaseView.Items.Add(testCase.Contents);
                }
            }
        }

        private void ShowAddWindow()
        {
            var addWindow = new AddEditTestCase();
            addWindow.Text = "Add Test Case";
            addWindow.Show();
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this.query.Text))
            {
                MessageBox.Show("Need to enter a query!");
            }

            var queryCorpus = this.query.Text;
            DataContext.Default.RunQuery(queryCorpus);
            RebindListView();
        }

        private void testCaseView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                testCaseView.SelectedIndex = testCaseView.IndexFromPoint(e.Location);
                if (testCaseView.SelectedIndex != -1)
                {
                    listBoxContextMenu.Show();
                }
            }
        }

        private void addNewCaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAddWindow();
        }

        private void editCaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (testCaseView.SelectedIndex != -1)
            {
                var addWindow = new AddEditTestCase();
                addWindow.Text = "Edit Test Case";
                addWindow.TestCase = DataContext.Default.ElementAt(testCaseView.SelectedIndex);
                addWindow.Show();
            }
        }

        private void removeCaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (testCaseView.SelectedIndex != -1)
            {
                var testCase = DataContext.Default.ElementAt(testCaseView.SelectedIndex);
                DataContext.Default.Remove(testCase.Id);
            }
        }
    }
}