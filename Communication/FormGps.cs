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
    public partial class FormGps : Form
    {
        public FormGps()
        {
            InitializeComponent();
        }
        public string Ip = "";
        public string Port = "";
        public string Space = "1";
        public string Lng = "";
        public string Lat = "";
        public string Time = "";
        private void FormGps_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            this.Owner.Activate();

            timer1.Enabled = false;
        }

        private void btn_GetAddress_Click(object sender, EventArgs e)
        {
            Program.frmMain.send8106_IpPort(lab_DeviceID.Text);
        }

        private void btn_SetAddress_Click(object sender, EventArgs e)
        {
            Program.frmMain.send8103_IpPort(lab_DeviceID.Text);
        }

        private void btn_GetSpace_Click(object sender, EventArgs e)
        {
            Program.frmMain.send8106_Space(lab_DeviceID.Text);
        }

        private void btn_SetSpace_Click(object sender, EventArgs e)
        {
            Program.frmMain.send8103_Space(lab_DeviceID.Text);
        }

        private void btn_GetGps_Click(object sender, EventArgs e)
        {
            Program.frmMain.send8201(lab_DeviceID.Text);
        }

        private void btn_Reset_Click(object sender, EventArgs e)
        {
            Program.frmMain.send8105(lab_DeviceID.Text);
        }

        private void FormGps_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                txB_ServerIp.Text = Ip;
                txB_ServerPort.Text = Port;
                num_Space.Value = int.Parse(Space);
                lab_Lng.Text = Lng;
                lab_Lat.Text = Lat;
                lab_Time.Text = Time;
            }
            catch
            { }
        }
        public void start()
        {
            Ip = "";
            Port = "";
            Space = "1";
            Lng = "";
            Lat = "";
            Time = "";
        }
    }
}
