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
            InitializeComponent();
            
            // FormClosing olayını ekle
            this.FormClosing += FrmAdmin_FormClosing;
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
                if (developerMode == null || !developerMode.IsLoggedIn)
                {
                    MessageBox.Show(Errors.ERROR_DEV_MODE_ACCESS_DENIED, 
                        "Erişim Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }
                
                var (devModeMessage, isDevModeActive) = developerMode.CheckDevModeStatus();
                
                if (!isDevModeActive)
                {
                    MessageBox.Show(Errors.ERROR_DEV_MODE_REQUIRED, 
                        "Geliştirici Modu Gerekli", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }
                
                this.devMode = developerMode;
                
                // Geliştirici modu bilgisini UI'a yansıt
                if (lblDevModeStatus != null)
                {
                    lblDevModeStatus.Text = $"Geliştirici: {developerMode.CurrentDevId}";
                    lblDevModeStatus.ForeColor = Color.Lime;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void FrmAdmin_Load(object sender, EventArgs e)
        {
            try
            {
                // Form ayarları
                this.TopMost = true;
                
                // Geliştirici modu etiketi ayarla
                lblDevModeStatus.Parent = pnlTitleBar;
                lblDevModeStatus.BringToFront();
                
                // Modern arka planı ayarla
                Cls_Background.Instance.CreateModernBackground(this);
                
                // Metin kutularını hazırla
                txtRsaKey.ReadOnly = true;
                txtIdentity.ReadOnly = true;
                
                // Form Load olayında yapılacak diğer işlemler...
                LoadInitialData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void LoadInitialData()
        {
            try
            {
                // Mevcut kimlik bilgisini yükle
                string configFilePath = Cls_Config.GetConfigFilePath();
                if (File.Exists(configFilePath))
                {
                    string jsonContent = File.ReadAllText(configFilePath);
                    
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
                            
                            // RSA anahtarlarını yükle ve göster
                            Cls_RsaHelper.EnsureKeyPair();
                            string rsaKeys = $"Private Key:\n{Cls_RsaHelper.GetPrivateKeyPem()}\n\nPublic Key:\n{Cls_RsaHelper.GetPublicKeyPem()}";
                            txtRsaKey.Text = rsaKeys;
                        }
                        else
                        {
                            txtIdentity.Text = "Sistem_Varsayılan_Kimlik";
                        }
                    }
                    else
                    {
                        txtIdentity.Text = "Sistem_Varsayılan_Kimlik";
                    }
                }
                else
                {
                    txtIdentity.Text = "Sistem_Varsayılan_Kimlik";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtIdentity.Text = "Sistem_Varsayılan_Kimlik";
            }
        }
        
        private void FrmAdmin_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Form kapatıldığında dispose et
            this.Dispose();
        }
        
        private void btnRandom_Click(object sender, EventArgs e)
        {
            try
            {
                // Yeni rastgele kimlik oluşturma işlemi
                Cls_IdentityCreate identityCreator = new Cls_IdentityCreate();
                int length = new Random().Next(20, 101); // 20 ile 100 arasında bir uzunluk seç
                string newIdentity = identityCreator.CreateRandomIdentity(length);
                txtNewIdentity.Text = newIdentity;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Kimliği kaydetme işlemi burada yapılacak
                if (string.IsNullOrEmpty(txtNewIdentity.Text))
                {
                    MessageBox.Show("Lütfen önce yeni bir kimlik girin veya oluşturun.", 
                        "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Yeni kimliği kaydet ve RSA anahtarlarını güncelle
                string saveResult = devMode.SaveIdentityToConfig(txtNewIdentity.Text);
                if (saveResult != Success.MESSAGE_GENERAL_SAVED && saveResult != Success.MESSAGE_GENERAL_UPDATED)
                {
                    MessageBox.Show(saveResult, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Yeni RSA anahtar çifti oluştur
                Cls_RsaHelper.EnsureKeyPair();
                string rsaKeys = $"Private Key:\n{Cls_RsaHelper.GetPrivateKeyPem()}\n\nPublic Key:\n{Cls_RsaHelper.GetPublicKeyPem()}";
                txtRsaKey.Text = rsaKeys;

                MessageBox.Show("Kimlik ve RSA anahtarları başarıyla güncellendi.", 
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                // Kimliği sıfırlama işlemi burada yapılacak
                DialogResult result = MessageBox.Show("Kimliği varsayılan değere sıfırlamak istediğinize emin misiniz?", 
                    "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                
                if (result == DialogResult.Yes)
                {
                    // Geliştirici modu izni kontrol et
                    if (devMode == null || !devMode.IsDevModeActive)
                    {
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
                            }
                            else
                            {
                                // DefaultSystemIdentity bulunamadıysa SystemIdentity'yi bul
                                pattern = "\"SystemIdentity\"\\s*:\\s*\"([^\"]*)\"";
                                match = System.Text.RegularExpressions.Regex.Match(jsonContent, pattern);
                                
                                if (match.Success && match.Groups.Count > 1)
                                {
                                    defaultIdentity = match.Groups[1].Value;
                                }
                            }
                        }
                        else
                        {
                            // Config dosyası bulunamadıysa yeni bir tane oluştur
                            Cls_Config.CreateDefaultConfigFile(configFilePath);
                            
                            // Varsayılan değeri kullan
                            defaultIdentity = "VARSAYILAN_KIMLIK_TUBITAK_KSSAL_2025_pVi4-IFdJkbp_-ETi_6x-RYOd-qD_4";
                        }
                    }
                    catch (Exception readEx)
                    {
                        MessageBox.Show($"Config dosyası okunurken bir hata oluştu: {readEx.Message}", 
                            "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    
                    // Config.json dosyasına varsayılan kimliği kaydet (SystemIdentity olarak)
                    string saveResult = devMode.SaveIdentityToConfig(defaultIdentity);
                    
                    if (saveResult == Success.MESSAGE_GENERAL_SAVED || saveResult == Success.MESSAGE_GENERAL_UPDATED)
                    {
                        txtIdentity.Text = defaultIdentity;
                        txtNewIdentity.Text = "";
                        MessageBox.Show("Kimlik varsayılan değere sıfırlandı.", 
                            "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Kimlik sıfırlanırken bir hata oluştu: {saveResult}", 
                            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnHelp_Click(object sender, EventArgs e)
        {
            // Yardım butonuna tıklandığında bilgi formunu aç
            FrmInfo frmInfo = new FrmInfo();
            frmInfo.Show();
        }

    }
}
