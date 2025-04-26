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
        private Cls_LsbEncrypt lsbEncrypt;
        private Cls_LsbDecrypt lsbDecrypt;
        
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
            
            // Keyler için ortak bir dizin belirle
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string keyDirectory = Path.Combine(appDataPath, "ImageBasedEncryptionSystem");
            
            // Dizin yoksa oluştur
            if (!Directory.Exists(keyDirectory))
            {
                Directory.CreateDirectory(keyDirectory);
            }
            
            // Business Layer sınıflarını başlat
            // Önce RSA sınıflarını başlat, diğerlerini bu sınıflara bağlı olarak oluştur
            InitializeEncryptionClasses();
           
        }

        /// <summary>
        /// Şifreleme için kullanılan tüm sınıfları başlatır
        /// </summary>
        private void InitializeEncryptionClasses()
        {
            try
            {
                // Anahtarları kullanarak RSA sınıflarını başlat
                rsaEncrypt = new Cls_RsaEncrypt();
                rsaDecrypt = new Cls_RsaDecrypt(rsaEncrypt);
                
                // AES sınıfları
                aesEncrypt = new Cls_AesEncrypt();
                aesDecrypt = new Cls_AesDecrypt();
                
                // LSB sınıfları
                lsbEncrypt = new Cls_LsbEncrypt();
                lsbDecrypt = new Cls_LsbDecrypt();
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
                // Resim seçme diyaloğunu oluştur
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    // Dosya filtresini ayarla
                    openFileDialog.Filter = "Görüntü Dosyaları|*.jpg;*.jpeg;*.png;*.bmp";
                    openFileDialog.Title = "Bir Resim Seçin";

                    // Dosya seçili değilse fonksiyondan çık
                    if (openFileDialog.ShowDialog() != DialogResult.OK)
                        return;

                    // Seçilen dosya yolunu kaydet
                    selectedImagePath = openFileDialog.FileName;

                    try
                    {
                        // Resimi yükle
                        using (Bitmap originalImage = new Bitmap(selectedImagePath))
                        {
                            // Resmin boyutunu kontrol et
                            if (originalImage.Width < 300 || originalImage.Height < 300)
                            {
                                MessageBox.Show(Errors.ERROR_IMAGE_TOO_SMALL,
                                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                selectedImagePath = string.Empty;
                                return;
                            }

                            // pboxImage kontrolünün boyutları
                            int maxWidth = pboxImage.Width;
                            int maxHeight = pboxImage.Height;

                            // Resmi PictureBox'a sığacak şekilde yeniden boyutlandır
                            Bitmap resizedImage = ResizeImage(originalImage, maxWidth, maxHeight);

                            // Eğer önceki resim varsa temizle
                            if (pboxImage.Image != null)
                            {
                                pboxImage.Image.Dispose();
                                pboxImage.Image = null;
                            }

                            // Yeni resmi PictureBox'a ata
                            pboxImage.Image = resizedImage;
                            pboxImage.SizeMode = PictureBoxSizeMode.Zoom;

                            // Başarı mesajı
                            lblImageInfo.Text = $"Seçilen Resim: {Path.GetFileName(selectedImagePath)} ({originalImage.Width}x{originalImage.Height})";
                            
                            // Şifreleme/çözme butonlarını aktif et
                            btnEncrypt.Enabled = true;
                            btnDecrypt.Enabled = true;
                            
                            // Kullanıcıya bilgi ver
                            txtOutput.Clear();
                            txtOutput.AppendText(Success.IMAGE_LOAD_SUCCESS);
                            txtOutput.AppendText(Environment.NewLine);
                            txtOutput.AppendText($"Boyut: {originalImage.Width}x{originalImage.Height} piksel");
                        }
                    }
                    catch (OutOfMemoryException)
                    {
                        MessageBox.Show(Errors.ERROR_IMAGE_DAMAGED,
                            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        selectedImagePath = string.Empty;
                    }
                    catch (FileNotFoundException)
                    {
                        MessageBox.Show(Errors.ERROR_FILE_NOT_FOUND,
                            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        selectedImagePath = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message),
                            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        selectedImagePath = string.Empty;
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
        /// Görüntüyü belirli boyuta sığdıracak şekilde yeniden boyutlandırır
        /// </summary>
        /// <param name="image">Orijinal görüntü</param>
        /// <param name="maxWidth">Maksimum genişlik</param>
        /// <param name="maxHeight">Maksimum yükseklik</param>
        /// <returns>Yeniden boyutlandırılmış görüntü</returns>
        private Bitmap ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            try
            {
                // Orijinal boyutları al
                int originalWidth = image.Width;
                int originalHeight = image.Height;

                // Yeni boyutları hesapla (en-boy oranını koru)
                double ratioX = (double)maxWidth / originalWidth;
                double ratioY = (double)maxHeight / originalHeight;
                double ratio = Math.Min(ratioX, ratioY);

                int newWidth = (int)(originalWidth * ratio);
                int newHeight = (int)(originalHeight * ratio);

                // Yeni boyutlarda bir bitmap oluştur
                Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb);

                // Yüksek kaliteli yeniden boyutlandırma için Graphics nesnesini yapılandır
                using (Graphics graphics = Graphics.FromImage(newImage))
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    // Görüntüyü çiz
                    graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                }

                return newImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message),
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // Hata durumunda orijinal görüntünün bir kopyasını döndür
                return new Bitmap(image);
            }
        }

        /// <summary>
        /// Şifreleme butonu işlevi
        /// </summary>
        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            try
            {
                // Girilen verileri kontrol et
                if (string.IsNullOrEmpty(txtInput.Text))
                {
                    MessageBox.Show(Errors.ERROR_TEXT_EMPTY, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrEmpty(txtPassword.Text))
                {
                    MessageBox.Show(Errors.ERROR_PASSWORD_EMPTY, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrEmpty(selectedImagePath) || pboxImage.Image == null)
                {
                    MessageBox.Show(Errors.ERROR_IMAGE_REQUIRED, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string inputText = txtInput.Text;
                string password = txtPassword.Text;

                // İşlem başladığını kullanıcıya bildir ve UI'yi güncelle
                txtOutput.Text = "Şifreleme işlemi başlatılıyor...";
                Application.DoEvents();

                // 1. AES ile metni şifrele
                byte[] encryptedText = null;
                try
                {
                    // AES ile metin şifrele
                    encryptedText = aesEncrypt.EncryptText(inputText, password);
                    if (encryptedText == null)
                    {
                        MessageBox.Show(aesEncrypt.LastErrorMessage, "AES Şifreleme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    txtOutput.AppendText(Environment.NewLine + aesEncrypt.LastSuccessMessage);
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(Errors.ERROR_AES_ENCRYPT, ex.Message), "AES Şifreleme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 2. RSA ile AES anahtarını şifrele
                byte[] encryptedAesKey = null;
                try
                {
                    // Parola ile AES anahtarını oluştur ve RSA ile şifrele
                    encryptedAesKey = rsaEncrypt.EncryptAesKeyWithPassword(password);
                    if (encryptedAesKey == null)
                    {
                        MessageBox.Show(Errors.ERROR_RSA_ENCRYPT, "RSA Şifreleme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    txtOutput.AppendText(Environment.NewLine + Success.ENCRYPT_SUCCESS_RSA);
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(Errors.ERROR_RSA_ENCRYPT, ex.Message), "RSA Şifreleme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 3. Veriyi resim içine gizle
                try
                {
                    // Görüntüyü yükle
                    using (Bitmap originalImage = new Bitmap(selectedImagePath))
                    {
                        // WaveletEncrypt sınıfını kullanarak metni ve anahtarı görüntüye gizle
                        Cls_WaveletEncrypt waveletEncrypt = new Cls_WaveletEncrypt();
                        
                        // Şifrelenmiş metin ve anahtarı birleştir (ilk bayt dizisi uzunluğunu kaydet)
                        byte[] dataToHide = CombineEncryptedData(encryptedText, encryptedAesKey);
                        
                        // Wavelet dönüşümü ile veriyi gizle
                        Bitmap encryptedImage = waveletEncrypt.EncryptTextToImage(originalImage, Convert.ToBase64String(dataToHide), password);
                        
                        if (encryptedImage == null)
                        {
                            MessageBox.Show(waveletEncrypt.LastErrorMessage, "Wavelet Şifreleme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Şifreli görüntüyü kaydet
                        SaveEncryptedImage(encryptedImage);
                        
                        // Kullanıcıya bilgi ver
                        txtOutput.AppendText(Environment.NewLine + waveletEncrypt.LastSuccessMessage);
                        txtOutput.AppendText(Environment.NewLine + "Şifreleme işlemi başarıyla tamamlandı!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(Errors.ERROR_WAVELET_ENCODE, ex.Message), "Wavelet Şifreleme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Şifrelenmiş metin ve anahtar verilerini birleştirir
        /// </summary>
        /// <param name="encryptedText">Şifrelenmiş metin</param>
        /// <param name="encryptedKey">Şifrelenmiş anahtar</param>
        /// <returns>Birleştirilmiş veri</returns>
        private byte[] CombineEncryptedData(byte[] encryptedText, byte[] encryptedKey)
        {
            try
            {
                // İlk veri boyutunu içerecek 4 baytlık ön ek
                byte[] textLengthBytes = BitConverter.GetBytes(encryptedText.Length);
                
                // Birleştirilmiş veri için yeni dizi oluştur
                byte[] combinedData = new byte[4 + encryptedText.Length + encryptedKey.Length];
                
                // İlk 4 bayt: şifrelenmiş metin uzunluğu
                Buffer.BlockCopy(textLengthBytes, 0, combinedData, 0, 4);
                
                // Sonraki baytlar: şifrelenmiş metin
                Buffer.BlockCopy(encryptedText, 0, combinedData, 4, encryptedText.Length);
                
                // Son baytlar: şifrelenmiş anahtar
                Buffer.BlockCopy(encryptedKey, 0, combinedData, 4 + encryptedText.Length, encryptedKey.Length);
                
                return combinedData;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), "Veri Birleştirme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Şifrelenmiş görüntüyü kaydeder
        /// </summary>
        /// <param name="encryptedImage">Şifrelenmiş görüntü</param>
        private void SaveEncryptedImage(Bitmap encryptedImage)
        {
            try
            {
                // Kayıt diyaloğunu oluştur
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "PNG Görüntü|*.png";
                    saveFileDialog.Title = "Şifreli Görüntüyü Kaydet";
                    saveFileDialog.FileName = "encryptedImage.png";

                    // Dosya kaydedilmediyse fonksiyondan çık
                    if (saveFileDialog.ShowDialog() != DialogResult.OK)
                        return;

                    // Görüntüyü belirtilen yola kaydet
                    encryptedImage.Save(saveFileDialog.FileName, ImageFormat.Png);
                    
                    // Kullanıcıya bilgi ver
                    txtOutput.AppendText(Environment.NewLine + Success.IMAGE_SAVE_SUCCESS);
                    txtOutput.AppendText(Environment.NewLine + $"Dosya yolu: {saveFileDialog.FileName}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_FILE_SAVE, ex.Message), "Dosya Kaydetme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Şifre çözme butonu işlevi
        /// </summary>
        private void btnDecrypt_Click(object sender, EventArgs e)
        {
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


