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
            try
            {
                Console.WriteLine(Debug.DEBUG_ADMIN_INITIAL_DATA_LOAD_STARTED);
                // Mevcut kimlik bilgisini yükle
                string configFilePath = Cls_Config.GetConfigFilePath();
                Console.WriteLine(string.Format(Debug.DEBUG_ADMIN_CONFIG_FILE_PATH_OBTAINED, configFilePath));
                
                if (File.Exists(configFilePath))
                {
                    string jsonContent = File.ReadAllText(configFilePath);
                    Console.WriteLine(Debug.DEBUG_ADMIN_CONFIG_FILE_CHECKED);
                    
                    // SystemIdentity değerini kontrol ets
                    if (jsonContent.Contains("SystemIdentity"))
                    {
                        // SystemIdentity değerini çıkar
                        string pattern = "\"SystemIdentity\"\\s*:\\s*\"([^\"]*)\"";
                        var match = System.Text.RegularExpressions.Regex.Match(jsonContent, pattern);
                        
                        if (match.Success && match.Groups.Count > 1)
                        {
                            string identity = match.Groups[1].Value;
                            txtIdentity.Text = identity;
                            Console.WriteLine(string.Format(Debug.DEBUG_ADMIN_IDENTITY_EXTRACTED, identity));
                            
                            // RSA anahtarlarını yükle ve göster
                            Cls_RsaHelper.EnsureKeyPair();
                            string rsaKeys = $"Private Key:\n{Cls_RsaHelper.GetPrivateKeyPem()}\n\nPublic Key:\n{Cls_RsaHelper.GetPublicKeyPem()}";
                            txtRsaKey.Text = rsaKeys;
                            Console.WriteLine(Debug.DEBUG_ADMIN_RSA_KEYS_LOADED);
                        }
                        else
                        {
                            txtIdentity.Text = "Sistem_Varsayılan_Kimlik";
                            Console.WriteLine(Debug.DEBUG_ADMIN_DEFAULT_IDENTITY_USED);
                        }
                    }
                    else
                    {
                        txtIdentity.Text = "Sistem_Varsayılan_Kimlik";
                        Console.WriteLine(Debug.DEBUG_ADMIN_DEFAULT_IDENTITY_USED);
                    }
                }
                else
                {
                    txtIdentity.Text = "Sistem_Varsayılan_Kimlik";
                    Console.WriteLine(Debug.DEBUG_ADMIN_DEFAULT_IDENTITY_USED);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Debug.DEBUG_SYSTEM_IDENTITY_RECEIVED, ex.Message));
                MessageBox.Show(string.Format(Errors.ERROR_ADMIN_INITIAL_DATA_LOAD, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtIdentity.Text = "Sistem_Varsayılan_Kimlik";
            }
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
                // Kimliği kaydetme işlemi burada yapılacak
                if (string.IsNullOrEmpty(txtNewIdentity.Text))
                {
                    Console.WriteLine(Debug.DEBUG_ADMIN_SAVE_IDENTITY_VALIDATION);
                    MessageBox.Show(Errors.ERROR_ADMIN_IDENTITY_EMPTY, 
                        "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Yeni kimliği kaydet ve RSA anahtarlarını güncelle
                string saveResult = devMode.SaveIdentityToConfig(txtNewIdentity.Text);
                if (saveResult != Success.MESSAGE_GENERAL_SAVED && saveResult != Success.MESSAGE_GENERAL_UPDATED)
                {
                    MessageBox.Show(string.Format(Errors.ERROR_ADMIN_IDENTITY_SAVE, saveResult), 
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Console.WriteLine(Debug.DEBUG_ADMIN_IDENTITY_SAVED);

                // Yeni RSA anahtar çifti oluştur
                Cls_RsaHelper.EnsureKeyPair();
                string rsaKeys = $"Private Key:\n{Cls_RsaHelper.GetPrivateKeyPem()}\n\nPublic Key:\n{Cls_RsaHelper.GetPublicKeyPem()}";
                txtRsaKey.Text = rsaKeys;
                Console.WriteLine(Debug.DEBUG_ADMIN_RSA_KEYS_UPDATED);

                MessageBox.Show(Success.ADMIN_IDENTITY_SAVED_SUCCESS, 
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Debug.DEBUG_SYSTEM_IDENTITY_RECEIVED, ex.Message));
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine(Debug.DEBUG_ADMIN_RESET_IDENTITY_STARTED);
                // Kimliği sıfırlama işlemi burada yapılacak
                DialogResult result = MessageBox.Show(Errors.ERROR_ADMIN_IDENTITY_RESET_CONFIRM, 
                    "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                
                Console.WriteLine(Debug.DEBUG_ADMIN_RESET_CONFIRMATION);
                
                if (result == DialogResult.Yes)
                {
                    // Geliştirici modu izni kontrol et
                    if (devMode == null || !devMode.IsDevModeActive)
                    {
                        Console.WriteLine(Debug.DEBUG_ADMIN_RESET_DEV_MODE_CHECKED);
                        MessageBox.Show(Errors.ERROR_DEV_MODE_REQUIRED, 
                            "Geliştirici Modu Gerekli", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Config.json dosyasının varsayılan değerini oku
                    string configFilePath = Cls_Config.GetConfigFilePath();
                    string defaultIdentity = "pVi4-IFdJkbp_-ETi_6x-RYOd-qD_4"; // Varsayılan değer (farklı bir değere ulaşılamazsa)
                    
                    try
                    {
                        if (File.Exists(configFilePath))
                        {
                            string jsonContent = File.ReadAllText(configFilePath);
                            
                            // DefaultSystemIdentity değerini çıkar
                            string pattern = "\"DefaultSystemIdentity\"\\s*:\\s*\"([^\"]*)\"";
                            var match = System.Text.RegularExpressions.Regex.Match(jsonContent, pattern);
                            
                            if (match.Success && match.Groups.Count > 1)
                            {
                                defaultIdentity = match.Groups[1].Value;
                                Console.WriteLine(string.Format(Debug.DEBUG_ADMIN_DEFAULT_IDENTITY_LOADED, defaultIdentity));
                            }
                            else
                            {
                                // DefaultSystemIdentity bulunamadıysa SystemIdentity'yi bul
                                pattern = "\"SystemIdentity\"\\s*:\\s*\"([^\"]*)\"";
                                match = System.Text.RegularExpressions.Regex.Match(jsonContent, pattern);
                                
                                if (match.Success && match.Groups.Count > 1)
                                {
                                    defaultIdentity = match.Groups[1].Value;
                                    Console.WriteLine(string.Format(Debug.DEBUG_ADMIN_DEFAULT_IDENTITY_LOADED, defaultIdentity));
                                }
                            }
                        }
                        else
                        {
                            // Config dosyası bulunamadıysa yeni bir tane oluştur
                            Cls_Config.CreateDefaultConfigFile(configFilePath);
                            
                            // Varsayılan değeri kullan
                            defaultIdentity = "VARSAYILAN_KIMLIK_TUBITAK_KSSAL_2025_pVi4-IFdJkbp_-ETi_6x-RYOd-qD_4";
                            Console.WriteLine(string.Format(Debug.DEBUG_ADMIN_DEFAULT_IDENTITY_LOADED, defaultIdentity));
                        }
                    }
                    catch (Exception readEx)
                    {
                        Console.WriteLine(string.Format(Debug.DEBUG_SYSTEM_IDENTITY_RECEIVED, readEx.Message));
                        MessageBox.Show(string.Format(Errors.ERROR_ADMIN_DEFAULT_IDENTITY_READ, readEx.Message), 
                            "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    
                    // Config.json dosyasına varsayılan kimliği kaydet (SystemIdentity olarak)
                    string saveResult = devMode.SaveIdentityToConfig(defaultIdentity);
                    
                    if (saveResult == Success.MESSAGE_GENERAL_SAVED || saveResult == Success.MESSAGE_GENERAL_UPDATED)
                    {
                        txtIdentity.Text = defaultIdentity;
                        txtNewIdentity.Text = "";
                        Console.WriteLine(Debug.DEBUG_ADMIN_IDENTITY_RESET_SUCCESS);
                        MessageBox.Show(Success.ADMIN_IDENTITY_RESET_SUCCESS, 
                            "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(string.Format(Errors.ERROR_ADMIN_IDENTITY_RESET, saveResult), 
                            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Debug.DEBUG_SYSTEM_IDENTITY_RECEIVED, ex.Message));
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnHelp_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine(Debug.DEBUG_ADMIN_HELP_BUTTON_CLICKED);
                // Yardım butonuna tıklandığında bilgi formunu aç
                FrmInfo frmInfo = new FrmInfo();
                frmInfo.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Debug.DEBUG_SYSTEM_IDENTITY_RECEIVED, ex.Message));
                MessageBox.Show(string.Format(Errors.ERROR_HELP_FORM, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
