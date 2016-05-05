using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Communication
{
    public partial class FormPwd : Form
    {
        public FormPwd()
        {
            InitializeComponent();
        }
        public string nextFrm="";

        private void btn_Set_Click(object sender, EventArgs e)
        {
            if (txB_Pwd.Text == "123456")
            {
                this.Close();
                if (nextFrm == "FormGps")
                {
                    Program.frmMain.frmGps.timer1.Enabled = true;
                    Program.frmMain.frmGps.start();
                    Program.frmMain.frmGps.ShowDialog(Program.frmMain);
                }
                if (nextFrm == "FormControl")
                {
                    Program.frmMain.frmControl.timer1.Enabled = true;
                    Program.frmMain.frmControl.start();
                    Program.frmMain.frmControl.ShowDialog(Program.frmMain);
                }
            }
            else
            {
                MessageBox.Show("密码错误！");
                txB_Pwd.Text = "";
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
