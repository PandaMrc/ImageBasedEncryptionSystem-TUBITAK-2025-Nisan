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
using ImageBasedEncryptionSystem.BusinessLayer.Helpers;

namespace ImageBasedEncryptionSystem.UI.Forms
{
    public partial class FrmMenu : Form
    {
        // Seçilen resmin yolunu tutacak değişken
        private string selectedImagePath = string.Empty;

        // Business Layer sınıflarının örnekleri

        
        // Geliştirici modu değişkenleri
        private Cls_DeveloperMode devMode;
        private bool isDevModeActive = false;
        
        // Dışarıdan erişim için gerekli özellikler
        public Guna2Panel TitleBarPanel => pnlTitleBar;
        public Guna2Button AnalysisButton => btnAnalysis;


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
                // Resim seçme işlemi
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    MessageBox.Show(Errors.ERROR_GENERAL_CANCELED, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Bitmap selectedImage = new Bitmap(openFileDialog.FileName);
                Console.WriteLine(string.Format(TypeLayer.Debug.DEBUG_IMAGE_LOAD_STARTED, openFileDialog.FileName));

                // Resim boyut kontrolü
                if (selectedImage.Width < 256 || selectedImage.Height < 256)
                {
                    MessageBox.Show(Errors.ERROR_IMAGE_TOO_SMALL, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Resmi PictureBox'a yükle
                pboxImage.Image = selectedImage;
                selectedImagePath = openFileDialog.FileName;
                Console.WriteLine(string.Format(TypeLayer.Debug.DEBUG_IMAGE_LOAD_SUCCESS, selectedImagePath));

                MessageBox.Show(Success.IMAGE_LOAD_SUCCESS, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Şifreleme butonu işlevi
        /// </summary>
        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure RSA Key Pair
                Cls_RsaHelper.EnsureKeyPair();

                // Resim seçilip seçilmediğini kontrol et
                if (string.IsNullOrEmpty(selectedImagePath))
                {
                    MessageBox.Show("Lütfen önce bir resim seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Resmin kopyasını oluştur
                Bitmap originalImage = new Bitmap(selectedImagePath);
                Bitmap imageCopy = new Bitmap(originalImage);

                // Kaydetme yeri seçimi
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    MessageBox.Show("Lütfen bir kaydetme yeri seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // AES anahtar oluşturma
                string password = txtPassword.Text;
                byte[] aesKey = Cls_AesHelper.GenerateAESKey(password);
                Console.WriteLine(string.Format(TypeLayer.Debug.DEBUG_AES_GENERATE_KEY_COMPLETED, BitConverter.ToString(aesKey)));

                // Metni şifreleme
                string inputText = txtInput.Text;
                string encryptedText = Cls_AesHelper.Encrypt(inputText, aesKey);
                Console.WriteLine(string.Format(TypeLayer.Debug.DEBUG_AES_ENCRYPT_PROCESSED, encryptedText));

                // AES anahtarını RSA ile şifreleme
                string rsaEncryptedAesKey = Cls_RsaHelper.Encrypt(Convert.ToBase64String(aesKey));
                Console.WriteLine(string.Format(TypeLayer.Debug.DEBUG_RSA_ENCRYPT_PROCESSED, rsaEncryptedAesKey));

                // Görsele veri gömme
                Cls_WaveletHelper.EmbedTextInImage(encryptedText, imageCopy);
                Cls_WaveletHelper.EmbedTextInImage(rsaEncryptedAesKey, imageCopy);
                Console.WriteLine(TypeLayer.Debug.DEBUG_WAVELET_EMBED_TEXT_PROCESSED);

                // Görsele hash ekleme
                Cls_WaveletHelper.AddHashToImage(imageCopy);
                Console.WriteLine(TypeLayer.Debug.DEBUG_HASH_ADD_COMPLETED);

                // Görsele hash ekleme
                Cls_WaveletHelper.EnsureTransparency(imageCopy);
                Console.WriteLine(TypeLayer.Debug.DEBUG_WAVELET_TRANSPARENCY_PROCESSED);

                // Kopyayı kaydet
                imageCopy.Save(saveFileDialog.FileName);
                MessageBox.Show(Success.IMAGE_ENCRYPTION_SUCCESS, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Şifre çözme butonu işlevi
        /// </summary>
        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            try
            {
                // Resim seçilip seçilmediğini kontrol et
                if (string.IsNullOrEmpty(selectedImagePath))
                {
                    MessageBox.Show("Lütfen önce bir resim seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Resmi yükle
                Bitmap image = new Bitmap(selectedImagePath);

                // HASH kontrolü
                if (!Cls_WaveletHelper.CheckHash(image))
                {
                    MessageBox.Show("Görselde geçerli bir HASH bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Console.WriteLine(TypeLayer.Debug.DEBUG_HASH_CHECK_PASSED);

                // Veriyi dışarı çıkar
                string extractedData = Cls_WaveletHelper.ExtractDataFromImage(image);
                Console.WriteLine(string.Format(TypeLayer.Debug.DEBUG_DATA_EXTRACTED, extractedData));

                // RSA ile şifre çözme
                string rsaEncryptedAesKey = Cls_WaveletHelper.ExtractRsaEncryptedKey(image);
                string aesKeyBase64 = Cls_RsaHelper.Decrypt(rsaEncryptedAesKey);
                byte[] aesKey = Convert.FromBase64String(aesKeyBase64);
                Console.WriteLine(string.Format(TypeLayer.Debug.DEBUG_RSA_DECRYPT_PROCESSED, aesKeyBase64));

                // Metin karşılaştırma
                string password = txtPassword.Text;
                if (password != Encoding.UTF8.GetString(aesKey))
                {
                    MessageBox.Show("Şifreler eşleşmiyor.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Console.WriteLine(TypeLayer.Debug.DEBUG_PASSWORD_MATCH);

                // AES ile şifre çözme
                string aesEncryptedText = Cls_WaveletHelper.ExtractAesEncryptedText(image);
                string decryptedText = Cls_AesHelper.Decrypt(aesEncryptedText, aesKey);
                Console.WriteLine(string.Format(TypeLayer.Debug.DEBUG_AES_DECRYPT_PROCESSED, decryptedText));

                // Sonucu txtOutput'a ilet
                txtOutput.Text = decryptedText;
                MessageBox.Show(Success.DECRYPT_SUCCESS_GENERAL, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


    }
}


