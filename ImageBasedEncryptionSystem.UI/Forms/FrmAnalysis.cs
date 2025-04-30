using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using System.IO;
using ImageBasedEncryptionSystem.TypeLayer;
using ImageBasedEncryptionSystem.BusinessLayer;
using ImageBasedEncryptionSystem.BusinessLayer.UI;
using ImageBasedEncryptionSystem.UI.Forms;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace ImageBasedEncryptionSystem.UI.Forms
{
    public partial class FrmAnalysis : Form
    {
        private Form _owner;
        private Bitmap _originalImage;
        private Bitmap _encryptedImage;
        private string _originalText;
        private string _encryptedText;
        private string _aesKey;
        private string _rsaEncryptedKey;
        private string _password;
        
        // Geliştirici modu durumu
        private bool isDeveloperModeActive = false;
        private Guna2HtmlLabel lblDevMode;

        private object _image;
        private string _username;

        public FrmAnalysis(Form owner = null)
        {
            InitializeComponent();
            _owner = owner;
            
            // FormClosing olayını ekle
            this.FormClosing += FrmAnalysis_FormClosing;
            
            // Form yükleme olayını ekle
            this.Load += FrmAnalysis_Load;
        }

        /// <summary>
        /// Form yüklenirken çağrılır
        /// </summary>
        private void FrmAnalysis_Load(object sender, EventArgs e)
        {
            // Modern arka planı ayarla
            Cls_Background.Instance.CreateModernBackground(this);
            
            // Analiz sekmesi varsayılan olarak seçilsin
            if (guna2TabControl1 != null)
            {
                guna2TabControl1.SelectedIndex = 0;
            }
        }
        
        /// <summary>
        /// Form kapatılırken çağrılır
        /// </summary>
        private void FrmAnalysis_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Ana forma dön
            if (_owner != null && !_owner.IsDisposed)
            {
                _owner.Show();
            }
        }
        
        // pbLogin hover efektleri
        private void pbLogin_MouseEnter(object sender, EventArgs e)
        {
            // Sadece büyüme efekti, kutu veya renk değişimi olmadan
            pbLogin.Size = new Size(44, 44);     // Hafif büyüme
            pbLogin.Location = new Point(8, -4); // Merkezi korumak için konum ayarla
            this.Cursor = Cursors.Hand;
        }

        private void pbLogin_MouseLeave(object sender, EventArgs e)
        {
            // Normal boyuta dönme
            pbLogin.Size = new Size(40, 40);     // Orijinal boyut
            pbLogin.Location = new Point(10, -2); // Orijinal konum
            this.Cursor = Cursors.Default;
        }
        
        private void pbLogin_Click(object sender, EventArgs e)
        {
        }
        
        private void btnHelp_Click(object sender, EventArgs e)
        {
            // Yardım butonuna animasyon ekleme
            Guna2Button helpButton = (Guna2Button)sender;
            
            // Animasyon başlangıcı - büyütme efekti
            var originalSize = helpButton.Size;
            var originalFont = helpButton.Font;
            
            // Büyütme efekti
            helpButton.Size = new Size(originalSize.Width + 5, originalSize.Height + 5);
            helpButton.Font = new Font(originalFont.FontFamily, originalFont.Size + 2f, originalFont.Style);
            helpButton.ForeColor = Color.Gold;
            
            // Animasyon için timer
            System.Windows.Forms.Timer animTimer = new System.Windows.Forms.Timer();
            animTimer.Interval = 300;
            animTimer.Tick += (s, args) => {
                // Efekti geri al
                helpButton.Size = originalSize;
                helpButton.Font = originalFont;
                helpButton.ForeColor = Color.White;
                
                animTimer.Stop();
                animTimer.Dispose();
                
                // Yardım formunu göster
                FrmInfo frmInfo = new FrmInfo();
                frmInfo.StartPosition = FormStartPosition.CenterScreen;
                frmInfo.Show();
            };
            
            animTimer.Start();
        }
    }
} 