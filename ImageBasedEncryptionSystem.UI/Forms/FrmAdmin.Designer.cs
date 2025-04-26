namespace ImageBasedEncryptionSystem.UI.Forms
{
    partial class FrmAdmin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAdmin));
            this.pnlTitleBar = new Guna.UI2.WinForms.Guna2Panel();
            this.pbLogin = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblDevModeStatus = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2ControlBox2 = new Guna.UI2.WinForms.Guna2ControlBox();
            this.guna2ControlBox1 = new Guna.UI2.WinForms.Guna2ControlBox();
            this.btnHelp = new Guna.UI2.WinForms.Guna2Button();
            this.guna2DragControl1 = new Guna.UI2.WinForms.Guna2DragControl(this.components);
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.pnlMain = new Guna.UI2.WinForms.Guna2Panel();
            this.lblRsaKey = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblIdentity = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblNewIdentity = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtRsaKey = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtIdentity = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtNewIdentity = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnReset = new Guna.UI2.WinForms.Guna2Button();
            this.btnSave = new Guna.UI2.WinForms.Guna2Button();
            this.btnRandom = new Guna.UI2.WinForms.Guna2Button();
            this.pnlTitleBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogin)).BeginInit();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTitleBar
            // 
            this.pnlTitleBar.BackColor = System.Drawing.Color.Transparent;
            this.pnlTitleBar.Controls.Add(this.pbLogin);
            this.pnlTitleBar.Controls.Add(this.lblTitle);
            this.pnlTitleBar.Controls.Add(this.lblDevModeStatus);
            this.pnlTitleBar.Controls.Add(this.guna2ControlBox2);
            this.pnlTitleBar.Controls.Add(this.guna2ControlBox1);
            this.pnlTitleBar.Controls.Add(this.btnHelp);
            this.pnlTitleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitleBar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.pnlTitleBar.Location = new System.Drawing.Point(0, 0);
            this.pnlTitleBar.Name = "pnlTitleBar";
            this.pnlTitleBar.Size = new System.Drawing.Size(650, 35);
            this.pnlTitleBar.TabIndex = 0;
            // 
            // pbLogin
            // 
            this.pbLogin.BackColor = System.Drawing.Color.Transparent;
            this.pbLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbLogin.Image = ((System.Drawing.Image)(resources.GetObject("pbLogin.Image")));
            this.pbLogin.ImageRotate = 0F;
            this.pbLogin.Location = new System.Drawing.Point(10, -2);
            this.pbLogin.Name = "pbLogin";
            this.pbLogin.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.pbLogin.Size = new System.Drawing.Size(40, 40);
            this.pbLogin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbLogin.TabIndex = 5;
            this.pbLogin.TabStop = false;
            this.pbLogin.UseTransparentBackground = true;
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(56, 6);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(123, 22);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Yönetici İşlemleri";
            // 
            // lblDevModeStatus
            // 
            this.lblDevModeStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblDevModeStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblDevModeStatus.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblDevModeStatus.Location = new System.Drawing.Point(421, 11);
            this.lblDevModeStatus.Name = "lblDevModeStatus";
            this.lblDevModeStatus.Size = new System.Drawing.Size(123, 17);
            this.lblDevModeStatus.TabIndex = 1;
            this.lblDevModeStatus.Text = "Geliştirici Modu: AÇIK";
            // 
            // guna2ControlBox2
            // 
            this.guna2ControlBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBox2.ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox;
            this.guna2ControlBox2.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBox2.IconColor = System.Drawing.Color.White;
            this.guna2ControlBox2.Location = new System.Drawing.Point(575, 6);
            this.guna2ControlBox2.Name = "guna2ControlBox2";
            this.guna2ControlBox2.Size = new System.Drawing.Size(34, 24);
            this.guna2ControlBox2.TabIndex = 3;
            // 
            // guna2ControlBox1
            // 
            this.guna2ControlBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBox1.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBox1.IconColor = System.Drawing.Color.White;
            this.guna2ControlBox1.Location = new System.Drawing.Point(614, 6);
            this.guna2ControlBox1.Name = "guna2ControlBox1";
            this.guna2ControlBox1.Size = new System.Drawing.Size(34, 24);
            this.guna2ControlBox1.TabIndex = 2;
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHelp.BackColor = System.Drawing.Color.Transparent;
            this.btnHelp.BorderRadius = 10;
            this.btnHelp.FillColor = System.Drawing.Color.Transparent;
            this.btnHelp.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnHelp.ForeColor = System.Drawing.Color.White;
            this.btnHelp.Location = new System.Drawing.Point(550, 6);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(34, 24);
            this.btnHelp.TabIndex = 4;
            this.btnHelp.Text = "?";
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // guna2DragControl1
            // 
            this.guna2DragControl1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2DragControl1.TargetControl = this.pnlTitleBar;
            this.guna2DragControl1.UseTransparentDrag = true;
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 15;
            this.guna2Elipse1.TargetControl = this;
            // 
            // pnlMain
            // 
            this.pnlMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.pnlMain.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.pnlMain.BorderRadius = 15;
            this.pnlMain.BorderThickness = 1;
            this.pnlMain.Controls.Add(this.lblRsaKey);
            this.pnlMain.Controls.Add(this.lblIdentity);
            this.pnlMain.Controls.Add(this.lblNewIdentity);
            this.pnlMain.Controls.Add(this.txtRsaKey);
            this.pnlMain.Controls.Add(this.txtIdentity);
            this.pnlMain.Controls.Add(this.txtNewIdentity);
            this.pnlMain.Controls.Add(this.btnReset);
            this.pnlMain.Controls.Add(this.btnSave);
            this.pnlMain.Controls.Add(this.btnRandom);
            this.pnlMain.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pnlMain.Location = new System.Drawing.Point(12, 50);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(625, 387);
            this.pnlMain.TabIndex = 1;
            // 
            // lblRsaKey
            // 
            this.lblRsaKey.BackColor = System.Drawing.Color.Transparent;
            this.lblRsaKey.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblRsaKey.ForeColor = System.Drawing.Color.White;
            this.lblRsaKey.Location = new System.Drawing.Point(15, 165);
            this.lblRsaKey.Name = "lblRsaKey";
            this.lblRsaKey.Size = new System.Drawing.Size(88, 19);
            this.lblRsaKey.TabIndex = 6;
            this.lblRsaKey.Text = "RSA Anahtarı:";
            // 
            // lblIdentity
            // 
            this.lblIdentity.BackColor = System.Drawing.Color.Transparent;
            this.lblIdentity.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblIdentity.ForeColor = System.Drawing.Color.White;
            this.lblIdentity.Location = new System.Drawing.Point(15, 85);
            this.lblIdentity.Name = "lblIdentity";
            this.lblIdentity.Size = new System.Drawing.Size(95, 19);
            this.lblIdentity.TabIndex = 2;
            this.lblIdentity.Text = "Mevcut Kimlik:";
            // 
            // lblNewIdentity
            // 
            this.lblNewIdentity.BackColor = System.Drawing.Color.Transparent;
            this.lblNewIdentity.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblNewIdentity.ForeColor = System.Drawing.Color.White;
            this.lblNewIdentity.Location = new System.Drawing.Point(15, 25);
            this.lblNewIdentity.Name = "lblNewIdentity";
            this.lblNewIdentity.Size = new System.Drawing.Size(77, 19);
            this.lblNewIdentity.TabIndex = 0;
            this.lblNewIdentity.Text = "Yeni Kimlik:";
            // 
            // txtRsaKey
            // 
            this.txtRsaKey.BackColor = System.Drawing.Color.Transparent;
            this.txtRsaKey.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.txtRsaKey.BorderRadius = 10;
            this.txtRsaKey.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtRsaKey.DefaultText = "";
            this.txtRsaKey.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.txtRsaKey.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtRsaKey.DisabledState.ForeColor = System.Drawing.Color.LightGray;
            this.txtRsaKey.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtRsaKey.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.txtRsaKey.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtRsaKey.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtRsaKey.ForeColor = System.Drawing.Color.LightCyan;
            this.txtRsaKey.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtRsaKey.Location = new System.Drawing.Point(15, 190);
            this.txtRsaKey.Multiline = true;
            this.txtRsaKey.Name = "txtRsaKey";
            this.txtRsaKey.PlaceholderText = "";
            this.txtRsaKey.ReadOnly = true;
            this.txtRsaKey.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRsaKey.SelectedText = "";
            this.txtRsaKey.Size = new System.Drawing.Size(595, 100);
            this.txtRsaKey.TabIndex = 7;
            // 
            // txtIdentity
            // 
            this.txtIdentity.BackColor = System.Drawing.Color.Transparent;
            this.txtIdentity.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.txtIdentity.BorderRadius = 10;
            this.txtIdentity.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtIdentity.DefaultText = "";
            this.txtIdentity.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.txtIdentity.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtIdentity.DisabledState.ForeColor = System.Drawing.Color.White;
            this.txtIdentity.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtIdentity.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.txtIdentity.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtIdentity.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.txtIdentity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.txtIdentity.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtIdentity.Location = new System.Drawing.Point(15, 110);
            this.txtIdentity.Name = "txtIdentity";
            this.txtIdentity.PlaceholderText = "";
            this.txtIdentity.ReadOnly = true;
            this.txtIdentity.SelectedText = "";
            this.txtIdentity.Size = new System.Drawing.Size(595, 36);
            this.txtIdentity.TabIndex = 3;
            // 
            // txtNewIdentity
            // 
            this.txtNewIdentity.BackColor = System.Drawing.Color.Transparent;
            this.txtNewIdentity.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.txtNewIdentity.BorderRadius = 10;
            this.txtNewIdentity.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtNewIdentity.DefaultText = "";
            this.txtNewIdentity.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.txtNewIdentity.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtNewIdentity.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtNewIdentity.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtNewIdentity.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.txtNewIdentity.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtNewIdentity.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNewIdentity.ForeColor = System.Drawing.Color.White;
            this.txtNewIdentity.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtNewIdentity.Location = new System.Drawing.Point(15, 50);
            this.txtNewIdentity.MaxLength = 50;
            this.txtNewIdentity.Name = "txtNewIdentity";
            this.txtNewIdentity.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtNewIdentity.PlaceholderText = "Yeni kimlik değerini girin veya rastgele oluşturun";
            this.txtNewIdentity.SelectedText = "";
            this.txtNewIdentity.Size = new System.Drawing.Size(595, 36);
            this.txtNewIdentity.TabIndex = 1;
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.Transparent;
            this.btnReset.BorderRadius = 10;
            this.btnReset.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnReset.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnReset.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnReset.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnReset.FillColor = System.Drawing.Color.Crimson;
            this.btnReset.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnReset.ForeColor = System.Drawing.Color.White;
            this.btnReset.Location = new System.Drawing.Point(15, 315);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(180, 45);
            this.btnReset.TabIndex = 8;
            this.btnReset.Text = "Kimliği Sıfırla";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.BorderRadius = 10;
            this.btnSave.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnSave.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnSave.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnSave.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnSave.FillColor = System.Drawing.Color.ForestGreen;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(430, 315);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(180, 45);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "Kimliği Kaydet";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnRandom
            // 
            this.btnRandom.BackColor = System.Drawing.Color.Transparent;
            this.btnRandom.BorderRadius = 10;
            this.btnRandom.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnRandom.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnRandom.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnRandom.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnRandom.FillColor = System.Drawing.Color.DodgerBlue;
            this.btnRandom.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRandom.ForeColor = System.Drawing.Color.White;
            this.btnRandom.Location = new System.Drawing.Point(222, 315);
            this.btnRandom.Name = "btnRandom";
            this.btnRandom.Size = new System.Drawing.Size(180, 45);
            this.btnRandom.TabIndex = 9;
            this.btnRandom.Text = "Yeni Kimlik Oluştur";
            this.btnRandom.Click += new System.EventHandler(this.btnRandom_Click);
            // 
            // FrmAdmin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(650, 460);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlTitleBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmAdmin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Yönetici Paneli";
            this.Load += new System.EventHandler(this.FrmAdmin_Load);
            this.pnlTitleBar.ResumeLayout(false);
            this.pnlTitleBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogin)).EndInit();
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel pnlTitleBar;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblDevModeStatus;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox1;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox2;
        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2Panel pnlMain;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblRsaKey;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblIdentity;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblNewIdentity;
        private Guna.UI2.WinForms.Guna2TextBox txtRsaKey;
        private Guna.UI2.WinForms.Guna2TextBox txtIdentity;
        private Guna.UI2.WinForms.Guna2TextBox txtNewIdentity;
        private Guna.UI2.WinForms.Guna2Button btnReset;
        private Guna.UI2.WinForms.Guna2Button btnSave;
        private Guna.UI2.WinForms.Guna2Button btnRandom;
        private Guna.UI2.WinForms.Guna2Button btnHelp;
        private Guna.UI2.WinForms.Guna2CirclePictureBox pbLogin;
    }
}