using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using ImageBasedEncryptionSystem.BusinessLayer.UI;
using ImageBasedEncryptionSystem.BusinessLayer;
using ImageBasedEncryptionSystem.TypeLayer;
using System.IO;
using ImageBasedEncryptionSystem.DataLayer;

namespace ImageBasedEncryptionSystem.UI.Forms
{
    public partial class FrmAdmin : Form
    {
        // Ana menüye referans
        private FrmMenu parentForm;
        
        // Geliştirici modu nesnesi
        private Cls_DeveloperMode devMode;
        
        /// <summary>
        /// Varsayılan kurucu
        /// </summary>
        public FrmAdmin()
        {
            Console.WriteLine(Debug.DEBUG_ADMIN_FORM_INITIALIZE_STARTED);
            InitializeComponent();
            
            // FormClosing olayını ekle
            this.FormClosing += FrmAdmin_FormClosing;
            Console.WriteLine(Debug.DEBUG_ADMIN_FORM_CLOSING_EVENT_BOUND);
        }
        
        /// <summary>
        /// Ana menü referansı ile kurucu
        /// </summary>
        /// <param name="parentForm">Ana menü formu</param>
        public FrmAdmin(FrmMenu parentForm) : this()
        {
            this.parentForm = parentForm;
        }
        
