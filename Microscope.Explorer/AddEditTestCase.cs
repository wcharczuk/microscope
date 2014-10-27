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
    public partial class AddEditTestCase : Form
    {
        public AddEditTestCase()
        {
            InitializeComponent();
        }

        public TestCase TestCase { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this.TestCase != null)
            {
                this.query.Text = TestCase.Contents;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            var contents = this.query.Text;

            if (this.TestCase == null)
            {
                DataContext.Default.Add(contents);
            }
            else
            {
                DataContext.Default.Edit(this.TestCase.Id, contents);
            }
            this.Close();
        }
    }
}
