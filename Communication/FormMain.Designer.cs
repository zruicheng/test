namespace Communication
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lab_Title = new System.Windows.Forms.Label();
            this.btn_Exit = new System.Windows.Forms.Button();
            this.btn_Log = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.treeView_All = new System.Windows.Forms.TreeView();
            this.imageList_Tree = new System.Windows.Forms.ImageList(this.components);
            this.lab_TitleR = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.lab_Phone = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lab_DeviceID = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.lab_CarNo = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lab_LocalPort = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lab_LocalIP = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btn_Ports = new System.Windows.Forms.Button();
            this.btn_Gps = new System.Windows.Forms.Button();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.dataGridView_DeviceList = new System.Windows.Forms.DataGridView();
            this.终端号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.车牌号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sim卡号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.卫星时间 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.经度 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.纬度 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.速度 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.连接 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.方向 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.高程 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.油口 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.报警标志 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.原始接收 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.透传接收 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.状态 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkBox_GoRevLine = new System.Windows.Forms.CheckBox();
            this.lab_RevCount = new System.Windows.Forms.Label();
            this.lab_TitleL = new System.Windows.Forms.Label();
            this.picB_Logo = new System.Windows.Forms.PictureBox();
            this.dataGridView_Log = new System.Windows.Forms.DataGridView();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerCheckConn = new System.Windows.Forms.Timer(this.components);
            this.timerCom = new System.Windows.Forms.Timer(this.components);
            this.timerSheetOut = new System.Windows.Forms.Timer(this.components);
            this.timerTranslate = new System.Windows.Forms.Timer(this.components);
            this.lab_Time = new System.Windows.Forms.Label();
            this.timerRead = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_DeviceList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picB_Logo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Log)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 397);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(942, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(73, 17);
            this.toolStripStatusLabel1.Text = "ConnStatus";
            // 
            // lab_Title
            // 
            this.lab_Title.BackColor = System.Drawing.Color.LightSteelBlue;
            this.lab_Title.Dock = System.Windows.Forms.DockStyle.Top;
            this.lab_Title.Font = new System.Drawing.Font("华文楷体", 21.75F, System.Drawing.FontStyle.Bold);
            this.lab_Title.ForeColor = System.Drawing.Color.Crimson;
            this.lab_Title.Location = new System.Drawing.Point(0, 0);
            this.lab_Title.Name = "lab_Title";
            this.lab_Title.Size = new System.Drawing.Size(942, 50);
            this.lab_Title.TabIndex = 13;
            this.lab_Title.Text = "车辆管理系统——通信服务器  ";
            this.lab_Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lab_Title.Click += new System.EventHandler(this.lab_Title_Click);
            // 
            // btn_Exit
            // 
            this.btn_Exit.Location = new System.Drawing.Point(93, 19);
            this.btn_Exit.Name = "btn_Exit";
            this.btn_Exit.Size = new System.Drawing.Size(75, 23);
            this.btn_Exit.TabIndex = 17;
            this.btn_Exit.Text = "关闭程序";
            this.btn_Exit.UseVisualStyleBackColor = true;
            this.btn_Exit.Click += new System.EventHandler(this.btn_Exit_Click);
            // 
            // btn_Log
            // 
            this.btn_Log.Location = new System.Drawing.Point(12, 19);
            this.btn_Log.Name = "btn_Log";
            this.btn_Log.Size = new System.Drawing.Size(75, 23);
            this.btn_Log.TabIndex = 16;
            this.btn_Log.Text = "刷新列表";
            this.btn_Log.UseVisualStyleBackColor = true;
            this.btn_Log.Click += new System.EventHandler(this.btn_Log_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 50);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(942, 347);
            this.splitContainer1.SplitterDistance = 260;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 18;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer2.Panel1.Controls.Add(this.treeView_All);
            this.splitContainer2.Panel1.Controls.Add(this.lab_TitleR);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer2.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer2.Size = new System.Drawing.Size(260, 347);
            this.splitContainer2.SplitterDistance = 257;
            this.splitContainer2.TabIndex = 0;
            // 
            // treeView_All
            // 
            this.treeView_All.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_All.Font = new System.Drawing.Font("宋体", 11F);
            this.treeView_All.ImageIndex = 0;
            this.treeView_All.ImageList = this.imageList_Tree;
            this.treeView_All.Location = new System.Drawing.Point(0, 23);
            this.treeView_All.Name = "treeView_All";
            this.treeView_All.SelectedImageIndex = 0;
            this.treeView_All.Size = new System.Drawing.Size(258, 232);
            this.treeView_All.TabIndex = 3;
            this.treeView_All.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_All_AfterSelect);
            this.treeView_All.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_All_NodeMouseClick);
            // 
            // imageList_Tree
            // 
            this.imageList_Tree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_Tree.ImageStream")));
            this.imageList_Tree.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_Tree.Images.SetKeyName(0, "0.ico");
            this.imageList_Tree.Images.SetKeyName(1, "1.ico");
            this.imageList_Tree.Images.SetKeyName(2, "2.ico");
            // 
            // lab_TitleR
            // 
            this.lab_TitleR.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.lab_TitleR.Dock = System.Windows.Forms.DockStyle.Top;
            this.lab_TitleR.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_TitleR.Location = new System.Drawing.Point(0, 0);
            this.lab_TitleR.Name = "lab_TitleR";
            this.lab_TitleR.Size = new System.Drawing.Size(258, 23);
            this.lab_TitleR.TabIndex = 2;
            this.lab_TitleR.Text = "组织架构表";
            this.lab_TitleR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(258, 84);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.AutoScroll = true;
            this.tabPage3.Controls.Add(this.lab_Phone);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Controls.Add(this.lab_DeviceID);
            this.tabPage3.Controls.Add(this.label16);
            this.tabPage3.Controls.Add(this.lab_CarNo);
            this.tabPage3.Controls.Add(this.label12);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(250, 58);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "车辆参数";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // lab_Phone
            // 
            this.lab_Phone.AutoSize = true;
            this.lab_Phone.Location = new System.Drawing.Point(116, 89);
            this.lab_Phone.Name = "lab_Phone";
            this.lab_Phone.Size = new System.Drawing.Size(35, 12);
            this.lab_Phone.TabIndex = 46;
            this.lab_Phone.Text = "Phone";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(47, 89);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 12);
            this.label8.TabIndex = 45;
            this.label8.Text = "SIM卡号：";
            // 
            // lab_DeviceID
            // 
            this.lab_DeviceID.AutoSize = true;
            this.lab_DeviceID.Location = new System.Drawing.Point(116, 26);
            this.lab_DeviceID.Name = "lab_DeviceID";
            this.lab_DeviceID.Size = new System.Drawing.Size(53, 12);
            this.lab_DeviceID.TabIndex = 36;
            this.lab_DeviceID.Text = "DeviceID";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(53, 26);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(53, 12);
            this.label16.TabIndex = 35;
            this.label16.Text = "终端号：";
            // 
            // lab_CarNo
            // 
            this.lab_CarNo.AutoSize = true;
            this.lab_CarNo.Location = new System.Drawing.Point(116, 56);
            this.lab_CarNo.Name = "lab_CarNo";
            this.lab_CarNo.Size = new System.Drawing.Size(35, 12);
            this.lab_CarNo.TabIndex = 34;
            this.lab_CarNo.Text = "CarNo";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(53, 56);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 12);
            this.label12.TabIndex = 33;
            this.label12.Text = "车牌号：";
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.Controls.Add(this.lab_LocalPort);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.lab_LocalIP);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(248, 58);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "本机通信参数";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lab_LocalPort
            // 
            this.lab_LocalPort.AutoSize = true;
            this.lab_LocalPort.Location = new System.Drawing.Point(126, 45);
            this.lab_LocalPort.Name = "lab_LocalPort";
            this.lab_LocalPort.Size = new System.Drawing.Size(29, 12);
            this.lab_LocalPort.TabIndex = 44;
            this.lab_LocalPort.Text = "Port";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(69, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 41;
            this.label6.Text = "IP地址：";
            // 
            // lab_LocalIP
            // 
            this.lab_LocalIP.AutoSize = true;
            this.lab_LocalIP.Location = new System.Drawing.Point(126, 15);
            this.lab_LocalIP.Name = "lab_LocalIP";
            this.lab_LocalIP.Size = new System.Drawing.Size(17, 12);
            this.lab_LocalIP.TabIndex = 45;
            this.lab_LocalIP.Text = "IP";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(69, 44);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 42;
            this.label7.Text = "端口号：";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btn_Ports);
            this.tabPage1.Controls.Add(this.btn_Gps);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(248, 58);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Text = "在线设置";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btn_Ports
            // 
            this.btn_Ports.Location = new System.Drawing.Point(128, 19);
            this.btn_Ports.Name = "btn_Ports";
            this.btn_Ports.Size = new System.Drawing.Size(75, 23);
            this.btn_Ports.TabIndex = 0;
            this.btn_Ports.Text = "阀锁设置";
            this.btn_Ports.UseVisualStyleBackColor = true;
            this.btn_Ports.Click += new System.EventHandler(this.btn_Ports_Click);
            // 
            // btn_Gps
            // 
            this.btn_Gps.Location = new System.Drawing.Point(26, 19);
            this.btn_Gps.Name = "btn_Gps";
            this.btn_Gps.Size = new System.Drawing.Size(75, 23);
            this.btn_Gps.TabIndex = 0;
            this.btn_Gps.Text = "定位设置";
            this.btn_Gps.UseVisualStyleBackColor = true;
            this.btn_Gps.Click += new System.EventHandler(this.btn_Gps_Click);
            // 
            // splitContainer3
            // 
            this.splitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer3.Panel1.Controls.Add(this.webBrowser1);
            this.splitContainer3.Panel1.Controls.Add(this.dataGridView_DeviceList);
            this.splitContainer3.Panel1.Controls.Add(this.checkBox_GoRevLine);
            this.splitContainer3.Panel1.Controls.Add(this.lab_RevCount);
            this.splitContainer3.Panel1.Controls.Add(this.lab_TitleL);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.picB_Logo);
            this.splitContainer3.Panel2.Controls.Add(this.dataGridView_Log);
            this.splitContainer3.Size = new System.Drawing.Size(679, 347);
            this.splitContainer3.SplitterDistance = 257;
            this.splitContainer3.TabIndex = 0;
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(-162, -39);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(250, 250);
            this.webBrowser1.TabIndex = 7;
            this.webBrowser1.Visible = false;
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            // 
            // dataGridView_DeviceList
            // 
            this.dataGridView_DeviceList.AllowUserToAddRows = false;
            this.dataGridView_DeviceList.AllowUserToDeleteRows = false;
            this.dataGridView_DeviceList.AllowUserToOrderColumns = true;
            this.dataGridView_DeviceList.AllowUserToResizeRows = false;
            this.dataGridView_DeviceList.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView_DeviceList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_DeviceList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.终端号,
            this.车牌号,
            this.Sim卡号,
            this.卫星时间,
            this.经度,
            this.纬度,
            this.速度,
            this.连接,
            this.方向,
            this.高程,
            this.油口,
            this.报警标志,
            this.原始接收,
            this.透传接收,
            this.状态});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView_DeviceList.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView_DeviceList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_DeviceList.Location = new System.Drawing.Point(0, 24);
            this.dataGridView_DeviceList.Name = "dataGridView_DeviceList";
            this.dataGridView_DeviceList.ReadOnly = true;
            this.dataGridView_DeviceList.RowHeadersVisible = false;
            this.dataGridView_DeviceList.RowTemplate.Height = 23;
            this.dataGridView_DeviceList.Size = new System.Drawing.Size(677, 231);
            this.dataGridView_DeviceList.TabIndex = 6;
            this.dataGridView_DeviceList.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_DeviceList_CellMouseClick);
            // 
            // 终端号
            // 
            this.终端号.DataPropertyName = "终端号";
            this.终端号.HeaderText = "终端号";
            this.终端号.Name = "终端号";
            this.终端号.ReadOnly = true;
            this.终端号.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.终端号.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // 车牌号
            // 
            this.车牌号.DataPropertyName = "车牌号";
            this.车牌号.HeaderText = "车牌号";
            this.车牌号.Name = "车牌号";
            this.车牌号.ReadOnly = true;
            this.车牌号.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Sim卡号
            // 
            this.Sim卡号.DataPropertyName = "Sim卡号";
            this.Sim卡号.HeaderText = "Sim卡号";
            this.Sim卡号.Name = "Sim卡号";
            this.Sim卡号.ReadOnly = true;
            this.Sim卡号.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // 卫星时间
            // 
            this.卫星时间.DataPropertyName = "卫星时间";
            this.卫星时间.HeaderText = "卫星时间";
            this.卫星时间.Name = "卫星时间";
            this.卫星时间.ReadOnly = true;
            this.卫星时间.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.卫星时间.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.卫星时间.Width = 150;
            // 
            // 经度
            // 
            this.经度.DataPropertyName = "经度";
            this.经度.HeaderText = "经度";
            this.经度.Name = "经度";
            this.经度.ReadOnly = true;
            this.经度.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // 纬度
            // 
            this.纬度.DataPropertyName = "纬度";
            this.纬度.HeaderText = "纬度";
            this.纬度.Name = "纬度";
            this.纬度.ReadOnly = true;
            this.纬度.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // 速度
            // 
            this.速度.DataPropertyName = "速度";
            this.速度.HeaderText = "速度";
            this.速度.Name = "速度";
            this.速度.ReadOnly = true;
            this.速度.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // 连接
            // 
            this.连接.DataPropertyName = "连接";
            this.连接.HeaderText = "连接";
            this.连接.Name = "连接";
            this.连接.ReadOnly = true;
            this.连接.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // 方向
            // 
            this.方向.DataPropertyName = "方向";
            this.方向.HeaderText = "方向";
            this.方向.Name = "方向";
            this.方向.ReadOnly = true;
            this.方向.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // 高程
            // 
            this.高程.DataPropertyName = "高程";
            this.高程.HeaderText = "高程";
            this.高程.Name = "高程";
            this.高程.ReadOnly = true;
            this.高程.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // 油口
            // 
            this.油口.DataPropertyName = "油口";
            this.油口.HeaderText = "油口";
            this.油口.Name = "油口";
            this.油口.ReadOnly = true;
            this.油口.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.油口.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // 报警标志
            // 
            this.报警标志.DataPropertyName = "报警标志";
            this.报警标志.HeaderText = "报警标志";
            this.报警标志.Name = "报警标志";
            this.报警标志.ReadOnly = true;
            this.报警标志.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // 原始接收
            // 
            this.原始接收.DataPropertyName = "原始接收";
            this.原始接收.HeaderText = "原始接收";
            this.原始接收.Name = "原始接收";
            this.原始接收.ReadOnly = true;
            this.原始接收.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // 透传接收
            // 
            this.透传接收.DataPropertyName = "透传接收";
            this.透传接收.HeaderText = "透传接收";
            this.透传接收.Name = "透传接收";
            this.透传接收.ReadOnly = true;
            this.透传接收.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // 状态
            // 
            this.状态.DataPropertyName = "状态";
            this.状态.HeaderText = "状态";
            this.状态.Name = "状态";
            this.状态.ReadOnly = true;
            this.状态.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // checkBox_GoRevLine
            // 
            this.checkBox_GoRevLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_GoRevLine.AutoSize = true;
            this.checkBox_GoRevLine.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.checkBox_GoRevLine.Location = new System.Drawing.Point(585, 5);
            this.checkBox_GoRevLine.Name = "checkBox_GoRevLine";
            this.checkBox_GoRevLine.Size = new System.Drawing.Size(84, 16);
            this.checkBox_GoRevLine.TabIndex = 5;
            this.checkBox_GoRevLine.Text = "跟踪新接收";
            this.checkBox_GoRevLine.UseVisualStyleBackColor = false;
            // 
            // lab_RevCount
            // 
            this.lab_RevCount.AutoSize = true;
            this.lab_RevCount.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.lab_RevCount.Location = new System.Drawing.Point(5, 6);
            this.lab_RevCount.Name = "lab_RevCount";
            this.lab_RevCount.Size = new System.Drawing.Size(83, 12);
            this.lab_RevCount.TabIndex = 4;
            this.lab_RevCount.Text = "在线车辆：0/x";
            // 
            // lab_TitleL
            // 
            this.lab_TitleL.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.lab_TitleL.Dock = System.Windows.Forms.DockStyle.Top;
            this.lab_TitleL.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_TitleL.Location = new System.Drawing.Point(0, 0);
            this.lab_TitleL.Name = "lab_TitleL";
            this.lab_TitleL.Size = new System.Drawing.Size(677, 24);
            this.lab_TitleL.TabIndex = 2;
            this.lab_TitleL.Text = "车辆状态列表";
            this.lab_TitleL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picB_Logo
            // 
            this.picB_Logo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picB_Logo.ImageLocation = "logo/logo.bmp";
            this.picB_Logo.Location = new System.Drawing.Point(448, 7);
            this.picB_Logo.Name = "picB_Logo";
            this.picB_Logo.Size = new System.Drawing.Size(227, 77);
            this.picB_Logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picB_Logo.TabIndex = 0;
            this.picB_Logo.TabStop = false;
            // 
            // dataGridView_Log
            // 
            this.dataGridView_Log.AllowUserToAddRows = false;
            this.dataGridView_Log.AllowUserToDeleteRows = false;
            this.dataGridView_Log.AllowUserToResizeRows = false;
            this.dataGridView_Log.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView_Log.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Log.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column9,
            this.Column10});
            this.dataGridView_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_Log.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_Log.Name = "dataGridView_Log";
            this.dataGridView_Log.ReadOnly = true;
            this.dataGridView_Log.RowHeadersVisible = false;
            this.dataGridView_Log.RowTemplate.Height = 23;
            this.dataGridView_Log.Size = new System.Drawing.Size(677, 84);
            this.dataGridView_Log.TabIndex = 1;
            // 
            // Column9
            // 
            this.Column9.DataPropertyName = "Time";
            this.Column9.HeaderText = "时间";
            this.Column9.Name = "Column9";
            this.Column9.ReadOnly = true;
            this.Column9.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column9.Width = 200;
            // 
            // Column10
            // 
            this.Column10.DataPropertyName = "Event";
            this.Column10.HeaderText = "事件";
            this.Column10.Name = "Column10";
            this.Column10.ReadOnly = true;
            this.Column10.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column10.Width = 300;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 300;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipText = "通信服务器";
            this.notifyIcon1.BalloonTipTitle = "车辆管理系统";
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "车辆管理系统——通信服务器";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.退出ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(101, 26);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.退出ToolStripMenuItem.Text = "退出";
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
            // 
            // timerCheckConn
            // 
            this.timerCheckConn.Enabled = true;
            this.timerCheckConn.Interval = 1000;
            this.timerCheckConn.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // timerCom
            // 
            this.timerCom.Enabled = true;
            this.timerCom.Interval = 2000;
            this.timerCom.Tick += new System.EventHandler(this.timer4_Tick);
            // 
            // timerSheetOut
            // 
            this.timerSheetOut.Enabled = true;
            this.timerSheetOut.Interval = 10000;
            this.timerSheetOut.Tick += new System.EventHandler(this.timerSheetOut_Tick);
            // 
            // timerTranslate
            // 
            this.timerTranslate.Interval = 2000;
            this.timerTranslate.Tick += new System.EventHandler(this.timerTranslate_Tick);
            // 
            // lab_Time
            // 
            this.lab_Time.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lab_Time.BackColor = System.Drawing.Color.Black;
            this.lab_Time.Font = new System.Drawing.Font("宋体", 13.5F, System.Drawing.FontStyle.Bold);
            this.lab_Time.ForeColor = System.Drawing.Color.Yellow;
            this.lab_Time.Location = new System.Drawing.Point(707, 0);
            this.lab_Time.Name = "lab_Time";
            this.lab_Time.Size = new System.Drawing.Size(235, 47);
            this.lab_Time.TabIndex = 19;
            this.lab_Time.Text = "Time";
            this.lab_Time.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timerRead
            // 
            this.timerRead.Interval = 5000;
            this.timerRead.Tick += new System.EventHandler(this.timerRead_Tick);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(942, 419);
            this.Controls.Add(this.lab_Time);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btn_Exit);
            this.Controls.Add(this.btn_Log);
            this.Controls.Add(this.lab_Title);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "通信服务器  V_1.0.151205";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_DeviceList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picB_Logo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Log)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Label lab_Title;
        private System.Windows.Forms.Button btn_Exit;
        private System.Windows.Forms.Button btn_Log;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TreeView treeView_All;
        private System.Windows.Forms.Label lab_TitleR;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.DataGridView dataGridView_DeviceList;
        private System.Windows.Forms.CheckBox checkBox_GoRevLine;
        private System.Windows.Forms.Label lab_RevCount;
        private System.Windows.Forms.Label lab_TitleL;
        private System.Windows.Forms.PictureBox picB_Logo;
        private System.Windows.Forms.DataGridView dataGridView_Log;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lab_DeviceID;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lab_LocalPort;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lab_LocalIP;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lab_Phone;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ImageList imageList_Tree;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.Timer timerCheckConn;
        private System.Windows.Forms.Timer timerCom;
        private System.Windows.Forms.Label lab_CarNo;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Timer timerSheetOut;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Timer timerTranslate;
        private System.Windows.Forms.Label lab_Time;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btn_Ports;
        private System.Windows.Forms.Button btn_Gps;
        private System.Windows.Forms.DataGridViewTextBoxColumn 终端号;
        private System.Windows.Forms.DataGridViewTextBoxColumn 车牌号;
        private System.Windows.Forms.DataGridViewTextBoxColumn Sim卡号;
        private System.Windows.Forms.DataGridViewTextBoxColumn 卫星时间;
        private System.Windows.Forms.DataGridViewTextBoxColumn 经度;
        private System.Windows.Forms.DataGridViewTextBoxColumn 纬度;
        private System.Windows.Forms.DataGridViewTextBoxColumn 速度;
        private System.Windows.Forms.DataGridViewTextBoxColumn 连接;
        private System.Windows.Forms.DataGridViewTextBoxColumn 方向;
        private System.Windows.Forms.DataGridViewTextBoxColumn 高程;
        private System.Windows.Forms.DataGridViewTextBoxColumn 油口;
        private System.Windows.Forms.DataGridViewTextBoxColumn 报警标志;
        private System.Windows.Forms.DataGridViewTextBoxColumn 原始接收;
        private System.Windows.Forms.DataGridViewTextBoxColumn 透传接收;
        private System.Windows.Forms.DataGridViewTextBoxColumn 状态;
        private System.Windows.Forms.Timer timerRead;
    }
}

