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
            txtInput.MaxLength = 10000; // Maksimum 10.000 karakter
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
                rtbInput.Visible = false; // Başlangıçta RichTextBox gizli
                btnDev.Text = "+"; // Zengin metin düzenlemeye geçmek için "+" gösterilir
                btnDev.Enabled = false; // Başlangıçta devre dışı
                btnDev.FillColor = Color.DarkGray; // Gri renk
                loginToolTip.SetToolTip(btnDev, "Zengin metin moduna geç");
                
                // İlerleme çubuğu ve durum etiketi ayarları
                progressOperation.Value = 0;
                lblOperation.Text = "İşlem Durumu: Hazır";
                
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

                // Giriş metni kontrolü - normal veya zengin metin alanlarından birisi aktif
                string inputText = "";
                if (txtInput.Visible)
                {
                    // Normal metin alanı kullanılıyor
                    if (string.IsNullOrWhiteSpace(txtInput.Text))
                    {
                        Console.WriteLine(TypeLayer.Debug.DEBUG_DATA_EXTRACTED);
                        MessageBox.Show(Errors.ERROR_TEXT_EMPTY, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    inputText = txtInput.Text;
                }
                else
                {
                    // Zengin metin alanı kullanılıyor
                    if (string.IsNullOrWhiteSpace(rtbInput.Text))
                    {
                        Console.WriteLine(TypeLayer.Debug.DEBUG_DATA_EXTRACTED);
                        MessageBox.Show(Errors.ERROR_TEXT_EMPTY, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    inputText = rtbInput.Text;
                }
                #endregion

                // İşlem durumunu göster
                lblOperation.Text = "İşlem Durumu: Başlatılıyor...";
                progressOperation.Value = 0;
                btnEncrypt.Enabled = false;
                btnDecrypt.Enabled = false;
                Application.DoEvents();

                // AES anahtarını oluştur
                string password = txtPassword.Text;
                byte[] aesKey = Cls_AesHelper.GenerateAESKey(password);
                Console.WriteLine(TypeLayer.Debug.DEBUG_AES_GENERATE_KEY_STARTED);
                Console.WriteLine(string.Format(TypeLayer.Debug.DEBUG_AES_KEY_GENERATION_COMPLETED, Convert.ToBase64String(aesKey)));
                
                // İlerleme durumunu güncelle
                lblOperation.Text = "İşlem Durumu: AES Anahtarı Oluşturuldu";
                progressOperation.Value = 20;
                Application.DoEvents();

                // Metni şifrele
                string aesEncryptedText = Cls_AesHelper.Encrypt(inputText, aesKey);
                Console.WriteLine(TypeLayer.Debug.DEBUG_AES_ENCRYPT_STARTED);
                Console.WriteLine(string.Format(TypeLayer.Debug.DEBUG_AES_ENCRYPTION_COMPLETED, aesEncryptedText));
                
                // İlerleme durumunu güncelle
                lblOperation.Text = "İşlem Durumu: Metin Şifrelendi";
                progressOperation.Value = 40;
                Application.DoEvents();

                // AES anahtarını RSA ile şifrele
                string rsaEncryptedAesKey = Cls_RsaHelper.Encrypt(Convert.ToBase64String(aesKey));
                Console.WriteLine(TypeLayer.Debug.DEBUG_RSA_ENCRYPT_STARTED);
                
                // İlerleme durumunu güncelle
                lblOperation.Text = "İşlem Durumu: RSA Şifreleme Tamamlandı";
                progressOperation.Value = 60;
                Application.DoEvents();

                // Resmin kopyasını oluştur
                Bitmap imageCopy = new Bitmap(selectedImagePath);
                
                // İlerleme durumunu güncelle
                lblOperation.Text = "İşlem Durumu: Resim Hazırlanıyor";
                progressOperation.Value = 70;
                Application.DoEvents();
                
                // Kullanıcıdan kaydedilecek yeri seçmesini iste
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "PNG Image|*.png";
                    saveFileDialog.Title = "Şifrelenen resmi kaydet";
                    saveFileDialog.FileName = "encrypted_image.png";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // İlerleme durumunu güncelle
                        lblOperation.Text = "İşlem Durumu: İmza Ekleniyor";
                        progressOperation.Value = 80;
                        Application.DoEvents();
                        
                        using (imageCopy)
                        {
                            // İmza ekleme işlemi
                            Console.WriteLine(TypeLayer.Debug.DEBUG_SIGNATURE_ADD_STARTED);
                            Bitmap signedImage = Cls_LsbHelper.EmbedSignature(imageCopy);
                            Console.WriteLine(TypeLayer.Debug.DEBUG_HASH_ADD_COMPLETED);

                            // İlerleme durumunu güncelle
                            lblOperation.Text = "İşlem Durumu: Veri Gömülüyor";
                            progressOperation.Value = 90;
                            Application.DoEvents();

                            // Şifrelenmiş veriyi resme gömme işlemi
                            using (Bitmap resultImage = Cls_LsbHelper.EmbedData(signedImage, Encoding.UTF8.GetBytes(aesEncryptedText + ";" + rsaEncryptedAesKey)))
                            {
                                // İlerleme durumunu güncelle
                                lblOperation.Text = "İşlem Durumu: Resim Kaydediliyor";
                                progressOperation.Value = 95;
                                Application.DoEvents();
                                
                                // Sonuç resmini kaydet
                                resultImage.Save(saveFileDialog.FileName, ImageFormat.Png);
                            }
                        }

                        // İlerleme durumunu güncelle
                        lblOperation.Text = "İşlem Durumu: Tamamlandı";
                        progressOperation.Value = 100;
                        Application.DoEvents();
                        
                        MessageBox.Show(Success.ENCRYPTION_SUCCESS, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // İptal edildi
                        lblOperation.Text = "İşlem Durumu: İptal Edildi";
                        progressOperation.Value = 0;
                    }
                }

                // Butonları etkinleştir
                btnEncrypt.Enabled = true;
                btnDecrypt.Enabled = true;
            }
            catch (Exception ex)
            {
                // Hata durumunda ilerleme çubuğunu güncelle
                lblOperation.Text = "İşlem Durumu: Hata!";
                progressOperation.Value = 0;
                
                // Butonları etkinleştir
                btnEncrypt.Enabled = true;
                btnDecrypt.Enabled = true;
                
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

                // İşlem durumunu göster
                lblOperation.Text = "İşlem Durumu: Başlatılıyor...";
                progressOperation.Value = 0;
                btnEncrypt.Enabled = false;
                btnDecrypt.Enabled = false;
                Application.DoEvents();

                // Resmi yükle
                Bitmap image = new Bitmap(selectedImagePath);
                
                // İlerleme durumunu güncelle
                lblOperation.Text = "İşlem Durumu: Resim Yüklendi";
                progressOperation.Value = 20;
                Application.DoEvents();

                // Resimden veri çıkarma
                byte[] extractedDataBytes = Cls_LsbHelper.ExtractData(image);
                if (extractedDataBytes.Length == 0)
                {
                    // İşlemi sonlandır
                    lblOperation.Text = "İşlem Durumu: Hata - Gizli Veri Bulunamadı";
                    progressOperation.Value = 0;
                    btnEncrypt.Enabled = true;
                    btnDecrypt.Enabled = true;
                    
                    MessageBox.Show(Errors.ERROR_NO_HIDDEN_DATA, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // İlerleme durumunu güncelle
                lblOperation.Text = "İşlem Durumu: Gizli Veri Çıkarıldı";
                progressOperation.Value = 40;
                Application.DoEvents();

                // İmza kontrolü
                Console.WriteLine(TypeLayer.Debug.DEBUG_SIGNATURE_CHECK_STARTED);
                if (!Cls_LsbHelper.CheckSignature(image))
                {
                    // İşlemi sonlandır
                    lblOperation.Text = "İşlem Durumu: Hata - Geçersiz İmza";
                    progressOperation.Value = 0;
                    btnEncrypt.Enabled = true;
                    btnDecrypt.Enabled = true;
                    
                    MessageBox.Show(Errors.ERROR_NOT_ENCRYPTED_WITH_APP, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Console.WriteLine(TypeLayer.Debug.DEBUG_SIGNATURE_CHECK_COMPLETED);
                
                // İlerleme durumunu güncelle
                lblOperation.Text = "İşlem Durumu: İmza Doğrulandı";
                progressOperation.Value = 50;
                Application.DoEvents();

                // 'txtPassword' verisiyle AES anahtarı oluştur
                string password = txtPassword.Text;
                byte[] newAesKey = Cls_AesHelper.GenerateAESKey(password);
                
                // İlerleme durumunu güncelle
                lblOperation.Text = "İşlem Durumu: AES Anahtarı Oluşturuldu";
                progressOperation.Value = 60;
                Application.DoEvents();

                // Veriyi resimden çıkar
                string extractedData = Encoding.UTF8.GetString(extractedDataBytes);
                string[] parts = extractedData.Split(';');
                if (parts.Length != 2)
                {
                    // İşlemi sonlandır
                    lblOperation.Text = "İşlem Durumu: Hata - Geçersiz Veri Formatı";
                    progressOperation.Value = 0;
                    btnEncrypt.Enabled = true;
                    btnDecrypt.Enabled = true;
                    
                    MessageBox.Show(Errors.ERROR_INVALID_DATA_FORMAT, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // İlerleme durumunu güncelle
                lblOperation.Text = "İşlem Durumu: Şifreli Veri Ayrıştırıldı";
                progressOperation.Value = 70;
                Application.DoEvents();

                string aesEncryptedText = parts[0];
                string rsaEncryptedAesKey = parts[1];

                // RSA ile şifrelenmiş AES anahtarını çöz
                byte[] decryptedAesKey = Convert.FromBase64String(Cls_RsaHelper.Decrypt(rsaEncryptedAesKey));
                
                // İlerleme durumunu güncelle
                lblOperation.Text = "İşlem Durumu: RSA Şifresi Çözüldü";
                progressOperation.Value = 80;
                Application.DoEvents();

                // Çözülen AES anahtarıyla yeni oluşturulan AES anahtarını karşılaştır
                if (!decryptedAesKey.SequenceEqual(newAesKey))
                {
                    // İşlemi sonlandır
                    lblOperation.Text = "İşlem Durumu: Hata - Parola Yanlış";
                    progressOperation.Value = 0;
                    btnEncrypt.Enabled = true;
                    btnDecrypt.Enabled = true;
                    
                    MessageBox.Show(Errors.ERROR_KEYS_MISMATCH, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // İlerleme durumunu güncelle
                lblOperation.Text = "İşlem Durumu: Parola Doğrulandı";
                progressOperation.Value = 90;
                Application.DoEvents();

                // Yeni oluşturulan AES anahtarıyla resimden çıkartılan AES ile şifrelenmiş veriyi çöz
                string decryptedText = Cls_AesHelper.Decrypt(aesEncryptedText, newAesKey);
                Console.WriteLine(string.Format(TypeLayer.Debug.DEBUG_AES_DECRYPTION_COMPLETED, decryptedText));
                
                // İlerleme durumunu güncelle
                lblOperation.Text = "İşlem Durumu: Metin Çözüldü";
                progressOperation.Value = 95;
                Application.DoEvents();

                // Sonucu 'txtOutput'a ilet
                if (extractedData.Length > 50000)
                {
                    txtOutput.Text = "Veri 50.000 karakterden uzun olduğu için metin belgesi oluşturuldu";

                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "Text Files|*.txt";
                        saveFileDialog.Title = "Çıkarılan Veriyi Kaydet";
                        saveFileDialog.FileName = "Decrypted_Data.txt";

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("Şifrelenmiş Metin:");
                            sb.AppendLine();
                            sb.AppendLine(decryptedText);

                            File.WriteAllText(saveFileDialog.FileName, sb.ToString());
                            MessageBox.Show("Veri başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    txtOutput.Text = decryptedText;
                }
                
                // İlerleme durumunu güncelle
                lblOperation.Text = "İşlem Durumu: Tamamlandı";
                progressOperation.Value = 100;
                Application.DoEvents();
                
                MessageBox.Show(Success.DECRYPTION_SUCCESS, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Butonları etkinleştir
                btnEncrypt.Enabled = true;
                btnDecrypt.Enabled = true;
            }
            catch (Exception ex)
            {
                // Hata durumunda ilerleme çubuğunu güncelle
                lblOperation.Text = "İşlem Durumu: Hata!";
                progressOperation.Value = 0;
                
                // Butonları etkinleştir
                btnEncrypt.Enabled = true;
                btnDecrypt.Enabled = true;
                
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
                    // Geliştirici modu kapatılmadan önce rtbInput aktifse kontrol et
                    if (!txtInput.Visible)
                    {
                        DialogResult switchResult = MessageBox.Show(
                            "Geliştirici modu devre dışı bırakmadan önce zengin metin modundan çıkmanız gerekmektedir. Şimdi normal metin moduna geçiş yapmak ister misiniz?",
                            "Zengin Metin Uyarısı",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );
                        
                        if (switchResult == DialogResult.Yes)
                        {
                            // btnDev_Click ile manuel geçiş yap
                            btnDev_Click(btnDev, EventArgs.Empty);
                            
                            // Geçiş iptal edildiyse (örneğin karakter sınırı nedeniyle) geliştirici modunu devre dışı bırakma işlemini de iptal et
                            if (!txtInput.Visible)
                            {
                                return;
                            }
                        }
                        else
                        {
                            // Kullanıcı normal metin moduna geçmek istemiyor, geliştirici modunu kapatma işlemini iptal et
                            MessageBox.Show("Geliştirici modu kapatmak için önce normal metin moduna geçmelisiniz.", 
                                "İşlem İptal Edildi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                    
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
                        // Çıkış yapmadan önce, eğer rtbInput aktifse metin kaybını önlemek için uyarı göster
                        if (!txtInput.Visible)
                        {
                            DialogResult switchResult = MessageBox.Show(
                                "Zengin metin modundan çıkılacak. Devam etmeden önce normal metin moduna geçmek ister misiniz?",
                                "Zengin Metin Uyarısı",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question
                            );
                            
                            if (switchResult == DialogResult.Yes)
                            {
                                // btnDev_Click ile manuel geçiş yap
                                btnDev_Click(btnDev, EventArgs.Empty);
                                
                                // Geçiş iptal edildiyse (örneğin karakter sınırı nedeniyle) çıkış işlemini de iptal et
                                if (!txtInput.Visible)
                                {
                                    return;
                                }
                            }
                        }
                        
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
                                
                            // Çıkış yapmadan önce, rtbInput gösteriliyorsa gizle, txtInput'u göster
                            if (!txtInput.Visible)
                            {
                                // Karakter sınırı kontrolünü burada yapmıyoruz, çünkü zaten çıkış yapıyoruz
                                // ve herhangi bir veriyi koruma amacımız kalmadı
                                txtInput.Text = rtbInput.Text.Length > txtInput.MaxLength 
                                    ? rtbInput.Text.Substring(0, txtInput.MaxLength) 
                                    : rtbInput.Text;
                                rtbInput.Visible = false;
                                txtInput.Visible = true;
                            }
                            
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
                    btnDev.Visible = false;
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
                    btnDev.Visible = false;
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
                    btnAnalysis.ForeColor = Color.White;
                    btnAnalysis.FillColor = Color.FromArgb(0, 122, 204); // Mavi renk

                    // Geliştirici metin modu butonu ayarları - Aktif
                    btnDev.Visible = true;
                    btnDev.Enabled = true; // Tıklanabilir
                    btnDev.FillColor = Color.FromArgb(0, 122, 204); // Mavi renk
                    
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

                    // Geliştirici metin modu butonu ayarları - Pasif
                    btnDev.Visible = true;
                    btnDev.Enabled = true; // Tıklanabilir
                    btnDev.FillColor = Color.DarkGray; // Gri renk
                    
                    // Geliştirici modu pasifken, normal metin girişine geçildiğinden emin ol
                    if (!txtInput.Visible)
                    {
                        txtInput.Text = rtbInput.Text.Length > txtInput.MaxLength 
                            ? rtbInput.Text.Substring(0, txtInput.MaxLength) 
                            : rtbInput.Text;
                        rtbInput.Visible = false;
                        txtInput.Visible = true;
                        btnDev.Text = "+";
                    }
                    
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
                    
                // Geliştirici metin modu butonu için tooltip
                loginToolTip.SetToolTip(btnDev, isDevModeActive ? 
                    (txtInput.Visible ? "Zengin metin moduna geç" : "Normal metin moduna geç") : 
                    "Bu butonu kullanabilmek için önce geliştirici modunu etkinleştirin");

            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Geliştirici butonu işlevi - gelişmiş metin girişi için rtbInput ile txtInput arasında geçiş yapar
        /// </summary>
        private void btnDev_Click(object sender, EventArgs e)
        {
            try
            {
                // Geliştirici girişi var mı kontrol et
                if (devMode == null)
                {
                    MessageBox.Show(Errors.ERROR_DEV_MODE_ACCESS_DENIED, 
                        "Erişim Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Giriş yapılıp yapılmadığını kontrol et
                var (loginMessage, isLoggedIn) = devMode.CheckLoginStatus();
                
                if (!isLoggedIn)
                {
                    MessageBox.Show(Errors.ERROR_DEV_MODE_ACCESS_DENIED, 
                        "Erişim Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Geliştirici modu aktif mi kontrol et
                var (devModeMessage, isDevModeActive) = devMode.CheckDevModeStatus();
                
                if (!isDevModeActive)
                {
                    MessageBox.Show(Errors.ERROR_DEV_MODE_REQUIRED, 
                        "Geliştirici Modu Gerekli", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // RichTextBox ile TextBox arasında geçiş yap
                if (txtInput.Visible)
                {
                    // Normal metin kutusundan zengin metin kutusuna geç
                    rtbInput.Text = txtInput.Text;
                    txtInput.Visible = false;
                    rtbInput.Visible = true;
                    btnDev.Text = "-";
                    loginToolTip.SetToolTip(btnDev, "Normal metin moduna geç");
                }
                else
                {
                    // Zengin metin kutusundan normal metin kutusuna geç
                    
                    // Karakter sayısı kontrolü
                    if (rtbInput.Text.Length > txtInput.MaxLength)
                    {
                        DialogResult result = MessageBox.Show(
                            $"Zengin metin içeriği {rtbInput.Text.Length} karakter içeriyor ve normal metin kutusunun maksimum karakter sınırı {txtInput.MaxLength}'dir.\n\n" +
                            "Devam ederseniz, metin ilk 10.000 karaktere kısaltılacaktır.\n\n" +
                            "Devam etmek istiyor musunuz?",
                            "Karakter Sınırı Aşıldı",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning
                        );
                        
                        if (result == DialogResult.No)
                        {
                            return; // İşlemi iptal et
                        }
                        
                        // Devam etmek istiyor, metni kısalt
                        txtInput.Text = rtbInput.Text.Substring(0, txtInput.MaxLength);
                    }
                    else
                    {
                        // Normal karakter sınırları içinde, doğrudan aktar
                        txtInput.Text = rtbInput.Text;
                    }
                    
                    rtbInput.Visible = false;
                    txtInput.Visible = true;
                    btnDev.Text = "+";
                    loginToolTip.SetToolTip(btnDev, "Zengin metin moduna geç");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtOutput_TextChanged(object sender, EventArgs e)
        {

        }
    }
}


