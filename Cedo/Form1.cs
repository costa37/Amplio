using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Amplio.Properties;
using NUnrar;
using System.Diagnostics;

namespace Amplio
{
    internal partial class Form1 : Form
    {
        Form2 fr2 = new Form2();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Settings.Default.LastPath != null)
            {
                this.textBox1.Text = Settings.Default.LastPath; 
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        internal void Browse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FolderBrowserDialog1 = new FolderBrowserDialog();
            DialogResult result = FolderBrowserDialog1.ShowDialog();
            textBox1.Text = FolderBrowserDialog1.SelectedPath;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text; // For any change in the textbox1 be saved

            this.Hide();
            fr2.ShowDialog();
            this.Show();

        }

        internal void OK_Click(object sender, EventArgs e)
        {
            Variables.path = textBox1.Text;
            this.Hide();
            if (fr2.checkBox1.Checked)
            {
                ReadOnly rA = new ReadOnly();
                rA.RAAll();
            }
            else
            {
                MainFunction MF = new MainFunction();
                MF.OK_ClickLogic();
            }
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }
    }
}