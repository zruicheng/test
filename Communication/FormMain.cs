using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Data.SqlClient;
using System.IO;
using Communicater;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace Communication
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisible(true)]
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;//允许线程访问窗体控件
        }

        //消息结构：标志位1 消息头 消息体 检验码1 标志位1
        //消息头：消息ID2 消息体属性2（保留2 分包1 数据加密方式3 消息体长度10） 终端手机号6 消息流水号2 消息包封装项（若分包0则无封装项，否则消息总包数2 包序号2）
        //消息体：
        //检验码：校验码指从消息头开始，同后一字节异或，直到校验码前一个字节，占用一个字节
        public struct Basic
        {
            public string Server;
            public string Database;
            public string User;
            public string Password;
        };
        ClassIni ClsIni1 = new ClassIni();
        Basic basic = new Basic();
        string PathStr = "";

        public string ConnStr = "";
        DataTable dtD = new DataTable();
        DataTable dtLog = new DataTable();

        int EventNo = -1;
        string EventStr = "";
        int[] lose;
        int loseTime = 600;

        Thread mythread;
        Thread storethread;
        Socket socket = null;
        Socket newSocket = null;
        Socket newSocket1 = null;
        //用字典对于通过IP地址查找套接口更方便          
        Dictionary<string, Socket> clientList = new Dictionary<string, Socket>();
        Dictionary<string, Thread> threadList = new Dictionary<string, Thread>();

        int[] YJCard;
        int[] comLose;
        string[] comLastStr;
        string[] P1;
        string[] P2;
        string[] P3;
        string[] P4;
        string[] P5;
        string[] P6;

        Thread threadCars;

        public FormGps frmGps = new FormGps();
        public FormControl frmControl = new FormControl();

        private void FormMain_Load(object sender, EventArgs e)
        {
            try
            {
                this.WindowState = FormWindowState.Maximized;
                //获取程序运行目录
                PathStr = Application.StartupPath.ToString();//
                getIni();

                dataGridView_Log.DataSource = dtLog;
                dtLog.Columns.Add("Time", typeof(string));
                dtLog.Columns.Add("Event", typeof(string));

                EventNo = 0;
                setLog("",EventNo, "程序启动，版本V_1.0.150608");
                EventStr = "";
                EventNo = -1;

                //dataGridView_DeviceList.Columns.Clear();
                dataGridView_DeviceList.DataSource = dtD;
                dtD.Columns.Add("终端号", typeof(string));
                dtD.Columns.Add("车牌号", typeof(string));            
                dtD.Columns.Add("Sim卡号", typeof(string));
                dtD.Columns.Add("卫星时间", typeof(string));
                dtD.Columns.Add("经度", typeof(string));
                dtD.Columns.Add("纬度", typeof(string));
                dtD.Columns.Add("速度", typeof(string));
                dtD.Columns.Add("连接", typeof(string));
                dtD.Columns.Add("方向", typeof(string));
                dtD.Columns.Add("高程", typeof(string));
                dtD.Columns.Add("油口", typeof(string));
                dtD.Columns.Add("原始接收", typeof(string));
                dtD.Columns.Add("透传接收", typeof(string));            
                dtD.Columns.Add("报警标志", typeof(string));
                dtD.Columns.Add("状态", typeof(string));

                try
                {
                    if (socket == null)
                    {
                        mythread = new Thread(new ThreadStart(BeginListen));
                        mythread.IsBackground = true;
                        mythread.Start();

                        storethread = new Thread(new ThreadStart(storeCarPosition));
                        storethread.IsBackground = true;
                        storethread.Start();
                    }
                    else
                    {

                    }
                }
                catch
                {
                    //MessageBox.Show(er.Message, "完成", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

                try
                {
                    threadCars = new Thread(new ThreadStart(dealCars));
                    threadCars.IsBackground = true;
                    threadCars.Start();
                }
                catch
                {
                    //MessageBox.Show(er.Message, "完成", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

                reLoad();

                string ipport = socket.LocalEndPoint.ToString();
                lab_LocalIP.Text = ipport.Split(':')[0];
                lab_LocalPort.Text = ipport.Split(':')[1];

                timerTranslate.Enabled = false;

                webBrowser1.ObjectForScripting = this;//具体公开的对象,这里可以公开自定义对象
                webBrowser1.ScriptErrorsSuppressed = true; //禁用错误脚本提示
                webBrowser1.IsWebBrowserContextMenuEnabled = false; //禁用右键菜单
                webBrowser1.WebBrowserShortcutsEnabled = false; //禁用快捷键
                string urlPath = Path.GetFullPath(PathStr + "/baidu.html").Replace(Path.DirectorySeparatorChar, '/');
                Uri url = new Uri(urlPath);//本来str_url是个地址
                webBrowser1.Url = url;
            }
            catch
            {
                MessageBox.Show("程序初始化失败！");
                exit = true;
                Application.Exit();
            }
        }

        int needGetCars = 0;
        public void dealCars()
        {
            while (true)
            {
                try
                {
                    string sql = "SELECT     DeviceID, CompanyID, CarNo, SimNo FROM  Car";
                    DataTable dtTem = sqlRead(sql, ConnStr);
                    if (dtTem.Rows.Count != dataGridView_DeviceList.Rows.Count)
                    {
                        needGetCars += 1;
                    }
                    else
                    {
                        for (int i = 0; i < dtTem.Rows.Count; i++)
                        {
                            string DeviceID = dtTem.Rows[i]["DeviceID"].ToString();
                            string CarNo = dtTem.Rows[i]["CarNo"].ToString();
                            string SimNo = dtTem.Rows[i]["SimNo"].ToString();
                            if (dtD.Rows[i]["终端号"].ToString() != DeviceID)
                            {
                                needGetCars += 1;
                                break;
                            }
                            else
                            {
                                if(dtD.Rows[i]["车牌号"].ToString() != CarNo)
                                    dtD.Rows[i]["车牌号"] = CarNo;
                                if (dtD.Rows[i]["Sim卡号"].ToString() != SimNo)
                                    dtD.Rows[i]["Sim卡号"] = SimNo;
                            }
                        }
                    }
                    
                    Thread.Sleep(10000);
                }
                catch
                {
                    Thread.Sleep(20000);
                }
            }
        }
        public void setLog(string deviceID, int No, string Event)
        {
            DataRow dr = dtLog.NewRow();
            dr["Time"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            dr["Event"] = Event;
            if (dtLog.Rows.Count > 50)
                dtLog.Rows.Remove(dtLog.Rows[0]);
            dtLog.Rows.Add(dr);
            if(dtLog.Rows.Count>0)
                dataGridView_Log.CurrentCell = dataGridView_Log.Rows[dtLog.Rows.Count - 1].Cells[0];

            if (No >= 0)
            {
                string sql = "insert into ServerLog (DeviceID,EventNo, Detail, StationType, Station, UserID, SysTime)values('" + deviceID + "'," + No + ",'" + Event + "',1,'" + "通信服务器" + "','" + "manager" + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                sqlCmd(sql, ConnStr);
            }
            EventStr = "";
            EventNo = -1;
        }
        public void reLoad()
        {
            EventNo = 5;
            setLog("", EventNo, "加载车辆、终端信息");
            EventStr = "";
            EventNo = -1;

            foreach (var item in clientList)
            {
                Socket s1 = (Socket)item.Value;
                s1.Shutdown(SocketShutdown.Both);
                s1.Close();
                s1.Dispose();

            }
            foreach (var item in threadList)
            {
                Thread s1 = (Thread)item.Value;
                s1.Abort();
                s1.DisableComObjectEagerCleanup();

            }
            clientList.Clear();
            threadList.Clear();

            if (newSocket != null)
            {
                newSocket.Dispose();
                newSocket = null;
            }
            if (newSocket1 != null)
            {
                newSocket1.Dispose();
                newSocket1 = null;
            }

            getTree();
        }
        public void getTree()
        {
            try
            {
                treeView_All.Nodes.Clear();
                dtD.Clear();

                string sql = "SELECT   MAX(CompanyLevel) as maxLevel FROM  Company";
                DataTable dtTem = sqlRead(sql, ConnStr);
                int maxLevel = int.Parse(dtTem.Rows[0]["maxLevel"].ToString());
                int level = 0;
                while (level <= maxLevel)
                {
                    sql = "SELECT  CompanyID, CompanyName, CompanyLevel, ParentID FROM  Company" +
                        " where CompanyLevel=" + level.ToString();
                    dtTem = sqlRead(sql, ConnStr);
                    for (int i = 0; i < dtTem.Rows.Count; i++)
                    {
                        string name = dtTem.Rows[i]["CompanyName"].ToString();
                        string ID = dtTem.Rows[i]["CompanyID"].ToString();
                        string ParentID = dtTem.Rows[i]["ParentID"].ToString();
                        if (level == 1)
                        {
                            TreeNode NdCurrent = new TreeNode();
                            NdCurrent.Name = ID;
                            NdCurrent.Text = name;
                            NdCurrent.Tag = "0";
                            treeView_All.Nodes.Add(NdCurrent);
                            NdCurrent.ImageIndex = 1;
                        }
                        else
                        {
                            TreeNode NdTem = null;
                            for (int j = 0; j < treeView_All.Nodes.Count; j++)
                            {
                                NdTem = FindNode(treeView_All.Nodes[j], ParentID, "0");
                                if (NdTem != null)
                                    break;
                            }
                            if (NdTem != null)
                            {
                                TreeNode NdCurrent = new TreeNode();
                                NdCurrent.Name = ID;
                                NdCurrent.Text = name;
                                NdCurrent.Tag = "0";
                                NdTem.Nodes.Add(NdCurrent);
                                NdCurrent.ImageIndex = 1;
                            }
                        }
                    }
                    level++;
                }

                sql = "SELECT     DeviceID, CompanyID, CarNo, P1, P2, P3, P4, P5, P6 FROM  Car";
                dtTem = sqlRead(sql, ConnStr);
                //if (dtTem.Rows.Count > 0)
                {
                    lose = new int[dtTem.Rows.Count];
                    YJCard = new int[dtTem.Rows.Count];
                    comLose = new int[dtTem.Rows.Count];
                    comLastStr = new string[dtTem.Rows.Count];
                    P1 = new string[dtTem.Rows.Count];
                    P2 = new string[dtTem.Rows.Count];
                    P3 = new string[dtTem.Rows.Count];
                    P4 = new string[dtTem.Rows.Count];
                    P5 = new string[dtTem.Rows.Count];
                    P6 = new string[dtTem.Rows.Count];
                }
                for (int i = 0; i < dtTem.Rows.Count; i++)
                {
                    string CompanyID = dtTem.Rows[i]["CompanyID"].ToString();
                    string DeviceID = dtTem.Rows[i]["DeviceID"].ToString();
                    string CarNo = dtTem.Rows[i]["CarNo"].ToString();
                    string strP1 = dtTem.Rows[i]["P1"].ToString();
                    string strP2 = dtTem.Rows[i]["P2"].ToString();
                    string strP3 = dtTem.Rows[i]["P3"].ToString();
                    string strP4 = dtTem.Rows[i]["P4"].ToString();
                    string strP5 = dtTem.Rows[i]["P5"].ToString();
                    string strP6 = dtTem.Rows[i]["P6"].ToString();

                    comLastStr[i] = "";
                    P1[i] = strP1;
                    P2[i] = strP2;
                    P3[i] = strP3;
                    P4[i] = strP4;
                    P5[i] = strP5;
                    P6[i] = strP6;

                    TreeNode NdTem = null;
                    for (int j = 0; j < treeView_All.Nodes.Count; j++)
                    {
                        NdTem = FindNode(treeView_All.Nodes[j], CompanyID, "0");
                        if (NdTem != null)
                            break;
                    }
                    if (NdTem != null)
                    {
                        TreeNode NdCurrent = new TreeNode();
                        NdCurrent.Name = DeviceID;
                        NdCurrent.Text = CarNo;
                        NdCurrent.Tag = "1";
                        NdTem.Nodes.Add(NdCurrent);
                        NdCurrent.ImageIndex = 2;

                        sql = "SELECT     DeviceID, CompanyID, CarNo, SimNo" +
                            " FROM         Car " +
                            " where DeviceID='" + DeviceID + "'";
                        DataTable dtTem1 = sqlRead(sql, ConnStr);
                        if (dtTem1 != null)
                        {
                            DataRow dr = dtD.NewRow();
                            dr["车牌号"] = dtTem1.Rows[0]["CarNo"].ToString();
                            dr["终端号"] = dtTem1.Rows[0]["DeviceID"].ToString();
                            dr["Sim卡号"] = dtTem1.Rows[0]["SimNo"].ToString();
                            dtD.Rows.Add(dr);
                        }
                    }
                }

                treeView_All.ExpandAll();
            }
            catch
            { }
        }
        private TreeNode FindNode(TreeNode tnParent, string strName, string Tag)
        {
            if (tnParent == null) return null;
            if (tnParent.Name == strName && tnParent.Tag.ToString() == Tag) return tnParent;
            TreeNode tnRet = null;
            foreach (TreeNode tn in tnParent.Nodes)
            {
                tnRet = FindNode(tn, strName, Tag);
                if (tnRet != null) break;
            }
            return tnRet;
        }
        int i = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                lab_Time.Text = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                if (EventStr.Length > 0)
                {
                    setLog("", EventNo, EventStr);
                    EventStr = "";
                    EventNo = -1;
                }
               
            }
            catch
            { }
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        bool exit = false;
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show(this, "你确定退出吗? \r\n\r\n点击\"确定\"退出程序 \r\n点击\"取消\"隐藏到系统托盘。", "退出提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                exit = true;

                EventNo = 4;
                setLog("", EventNo, "退出程序");
                EventStr = "";
                EventNo = -1;
            }
            if (dr == DialogResult.Cancel)
            {

                if (exit == false)
                {
                    this.Hide();
                    notifyIcon1.Visible = true;
                    this.notifyIcon1.ShowBalloonTip(500);

                    e.Cancel = true;
                }
                else
                { }
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            if (this.Visible == false)
            {
                this.Show();
                this.Activate();
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.Hide();
                notifyIcon1.Visible = true;
                this.notifyIcon1.ShowBalloonTip(500);
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.notifyIcon1.Visible = true;
                this.notifyIcon1.ShowBalloonTip(500);
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            stopAll();

            string sql = "update car set Status=0 ";
            sqlCmd(sql, ConnStr);

            try
            {
                sql = "Insert into RecordCarPosition( DeviceID, CarNo, Speed, Address, lng, lat,Blng,Blat, DTime,P1,P2,P3,P4,P5,P6, Direction, Status)  SELECT   DeviceID, CarNo, Speed, Address, lng, lat,Blng,Blat, DTime,P1,P2,P3,P4,P5,P6, Direction, Status from Car ";
                sqlCmd(sql, ConnStr);
                setRecord(sql);
            }
            catch
            { }
        }

        // 清理所有正在使用的资源。 
        public static IPAddress GetServerIP()
        {
            IPHostEntry ieh = Dns.GetHostByName(Dns.GetHostName());
            return ieh.AddressList[0];
            //return ieh.AddressList[3];
        }

        private void BeginListen()
        {

            IPAddress ServerIp = GetServerIP();//
            //IPAddress ServerIp = Dns.GetHostByName("169.254.210.215").AddressList[0];
            IPEndPoint iep = new IPEndPoint(ServerIp, int.Parse("8888"));

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Bind(iep);
                EventNo = 1;
                toolStripStatusLabel1.Text = iep.ToString();
                EventStr = "启动端口：" + iep.ToString();
            }
            catch
            {
                MessageBox.Show("端口已占用");
                EventStr = "端口已占用" + iep.ToString();
            }
            while (true)
            {
                try
                {
                    socket.Listen(100);
                    int xdd = socket.Available;
                    if (xdd != 0)
                    {
                        int dfd = xdd;
                    }
                    {
                        newSocket1 = socket.Accept();
                        string pointFromClient = newSocket1.RemoteEndPoint.ToString();
                        if (!clientList.ContainsKey(pointFromClient))
                        {
                            clientList.Add(pointFromClient, newSocket1);

                            Thread mythread1 = new Thread(receive);
                            mythread1.IsBackground = true;
                            mythread1.Start(newSocket1);
                            if (!threadList.ContainsKey(pointFromClient))
                            {
                                threadList.Add(pointFromClient, mythread1);
                            }
                            //EventStr"终端连接：" + newSocket1.RemoteEndPoint.ToString();
                        }
                    }
                }
                catch
                { }
            }
        }

        private void receive(object sokConnectionparn)
        {
            Socket socket12 = (Socket)sokConnectionparn;

            try
            {

                byte[] byteMessage = new byte[500];
                int x = 0;

                while ((x = socket12.Receive(byteMessage)) > 0)
                {
                    try
                    {
                        DataRow dr = null;
                        int index = -1;
                        for (int i = 0; i < dtD.Rows.Count; i++)
                        {
                            if (dtD.Rows[i]["连接"].ToString() == socket12.RemoteEndPoint.ToString())
                            {
                                index = i;
                                dr = dtD.Rows[i];
                                break;
                            }
                        }

                        string ConnNo = "";
                        string AlarmFlag = "";
                        string Status = "";
                        string Lng = "";
                        string Lat = "";
                        string Altitude = "";
                        string Speed = "";
                        string Direction = "";
                        string Time = "";
                        string ComRev = "";
                        string RevStr = "";
                        byte[] inbuf = new byte[x];
                        for (int i = 0; i < x; i++)
                        {
                            inbuf[i] = byteMessage[i];
                        }
                        string sTime = DateTime.Now.ToShortTimeString();
                        string ad = Encoding.GetEncoding("GBK").GetString(inbuf);
                        if (ad != "\r\n")
                        {
                            ConnNo = "";
                            AlarmFlag = "";
                            Status = "";
                            Lng = "0";
                            Lat = "0";
                            Altitude = "";
                            Speed = "0";
                            Direction = "0";
                            Time = "";
                            ComRev = "";
                            RevStr = "";

                            RevStr = byteToHexStr(inbuf);
                            if (inbuf.Length >= 12)
                            {
                                //去标识位
                                byte[] data = PackBack(inbuf);

                                //转义还原
                                data = TransBack(data);

                                byte checkByte = data[data.Length - 1];
                                byte[] dataTem = data;
                                data = new byte[data.Length - 1];
                                Array.Copy(dataTem, 0, data, 0, data.Length);
                                //校验
                                byte check = Check(data);
                                if (check == checkByte)
                                {
                                    //校验成功

                                    string IpPort = socket12.RemoteEndPoint.ToString();
                                    ////////解析消息头
                                    //解析“消息ID”
                                    string MsgID = getMsgID(data);
                                    //解析“消息体属性”
                                    string MsgStyle = getMsgStyle(data);
                                    //解析“终端手机号”
                                    string PhoneTem = getPhone(data);
                                    //解析“消息流水号”
                                    string MsgNo = getMsgNo(data);

                                    //获取电话号码
                                    ConnNo = PhoneTem.Replace(" ", "");

                                    {
                                        string sql = "update Car set  RevTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where DeviceID='" + ConnNo + "'";
                                        sqlCmd(sql, ConnStr);
                                    }
                                    if (dr == null)
                                    {
                                        index = -1;
                                        for (int i = 0; i < dtD.Rows.Count; i++)
                                        {
                                            string DeviceID = dtD.Rows[i]["终端号"].ToString();
                                            if (DeviceID == ConnNo)
                                            {
                                                index = i;
                                                dr = dtD.Rows[i];
                                                break;
                                            }
                                        }
                                    }
                                    if (index >= 0)
                                        lose[index] = 0;
                                    if (dr != null)
                                        dr["终端号"] = ConnNo;


                                    //分离“消息包封装项”
                                    byte[] dataBody = getMsgBody(data);

                                    ////////解析消息封装包
                                    string title = "";
                                    string[] strBody = null;
                                    if (MsgID == "0001")//终端通用应答  消息 ID：0x0001
                                    {
                                        title = "终端通用应答";
                                        strBody = get0001(dataBody);
                                    }
                                    if (MsgID == "0002")//终端心跳  消息 ID：0x0002
                                    {
                                        title = "终端心跳";

                                        //终端通用应答
                                        //send8001(IpPort, ConnNo, MsgNo, "0");
                                    }
                                    if (MsgID == "0100")//终端注册  消息 ID：0x0100
                                    {
                                        title = "终端注册";
                                        strBody = get0100(dataBody);
                                    }
                                    if (MsgID == "0003")//终端注销  消息 ID：0x0003
                                    {
                                        title = "终端注销";
                                    }
                                    if (MsgID == "0102")//终端鉴权  消息 ID：0x0102
                                    {
                                        title = "终端鉴权";
                                        strBody = get0102(dataBody);
                                        //鉴权码
                                        string Code = byteToHexStr(dataBody);

                                        //终端鉴权应答
                                        //send8100(IpPort, ConnNo, MsgNo, "0", Code);

                                        //SendDeviceAreas(IpPort, ConnNo);//更新终端内部区域
                                        SendDeviceCard(ConnNo);//下发终端应急卡
                                        SendDeviceTime(ConnNo);//下发终端时间

                                        //终端通用应答
                                        //send8001(IpPort, ConnNo, MsgNo, "0");
                                    }
                                    if (MsgID == "0104")//查询终端参数应答  消息 ID：0x0104
                                    {
                                        title = "查询终端参数应答";
                                        strBody = get0104(ConnNo,dataBody);
                                    }
                                    if (MsgID == "0107")//查询终端属性应答  消息 ID：0x0107
                                    {
                                        title = "查询终端属性应答";
                                        strBody = get0107(dataBody);
                                    }
                                    if (MsgID == "0108")//终端升级结果通知  消息 ID：0x0108
                                    {
                                        title = "终端升级结果通知";
                                        strBody = get0108(dataBody);
                                    }
                                    if (MsgID == "0200")//位置信息汇报  消息 ID：0x0200
                                    {
                                        title = "位置信息汇报";
                                        strBody = get0200(ConnNo, dataBody);

                                        //终端通用应答
                                        send8001(IpPort, ConnNo, MsgNo, "0");
                                    }
                                    if (MsgID == "0201")//位置信息查询应答  消息 ID：0x0201
                                    {
                                        title = "位置信息查询应答";
                                        strBody = get0201(ConnNo, dataBody);
                                    }
                                    if (MsgID == "0301")//事件报告  消息 ID：0x0301
                                    {
                                        title = "事件报告";
                                        strBody = get0301(dataBody);
                                    }
                                    if (MsgID == "0302")//提问应答  消息 ID：0x0302
                                    {
                                        title = "提问应答";
                                        strBody = get0302(dataBody);
                                    }
                                    if (MsgID == "0303")//信息点播/取消  消息 ID：0x0303
                                    {
                                        title = "信息点播/取消";
                                        strBody = get0303(dataBody);
                                    }
                                    if (MsgID == "0500")//车辆控制应答  消息 ID：0x0500
                                    {
                                        title = "车辆控制应答";
                                        strBody = get0500(ConnNo, dataBody);
                                    }
                                    if (MsgID == "0700")//行驶记录数据上传  消息 ID：0x0700
                                    {
                                        title = "行驶记录数据上传";
                                        strBody = get0700(dataBody);
                                    }
                                    if (MsgID == "0701")//电子运单上报  消息 ID：0x0701
                                    {
                                        title = "电子运单上报";
                                        strBody = get0701(dataBody);
                                    }
                                    if (MsgID == "0702")//驾驶员身份信息采集上报  消息 ID：0x0702
                                    {
                                        title = "驾驶员身份信息采集上报";
                                        strBody = get0702(dataBody);
                                    }
                                    if (MsgID == "0704")//定位数据批量上传  消息 ID：0x0704
                                    {
                                        title = "定位数据批量上传";
                                        strBody = get0704(dataBody);
                                    }
                                    if (MsgID == "0705")//CAN 总线数据上传  消息 ID：0x0705
                                    {
                                        title = "CAN 总线数据上传";
                                        strBody = get0705(dataBody);
                                    }
                                    if (MsgID == "0800")//多媒体事件信息上传  消息 ID：0x0800
                                    {
                                        title = "多媒体事件信息上传";
                                        strBody = get0800(dataBody);
                                    }
                                    if (MsgID == "0801")//多媒体数据上传  消息 ID：0x0801
                                    {
                                        title = "多媒体数据上传";
                                        strBody = get0801(ConnNo, dataBody);
                                    }
                                    if (MsgID == "0805")//摄像头立即拍摄命令应答  消息 ID：0x0805
                                    {
                                        title = "摄像头立即拍摄命令应答";
                                        strBody = get0805(dataBody);
                                    }
                                    if (MsgID == "0802")//存储多媒体数据检索应答  消息 ID：0x0802
                                    {
                                        title = "存储多媒体数据检索应答";
                                        strBody = get0802(dataBody);
                                    }
                                    if (MsgID == "0900")//数据上行透传  消息 ID：0x0900
                                    {
                                        title = "数据上行透传";
                                        strBody = get0900(dataBody);

                                        //终端通用应答
                                        //send8001(IpPort, ConnNo, MsgNo, "0");
                                    }
                                    if (MsgID == "0901")//数据压缩上报  消息 ID：0x0901
                                    {
                                        title = "数据压缩上报";
                                        strBody = get0901(dataBody);
                                    }
                                    if (MsgID == "0A00")//终端 RSA 公钥  消息 ID：0x0A00
                                    {
                                        title = "终端 RSA 公钥";
                                        strBody = get0A00(dataBody);
                                    }

                                    //解析坐标数据
                                    if (MsgID == "0200")
                                    {

                                        //报警标志
                                        AlarmFlag = strBody[0];
                                        if (dr != null)
                                            dr["报警标志"] = AlarmFlag;
                                        string AlarmTem = AlarmFlag.Replace(" ", "");
                                        if (AlarmTem.Substring(31 - 20, 1) == "1")
                                        {
                                            EventStr = "进出区域";
                                            EventNo = 6;
                                            //setLog(dr["终端号"].ToString(), EventNo, EventStr);
                                        }

                                        //状态
                                        Status = strBody[1];
                                        if (dr != null)
                                            dr["状态"] = Status;
                                        string StatusTem = Status.Replace(" ", "");
                                        if (StatusTem.Substring(31 - 1, 1) == "0")
                                        {
                                            //未定位
                                            string sql = "update car set Status=3, SiteIDNow=0  where DeviceID='" + ConnNo + "'";
                                            sqlCmd(sql, ConnStr);
                                        }
                                        if (StatusTem.Substring(31 - 1, 1) == "1")
                                        {
                                            //定位
                                            if (float.Parse(strBody[5]) > 0)
                                            {
                                                //运动
                                                string sql = "update car set Status=2  where DeviceID='" + ConnNo + "'";
                                                sqlCmd(sql, ConnStr);
                                            }
                                            else
                                            {
                                                //静止
                                                string sql = "update car set Status=1  where DeviceID='" + ConnNo + "'";
                                                sqlCmd(sql, ConnStr);
                                            }
                                        }

                                        //纬度
                                        Lat = strBody[2];
                                        if (dr != null)
                                            dr["纬度"] = Lat + "°";

                                        //经度
                                        Lng = strBody[3];
                                        if (dr != null)
                                            dr["经度"] = Lng + "°";

                                        //高程
                                        Altitude = strBody[4];
                                        if (dr != null)
                                            dr["高程"] = Altitude + "m";

                                        //速度
                                        Speed = strBody[5];
                                        if (dr != null)
                                            dr["速度"] = Speed + "km/h";

                                        //方向
                                        Direction = strBody[6];
                                        if (dr != null)
                                            dr["方向"] = Direction + "°";

                                        //卫星时间
                                        Time = strBody[7];

                                        try
                                        {
                                            if (frmGps.Visible)
                                            {
                                                if (ConnNo == frmGps.lab_DeviceID.Text)
                                                {
                                                    frmGps.Lng = Lng;
                                                    frmGps.Lat = Lat;
                                                    frmGps.Time = Time;
                                                }
                                            }
                                        }
                                        catch
                                        { }

                                        if (dr != null)
                                            dr["卫星时间"] = Time;

                                        {
                                            string sql = "update Car set Speed=" + Speed + ", Direction=" + Direction + ", DTime='" + Time + "' where DeviceID='" + ConnNo + "'";
                                            sqlCmd(sql, ConnStr);
                                        }
                                        double lngtem = double.Parse(Lng);
                                        double lattem = double.Parse(Lat);
                                        if (lngtem > 0 && lattem > 0 )
                                        {
                                            
                                                string sql = "select DeviceID,lng,lat from Car where DeviceID='" + ConnNo + "' ";
                                                DataTable dtTem = sqlRead(sql, ConnStr);
                                                for (int i = 0; i < dtTem.Rows.Count; i++)
                                                {
                                                    string lngStr = dtTem.Rows[i]["lng"].ToString();
                                                    string latStr = dtTem.Rows[i]["lat"].ToString();
                                                    if (lngStr != Lng || latStr != Lat)
                                                    {
                                                        sql = "update Car set Lng=" + Lng + ", Lat=" + Lat + ",BDFlag=0 where DeviceID='" + ConnNo + "'";
                                                        sqlCmd(sql, ConnStr);
                                                        //if (int.Parse(Speed) > 0)
                                                        {
                                                            //sql = "Insert into RecordCarPosition( DeviceID, CarNo, Speed, Address, lng, lat,Blng,Blat, DTime,P1,P2,P3,P4,P5,P6, Direction, Status)  SELECT   DeviceID, CarNo, Speed, Address, lng, lat,Blng,Blat, DTime,P1,P2,P3,P4,P5,P6, Direction, Status from Car where DeviceID='" + ConnNo + "'";
                                                            //sqlCmd(sql, ConnStr);

                                                            //setRecord(sql);
                                                        }
                                                    }
                                                }
                                        }
                                        {
                                            //发送com查询指令
                                            byte[] byteTem = comRead();
                                            string strTem = byteToHexStr(byteTem);
                                            string sql = "insert into DownCom  (DeviceID,CmdStr) values ('" + dr["终端号"].ToString() + "','" + strTem + "') ";
                                            sqlCmd(sql, ConnStr);

                                            comLose[index]++;
                                        }

                                    }
                                    //解析透传数据
                                    if (MsgID == "0900")
                                    {
                                        if (comLose[index] > 0)
                                            comLose[index]--;

                                        ComRev = strBody[1];
                                        if (comLastStr[index] != ComRev)//com上传与上次记录不同时
                                        {
                                            comLastStr[index] = ComRev;

                                            byte[] dataCom = strToToHexByte(ComRev);
                                            if (dataCom.Length > 4)
                                            {
                                                int start = -1;
                                                int end = -1;
                                                for (int i = 0; i < dataCom.Length; i++)
                                                {
                                                    if (dataCom[i] == 0x7E && start < 0)
                                                    {
                                                        start = i;
                                                    }
                                                    if (dataCom[i] == 0x7F && end < 0)
                                                    {
                                                        end = i;
                                                    }
                                                }
                                                if (end > start && end - start >= 4)
                                                {
                                                    byte[] dataTem1 = new byte[end - start + 1];
                                                    Array.Copy(dataCom, start, dataTem1, 0, dataTem1.Length);
                                                    byte[] dataBody1 = new byte[dataTem1.Length - 4];
                                                    Array.Copy(dataTem1, 1, dataBody1, 0, dataBody1.Length);
                                                    byte[] dataCrc = new byte[2];
                                                    Array.Copy(dataTem1, dataTem1.Length - 3, dataCrc, 0, dataCrc.Length);
                                                    byte[] crcTem = comCrc(dataBody1);
                                                    if (crcTem[0] == dataCrc[0] && crcTem[1] == dataCrc[1])//校验成功
                                                    {
                                                        byte cmdB = dataBody1[0];
                                                        switch (cmdB)
                                                        {
                                                            case 0x0A://下位机返回查询状态指令
                                                                {
                                                                    string P1 = dataBody1[1].ToString("X2");
                                                                    string P2 = dataBody1[2].ToString("X2");
                                                                    string P3 = dataBody1[3].ToString("X2");
                                                                    string P4 = dataBody1[4].ToString("X2");
                                                                    string P5 = dataBody1[5].ToString("X2");
                                                                    string P6 = dataBody1[6].ToString("X2");
                                                                    string Wd = dataBody1[7].ToString("X2");
                                                                    string Sd = dataBody1[8].ToString("X2");

                                                                    byte[] byteTem = new byte[6];
                                                                    Array.Copy(dataBody1, 1, byteTem, 0, byteTem.Length);
                                                                    //判断是否安装，未安装状态：CC
                                                                    string sql = "SELECT      DeviceID, P1, PNo1, P2, PNo2, P3, PNo3, P4, PNo4, P5, PNo5, P6, PNo6  FROM         CarPorts where DeviceID ='" + dr["终端号"].ToString() + "' ";
                                                                    DataTable dtTem = sqlRead(sql, ConnStr);
                                                                    for (int i = 0; i < dtTem.Rows.Count; i++)
                                                                    {
                                                                        string P1Tem = dtTem.Rows[i]["P1"].ToString();
                                                                        string P2Tem = dtTem.Rows[i]["P2"].ToString();
                                                                        string P3Tem = dtTem.Rows[i]["P3"].ToString();
                                                                        string P4Tem = dtTem.Rows[i]["P4"].ToString();
                                                                        string P5Tem = dtTem.Rows[i]["P5"].ToString();
                                                                        string P6Tem = dtTem.Rows[i]["P6"].ToString();
                                                                        if (P1Tem == "0")
                                                                        {
                                                                            P1 = "CC";
                                                                            byteTem[0] = 0xCC;
                                                                        }
                                                                        if (P2Tem == "0")
                                                                        {
                                                                            P2 = "CC";
                                                                            byteTem[1] = 0xCC;
                                                                        }
                                                                        if (P3Tem == "0")
                                                                        {
                                                                            P3 = "CC";
                                                                            byteTem[2] = 0xCC;
                                                                        }
                                                                        if (P4Tem == "0")
                                                                        {
                                                                            P4 = "CC";
                                                                            byteTem[3] = 0xCC;
                                                                        }
                                                                        if (P5Tem == "0")
                                                                        {
                                                                            P5 = "CC";
                                                                            byteTem[4] = 0xCC;
                                                                        }
                                                                        if (P6Tem == "0")
                                                                        {
                                                                            P6 = "CC";
                                                                            byteTem[5] = 0xCC;
                                                                        }
                                                                    }
                                                                    EventStr = "返回查询结果" + P1 + " " + P2 + " " + P3 + " " + P4 + " " + P5 + " " + P6 + " " + Wd + " " + Sd;
                                                                    EventNo = 6;
                                                                    //setLog(dr["终端号"].ToString(), EventNo, EventStr);

                                                                    //sql = "update Car set P1='" + P1 + "', P2='" + P2 + "', P3='" + P3 + "', P4='" + P4 + "', P5='" + P5 + "', P6='" + P6 + "', Wd='" + Wd + "', Sd='" + Sd + "' where DeviceID='" + ConnNo + "'";
                                                                    //sqlCmd(sql, ConnStr);
                                                                    
                                                                    checkPort(byteTem, ConnNo, index, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                                                                    string ID = "";
                                                                    string strSQL = "SELECT    top(1) ID, DeviceID, CmdStr, Status, Flag, CreateTime, SendTime  FROM         DownCom  " +
                                                                                     " WHERE DeviceID = '" + dr["终端号"] + "' and CmdStr like('7E 01%') and status=1  order by id desc";
                                                                    dtTem = sqlRead(strSQL, ConnStr);
                                                                    for (int i = 0; i < dtTem.Rows.Count; i++)
                                                                    {
                                                                        ID = dtTem.Rows[i]["ID"].ToString();
                                                                    }
                                                                    if (ID.Length > 0)
                                                                    {
                                                                        strSQL = "update DownCom  set Status=2 where ID= " + ID;
                                                                        sqlCmd(strSQL, ConnStr);
                                                                    }
                                                                    try
                                                                    {
                                                                        if (frmControl.Visible)
                                                                        {
                                                                            if (ConnNo == frmControl.lab_DeviceID.Text)
                                                                            {
                                                                                frmControl.P1 = LockStatus(P1);
                                                                                frmControl.P2 = LockStatus(P2);
                                                                                frmControl.P3 = LockStatus(P3);
                                                                                frmControl.P4 = LockStatus(P4);
                                                                                frmControl.P5 = LockStatus(P5);
                                                                                frmControl.P6 = LockStatus(P6);
                                                                                frmControl.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                                            }
                                                                        }
                                                                    }
                                                                    catch
                                                                    { }
                                                                }
                                                                break;
                                                            case 0x0B://下位机刷卡请求协议  （有两种卡一种施封卡另一种解封卡）
                                                                {
                                                                    byte[] CardBT = new byte[8];
                                                                    Array.Copy(dataBody1, 1, CardBT, 0, CardBT.Length);

                                                                    byte[] CardB = new byte[CardBT.Length];
                                                                    for (int i = 0; i < CardB.Length; i++)
                                                                    {
                                                                        CardB[CardB.Length - 1 - i] = CardBT[i];
                                                                    }

                                                                    string cardid = byteToHexStr(CardB);
                                                                    cardid = cardid.Replace(" ", "");
                                                                    EventStr = "刷卡请求" + cardid;
                                                                    EventNo = 6;
                                                                    //setLog(dr["终端号"].ToString(), EventNo, EventStr);
                                                                    dealCard(ConnNo, cardid);
                                                                }
                                                                break;
                                                            case 0x0C://下位机在失去通讯时要上传记录
                                                                {
                                                                    byte[] Tem1 = new byte[1];
                                                                    byte[] Tem2 = new byte[6];
                                                                    byte[] Tem3 = new byte[6];
                                                                    Array.Copy(dataBody1, 0, Tem1, 0, Tem1.Length);
                                                                    Array.Copy(dataBody1, 1, Tem2, 0, Tem2.Length);
                                                                    Array.Copy(dataBody1, 7, Tem3, 0, Tem3.Length);
                                                                    string P1 = Tem2[0].ToString("X2");
                                                                    string P2 = Tem2[1].ToString("X2");
                                                                    string P3 = Tem2[2].ToString("X2");
                                                                    string P4 = Tem2[3].ToString("X2");
                                                                    string P5 = Tem2[4].ToString("X2");
                                                                    string P6 = Tem2[5].ToString("X2");
                                                                    string TimeTem = "20" + Tem3[0].ToString("X2") + "-" + Tem3[1].ToString("X2") + "-" + Tem3[2].ToString("X2") + " " + Tem3[3].ToString("X2") + ":" + Tem3[4].ToString("X2") + ":" + Tem3[5].ToString("X2");
                                                                    //判断是否安装，未安装状态：CC
                                                                    string sql = "SELECT      DeviceID, P1, PNo1, P2, PNo2, P3, PNo3, P4, PNo4, P5, PNo5, P6, PNo6  FROM         CarPorts where DeviceID ='" + dr["终端号"].ToString() + "' ";
                                                                    DataTable dtTem = sqlRead(sql, ConnStr);
                                                                    for (int i = 0; i < dtTem.Rows.Count; i++)
                                                                    {
                                                                        string P1Tem = dtTem.Rows[i]["P1"].ToString();
                                                                        string P2Tem = dtTem.Rows[i]["P2"].ToString();
                                                                        string P3Tem = dtTem.Rows[i]["P3"].ToString();
                                                                        string P4Tem = dtTem.Rows[i]["P4"].ToString();
                                                                        string P5Tem = dtTem.Rows[i]["P5"].ToString();
                                                                        string P6Tem = dtTem.Rows[i]["P6"].ToString();
                                                                        if (P1Tem == "0")
                                                                        {
                                                                            P1 = "CC";
                                                                            Tem2[0] = 0xCC;
                                                                        }
                                                                        if (P2Tem == "0")
                                                                        {
                                                                            P2 = "CC";
                                                                            Tem2[1] = 0xCC;
                                                                        }
                                                                        if (P3Tem == "0")
                                                                        {
                                                                            P3 = "CC";
                                                                            Tem2[2] = 0xCC;
                                                                        }
                                                                        if (P4Tem == "0")
                                                                        {
                                                                            P4 = "CC";
                                                                            Tem2[3] = 0xCC;
                                                                        }
                                                                        if (P5Tem == "0")
                                                                        {
                                                                            P5 = "CC";
                                                                            Tem2[4] = 0xCC;
                                                                        }
                                                                        if (P6Tem == "0")
                                                                        {
                                                                            P6 = "CC";
                                                                            Tem2[5] = 0xCC;
                                                                        }
                                                                    }
                                                                    sql = "Insert into Record(DeviceID, SimNo, CarNo, Speed, lng, lat,Blng,Blat, Direction, Address, DTime, RevTime, RevStr, ComStr,P1,P2,P3,P4,P5,P6,Status)  SELECT     DeviceID, SimNo, CarNo, Speed, Lng, Lat,Blng,Blat, Direction, Address, '" + TimeTem + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + RevStr + "','" + ComRev + "','" + P1 + "','" + P2 + "','" + P3 + "','" + P4 + "','" + P5 + "','" + P6 + "',Status  FROM         Record where ID in (select max(ID) from Record where DTime <= '" + TimeTem + "' and DeviceID ='" + dr["终端号"].ToString() + "')";
                                                                    sqlCmd(sql, ConnStr);

                                                                    try
                                                                    {
                                                                        sql = "Insert into RecordCarPosition( DeviceID, CarNo, Speed, Address, lng, lat,Blng,Blat, DTime,P1,P2,P3,P4,P5,P6, Direction, Status)  SELECT   DeviceID, CarNo, Speed, Address, lng, lat,Blng,Blat, '" + TimeTem + "','" + P1 + "','" + P2 + "','" + P3 + "','" + P4 + "','" + P5 + "','" + P6 + "', Direction, 0 from RecordCarPosition  where ID in (select max(ID) from RecordCarPosition where DTime <= '" + TimeTem + "' and DeviceID ='" + dr["终端号"].ToString() + "')";
                                                                        sqlCmd(sql, ConnStr);
                                                                        setRecord(sql);
                                                                    }
                                                                    catch
                                                                    { }

                                                                    //checkPort(Tem2, ConnNo, index, TimeTem);
                                                                    sql = "SELECT      DeviceID, P1,P2,P3,P4,P5,P6  FROM         RecordCarPosition   where ID in (select max(ID) from RecordCarPosition where DTime < '" + TimeTem + "' and DeviceID ='" + dr["终端号"].ToString() + "')";
                                                                    dtTem = sqlRead(sql, ConnStr);
                                                                    for (int i = 0; i < dtTem.Rows.Count; i++)
                                                                    {
                                                                        string P1Tem = dtTem.Rows[i]["P1"].ToString();
                                                                        string P2Tem = dtTem.Rows[i]["P2"].ToString();
                                                                        string P3Tem = dtTem.Rows[i]["P3"].ToString();
                                                                        string P4Tem = dtTem.Rows[i]["P4"].ToString();
                                                                        string P5Tem = dtTem.Rows[i]["P5"].ToString();
                                                                        string P6Tem = dtTem.Rows[i]["P6"].ToString();
                                                                        if (P1Tem != P1)
                                                                        {
                                                                            sql = "insert into RecordPorts(DeviceID, CarNo, Port, Status, Address, lng, lat,Blng,Blat, DTime) SELECT  DeviceID,CarNo,1,'" + P1 + "',Address,Lng,Lat,Blng,Blat,'" + TimeTem + "'  FROM           RecordCarPosition   where ID in (select max(ID) from RecordCarPosition where DTime <= '" + TimeTem + "' and DeviceID ='" + dr["终端号"].ToString() + "')";
                                                                            sqlCmd(sql, ConnStr);
                                                                        }
                                                                        if (P2Tem != P2)
                                                                        {
                                                                            sql = "insert into RecordPorts(DeviceID, CarNo, Port, Status, Address, lng, lat,Blng,Blat, DTime) SELECT  DeviceID,CarNo,2,'" + P2 + "',Address,Lng,Lat,Blng,Blat,'" + TimeTem + "'  FROM           RecordCarPosition   where ID in (select max(ID) from RecordCarPosition where DTime <= '" + TimeTem + "' and DeviceID ='" + dr["终端号"].ToString() + "')";
                                                                            sqlCmd(sql, ConnStr);
                                                                        }
                                                                        if (P3Tem != P3)
                                                                        {
                                                                            sql = "insert into RecordPorts(DeviceID, CarNo, Port, Status, Address, lng, lat,Blng,Blat, DTime) SELECT  DeviceID,CarNo,3,'" + P3 + "',Address,Lng,Lat,Blng,Blat,'" + TimeTem + "'  FROM           RecordCarPosition   where ID in (select max(ID) from RecordCarPosition where DTime <= '" + TimeTem + "' and DeviceID ='" + dr["终端号"].ToString() + "')";
                                                                            sqlCmd(sql, ConnStr);
                                                                        }
                                                                        if (P4Tem != P4)
                                                                        {
                                                                            sql = "insert into RecordPorts(DeviceID, CarNo, Port, Status, Address, lng, lat,Blng,Blat, DTime) SELECT  DeviceID,CarNo,4,'" + P4 + "',Address,Lng,Lat,Blng,Blat,'" + TimeTem + "'  FROM           RecordCarPosition   where ID in (select max(ID) from RecordCarPosition where DTime <= '" + TimeTem + "' and DeviceID ='" + dr["终端号"].ToString() + "')";
                                                                            sqlCmd(sql, ConnStr);
                                                                        }
                                                                        if (P5Tem != P5)
                                                                        {
                                                                            sql = "insert into RecordPorts(DeviceID, CarNo, Port, Status, Address, lng, lat,Blng,Blat, DTime) SELECT  DeviceID,CarNo,5,'" + P5 + "',Address,Lng,Lat,Blng,Blat,'" + TimeTem + "'  FROM           RecordCarPosition   where ID in (select max(ID) from RecordCarPosition where DTime <= '" + TimeTem + "' and DeviceID ='" + dr["终端号"].ToString() + "')";
                                                                            sqlCmd(sql, ConnStr);
                                                                        }
                                                                        if (P6Tem != P6)
                                                                        {
                                                                            sql = "insert into RecordPorts(DeviceID, CarNo, Port, Status, Address, lng, lat,Blng,Blat, DTime) SELECT  DeviceID,CarNo,6,'" + P6 + "',Address,Lng,Lat,Blng,Blat,'" + TimeTem + "'  FROM           RecordCarPosition   where ID in (select max(ID) from RecordCarPosition where DTime <= '" + TimeTem + "' and DeviceID ='" + dr["终端号"].ToString() + "')";
                                                                            sqlCmd(sql, ConnStr);
                                                                        }
                                                                    }

                                                                    switch (Tem1[0])
                                                                    {
                                                                        case 0x00:
                                                                            {
                                                                                EventStr = "上传记录'应急卡使用开始'" + ComRev;
                                                                                EventNo = 6;
                                                                                //setLog(dr["终端号"].ToString(), EventNo, EventStr);

                                                                                //报警
                                                                                sql = "update car set Status=4  where DeviceID='" + dr["终端号"].ToString() + "'";
                                                                                sqlCmd(sql, ConnStr);

                                                                                //写入报警记录
                                                                                sql = "insert into RecordAlarm (DeviceID,CarNo, EventNo, Detail, Flag,Address,lng,lat,Blng,Blat) SELECT  '" + dr["终端号"].ToString() + "','" + dr["车牌号"].ToString() + "',1,'应急卡使用开始',0 ,Address,Lng,Lat,Blng,Blat  FROM         RecordCarPosition where ID in (select max(ID) from RecordCarPosition where DTime <= '" + TimeTem + "' and DeviceID ='" + dr["终端号"].ToString() + "')";
                                                                                sqlCmd(sql, ConnStr);
                                                                            }
                                                                            break;
                                                                        case 0x01:
                                                                            {
                                                                                EventStr = "上传记录'应急卡使用过程'" + ComRev;
                                                                                EventNo = 6;
                                                                                //setLog(dr["终端号"].ToString(), EventNo, EventStr);

                                                                                //报警
                                                                                sql = "update car set Status=4  where DeviceID='" + dr["终端号"].ToString() + "'";
                                                                                sqlCmd(sql, ConnStr);

                                                                                //写入报警记录
                                                                                sql = "insert into RecordAlarm (DeviceID,CarNo, EventNo, Detail, Flag,Address,lng,lat,Blng,Blat) SELECT  '" + dr["终端号"].ToString() + "','" + dr["车牌号"].ToString() + "',2,'应急卡使用过程',0 ,Address,Lng,Lat,Blng,Blat FROM         RecordCarPosition where ID in (select max(ID) from RecordCarPosition where DTime <= '" + TimeTem + "' and DeviceID ='" + dr["终端号"].ToString() + "')";
                                                                                sqlCmd(sql, ConnStr);
                                                                            }
                                                                            break;
                                                                        case 0x02:
                                                                            {
                                                                                EventStr = "上传记录'应急卡使用结束'" + ComRev;
                                                                                EventNo = 6;
                                                                                //setLog(dr["终端号"].ToString(), EventNo, EventStr);

                                                                                //报警
                                                                                sql = "update car set Status=4  where DeviceID='" + dr["终端号"].ToString() + "'";
                                                                                sqlCmd(sql, ConnStr);

                                                                                //写入报警记录
                                                                                sql = "insert into RecordAlarm (DeviceID,CarNo, EventNo, Detail, Flag,Address,lng,lat,Blng,Blat) SELECT  '" + dr["终端号"].ToString() + "','" + dr["车牌号"].ToString() + "',3,'应急卡使用结束',0 ,Address,Lng,Lat,Blng,Blat  FROM         RecordCarPosition where ID in (select max(ID) from RecordCarPosition where DTime <= '" + TimeTem + "' and DeviceID ='" + dr["终端号"].ToString() + "')";
                                                                                sqlCmd(sql, ConnStr);
                                                                            }
                                                                            break;
                                                                        case 0x03:
                                                                            {
                                                                                EventStr = "上传记录'锁非法开启记录'" + ComRev;
                                                                                EventNo = 6;
                                                                                //setLog(dr["终端号"].ToString(), EventNo, EventStr);

                                                                                //报警
                                                                                sql = "update car set Status=4  where DeviceID='" + dr["终端号"].ToString() + "'";
                                                                                sqlCmd(sql, ConnStr);

                                                                                //写入报警记录
                                                                                sql = "insert into RecordAlarm (DeviceID,CarNo, EventNo, Detail, Flag,Address,lng,lat,Blng,Blat) SELECT  '" + dr["终端号"].ToString() + "','" + dr["车牌号"].ToString() + "',4,'锁非法开启记录',0 ,Address,Lng,Lat,Blng,Blat FROM         RecordCarPosition where ID in (select max(ID) from RecordCarPosition where DTime <= '" + TimeTem + "' and DeviceID ='" + dr["终端号"].ToString() + "')";
                                                                                sqlCmd(sql, ConnStr);
                                                                            }
                                                                            break;
                                                                        default:
                                                                            break;
                                                                    }
                                                                    //接收成功回应
                                                                    byte[] byteTem = comSendRevOk(0x0C);
                                                                    string strTem = byteToHexStr(byteTem);
                                                                    sql = "insert into DownCom  (DeviceID,CmdStr) values ('" + dr["终端号"] + "','" + strTem + "') ";
                                                                    sqlCmd(sql, ConnStr);
                                                                }
                                                                break;
                                                            case 0xAA://下位机接收正确0k返回指令
                                                                {
                                                                    byte cmdUB = dataBody1[1];
                                                                    switch (cmdUB)
                                                                    {
                                                                        case 0x02:
                                                                            {
                                                                                EventStr = "下位机接收‘解施封’正确Ok";
                                                                                EventNo = 6;
                                                                                //setLog(dr["终端号"].ToString(), EventNo, EventStr);

                                                                                string ID = "";
                                                                                string strSQL = "SELECT    top(1) ID, DeviceID, CmdStr, Status, Flag, CreateTime, SendTime  FROM         DownCom  "+
                                                                                                 " WHERE DeviceID = '" + dr["终端号"] + "' and CmdStr like('7E 02%')  order by id desc";
                                                                                DataTable dtTem = sqlRead(strSQL, ConnStr);
                                                                                for (int i = 0; i < dtTem.Rows.Count; i++)
                                                                                {
                                                                                    ID = dtTem.Rows[i]["ID"].ToString();
                                                                                }
                                                                                if (ID.Length > 0)
                                                                                {
                                                                                    strSQL = "update DownCom  set Status=2 where ID= " + ID;
                                                                                    sqlCmd(strSQL, ConnStr);
                                                                                }
                                                                            }
                                                                            break;
                                                                        case 0x03:
                                                                            {
                                                                                EventStr = "下位机接收‘管理卡’正确Ok";
                                                                                EventNo = 6;
                                                                                //setLog(dr["终端号"].ToString(), EventNo, EventStr);
                                                                            }
                                                                            break;
                                                                        case 0x04:
                                                                            {
                                                                                EventStr = "下位机接收‘日期时间’正确Ok";
                                                                                EventNo = 6;
                                                                                //setLog(dr["终端号"].ToString(), EventNo, EventStr);
                                                                            }
                                                                            break;
                                                                        case 0x05:
                                                                            {
                                                                                EventStr = "下位机接收‘复位指令’正确Ok";
                                                                                EventNo = 6;
                                                                                //setLog(dr["终端号"].ToString(), EventNo, EventStr);


                                                                                string ID = "";
                                                                                string strSQL = "SELECT    top(1) ID, DeviceID, CmdStr, Status, Flag, CreateTime, SendTime  FROM         DownCom  " +
                                                                                                 " WHERE DeviceID = '" + dr["终端号"] + "' and CmdStr like('7E 05%')  order by id desc";
                                                                                DataTable dtTem = sqlRead(strSQL, ConnStr);
                                                                                for (int i = 0; i < dtTem.Rows.Count; i++)
                                                                                {
                                                                                    ID = dtTem.Rows[i]["ID"].ToString();
                                                                                }
                                                                                if (ID.Length > 0)
                                                                                {
                                                                                    strSQL = "update DownCom  set Status=2 where ID= " + ID;
                                                                                    sqlCmd(strSQL, ConnStr);
                                                                                }
                                                                            }
                                                                            break;
                                                                        default:
                                                                            break;
                                                                    }
                                                                }
                                                                break;
                                                            case 0xBB://下位机接收错误ERR返回请求重发指令
                                                                {
                                                                    EventStr = "下位机接收错误";
                                                                    EventNo = 6;
                                                                    //setLog(dr["终端号"].ToString(), EventNo, EventStr);

                                                                    string ID = "";
                                                                    string strSQL = "SELECT    top(1) ID, DeviceID, CmdStr, Status, Flag, CreateTime, SendTime  FROM         DownCom  " +
                                                                                     " WHERE DeviceID = '" + dr["终端号"] + "' and status=1  order by id desc";
                                                                    DataTable dtTem = sqlRead(strSQL, ConnStr);
                                                                    for (int i = 0; i < dtTem.Rows.Count; i++)
                                                                    {
                                                                        ID = dtTem.Rows[i]["ID"].ToString();
                                                                    }
                                                                    if (ID.Length > 0)
                                                                    {
                                                                        strSQL = "update DownCom  set Status=3 where ID= " + ID;
                                                                        sqlCmd(strSQL, ConnStr);
                                                                    }
                                                                }
                                                                break;
                                                            default:
                                                                break;
                                                        }
                                                    }
                                                    else//校验失败
                                                    {
                                                        byte[] byteTem = comSendRevErr();
                                                        string strTem = byteToHexStr(byteTem);
                                                        string sql = "insert into DownCom  (DeviceID,CmdStr) values ('" + dr["终端号"] + "','" + strTem + "') ";
                                                        sqlCmd(sql, ConnStr);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (dr != null)
                                    {
                                        dr["透传接收"] = ComRev;
                                        if (ComRev.Length > 33)
                                            dr["油口"] = ComRev.Substring(23, 10);
                                        else
                                            dr["油口"] = "";
                                    }
                                    if (ComRev.Length > 0 || double.Parse(Lng) > 0)
                                    {
                                        if (ComRev.Length > 200)
                                            ComRev = ComRev.Substring(0, 200);
                                        if (RevStr.Length > 500)
                                            RevStr = RevStr.Substring(0, 500);
                                        string sql = "Insert into Record(DeviceID, SimNo, CarNo, Speed, lng, lat,Blng,Blat, Direction, Address, DTime, RevTime, RevStr, ComStr,P1,P2,P3,P4,P5,P6,Status)  SELECT     DeviceID, SimNo, CarNo, Speed, Lng, Lat,Blng,Blat, Direction, Address, DTime, RevTime, '" + RevStr + "','" + ComRev + "',P1,P2,P3,P4,P5,P6,Status  FROM         Car where DeviceID ='" + ConnNo + "'";
                                        sqlCmd(sql, ConnStr);
                                    }


                                }
                                if (dr != null)
                                {
                                    if (dr["连接"].ToString() == "" || dr["连接"].ToString() == "断开")
                                    {
                                        EventNo = 2;
                                        EventStr = "终端连接：" + dr["车牌号"] + " " + socket12.RemoteEndPoint.ToString();

                                        string sql = "Insert into RecordCarOnLine(DeviceID, CarNo, Detail, Address, lng, lat,Blng,Blat)  SELECT   DeviceID, CarNo, '上线', Address, lng, lat,Blng,Blat from Car where  DeviceID='" + ConnNo + "'";
                                        sqlCmd(sql, ConnStr);

                                        sql = "Update RecordAlarm set Flag=1, DealTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',UserID='自动处理' where id in (select max(id) from RecordAlarm where  DeviceID='" + ConnNo + "' and Flag=0 and Detail like '通讯中断报警%') ";
                                        sqlCmd(sql, ConnStr);
                                    }
                                    if (RevStr.Length > 200)
                                        RevStr = RevStr.Substring(0, 200);
                                    dr["原始接收"] = RevStr;
                                    dr["连接"] = socket12.RemoteEndPoint.ToString();
                                }
                            }
                            if (checkBox_GoRevLine.Checked)
                            {
                                if (dr != null)
                                {
                                    for (int j = 0; j < dataGridView_DeviceList.Rows.Count; j++)
                                    {
                                        string tem = dataGridView_DeviceList.Rows[j].Cells["连接"].Value.ToString();
                                        if (dataGridView_DeviceList.Rows[j].Cells["连接"].Value.ToString() == dr["连接"].ToString())
                                        {
                                            dataGridView_DeviceList.ClearSelection();
                                            dataGridView_DeviceList.Rows[j].Selected = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch
                    { }
                }
                if (x <= 0)
                {
                    string atemp = socket12.RemoteEndPoint.ToString();
                    clientList.Remove(socket12.RemoteEndPoint.ToString());
                    socket12.Shutdown(SocketShutdown.Both);
                    socket12.Close();
                    socket12.Dispose();

                    foreach (var item in threadList)
                    {
                        if (item.Key == atemp)
                        {
                            Thread s1 = (Thread)item.Value;
                            s1.Abort();
                            s1.DisableComObjectEagerCleanup();
                        }
                    }
                    threadList.Remove(atemp);

                    /*
                    if (dr != null)
                    {
                        dr["连接"] = "断开";
                        EventNo = 3;
                        EventStr = "终端断开：" + dr["车牌号"] + " " + dr["连接"];
                    }
                     * */
                }
            }
            catch
            {
                try
                {
                    threadList.Remove(socket12.RemoteEndPoint.ToString());
                }
                catch
                { }
                try
                {
                    clientList.Remove(socket12.RemoteEndPoint.ToString());
                }
                catch
                { }
                /*
                if (dr != null)
                {
                    EventNo = 3;
                    EventStr = "终端断开：" + dr["车牌号"] + " " + dr["连接"];
                }
                 * */
            }
                
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
            /*
            //7E 0B 01 F9 2D 90 01 00 00 87 F2 FF 7F
            //7E 0B 01 F0 02 D4 01 00 00 68 78 A1 7F
            string tem = "7E 0B 01 F0 02 D4 01 00 00 68";
            byte[] temb = strToToHexByte(tem);
            for (int i = 0; i < temb.Length; i++)
            {
                TRCRC(temb[i]);
            }
            byte[] CrcTem = BitConverter.GetBytes(CRC_HL);
            byte[] byteNew = new byte[CrcTem.Length];
            for (int i = CrcTem.Length - 1; i >= 0; i--)
            {
                byteNew[CrcTem.Length - i - 1] = CrcTem[i];
            }

            string tem1 = byteToHexStr(byteNew);
            //ushort CRC_HL;
            //void TRCRC(byte a)
             * */
        

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
            byte[] re=new byte[data.Length+2];
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
        public byte[] comSendCard(byte type, string cardid)//com发送管理卡ID指令   卡序号 0=应急卡、1=施封卡、2=解封卡
        {
            byte[] re;
            byte cmdB = 0x03;

            cardid = cardid.Replace(" ", "");
            if (cardid.Length < 16)
                cardid = "0000000000000000" + cardid;
            cardid = cardid.Substring(cardid.Length - 16, 16);
            byte[] cardBT = strToToHexByte(cardid);

            byte[] cardB = new byte[cardBT.Length]; 
            for (int i = 0; i < cardB.Length; i++)
            {
                cardB[cardB.Length - 1 - i] = cardBT[i];
            }

            byte[] dataTem = new byte[cardB.Length + 2];
            dataTem[0] = cmdB;
            dataTem[1] = type;
            Array.Copy(cardB, 0, dataTem, 2, cardB.Length);
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
        public byte[] comSendRevErr()//com上位机接收错误ERR返回请求重发指令
        {
            byte[] re;
            byte cmdB = 0xBB;
            byte[] dataTem = { cmdB };
            re = comSetCrc(dataTem);
            re = comSetHead(re);
            return re;
        }
        public byte[] comSendRevOk(byte cmd)//com上位机接收正确0k返回指令
        {
            byte[] re;
            byte cmdB = 0xAA;
            byte[] dataTem = { cmdB, cmd };
            re = comSetCrc(dataTem);
            re = comSetHead(re);
            return re;
        }



        //发送消息时：消息封装——>计算并填充校验码——>转义；
        //接收消息时：转义还原——>验证校验码——>解析消息；

        ////////////收发处理
        //转义函数，发送中使用
        //0x7e <————> 0x7d 后紧跟一个 0x02；
        //0x7d <————> 0x7d 后紧跟一个 0x01；
        public byte[] Trans(byte[] data)
        {
            byte[] byteTrans = new byte[data.Length];
            int x = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == 0x7e)
                {
                    byte[] temB = new byte[byteTrans.Length];
                    Array.Copy(byteTrans, 0, temB, 0, byteTrans.Length);
                    byteTrans = new byte[byteTrans.Length + 1];
                    Array.Copy(temB, 0, byteTrans, 0, temB.Length);
                    byteTrans[i + x] = 0x7d;
                    x++;
                    byteTrans[i + x] = 0x02;
                }
                else if (data[i] == 0x7d)
                {
                    byte[] temB = new byte[byteTrans.Length];
                    Array.Copy(byteTrans, 0, temB, 0, byteTrans.Length);
                    byteTrans = new byte[byteTrans.Length + 1];
                    Array.Copy(temB, 0, byteTrans, 0, temB.Length);
                    byteTrans[i + x] = data[i];
                    x++;
                    byteTrans[i + x] = 0x01;
                    
                }
                else
                {
                    byteTrans[i + x] = data[i];
                }
            }
            return byteTrans;
        }

        //转义还原函数，接收
        public byte[] TransBack(byte[] data)
        {
            byte[] byteTransBack = new byte[data.Length];
            int x = 0;
            for (int i = 0; i+x < data.Length; i++)
            {
                if (data[i + x] == 0x7d)
                {
                    byte[] temB = byteTransBack;
                    byteTransBack = new byte[byteTransBack.Length - 1];
                    Array.Copy(temB, 0, byteTransBack, 0, byteTransBack.Length);
                    if (data[i + x + 1] == 0x02)
                    {
                        byteTransBack[i] = 0x7e;
                    }
                    if (data[i + x + 1] == 0x01)
                    {
                        byteTransBack[i] = 0x7d;
                    }
                    x++;
                }
                else
                {
                    byteTransBack[i] = data[i + x];
                }
            }
            return byteTransBack;
        }

        //加标识位函数，发送
        public byte[] Pack(byte[] data)
        {
            byte[] bytePack = new byte[data.Length + 2];
            Array.Copy(data, 0, bytePack, 1, data.Length);
            bytePack[0] = 0x7e;
            bytePack[bytePack.Length - 1] = 0x7e;
            return bytePack;
        }

        //去标识位函数，接收
        public byte[] PackBack(byte[] data)
        {
            byte[] bytePackBack = new byte[data.Length - 2];
            Array.Copy(data, 1, bytePackBack, 0, bytePackBack.Length);
            return bytePackBack;
        }

        //校验函数，发送、接收
        public byte Check(byte[] data)
        {
            byte byteCheck = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (i == 0)
                {
                    byteCheck = data[i];
                }
                else
                {
                    byteCheck = (byte)(byteCheck ^ data[i]);
                }
            }
            return byteCheck;
        }

        ////////解析消息头
        //解析“消息ID”
        public string getMsgID(byte[] data)
        {
            string MsgID = "";
            byte[] byteTem = new byte[2];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            MsgID = byteToHexStr(byteTem);
            MsgID = MsgID.Replace(" ", "");
            return MsgID;
        }

        //解析“消息体属性”
        public string getMsgStyle(byte[] data)
        {
            string MsgStyle = "";
            byte[] byteTem = new byte[2];
            Array.Copy(data, 2, byteTem, 0, byteTem.Length);

            for (int i = 0; i < byteTem.Length; i++)
            {
                string tem = Convert.ToString(byteTem[i], 2);//byte转二进制
                tem = "00000000" + tem;
                tem = tem.Substring(tem.Length - 8);
                MsgStyle = MsgStyle + " " + tem;
            }
            return MsgStyle;
        }

        //解析“终端手机号”
        public string getPhone(byte[] data)
        {
            string Phone = "";
            byte[] byteTem = new byte[6];
            Array.Copy(data, 4, byteTem, 0, byteTem.Length);
            Phone = byteToHexStr(byteTem);

            Phone = Phone.Replace(" ", "");
            return Phone;
        }

        //解析“消息流水号”
        public string getMsgNo(byte[] data)
        {
            string MsgNo = "";
            byte[] byteTem = new byte[2];
            Array.Copy(data, 10, byteTem, 0, byteTem.Length);
            string temStr = byteToHexStr(byteTem);
            temStr = temStr.Replace(" ", "");
            int temInt = Convert.ToInt32(temStr, 16);
            MsgNo = temInt.ToString();
            return MsgNo;
        }

        //////解析“消息包封装项”
        //解析“消息包封装项”
        public byte[] getMsgBody(byte[] data)
        {
            byte[] re = null;
            if (data.Length > 12)
            {
                re = new byte[data.Length - 12];
                Array.Copy(data, 12, re, 0, re.Length);
            }
            return re;
        }

        //终端通用应答  消息 ID：0x0001
        public string[] get0001(byte[] data)
        {
            string[] re = new string[3];

            string strTem = "";
            byte[] byteTem = null;

            //应答流水号
            byteTem = new byte[2];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            int intTem = Convert.ToInt32(strTem, 16);
            strTem = intTem.ToString();
            re[0] = strTem;

            //应答ID
            byteTem = new byte[2];
            Array.Copy(data, 2, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[1] = strTem;

            //结果
            byteTem = new byte[1];
            Array.Copy(data, 4, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            intTem = Convert.ToInt32(strTem, 16);
            switch (intTem)//0：成功/确认；1：失败；2：消息有误；3：不支持
            {
                case 0: strTem = "成功/确认"; break;
                case 1: strTem = "失败"; break;
                case 2: strTem = "消息有误"; break;

                default: strTem = "不支持"; break;
            }
            re[2] = strTem;

            return re;
        }
        //终端注册  消息 ID：0x0100
        public string[] get0100(byte[] data)
        {
            string[] re = new string[7];

            string strTem = "";
            byte[] byteTem = null;

            //省域ID
            byteTem = new byte[2];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[0] = strTem;

            //市县域 ID
            byteTem = new byte[2];
            Array.Copy(data, 2, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[1] = strTem;

            //制造商 ID
            byteTem = new byte[5];
            Array.Copy(data, 4, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[2] = strTem;

            //终端型号
            byteTem = new byte[20];
            Array.Copy(data, 9, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[3] = strTem;

            //终端 ID
            byteTem = new byte[7];
            Array.Copy(data, 29, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[4] = strTem;

            //车牌颜色
            byteTem = new byte[1];
            Array.Copy(data, 36, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[5] = strTem;

            //车辆标识
            byteTem = new byte[data.Length - 37];
            Array.Copy(data, 37, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[6] = strTem;

            return re;
        }
        //终端鉴权  消息 ID：0x0102
        public string[] get0102(byte[] data)
        {
            string[] re = new string[1];

            string strTem = "";
            byte[] byteTem = null;

            //鉴权码
            byteTem = new byte[data.Length];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[0] = strTem;

            return re;
        }
        //查询终端参数应答  消息 ID：0x0104
        public string[] get0104(string ConnNo,byte[] data)
        {
            string[] re = new string[2];

            string strTem = "";
            byte[] byteTem = null;

            //应答流水号
            byteTem = new byte[2];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            int intTem = Convert.ToInt32(strTem, 16);
            strTem = intTem.ToString();
            re[0] = strTem;

            //应答参数个数
            byteTem = new byte[1];
            Array.Copy(data, 2, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            intTem = Convert.ToInt32(strTem, 16);
            int n = intTem;
            strTem = intTem.ToString();
            re[1] = strTem;

            string[] temStrs = re;
            re = new string[2 + n];
            Array.Copy(temStrs, 0, re, 0, temStrs.Length);

            //参数项列表
            int xx = 0;
            for (int i = 0; i < n; i++)
            {
                //参数 ID 
                byteTem = new byte[4];
                Array.Copy(data, 3 + xx, byteTem, 0, byteTem.Length);
                xx += 4;
                strTem = byteToHexStr(byteTem);
                re[2 + i] = "参数ID" + strTem;

                string ID = strTem.Replace(" ", "");

                //参数长度
                byteTem = new byte[1];
                Array.Copy(data, 3 + xx, byteTem, 0, byteTem.Length);
                xx += 1;
                strTem = byteToHexStr(byteTem);
                strTem = strTem.Replace(" ", "");
                intTem = Convert.ToInt32(strTem, 16);
                int m = intTem;
                strTem = intTem.ToString();
                re[2 + i] += " " + strTem;
                re[2 + i] += "";

                //参数值
                byteTem = new byte[m];
                Array.Copy(data, 3 + xx, byteTem, 0, byteTem.Length);
                xx += m;
                strTem = byteToHexStr(byteTem);
                re[2 + i] += "值" + strTem;

                byte[] byteValue = byteTem;
                string value = strTem;

                try
                {
                    if (frmGps.Visible)
                    {
                        if (ConnNo == frmGps.lab_DeviceID.Text)
                        {
                            if (ID == "00000013")
                            {
                                frmGps.Ip = Encoding.GetEncoding("GBK").GetString(byteValue);
                            }
                            if (ID == "00000018")
                            {
                                frmGps.Port = (Convert.ToInt32(value, 16)).ToString();
                            }
                            if (ID == "00000029")
                            {
                                frmGps.Space = (Convert.ToInt32(value, 16)).ToString();
                            }
                        }
                    }
                }
                catch
                { }
            }

            /*
            byteTem = new byte[data.Length - 3];
            Array.Copy(data, 3, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[2] = strTem;
             * */

            return re;
        }
        //查询终端属性应答  消息 ID：0x0107
        public string[] get0107(byte[] data)
        {
            string[] re = new string[11];

            string strTem = "";
            byte[] byteTem = null;

            //终端类型
            byteTem = new byte[1];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[0] = "终端类型" + strTem;

            //制造商 ID 
            byteTem = new byte[5];
            Array.Copy(data, 1, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[1] = "制造商ID" + strTem;

            //终端型号
            byteTem = new byte[20];
            Array.Copy(data, 6, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[2] = "终端型号" + strTem;

            //终端 ID
            byteTem = new byte[7];
            Array.Copy(data, 26, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[3] = "终端ID" + strTem;

            //终端 SIM 卡 ICCID 
            byteTem = new byte[10];
            Array.Copy(data, 33, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[4] = "ICCID" + strTem;

            //终端硬件版本号长度 
            byteTem = new byte[1];
            Array.Copy(data, 43, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            int intTem = Convert.ToInt32(strTem, 16);
            int n = intTem;
            strTem = intTem.ToString();
            re[5] = strTem;
            re[5] = "";

            //终端硬件版本号 
            byteTem = new byte[n];
            Array.Copy(data, 44, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[6] = "终端硬件版本号" + strTem;

            //终端固件版本号长度 
            byteTem = new byte[1];
            Array.Copy(data, 44 + n, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            intTem = Convert.ToInt32(strTem, 16);
            int m = intTem;
            strTem = intTem.ToString();
            re[7] = strTem;
            re[7] = "";

            //终端固件版本号 
            byteTem = new byte[m];
            Array.Copy(data, 45 + n, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[8] = "终端固件版本号" + strTem;

            //GNSS 模块属性 
            byteTem = new byte[1];
            Array.Copy(data, 45 + n + m, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[9] = "GNSS模块属性" + strTem;

            //通信模块属性
            byteTem = new byte[1];
            Array.Copy(data, 46 + n + m, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[10] = "通信模块属性" + strTem;

            return re;
        }
        //终端升级结果通知  消息 ID：0x0108
        public string[] get0108(byte[] data)
        {
            string[] re = new string[2];

            string strTem = "";
            byte[] byteTem = null;

            //升级类型
            byteTem = new byte[1];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[0] = strTem;

            //升级结果
            byteTem = new byte[1];
            Array.Copy(data, 1, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[1] = strTem;

            return re;
        }
        //位置信息汇报  消息 ID：0x0200
        public string[] get0200(string connNo,byte[] data)
        {
            string[] re1 = new string[8];

            string strTem = "";
            byte[] byteTem = null;

            //报警标志
            byteTem = new byte[4];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = "";
            for (int i = 0; i < byteTem.Length; i++)
            {
                string tem = Convert.ToString(byteTem[i], 2);
                tem = "00000000" + tem;
                tem = tem.Substring(tem.Length - 8);
                strTem = strTem + " " + tem;
            }
            re1[0] = strTem;

            //状态
            byteTem = new byte[4];
            Array.Copy(data, 4, byteTem, 0, byteTem.Length);
            strTem = "";
            for (int i = 0; i < byteTem.Length; i++)
            {
                string tem = Convert.ToString(byteTem[i], 2);
                tem = "00000000" + tem;
                tem = tem.Substring(tem.Length - 8);
                strTem = strTem + " " + tem;
            }
            re1[1] = strTem;

            //纬度
            byteTem = new byte[4];
            Array.Copy(data, 8, byteTem, 0, byteTem.Length);
            strTem = "";
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            strTem = ((double)(Convert.ToInt32(strTem.Replace(" ", ""), 16)) / 1000000).ToString("0.000000");//十六进制转十进制
            re1[2] = strTem;

            //经度
            byteTem = new byte[4];
            Array.Copy(data, 12, byteTem, 0, byteTem.Length);
            strTem = "";
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            strTem = ((double)(Convert.ToInt32(strTem.Replace(" ", ""), 16)) / 1000000).ToString("0.000000");//十六进制转十进制
            re1[3] = strTem;

            //高程
            byteTem = new byte[2];
            Array.Copy(data, 16, byteTem, 0, byteTem.Length);
            strTem = "";
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            strTem = Convert.ToInt32(strTem.Replace(" ", ""), 16).ToString();//十六进制转十进制
            re1[4] = strTem;

            //速度
            byteTem = new byte[2];
            Array.Copy(data, 18, byteTem, 0, byteTem.Length);
            strTem = "";
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            strTem = ((double)(Convert.ToInt32(strTem.Replace(" ", ""), 16)) / 10).ToString("0.0");//十六进制转十进制
            re1[5] = strTem;

            //方向
            byteTem = new byte[2];
            Array.Copy(data, 20, byteTem, 0, byteTem.Length);
            strTem = "";
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            strTem = Convert.ToInt32(strTem.Replace(" ", ""), 16).ToString();//十六进制转十进制
            re1[6] = strTem;

            //时间
            byteTem = new byte[6];
            Array.Copy(data, 22, byteTem, 0, byteTem.Length);
            strTem = "";
            strTem = byteToHexStr(byteTem);
            string[] TemS = strTem.Split(' ');
            strTem = "20";
            for (int i = 0; i < TemS.Length - 1; i++)
            {
                if (i == 0)
                {
                    strTem += TemS[i];
                }
                else if (i < 3)
                {
                    strTem += "-" + TemS[i];
                }
                else if (i == 3)
                {
                    strTem += " " + TemS[i];
                }
                else if (i > 3)
                {
                    strTem += ":" + TemS[i];
                }
            }
            re1[7] = strTem;

            string[] re = re1;

            string[] re2 = null;

            int xx = 0;
            while (data.Length > 28 + xx)
            {
                re2 = new string[3];
                strTem = "";
                byteTem = null;

                //附加信息 ID
                byteTem = new byte[1];
                Array.Copy(data, 28 + xx, byteTem, 0, byteTem.Length);
                strTem = byteToHexStr(byteTem);
                strTem = strTem.Replace(" ", "");
                switch (byteTem[0])
                {
                    case 0x01:
                        {
                            strTem += "," + "里程";
                        } break;
                    case 0x02:
                        {
                            strTem += "," + "油量";
                        } break;
                    case 0x03:
                        {
                            strTem += "," + "行驶记录功能获取的速度";
                        } break;
                    case 0x04:
                        {
                            strTem += "," + "需要人工确认报警事件的 ID";
                        } break;

                    case 0x11:
                        {
                            strTem += "," + "超速报警";
                        } break;
                    case 0x12:
                        {
                            strTem += ":" + "进出区域/路线报警";
                        } break;
                    case 0x13:
                        {
                            strTem += "," + "路段行驶时间不足/过长报警";
                        } break;

                    case 0x25:
                        {
                            strTem += "," + "扩展车辆信号状态位";
                        } break;
                    case 0x2A:
                        {
                            strTem += "," + "IO 状态位";
                        } break;
                    case 0x2B:
                        {
                            strTem += "," + "模拟量";
                        } break;
                    case 0x30:
                        {
                            strTem += "," + "无线通信网络信号强度";
                        } break;
                    case 0x31:
                        {
                            strTem += "," + "GNSS 定位卫星数";
                        } break;
                    case 0xE0:
                        {
                            strTem += "," + "后续自定义信息长度";
                        } break;

                    default: strTem += "," + "自定义区域/保留"; break;
                }
                re2[0] = strTem;

                //附加信息长度
                byteTem = new byte[1];
                Array.Copy(data, 29 + xx, byteTem, 0, byteTem.Length);
                strTem = byteToHexStr(byteTem);
                strTem = strTem.Replace(" ", "");
                int intTem = Convert.ToInt32(strTem, 16);
                int n = intTem;
                strTem = "长度" + intTem.ToString();
                re2[1] = strTem;
                re2[1] = "";

                //附加信息
                byteTem = new byte[n];
                if (30 + xx + n <= data.Length)
                {
                    Array.Copy(data, 30 + xx, byteTem, 0, byteTem.Length);
                }
                else
                {
                    Array.Copy(data, 30 + xx, byteTem, 0, data.Length - 30 - xx);
                }
                strTem = byteToHexStr(byteTem);
                re2[2] = strTem;

                // 进出区域处理！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
                if (re2[0].IndexOf("进出区域") >= 0)
                {
                    string AreaType = "";
                    string AreaID = "";
                    string AreaFlag = "";
                    switch (byteTem[0])
                    {
                        case 0x01:
                            {
                                AreaType="圆形";
                            } break;
                        case 0x02:
                            {
                                AreaType = "矩形";
                            } break;
                        case 0x03:
                            {
                                AreaType = "多边形";
                            } break;
                        case 0x04:
                            {
                                AreaType = "路线";
                            } break;
                        default: break;
                    }
                    byte[] byteAreaID = new byte[4];
                    Array.Copy(byteTem, 1, byteAreaID, 0, byteAreaID.Length);
                    AreaID = byteToHexStr(byteAreaID);
                    AreaID=AreaID.Replace(" ","");

                    switch (byteTem[5])
                    {
                        case 0x00:
                            {
                                AreaFlag = "进";
                                int areaTem = int.Parse(AreaID);
                                string sql = "update Car set  siteIDNow='" + areaTem.ToString() + "' where DeviceID = '" + connNo + "'";
                                sqlCmd(sql, ConnStr);
                            } break;
                        case 0x01:
                            {
                                AreaFlag = "出";
                                string sql = "update Car set  siteIDNow='' where DeviceID = '" + connNo + "'";
                                sqlCmd(sql, ConnStr);
                            } break;
                        default: break;
                    }
                    EventStr=AreaType + "-" + AreaID + "-" + AreaFlag;
                    EventNo=6;
                }

                string[] temStrs = re;
                re = new string[temStrs.Length + re2.Length];
                Array.Copy(temStrs, 0, re, 0, temStrs.Length);
                Array.Copy(re2, 0, re, temStrs.Length, re2.Length);

                xx += 2 + n;
            }

            return re;
        }
        //位置信息查询应答  消息 ID：0x0201
        public string[] get0201(string connNo, byte[] data)
        {
            string[] re = new string[1];

            string strTem = "";
            byte[] byteTem = null;

            //应答流水号
            byteTem = new byte[2];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            int intTem = Convert.ToInt32(strTem, 16);
            strTem = intTem.ToString();
            re[0] = strTem;

            //位置信息汇报
            byteTem = new byte[data.Length - 2];
            Array.Copy(data, 2, byteTem, 0, byteTem.Length);
            string[] strS = get0200(connNo, byteTem);

            string[] temStrs = re;
            re = new string[temStrs.Length + strS.Length];
            Array.Copy(temStrs, 0, re, 0, temStrs.Length);

            for (int i = 0; i < strS.Length; i++)
            {
                re[i + 1] = strS[i];
            }

            return re;
        }

        //事件报告  消息 ID：0x0301
        public string[] get0301(byte[] data)
        {
            string[] re = new string[1];

            string strTem = "";
            byte[] byteTem = null;

            //事件 ID
            byteTem = new byte[1];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[0] = strTem;

            return re;
        }

        //提问应答  消息 ID：0x0302
        public string[] get0302(byte[] data)
        {
            string[] re = new string[2];

            string strTem = "";
            byte[] byteTem = null;

            //应答流水号
            byteTem = new byte[2];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            int intTem = Convert.ToInt32(strTem, 16);
            strTem = intTem.ToString();
            re[0] = strTem;

            //答案 ID
            byteTem = new byte[1];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[1] = strTem;

            return re;
        }

        //信息点播/取消  消息 ID：0x0303
        public string[] get0303(byte[] data)
        {
            string[] re = new string[2];

            string strTem = "";
            byte[] byteTem = null;

            //信息类型
            byteTem = new byte[1];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[0] = strTem;

            //点播/取消标志
            byteTem = new byte[1];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[1] = strTem;

            return re;
        }

        //车辆控制应答  消息 ID：0x0500
        public string[] get0500(string connNo, byte[] data)
        {
            string[] re = new string[9];

            string strTem = "";
            byte[] byteTem = null;

            //应答流水号
            byteTem = new byte[2];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            int intTem = Convert.ToInt32(strTem, 16);
            strTem = intTem.ToString();
            re[0] = strTem;

            //位置信息汇报
            byteTem = new byte[data.Length - 2];
            Array.Copy(data, 2, byteTem, 0, byteTem.Length);
            string[] strS = get0200(connNo, byteTem);

            string[] temStrs = re;
            re = new string[temStrs.Length + strS.Length];
            Array.Copy(temStrs, 0, re, 0, temStrs.Length);

            for (int i = 0; i < strS.Length; i++)
            {
                re[i + 1] = strS[i];
            }

            return re;
        }

        //行驶记录数据上传  消息 ID：0x0700
        public string[] get0700(byte[] data)
        {
            string[] re = new string[3];

            string strTem = "";
            byte[] byteTem = null;

            //应答流水号
            byteTem = new byte[2];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            int intTem = Convert.ToInt32(strTem, 16);
            strTem = intTem.ToString();
            strTem = "流水号" + intTem;
            re[0] = strTem;

            //命令字
            byteTem = new byte[1];
            Array.Copy(data, 2, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);

            intTem = byteTem[0];
            //re[1] = cmb_8700.Items[intTem - 1].ToString();
            re[1] = strTem;

            //数据块
            int len = data.Length - 3;
            byteTem = new byte[len];
            Array.Copy(data, 3, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[2] = strTem;

            return re;
        }
        //电子运单上报  消息 ID：0x0701
        public string[] get0701(byte[] data)
        {
            string[] re = new string[2];

            string strTem = "";
            byte[] byteTem = null;

            //电子运单长度
            byteTem = new byte[2];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            int intTem = Convert.ToInt32(strTem, 16);
            strTem = intTem.ToString();
            re[0] = strTem;
            re[0] = "";

            //电子运单内容
            byteTem = new byte[intTem];
            Array.Copy(data, 2, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[1] = strTem;

            return re;
        }

        //驾驶员身份信息采集上报  消息 ID：0x0702
        public string[] get0702(byte[] data)
        {
            string[] re = new string[3];

            string strTem = "";
            byte[] byteTem = null;

            //状态
            byteTem = new byte[1];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            switch (byteTem[0])
            {
                case 0x01: strTem += "从业资格证IC卡插入"; break;
                case 0x02: strTem += "从业资格证IC卡拔出"; break;
                default: strTem += "…"; break;
            }
            re[0] = strTem;

            //时间
            byteTem = new byte[6];
            Array.Copy(data, 1, byteTem, 0, byteTem.Length);
            strTem = "";
            strTem = byteToHexStr(byteTem);
            string[] TemS = strTem.Split(' ');
            strTem = "20";
            for (int i = 0; i < TemS.Length - 1; i++)
            {
                if (i == 0)
                {
                    strTem += TemS[i];
                }
                else if (i < 3)
                {
                    strTem += "-" + TemS[i];
                }
                else if (i == 3)
                {
                    strTem += " " + TemS[i];
                }
                else if (i > 3)
                {
                    strTem += ":" + TemS[i];
                }
            }
            re[1] = strTem;


            //IC 卡读取结果  
            /*
            0x00：IC 卡读卡成功；
            0x01：读卡失败，原因为卡片密钥认证未通过；
            0x02：读卡失败，原因为卡片已被锁定；
            0x03：读卡失败，原因为卡片被拔出；
            0x04：读卡失败，原因为数据校验错误
             * */
            byteTem = new byte[1];
            Array.Copy(data, 7, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            switch (byteTem[0])
            {
                case 0x00: strTem += "IC卡读卡成功"; break;
                case 0x01: strTem += "读卡失败,原因为卡片密钥认证未通过"; break;
                case 0x02: strTem += "读卡失败,原因为卡片已被锁定"; break;
                case 0x03: strTem += "读卡失败,原因为卡片被拔出"; break;
                case 0x04: strTem += "读卡失败,原因为数据校验错误"; break;
                default: strTem += "读卡失败…"; break;
            }
            re[2] = strTem;

            if (byteTem[0] == 0x00)
            {
                string[] temStrs = re;
                re = new string[9];
                Array.Copy(temStrs, 0, re, 0, temStrs.Length);

                //驾驶员姓名长度
                byteTem = new byte[1];
                Array.Copy(data, 8, byteTem, 0, byteTem.Length);
                strTem = byteToHexStr(byteTem);
                strTem = strTem.Replace(" ", "");
                int intTem = Convert.ToInt32(strTem, 16);
                int n = intTem;
                strTem = intTem.ToString();
                re[3] = strTem;
                re[3] = "";

                //驾驶员姓名
                byteTem = new byte[n];
                Array.Copy(data, 9, byteTem, 0, byteTem.Length);
                strTem = byteToHexStr(byteTem);
                re[4] = strTem;

                //从业资格证编码
                byteTem = new byte[20];
                Array.Copy(data, 9 + n, byteTem, 0, byteTem.Length);
                strTem = byteToHexStr(byteTem);
                re[5] = strTem;

                //发证机构名称长度
                byteTem = new byte[1];
                Array.Copy(data, 29 + n, byteTem, 0, byteTem.Length);
                strTem = byteToHexStr(byteTem);
                strTem = strTem.Replace(" ", "");
                intTem = Convert.ToInt32(strTem, 16);
                int m = intTem;
                strTem = intTem.ToString();
                re[6] = strTem;
                re[6] = "";

                //发证机构名称
                byteTem = new byte[m];
                Array.Copy(data, 30 + n, byteTem, 0, byteTem.Length);
                strTem = byteToHexStr(byteTem);
                re[7] = strTem;

                //证件有效期
                byteTem = new byte[4];
                Array.Copy(data, 30 + n + m, byteTem, 0, byteTem.Length);
                strTem = byteToHexStr(byteTem);
                re[8] = strTem;
            }

            return re;
        }

        //定位数据批量上传  消息 ID：0x0704
        public string[] get0704(byte[] data)
        {
            string[] re = new string[3];

            string strTem = "";
            byte[] byteTem = null;

            //数据项个数
            byteTem = new byte[1];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            int intTem = Convert.ToInt32(strTem, 16);
            strTem = intTem.ToString();
            re[0] = strTem;

            //位置数据类型
            byteTem = new byte[1];
            Array.Copy(data, 1, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[1] = strTem;

            //位置汇报数据项 ??????????????????????????
            byteTem = new byte[data.Length - 2];
            Array.Copy(data, 2, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[2] = strTem;

            return re;
        }

        //CAN 总线数据上传  消息 ID：0x0705
        public string[] get0705(byte[] data)
        {
            string[] re = new string[3];

            string strTem = "";
            byte[] byteTem = null;

            //数据项个数
            byteTem = new byte[2];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            int intTem = Convert.ToInt32(strTem, 16);
            strTem = intTem.ToString();
            re[0] = strTem;

            //CAN 总线数据接收时间
            byteTem = new byte[5];
            Array.Copy(data, 2, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[1] = strTem;

            //CAN 总线数据项  ??????????????????????????
            byteTem = new byte[data.Length - 8];
            Array.Copy(data, 8, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[2] = strTem;

            return re;
        }

        //多媒体事件信息上传  消息 ID：0x0800
        public string[] get0800(byte[] data)
        {
            string[] re = new string[5];

            string strTem = "";
            byte[] byteTem = null;

            //多媒体数据 ID 
            byteTem = new byte[4];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[0] = strTem;

            //多媒体类型
            byteTem = new byte[1];
            Array.Copy(data, 4, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[1] = strTem;

            //多媒体格式编码
            byteTem = new byte[1];
            Array.Copy(data, 5, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[2] = strTem;

            //事件项编码
            byteTem = new byte[1];
            Array.Copy(data, 6, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[3] = strTem;

            //通道 ID 
            byteTem = new byte[1];
            Array.Copy(data, 7, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[4] = strTem;

            return re;
        }

        //多媒体数据上传  消息 ID：0x0801
        public string[] get0801(string connNo, byte[] data)
        {
            string[] re = new string[7];

            string strTem = "";
            byte[] byteTem = null;

            //多媒体 ID  
            byteTem = new byte[4];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[0] = strTem;

            //多媒体类型
            byteTem = new byte[1];
            Array.Copy(data, 4, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[1] = strTem;

            //多媒体格式编码
            byteTem = new byte[1];
            Array.Copy(data, 5, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[2] = strTem;

            //事件项编码
            byteTem = new byte[1];
            Array.Copy(data, 6, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[3] = strTem;

            //通道 ID 
            byteTem = new byte[1];
            Array.Copy(data, 7, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[4] = strTem;

            //位置信息汇报
            byteTem = new byte[data.Length - 8];
            Array.Copy(data, 8, byteTem, 0, byteTem.Length);
            string[] strS = get0200(connNo, byteTem);

            string[] temStrs = re;
            re = new string[temStrs.Length + strS.Length];
            Array.Copy(temStrs, 0, re, 0, temStrs.Length); ;

            for (int i = 0; i < strS.Length; i++)
            {
                re[i + 5] = strS[i];
            }

            //多媒体数据包
            byteTem = new byte[data.Length - 36];
            Array.Copy(data, 36, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[strS.Length + 5] = strTem;

            return re;
        }

        //摄像头立即拍摄命令应答  消息 ID：0x0805
        public string[] get0805(byte[] data)
        {
            string[] re = new string[4];

            string strTem = "";
            byte[] byteTem = null;

            //应答流水号
            byteTem = new byte[2];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            int intTem = Convert.ToInt32(strTem, 16);
            strTem = intTem.ToString();
            re[0] = strTem;

            //结果
            byteTem = new byte[1];
            Array.Copy(data, 2, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[1] = strTem;

            //多媒体 ID 个数
            byteTem = new byte[1];
            Array.Copy(data, 3, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            intTem = Convert.ToInt32(strTem, 16);
            strTem = intTem.ToString();
            re[2] = strTem;

            //多媒体 ID 列表  ???????????????????????????
            byteTem = new byte[data.Length - 4];
            Array.Copy(data, 4, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[3] = strTem;

            return re;
        }

        //存储多媒体数据检索应答  消息 ID：0x0802
        public string[] get0802(byte[] data)
        {
            string[] re = new string[3];

            string strTem = "";
            byte[] byteTem = null;

            //应答流水号
            byteTem = new byte[2];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            int intTem = Convert.ToInt32(strTem, 16);
            strTem = intTem.ToString();
            re[0] = strTem;

            //多媒体数据总项数
            byteTem = new byte[2];
            Array.Copy(data, 2, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            intTem = Convert.ToInt32(strTem, 16);
            strTem = intTem.ToString();
            re[1] = strTem;

            //检索项  ???????????????????????????
            byteTem = new byte[data.Length - 4];
            Array.Copy(data, 4, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[2] = strTem;

            return re;
        }

        //数据上行透传  消息 ID：0x0900
        public string[] get0900(byte[] data)
        {
            string[] re = new string[2];

            string strTem = "";
            byte[] byteTem = null;

            //透传消息类型
            byteTem = new byte[1];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[0] = strTem;

            //透传消息内容
            byteTem = new byte[data.Length - 1];
            Array.Copy(data, 1, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[1] = strTem;

            return re;
        }

        //数据压缩上报  消息 ID：0x0901
        public string[] get0901(byte[] data)
        {
            string[] re = new string[2];

            string strTem = "";
            byte[] byteTem = null;

            //压缩消息长度
            byteTem = new byte[4];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            strTem = strTem.Replace(" ", "");
            int intTem = Convert.ToInt32(strTem, 16);
            strTem = intTem.ToString();
            re[0] = strTem;
            re[0] = "";

            //压缩消息体  ??????????????????????????????
            byteTem = new byte[data.Length - 4];
            Array.Copy(data, 4, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[1] = strTem;

            return re;
        }
        //终端 RSA 公钥  消息 ID：0x0A00
        public string[] get0A00(byte[] data)
        {
            string[] re = new string[2];

            string strTem = "";
            byte[] byteTem = null;

            //e
            byteTem = new byte[4];
            Array.Copy(data, 0, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[0] = strTem;

            //n
            byteTem = new byte[128];
            Array.Copy(data, 4, byteTem, 0, byteTem.Length);
            strTem = byteToHexStr(byteTem);
            re[1] = strTem;

            return re;
        }


        public void stopAll()
        {
            foreach (var item in clientList)
            {
                Socket s1 = (Socket)item.Value;
                s1.Shutdown(SocketShutdown.Both);
                s1.Close();
                s1.Dispose();

            }
            foreach (var item in threadList)
            {
                Thread s1 = (Thread)item.Value;
                s1.Abort();
                s1.DisableComObjectEagerCleanup();

            }
            clientList.Clear();
            threadList.Clear();

            if (newSocket != null)
            {
                newSocket.Dispose();
                newSocket = null;
            }
            if (newSocket1 != null)
            {
                newSocket1.Dispose();
                newSocket1 = null;
            }
            if (socket != null)
            {
                socket.Dispose();
                socket = null;
            }
            if (mythread != null)
            {
                mythread.Abort();
                mythread.DisableComObjectEagerCleanup();
            }
            
            if (storethread != null)
            {
                storethread.Abort();
                storethread.DisableComObjectEagerCleanup();
            }
            if (threadCars != null)
            {
                threadCars.Abort();
                threadCars.DisableComObjectEagerCleanup();
            }
            toolStripStatusLabel1.Text = "已停止";
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

        private void btn_Log_Click(object sender, EventArgs e)
        {
            reLoad();

            //checkCarArea("0010011");//通过百度地图确定车辆是否在要配送的站点内
            //dealCard("015553172907", "01902DF9");

            string tem = DateTime.Now.ToString("[yyyy_MM]..[dd]");
            string adf = tem;
        }
        private void Translate()
        {
            try
            {
                {
                    try
                    {
                        string[] re=null;
                        string sql = "select top(20) DeviceID,lng,lat from Car where BDFlag = 0 ";
                        DataTable dtTem = sqlRead(sql, ConnStr);
                        if (dtTem.Rows.Count > 0)
                            re = new string[2];
                        for (int i = 0; i < dtTem.Rows.Count; i++)
                        {
                            if (dtTem.Rows[i]["lng"].ToString().Length > 0)
                            {
                                if (dtTem.Rows[i]["lat"].ToString().Length > 0)
                                {
                                    string DeviceID = dtTem.Rows[i]["DeviceID"].ToString();
                                    string lng = dtTem.Rows[i]["lng"].ToString();
                                    string lat = dtTem.Rows[i]["lat"].ToString();
                                    if (lng.Length > 0)
                                    {
                                        re[0] += DeviceID + ",";
                                        re[1] += lng + "," + lat + ";";
                                    }
                                }
                            }
                        }
                        if (re != null)
                        {
                            if (re[0].Length > 0)
                            {
                                re[0] = re[0].Substring(0, re[0].LastIndexOf(","));
                                re[1] = re[1].Substring(0, re[1].LastIndexOf(";"));
                                getCarBlnglats(re);
                            }
                        }
                    }
                    catch
                    { }
                }
                {
                    string sql = "select DeviceID,Blng,Blat from Car where BDFlag1 = 0 ";
                    DataTable dtTem = sqlRead(sql, ConnStr);
                    for (int i = 0; i < dtTem.Rows.Count; i++)
                    {
                        if (dtTem.Rows[i]["Blng"].ToString().Length > 0)
                        {
                            if (i == 0)
                            {
                                if (dtTem.Rows[i]["Blat"].ToString().Length > 0)
                                {
                                    string DeviceID = dtTem.Rows[i]["DeviceID"].ToString();
                                    string lng = dtTem.Rows[i]["Blng"].ToString();
                                    string lat = dtTem.Rows[i]["Blat"].ToString();
                                    if (lng.Length > 0)
                                    {
                                        getAddr(DeviceID, lng, lat);
                                        //Thread.Sleep(300);
                                        checkCarArea(DeviceID);//通过百度地图确定车辆是否在要配送的站点内
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
                {
                    string sql = "SELECT     PointID, BLng, BLat  FROM         Points where  Lng is null or Lat is null";
                    DataTable dtTem = sqlRead(sql, ConnStr);
                    for (int i = 0; i < dtTem.Rows.Count; i++)
                    {
                        if (dtTem.Rows[i]["BLng"].ToString().Length > 0)
                        {
                            if (i == 0)
                            {
                                if (dtTem.Rows[i]["BLat"].ToString().Length > 0)
                                {
                                    string PointID = dtTem.Rows[i]["PointID"].ToString();
                                    string BLng = dtTem.Rows[i]["BLng"].ToString();
                                    string BLat = dtTem.Rows[i]["BLat"].ToString();
                                    if (BLng.Length > 0)
                                    {
                                        getGpsP(PointID, BLng, BLat);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                try
                {
                    timerTranslate.Enabled = false;

                    webBrowser1.ObjectForScripting = this;//具体公开的对象,这里可以公开自定义对象
                    webBrowser1.ScriptErrorsSuppressed = true; //禁用错误脚本提示
                    webBrowser1.IsWebBrowserContextMenuEnabled = false; //禁用右键菜单
                    webBrowser1.WebBrowserShortcutsEnabled = false; //禁用快捷键
                    string urlPath = Path.GetFullPath(PathStr + "/baidu.html").Replace(Path.DirectorySeparatorChar, '/');
                    Uri url = new Uri(urlPath);//本来str_url是个地址
                    webBrowser1.Url = url;
                }
                catch
                { }
            }
        }


        public void getGpsP(string id, string BLng, string BLat)
        {
            object[] objects = new object[3];
            objects[0] = id;
            objects[1] = BLng;
            objects[2] = BLat;
            webBrowser1.Document.InvokeScript("getGpsPoint", objects);
        }

        public void backGpsP(string id, double lng, double lat)
        {
            if (lng.ToString().Length > 0)
            {
                string sql = "update Points set lat=" + lat + ",lng=" + lng + " where PointID=" + id;
                sqlCmd(sql, ConnStr);
            }
        }

        public void getCarBlnglats(string[] strs)
        {
            if (strs.Length > 0)
            {
                object[] objects = (object[])strs;
                webBrowser1.Document.InvokeScript("getBlnglats", objects);
            }
        }

        public void backBlnglats(string ids, string lnglats)
        {
            try
            {
                string[] deviceids = ids.Split(',');
                string[] strTems = lnglats.Split(';');
                for (int i = 0; i < deviceids.Length; i++)
                {
                    if (deviceids[i].Length > 0)
                    {
                        string[] strTems1 = strTems[i].Split(',');
                        string sql = "update Car set Blng=" + strTems1[0] + ",Blat=" + strTems1[1] + ",BDFlag=1,BDFlag1=0  where deviceid='" + deviceids[i] + "'";
                        sqlCmd(sql, ConnStr);
                    }
                }
            }
            catch
            { }
        }

        public void getAddr(string id,string lng,string lat)
        {
            object[] objects = new object[3];
            objects[0] = id;
            objects[1] = lng;
            objects[2] = lat;
            webBrowser1.Document.InvokeScript("getaddress", objects);
        }
  
        public void backAddr(string deviceid,string address,double lng,double lat) {
            if (address.Length > 0)
            {
                address=address.Replace(" ", "");
                if (address.Length == 0)
                    address = "-";
                string sql = "update Car set address='" + address + "',Blat=" + lat + ",Blng=" + lng + ",BDFlag1=1 where DeviceID='" + deviceid + "'";
                sqlCmd(sql, ConnStr);
                //checkCarArea(deviceid);//通过百度地图确定车辆是否在要配送的站点内
            }
        }

        public void getIni()
        {
            string iniPath = PathStr + @"\Config.ini";
            if (File.Exists(iniPath))
            {
                ClsIni1.clsIni(iniPath);
                basic.Server = ClsIni1.IniReadValue("Basic", "Server");
                basic.Database = ClsIni1.IniReadValue("Basic", "Database");
                basic.User = ClsIni1.IniReadValue("Basic", "User");
                basic.Password = ClsIni1.IniReadValue("Basic", "Password");

                ConnStr = "Data Source=" + basic.Server + ";Initial Catalog=" + basic.Database + ";uid=" + basic.User + ";pwd=" + basic.Password;
            }
        }

        public bool sqlCmd(string sql, string conStr)
        {
            try
            {
                SqlConnection conn = new SqlConnection(conStr);
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public DataTable sqlRead(string sql, string conStr)
        {
            DataTable dtTem = null;
            try
            {
                dtTem = new DataTable();
                SqlConnection conn = new SqlConnection(conStr);
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                conn.Open();
                da.Fill(dtTem);
                da.Dispose();
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            catch
            { }
            return dtTem;
        }

        private void treeView_All_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag.ToString() == "1")
            {
                for (int i = 0; i < dataGridView_DeviceList.Rows.Count; i++)
                {
                    if (dataGridView_DeviceList.Rows[i].Cells["终端号"].Value.ToString() == e.Node.Name)
                    {
                        dataGridView_DeviceList.ClearSelection();
                        dataGridView_DeviceList.Rows[i].Selected=true;
                        break;
                    }
                }
            }
        }

        private void treeView_All_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag.ToString() == "1")
            {
                for (int i = 0; i < dataGridView_DeviceList.Rows.Count; i++)
                {
                    if (dataGridView_DeviceList.Rows[i].Cells["终端号"].Value.ToString() == e.Node.Name)
                    {
                        lab_CarNo.Text = dataGridView_DeviceList.Rows[i].Cells["车牌号"].Value.ToString();
                        lab_DeviceID.Text = dataGridView_DeviceList.Rows[i].Cells["终端号"].Value.ToString();
                        lab_Phone.Text = dataGridView_DeviceList.Rows[i].Cells["Sim卡号"].Value.ToString();
                        break;
                    }
                }
            }
        }

        private void dataGridView_DeviceList_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            if (dataGridView_DeviceList.CurrentRow != null)
            {
                string DeviceID = dataGridView_DeviceList.CurrentRow.Cells["终端号"].Value.ToString();

                TreeNode NdTem = null;
                for (int j = 0; j < treeView_All.Nodes.Count; j++)
                {
                    NdTem = FindNode(treeView_All.Nodes[j], DeviceID, "1");
                    if (NdTem != null)
                        break;
                }
                if (NdTem != null)
                {
                    treeView_All.SelectedNode = NdTem;
                    treeView_All.Select();
                }
            }
             
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < lose.Length; i++)
                {
                    string add = dtD.Rows[i]["连接"].ToString();
                    if (lose[i] < loseTime && dtD.Rows[i]["连接"].ToString() != "断开")
                        lose[i]++;
                    if (lose[i] >= loseTime && dtD.Rows[i]["连接"].ToString() != "断开")
                    {
                        dtD.Rows[i]["连接"] = "断开";
                        //离线
                        string sql = "update car set Status=0  where DeviceID='" + dtD.Rows[i]["终端号"] + "'";
                        sqlCmd(sql, ConnStr);
                        //sql = "Insert into Record(DeviceID, SimNo, CarNo, Speed, lng, lat,Blng,Blat, Direction, Address, DTime, RevTime, RevStr, ComStr,P1,P2,P3,P4,P5,P6,Status)  SELECT     DeviceID, SimNo, CarNo, Speed, Lng, Lat,Blng,Blat, Direction, Address, DTime, RevTime, '','',P1,P2,P3,P4,P5,P6,Status  FROM         Car  where  DeviceID='" + dtD.Rows[i]["终端号"] + "'";
                        //sqlCmd(sql, ConnStr);

                        try
                        {
                            sql = "Insert into RecordCarPosition( DeviceID, CarNo, Speed, Address, lng, lat,Blng,Blat, DTime,P1,P2,P3,P4,P5,P6, Direction, Status)  SELECT   DeviceID, CarNo, Speed, Address, lng, lat,Blng,Blat, DTime,P1,P2,P3,P4,P5,P6, Direction, Status from Car where  DeviceID='" + dtD.Rows[i]["终端号"] + "'";
                            sqlCmd(sql, ConnStr);
                            setRecord(sql);
                        }
                        catch
                        { }

                        EventNo = 3;
                        EventStr = "终端断开：" + dtD.Rows[i]["车牌号"] + " " + dtD.Rows[i]["连接"];
                        //setLog(dtD.Rows[i]["终端号"].ToString(),EventNo, "终端断开：" + dtD.Rows[i]["车牌号"] + " " + dtD.Rows[i]["连接"]);

                        //写入报警记录  通讯中断报警
                        sql = "insert into RecordAlarm (DeviceID,CarNo, EventNo, Detail, Flag,Address,lng,lat,Blng,Blat) SELECT  '" + dtD.Rows[i]["终端号"] + "','" + dtD.Rows[i]["车牌号"] + "',5,'通讯中断报警',0 ,Address,Lng,Lat,Blng,Blat FROM         Car  where  DeviceID='" + dtD.Rows[i]["终端号"] + "'";
                        sqlCmd(sql, ConnStr);

                        sql = "Insert into RecordCarOnLine(DeviceID, CarNo, Detail, Address, lng, lat,Blng,Blat)  SELECT   DeviceID, CarNo, '断开', Address, lng, lat ,Blng,Blat from Car where DeviceID='" + dtD.Rows[i]["终端号"] + "'";
                        sqlCmd(sql, ConnStr);
                    }
                }

                int count = dtD.Rows.Count;
                int x = 0;
                for (int i = 0; i < dtD.Rows.Count; i++)
                {
                    if (dtD.Rows[i]["连接"].ToString() != "" && dtD.Rows[i]["连接"].ToString() != "断开")
                        x++;
                }
                lab_RevCount.Text = "在线车辆：" + x.ToString() + @"/" + count.ToString();
            }
            catch
            { }
        }

        public string replaceStr(string str1,string str2,int begin,int end)
        {
            string str = str1;
            string strT1 = str.Substring(0, begin);
            int x = str.Length;
            if (x > end)
                x = end;
            string strT2 = str.Substring(x);
            str = strT1 + str2 + strT2;

            return str;
        }

        public void sendCom(string connNo, byte[] byteSendCom)
        {
            string SendMsgID = "8900";
            byte[] byteSendBody;

            byte type = strToToHexByte("41")[0];//透传消息类型
            byteSendBody = byteSendCom;//透传消息内容

            byte[] byteTem = byteSendBody;
            byteSendBody = new byte[byteSendBody.Length + 1];
            byteSendBody[0] = type;
            Array.Copy(byteTem, 0, byteSendBody, 1, byteTem.Length);

            if (SendMsgID.Length > 0)//发送处理
            {
                DataRow dr = null;
                if (dr == null)
                {
                    for (int i = 0; i < dtD.Rows.Count; i++)
                    {
                        string deviceID = dtD.Rows[i]["终端号"].ToString();
                        if (deviceID == connNo)
                        {
                            dr = dtD.Rows[i];
                            break;
                        }
                    }
                }

                if (dr != null)
                {
                    string device = dr["终端号"].ToString();
                    string IpPort = dr["连接"].ToString();
                    if (device.Length > 0 && IpPort.Length > 0)
                    {
                        byte[] SendBytesTem = GetSendData(SendMsgID, device, byteSendBody);

                        byte[] SendBytes = new byte[SendBytesTem.Length + 1];
                        Array.Copy(SendBytesTem, 0, SendBytes, 0, SendBytesTem.Length);
                        //消息校验
                        SendBytes[SendBytes.Length - 1] = Check(SendBytesTem);
                        //消息转义
                        SendBytes = Trans(SendBytes);
                        //消息添加标识位
                        SendBytes = Pack(SendBytes);
                        //byteToHexStr(SendBytes);
                        //if (cmb_Conn.SelectedIndex == 0)//TCP
                        {
                            foreach (var item in clientList)
                            {
                                if (item.Key == IpPort)
                                {
                                    newSocket = (Socket)item.Value;
                                    string temd = byteToHexStr(SendBytes);
                                    BeginSend(SendBytes);
                                    break;
                                }
                            }
                        }
                    }
                }
                
            }
        }

        public void send8001(string IpPort, string connNo, string MsgNo, string re)
        {
            string MsgID = "8001";
            if (IpPort.Length > 0 && connNo.Length > 0 && MsgNo.Length > 0 && MsgID.Length > 0 && re.Length > 0)//发送处理
            {
                connNo = "000000000000" + connNo;
                connNo = connNo.Substring(connNo.Length - 12, 12);
                MsgNo = "0000" + MsgNo;
                MsgNo = MsgNo.Substring(MsgNo.Length - 4, 4);
                re = "00" + re;
                re = re.Substring(re.Length - 2, 2);

                byte[] byteSendBody = new byte[5];
                //应答流水号
                byte[] byteTem = strToToHexByte(MsgNo);
                Array.Copy(byteTem, 0, byteSendBody, 0, byteTem.Length);
                //应答 ID
                byteTem = strToToHexByte(MsgID);//
                Array.Copy(byteTem, 0, byteSendBody, 2, byteTem.Length);
                //结果  0：成功/确认；1：失败；2：消息有误；3：不支持；4：报警处理确认；
                byteTem = strToToHexByte(re);//成功/确认
                Array.Copy(byteTem, 0, byteSendBody, 4, byteTem.Length);

                byte[] SendBytesTem = GetSendData(MsgID, connNo, byteSendBody);

                byte[] SendBytes = new byte[SendBytesTem.Length + 1];
                Array.Copy(SendBytesTem, 0, SendBytes, 0, SendBytesTem.Length);
                //消息校验
                SendBytes[SendBytes.Length - 1] = Check(SendBytesTem);
                //消息转义
                SendBytes = Trans(SendBytes);
                //消息添加标识位
                SendBytes = Pack(SendBytes);

                {
                    foreach (var item in clientList)
                    {
                        if (item.Key == IpPort)
                        {
                            newSocket = (Socket)item.Value;
                            string temd = byteToHexStr(SendBytes);
                            BeginSend(SendBytes);
                            break;
                        }
                    }
                }
            }
        }
        public void send8106_IpPort(string connNo)//查询  服务器地址、端口
        {
            try
            {
                string SendMsgID = "8106";
                byte[] byteSendBody;
                int length = 1 + 4 * 2;
                byteSendBody = new byte[length];

                //参数总数
                byteSendBody[0] = (byte)2;

                //参数项
                {
                    string strTem = "00 13";
                    strTem = strTem.Replace(" ", "");
                    byte[] byteTem = strToToHexByte(strTem);
                    Array.Copy(byteTem, 0, byteSendBody, 1 + 4 * 0, byteTem.Length);
                }
                //参数项
                {
                    string strTem = "00 18";
                    strTem = strTem.Replace(" ", "");
                    byte[] byteTem = strToToHexByte(strTem);
                    Array.Copy(byteTem, 0, byteSendBody, 1 + 4 * 1, byteTem.Length);
                }

                if (SendMsgID.Length > 0)//发送处理
                {
                    DataRow dr = null;
                    if (dr == null)
                    {
                        for (int i = 0; i < dtD.Rows.Count; i++)
                        {
                            string deviceID = dtD.Rows[i]["终端号"].ToString();
                            if (deviceID == connNo)
                            {
                                dr = dtD.Rows[i];
                                break;
                            }
                        }
                    }

                    if (dr != null)
                    {
                        string device = dr["终端号"].ToString();
                        string IpPort = dr["连接"].ToString();
                        if (device.Length > 0 && IpPort.Length > 0)
                        {
                            byte[] SendBytesTem = GetSendData(SendMsgID, device, byteSendBody);

                            byte[] SendBytes = new byte[SendBytesTem.Length + 1];
                            Array.Copy(SendBytesTem, 0, SendBytes, 0, SendBytesTem.Length);
                            //消息校验
                            SendBytes[SendBytes.Length - 1] = Check(SendBytesTem);
                            //消息转义
                            SendBytes = Trans(SendBytes);
                            //消息添加标识位
                            SendBytes = Pack(SendBytes);
                            //byteToHexStr(SendBytes);
                            //if (cmb_Conn.SelectedIndex == 0)//TCP
                            {
                                foreach (var item in clientList)
                                {
                                    if (item.Key == IpPort)
                                    {
                                        newSocket = (Socket)item.Value;
                                        string temd = byteToHexStr(SendBytes);
                                        BeginSend(SendBytes);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            { }
        }
        public void send8106_Space(string connNo)//查询  缺省时间汇报间隔
        {
            try
            {
                string SendMsgID = "8106";
                byte[] byteSendBody;
                int length = 1 + 4 * 1;
                byteSendBody = new byte[length];

                //参数总数
                byteSendBody[0] = (byte)1;

                //参数项
                {
                    string strTem = "00 29";
                    strTem = strTem.Replace(" ", "");
                    byte[] byteTem = strToToHexByte(strTem);
                    Array.Copy(byteTem, 0, byteSendBody, 1 + 4 * 0, byteTem.Length);
                }

                if (SendMsgID.Length > 0)//发送处理
                {
                    DataRow dr = null;
                    if (dr == null)
                    {
                        for (int i = 0; i < dtD.Rows.Count; i++)
                        {
                            string deviceID = dtD.Rows[i]["终端号"].ToString();
                            if (deviceID == connNo)
                            {
                                dr = dtD.Rows[i];
                                break;
                            }
                        }
                    }

                    if (dr != null)
                    {
                        string device = dr["终端号"].ToString();
                        string IpPort = dr["连接"].ToString();
                        if (device.Length > 0 && IpPort.Length > 0)
                        {
                            byte[] SendBytesTem = GetSendData(SendMsgID, device, byteSendBody);

                            byte[] SendBytes = new byte[SendBytesTem.Length + 1];
                            Array.Copy(SendBytesTem, 0, SendBytes, 0, SendBytesTem.Length);
                            //消息校验
                            SendBytes[SendBytes.Length - 1] = Check(SendBytesTem);
                            //消息转义
                            SendBytes = Trans(SendBytes);
                            //消息添加标识位
                            SendBytes = Pack(SendBytes);
                            //byteToHexStr(SendBytes);
                            //if (cmb_Conn.SelectedIndex == 0)//TCP
                            {
                                foreach (var item in clientList)
                                {
                                    if (item.Key == IpPort)
                                    {
                                        newSocket = (Socket)item.Value;
                                        string temd = byteToHexStr(SendBytes);
                                        BeginSend(SendBytes);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            { }
        }

        public void send8201(string connNo)//位置信息查询
        {
            try
            {
                string SendMsgID = "8201";
                byte[] byteSendBody = null;

                if (SendMsgID.Length > 0)//发送处理
                {
                    DataRow dr = null;
                    if (dr == null)
                    {
                        for (int i = 0; i < dtD.Rows.Count; i++)
                        {
                            string deviceID = dtD.Rows[i]["终端号"].ToString();
                            if (deviceID == connNo)
                            {
                                dr = dtD.Rows[i];
                                break;
                            }
                        }
                    }

                    if (dr != null)
                    {
                        string device = dr["终端号"].ToString();
                        string IpPort = dr["连接"].ToString();
                        if (device.Length > 0 && IpPort.Length > 0)
                        {
                            byte[] SendBytesTem = GetSendData(SendMsgID, device, byteSendBody);

                            byte[] SendBytes = new byte[SendBytesTem.Length + 1];
                            Array.Copy(SendBytesTem, 0, SendBytes, 0, SendBytesTem.Length);
                            //消息校验
                            SendBytes[SendBytes.Length - 1] = Check(SendBytesTem);
                            //消息转义
                            SendBytes = Trans(SendBytes);
                            //消息添加标识位
                            SendBytes = Pack(SendBytes);
                            //byteToHexStr(SendBytes);
                            //if (cmb_Conn.SelectedIndex == 0)//TCP
                            {
                                foreach (var item in clientList)
                                {
                                    if (item.Key == IpPort)
                                    {
                                        newSocket = (Socket)item.Value;
                                        string temd = byteToHexStr(SendBytes);
                                        BeginSend(SendBytes);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            { }
        }

        public void send8105(string connNo)//Gps车载终端重启
        {
            try
            {
                string SendMsgID = "8105";
                byte[] byteSendBody = { 0x04 };

                if (SendMsgID.Length > 0)//发送处理
                {
                    DataRow dr = null;
                    if (dr == null)
                    {
                        for (int i = 0; i < dtD.Rows.Count; i++)
                        {
                            string deviceID = dtD.Rows[i]["终端号"].ToString();
                            if (deviceID == connNo)
                            {
                                dr = dtD.Rows[i];
                                break;
                            }
                        }
                    }

                    if (dr != null)
                    {
                        string device = dr["终端号"].ToString();
                        string IpPort = dr["连接"].ToString();
                        if (device.Length > 0 && IpPort.Length > 0)
                        {
                            byte[] SendBytesTem = GetSendData(SendMsgID, device, byteSendBody);

                            byte[] SendBytes = new byte[SendBytesTem.Length + 1];
                            Array.Copy(SendBytesTem, 0, SendBytes, 0, SendBytesTem.Length);
                            //消息校验
                            SendBytes[SendBytes.Length - 1] = Check(SendBytesTem);
                            //消息转义
                            SendBytes = Trans(SendBytes);
                            //消息添加标识位
                            SendBytes = Pack(SendBytes);
                            //byteToHexStr(SendBytes);
                            //if (cmb_Conn.SelectedIndex == 0)//TCP
                            {
                                foreach (var item in clientList)
                                {
                                    if (item.Key == IpPort)
                                    {
                                        newSocket = (Socket)item.Value;
                                        string temd = byteToHexStr(SendBytes);
                                        BeginSend(SendBytes);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            { }
        }

        public void send8103_IpPort(string connNo)//设置Gps终端主服务器地址、Tcp端口
        {
            try
            {
                string SendMsgID = "8103";
                byte[] byteSendBody = new byte[1];

                //参数总数
                byteSendBody[0] = (byte)2;

                string IP = frmGps.txB_ServerIp.Text;
                string Port = int.Parse(frmGps.txB_ServerPort.Text).ToString("X2");
                Port = Port.Replace(" ", "");
                Port = "00000000" + Port;
                Port = Port.Substring(Port.Length - 8);
                //参数项
                {
                    //值长度
                    byte[] byteIp = Encoding.GetEncoding("GBK").GetBytes(IP);
                    byte[] lenB = { (byte)(byteIp.Length) };

                    string strTem = byteToHexStr(lenB);

                    strTem = "00 13" + strTem + byteToHexStr(byteIp);
                    strTem = strTem.Replace(" ", "");
                    byte[] byteTem = strToToHexByte(strTem);

                    byte[] temB = byteSendBody;
                    byteSendBody = new byte[temB.Length + byteTem.Length];
                    Array.Copy(temB, 0, byteSendBody, 0, temB.Length);

                    Array.Copy(byteTem, 0, byteSendBody, temB.Length, byteTem.Length);
                }
                //参数项
                {
                    //值长度
                    string strTem = Port;
                    strTem = strTem.Replace(" ", "");
                    byte[] byteTem = strToToHexByte(strTem);
                    byte[] lenB = { (byte)(byteTem.Length) };

                    strTem = byteToHexStr(lenB);

                    strTem = "00 18" + strTem + Port;
                    strTem = strTem.Replace(" ", "");
                    byteTem = strToToHexByte(strTem);

                    byte[] temB = byteSendBody;
                    byteSendBody = new byte[temB.Length + byteTem.Length];
                    Array.Copy(temB, 0, byteSendBody, 0, temB.Length);

                    Array.Copy(byteTem, 0, byteSendBody, temB.Length, byteTem.Length);
                }

                if (SendMsgID.Length > 0)//发送处理
                {
                    DataRow dr = null;
                    if (dr == null)
                    {
                        for (int i = 0; i < dtD.Rows.Count; i++)
                        {
                            string deviceID = dtD.Rows[i]["终端号"].ToString();
                            if (deviceID == connNo)
                            {
                                dr = dtD.Rows[i];
                                break;
                            }
                        }
                    }

                    if (dr != null)
                    {
                        string device = dr["终端号"].ToString();
                        string IpPort = dr["连接"].ToString();
                        if (device.Length > 0 && IpPort.Length > 0)
                        {
                            byte[] SendBytesTem = GetSendData(SendMsgID, device, byteSendBody);

                            byte[] SendBytes = new byte[SendBytesTem.Length + 1];
                            Array.Copy(SendBytesTem, 0, SendBytes, 0, SendBytesTem.Length);
                            //消息校验
                            SendBytes[SendBytes.Length - 1] = Check(SendBytesTem);
                            //消息转义
                            SendBytes = Trans(SendBytes);
                            //消息添加标识位
                            SendBytes = Pack(SendBytes);
                            //byteToHexStr(SendBytes);
                            //if (cmb_Conn.SelectedIndex == 0)//TCP
                            {
                                foreach (var item in clientList)
                                {
                                    if (item.Key == IpPort)
                                    {
                                        newSocket = (Socket)item.Value;
                                        string temd = byteToHexStr(SendBytes);
                                        BeginSend(SendBytes);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            { }
        }

        public void send8103_Space(string connNo)//设置 缺省时间汇报间隔
        {
            try
            {
                string SendMsgID = "8103";
                byte[] byteSendBody = new byte[1];

                //参数总数
                byteSendBody[0] = (byte)2;

                string Space = frmGps.num_Space.Value.ToString("X2");
                Space = Space.Replace(" ", "");
                Space = "00000000" + Space;
                Space = Space.Substring(Space.Length - 8);
                //参数项
                {
                    //值长度
                    string strTem = Space;
                    strTem = strTem.Replace(" ", "");
                    byte[] byteTem = strToToHexByte(strTem);
                    byte[] lenB = { (byte)(byteTem.Length) };

                    strTem = byteToHexStr(lenB);

                    strTem = "00 29" + strTem + Space;
                    strTem = strTem.Replace(" ", "");
                    byteTem = strToToHexByte(strTem);

                    byte[] temB = byteSendBody;
                    byteSendBody = new byte[temB.Length + byteTem.Length];
                    Array.Copy(temB, 0, byteSendBody, 0, temB.Length);

                    Array.Copy(byteTem, 0, byteSendBody, temB.Length, byteTem.Length);
                }

                if (SendMsgID.Length > 0)//发送处理
                {
                    DataRow dr = null;
                    if (dr == null)
                    {
                        for (int i = 0; i < dtD.Rows.Count; i++)
                        {
                            string deviceID = dtD.Rows[i]["终端号"].ToString();
                            if (deviceID == connNo)
                            {
                                dr = dtD.Rows[i];
                                break;
                            }
                        }
                    }

                    if (dr != null)
                    {
                        string device = dr["终端号"].ToString();
                        string IpPort = dr["连接"].ToString();
                        if (device.Length > 0 && IpPort.Length > 0)
                        {
                            byte[] SendBytesTem = GetSendData(SendMsgID, device, byteSendBody);

                            byte[] SendBytes = new byte[SendBytesTem.Length + 1];
                            Array.Copy(SendBytesTem, 0, SendBytes, 0, SendBytesTem.Length);
                            //消息校验
                            SendBytes[SendBytes.Length - 1] = Check(SendBytesTem);
                            //消息转义
                            SendBytes = Trans(SendBytes);
                            //消息添加标识位
                            SendBytes = Pack(SendBytes);
                            //byteToHexStr(SendBytes);
                            //if (cmb_Conn.SelectedIndex == 0)//TCP
                            {
                                foreach (var item in clientList)
                                {
                                    if (item.Key == IpPort)
                                    {
                                        newSocket = (Socket)item.Value;
                                        string temd = byteToHexStr(SendBytes);
                                        BeginSend(SendBytes);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            { }
        }

        public void send8100(string IpPort, string connNo, string MsgNo, string re, string Code)
        {
            string MsgID = "8100";
            if (IpPort.Length > 0 && connNo.Length > 0 && MsgNo.Length > 0 && MsgID.Length > 0 && re.Length > 0)//发送处理
            {
                connNo = "000000000000" + connNo;
                connNo = connNo.Substring(connNo.Length - 12, 12);
                MsgNo = "0000" + MsgNo;
                MsgNo = MsgNo.Substring(MsgNo.Length - 4, 4);
                re = "00" + re;
                re = re.Substring(re.Length - 2, 2);

                byte[] byteSendBody = new byte[14];
                //应答流水号
                byte[] byteTem = strToToHexByte(MsgNo);
                Array.Copy(byteTem, 0, byteSendBody, 0, byteTem.Length);
                //结果  0：成功；1：车辆已被注册；2：数据库中无该车辆；3：终端已被注册；4：数据库中无该终端
                byteTem = strToToHexByte(re);//成功/确认
                Array.Copy(byteTem, 0, byteSendBody, 2, byteTem.Length);
                //鉴权码
                Code = Code.Substring(0, 33);
                byteTem = strToToHexByte(Code);//
                Array.Copy(byteTem, 0, byteSendBody, 3, byteTem.Length);

                byte[] SendBytesTem = GetSendData(MsgID, connNo, byteSendBody);

                byte[] SendBytes = new byte[SendBytesTem.Length + 1];
                Array.Copy(SendBytesTem, 0, SendBytes, 0, SendBytesTem.Length);
                //消息校验
                SendBytes[SendBytes.Length - 1] = Check(SendBytesTem);
                //消息转义
                SendBytes = Trans(SendBytes);
                //消息添加标识位
                SendBytes = Pack(SendBytes);

                {
                    foreach (var item in clientList)
                    {
                        if (item.Key == IpPort)
                        {
                            newSocket = (Socket)item.Value;
                            string temd = byteToHexStr(SendBytes);
                            BeginSend(SendBytes);
                            break;
                        }
                    }
                }
            }
        }

        //消息封装
        private byte[] GetSendData(string msgID, string ConnNo, byte[] data)
        {
            byte[] re = new byte[12];
            if (data != null)
            {
                re = new byte[12 + data.Length];
            }
            byte[] byteTem = null;
            //消息 ID
            byteTem = strToToHexByte(msgID);
            Array.Copy(byteTem, 0, re, 0, byteTem.Length);

            //消息体属性
            byteTem = new byte[2];
            {
                if (data != null)
                {
                    byteTem[0] = (byte)((data.Length) / 255);
                    byteTem[1] = (byte)((data.Length) % 255);
                }
                else
                {
                    byteTem[0] = 0x00;
                    byteTem[1] = 0x00;
                }
            }
            Array.Copy(byteTem, 0, re, 2, byteTem.Length);

            //终端手机号
            byteTem = strToToHexByte(ConnNo);
            Array.Copy(byteTem, 0, re, 4, byteTem.Length);

            //消息流水号
            byteTem = strToToHexByte("00 0C");
            Array.Copy(byteTem, 0, re, 10, byteTem.Length);

            //消息包封装项
            if (data != null)
            {
                Array.Copy(data, 0, re, 12, data.Length);
            }

            return re;
        }

        //发送消息时：消息封装——>计算并填充校验码——>转义；
        private void BeginSend(byte[] data)
        {
            try
            {
                if (newSocket != null && newSocket.Connected)
                {
                    //发送
                    newSocket.Send(data);
                }
            }
            catch
            { }
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            try
            {
                {
                    string sql = "update DownCom set Status=3 where datediff(ss,createtime,getdate())>100 and (Status = 0) and (Flag = 0)";//超过100秒，为发送失败
                    sqlCmd(sql, ConnStr);

                    //sql = "SELECT     ID, DeviceID, CmdStr  FROM         DownCom   WHERE     Status = 0 AND Flag = 0 ";
                    sql = "SELECT     DownCom.ID, DownCom.DeviceID, DownCom.CmdStr, Car.Status "+
                            " FROM         DownCom INNER JOIN "+
                                                 " Car ON DownCom.DeviceID = Car.DeviceID "+
                            " WHERE     (DownCom.Status = 0) AND (DownCom.Flag = 0) and (Car.Status>0)";
                    DataTable dtTem = sqlRead(sql, ConnStr);
                    for (int i = 0; i < dtTem.Rows.Count; i++)
                    {
                        string id = dtTem.Rows[i]["ID"].ToString();
                        string connNo = dtTem.Rows[i]["DeviceID"].ToString();
                        string data = dtTem.Rows[i]["CmdStr"].ToString();
                        if (connNo.Length > 0)
                        {
                            
                            byte[] sendByte = strToToHexByte(data);
                            sendCom(connNo, sendByte);

                            string sql1 = "update DownCom set Flag=1,SendTime='" + DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + "' where ID=" + id;
                            sqlCmd(sql1, ConnStr);
                        }
                    }
                }

                //发送com复位指令
                for (int i = 0; i < comLose.Length; i++)
                {
                    if (comLose[i] > 5)
                    {
                        byte[] byteTem = comSendReset();
                        string strTem = byteToHexStr(byteTem);
                        string sql = "insert into DownCom  (DeviceID,CmdStr) values ('" + dataGridView_DeviceList.Rows[i].Cells["终端号"].Value.ToString() + "','" + strTem + "') ";
                        sqlCmd(sql, ConnStr);

                        comLose[i] = 0;
                    }
                }

                //删除过期指令
                {
                    string sql = "delete from DownCom  where  CreateTime <='" + DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd 00:00:00") + "' ";
                    sqlCmd(sql, ConnStr);
                }
            }
            catch
            { }
        }
        //删除终端所有圆形区域
        public void send8601_All(string IpPort, string connNO)
        {
            connNO = "000000000000" + connNO;
            connNO = connNO.Substring(connNO.Length - 12, 12);

            string MsgID = "8601";
            int length = 1;
            byte[] byteSendBody = new byte[length];
            //区域总数
            byteSendBody[0] = (byte)(1);

            byte[] SendBytesTem = GetSendData(MsgID, connNO, byteSendBody);

            byte[] SendBytes = new byte[SendBytesTem.Length + 1];
            Array.Copy(SendBytesTem, 0, SendBytes, 0, SendBytesTem.Length);
            //消息校验
            SendBytes[SendBytes.Length - 1] = Check(SendBytesTem);
            //消息转义
            SendBytes = Trans(SendBytes);
            //消息添加标识位
            SendBytes = Pack(SendBytes);

            {
                foreach (var item in clientList)
                {
                    if (item.Key == IpPort)
                    {
                        newSocket = (Socket)item.Value;
                        string temd = byteToHexStr(SendBytes);
                        BeginSend(SendBytes);
                        break;
                    }
                }
            }
        }
        //删除终端圆形区域
        public void send8601(string IpPort, string connNO, string areaID)
        {
            connNO = "000000000000" + connNO;
            connNO = connNO.Substring(connNO.Length - 12, 12);
            areaID = "00000000" + areaID;
            areaID = areaID.Substring(areaID.Length - 8, 8);

            string MsgID = "8601";
            int length = 1 + 4;
            byte[] byteSendBody = new byte[length];
            //区域总数
            byteSendBody[0] = (byte)(1);
            //区域项
            byte[] byteTem = strToToHexByte(areaID);
            Array.Copy(byteTem, 0, byteSendBody, 1, byteTem.Length);

            byte[] SendBytesTem = GetSendData(MsgID, connNO, byteSendBody);

            byte[] SendBytes = new byte[SendBytesTem.Length + 1];
            Array.Copy(SendBytesTem, 0, SendBytes, 0, SendBytesTem.Length);
            //消息校验
            SendBytes[SendBytes.Length - 1] = Check(SendBytesTem);
            //消息转义
            SendBytes = Trans(SendBytes);
            //消息添加标识位
            SendBytes = Pack(SendBytes);

            {
                foreach (var item in clientList)
                {
                    if (item.Key == IpPort)
                    {
                        newSocket = (Socket)item.Value;
                        string temd = byteToHexStr(SendBytes);
                        BeginSend(SendBytes);
                        break;
                    }
                }
            }
        }
        //添加终端圆形区域
        public void send8600(string IpPort, string connNO, string areaID, string lat, string lng, string Radius)
        {
            connNO = "000000000000" + connNO;
            connNO = connNO.Substring(connNO.Length - 12, 12);
            areaID = "00000000" + areaID;
            areaID = areaID.Substring(areaID.Length - 8, 8);

            string MsgID = "8600";
            int length = 2 + 18;
            byte[] byteSendBody = new byte[length];
            //设置属性
            byteSendBody[0] = (byte)0;
            //区域总数
            byteSendBody[1] = (byte)1;
            //区域项
            string AreaIDTem = "00 00 00 01";//区域编号
            AreaIDTem = areaID;
            string AreaFlagTem = "3C 00";//区域属性
            string LatTem = "02 2F 9C BF";//中心点纬度
            double LatD = double.Parse(lat)*1000000;
            LatTem = "00000000" + ((int)LatD).ToString("X2");//十进制转十六进制
            LatTem = LatTem.Substring(LatTem.Length - 8, 8);
            string LngTem = "06 FB 45 D6";//中心点经度
            double LngD = double.Parse(lng) * 1000000;
            LngTem = "00000000" + ((int)LngD).ToString("X2");//十进制转十六进制
            LngTem = LngTem.Substring(LngTem.Length - 8, 8);
            string bj = "00 00 00 64";//半径
            bj = "00000000" + ((int)double.Parse(Radius)).ToString("X2");
            bj = bj.Substring(bj.Length - 8, 8);

            string strTem = AreaIDTem + AreaFlagTem + LatTem + LngTem + bj;
            strTem = strTem.Replace(" ", "");
            byte[] byteTem = strToToHexByte(strTem);
            Array.Copy(byteTem, 0, byteSendBody, 2, byteTem.Length);

            byte[] SendBytesTem = GetSendData(MsgID, connNO, byteSendBody);

            byte[] SendBytes = new byte[SendBytesTem.Length + 1];
            Array.Copy(SendBytesTem, 0, SendBytes, 0, SendBytesTem.Length);
            //消息校验
            SendBytes[SendBytes.Length - 1] = Check(SendBytesTem);
            //消息转义
            SendBytes = Trans(SendBytes);
            //消息添加标识位
            SendBytes = Pack(SendBytes);

            {
                foreach (var item in clientList)
                {
                    if (item.Key == IpPort)
                    {
                        newSocket = (Socket)item.Value;
                        string temd = byteToHexStr(SendBytes);
                        BeginSend(SendBytes);
                        break;
                    }
                }
            }
        }
        //删除终端所有多边形区域
        public void send8605_All(string IpPort, string connNO)
        {
            connNO = "000000000000" + connNO;
            connNO = connNO.Substring(connNO.Length - 12, 12);

            string MsgID = "8605";
            int length = 1;
            byte[] byteSendBody = new byte[length];
            //区域总数
            byteSendBody[0] = (byte)(0);

            byte[] SendBytesTem = GetSendData(MsgID, connNO, byteSendBody);

            byte[] SendBytes = new byte[SendBytesTem.Length + 1];
            Array.Copy(SendBytesTem, 0, SendBytes, 0, SendBytesTem.Length);
            //消息校验
            SendBytes[SendBytes.Length - 1] = Check(SendBytesTem);
            //消息转义
            SendBytes = Trans(SendBytes);
            //消息添加标识位
            SendBytes = Pack(SendBytes);

            {
                foreach (var item in clientList)
                {
                    if (item.Key == IpPort)
                    {
                        newSocket = (Socket)item.Value;
                        string temd = byteToHexStr(SendBytes);
                        BeginSend(SendBytes);
                        break;
                    }
                }
            }
        }
        //删除终端多边形区域
        public void send8605(string IpPort, string connNO, string areaID)
        {
            connNO = "000000000000" + connNO;
            connNO = connNO.Substring(connNO.Length - 12, 12);
            areaID = "00000000" + areaID;
            areaID = areaID.Substring(areaID.Length - 8, 8);

            string MsgID = "8605";
            int length = 1+4;
            byte[] byteSendBody = new byte[length];
            //区域总数
            byteSendBody[0] = (byte)(1);
            //区域项
            byte[] byteTem = strToToHexByte(areaID);
            Array.Copy(byteTem, 0, byteSendBody, 1, byteTem.Length);

            byte[] SendBytesTem = GetSendData(MsgID, connNO, byteSendBody);

            byte[] SendBytes = new byte[SendBytesTem.Length + 1];
            Array.Copy(SendBytesTem, 0, SendBytes, 0, SendBytesTem.Length);
            //消息校验
            SendBytes[SendBytes.Length - 1] = Check(SendBytesTem);
            //消息转义
            SendBytes = Trans(SendBytes);
            //消息添加标识位
            SendBytes = Pack(SendBytes);

            {
                foreach (var item in clientList)
                {
                    if (item.Key == IpPort)
                    {
                        newSocket = (Socket)item.Value;
                        string temd = byteToHexStr(SendBytes);
                        BeginSend(SendBytes);
                        break;
                    }
                }
            }
        }
        //添加终端多边形区域
        public void send8604(string IpPort, string connNO, string areaID, string[] lats, string[] lngs)
        {
            connNO = "000000000000" + connNO;
            connNO = connNO.Substring(connNO.Length - 12, 12);
            areaID = "00000000" + areaID;
            areaID = areaID.Substring(areaID.Length - 8, 8);

            string MsgID = "8604";
            int length = 8 + 8 * (lats.Length);
            byte[] byteSendBody = new byte[length];

            //区域 ID
            byte[] byteTem = strToToHexByte(areaID);
            Array.Copy(byteTem, 0, byteSendBody, 0, byteTem.Length);
            //区域属性
            byteTem = strToToHexByte("3C 00");
            Array.Copy(byteTem, 0, byteSendBody, 4, byteTem.Length);
            //区域总顶点数
            string strTem = (lats.Length).ToString();
            strTem = "0000" + strTem;
            strTem = strTem.Substring(strTem.Length - 4);
            byteTem = strToToHexByte(strTem);
            Array.Copy(byteTem, 0, byteSendBody, 6, byteTem.Length);

            //区域项
            for (int i = 0; i < lats.Length; i++)
            {
                strTem = "";
                string LatTem = "02 2F 9C BF";//点纬度
                double LatD = double.Parse(lats[i]) * 1000000;
                LatTem = "00000000" + ((int)LatD).ToString("X2");//十进制转十六进制
                LatTem = LatTem.Substring(LatTem.Length - 8, 8);
                string LngTem = "06 FB 45 D6";//点经度
                double LngD = double.Parse(lngs[i]) * 1000000;
                LngTem = "00000000" + ((int)LngD).ToString("X2");//十进制转十六进制
                LngTem = LngTem.Substring(LngTem.Length - 8, 8);
                strTem = LatTem + LngTem;
                strTem = strTem.Replace(" ", "");
                byteTem = strToToHexByte(strTem);
                Array.Copy(byteTem, 0, byteSendBody, 8 + 8 * i, byteTem.Length);
            }
            byte[] SendBytesTem = GetSendData(MsgID, connNO, byteSendBody);

            byte[] SendBytes = new byte[SendBytesTem.Length + 1];
            Array.Copy(SendBytesTem, 0, SendBytes, 0, SendBytesTem.Length);
            //消息校验
            SendBytes[SendBytes.Length - 1] = Check(SendBytesTem);
            //消息转义
            SendBytes = Trans(SendBytes);
            //消息添加标识位
            SendBytes = Pack(SendBytes);

            {
                foreach (var item in clientList)
                {
                    if (item.Key == IpPort)
                    {
                        newSocket = (Socket)item.Value;
                        string temd = byteToHexStr(SendBytes);
                        temd = temd.Replace(" ", "");
                        BeginSend(SendBytes);
                        break;
                    }
                }
            }
        }

        //更新终端内部区域
        public void SendDeviceAreas(string IpPort, string connNO)
        {
            if (IpPort.Length > 0 && connNO.Length > 0)
            {
                //send8605_All(IpPort, connNo);//清空多边形区域
                //Thread.Sleep(1000);
                //send8601_All(IpPort, connNo);//清空圆形区域
                //Thread.Sleep(1000);
                {//发送出发站点
                    string sql = "SELECT     SheetRecord.ID, Car.SimNo, SheetRecord.FromSiteID, Site.AreaType, Site.Radius, SheetRecord.Result, SheetRecord.LoadFlag " +
                                " FROM         SheetRecord INNER JOIN " +
                                                     " Car ON SheetRecord.DeviceID = Car.DeviceID INNER JOIN " +
                                                     " Site ON SheetRecord.FromSiteID = Site.SiteID " +
                                "where Car.DeviceID='" + connNO + "'";
                    DataTable dtTem = sqlRead(sql, ConnStr);
                    for (int i = 0; i < dtTem.Rows.Count; i++)
                    {
                        string ID = dtTem.Rows[i]["ID"].ToString();
                        string FromSiteID = dtTem.Rows[i]["FromSiteID"].ToString();
                        string AreaType = dtTem.Rows[i]["AreaType"].ToString();
                        string Radius = dtTem.Rows[i]["Radius"].ToString();
                        if (connNO.Length > 0)
                        {
                            if (AreaType == "0")//圆形区域
                            {
                                sql = "SELECT     PointID, SiteID, Lng, Lat, BLng, BLat  FROM         Points " +
                                  "where SiteID = " + FromSiteID + "order by PointID";
                                DataTable dtTem1 = sqlRead(sql, ConnStr);
                                string LngTem = "";
                                string LatTem = "";
                                for (int i1 = 0; i1 < dtTem1.Rows.Count; i1++)
                                {
                                    LngTem = dtTem1.Rows[i1]["Lng"].ToString();
                                    LatTem = dtTem1.Rows[i1]["Lat"].ToString();
                                }
                                if (LngTem.Length > 0 && LatTem.Length > 0)
                                {
                                    //删除圆形区域
                                    //send8601(IpPort, connNO, FromSiteID);
                                    //Thread.Sleep(300);
                                    //追加圆形区域
                                    //send8600(IpPort, connNO, FromSiteID, LatTem, LngTem, Radius);
                                    //Thread.Sleep(1000);
                                }
                            }
                            if (AreaType == "1" || AreaType == "2")//多边形区域
                            {
                                sql = "SELECT     PointID, SiteID, Lng, Lat, BLng, BLat  FROM         Points " +
                                      "where SiteID = " + FromSiteID + "order by PointID";
                                DataTable dtTem1 = sqlRead(sql, ConnStr);
                                string[] lats = new string[dtTem1.Rows.Count];
                                string[] lngs = new string[dtTem1.Rows.Count];
                                for (int i1 = 0; i1 < dtTem1.Rows.Count; i1++)
                                {
                                    string LngTem = dtTem1.Rows[i1]["Lng"].ToString();
                                    string LatTem = dtTem1.Rows[i1]["Lat"].ToString();
                                    lats[i1] = LatTem;
                                    lngs[i1] = LngTem;
                                }
                                //删除多边形区域
                                //send8605(IpPort, connNO, FromSiteID);
                                //追加多边形区域
                                //send8604(IpPort, connNO, FromSiteID, lats, lngs);
                                //Thread.Sleep(1000);
                            }
                        }
                    }
                }
                {//发送目的站点
                    string sql = "SELECT     SheetRecord.ID, Car.SimNo, Site.AreaType, Site.Radius, SheetRecord.Result, SheetRecord.LoadFlag, SheetRecord.DirSiteID " +
                                " FROM         SheetRecord INNER JOIN " +
                                                     " Car ON SheetRecord.DeviceID = Car.DeviceID INNER JOIN" +
                                                     " Site ON SheetRecord.DirSiteID = Site.SiteID " +
                                "where Car.DeviceID='" + connNO + "'";
                    DataTable dtTem = sqlRead(sql, ConnStr);
                    for (int i = 0; i < dtTem.Rows.Count; i++)
                    {
                        string ID = dtTem.Rows[i]["ID"].ToString();
                        string DirSiteID = dtTem.Rows[i]["DirSiteID"].ToString();
                        string AreaType = dtTem.Rows[i]["AreaType"].ToString();
                        string Radius = dtTem.Rows[i]["Radius"].ToString();
                        if (connNO.Length > 0)
                        {
                            if (AreaType == "0")//圆形区域
                            {
                                sql = "SELECT     PointID, SiteID, Lng, Lat, BLng, BLat  FROM         Points " +
                                  "where SiteID = " + DirSiteID + "order by PointID";
                                DataTable dtTem1 = sqlRead(sql, ConnStr);
                                string LngTem = "";
                                string LatTem = "";
                                for (int i1 = 0; i1 < dtTem1.Rows.Count; i1++)
                                {
                                    LngTem = dtTem1.Rows[i1]["Lng"].ToString();
                                    LatTem = dtTem1.Rows[i1]["Lat"].ToString();
                                }
                                if (LngTem.Length > 0 && LatTem.Length > 0)
                                {
                                    //删除圆形区域
                                    //send8601(IpPort, connNO, DirSiteID);
                                    //追加圆形区域
                                    //send8600(IpPort, connNO, DirSiteID, LatTem, LngTem, Radius);
                                    //Thread.Sleep(1000);
                                }
                            }
                            if (AreaType == "1" || AreaType == "2")//多边形区域
                            {
                                sql = "SELECT     PointID, SiteID, Lng, Lat, BLng, BLat  FROM         Points " +
                                      "where SiteID = " + DirSiteID + "order by PointID";
                                DataTable dtTem1 = sqlRead(sql, ConnStr);
                                string[] lats = new string[dtTem1.Rows.Count];
                                string[] lngs = new string[dtTem1.Rows.Count];
                                for (int i1 = 0; i1 < dtTem1.Rows.Count; i1++)
                                {
                                    string LngTem = dtTem1.Rows[i1]["Lng"].ToString();
                                    string LatTem = dtTem1.Rows[i1]["Lat"].ToString();
                                    lats[i1] = LatTem;
                                    lngs[i1] = LngTem;
                                }
                                //删除多边形区域
                                //send8605(IpPort, connNO, DirSiteID);
                                //追加多边形区域
                                //send8604(IpPort, connNO, DirSiteID, lats, lngs);
                                //Thread.Sleep(1000);
                            }
                        }
                    }
                }
            }
        }
        //根据单据更新区域到所有在线终端
        public void SendSheetDevices(string id)
        {
            if (id.Length > 0)
            {
                for (int j = 0; j < dtD.Rows.Count; j++)
                {
                    string IpPort = dtD.Rows[j]["连接"].ToString();
                    string connNo = dtD.Rows[j]["终端号"].ToString();

                    {//发送出发站点
                        string sql = "SELECT     SheetRecord.ID, Car.SimNo, SheetRecord.FromSiteID, Site.AreaType, Site.Radius, SheetRecord.Result, SheetRecord.LoadFlag " +
                                    " FROM         SheetRecord INNER JOIN " +
                                                         " Car ON SheetRecord.DeviceID = Car.DeviceID INNER JOIN " +
                                                         " Site ON SheetRecord.FromSiteID = Site.SiteID " +
                                    "where SheetRecord.ID='" + id + "'";
                        DataTable dtTem = sqlRead(sql, ConnStr);
                        for (int i = 0; i < dtTem.Rows.Count; i++)
                        {
                            string ID = dtTem.Rows[i]["ID"].ToString();
                            string FromSiteID = dtTem.Rows[i]["FromSiteID"].ToString();
                            string AreaType = dtTem.Rows[i]["AreaType"].ToString();
                            string Radius = dtTem.Rows[i]["Radius"].ToString();
                            if (AreaType == "0")//圆形区域
                            {
                                sql = "SELECT     PointID, SiteID, Lng, Lat, BLng, BLat  FROM         Points " +
                                  "where SiteID = " + FromSiteID + "order by PointID";
                                DataTable dtTem1 = sqlRead(sql, ConnStr);
                                string LngTem = "";
                                string LatTem = "";
                                for (int i1 = 0; i1 < dtTem1.Rows.Count; i1++)
                                {
                                    LngTem = dtTem1.Rows[i1]["Lng"].ToString();
                                    LatTem = dtTem1.Rows[i1]["Lat"].ToString();
                                }
                                if (LngTem.Length > 0 && LatTem.Length > 0)
                                {
                                    //删除圆形区域
                                    //send8601(IpPort, connNo, FromSiteID);
                                    //Thread.Sleep(1000);
                                    //追加圆形区域
                                    //send8600(IpPort, connNo, FromSiteID, LatTem, LngTem, Radius);
                                    //Thread.Sleep(1000);
                                }
                            }
                            if (AreaType == "1")//多边形区域
                            {
                                sql = "SELECT     PointID, SiteID, Lng, Lat, BLng, BLat  FROM         Points " +
                                      "where SiteID = " + FromSiteID + "order by PointID";
                                DataTable dtTem1 = sqlRead(sql, ConnStr);
                                string[] lats = new string[dtTem1.Rows.Count];
                                string[] lngs = new string[dtTem1.Rows.Count];
                                for (int i1 = 0; i1 < dtTem1.Rows.Count; i1++)
                                {
                                    string LngTem = dtTem1.Rows[i1]["Lng"].ToString();
                                    string LatTem = dtTem1.Rows[i1]["Lat"].ToString();
                                    lats[i1] = LatTem;
                                    lngs[i1] = LngTem;
                                }
                                //删除多边形区域
                                //send8605(IpPort, connNo, FromSiteID);
                                //Thread.Sleep(1000);
                                //追加多边形区域
                                //send8604(IpPort, connNo, FromSiteID, lats, lngs);
                                //Thread.Sleep(1000);
                            }
                        }
                    }
                    {//发送目的站点
                        string sql = "SELECT     SheetRecord.ID, Car.SimNo, Site.AreaType, Site.Radius, SheetRecord.Result, SheetRecord.LoadFlag, SheetRecord.DirSiteID " +
                                    " FROM         SheetRecord INNER JOIN " +
                                                         " Car ON SheetRecord.DeviceID = Car.DeviceID INNER JOIN" +
                                                         " Site ON SheetRecord.DirSiteID = Site.SiteID " +
                                    "where SheetRecord.ID='" + id + "'";
                        DataTable dtTem = sqlRead(sql, ConnStr);
                        for (int i = 0; i < dtTem.Rows.Count; i++)
                        {
                            string ID = dtTem.Rows[i]["ID"].ToString();
                            string DirSiteID = dtTem.Rows[i]["DirSiteID"].ToString();
                            string AreaType = dtTem.Rows[i]["AreaType"].ToString();
                            string Radius = dtTem.Rows[i]["Radius"].ToString();
                            if (AreaType == "0")//圆形区域
                            {
                                sql = "SELECT     PointID, SiteID, Lng, Lat, BLng, BLat  FROM         Points " +
                                  "where SiteID = " + DirSiteID + "order by PointID";
                                DataTable dtTem1 = sqlRead(sql, ConnStr);
                                string LngTem = "";
                                string LatTem = "";
                                for (int i1 = 0; i1 < dtTem1.Rows.Count; i1++)
                                {
                                    LngTem = dtTem1.Rows[i1]["Lng"].ToString();
                                    LatTem = dtTem1.Rows[i1]["Lat"].ToString();
                                }
                                if (LngTem.Length > 0 && LatTem.Length > 0)
                                {
                                    //删除圆形区域
                                    //send8601(IpPort, connNo, DirSiteID);
                                    //Thread.Sleep(1000);
                                    //追加圆形区域
                                    //send8600(IpPort, connNo, DirSiteID, LatTem, LngTem, Radius);
                                    //Thread.Sleep(1000);
                                }
                            }
                            if (AreaType == "1")//多边形区域
                            {
                                sql = "SELECT     PointID, SiteID, Lng, Lat, BLng, BLat  FROM         Points " +
                                      "where SiteID = " + DirSiteID + "order by PointID";
                                DataTable dtTem1 = sqlRead(sql, ConnStr);
                                string[] lats = new string[dtTem1.Rows.Count];
                                string[] lngs = new string[dtTem1.Rows.Count];
                                for (int i1 = 0; i1 < dtTem1.Rows.Count; i1++)
                                {
                                    string LngTem = dtTem1.Rows[i1]["Lng"].ToString();
                                    string LatTem = dtTem1.Rows[i1]["Lat"].ToString();
                                    lats[i1] = LatTem;
                                    lngs[i1] = LngTem;
                                }
                                //删除多边形区域
                                //send8605(IpPort, connNo, DirSiteID);
                                //Thread.Sleep(1000);
                                //追加多边形区域
                                //send8604(IpPort, connNo, DirSiteID, lats, lngs);
                                //Thread.Sleep(300);
                            }
                        }
                    }
                }
            }
        }


        private void timerSheetOut_Tick(object sender, EventArgs e)
        {
            try
            {
                if (needGetCars > 0)
                {
                    needGetCars = 0;
                    getTree();
                }
                //未处理单据设为超时
                string sql = "update SheetRecord set Result=3  where Result < 2  and CreateTime<='" + DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd 00:00:00") + "'";
                sqlCmd(sql, ConnStr);

                //下发单据到设备
                sql = "SELECT     ID  FROM         SheetRecord " +
                        "where LoadFlag = 0";
                DataTable dtTem1 = sqlRead(sql, ConnStr);
                for (int i1 = 0; i1 < dtTem1.Rows.Count; i1++)
                {
                    string ID = dtTem1.Rows[i1]["ID"].ToString();
                    SendSheetDevices(ID);
                    string sql1 = "update SheetRecord set LoadFlag=1  where ID = '" + ID + "'";
                    sqlCmd(sql1, ConnStr);
                    break;
                }

                //下发应急卡到设备
                sql = "SELECT     DeviceID  FROM         Car " +
                        "where LoadFlag = 0";
                DataTable dtTem2 = sqlRead(sql, ConnStr);
                for (int i1 = 0; i1 < dtTem2.Rows.Count; i1++)
                {
                    string DeviceID = dtTem2.Rows[i1]["DeviceID"].ToString();
                    SendDeviceCard(DeviceID);
                    string sql1 = "update Car set LoadFlag=1  where DeviceID = '" + DeviceID + "'";
                    sqlCmd(sql1, ConnStr);
                    break;
                }
            }
            catch
            { }
        }
        public void dealCard(string connNO, string cardid)
        {
            int cardType = 0;
            int cardType1 = 0; string SiteName = "";//写入刷卡记录
            {
                //处理应急卡
                string sql = "SELECT     DeviceID, CarNo, CardID, SimNo  FROM         Car " +
                              "where  CardID='" + cardid + "' and DeviceID='" + connNO + "'";
                DataTable dtTem = sqlRead(sql, ConnStr);
                for (int i = 0; i < dtTem.Rows.Count; i++)
                {
                    string DeviceID = dtTem.Rows[i]["DeviceID"].ToString();
                    string CarNo = dtTem.Rows[i]["CarNo"].ToString();

                    cardType1 = 1;

                    int x = -1;
                    for (int j = 0; j < dtD.Rows.Count; j++)
                    {
                        if (dtD.Rows[j]["终端号"].ToString() == DeviceID)
                        {
                            x = j;
                            break;
                        }
                    }
                    if (x >= 0)
                    {
                        cardType = 1;
                        if (YJCard.Length > x)
                        {
                            if (YJCard[x] == 0)
                            {
                                YJCard[x] = 1;

                                byte[] Ps = { 0x09, 0x09, 0x09, 0x09, 0x09, 0x09 };
                                byte[] byteTem = comControl(Ps);
                                string strTem = byteToHexStr(byteTem);
                                sql = "insert into DownCom  (DeviceID,CmdStr) values ('" + DeviceID + "','" + strTem + "') ";
                                sqlCmd(sql, ConnStr);

                                EventStr = "应急卡施封命令" + cardid;
                                EventNo = 6;
                                //setLog(DeviceID, EventNo, EventStr);

                                //写入报警记录
                                //sql = "insert into RecordAlarm (DeviceID,CarNo, EventNo, Detail, Flag,Address,lng,lat,Blng,Blat) SELECT  '" + DeviceID + "','" + CarNo + "',2,'应急卡施封命令',0 ,Address,Lng,Lat,Blng,Blat FROM         Car  where  DeviceID='" + DeviceID + "'";
                                //sqlCmd(sql, ConnStr);

                                sql = "Insert into RecordControl(  DeviceID, CarNo, UserID, CardID, Control, Address, lng, lat,Blng,Blat)  SELECT   DeviceID, CarNo, '应急卡', '" + cardid + "', '1施封,2施封，3施封，4施封，5施封，6施封', Address, lng, lat,Blng,Blat from Car where  DeviceID='" + DeviceID + "'";
                                sqlCmd(sql, ConnStr);
                            }
                            else
                            {
                                YJCard[x] = 0;

                                byte[] Ps = { 0x0A, 0x0A, 0x0A, 0x0A, 0x0A, 0x0A };
                                byte[] byteTem = comControl(Ps);
                                string strTem = byteToHexStr(byteTem);
                                sql = "insert into DownCom  (DeviceID,CmdStr) values ('" + DeviceID + "','" + strTem + "') ";
                                sqlCmd(sql, ConnStr);

                                EventStr = "应急卡解封命令" + cardid;
                                EventNo = 6;
                                //setLog(DeviceID, EventNo, EventStr);

                                //写入报警记录
                                //sql = "insert into RecordAlarm (DeviceID,CarNo, EventNo, Detail, Flag,Address,lng,lat,Blng,Blat) SELECT  '" + DeviceID + "','" + CarNo + "',2,'应急卡解封命令',0 ,Address,Lng,Lat,Blng,Blat FROM         Car  where  DeviceID='" + DeviceID + "'";
                                //sqlCmd(sql, ConnStr);

                                sql = "Insert into RecordControl(  DeviceID, CarNo, UserID, CardID, Control, Address, lng, lat,Blng,Blat)  SELECT   DeviceID, CarNo, '应急卡', '" + cardid + "', '1解封,2解封，3解封，4解封，5解封，6解封', Address, lng, lat,Blng,Blat from Car where  DeviceID='" + DeviceID + "'";
                                sqlCmd(sql, ConnStr);
                            }
                            startRead(DeviceID);

                        }
                    }
                }
            }
            {
                //处理普通卡
                connNO = connNO.Replace(" ", "");
                cardid = cardid.Replace(" ", "");
                string DeviceID = "";
                string SiteIDNow = "";
                string P1 = "";
                string P2 = "";
                string P3 = "";
                string P4 = "";
                string P5 = "";
                string P6 = "";
                string sql = "SELECT     DeviceID, CarNo, CardID, SimNo, P1, P2, P3, P4, P5, P6, SiteIDNow  FROM         Car " +
                            "where DeviceID = '" + connNO + "'";
                DataTable dtTem = sqlRead(sql, ConnStr);
                for (int i = 0; i < dtTem.Rows.Count; i++)
                {
                    DeviceID = dtTem.Rows[i]["DeviceID"].ToString();
                    SiteIDNow = dtTem.Rows[i]["SiteIDNow"].ToString();
                    P1 = dtTem.Rows[i]["P1"].ToString();
                    P2 = dtTem.Rows[i]["P2"].ToString();
                    P3 = dtTem.Rows[i]["P3"].ToString();
                    P4 = dtTem.Rows[i]["P4"].ToString();
                    P5 = dtTem.Rows[i]["P5"].ToString();
                    P6 = dtTem.Rows[i]["P6"].ToString();
                }
                if (DeviceID.Length > 0)
                {
                    //checkCarArea(DeviceID);//通过百度地图确定车辆是否在要配送的站点内
                }
                string SiteID = "";
                
                string Type = "";// 9解封，A施封
                sql = "SELECT     SiteID, Card1, Card2, SiteName  FROM         Site " +
                        "where  Card1='" + cardid + "' or Card2='" + cardid + "'";
                DataTable dtTem2 = sqlRead(sql, ConnStr);
                for (int i2 = 0; i2 < dtTem2.Rows.Count; i2++)
                {
                    SiteID = dtTem2.Rows[i2]["SiteID"].ToString();
                    SiteName = dtTem2.Rows[i2]["SiteName"].ToString();
                    string Card1 = dtTem2.Rows[i2]["Card1"].ToString();
                    string Card2 = dtTem2.Rows[i2]["Card2"].ToString();
                    if (Card1 == cardid)
                    {
                        Type = "9";
                        cardType1 = 2;
                    }
                    else
                    {
                        Type = "A";
                        cardType1 = 3;
                    }
                }
                
                string PortRight = "";
                string ID = "";
                string siteType = "";//1出发站，2目的站
                if (SiteID.Length > 0 && SiteIDNow.Length > 0)
                {
                    if (SiteID == SiteIDNow)
                    {
                        sql = "SELECT     ID, DeviceID, FromSiteID, FromPorts, DirSiteID, DirPorts, Result   FROM         SheetRecord " +
                                    "where Result<2 and DeviceID = '" + DeviceID + "' and(FromSiteID='" + SiteIDNow + "' or DirSiteID='" + SiteIDNow + "') ";
                        DataTable dtTem1 = sqlRead(sql, ConnStr);
                        for (int i = 0; i < dtTem1.Rows.Count; i++)
                        {
                            ID = dtTem1.Rows[i]["ID"].ToString();
                            string FromSiteID = dtTem1.Rows[i]["FromSiteID"].ToString();
                            string FromPorts = dtTem1.Rows[i]["FromPorts"].ToString();
                            string DirSiteID = dtTem1.Rows[i]["DirSiteID"].ToString();
                            string DirPorts = dtTem1.Rows[i]["DirPorts"].ToString();
                            if (FromSiteID == SiteID)
                            {
                                PortRight = FromPorts;
                                siteType = "1";
                            }
                            if (DirSiteID == SiteID)
                            {
                                PortRight = DirPorts;
                                siteType = "2";
                            }

                            if (Type.Length > 0 && PortRight.Length > 0)
                            {
                                cardType = 2;

                                string controlStr = "";
                                byte[] Ps = { 0, 0, 0, 0, 0, 0 };
                                if (PortRight.Substring(0, 1) == "1")
                                {
                                    Ps[0] = (byte)Convert.ToInt32(Type, 16);
                                    if (Type == "9")
                                        controlStr += "1解封,";
                                    else
                                        controlStr += "1施封,";
                                }
                                if (PortRight.Substring(1, 1) == "1")
                                {
                                    Ps[1] = (byte)Convert.ToInt32(Type, 16);
                                    if (Type == "9")
                                        controlStr += "2解封,";
                                    else
                                        controlStr += "2施封,";
                                }
                                if (PortRight.Substring(2, 1) == "1")
                                {
                                    Ps[2] = (byte)Convert.ToInt32(Type, 16);
                                    if (Type == "9")
                                        controlStr += "3解封,";
                                    else
                                        controlStr += "3施封,";
                                }
                                if (PortRight.Substring(3, 1) == "1")
                                {
                                    Ps[3] = (byte)Convert.ToInt32(Type, 16);
                                    if (Type == "9")
                                        controlStr += "4解封,";
                                    else
                                        controlStr += "4施封,";
                                }
                                if (PortRight.Substring(4, 1) == "1")
                                {
                                    Ps[4] = (byte)Convert.ToInt32(Type, 16);
                                    if (Type == "9")
                                        controlStr += "5解封,";
                                    else
                                        controlStr += "5施封,";
                                }
                                if (PortRight.Substring(5, 1) == "1")
                                {
                                    Ps[5] = (byte)Convert.ToInt32(Type, 16);
                                    if (Type == "9")
                                        controlStr += "6解封,";
                                    else
                                        controlStr += "6施封,";
                                }
                                if (controlStr.Length > 0)
                                    controlStr = controlStr.Substring(0, controlStr.Length - 1);

                                byte[] byteTem = comControl(Ps);
                                string strTem = byteToHexStr(byteTem);
                                sql = "insert into DownCom  (DeviceID,CmdStr) values ('" + DeviceID + "','" + strTem + "') ";
                                sqlCmd(sql, ConnStr);

                                if (Type == "A")
                                {
                                    sql = "update SheetRecord set Result=" + siteType + "  where ID='" + ID + "'";
                                    sqlCmd(sql, ConnStr);

                                    EventStr = "刷卡施封命令" + cardid;
                                    EventNo = 6;
                                    //setLog(DeviceID, EventNo, EventStr);

                                    sql = "Insert into RecordControl(  DeviceID, CarNo, UserID, CardID, Control, Address, lng, lat,Blng,Blat)  SELECT   DeviceID, CarNo, '施封卡', '" + cardid + "', '" + controlStr + "', Address, lng, lat,Blng,Blat from Car where  DeviceID='" + DeviceID + "'";
                                    sqlCmd(sql, ConnStr);
                                }
                                if (Type == "9")
                                {
                                    EventStr = "刷卡解封命令" + cardid;
                                    EventNo = 6;
                                    //setLog(DeviceID, EventNo, EventStr);

                                    sql = "Insert into RecordControl(  DeviceID, CarNo, UserID, CardID, Control, Address, lng, lat,Blng,Blat)  SELECT   DeviceID, CarNo, '解封卡', '" + cardid + "', '" + controlStr + "', Address, lng, lat,Blng,Blat from Car where  DeviceID='" + DeviceID + "'";
                                    sqlCmd(sql, ConnStr);
                                }

                                startRead(DeviceID);
                            }
                        }
                    }
                }
            }

            {
                if (cardType <= 0)
                {
                    //无效卡
                    string sql = "SELECT     DeviceID, CarNo, CardID, SimNo  FROM         Car " +
                              "where  DeviceID='" + connNO + "'";
                    DataTable dtTem = sqlRead(sql, ConnStr);
                    for (int i = 0; i < dtTem.Rows.Count; i++)
                    {
                        string DeviceID = dtTem.Rows[i]["DeviceID"].ToString();
                        string CarNo = dtTem.Rows[i]["CarNo"].ToString();

                        byte[] Ps = { 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE };
                        byte[] byteTem = comControl(Ps);
                        string strTem = byteToHexStr(byteTem);
                        sql = "insert into DownCom  (DeviceID,CmdStr) values ('" + DeviceID + "','" + strTem + "') ";
                        sqlCmd(sql, ConnStr);

                        //EventStr = "无效卡" + cardid;
                        //EventNo = 6;

                        //写入报警记录  无效卡
                        //sql = "insert into RecordAlarm (DeviceID,CarNo, EventNo, Detail, Flag,Address,lng,lat,Blng,Blat) SELECT  '" + DeviceID + "','" + CarNo + "',6,'无效卡 " + cardid + "',0 ,Address,Lng,Lat,Blng,Blat FROM         Car  where  DeviceID='" + DeviceID + "'";
                        //sqlCmd(sql, ConnStr);

                        //报警
                        //sql = "update car set Status=4  where DeviceID='" + connNO + "'";
                        //sqlCmd(sql, ConnStr);
                    }
                }
            }

            if (connNO.Length > 0)
            {
                if (cardType1 == 0)
                {
                    string sql = "insert into RecordCard  (DeviceID, CarNo, Detail, SiteID, SiteName, Address, lng, lat, Blng, Blat, DTime)  " +
                        " SELECT     Car.DeviceID, Car.CarNo, '无效卡(" + cardid + ")' AS detail, Site.SiteID, Site.SiteName, Car.Address, Car.Lng, Car.Lat, Car.Blng, Car.Blat, Car.DTime " +
                        " FROM         Car LEFT JOIN " +
                                            "  Site ON Car.SiteIDNow = Site.SiteID " +
                        " WHERE     (Car.DeviceID = '" + connNO + "') ";
                    sqlCmd(sql, ConnStr);
                }
                else if (cardType1 == 1)
                {
                    string sql = "insert into RecordCard  (DeviceID, CarNo, Detail, SiteID, SiteName, Address, lng, lat, Blng, Blat, DTime)  " +
                            " SELECT     Car.DeviceID, Car.CarNo, '应急卡' AS detail, Site.SiteID, Site.SiteName, Car.Address, Car.Lng, Car.Lat, Car.Blng, Car.Blat, Car.DTime " +
                            " FROM         Car LEFT JOIN " +
                                                "  Site ON Car.SiteIDNow = Site.SiteID " +
                            " WHERE     (Car.DeviceID = '" + connNO + "') ";
                    sqlCmd(sql, ConnStr);
                }
                else if (cardType1 == 2)
                {
                    string sql = "insert into RecordCard  (DeviceID, CarNo, Detail, SiteID, SiteName, Address, lng, lat, Blng, Blat, DTime)  " +
                            " SELECT     Car.DeviceID, Car.CarNo, '解封卡(" + SiteName + ")' AS detail, Site.SiteID, Site.SiteName, Car.Address, Car.Lng, Car.Lat, Car.Blng, Car.Blat, Car.DTime " +
                            " FROM         Car LEFT JOIN " +
                                                "  Site ON Car.SiteIDNow = Site.SiteID " +
                            " WHERE     (Car.DeviceID = '" + connNO + "') ";
                    sqlCmd(sql, ConnStr);
                }
                else if (cardType1 == 3)
                {
                    string sql = "insert into RecordCard  (DeviceID, CarNo, Detail, SiteID, SiteName, Address, lng, lat, Blng, Blat, DTime)  " +
                            " SELECT     Car.DeviceID, Car.CarNo, '施封卡(" + SiteName + ")' AS detail, Site.SiteID, Site.SiteName, Car.Address, Car.Lng, Car.Lat, Car.Blng, Car.Blat, Car.DTime " +
                            " FROM         Car LEFT JOIN " +
                                                "  Site ON Car.SiteIDNow = Site.SiteID " +
                            " WHERE     (Car.DeviceID = '" + connNO + "') ";
                    sqlCmd(sql, ConnStr);
                }
            }
        }
        //下发终端应急卡
        public void SendDeviceCard(string connNO)
        {
            string sql = "SELECT     DeviceID, CarNo, CardID, SimNo   FROM         Car " +
             "where DeviceID ='" + connNO + "' ";
            DataTable dtTem1 = sqlRead(sql, ConnStr);
            for (int i = 0; i < dtTem1.Rows.Count; i++)
            {
                string DeviceID = dtTem1.Rows[i]["DeviceID"].ToString();
                string CardID = dtTem1.Rows[i]["CardID"].ToString();
                CardID = CardID.Replace(" ","");
                CardID = "0000000000000000" + CardID;
                CardID = CardID.Substring(CardID.Length - 16, 16);
                byte[] byteTem = comSendCard(0, CardID);
                string strTem = byteToHexStr(byteTem);
                sql = "insert into DownCom  (DeviceID,CmdStr) values ('" + DeviceID + "','" + strTem + "') ";
                sqlCmd(sql, ConnStr);  
            }
        }
        //下发上位机对日期时间指令
        public void SendDeviceTime(string connNO)
        {
            string sql = "SELECT     DeviceID, CarNo, CardID, SimNo   FROM         Car " +
             "where DeviceID ='" + connNO + "' ";
            DataTable dtTem1 = sqlRead(sql, ConnStr);
            for (int i = 0; i < dtTem1.Rows.Count; i++)
            {
                string DeviceID = dtTem1.Rows[i]["DeviceID"].ToString();
                string CardID = dtTem1.Rows[i]["CardID"].ToString();
                CardID = CardID.Replace(" ", "");
                CardID = "0000000000000000" + CardID;
                CardID = CardID.Substring(CardID.Length - 16, 16);
                byte[] byteTem = comSendTime();
                string strTem = byteToHexStr(byteTem);
                sql = "insert into DownCom  (DeviceID,CmdStr) values ('" + DeviceID + "','" + strTem + "') ";
                sqlCmd(sql, ConnStr);
            }
        }


        //通过百度地图比对，车辆是否进入要配送的区域
        public void checkCarArea(string DeviceID)
        {
            try
            {
                string lng = "";
                string lat = "";
                string sql = "SELECT     DeviceID, Blng, Blat  FROM         Car " +
                    " where DeviceID='" + DeviceID + "'";
                DataTable dtTem1 = sqlRead(sql, ConnStr);
                for (int i1 = 0; i1 < dtTem1.Rows.Count; i1++)
                {
                    lng = dtTem1.Rows[i1]["Blng"].ToString();
                    lat = dtTem1.Rows[i1]["Blat"].ToString();
                }
                if (lng.Length > 0 && lat.Length > 0)
                {
                    sql = "SELECT     SheetRecord.ID, SheetRecord.DeviceID, SheetRecord.FromSiteID, SheetRecord.DirSiteID, Site.AreaType AS AreaType1, Site.Radius AS Radius1,                      Site_1.AreaType AS AreaType2, Site_1.Radius AS Radius2  FROM         SheetRecord INNER JOIN             Site ON SheetRecord.FromSiteID = Site.SiteID INNER JOIN               Site AS Site_1 ON SheetRecord.DirSiteID = Site_1.SiteID " +
                        " where SheetRecord.DeviceID='" + DeviceID + "' and SheetRecord.Result<2";
                    DataTable dtTem = sqlRead(sql, ConnStr);
                    for (int i = 0; i < dtTem.Rows.Count; i++)
                    {
                        string FromSiteID = dtTem.Rows[i]["FromSiteID"].ToString();
                        string AreaType1 = dtTem.Rows[i]["AreaType1"].ToString();
                        string Radius1 = dtTem.Rows[i]["Radius1"].ToString();
                        string DirSiteID = dtTem.Rows[i]["DirSiteID"].ToString();
                        string AreaType2 = dtTem.Rows[i]["AreaType2"].ToString();
                        string Radius2 = dtTem.Rows[i]["Radius2"].ToString();
                        if (FromSiteID.Length > 0)
                        {
                            string sql21 = "SELECT   SiteID,  PointID, BLng, BLat  FROM         Points " +
                                " where SiteID='" + FromSiteID + "'";
                            DataTable dtTem21 = sqlRead(sql21, ConnStr);
                            string tem = "";
                            for (int i21 = 0; i21 < dtTem21.Rows.Count; i21++)
                            {
                                string Lng = dtTem21.Rows[i21]["BLng"].ToString();
                                string Lat = dtTem21.Rows[i21]["BLat"].ToString();
                                tem += Lng + "," + Lat + ";";
                            }

                            string sendStr1 = DeviceID + "," + FromSiteID;
                            string sendStr2 = AreaType1 + "," + Radius1 + "$";
                            sendStr2 += tem;//
                            object[] objects = new object[4];
                            objects[0] = sendStr1;//deviceID ， SiteID
                            objects[1] = sendStr2;
                            objects[2] = lng;
                            objects[3] = lat;
                            webBrowser1.Document.InvokeScript("checkPoint", objects);
                        }
                        if (DirSiteID.Length > 0)
                        {
                            string sql21 = "SELECT   SiteID,  PointID, BLng, BLat  FROM         Points " +
                                " where SiteID='" + DirSiteID + "'";
                            DataTable dtTem21 = sqlRead(sql21, ConnStr);
                            string tem = "";
                            for (int i21 = 0; i21 < dtTem21.Rows.Count; i21++)
                            {
                                string Lng = dtTem21.Rows[i21]["BLng"].ToString();
                                string Lat = dtTem21.Rows[i21]["BLat"].ToString();
                                tem += Lng + "," + Lat + ";";
                            }

                            string sendStr1 = DeviceID + "," + DirSiteID;
                            string sendStr2 = AreaType2 + "," + Radius2 + "$";
                            sendStr2 += tem;//
                            object[] objects = new object[4];
                            objects[0] = sendStr1;//deviceID ， SiteID
                            objects[1] = sendStr2;
                            objects[2] = lng;
                            objects[3] = lat;
                            webBrowser1.Document.InvokeScript("checkPoint", objects);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public void reCheckPoint(string str, string re)//deviceID ， SiteID
        {
            try
            {
                string DeviceID = "";
                string SiteID = "";
                string[] tems = str.Split(',');
                if (tems.Length == 2)
                {
                    DeviceID = tems[0];
                    SiteID = tems[1];
                }
                if (re == "1")//刷卡与站点匹配成功
                {
                    string sql = "SELECT     DeviceID, SiteIDNow  FROM         Car " +
                             " where DeviceID='" + DeviceID + "'";
                    DataTable dtTem = sqlRead(sql, ConnStr);
                    for (int i = 0; i < dtTem.Rows.Count; i++)
                    {
                        string SiteIDNow = dtTem.Rows[i]["SiteIDNow"].ToString();
                        if (SiteID != SiteIDNow)
                        {
                            //进入区域
                            sql = "update Car set SiteIDNow=" + SiteID + " " +
                                  " where DeviceID='" + DeviceID + "'";
                            sqlCmd(sql, ConnStr);
                        }
                    }
                }
                if (re == "0")//刷卡与站点匹配失败
                {
                    string sql = "SELECT     DeviceID, SiteIDNow,P1,P2,P3,P4,P5,P6,CarNo  FROM         Car " +
                             " where DeviceID='" + DeviceID + "'";
                    DataTable dtTem = sqlRead(sql, ConnStr);
                    for (int i = 0; i < dtTem.Rows.Count; i++)
                    {
                        string SiteIDNow = dtTem.Rows[i]["SiteIDNow"].ToString();
                        string P1 = dtTem.Rows[i]["P1"].ToString();
                        string P2 = dtTem.Rows[i]["P2"].ToString();
                        string P3 = dtTem.Rows[i]["P3"].ToString();
                        string P4 = dtTem.Rows[i]["P4"].ToString();
                        string P5 = dtTem.Rows[i]["P5"].ToString();
                        string P6 = dtTem.Rows[i]["P6"].ToString();
                        string CarNo = dtTem.Rows[i]["CarNo"].ToString();
                        if (SiteID == SiteIDNow)
                        {
                            //区域，离开区域
                            sql = "update Car set SiteIDNow=0 " +
                                " where DeviceID='" + DeviceID + "'";
                            sqlCmd(sql, ConnStr);

                            string temStr = "";
                            if (P1 == "00")
                                temStr += "1锁,";
                            if (P2 == "00")
                                temStr += "2锁,";
                            if (P3 == "00")
                                temStr += "3锁,";
                            if (P4 == "00")
                                temStr += "4锁,";
                            if (P5 == "00")
                                temStr += "5锁,";
                            if (P6 == "00")
                                temStr += "6锁,";
                            if (temStr.Length > 0)
                                temStr = temStr.Substring(0, temStr.Length - 1);
                            if ((P1 != "00" && P1 != "08" && P1 != "0A" && P1 != "0B" && P1 != "CC") || (P2 != "00" && P2 != "08" && P2 != "0A" && P2 != "0B" && P2 != "CC") || (P3 != "00" && P3 != "08" && P3 != "0A" && P3 != "0B" && P3 != "CC") || (P4 != "00" && P4 != "08" && P4 != "0A" && P4 != "0B" && P4 != "CC") || (P5 != "00" && P5 != "08" && P5 != "0A" && P5 != "0B" && P5 != "CC") || (P6 != "00" && P6 != "08" && P6 != "0A" && P6 != "0B" && P6 != "CC"))
                            {
                                //写入报警，油口未关闭，离开区域
                                sql = "insert into RecordAlarm (DeviceID,CarNo, EventNo, Detail, Flag,Address,lng,lat,Blng,Blat) SELECT  '" + DeviceID + "','" + CarNo + "',7,'油口未关闭，离开区域(" + temStr + ")',0 ,Address,Lng,Lat,Blng,Blat FROM         Car  where  DeviceID='" + DeviceID + "'";
                                sqlCmd(sql, ConnStr);

                                //报警
                                sql = "update car set Status=4  where DeviceID='" + DeviceID + "'";
                                sqlCmd(sql, ConnStr);
                            }
                        }

                    }
                }

            }
            catch
            { }
        }
        int transInt = 0;
        private void timerTranslate_Tick(object sender, EventArgs e)
        {
            try
            {
                Translate();
                transInt++;
                if (transInt > 1000)
                {
                    transInt = 0;
                    try
                    {
                        timerTranslate.Enabled = false;

                        webBrowser1.ObjectForScripting = this;//具体公开的对象,这里可以公开自定义对象
                        webBrowser1.ScriptErrorsSuppressed = true; //禁用错误脚本提示
                        webBrowser1.IsWebBrowserContextMenuEnabled = false; //禁用右键菜单
                        webBrowser1.WebBrowserShortcutsEnabled = false; //禁用快捷键
                        string urlPath = Path.GetFullPath(PathStr + "/baidu.html").Replace(Path.DirectorySeparatorChar, '/');
                        Uri url = new Uri(urlPath);//本来str_url是个地址
                        webBrowser1.Url = url;
                    }
                    catch
                    { }
                }
            }
            catch
            { }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            timerTranslate.Enabled = true;
        }

        private void lab_Title_Click(object sender, EventArgs e)
        {

        }

        public void autoDealAlarm(int x, string lastTem, string connNO)
        {
            try
            {
                string sql = "";
                //00阀锁关闭;01开阀抖动;02解封开锁;03检测锁开;04锁开阀关;
                //05锁开阀开;06关阀抖动;07非法开阀;08非法阀关;09电磁阀抖动;
                //0A电磁阀故障;0B电磁阀故障;CC未安装;
                if (lastTem == "07")
                    sql = "Update RecordAlarm set Flag=1,DealTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',UserID='自动处理' where id in (select max(id) from RecordAlarm where  DeviceID='" + connNO + "' and Flag=0 and "
                        + " Detail like '%非法开阀 （" + (x + 1).ToString() + "锁）%') ";
                if (lastTem == "08")
                    sql = "Update RecordAlarm set Flag=1,DealTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',UserID='自动处理' where id in (select max(id) from RecordAlarm where  DeviceID='" + connNO + "' and Flag=0 and "
                        + " Detail like '%非法开阀关闭 （" + (x + 1).ToString() + "锁）%') ";
                sqlCmd(sql, ConnStr);
                if (lastTem == "0A")
                    sql = "Update RecordAlarm set Flag=1,DealTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',UserID='自动处理' where id in (select max(id) from RecordAlarm where  DeviceID='" + connNO + "' and Flag=0 and "
                        + " Detail like '%电磁阀故障0A （" + (x + 1).ToString() + "锁）%') ";
                sqlCmd(sql, ConnStr);
                if (lastTem == "0B")
                    sql = "Update RecordAlarm set Flag=1,DealTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',UserID='自动处理' where id in (select max(id) from RecordAlarm where  DeviceID='" + connNO + "' and Flag=0 and "
                        + " Detail like '%电磁阀故障0B （" + (x + 1).ToString() + "锁）%') ";
                sqlCmd(sql, ConnStr);
            }
            catch
            { }
        }
        public void checkPort(byte[] PortB, string connNO, int index, string time)
        {
            try
            {
                for (int i = 0; i < 6; i++)
                {
                    string Port = PortB[i].ToString("X2");
                    if (Port.Length > 0)
                    {
                        if (i == 0)
                        {
                            if (P1[index] != Port)
                            {
                                string lastTem = P1[index];
                                autoDealAlarm(i, lastTem, connNO);

                                P1[index] = Port;
                                checkPtOpenAlarm(PortB[i], connNO, i, time);

                                string sql = "insert into RecordPorts(DeviceID, CarNo, Port, Status, Address, lng, lat,Blng,Blat, DTime) SELECT  DeviceID,CarNo," + (i + 1).ToString() + ",'" + Port + "',Address,Lng,Lat,Blng,Blat,'" + time + "'  FROM         Car where  DeviceID ='" + connNO + "'";
                                sqlCmd(sql, ConnStr);

                                sql = "update Car set P1='" + P1[index] + "' where DeviceID='" + connNO + "'";
                                sqlCmd(sql, ConnStr);
                            }
                        }
                        if (i == 1)
                        {
                            if (P2[index] != Port)
                            {
                                string lastTem = P1[index];
                                autoDealAlarm(i, lastTem, connNO);

                                P2[index] = Port;
                                checkPtOpenAlarm(PortB[i], connNO, i, time);

                                string sql = "insert into RecordPorts(DeviceID, CarNo, Port, Status, Address, lng, lat,Blng,Blat, DTime) SELECT  DeviceID,CarNo," + (i + 1).ToString() + ",'" + Port + "',Address,Lng,Lat,Blng,Blat,'" + time + "'  FROM         Car where  DeviceID ='" + connNO + "'";
                                sqlCmd(sql, ConnStr);

                                sql = "update Car set P2='" + P2[index] + "' where DeviceID='" + connNO + "'";
                                sqlCmd(sql, ConnStr);
                            }
                        }
                        if (i == 2)
                        {
                            if (P3[index] != Port)
                            {
                                string lastTem = P1[index];
                                autoDealAlarm(i, lastTem, connNO);

                                P3[index] = Port;
                                checkPtOpenAlarm(PortB[i], connNO, i, time);

                                string sql = "insert into RecordPorts(DeviceID, CarNo, Port, Status, Address, lng, lat,Blng,Blat, DTime) SELECT  DeviceID,CarNo," + (i + 1).ToString() + ",'" + Port + "',Address,Lng,Lat,Blng,Blat,'" + time + "'  FROM         Car where  DeviceID ='" + connNO + "'";
                                sqlCmd(sql, ConnStr);

                                sql = "update Car set P3='" + P3[index] + "' where DeviceID='" + connNO + "'";
                                sqlCmd(sql, ConnStr);
                            }
                        }
                        if (i == 3)
                        {
                            if (P4[index] != Port)
                            {
                                string lastTem = P1[index];
                                autoDealAlarm(i, lastTem, connNO);

                                P4[index] = Port;
                                checkPtOpenAlarm(PortB[i], connNO, i, time);

                                string sql = "insert into RecordPorts(DeviceID, CarNo, Port, Status, Address, lng, lat,Blng,Blat, DTime) SELECT  DeviceID,CarNo," + (i + 1).ToString() + ",'" + Port + "',Address,Lng,Lat,Blng,Blat,'" + time + "'  FROM         Car where  DeviceID ='" + connNO + "'";
                                sqlCmd(sql, ConnStr);

                                sql = "update Car set P4='" + P4[index] + "' where DeviceID='" + connNO + "'";
                                sqlCmd(sql, ConnStr);
                            }
                        }
                        if (i == 4)
                        {
                            if (P5[index] != Port)
                            {
                                string lastTem = P1[index];
                                autoDealAlarm(i, lastTem, connNO);

                                P5[index] = Port;
                                checkPtOpenAlarm(PortB[i], connNO, i, time);

                                string sql = "insert into RecordPorts(DeviceID, CarNo, Port, Status, Address, lng, lat,Blng,Blat, DTime) SELECT  DeviceID,CarNo," + (i + 1).ToString() + ",'" + Port + "',Address,Lng,Lat,Blng,Blat,'" + time + "'  FROM         Car where  DeviceID ='" + connNO + "'";
                                sqlCmd(sql, ConnStr);

                                sql = "update Car set P5='" + P5[index] + "' where DeviceID='" + connNO + "'";
                                sqlCmd(sql, ConnStr);
                            }
                        }
                        if (i == 5)
                        {
                            if (P6[index] != Port)
                            {
                                string lastTem = P1[index];
                                autoDealAlarm(i, lastTem, connNO);

                                P6[index] = Port;
                                checkPtOpenAlarm(PortB[i], connNO, i, time);

                                string sql = "insert into RecordPorts(DeviceID, CarNo, Port, Status, Address, lng, lat,Blng,Blat, DTime) SELECT  DeviceID,CarNo," + (i + 1).ToString() + ",'" + Port + "',Address,Lng,Lat,Blng,Blat,'" + time + "'  FROM         Car where  DeviceID ='" + connNO + "'";
                                sqlCmd(sql, ConnStr);

                                sql = "update Car set P6='" + P6[index] + "' where DeviceID='" + connNO + "'";
                                sqlCmd(sql, ConnStr);
                            }
                        }
                    }
                }
            }
            catch
            { }
        }
        public void checkPtOpenAlarm(byte PortB, string connNO, int Pindex, string time)
        {
            try
            {
                switch (PortB)
                {
                    case 0x07://非法开阀
                        {
                            //报警
                            string sql = "update car set Status=4  where DeviceID='" + connNO + "'";
                            sqlCmd(sql, ConnStr);

                            //写入报警记录
                            sql = "insert into RecordAlarm (DeviceID,CarNo, EventNo, Detail, Flag,Address,lng,lat,Blng,Blat,CreateTime) SELECT  DeviceID,CarNo,8,'非法开阀 （" + (Pindex + 1).ToString() + "锁）',0 ,Address,Lng,Lat,Blng,Blat,'" + time + "'  FROM         Car where  DeviceID ='" + connNO + "'";
                            sqlCmd(sql, ConnStr);
                            break;
                        }
                    case 0x08://非法开阀关闭
                        {
                            //报警
                            string sql = "update car set Status=4  where DeviceID='" + connNO + "'";
                            sqlCmd(sql, ConnStr);

                            //写入报警记录
                            sql = "insert into RecordAlarm (DeviceID,CarNo, EventNo, Detail, Flag,Address,lng,lat,Blng,Blat,CreateTime) SELECT  DeviceID,CarNo,9,'非法开阀关闭 （" + (Pindex + 1).ToString() + "锁）',0 ,Address,Lng,Lat,Blng,Blat,'" + time + "'  FROM         Car where  DeviceID ='" + connNO + "'";
                            sqlCmd(sql, ConnStr);
                            break;
                        }
                    case 0x0A://电磁阀故障
                        {
                            //报警
                            string sql = "update car set Status=4  where DeviceID='" + connNO + "'";
                            sqlCmd(sql, ConnStr);

                            //写入报警记录
                            sql = "insert into RecordAlarm (DeviceID,CarNo, EventNo, Detail, Flag,Address,lng,lat,Blng,Blat,CreateTime) SELECT  DeviceID,CarNo,10,'电磁阀故障0A （" + (Pindex + 1).ToString() + "锁）',0 ,Address,Lng,Lat,Blng,Blat,'" + time + "'   FROM         Car where  DeviceID ='" + connNO + "'";
                            sqlCmd(sql, ConnStr);
                            break;
                        }
                    case 0x0B://电磁阀故障
                        {
                            //报警
                            string sql = "update car set Status=4  where DeviceID='" + connNO + "'";
                            sqlCmd(sql, ConnStr);

                            //写入报警记录
                            sql = "insert into RecordAlarm (DeviceID,CarNo, EventNo, Detail, Flag,Address,lng,lat,Blng,Blat,CreateTime) SELECT  DeviceID,CarNo,10,'电磁阀故障0B （" + (Pindex + 1).ToString() + "锁）',0 ,Address,Lng,Lat,Blng,Blat,'" + time + "'   FROM         Car where  DeviceID ='" + connNO + "'";
                            sqlCmd(sql, ConnStr);
                            break;
                        }
                    default:
                        break;
                }
            }
            catch
            { }
        }

        private void btn_Gps_Click(object sender, EventArgs e)
        {
            if (lab_DeviceID.Text != "DeviceID")
            {
                frmGps.lab_DeviceID.Text = lab_DeviceID.Text;
                frmGps.lab_CarNo.Text = lab_CarNo.Text;
                frmGps.lab_Sim.Text = lab_Phone.Text;
                //frmGps.ShowDialog(this);

                FormPwd frmPwd = new FormPwd();
                frmPwd.nextFrm = "FormGps";
                frmPwd.Show(this);
            }
            else
            {
                MessageBox.Show("请选择车辆！");
            }
        }

        private void btn_Ports_Click(object sender, EventArgs e)
        {
            if (lab_DeviceID.Text != "DeviceID")
            {
                frmControl.lab_DeviceID.Text = lab_DeviceID.Text;
                frmControl.lab_CarNo.Text = lab_CarNo.Text;
                frmControl.lab_Sim.Text = lab_Phone.Text;
                //frmControl.ShowDialog(this);

                FormPwd frmPwd = new FormPwd();
                frmPwd.nextFrm = "FormControl";
                frmPwd.Show(this);
            }
            else
            {
                MessageBox.Show("请选择车辆！");
            }
        }

        public string LockStatus(string str)
        {
            string re = "";
            if (str == "00")
                re = "阀锁关闭";
            else if (str == "01")
                re = "开阀抖动";
            else if (str == "02")
                re = "解封开锁";
            else if (str == "03")
                re = "检测锁开";
            else if (str == "04")
                re = "锁开阀关";
            else if (str == "05")
                re = "锁开阀开";
            else if (str == "06")
                re = "关阀抖动";
            else if (str == "07")
                re = "非法开阀";
            else if (str == "08")
                re = "非法阀关";
            else if (str == "09")
                re = "电磁阀抖动";
            else if (str == "0A")
                re = "电磁阀故障";
            else if (str == "0B")
                re = "电磁阀故障";
            else if (str == "CC")
                re = "未安装";
            return re;
        }

        public void startRead(string id)
        {
            readTimes = 0;
            readDevice = id;
            timerRead.Enabled = true;
        }

        int readTimes = 0;
        string readDevice = "";
        private void timerRead_Tick(object sender, EventArgs e)
        {
            readTimes++;
            if (readDevice.Length > 0)
            {
                byte[] byteTem = comRead();
                string strTem = byteToHexStr(byteTem);
                string sqlStr = "insert into DownCom  (DeviceID,CmdStr) values ('" + readDevice + "','" + strTem + "') ";
                sqlCmd(sqlStr, ConnStr);
            }

            if (readTimes > 10)//5秒读一次，连续读取十次
            {
                timerRead.Enabled = false;
            }
        }


        public void setRecord(string sqlInsert)
        {
            try
            {
                DateTime timeTem = DateTime.Now;
                string database = timeTem.ToString("yyyy_MM");
                string table = timeTem.ToString("dd");
                string sqlTem ="IF not EXISTS (SELECT name FROM sys.databases WHERE name = N'" + database + "')  " +
                            "CREATE DATABASE [" + database + "]    ";
                string connStrTem = ConnStr.Replace(basic.Database, "master");
                sqlCmd(sqlTem, connStrTem);

                sqlTem = "IF not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + table + "]') AND type in (N'U')) " +
                    " CREATE TABLE [dbo].[" + table + "]( " +
                    " [ID] [bigint] IDENTITY(1,1) NOT NULL , " +
                    " [DeviceID] [nvarchar](20) NULL, " +
                    " [SimNo] [nvarchar](20) NULL, " +
                    " [CarNo] [nvarchar](30) NULL, " +
                    " [Speed] [int] NULL DEFAULT ((0)), " +
                    " [lng] [decimal](13, 10) NULL, " +
                    " [lat] [decimal](13, 10) NULL, " +
                    " [Blng] [decimal](13, 10) NULL, " +
                    " [Blat] [decimal](13, 10) NULL, " +
                    " [Direction] [float] NULL, " +
                    " [Address] [nvarchar](100) NULL, " +
                    " [DTime] [nvarchar](20) NULL, " +
                    " [RevStr] [nvarchar](500) NULL, " +
                    " [ComStr] [nvarchar](200) NULL, " +
                    " [RevTime] [datetime] NULL DEFAULT (getdate()), " +
                    " [P1] [nvarchar](10) NULL, " +
                    " [P2] [nvarchar](10) NULL, " +
                    " [P3] [nvarchar](10) NULL, " +
                    " [P4] [nvarchar](10) NULL, " +
                    " [P5] [nvarchar](10) NULL, " +
                    " [P6] [nvarchar](10) NULL, " +
                    " [Wd] [nvarchar](20) NULL, " +
                    " [Sd] [nvarchar](20) NULL, " +
                    " [Status] [smallint] NULL DEFAULT ((0)) , " +
                    " PRIMARY KEY (ID) " +
                    " ) ";
                connStrTem = ConnStr.Replace(basic.Database, database);
                sqlCmd(sqlTem, connStrTem);

                sqlInsert = sqlInsert.Replace("RecordCarPosition", "[" + database + "]..[" + table + "]");
                sqlCmd(sqlInsert, ConnStr);
            }
            catch
            { }
        }

        private void storeCarPosition()
        {
            while (true)
            {
                try
                {
                    string sql = "SELECT  DeviceID, DTime, Status FROM  Car";
                    DataTable dtTem = sqlRead(sql, ConnStr);
                    for (int i = 0; i < dtTem.Rows.Count; i++)
                    {
                        string DeviceID = dtTem.Rows[i]["DeviceID"].ToString();
                        string DTime = dtTem.Rows[i]["DTime"].ToString();
                        string Status = dtTem.Rows[i]["Status"].ToString();
                        sql = "SELECT DeviceID, DTime, Status FROM  RecordCarPosition where  (id IN           (SELECT     MAX(id) AS ID      FROM          RecordCarPosition  where DeviceID='" + DeviceID + "' ))";
                        DataTable dtTem1 = sqlRead(sql, ConnStr);
                        string DTime1 = "";
                        string Status1 = "";
                        for (int j = 0; j < dtTem1.Rows.Count; j++)
                        {
                            DTime1 = dtTem1.Rows[j]["DTime"].ToString();
                            Status1 = dtTem1.Rows[j]["Status"].ToString();
                        }
                        if (DateTime.Parse( DTime1) != DateTime.Parse( DTime) || Status1 != Status)//当记录变化时才保存，排除离线情况重复数据
                        {
                            sql = "Insert into RecordCarPosition( DeviceID, CarNo, Speed, Address, lng, lat,Blng,Blat, DTime,P1,P2,P3,P4,P5,P6, Direction, Status)  SELECT   DeviceID, CarNo, Speed, Address, lng, lat,Blng,Blat, DTime,P1,P2,P3,P4,P5,P6, Direction, Status from Car where DeviceID='" + DeviceID + "'";
                            sqlCmd(sql, ConnStr);
                            setRecord(sql);
                        }
                    }
                }
                catch
                { }
                Thread.Sleep(60000);//每60秒保存一次
            }
        }

    }
}

