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
using System.Security.Cryptography;
using ImageBasedEncryptionSystem.BusinessLayer;
using ImageBasedEncryptionSystem.UI;
using ImageBasedEncryptionSystem.TypeLayer;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace ImageBasedEncryptionSystem.UI.Forms
{
    public partial class FrmMenu : Form
    {
        // Seçilen resmin yolunu tutacak değişken
        private string selectedImagePath = string.Empty;

        // Business Layer sınıflarının örnekleri
        private Cls_AesEncrypt aesEncrypt;
        private Cls_AesDecrypt aesDecrypt;
        private Cls_RsaEncrypt rsaEncrypt;
        private Cls_RsaDecrypt rsaDecrypt;
        private Cls_WaveletEncrypt waveletEncrypt;
        private Cls_WaveletDecrypt waveletDecrypt;
        
        // Geliştirici modu değişkenleri
        private Cls_DeveloperMode devMode;
        private bool isDevModeActive = false;
        
        // Dışarıdan erişim için gerekli özellikler
        public Guna2Panel TitleBarPanel => pnlTitleBar;
        public Guna2Button AnalysisButton => btnAnalysis;
        public Cls_RsaEncrypt RsaEncrypt => rsaEncrypt;

        // Form başlatılırken kullanılacak metot
        public FrmMenu()
        {
            InitializeComponent();
            
            // Geliştirici modu nesnesini başlat
            devMode = new Cls_DeveloperMode();
            
            // Business Layer sınıflarını başlat
            InitializeEncryptionClasses();
            
            // Metin girişi için maksimum karakter sınırı ayarla
            txtInput.MaxLength = 10000; // Maksimum 10.000 karakter
        }

        /// <summary>
        /// Şifreleme için kullanılan tüm sınıfları başlatır
        /// </summary>
        private void InitializeEncryptionClasses()
        {
            try
            {
                // Sınıfları başlat
                rsaEncrypt = new Cls_RsaEncrypt();
                rsaDecrypt = new Cls_RsaDecrypt(rsaEncrypt);
                aesEncrypt = new Cls_AesEncrypt();
                aesDecrypt = new Cls_AesDecrypt();
                waveletEncrypt = new Cls_WaveletEncrypt();
                waveletDecrypt = new Cls_WaveletDecrypt();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Şifreleme sınıfları başlatılırken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Form yükleme işlemi
        private void FrmMenu_Load(object sender, EventArgs e)
        {
            try
            {
                // Form ayarları
                txtOutput.ReadOnly = true;

                // Arka plan oluştur - Business Layer'daki Cls_Background sınıfını kullan
                BusinessLayer.UI.Cls_Background.Instance.CreateModernBackground(this);

                // Olayları bağla
                btnDevMode.Click += btnDevMode_Click;
                btnAdmin.Click += btnAdmin_Click;
                btnInfo.Click += btnHelp_Click;

                // Geliştirici modu durumunu kontrol et ve UI'ı güncelle
                CheckDeveloperModeStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// Resim seçme butonu işlevi
        /// </summary>
        private void btnImage_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Title = "Resim Seçin";
                    openFileDialog.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp";
                    openFileDialog.Multiselect = false;
                    
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        selectedImagePath = openFileDialog.FileName;
                        
                        // Resmi PictureBox'a yükle
                        using (var stream = File.OpenRead(selectedImagePath))
                        {
                            pboxImage.Image = Image.FromStream(stream);
                        }
                        
                        // Şifre çözme butonu etkinleştir
                        UpdateDecryptButtonStatus();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message),
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Şifreleme butonu işlevi
        /// </summary>
        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            // Şifreleme işlemi sırasında kullanılacak nesneler
            Bitmap originalImage = null;
            Bitmap resultImage = null;
            
            try
            {
                // Gerekli alanları kontrol et
                if (string.IsNullOrEmpty(txtInput.Text))
                {
                    MessageBox.Show(Errors.ERROR_TEXT_REQUIRED, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (string.IsNullOrEmpty(txtPassword.Text))
                {
                    MessageBox.Show(Errors.ERROR_PASSWORD_REQUIRED, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (string.IsNullOrEmpty(selectedImagePath) || pboxImage.Image == null)
                {
                    MessageBox.Show(Errors.ERROR_IMAGE_REQUIRED, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Parola güvenliği kontrolü
                if (!IsPasswordSecure(txtPassword.Text))
                {
                    string errorMessage = "Parolanız güvenli değil. Lütfen şu kriterleri sağlayan bir parola girin:\n\n";
                    errorMessage += "- En az 8 karakter uzunluğunda olmalı\n";
                    errorMessage += "- En az bir harf içermeli\n";
                    errorMessage += "- En az bir rakam içermeli\n";
                    errorMessage += "- En az bir özel karakter içermeli (örn: !@#$%^&*)";
                    
                    MessageBox.Show(errorMessage, 
                        "Parola Güvenliği", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // İlerleme göstergesi görünür yap
                ShowProgress(true, "Veri şifreleniyor ve gizleniyor...");
                
                // Orijinal görüntüyü alma ve formatını kontrol etme
                try 
                {
                    // Orijinal görüntüyü al
                    originalImage = new Bitmap(pboxImage.Image);
                    
                    // Görüntünün boyutunu kontrol et - wavelet için minimum 256x256
                    if (originalImage.Width < 256 || originalImage.Height < 256)
                    {
                        ShowProgress(false);
                        MessageBox.Show("Seçilen resim çok küçük. En az 256x256 boyutunda bir resim kullanın.", 
                            "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ShowProgress(false);
                    MessageBox.Show($"Görüntü işlenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                // Veriyi şifrele ve görsele gizle
                try 
                {
                    // Wavelet sınıfını kullanarak şifreleme ve veri gizleme işlemi
                    resultImage = waveletEncrypt.HideData(txtInput.Text, originalImage, txtPassword.Text);
                    
                    // Orijinal görüntüye artık ihtiyacımız yok
                    if (originalImage != null && originalImage != pboxImage.Image)
                    {
                        originalImage.Dispose();
                        originalImage = null;
                    }
                }
                catch (Exception ex)
                {
                    ShowProgress(false);
                    
                    if (ex.Message.Contains(Errors.ERROR_DATA_TOO_LARGE))
                    {
                        MessageBox.Show("Gizlenecek veri boyutu seçilen resim için çok büyük. Lütfen daha kısa bir metin veya daha büyük bir resim kullanın.", 
                            "Veri Boyutu Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"Veri gizleme işlemi başarısız: {ex.Message}", 
                            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
                
                // Sonuçları göster
                pboxImage.Image = resultImage;
                
                // Kaydet seçeneği sun
                if (SaveResultImage(resultImage))
                {
                    ShowProgress(false);
                    MessageBox.Show(Success.ENCRYPT_SUCCESS_DETAILED, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    ShowProgress(false);
                    MessageBox.Show("İşlem tamamlandı fakat görüntü kaydedilmedi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ShowProgress(false);
                MessageBox.Show($"Şifreleme işlemi sırasında beklenmeyen bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Temizlik işlemleri
                if (originalImage != null && originalImage != pboxImage.Image)
                {
                    originalImage.Dispose();
                }
                
                // resultImage zaten pboxImage.Image'a atandığı için burada dispose etmemeliyiz
            }
        }

        /// <summary>
        /// Şifre çözme butonu işlevi
        /// </summary>
        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            try
            {
                // Gerekli alanları kontrol et
                if (string.IsNullOrEmpty(txtPassword.Text))
                {
                    MessageBox.Show(Errors.ERROR_PASSWORD_REQUIRED, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (string.IsNullOrEmpty(selectedImagePath) || pboxImage.Image == null)
                {
                    MessageBox.Show(Errors.ERROR_IMAGE_REQUIRED, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // İlerleme göstergesi görünür yap
                ShowProgress(true, "Veri çıkarılıyor ve şifre çözülüyor...");
                
                // Görüntüden veri çıkar ve şifre çöz
                string decryptedText = waveletDecrypt.ExtractData((Bitmap)pboxImage.Image, txtPassword.Text);
                
                // Sonucu göster
                txtOutput.Text = decryptedText;
                ShowProgress(false);
                
                if (string.IsNullOrEmpty(decryptedText))
                {
                    MessageBox.Show("Görüntüden veri çıkarılamadı veya şifre çözülemedi.", 
                        "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(Success.DECRYPT_SUCCESS_GENERAL, "Başarılı", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ShowProgress(false);
                MessageBox.Show($"Şifre çözme işlemi sırasında bir hata oluştu: {ex.Message}", 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Şifre çözme butonunun durumunu günceller
        /// </summary>
        private void UpdateDecryptButtonStatus()
        {
            btnDecrypt.Enabled = !string.IsNullOrEmpty(selectedImagePath) && !string.IsNullOrEmpty(txtPassword.Text);
        }

        /// <summary>
        /// İlerleme göstergesini göster/gizle
        /// </summary>
        private void ShowProgress(bool visible, string message = "")
        {
            // Basit bir mesaj gösterimi
            if (visible && !string.IsNullOrEmpty(message))
            {
                this.Text = message;
            }
            else
            {
                this.Text = "Image Based Encryption System";
            }
        }

        /// <summary>
        /// Sonuç görüntüsünü kaydet
        /// </summary>
        private bool SaveResultImage(Bitmap image)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Title = "Sonuç Görüntüsünü Kaydet";
                    saveFileDialog.Filter = "PNG Dosyası|*.png|JPEG Dosyası|*.jpg|BMP Dosyası|*.bmp";
                    saveFileDialog.DefaultExt = "png";
                    saveFileDialog.FileName = "şifreli_görsel";
                    
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Dosya formatını belirleme
                        ImageFormat format = ImageFormat.Png;
                        string extension = Path.GetExtension(saveFileDialog.FileName).ToLower();
                        
                        if (extension == ".jpg" || extension == ".jpeg")
                            format = ImageFormat.Jpeg;
                        else if (extension == ".bmp")
                            format = ImageFormat.Bmp;
                        
                        // Görüntüyü kaydet
                        image.Save(saveFileDialog.FileName, format);
                        return true;
                    }
                }
                
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Görüntü kaydedilirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Analiz butonuna tıklandığında FrmAnalysis formunu açar veya geliştirici modu kontrolü yapar
        /// </summary>
        private void btnAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                // Geliştirici girişi var ama mod aktif değilse
                if (devMode != null)
                {
                    var (loginMessage, isLoggedIn) = devMode.CheckLoginStatus();
                    var (devModeMessage, isDevModeActive) = devMode.CheckDevModeStatus();
                    
                    if (isLoggedIn && !isDevModeActive)
                    {
                        // Geliştirici girişi yapılmış ama mod aktif değil
                        MessageBox.Show("Bu özelliği kullanabilmek için önce geliştirici modunu aktif etmelisiniz.", 
                            "Geliştirici Modu Gerekli", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Yeterli izin var, analiz formunu göster
                // Yardım formunu göster
                // TODO: FrmAnalysis formunu göster
                FrmAnalysis frmAnalysis = new FrmAnalysis(this);
                frmAnalysis.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnHelp_Click(object sender, EventArgs e)
        {
            try
            {
                // Yardım formunu göster
                FrmInfo frmInfo = new FrmInfo(this);
                frmInfo.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Yardım formu açılırken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// Geliştirici modu etiketi tıklandığında modu aktifleştirir veya devre dışı bırakır
        /// </summary>
        private void btnDevMode_Click(object sender, EventArgs e)
        {
            try
            {
                // Geliştirici modunu kontrol et
                if (devMode == null)
                    return;
                
                var (loginMessage, isLoggedIn) = devMode.CheckLoginStatus();
                
                if (!isLoggedIn)
                    return;
                    
                var (devModeMessage, isDevModeActive) = devMode.CheckDevModeStatus();
                
                if (isDevModeActive)
                {
                    // Modu devre dışı bırak
                    string result = devMode.DeactivateDevMode();
                    MessageBox.Show(result, "Geliştirici Modu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Modu etkinleştir
                    string result = devMode.ActivateDevMode();
                    MessageBox.Show(result, "Geliştirici Modu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
                // UI'ı güncelle
                CheckDeveloperModeStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// Login ikonu tıklandığında giriş diyalogu gösterir ya da çıkış yapar
        /// </summary>
        private void pbLogin_Click(object sender, EventArgs e)
        {
            try
            {
                // Geliştirici girişi var mı kontrol et
                if (devMode != null)
                {
                    var (loginMessage, isLoggedIn) = devMode.CheckLoginStatus();
                    
                    if (isLoggedIn)
                    {
                        // Çıkış onay mesajı göster
                        DialogResult result = MessageBox.Show(
                            "Geliştirici hesabından çıkış yaparsanız Geliştirici Modu kapatılacaktır. Yine de devam etmek istiyor musunuz?", 
                            "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            
                        if (result == DialogResult.Yes)
                        {
                            // Geliştirici modundan çıkış yap
                            string logoutResult = devMode.Logout();
                            MessageBox.Show("Geliştirici Hesabından Çıkış Yaptınız", 
                                "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                
                            // UI'ı güncelle
                            CheckDeveloperModeStatus();
                        }
                        
                        return;
                    }
                }
                
                // Giriş yok, login formunu göster
                FrmLogin loginForm = new FrmLogin(this);
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Giriş başarılı, UI'ı güncelle
                    CheckDeveloperModeStatus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        // Login ikonu üzerine gelindiğinde efekt
        private void pbLogin_MouseEnter(object sender, EventArgs e)
        {
            // Fare üzerine geldiğinde efekt (büyütme veya parlaklık değişimi)
            pbLogin.Size = new Size(45, 45);
            pbLogin.Location = new Point(8, -4);
        }
        
        // Login ikonundan ayrılındığında efekt
        private void pbLogin_MouseLeave(object sender, EventArgs e)
        {
            // Normal boyuta dön
            pbLogin.Size = new Size(40, 40);
            pbLogin.Location = new Point(10, -2);
        }
        
        /// <summary>
        /// Geliştirici modunu etkinleştirir
        /// </summary>
        /// <param name="developerMode">Giriş yapmış geliştirici modu nesnesi</param>
        public void ActivateDeveloperMode(Cls_DeveloperMode developerMode)
        {
            try
            {
                if (developerMode == null)
                {
                    MessageBox.Show("Geliştirici modu nesnesi boş!", "Hata", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                // Giriş yapılıp yapılmadığını kontrol et
                var (loginMessage, isLoggedIn) = developerMode.CheckLoginStatus();
                if (!isLoggedIn)
                {
                    MessageBox.Show(loginMessage, "Hata", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Geliştirici modu nesnesini kaydet
                this.devMode = developerMode;
                
                // Geliştirici modunu etkinleştir
                string activateResult = developerMode.ActivateDevMode();
                
                // Başarı mesajı göster
                MessageBox.Show(Success.DEV_MODE_LOGIN_SUCCESS, "Başarılı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // UI'ı güncelle
                CheckDeveloperModeStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_DEV_MODE_ACTIVATE, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// Admin butonuna tıklandığında yönetim sayfasını açar veya uyarı gösterir
        /// </summary>
        private void btnAdmin_Click(object sender, EventArgs e)
        {
            try
            {
                // Geliştirici girişi kontrol et
                if (devMode == null)
                {
                    // Giriş yapılmamış
                    MessageBox.Show(Errors.ERROR_DEV_MODE_ACCESS_DENIED, 
                        "Erişim Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Giriş yapılıp yapılmadığını kontrol et
                var (loginMessage, isLoggedIn) = devMode.CheckLoginStatus();
                
                if (!isLoggedIn)
                {
                    // Giriş yapılmamış
                    MessageBox.Show(Errors.ERROR_DEV_MODE_ACCESS_DENIED, 
                        "Erişim Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Geliştirici modu aktif mi kontrol et
                var (devModeMessage, isDevModeActive) = devMode.CheckDevModeStatus();
                
                if (!isDevModeActive)
                {
                    // Geliştirici modu aktif değil - uyarı göster
                    MessageBox.Show(Errors.ERROR_DEV_MODE_REQUIRED, 
                        "Geliştirici Modu Gerekli", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Geliştirici modu aktif - Admin sayfasını aç
                OpenFrmAdmin();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// Admin formunu açar
        /// </summary>
        private void OpenFrmAdmin()
        {
            try
            {
                // Yeni form oluştur
                FrmAdmin adminForm = new FrmAdmin(this);
                
                // Geliştirici modu nesnesini aktar
                adminForm.SetDeveloperMode(devMode);
                
                // Formu göster
                adminForm.ShowDialog(this);
                
                // Formu gösterdikten sonra UI güncellemesi yapılabilir
                CheckDeveloperModeStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// Geliştirici modu durumunu kontrol eder ve UI bileşenlerini günceller
        /// </summary>
        private void CheckDeveloperModeStatus()
        {
            try
            {
                // Geliştirici modu var mı kontrol et
                if (devMode == null)
                {
                    // Geliştirici modu nesnesi yok
                    btnDevMode.Visible = false;
                    btnAdmin.Visible = false;
                    btnAnalysis.Visible = false;
                    this.Text = "Resim Tabanlı Şifreleme Sistemi";
                    return;
                }
                
                // Giriş yapılmış mı kontrol et
                var (loginMessage, isLoggedIn) = devMode.CheckLoginStatus();
                
                if (!isLoggedIn)
                {
                    // Giriş yapılmamış
                    btnDevMode.Visible = false;
                    btnAdmin.Visible = false;
                    btnAnalysis.Visible = false;
                    this.Text = "Resim Tabanlı Şifreleme Sistemi";
                    return;
                }
                
                // Geliştirici modu aktif mi kontrol et
                var (devModeMessage, isDevModeActive) = devMode.CheckDevModeStatus();
                
                // Geliştirici moduna göre UI'ı güncelle
                if (isDevModeActive)
                {
                    // Developer Mode Butonu Ayarları - Geliştirici modu aktif
                    btnDevMode.Text = $"Geliştirici Modu: AKTİF - {devMode.CurrentDevId}";
                    btnDevMode.ForeColor = Color.White;
                    btnDevMode.FillColor = Color.DarkGreen;
                    btnDevMode.Visible = true;

                    // Admin butonu ayarları  - Geliştirici modu aktif
                    btnAdmin.Visible = true;
                    btnAdmin.Text = "Admin";
                    btnAdmin.ForeColor = Color.White;
                    btnAdmin.FillColor = Color.FromArgb(0, 122, 204); // Mavi renk


                    // Analiz Butonu Ayarları  - Geliştirici modu aktif
                    btnAnalysis.Visible = true;
                    btnAdmin.ForeColor = Color.White;
                    btnAdmin.FillColor = Color.FromArgb(0, 122, 204);
                    this.Text = "Resim Tabanlı Şifreleme Sistemi - Geliştirici Modu";
                }
                else
                {
                    // Developer Mode Butonu Ayarları - Geliştirici modu aktif değil
                    btnDevMode.Text = $"Geliştirici Modu: KAPALI - {devMode.CurrentDevId}";
                    btnDevMode.ForeColor = Color.White;
                    btnDevMode.FillColor = Color.DarkRed;
                    btnDevMode.Visible = true;

                    // Admin butonu ayarları - Geliştirici modu aktif değil
                    btnAdmin.Visible = true;
                    btnAdmin.Text = "Admin";
                    btnAdmin.ForeColor = Color.White;
                    btnAdmin.FillColor = Color.DarkGray; // Gri renk


                    // Analiz Butonu Ayarları - Geliştirici modu aktif değil
                    btnAnalysis.Visible = true;
                    btnAnalysis.ForeColor = Color.White;
                    btnAnalysis.FillColor = Color.DarkGray; // Gri renk
                    this.Text = "Resim Tabanlı Şifreleme Sistemi";
                }
                
                // Tool tip güncelle
                loginToolTip.SetToolTip(pbLogin, isLoggedIn ? 
                    $"Çıkış yap ({devMode.CurrentDevId})" : 
                    "Geliştirici Girişi");
                
                // Geliştirici modu butonu için tooltip
                loginToolTip.SetToolTip(btnDevMode, isDevModeActive ? 
                    "Geliştirici modunu devre dışı bırakmak için tıklayın" : 
                    "Geliştirici modunu aktif etmek için tıklayın");
                    
                // Admin butonu için tooltip
                loginToolTip.SetToolTip(btnAdmin, isDevModeActive ? 
                    "Yönetim panelini açmak için tıklayın" : 
                    "Bu butonu kullanabilmek için önce geliştirici modunu etkinleştirin");

            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Şifre güvenliği kontrolü
        /// </summary>
        private bool IsPasswordSecure(string password)
        {
            // Şifre en az 8 karakter uzunluğunda olmalı
            if (password.Length < 8)
            {
                System.Diagnostics.Debug.WriteLine($"Parola uzunluğu yetersiz: {password.Length} karakter");
                return false;
            }

            // Şifre en az bir harf, bir rakam ve bir özel karakter içermelidir
            bool hasLetter = false;
            bool hasDigit = false;
            bool hasSpecialChar = false;

            foreach (char c in password)
            {
                if (char.IsLetter(c))
                    hasLetter = true;
                else if (char.IsDigit(c))
                    hasDigit = true;
                else if (!char.IsLetterOrDigit(c))
                    hasSpecialChar = true;
            }

            // Hata ayıklama mesajları
            if (!hasLetter)
                System.Diagnostics.Debug.WriteLine("Parola harf içermiyor");
            if (!hasDigit)
                System.Diagnostics.Debug.WriteLine("Parola rakam içermiyor");
            if (!hasSpecialChar)
                System.Diagnostics.Debug.WriteLine("Parola özel karakter içermiyor");

            return hasLetter && hasDigit && hasSpecialChar;
        }

        /// <summary>
        /// Standart hata mesajı gösterir
        /// </summary>
        private void ShowPasswordError()
        {
            MessageBox.Show(Errors.ERROR_PASSWORD_WRONG, "Şifre Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Wavelet ComboBox'larını ayarlar
        /// </summary>
        private void SetupWaveletComboBoxes()
        {
            // Bu metot UI'da wavelet ayarları için ComboBox'ları hazırlar
            // Geliştirici Modunda aktif olacak şekilde ayarlanabilir
        }

    }
}