        /// <summary>
        /// Geliştirici modunu ayarlar
        /// </summary>
        /// <param name="developerMode">Geliştirici modu nesnesi</param>
        public void SetDeveloperMode(Cls_DeveloperMode developerMode)
        {
            try
            {
                Console.WriteLine(Debug.DEBUG_ADMIN_SET_DEV_MODE_STARTED);
                if (developerMode == null || !developerMode.IsLoggedIn)
                {
                    Console.WriteLine(Debug.DEBUG_ADMIN_DEV_MODE_ACCESS_DENIED);
                    MessageBox.Show(Errors.ERROR_DEV_MODE_ACCESS_DENIED, 
                        "Erişim Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }
                
                var (devModeMessage, isDevModeActive) = developerMode.CheckDevModeStatus();
                Console.WriteLine(Debug.DEBUG_ADMIN_DEV_MODE_STATUS_CHECKED);
                
                if (!isDevModeActive)
                {
                    Console.WriteLine(Debug.DEBUG_ADMIN_DEV_MODE_NOT_ACTIVE);
                    MessageBox.Show(Errors.ERROR_DEV_MODE_REQUIRED, 
                        "Geliştirici Modu Gerekli", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }
                
                this.devMode = developerMode;
                Console.WriteLine(Debug.DEBUG_ADMIN_DEV_MODE_SET_SUCCESS);
                
                // Geliştirici modu bilgisini UI'a yansıt
                if (lblDevModeStatus != null)
                {
                    lblDevModeStatus.Text = $"Geliştirici: {developerMode.CurrentDevId}";
                    lblDevModeStatus.ForeColor = Color.Lime;
                    Console.WriteLine(string.Format(Debug.DEBUG_ADMIN_DEV_MODE_UI_UPDATED, developerMode.CurrentDevId));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Debug.DEBUG_SYSTEM_IDENTITY_RECEIVED, ex.Message));
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void FrmAdmin_Load(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine(Debug.DEBUG_ADMIN_FORM_LOADED);
                // Form ayarları
                this.TopMost = true;
                Console.WriteLine(Debug.DEBUG_ADMIN_FORM_SETTINGS_APPLIED);
                
                // Geliştirici modu etiketi ayarla
                lblDevModeStatus.Parent = pnlTitleBar;
                lblDevModeStatus.BringToFront();
                Console.WriteLine(Debug.DEBUG_ADMIN_TITLE_BAR_SETUP);
                
                // Modern arka planı ayarla
                Cls_Background.Instance.CreateModernBackground(this);
                Console.WriteLine(Debug.DEBUG_ADMIN_BACKGROUND_CREATED);
                
                // Metin kutularını hazırla
                txtRsaKey.ReadOnly = true;
                txtIdentity.ReadOnly = true;
                Console.WriteLine(Debug.DEBUG_ADMIN_TEXT_BOXES_PREPARED);
                

                // Generate and load RSA keys into txtRsaKey
                Cls_RsaHelper.EnsureKeyPair();
                string rsaKeys = $"Private Key:\n{Cls_RsaHelper.GetPrivateKeyPem()}\n\nPublic Key:\n{Cls_RsaHelper.GetPublicKeyPem()}";
                txtRsaKey.Text = rsaKeys;
                
                // Form Load olayında yapılacak diğer işlemler...
                LoadInitialData();
                
                Console.WriteLine(Success.ADMIN_FORM_SUCCESS);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Debug.DEBUG_SYSTEM_IDENTITY_RECEIVED, ex.Message));
                MessageBox.Show(string.Format(Errors.ERROR_ADMIN_FORM_LOAD, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void LoadInitialData()
        {
            // Load SystemIdentity into txtIdentity
            txtIdentity.Text = Cls_Config.GetSystemIdentity();
        }
        
        private void FrmAdmin_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Console.WriteLine(Debug.DEBUG_ADMIN_FORM_CLOSING);
                // Form kapatıldığında dispose et
                this.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Debug.DEBUG_SYSTEM_IDENTITY_RECEIVED, ex.Message));
                MessageBox.Show(string.Format(Errors.ERROR_ADMIN_FORM_CLOSING, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnRandom_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine(Debug.DEBUG_ADMIN_RANDOM_IDENTITY_STARTED);
                // Yeni rastgele kimlik oluşturma işlemi
                Cls_IdentityCreate identityCreator = new Cls_IdentityCreate();
                int length = new Random().Next(20, 101); // 20 ile 100 arasında bir uzunluk seç
                Console.WriteLine(string.Format(Debug.DEBUG_ADMIN_RANDOM_IDENTITY_LENGTH, length));
                
                string newIdentity = identityCreator.CreateRandomIdentity(length);
                txtNewIdentity.Text = newIdentity;
                Console.WriteLine(string.Format(Debug.DEBUG_ADMIN_RANDOM_IDENTITY_CREATED, newIdentity));
                
                MessageBox.Show(Success.ADMIN_RANDOM_IDENTITY_SUCCESS, 
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Debug.DEBUG_SYSTEM_IDENTITY_RECEIVED, ex.Message));
                MessageBox.Show(string.Format(Errors.ERROR_ADMIN_RANDOM_IDENTITY, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine(Debug.DEBUG_ADMIN_SAVE_IDENTITY_STARTED);
                if (string.IsNullOrEmpty(txtNewIdentity.Text))
                {
                    Console.WriteLine(Debug.DEBUG_ADMIN_SAVE_IDENTITY_VALIDATION);
                    MessageBox.Show(Errors.ERROR_ADMIN_IDENTITY_EMPTY, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Update SystemIdentity
                bool updateResult = Cls_Config.UpdateSystemIdentity(txtNewIdentity.Text);
                if (!updateResult)
                {
                    MessageBox.Show(Errors.ERROR_ADMIN_IDENTITY_SAVE, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check for changes in SystemIdentity
                string currentIdentity = Cls_Config.GetSystemIdentity();
                if (currentIdentity != Cls_RsaHelper.GetCachedSystemIdentity())
                {
                    // Generate new RSA keys
                    Cls_RsaHelper.EnsureKeyPair();
                    string rsaKeys = $"Private Key:\n{Cls_RsaHelper.GetPrivateKeyPem()}\n\nPublic Key:\n{Cls_RsaHelper.GetPublicKeyPem()}";
                    txtRsaKey.Text = rsaKeys;

                    // Update txtIdentity with the current SystemIdentity
                    txtIdentity.Text = currentIdentity;
                }

                Console.WriteLine(Debug.DEBUG_ADMIN_IDENTITY_SAVED);
                MessageBox.Show(Success.ADMIN_IDENTITY_SAVED_SUCCESS, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Debug.DEBUG_SYSTEM_IDENTITY_RECEIVED, ex.Message));
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine(Debug.DEBUG_ADMIN_RESET_IDENTITY_STARTED);
                DialogResult result = MessageBox.Show(Errors.ERROR_ADMIN_IDENTITY_RESET_CONFIRM, "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Reset SystemIdentity to DefaultSystemIdentity
                    string defaultIdentity = Cls_Config.GetDefaultSystemIdentity();
                    bool updateResult = Cls_Config.UpdateSystemIdentity(defaultIdentity);

                    if (updateResult)
                    {
                        // Generate new RSA keys
                        Cls_RsaHelper.EnsureKeyPair();
                        string rsaKeys = $"Private Key:\n{Cls_RsaHelper.GetPrivateKeyPem()}\n\nPublic Key:\n{Cls_RsaHelper.GetPublicKeyPem()}";
                        txtRsaKey.Text = rsaKeys;

                        // Update txtIdentity with the current SystemIdentity
                        txtIdentity.Text = Cls_Config.GetSystemIdentity();

                        Console.WriteLine(Debug.DEBUG_ADMIN_IDENTITY_RESET_SUCCESS);
                        MessageBox.Show(Success.ADMIN_IDENTITY_RESET_SUCCESS, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(Errors.ERROR_ADMIN_IDENTITY_RESET, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Debug.DEBUG_SYSTEM_IDENTITY_RECEIVED, ex.Message));
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
