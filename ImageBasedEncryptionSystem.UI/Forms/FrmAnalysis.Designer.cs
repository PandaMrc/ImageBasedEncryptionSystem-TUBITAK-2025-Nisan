namespace ImageBasedEncryptionSystem.UI.Forms
{
    partial class FrmAnalysis
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ToolTip helpToolTip;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAnalysis));
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.guna2DragControl1 = new Guna.UI2.WinForms.Guna2DragControl(this.components);
            this.pnlTitleBar = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2ControlBox2 = new Guna.UI2.WinForms.Guna2ControlBox();
            this.guna2ControlBox1 = new Guna.UI2.WinForms.Guna2ControlBox();
            this.btnHelp = new Guna.UI2.WinForms.Guna2Button();
            this.helpToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2TabControl1 = new Guna.UI2.WinForms.Guna2TabControl();
            this.tabImageAnalysis = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabEncryptionDetails = new System.Windows.Forms.TabPage();
            this.tabHistogram = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tabReport = new System.Windows.Forms.TabPage();
            this.btnReturn = new Guna.UI2.WinForms.Guna2Button();
            this.lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pbLogin = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.guna2GroupBox1 = new Guna.UI2.WinForms.Guna2GroupBox();
            this.guna2GroupBox2 = new Guna.UI2.WinForms.Guna2GroupBox();
            this.txtPassword = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblPassword = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtRsaEncryptedKey = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblRsaEncryptedKey = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtAesKey = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblAesKey = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtEncryptedText = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblEncryptedText = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtOriginalText = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblOriginalText = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtLog = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblLog = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.btnSaveLog = new Guna.UI2.WinForms.Guna2Button();
            this.btnAnalyze = new Guna.UI2.WinForms.Guna2Button();
            this.pnlTitleBar.SuspendLayout();
            this.guna2Panel1.SuspendLayout();
            this.guna2TabControl1.SuspendLayout();
            this.tabImageAnalysis.SuspendLayout();
            this.tabEncryptionDetails.SuspendLayout();
            this.tabHistogram.SuspendLayout();
            this.tabReport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogin)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 15;
            this.guna2Elipse1.TargetControl = this;
            // 
            // guna2DragControl1
            // 
            this.guna2DragControl1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2DragControl1.TargetControl = this.pnlTitleBar;
            this.guna2DragControl1.UseTransparentDrag = true;
            // 
            // pnlTitleBar
            // 
            this.pnlTitleBar.BackColor = System.Drawing.Color.Transparent;
            this.pnlTitleBar.Controls.Add(this.pbLogin);
            this.pnlTitleBar.Controls.Add(this.guna2ControlBox2);
            this.pnlTitleBar.Controls.Add(this.guna2ControlBox1);
            this.pnlTitleBar.Controls.Add(this.btnHelp);
            this.pnlTitleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitleBar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.pnlTitleBar.Location = new System.Drawing.Point(0, 0);
            this.pnlTitleBar.Margin = new System.Windows.Forms.Padding(2);
            this.pnlTitleBar.Name = "pnlTitleBar";
            this.pnlTitleBar.Size = new System.Drawing.Size(1200, 32);
            this.pnlTitleBar.TabIndex = 5;
            // 
            // guna2ControlBox2
            // 
            this.guna2ControlBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBox2.ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox;
            this.guna2ControlBox2.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBox2.IconColor = System.Drawing.Color.White;
            this.guna2ControlBox2.Location = new System.Drawing.Point(1126, 5);
            this.guna2ControlBox2.Margin = new System.Windows.Forms.Padding(2);
            this.guna2ControlBox2.Name = "guna2ControlBox2";
            this.guna2ControlBox2.Size = new System.Drawing.Size(34, 24);
            this.guna2ControlBox2.TabIndex = 1;
            // 
            // guna2ControlBox1
            // 
            this.guna2ControlBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBox1.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBox1.IconColor = System.Drawing.Color.White;
            this.guna2ControlBox1.Location = new System.Drawing.Point(1164, 5);
            this.guna2ControlBox1.Margin = new System.Windows.Forms.Padding(2);
            this.guna2ControlBox1.Name = "guna2ControlBox1";
            this.guna2ControlBox1.Size = new System.Drawing.Size(34, 24);
            this.guna2ControlBox1.TabIndex = 0;
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHelp.BackColor = System.Drawing.Color.Transparent;
            this.btnHelp.BorderRadius = 10;
            this.btnHelp.Cursor = System.Windows.Forms.Cursors.Help;
            this.btnHelp.FillColor = System.Drawing.Color.Transparent;
            this.btnHelp.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnHelp.ForeColor = System.Drawing.Color.White;
            this.btnHelp.Location = new System.Drawing.Point(1087, 5);
            this.btnHelp.Margin = new System.Windows.Forms.Padding(2);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(34, 24);
            this.btnHelp.TabIndex = 2;
            this.btnHelp.Text = "?";
            this.helpToolTip.SetToolTip(this.btnHelp, "Görüntü Tabanlı Şifreleme Sistemi - Yardım");
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.BorderRadius = 15;
            this.guna2Panel1.Controls.Add(this.guna2TabControl1);
            this.guna2Panel1.Controls.Add(this.btnReturn);
            this.guna2Panel1.Controls.Add(this.lblTitle);
            this.guna2Panel1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.guna2Panel1.Location = new System.Drawing.Point(33, 50);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Size = new System.Drawing.Size(1134, 650);
            this.guna2Panel1.TabIndex = 6;
            // 
            // guna2TabControl1
            // 
            this.guna2TabControl1.Controls.Add(this.tabImageAnalysis);
            this.guna2TabControl1.Controls.Add(this.tabEncryptionDetails);
            this.guna2TabControl1.Controls.Add(this.tabHistogram);
            this.guna2TabControl1.Controls.Add(this.tabReport);
            this.guna2TabControl1.ItemSize = new System.Drawing.Size(120, 40);
            this.guna2TabControl1.Location = new System.Drawing.Point(24, 60);
            this.guna2TabControl1.Name = "guna2TabControl1";
            this.guna2TabControl1.SelectedIndex = 0;
            this.guna2TabControl1.Size = new System.Drawing.Size(1086, 510);
            this.guna2TabControl1.TabButtonHoverState.BorderColor = System.Drawing.Color.Empty;
            this.guna2TabControl1.TabButtonHoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(52)))), ((int)(((byte)(70)))));
            this.guna2TabControl1.TabButtonHoverState.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.guna2TabControl1.TabButtonHoverState.ForeColor = System.Drawing.Color.White;
            this.guna2TabControl1.TabButtonIdleState.BorderColor = System.Drawing.Color.Empty;
            this.guna2TabControl1.TabButtonIdleState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.guna2TabControl1.TabButtonIdleState.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.guna2TabControl1.TabButtonIdleState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(160)))), ((int)(((byte)(167)))));
            this.guna2TabControl1.TabButtonSelectedState.BorderColor = System.Drawing.Color.Empty;
            this.guna2TabControl1.TabButtonSelectedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(37)))), ((int)(((byte)(49)))));
            this.guna2TabControl1.TabButtonSelectedState.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.guna2TabControl1.TabButtonSelectedState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.guna2TabControl1.TabMenuBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.guna2TabControl1.TabMenuOrientation = Guna.UI2.WinForms.TabMenuOrientation.HorizontalTop;
            // 
            // tabImageAnalysis
            // 
            this.tabImageAnalysis.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.tabImageAnalysis.Controls.Add(this.splitContainer1);
            this.tabImageAnalysis.Location = new System.Drawing.Point(4, 44);
            this.tabImageAnalysis.Name = "tabImageAnalysis";
            this.tabImageAnalysis.Padding = new System.Windows.Forms.Padding(3);
            this.tabImageAnalysis.Size = new System.Drawing.Size(1078, 462);
            this.tabImageAnalysis.TabIndex = 0;
            this.tabImageAnalysis.Text = "Görsel Analiz";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Size = new System.Drawing.Size(1072, 400);
            this.splitContainer1.SplitterDistance = 530;
            this.splitContainer1.SplitterWidth = 10;
            this.splitContainer1.TabIndex = 0;
            // 
            // tabEncryptionDetails
            // 
            this.tabEncryptionDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.tabEncryptionDetails.Controls.Add(this.guna2GroupBox1);
            this.tabEncryptionDetails.Controls.Add(this.guna2GroupBox2);
            this.tabEncryptionDetails.Location = new System.Drawing.Point(4, 44);
            this.tabEncryptionDetails.Name = "tabEncryptionDetails";
            this.tabEncryptionDetails.Padding = new System.Windows.Forms.Padding(3);
            this.tabEncryptionDetails.Size = new System.Drawing.Size(1078, 462);
            this.tabEncryptionDetails.TabIndex = 1;
            this.tabEncryptionDetails.Text = "Şifreleme Detayları";
            // 
            // guna2GroupBox1
            // 
            this.guna2GroupBox1.BorderRadius = 10;
            this.guna2GroupBox1.Controls.Add(this.txtPassword);
            this.guna2GroupBox1.Controls.Add(this.lblPassword);
            this.guna2GroupBox1.Controls.Add(this.txtRsaEncryptedKey);
            this.guna2GroupBox1.Controls.Add(this.lblRsaEncryptedKey);
            this.guna2GroupBox1.Controls.Add(this.txtAesKey);
            this.guna2GroupBox1.Controls.Add(this.lblAesKey);
            this.guna2GroupBox1.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.guna2GroupBox1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.guna2GroupBox1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.guna2GroupBox1.ForeColor = System.Drawing.Color.White;
            this.guna2GroupBox1.Location = new System.Drawing.Point(20, 20);
            this.guna2GroupBox1.Name = "guna2GroupBox1";
            this.guna2GroupBox1.Size = new System.Drawing.Size(1038, 200);
            this.guna2GroupBox1.TabIndex = 0;
            this.guna2GroupBox1.Text = "Şifreleme Anahtarları";
            // 
            // txtPassword
            // 
            this.txtPassword.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtPassword.BorderRadius = 5;
            this.txtPassword.Cursor = System.Windows.Forms.Cursors.Hand;
            this.txtPassword.DefaultText = "";
            this.txtPassword.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtPassword.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtPassword.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtPassword.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtPassword.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtPassword.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPassword.ForeColor = System.Drawing.Color.White;
            this.txtPassword.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.txtPassword.Location = new System.Drawing.Point(135, 245);
            this.txtPassword.Multiline = true;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PlaceholderText = "";
            this.txtPassword.ReadOnly = true;
            this.txtPassword.SelectedText = "";
            this.txtPassword.Size = new System.Drawing.Size(390, 45);
            this.txtPassword.TabIndex = 10;
            // 
            // lblPassword
            // 
            this.lblPassword.BackColor = System.Drawing.Color.Transparent;
            this.lblPassword.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblPassword.ForeColor = System.Drawing.Color.White;
            this.lblPassword.Location = new System.Drawing.Point(14, 250);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(40, 17);
            this.lblPassword.TabIndex = 9;
            this.lblPassword.Text = "Parola:";
            // 
            // txtRsaEncryptedKey
            // 
            this.txtRsaEncryptedKey.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtRsaEncryptedKey.BorderRadius = 5;
            this.txtRsaEncryptedKey.Cursor = System.Windows.Forms.Cursors.Hand;
            this.txtRsaEncryptedKey.DefaultText = "";
            this.txtRsaEncryptedKey.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtRsaEncryptedKey.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtRsaEncryptedKey.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtRsaEncryptedKey.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtRsaEncryptedKey.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtRsaEncryptedKey.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtRsaEncryptedKey.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtRsaEncryptedKey.ForeColor = System.Drawing.Color.White;
            this.txtRsaEncryptedKey.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.txtRsaEncryptedKey.Location = new System.Drawing.Point(135, 195);
            this.txtRsaEncryptedKey.Multiline = true;
            this.txtRsaEncryptedKey.Name = "txtRsaEncryptedKey";
            this.txtRsaEncryptedKey.PlaceholderText = "";
            this.txtRsaEncryptedKey.ReadOnly = true;
            this.txtRsaEncryptedKey.SelectedText = "";
            this.txtRsaEncryptedKey.Size = new System.Drawing.Size(390, 45);
            this.txtRsaEncryptedKey.TabIndex = 8;
            // 
            // lblRsaEncryptedKey
            // 
            this.lblRsaEncryptedKey.BackColor = System.Drawing.Color.Transparent;
            this.lblRsaEncryptedKey.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblRsaEncryptedKey.ForeColor = System.Drawing.Color.White;
            this.lblRsaEncryptedKey.Location = new System.Drawing.Point(14, 200);
            this.lblRsaEncryptedKey.Name = "lblRsaEncryptedKey";
            this.lblRsaEncryptedKey.Size = new System.Drawing.Size(112, 17);
            this.lblRsaEncryptedKey.TabIndex = 7;
            this.lblRsaEncryptedKey.Text = "RSA Şifreli Anahtar:";
            // 
            // txtAesKey
            // 
            this.txtAesKey.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtAesKey.BorderRadius = 5;
            this.txtAesKey.Cursor = System.Windows.Forms.Cursors.Hand;
            this.txtAesKey.DefaultText = "";
            this.txtAesKey.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtAesKey.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtAesKey.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtAesKey.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtAesKey.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtAesKey.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtAesKey.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtAesKey.ForeColor = System.Drawing.Color.White;
            this.txtAesKey.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.txtAesKey.Location = new System.Drawing.Point(135, 145);
            this.txtAesKey.Multiline = true;
            this.txtAesKey.Name = "txtAesKey";
            this.txtAesKey.PlaceholderText = "";
            this.txtAesKey.ReadOnly = true;
            this.txtAesKey.SelectedText = "";
            this.txtAesKey.Size = new System.Drawing.Size(390, 45);
            this.txtAesKey.TabIndex = 6;
            // 
            // lblAesKey
            // 
            this.lblAesKey.BackColor = System.Drawing.Color.Transparent;
            this.lblAesKey.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblAesKey.ForeColor = System.Drawing.Color.White;
            this.lblAesKey.Location = new System.Drawing.Point(14, 150);
            this.lblAesKey.Name = "lblAesKey";
            this.lblAesKey.Size = new System.Drawing.Size(77, 17);
            this.lblAesKey.TabIndex = 5;
            this.lblAesKey.Text = "AES Anahtarı:";
            // 
            // guna2GroupBox2
            // 
            this.guna2GroupBox2.BorderRadius = 10;
            this.guna2GroupBox2.Controls.Add(this.txtEncryptedText);
            this.guna2GroupBox2.Controls.Add(this.lblEncryptedText);
            this.guna2GroupBox2.Controls.Add(this.txtOriginalText);
            this.guna2GroupBox2.Controls.Add(this.lblOriginalText);
            this.guna2GroupBox2.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.guna2GroupBox2.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.guna2GroupBox2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.guna2GroupBox2.ForeColor = System.Drawing.Color.White;
            this.guna2GroupBox2.Location = new System.Drawing.Point(20, 230);
            this.guna2GroupBox2.Name = "guna2GroupBox2";
            this.guna2GroupBox2.Size = new System.Drawing.Size(1038, 210);
            this.guna2GroupBox2.TabIndex = 1;
            this.guna2GroupBox2.Text = "Metin Verileri";
            // 
            // txtEncryptedText
            // 
            this.txtEncryptedText.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtEncryptedText.BorderRadius = 5;
            this.txtEncryptedText.Cursor = System.Windows.Forms.Cursors.Hand;
            this.txtEncryptedText.DefaultText = "";
            this.txtEncryptedText.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtEncryptedText.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtEncryptedText.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtEncryptedText.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtEncryptedText.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtEncryptedText.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtEncryptedText.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtEncryptedText.ForeColor = System.Drawing.Color.White;
            this.txtEncryptedText.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.txtEncryptedText.Location = new System.Drawing.Point(135, 95);
            this.txtEncryptedText.Multiline = true;
            this.txtEncryptedText.Name = "txtEncryptedText";
            this.txtEncryptedText.PlaceholderText = "";
            this.txtEncryptedText.ReadOnly = true;
            this.txtEncryptedText.SelectedText = "";
            this.txtEncryptedText.Size = new System.Drawing.Size(390, 45);
            this.txtEncryptedText.TabIndex = 4;
            // 
            // txtOriginalText
            // 
            this.txtOriginalText.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtOriginalText.BorderRadius = 5;
            this.txtOriginalText.Cursor = System.Windows.Forms.Cursors.Hand;
            this.txtOriginalText.DefaultText = "";
            this.txtOriginalText.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtOriginalText.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtOriginalText.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtOriginalText.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtOriginalText.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtOriginalText.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtOriginalText.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtOriginalText.ForeColor = System.Drawing.Color.White;
            this.txtOriginalText.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.txtOriginalText.Location = new System.Drawing.Point(135, 45);
            this.txtOriginalText.Multiline = true;
            this.txtOriginalText.Name = "txtOriginalText";
            this.txtOriginalText.PlaceholderText = "";
            this.txtOriginalText.ReadOnly = true;
            this.txtOriginalText.SelectedText = "";
            this.txtOriginalText.Size = new System.Drawing.Size(390, 45);
            this.txtOriginalText.TabIndex = 2;
            // 
            // lblOriginalText
            // 
            this.lblOriginalText.BackColor = System.Drawing.Color.Transparent;
            this.lblOriginalText.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblOriginalText.ForeColor = System.Drawing.Color.White;
            this.lblOriginalText.Location = new System.Drawing.Point(14, 50);
            this.lblOriginalText.Name = "lblOriginalText";
            this.lblOriginalText.Size = new System.Drawing.Size(81, 17);
            this.lblOriginalText.TabIndex = 1;
            this.lblOriginalText.Text = "Orijinal Metin:";
            // 
            // tabHistogram
            // 
            this.tabHistogram.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.tabHistogram.Controls.Add(this.splitContainer2);
            this.tabHistogram.Location = new System.Drawing.Point(4, 44);
            this.tabHistogram.Name = "tabHistogram";
            this.tabHistogram.Padding = new System.Windows.Forms.Padding(3);
            this.tabHistogram.Size = new System.Drawing.Size(1078, 462);
            this.tabHistogram.TabIndex = 2;
            this.tabHistogram.Text = "Histogram Analizi";
            // 
            // splitContainer2
            // 
            this.splitContainer2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Size = new System.Drawing.Size(1072, 456);
            this.splitContainer2.SplitterDistance = 530;
            this.splitContainer2.SplitterWidth = 10;
            this.splitContainer2.TabIndex = 0;
            // 
            // tabReport
            // 
            this.tabReport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.tabReport.Controls.Add(this.txtLog);
            this.tabReport.Controls.Add(this.lblLog);
            this.tabReport.Controls.Add(this.btnSaveLog);
            this.tabReport.Controls.Add(this.btnAnalyze);
            this.tabReport.Location = new System.Drawing.Point(4, 44);
            this.tabReport.Name = "tabReport";
            this.tabReport.Padding = new System.Windows.Forms.Padding(3);
            this.tabReport.Size = new System.Drawing.Size(1078, 462);
            this.tabReport.TabIndex = 3;
            this.tabReport.Text = "Analiz Raporu";
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.txtLog.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.txtLog.BorderRadius = 10;
            this.txtLog.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtLog.DefaultText = "";
            this.txtLog.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtLog.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtLog.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtLog.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtLog.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.txtLog.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtLog.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtLog.ForeColor = System.Drawing.Color.White;
            this.txtLog.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtLog.Location = new System.Drawing.Point(20, 50);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.PasswordChar = '\0';
            this.txtLog.PlaceholderText = "Analiz sonuçları burada görüntülenecek...";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.SelectedText = "";
            this.txtLog.Size = new System.Drawing.Size(1038, 349);
            this.txtLog.TabIndex = 13;
            // 
            // lblLog
            // 
            this.lblLog.BackColor = System.Drawing.Color.Transparent;
            this.lblLog.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblLog.ForeColor = System.Drawing.Color.White;
            this.lblLog.Location = new System.Drawing.Point(20, 20);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(129, 19);
            this.lblLog.TabIndex = 12;
            this.lblLog.Text = "Analiz Rapor Çıktısı:";
            // 
            // btnSaveLog
            // 
            this.btnSaveLog.BackColor = System.Drawing.Color.Transparent;
            this.btnSaveLog.BorderRadius = 10;
            this.btnSaveLog.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnSaveLog.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnSaveLog.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnSaveLog.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnSaveLog.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnSaveLog.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSaveLog.ForeColor = System.Drawing.Color.White;
            this.btnSaveLog.Location = new System.Drawing.Point(719, 409);
            this.btnSaveLog.Name = "btnSaveLog";
            this.btnSaveLog.Size = new System.Drawing.Size(160, 45);
            this.btnSaveLog.TabIndex = 14;
            this.btnSaveLog.Text = "Raporu Kaydet";
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.BackColor = System.Drawing.Color.Transparent;
            this.btnAnalyze.BorderRadius = 10;
            this.btnAnalyze.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnAnalyze.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnAnalyze.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnAnalyze.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnAnalyze.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnAnalyze.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnAnalyze.ForeColor = System.Drawing.Color.White;
            this.btnAnalyze.Location = new System.Drawing.Point(200, 409);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(160, 45);
            this.btnAnalyze.TabIndex = 11;
            this.btnAnalyze.Text = "Analiz Raporunu Oluştur";
            // 
            // btnReturn
            // 
            this.btnReturn.BackColor = System.Drawing.Color.Transparent;
            this.btnReturn.BorderRadius = 10;
            this.btnReturn.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnReturn.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnReturn.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnReturn.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnReturn.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnReturn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnReturn.ForeColor = System.Drawing.Color.White;
            this.btnReturn.Location = new System.Drawing.Point(980, 590);
            this.btnReturn.Name = "btnReturn";
            this.btnReturn.Size = new System.Drawing.Size(130, 45);
            this.btnReturn.TabIndex = 10;
            this.btnReturn.Text = "Ana Menüye Dön";
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(496, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(142, 27);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "ANALİZ PANELİ";
            this.lblTitle.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbLogin
            // 
            this.pbLogin.BackColor = System.Drawing.Color.Transparent;
            this.pbLogin.FillColor = System.Drawing.Color.Transparent;
            this.pbLogin.Image = ((System.Drawing.Image)(resources.GetObject("pbLogin.Image")));
            this.pbLogin.ImageRotate = 0F;
            this.pbLogin.Location = new System.Drawing.Point(10, -2);
            this.pbLogin.Margin = new System.Windows.Forms.Padding(2);
            this.pbLogin.Name = "pbLogin";
            this.pbLogin.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.pbLogin.Size = new System.Drawing.Size(40, 40);
            this.pbLogin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbLogin.TabIndex = 7;
            this.pbLogin.TabStop = false;
            this.pbLogin.Click += new System.EventHandler(this.pbLogin_Click);
            this.pbLogin.MouseEnter += new System.EventHandler(this.pbLogin_MouseEnter);
            this.pbLogin.MouseLeave += new System.EventHandler(this.pbLogin_MouseLeave);
            // 
            // FrmAnalysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(1200, 729);
            this.Controls.Add(this.guna2Panel1);
            this.Controls.Add(this.pnlTitleBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmAnalysis";
            this.Text = "FrmAnalysis";
            this.pnlTitleBar.ResumeLayout(false);
            this.guna2Panel1.ResumeLayout(false);
            this.guna2Panel1.PerformLayout();
            this.guna2TabControl1.ResumeLayout(false);
            this.tabImageAnalysis.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabEncryptionDetails.ResumeLayout(false);
            this.tabHistogram.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabReport.ResumeLayout(false);
            this.tabReport.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogin)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
        private Guna.UI2.WinForms.Guna2Panel pnlTitleBar;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox2;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox1;
        private Guna.UI2.WinForms.Guna2Button btnHelp;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2TabControl guna2TabControl1;
        private System.Windows.Forms.TabPage tabImageAnalysis;
        private System.Windows.Forms.TabPage tabEncryptionDetails;
        private System.Windows.Forms.TabPage tabHistogram;
        private System.Windows.Forms.TabPage tabReport;
        private Guna.UI2.WinForms.Guna2Button btnReturn;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2CirclePictureBox pbLogin;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private Guna.UI2.WinForms.Guna2GroupBox guna2GroupBox1;
        private Guna.UI2.WinForms.Guna2GroupBox guna2GroupBox2;
        private Guna.UI2.WinForms.Guna2TextBox txtPassword;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblPassword;
        private Guna.UI2.WinForms.Guna2TextBox txtRsaEncryptedKey;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblRsaEncryptedKey;
        private Guna.UI2.WinForms.Guna2TextBox txtAesKey;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblAesKey;
        private Guna.UI2.WinForms.Guna2TextBox txtEncryptedText;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblEncryptedText;
        private Guna.UI2.WinForms.Guna2TextBox txtOriginalText;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblOriginalText;
        private Guna.UI2.WinForms.Guna2TextBox txtLog;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblLog;
        private Guna.UI2.WinForms.Guna2Button btnSaveLog;
        private Guna.UI2.WinForms.Guna2Button btnAnalyze;
    }
}