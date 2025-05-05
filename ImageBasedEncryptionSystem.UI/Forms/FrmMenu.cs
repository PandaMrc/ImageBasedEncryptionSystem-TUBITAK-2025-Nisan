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
            
            Console.WriteLine(TypeLayer.Debug.DEBUG_FORM_INITIALIZE_STARTED);
            // Geliştirici modu nesnesini başlat
            devMode = new Cls_DeveloperMode();
            Console.WriteLine(TypeLayer.Debug.DEBUG_DEV_MODE_OBJECT_INITIALIZED);
            
            // Metin girişi için maksimum karakter sınırı ayarla
            txtInput.MaxLength = 100000000; // Maksimum 100.000.000 karakter
            Console.WriteLine(TypeLayer.Debug.DEBUG_MAX_LENGTH_SET);
        }


        // Form yükleme işlemi
        private void FrmMenu_Load(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine(TypeLayer.Debug.DEBUG_SYSTEM_IDENTITY_CHECK);
                
                // Form ayarları
                txtOutput.ReadOnly = true;
                Console.WriteLine(TypeLayer.Debug.DEBUG_FORM_SETTINGS_APPLIED);

                // Arka plan oluştur - Business Layer'daki Cls_Background sınıfını kullan
                BusinessLayer.UI.Cls_Background.Instance.CreateModernBackground(this);
                Console.WriteLine(TypeLayer.Debug.DEBUG_BACKGROUND_CREATED);

                // Olayları bağla
                btnDevMode.Click += btnDevMode_Click;
                btnAdmin.Click += btnAdmin_Click;
                Console.WriteLine(TypeLayer.Debug.DEBUG_EVENTS_BOUND);

                // Geliştirici modu durumunu kontrol et ve UI'ı güncelle
                CheckDeveloperModeStatus();
                Console.WriteLine(TypeLayer.Debug.DEBUG_DEV_MODE_STATUS_CHECKED);

                // RSA anahtar çiftini oluştur
                Cls_RsaHelper.EnsureKeyPair();
                Console.WriteLine(TypeLayer.Debug.DEBUG_RSA_ENSURE_KEY_PAIR_STARTED);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(TypeLayer.Debug.DEBUG_SYSTEM_IDENTITY_RECEIVED, ex.Message));
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Şifreleme ve Şifre Çözme İşlemleri
        /// <summary>
        /// Resim seçme butonu işlevi
        /// </summary>
        private void btnImage_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine(TypeLayer.Debug.DEBUG_IMAGE_SELECTION_STARTED);
                // Resim seçme işlemi
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image Files|*.jpeg;*jpg;*.png;*.bmp";
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    Console.WriteLine(TypeLayer.Debug.DEBUG_IMAGE_LOAD_STARTED);
                    MessageBox.Show(Errors.ERROR_IMAGE_CANCELED, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Bitmap selectedImage = new Bitmap(openFileDialog.FileName);
                Console.WriteLine(string.Format(TypeLayer.Debug.DEBUG_IMAGE_LOAD_STARTED, openFileDialog.FileName));

                // Resim boyut kontrolü
                if (selectedImage.Width < 256 || selectedImage.Height < 256)
                {
                    Console.WriteLine(TypeLayer.Debug.DEBUG_IMAGE_LOAD_SUCCESS);
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
                Console.WriteLine(string.Format(TypeLayer.Debug.DEBUG_SYSTEM_IDENTITY_RECEIVED, ex.Message));
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
                Console.WriteLine(TypeLayer.Debug.DEBUG_ENCRYPTION_STARTED);
                #region Uyarılar
                // Resim seçilmemişse uyarı ver
                if (string.IsNullOrEmpty(selectedImagePath))
                {
                    Console.WriteLine(TypeLayer.Debug.DEBUG_IMAGE_LOAD_STARTED);
                    MessageBox.Show(Errors.ERROR_IMAGE_NOT_SELECTED, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 'txtPassword' boş olamaz ve boşluk karakteri içeremez
                if (string.IsNullOrWhiteSpace(txtPassword.Text) || txtPassword.Text.Contains(" "))
                {
                    Console.WriteLine(TypeLayer.Debug.DEBUG_PASSWORD_MATCH);
                    MessageBox.Show(Errors.ERROR_PASSWORD_INVALID, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 'txtInput' boş olamaz
                if (string.IsNullOrWhiteSpace(txtInput.Text))
                {
                    Console.WriteLine(TypeLayer.Debug.DEBUG_DATA_EXTRACTED);
                    MessageBox.Show(Errors.ERROR_TEXT_EMPTY, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                #endregion

                // AES anahtarını oluştur
                string password = txtPassword.Text;
                byte[] aesKey = Cls_AesHelper.GenerateAESKey(password);
                Console.WriteLine(TypeLayer.Debug.DEBUG_AES_GENERATE_KEY_STARTED);
                Console.WriteLine(string.Format(TypeLayer.Debug.DEBUG_AES_KEY_GENERATION_COMPLETED, Convert.ToBase64String(aesKey)));

                // Metni şifrele
                string inputText = txtInput.Text;
                string aesEncryptedText = Cls_AesHelper.Encrypt(inputText, aesKey);
                Console.WriteLine(TypeLayer.Debug.DEBUG_AES_ENCRYPT_STARTED);
                Console.WriteLine(string.Format(TypeLayer.Debug.DEBUG_AES_ENCRYPTION_COMPLETED, aesEncryptedText));

                // AES anahtarını RSA ile şifrele
                string rsaEncryptedAesKey = Cls_RsaHelper.Encrypt(Convert.ToBase64String(aesKey));
                Console.WriteLine(TypeLayer.Debug.DEBUG_RSA_ENCRYPT_STARTED);

                // Resmin kopyasını oluştur
                Bitmap imageCopy = new Bitmap(selectedImagePath);
                
                // Kullanıcıdan kaydedilecek yeri seçmesini iste
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "PNG Image|*.png";
                    saveFileDialog.Title = "Şifrelenen resmi kaydet";
                    saveFileDialog.FileName = "encrypted_image.png";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        using (imageCopy)
                        {
                            // İmza ekleme işlemi
                            Console.WriteLine(TypeLayer.Debug.DEBUG_SIGNATURE_ADD_STARTED);
                            Bitmap signedImage = Cls_LsbHelper.EmbedSignature(imageCopy);
                            Console.WriteLine(TypeLayer.Debug.DEBUG_HASH_ADD_COMPLETED);

                            // Şifrelenmiş veriyi resme gömme işlemi
                            using (Bitmap resultImage = Cls_LsbHelper.EmbedData(signedImage, Encoding.UTF8.GetBytes(aesEncryptedText + ";" + rsaEncryptedAesKey)))
                            {
                                // Sonuç resmini kaydet
                                resultImage.Save(saveFileDialog.FileName, ImageFormat.Png);
                            }
                        }

                        MessageBox.Show(Success.ENCRYPTION_SUCCESS, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(TypeLayer.Debug.DEBUG_SYSTEM_IDENTITY_RECEIVED, ex.Message));
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
                Console.WriteLine(TypeLayer.Debug.DEBUG_DECRYPTION_STARTED);
                // Resim seçilmemişse uyarı ver
                if (string.IsNullOrEmpty(selectedImagePath))
                {
                    MessageBox.Show(Errors.ERROR_IMAGE_NOT_SELECTED, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 'txtPassword' boş olamaz ve boşluk karakteri içeremez
                if (string.IsNullOrWhiteSpace(txtPassword.Text) || txtPassword.Text.Contains(" "))
                {
                    MessageBox.Show(Errors.ERROR_PASSWORD_INVALID, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Resmi yükle
                Bitmap image = new Bitmap(selectedImagePath);

                // Resimden veri çıkarma
                byte[] extractedDataBytes = Cls_LsbHelper.ExtractData(image);
                if (extractedDataBytes.Length == 0)
                {
                    MessageBox.Show(Errors.ERROR_NO_HIDDEN_DATA, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                 // İmza kontrolü
                Console.WriteLine(TypeLayer.Debug.DEBUG_SIGNATURE_CHECK_STARTED);
                if (!Cls_LsbHelper.CheckSignature(image))
                {
                    MessageBox.Show(Errors.ERROR_NOT_ENCRYPTED_WITH_APP, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Console.WriteLine(TypeLayer.Debug.DEBUG_SIGNATURE_CHECK_COMPLETED);

                // 'txtPassword' verisiyle AES anahtarı oluştur
                string password = txtPassword.Text;
                byte[] newAesKey = Cls_AesHelper.GenerateAESKey(password);

                // Veriyi resimden çıkar
                string extractedData = Encoding.UTF8.GetString(extractedDataBytes);
                string[] parts = extractedData.Split(';');
                if (parts.Length != 2)
                {
                    MessageBox.Show(Errors.ERROR_INVALID_DATA_FORMAT, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string aesEncryptedText = parts[0];
                string rsaEncryptedAesKey = parts[1];

                // RSA ile şifrelenmiş AES anahtarını çöz
                byte[] decryptedAesKey = Convert.FromBase64String(Cls_RsaHelper.Decrypt(rsaEncryptedAesKey));

                // Çözülen AES anahtarıyla yeni oluşturulan AES anahtarını karşılaştır
                if (!decryptedAesKey.SequenceEqual(newAesKey))
                {
                    MessageBox.Show(Errors.ERROR_KEYS_MISMATCH, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Yeni oluşturulan AES anahtarıyla resimden çıkartılan AES ile şifrelenmiş veriyi çöz
                string decryptedText = Cls_AesHelper.Decrypt(aesEncryptedText, newAesKey);
                Console.WriteLine(string.Format(TypeLayer.Debug.DEBUG_AES_DECRYPTION_COMPLETED, decryptedText));

                // Sonucu 'txtOutput'a ilet
                txtOutput.Text = decryptedText;
                MessageBox.Show(Success.DECRYPTION_SUCCESS, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        /// <summary>
        /// Analiz butonuna tıklandığında FrmAnalysis formunu açar veya geliştirici modu kontrolü yapar
        /// </summary>
        private void btnAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine(TypeLayer.Debug.DEBUG_ANALYSIS_STARTED);
                // Geliştirici girişi var ama mod aktif değilse
                if (devMode != null)
                {
                    var (loginMessage, isLoggedIn) = devMode.CheckLoginStatus();
                    var (devModeMessage, isDevModeActive) = devMode.CheckDevModeStatus();
                    
                    if (isLoggedIn && !isDevModeActive)
                    {
                        // Geliştirici girişi yapılmış ama mod aktif değil
                        MessageBox.Show(Errors.ERROR_DEV_MODE_REQUIRED, 
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

        /// <summary>
        /// Geliştirici modu etiketi tıklandığında modu aktifleştirir veya devre dışı bırakır
        /// </summary>
        private void btnDevMode_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine(TypeLayer.Debug.DEBUG_DEV_MODE_TOGGLE_STARTED);
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
                Console.WriteLine(TypeLayer.Debug.DEBUG_LOGIN_ICON_CLICKED);
                // Geliştirici girişi var mı kontrol et
                if (devMode != null)
                {
                    var (loginMessage, isLoggedIn) = devMode.CheckLoginStatus();
                    
                    if (isLoggedIn)
                    {
                        // Çıkış onay mesajı göster
                        DialogResult result = MessageBox.Show(
                            Errors.ERROR_LOGOUT_CONFIRMATION, 
                            "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            
                        if (result == DialogResult.Yes)
                        {
                            // Geliştirici modundan çıkış yap
                            string logoutResult = devMode.Logout();
                            MessageBox.Show(Success.DEVELOPER_LOGOUT_SUCCESS, 
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
            Console.WriteLine(TypeLayer.Debug.DEBUG_LOGIN_ICON_MOUSE_ENTER);
            // Fare üzerine geldiğinde efekt (büyütme veya parlaklık değişimi)
            pbLogin.Size = new Size(45, 45);
            pbLogin.Location = new Point(8, -4);
        }
        
        // Login ikonundan ayrılındığında efekt
        private void pbLogin_MouseLeave(object sender, EventArgs e)
        {
            Console.WriteLine(TypeLayer.Debug.DEBUG_LOGIN_ICON_MOUSE_LEAVE);
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
                Console.WriteLine(TypeLayer.Debug.DEBUG_DEV_MODE_ACTIVATION_STARTED);
                if (developerMode == null)
                {
                    MessageBox.Show(Errors.ERROR_DEV_MODE_OBJECT_NULL, "Hata", 
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
                Console.WriteLine(TypeLayer.Debug.DEBUG_ADMIN_BUTTON_CLICKED);
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
                Console.WriteLine(TypeLayer.Debug.DEBUG_ADMIN_FORM_OPENING);
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
                Console.WriteLine(TypeLayer.Debug.DEBUG_DEV_MODE_STATUS_CHECK_STARTED);
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


