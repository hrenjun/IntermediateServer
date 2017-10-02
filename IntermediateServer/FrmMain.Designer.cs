namespace IntermediateServer
{
    partial class FrmMain
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
            this.btnStartUp = new System.Windows.Forms.Button();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.msMenu = new System.Windows.Forms.MenuStrip();
            this.tismInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.系统设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.lblIP = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.listBoxOnlineList = new System.Windows.Forms.ListBox();
            this.txtNoticias = new System.Windows.Forms.TextBox();
            this.btnOut = new System.Windows.Forms.Button();
            this.lblNoticias = new System.Windows.Forms.Label();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.定时任务ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.开始ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.停止ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.msMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStartUp
            // 
            this.btnStartUp.Location = new System.Drawing.Point(709, 519);
            this.btnStartUp.Name = "btnStartUp";
            this.btnStartUp.Size = new System.Drawing.Size(89, 34);
            this.btnStartUp.TabIndex = 0;
            this.btnStartUp.Text = "启动";
            this.btnStartUp.UseVisualStyleBackColor = true;
            this.btnStartUp.Click += new System.EventHandler(this.btnStartUp_Click);
            // 
            // txtInfo
            // 
            this.txtInfo.Location = new System.Drawing.Point(0, 27);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            this.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtInfo.Size = new System.Drawing.Size(507, 354);
            this.txtInfo.TabIndex = 1;
            // 
            // msMenu
            // 
            this.msMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tismInfo,
            this.系统设置ToolStripMenuItem,
            this.定时任务ToolStripMenuItem});
            this.msMenu.Location = new System.Drawing.Point(0, 0);
            this.msMenu.Name = "msMenu";
            this.msMenu.Size = new System.Drawing.Size(813, 24);
            this.msMenu.TabIndex = 3;
            this.msMenu.Text = "menuStrip2";
            // 
            // tismInfo
            // 
            this.tismInfo.Name = "tismInfo";
            this.tismInfo.Size = new System.Drawing.Size(45, 20);
            this.tismInfo.Text = "信息";
            // 
            // 系统设置ToolStripMenuItem
            // 
            this.系统设置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.关于ToolStripMenuItem,
            this.退出ToolStripMenuItem});
            this.系统设置ToolStripMenuItem.Name = "系统设置ToolStripMenuItem";
            this.系统设置ToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.系统设置ToolStripMenuItem.Text = "系统设置";
            // 
            // 关于ToolStripMenuItem
            // 
            this.关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            this.关于ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.关于ToolStripMenuItem.Text = "关于";
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.退出ToolStripMenuItem.Text = "退出";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblPort.Location = new System.Drawing.Point(554, 406);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(73, 20);
            this.lblPort.TabIndex = 4;
            this.lblPort.Text = "端口号：";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(620, 408);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(178, 20);
            this.txtPort.TabIndex = 5;
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(620, 457);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(178, 20);
            this.txtIP.TabIndex = 7;
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblIP.Location = new System.Drawing.Point(554, 455);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(72, 20);
            this.lblIP.TabIndex = 6;
            this.lblIP.Text = "IP地址：";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick_1);
            // 
            // listBoxOnlineList
            // 
            this.listBoxOnlineList.FormattingEnabled = true;
            this.listBoxOnlineList.Location = new System.Drawing.Point(513, 26);
            this.listBoxOnlineList.Name = "listBoxOnlineList";
            this.listBoxOnlineList.Size = new System.Drawing.Size(285, 355);
            this.listBoxOnlineList.TabIndex = 8;
            // 
            // txtNoticias
            // 
            this.txtNoticias.Location = new System.Drawing.Point(0, 418);
            this.txtNoticias.Multiline = true;
            this.txtNoticias.Name = "txtNoticias";
            this.txtNoticias.Size = new System.Drawing.Size(507, 155);
            this.txtNoticias.TabIndex = 9;
            // 
            // btnOut
            // 
            this.btnOut.Location = new System.Drawing.Point(558, 519);
            this.btnOut.Name = "btnOut";
            this.btnOut.Size = new System.Drawing.Size(89, 34);
            this.btnOut.TabIndex = 10;
            this.btnOut.Text = "发送";
            this.btnOut.UseVisualStyleBackColor = true;
            this.btnOut.Click += new System.EventHandler(this.btnOut_Click);
            this.btnOut.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnOut_KeyDown);
            // 
            // lblNoticias
            // 
            this.lblNoticias.AutoSize = true;
            this.lblNoticias.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblNoticias.Location = new System.Drawing.Point(12, 395);
            this.lblNoticias.Name = "lblNoticias";
            this.lblNoticias.Size = new System.Drawing.Size(57, 20);
            this.lblNoticias.TabIndex = 11;
            this.lblNoticias.Text = "消息：";
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // 定时任务ToolStripMenuItem
            // 
            this.定时任务ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.开始ToolStripMenuItem,
            this.停止ToolStripMenuItem});
            this.定时任务ToolStripMenuItem.Name = "定时任务ToolStripMenuItem";
            this.定时任务ToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.定时任务ToolStripMenuItem.Text = "定时任务";
            this.定时任务ToolStripMenuItem.Click += new System.EventHandler(this.定时任务ToolStripMenuItem_Click);
            
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(813, 576);
            this.Controls.Add(this.lblNoticias);
            this.Controls.Add(this.btnOut);
            this.Controls.Add(this.txtNoticias);
            this.Controls.Add(this.listBoxOnlineList);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.lblIP);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.txtInfo);
            this.Controls.Add(this.btnStartUp);
            this.Controls.Add(this.msMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "中间服务器";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.msMenu.ResumeLayout(false);
            this.msMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStartUp;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.MenuStrip msMenu;
        private System.Windows.Forms.ToolStripMenuItem tismInfo;
        private System.Windows.Forms.ToolStripMenuItem 系统设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 关于ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ListBox listBoxOnlineList;
        private System.Windows.Forms.TextBox txtNoticias;
        private System.Windows.Forms.Button btnOut;
        private System.Windows.Forms.Label lblNoticias;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.ToolStripMenuItem 定时任务ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 开始ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 停止ToolStripMenuItem;
    }
}

