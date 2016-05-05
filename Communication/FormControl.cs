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
    public partial class FormControl : Form
    {
        public FormControl()
        {
            InitializeComponent();
        }
        public string P1 = "";
        public string P2 = "";
        public string P3 = "";
        public string P4 = "";
        public string P5 = "";
        public string P6 = "";
        public string Time = "";
        private void FormControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            timer1.Enabled = false;
        }

        private void FormControl_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                lab_P1.Text = P1;
                lab_P2.Text = P2;
                lab_P3.Text = P3;
                lab_P4.Text = P4;
                lab_P5.Text = P5;
                lab_P6.Text = P6;
                lab_Time.Text = Time;
            }
            catch
            { }
        }
        public void start()
        {
            P1 = "";
            P2 = "";
            P3 = "";
            P4 = "";
            P5 = "";
            P6 = "";
            Time = "";
        }

        public void closeLock(string deviceID) //施封
        {
            string p1 = "00", p2 = "00", p3 = "00", p4 = "00", p5 = "00", p6 = "00";
            if (cheB_P1.Checked)
            {
                p1 = "0A";
            }
            if (cheB_P2.Checked)
            {
                p2 = "0A";
            }
            if (cheB_P3.Checked)
            {
                p3 = "0A";
            }
            if (cheB_P4.Checked)
            {
                p4 = "0A";
            }
            if (cheB_P5.Checked)
            {
                p5 = "0A";
            }
            if (cheB_P6.Checked)
            {
                p6 = "0A";
            }
            var tem = p1 + p2 + p3 + p4 + p5 + p6;

            string strTem = tem.Replace(" ", "");
            byte[] Ps = strToToHexByte(strTem);
            byte[] byteTem = comControl(Ps);
            strTem = byteToHexStr(byteTem);

            string sqlStr = "insert into DownCom  (DeviceID,CmdStr) values ('" + deviceID + "','" + strTem + "') ";
           Program.frmMain.sqlCmd(sqlStr,Program.frmMain.ConnStr);
        }
        public void openLock(string deviceID) //解封
        {
            string p1 = "00", p2 = "00", p3 = "00", p4 = "00", p5 = "00", p6 = "00";
            if (cheB_P1.Checked)
            {
                p1 = "09";
            }
            if (cheB_P2.Checked)
            {
                p2 = "09";
            }
            if (cheB_P3.Checked)
            {
                p3 = "09";
            }
            if (cheB_P4.Checked)
            {
                p4 = "09";
            }
            if (cheB_P5.Checked)
            {
                p5 = "09";
            }
            if (cheB_P6.Checked)
            {
                p6 = "09";
            }
            var tem = p1 + p2 + p3 + p4 + p5 + p6;
            string strTem = tem.Replace(" ", "");
            byte[] Ps = strToToHexByte(strTem);
            byte[] byteTem = comControl(Ps);
            strTem = byteToHexStr(byteTem);

            string sqlStr = "insert into DownCom  (DeviceID,CmdStr) values ('" + deviceID + "','" + strTem + "') ";
            Program.frmMain.sqlCmd(sqlStr, Program.frmMain.ConnStr);
        }
        public void readLock(string deviceID) //查询
        {
            byte[] byteTem = comRead();
            string strTem = byteToHexStr(byteTem);
            string sqlStr = "insert into DownCom  (DeviceID,CmdStr) values ('" + deviceID + "','" + strTem + "') ";
            Program.frmMain.sqlCmd(sqlStr, Program.frmMain.ConnStr);
        }
        public void resetLock(string deviceID) //复位
        {
            byte[] byteTem = comSendReset();
            string strTem = byteToHexStr(byteTem);
            string sqlStr = "insert into DownCom  (DeviceID,CmdStr) values ('" + deviceID + "','" + strTem + "') ";
            Program.frmMain.sqlCmd(sqlStr, Program.frmMain.ConnStr);
        }


        public byte[] comCrc(byte[] data)//com校验
        {
            CRC_HL = 0;

            byte[] re = { 0x00, 0x00 };

            string tem = byteToHexStr(data);
            tem = "7E" + tem;
            byte[] temb = strToToHexByte(tem);
            for (int i = 0; i < temb.Length; i++)
            {
                TRCRC(temb[i]);
            }
            byte[] CrcTem = BitConverter.GetBytes(CRC_HL);
            for (int i = CrcTem.Length - 1; i >= 0; i--)
            {
                re[CrcTem.Length - i - 1] = CrcTem[i];
            }

            return re;
        }

        UInt16 CRC_HL;
        void TRCRC(byte a)
        {
            byte i, j;
            ushort k;
            //CRC计算子程序（1字节）
            for (i = 0; i < 8; i++)
            {
                j = (byte)(a & 0x80);
                a = (byte)(a << 1);
                k = (UInt16)(CRC_HL & 0X8000);
                CRC_HL = (UInt16)(CRC_HL << 1);
                if (j != 0)
                    CRC_HL = (UInt16)(CRC_HL | 0X01);
                if (k != 0)
                    CRC_HL = (UInt16)(CRC_HL ^ 0X1021);
            }
        }

        public byte[] comSetHead(byte[] data)//com发送内容添加头尾
        {
            byte[] re = new byte[data.Length + 2];
            Array.Copy(data, 0, re, 1, data.Length);
            re[0] = 0x7E;
            re[re.Length - 1] = 0x7F;
            return re;
        }
        public byte[] comSetCrc(byte[] data)//com发送内容添加校验位
        {
            byte[] re = new byte[data.Length + 2];
            Array.Copy(data, 0, re, 0, data.Length);
            byte[] crc = comCrc(data);
            Array.Copy(crc, 0, re, re.Length - 2, crc.Length);
            return re;
        }
        public byte[] comRead()//com发送查询状态指令
        {
            byte[] re;
            byte cmdB = 0x01;
            byte[] dataTem = { cmdB };
            re = comSetCrc(dataTem);
            re = comSetHead(re);
            return re;
        }
        public byte[] comControl(byte[] data)//com发送解封/施封指令
        {
            byte[] re;
            byte cmdB = 0x02;
            byte[] dataTem = new byte[data.Length + 1];
            dataTem[0] = cmdB;
            Array.Copy(data, 0, dataTem, 1, data.Length);
            re = comSetCrc(dataTem);
            re = comSetHead(re);
            return re;
        }
        public byte[] comSendTime()//com发送日期时间指令
        {
            byte[] re;
            byte cmdB = 0x04;

            string dateStr = DateTime.Now.ToString("yyMMddHHmmss");
            byte[] dateB = strToToHexByte(dateStr);

            byte[] dataTem = new byte[dateB.Length + 1];
            dataTem[0] = cmdB;
            Array.Copy(dateB, 0, dataTem, 1, dateB.Length);
            re = comSetCrc(dataTem);
            re = comSetHead(re);
            return re;
        }
        public byte[] comSendReset()//com发送复位指令
        {
            byte[] re;
            byte cmdB = 0x05;
            byte[] dataTem = { cmdB };
            re = comSetCrc(dataTem);
            re = comSetHead(re);
            return re;
        }

        /// <summary> 
        /// 字符串转16进制字节数组 
        /// </summary> 
        /// <param name="hexString"></param> 
        /// <returns></returns> 
        private static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        ///字节数组转16进制字符串 
        /// <summary> 
        /// 字节数组转16进制字符串 
        /// </summary> 
        /// <param name="bytes"></param> 
        /// <returns></returns> 
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2") + " ";
                }
            }
            return returnStr;
        }

        private void btn_Sel_Click(object sender, EventArgs e)
        {
            readLock(lab_DeviceID.Text);
        }

        private void btn_Open_Click(object sender, EventArgs e)
        {
            openLock(lab_DeviceID.Text);
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            closeLock(lab_DeviceID.Text);
        }

        private void btn_SetTime_Click(object sender, EventArgs e)
        {
            Program.frmMain.SendDeviceTime(lab_DeviceID.Text);
        }

        private void btn_Reset_Click(object sender, EventArgs e)
        {
            resetLock(lab_DeviceID.Text);
        }

    }
}
