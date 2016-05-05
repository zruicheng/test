namespace Communication
{
    partial class FormGps
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.txB_ServerIp = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txB_ServerPort = new System.Windows.Forms.TextBox();
            this.btn_GetAddress = new System.Windows.Forms.Button();
            this.btn_SetAddress = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_GetSpace = new System.Windows.Forms.Button();
            this.btn_SetSpace = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btn_GetGps = new System.Windows.Forms.Button();
            this.btn_Reset = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lab_DeviceID = new System.Windows.Forms.Label();
            this.lab_CarNo = new System.Windows.Forms.Label();
            this.lab_Sim = new System.Windows.Forms.Label();
            this.num_Space = new System.Windows.Forms.NumericUpDown();
            this.lab_Lng = new System.Windows.Forms.Label();
            this.lab_Lat = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label12 = new System.Windows.Forms.Label();
            this.lab_Time = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.num_Space)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 84);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "服务器IP：";
            // 
            // txB_ServerIp
            // 
            this.txB_ServerIp.Location = new System.Drawing.Point(109, 75);
            this.txB_ServerIp.Name = "txB_ServerIp";
            this.txB_ServerIp.Size = new System.Drawing.Size(175, 21);
            this.txB_ServerIp.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(298, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "端口：";
            // 
            // txB_ServerPort
            // 
            this.txB_ServerPort.Location = new System.Drawing.Point(345, 75);
            this.txB_ServerPort.Name = "txB_ServerPort";
            this.txB_ServerPort.Size = new System.Drawing.Size(104, 21);
            this.txB_ServerPort.TabIndex = 1;
            // 
            // btn_GetAddress
            // 
            this.btn_GetAddress.Location = new System.Drawing.Point(473, 73);
            this.btn_GetAddress.Name = "btn_GetAddress";
            this.btn_GetAddress.Size = new System.Drawing.Size(75, 23);
            this.btn_GetAddress.TabIndex = 2;
            this.btn_GetAddress.Text = "查询";
            this.btn_GetAddress.UseVisualStyleBackColor = true;
            this.btn_GetAddress.Click += new System.EventHandler(this.btn_GetAddress_Click);
            // 
            // btn_SetAddress
            // 
            this.btn_SetAddress.Location = new System.Drawing.Point(554, 73);
            this.btn_SetAddress.Name = "btn_SetAddress";
            this.btn_SetAddress.Size = new System.Drawing.Size(75, 23);
            this.btn_SetAddress.TabIndex = 2;
            this.btn_SetAddress.Text = "修改";
            this.btn_SetAddress.UseVisualStyleBackColor = true;
            this.btn_SetAddress.Click += new System.EventHandler(this.btn_SetAddress_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 139);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "位置汇报间隔：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(266, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "秒";
            // 
            // btn_GetSpace
            // 
            this.btn_GetSpace.Location = new System.Drawing.Point(473, 127);
            this.btn_GetSpace.Name = "btn_GetSpace";
            this.btn_GetSpace.Size = new System.Drawing.Size(75, 23);
            this.btn_GetSpace.TabIndex = 2;
            this.btn_GetSpace.Text = "查询";
            this.btn_GetSpace.UseVisualStyleBackColor = true;
            this.btn_GetSpace.Click += new System.EventHandler(this.btn_GetSpace_Click);
            // 
            // btn_SetSpace
            // 
            this.btn_SetSpace.Location = new System.Drawing.Point(554, 127);
            this.btn_SetSpace.Name = "btn_SetSpace";
            this.btn_SetSpace.Size = new System.Drawing.Size(75, 23);
            this.btn_SetSpace.TabIndex = 2;
            this.btn_SetSpace.Text = "修改";
            this.btn_SetSpace.UseVisualStyleBackColor = true;
            this.btn_SetSpace.Click += new System.EventHandler(this.btn_SetSpace_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(62, 196);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "经度：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(266, 196);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 4;
            this.label6.Text = "纬度：";
            // 
            // btn_GetGps
            // 
            this.btn_GetGps.Location = new System.Drawing.Point(473, 185);
            this.btn_GetGps.Name = "btn_GetGps";
            this.btn_GetGps.Size = new System.Drawing.Size(75, 23);
            this.btn_GetGps.TabIndex = 2;
            this.btn_GetGps.Text = "查询";
            this.btn_GetGps.UseVisualStyleBackColor = true;
            this.btn_GetGps.Click += new System.EventHandler(this.btn_GetGps_Click);
            // 
            // btn_Reset
            // 
            this.btn_Reset.Location = new System.Drawing.Point(473, 238);
            this.btn_Reset.Name = "btn_Reset";
            this.btn_Reset.Size = new System.Drawing.Size(75, 23);
            this.btn_Reset.TabIndex = 5;
            this.btn_Reset.Text = "重启";
            this.btn_Reset.UseVisualStyleBackColor = true;
            this.btn_Reset.Click += new System.EventHandler(this.btn_Reset_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(253, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 6;
            this.label7.Text = "车牌号：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(456, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 12);
            this.label8.TabIndex = 6;
            this.label8.Text = "Sim卡号：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(38, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 6;
            this.label9.Text = "终端编号：";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(40, 300);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(299, 12);
            this.label10.TabIndex = 7;
            this.label10.Text = "注意：如须初始化终端，请在初始化后发送短信到Sim卡";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(76, 327);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(347, 12);
            this.label11.TabIndex = 7;
            this.label11.Text = "短信格式为“*2012*IP120.031.136.058；6973；13798270635#”";
            // 
            // lab_DeviceID
            // 
            this.lab_DeviceID.AutoSize = true;
            this.lab_DeviceID.Location = new System.Drawing.Point(107, 22);
            this.lab_DeviceID.Name = "lab_DeviceID";
            this.lab_DeviceID.Size = new System.Drawing.Size(77, 12);
            this.lab_DeviceID.TabIndex = 8;
            this.lab_DeviceID.Text = "123456789012";
            // 
            // lab_CarNo
            // 
            this.lab_CarNo.AutoSize = true;
            this.lab_CarNo.Location = new System.Drawing.Point(311, 22);
            this.lab_CarNo.Name = "lab_CarNo";
            this.lab_CarNo.Size = new System.Drawing.Size(65, 12);
            this.lab_CarNo.TabIndex = 8;
            this.lab_CarNo.Text = "京A 888888";
            // 
            // lab_Sim
            // 
            this.lab_Sim.AutoSize = true;
            this.lab_Sim.Location = new System.Drawing.Point(521, 22);
            this.lab_Sim.Name = "lab_Sim";
            this.lab_Sim.Size = new System.Drawing.Size(71, 12);
            this.lab_Sim.TabIndex = 8;
            this.lab_Sim.Text = "12345678901";
            // 
            // num_Space
            // 
            this.num_Space.Location = new System.Drawing.Point(109, 130);
            this.num_Space.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.num_Space.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_Space.Name = "num_Space";
            this.num_Space.Size = new System.Drawing.Size(139, 21);
            this.num_Space.TabIndex = 9;
            this.num_Space.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // lab_Lng
            // 
            this.lab_Lng.AutoSize = true;
            this.lab_Lng.Location = new System.Drawing.Point(109, 196);
            this.lab_Lng.Name = "lab_Lng";
            this.lab_Lng.Size = new System.Drawing.Size(23, 12);
            this.lab_Lng.TabIndex = 10;
            this.lab_Lng.Text = "lng";
            // 
            // lab_Lat
            // 
            this.lab_Lat.AutoSize = true;
            this.lab_Lat.Location = new System.Drawing.Point(313, 196);
            this.lab_Lat.Name = "lab_Lat";
            this.lab_Lat.Size = new System.Drawing.Size(23, 12);
            this.lab_Lat.TabIndex = 10;
            this.lab_Lat.Text = "lat";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(38, 243);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 12);
            this.label12.TabIndex = 11;
            this.label12.Text = "模块时间：";
            // 
            // lab_Time
            // 
            this.lab_Time.AutoSize = true;
            this.lab_Time.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_Time.Location = new System.Drawing.Point(109, 235);
            this.lab_Time.Name = "lab_Time";
            this.lab_Time.Size = new System.Drawing.Size(49, 20);
            this.lab_Time.TabIndex = 12;
            this.lab_Time.Text = "time";
            // 
            // FormGps
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 382);
            this.Controls.Add(this.lab_Time);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.lab_Lat);
            this.Controls.Add(this.lab_Lng);
            this.Controls.Add(this.num_Space);
            this.Controls.Add(this.lab_Sim);
            this.Controls.Add(this.lab_CarNo);
            this.Controls.Add(this.lab_DeviceID);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btn_Reset);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btn_SetSpace);
            this.Controls.Add(this.btn_GetGps);
            this.Controls.Add(this.btn_GetSpace);
            this.Controls.Add(this.btn_SetAddress);
            this.Controls.Add(this.btn_GetAddress);
            this.Controls.Add(this.txB_ServerPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txB_ServerIp);
            this.Controls.Add(this.label1);
            this.Name = "FormGps";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "定位模块设置";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormGps_FormClosing);
            this.Load += new System.EventHandler(this.FormGps_Load);
            ((System.ComponentModel.ISupportInitialize)(this.num_Space)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_GetAddress;
        private System.Windows.Forms.Button btn_SetAddress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_GetSpace;
        private System.Windows.Forms.Button btn_SetSpace;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btn_GetGps;
        private System.Windows.Forms.Button btn_Reset;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lab_Lng;
        private System.Windows.Forms.Label lab_Lat;
        public System.Windows.Forms.Label lab_DeviceID;
        public System.Windows.Forms.Label lab_CarNo;
        public System.Windows.Forms.Label lab_Sim;
        public System.Windows.Forms.TextBox txB_ServerIp;
        public System.Windows.Forms.TextBox txB_ServerPort;
        public System.Windows.Forms.NumericUpDown num_Space;
        public System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lab_Time;
    }
}