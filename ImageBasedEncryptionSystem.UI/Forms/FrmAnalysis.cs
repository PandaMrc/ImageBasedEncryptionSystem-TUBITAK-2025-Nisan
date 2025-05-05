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
using ImageBasedEncryptionSystem.BusinessLayer.Helpers;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Linq.Expressions;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing.Imaging;


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

        // devMode değişkeni tanımlandı
        private Cls_DeveloperMode devMode;

        private object _image;
        private string _username;

        public FrmAnalysis(Form owner = null)
        {
            InitializeComponent();
            _owner = owner;
            
            // devMode örneği oluşturuldu
            devMode = new Cls_DeveloperMode();
            
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

            SetPictureBoxSizeMode();
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


        #region TAB - Resim Yükleme
        private void btnImageOrginal_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.png;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _originalImage = new Bitmap(openFileDialog.FileName);
                    pbOrginal.Image = _originalImage;
                }
            }
        }






        #endregion TAB - Resim Yükleme

        private void btnImageEncrypted_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.png;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _encryptedImage = new Bitmap(openFileDialog.FileName);
                    pbEncrypted.Image = _encryptedImage;
                }
            }
        }
        #region TAB - Veri Detayları
        private void btnDetails_Click(object sender, EventArgs e)
        {
            if (_encryptedImage == null)
            {
                MessageBox.Show("Lütfen şifrelenmiş resmi yükleyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            byte[] extractedData = Cls_LsbHelper.ExtractData(_encryptedImage);
            string extractedString = Encoding.UTF8.GetString(extractedData);
            string[] parts = extractedString.Split(';');

            if (parts.Length == 2)
            {
                txtEncryptedAesKey.Text = parts[1];
                txtEncryptedText.Text = parts[0];

                byte[] decryptedAesKey = Convert.FromBase64String(Cls_RsaHelper.Decrypt(parts[1]));
                txtAesKey.Text = Encoding.UTF8.GetString(decryptedAesKey);

                string decryptedText = Cls_AesHelper.Decrypt(parts[0], decryptedAesKey);
                txtOrginalText.Text = decryptedText;
            }
            else
            {
                MessageBox.Show("Veri formatı geçersiz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion TAB - Veri Detayları

        #region TAB - Bit Plane Analizi
        /// <summary>
        /// Bit Plane Analizi İşlemleri


        private void btnBitPlane_Click(object sender, EventArgs e)
        {
            if (_encryptedImage != null)
            {
                try
                {
                    // Örnek olarak kırmızı kanalın 0. bit düzlemi analiz ediliyor
                    Bitmap result = Cls_BitPlaneHelper.ExtractBitPlane(_encryptedImage, 0, ColorChannel.Red);
                    // Analiz sonucunu pbEncrypted üzerinde göster
                    pbBitPlane.Image = result;
                    // Orijinal resmi pbChiSquareOrginal üzerinde göster
                    pbChiSquareOrginal.Image = pbOrginal.Image;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Lütfen önce bir resim seçin.");
            }
        }

        private void btnSaveBitPlane_Click(object sender, EventArgs e)
        {
            if (pbBitPlane.Image != null)
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Image Files|*.png;*.bmp";
                    saveFileDialog.Title = "Bit Plane Analiz Sonucunu Kaydet";
                    saveFileDialog.FileName = "BitPlaneAnalysis.png";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        pbBitPlane.Image.Save(saveFileDialog.FileName);
                    }
                }
            }
            else
            {
                MessageBox.Show("Kaydedilecek bir analiz sonucu yok.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



        #endregion TAB - Bit Plane Analizi

        #region TAB - Chi-Square Analizi
        private void btnChiSquare_Click(object sender, EventArgs e)
        {
            if (_encryptedImage != null)
            {
                try
                {
                    // Isı haritası oluştur
                    Bitmap heatmap = Cls_ChiSquareHelper.GenerateChiSquareHeatmap(
                        _encryptedImage,
                        ColorChannel.Red, // Örnek olarak kırmızı kanal
                        8, // Blok boyutu
                        Cls_ChiSquareHelper.DefaultColorMap // Varsayılan renk haritası
                    );
                    // Isı haritasını pbChiSquare üzerinde göster
                    pbChiSquare.Image = heatmap;

                    // Orijinal resmi pbChiSquareOrginal üzerinde göster
                    pbChiSquareOrginal.Image = pbOrginal.Image;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Lütfen önce bir resim seçin.");
            }
        }

        private void btnSaveChiSquare_Click(object sender, EventArgs e)
        {
            if (pbChiSquare.Image != null)
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Image Files|*.png;*.bmp";
                    saveFileDialog.Title = "Chi-Square Isı Haritasını Kaydet";
                    saveFileDialog.FileName = "ChiSquareHeatmap.png";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        pbChiSquare.Image.Save(saveFileDialog.FileName);
                    }
                }
            }
            else
            {
                MessageBox.Show("Kaydedilecek bir analiz sonucu yok.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion TAB - Chi-Square Analizi

        #region TAB - Histogram
        private void btnHistogram_Click(object sender, EventArgs e)
        {
            if (_encryptedImage != null)
            {
                try
                {
                    // Histogram hesapla
                    long[] histogram = Cls_Histogram.CalculateHistogram(_encryptedImage, HistogramMode.Grayscale);
                    // Histogramı görselleştir
                    Bitmap histogramImage = Cls_Histogram.VisualizeHistogram(histogram, new Size(400, 300), "Histogram");
                    // Histogramı pbHistogram üzerinde göster
                    pbHistogram.Image = histogramImage;

                    // Orijinal resmi pbHistogramOrginal üzerinde göster
                    pbHistogramOrginal.Image = _originalImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Lütfen önce bir resim seçin.");
            }
        }

        private void btnSaveHistogram_Click(object sender, EventArgs e)
        {
            if (pbHistogram.Image != null)
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Image Files|*.png;*.bmp";
                    saveFileDialog.Title = "Histogram Görselini Kaydet";
                    saveFileDialog.FileName = "Histogram.png";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        pbHistogram.Image.Save(saveFileDialog.FileName);
                    }
                }
            }
            else
            {
                MessageBox.Show("Kaydedilecek bir analiz sonucu yok.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion TAB - Histogram

        #region TAB - R/S Analizi
        private async void btnRS_Click(object sender, EventArgs e)
        {
            if (_encryptedImage == null)
            {
                MessageBox.Show("Lütfen önce bir resim yükleyin veya şifreleyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Bitmap imageToAnalyze = null; // Analiz edilecek resmin kopyası
            Bitmap rsChartBitmap = null;  // Sonuç grafik bitmap'i
            string errorMessage = null;   // Olası hata mesajı

            // İşlem başlarken UI'yı ayarla
            this.Cursor = Cursors.WaitCursor;
            btnRS.Enabled = false;
            pbRSImage.Image?.Dispose(); // Önceki grafiği temizle
            pbRSImage.Image = null;
            pbRSOrginal.Image?.Dispose();
            pbRSOrginal.Image = null;


            try
            {
                // Resmi klonla (arka plan thread'ine güvenli göndermek için)
                imageToAnalyze = (Bitmap)_encryptedImage.Clone();

                // --- Arka Planda Çalışacak İşlemler ---
                var analysisTask = Task.Run(() => {
                    double[] data = PrepareDataFromImage(imageToAnalyze); // Optimize edilmiş hazırlama

                    if (data == null || data.Length < 16) // Daha makul bir alt sınır (minSubSeriesLength * 2)
                    {
                        // Hata mesajını dışarı döndür
                        return (Success: false, Hurst: double.NaN, Values: (List<(int n, double rs)>)null, ErrMsg: "Analiz için yetersiz veri.");
                    }

                    // R/S Analizi
                    var result = Cls_RsHelper.RSAnalysis(data);

                    // Sonuçları kontrol et
                    if (result.rsValues == null || !result.rsValues.Any())
                    {
                        return (Success: false, Hurst: double.NaN, Values: (List<(int n, double rs)>)null, ErrMsg: "R/S Analizi sonuç üretmedi.");
                    }

                    return (Success: true, Hurst: result.hurstExponent, Values: result.rsValues, ErrMsg: (string)null);
                });

                // Görevin bitmesini bekle
                var taskResult = await analysisTask;
                // --- Arka Plan İşlemleri Bitti ---


                // Görev sonucunu işle
                if (taskResult.Success)
                {
                    // Görselleştirmeyi oluştur (UI thread'inde)
                    Chart rsChart = null;
                    Bitmap tempBitmap = null;
                    try
                    {
                        rsChart = Cls_RsHelper.VisualizeRSAnalysis(taskResult.Values, taskResult.Hurst);

                        // Chart'ı Bitmap'e render et
                        tempBitmap = new Bitmap(pbRSImage.Width > 0 ? pbRSImage.Width : 600, pbRSImage.Height > 0 ? pbRSImage.Height : 400); // PictureBox boyutunu kullan
                        rsChart.DrawToBitmap(tempBitmap, new Rectangle(0, 0, tempBitmap.Width, tempBitmap.Height));
                        rsChartBitmap = tempBitmap; // Başarılıysa ata
                        tempBitmap = null; // Geçici referansı kaldır
                    }
                    catch (Exception visEx)
                    {
                        errorMessage = $"Görselleştirme hatası: {visEx.Message}";
                    }
                    finally
                    {
                        rsChart?.Dispose(); // Oluşturulan Chart nesnesini temizle
                        tempBitmap?.Dispose(); // Hata oluşursa geçici bitmap'i temizle
                    }
                }
                else
                {
                    errorMessage = taskResult.ErrMsg; // Arka plandan gelen hata mesajı
                }

            }
            catch (Exception ex)
            {
                // Genel veya beklenmedik hatalar
                errorMessage = $"Genel Hata: {ex.Message}\n{ex.StackTrace}";
            }
            finally
            {
                // UI'yı Son Haline Getir
                this.Cursor = Cursors.Default;
                btnRS.Enabled = true;
                imageToAnalyze?.Dispose(); // Klonlanan resmi temizle


                if (rsChartBitmap != null)
                {
                    pbRSImage.Image = rsChartBitmap; // Sonuç grafiğini göster
                }
                else if (!string.IsNullOrEmpty(errorMessage))
                {
                    // Eğer grafik oluşturulamadıysa ve hata varsa göster
                    MessageBox.Show(errorMessage, "Analiz Sonucu / Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (errorMessage == null) // Başarılı ama grafik yoksa (olmamalı ama)
                {
                    MessageBox.Show("Analiz tamamlandı ancak görselleştirme oluşturulamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }


                // Orijinal resmi göster (hata olsa bile göstermeye çalışabiliriz)
                if (_originalImage != null && pbRSOrginal != null)
                {
                    try { pbRSOrginal.Image = (Bitmap)_originalImage.Clone(); } catch { }
                }
                pbRSImage.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        // Optimize edilmiş PrepareDataFromImage metodu (önceki cevapta olduğu gibi)
        private double[] PrepareDataFromImage(Bitmap sourceImage)
        {
            if (sourceImage == null) return null;
            // ... (LockBits ve Marshal.Copy kullanan kod) ...
            int width = sourceImage.Width;
            int height = sourceImage.Height;
            double[] data = new double[width * height];
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = null;

            try
            {
                bmpData = sourceImage.LockBits(rect, ImageLockMode.ReadOnly, sourceImage.PixelFormat);
                IntPtr ptr = bmpData.Scan0;
                int stride = bmpData.Stride;
                int bytesPerPixel = Image.GetPixelFormatSize(bmpData.PixelFormat) / 8;
                if (bytesPerPixel < 3) return null;
                int totalBytes = Math.Abs(stride) * height;
                byte[] rgbValues = new byte[totalBytes];
                Marshal.Copy(ptr, rgbValues, 0, totalBytes);

                int dataIndex = 0;
                for (int y = 0; y < height; y++)
                {
                    int lineStartIndex = y * Math.Abs(stride);
                    for (int x = 0; x < width; x++)
                    {
                        int pixelStartIndex = lineStartIndex + x * bytesPerPixel;
                        if (pixelStartIndex + 2 >= rgbValues.Length) continue;

                        byte b = rgbValues[pixelStartIndex];
                        byte g = rgbValues[pixelStartIndex + 1];
                        byte r = rgbValues[pixelStartIndex + 2];
                        data[dataIndex++] = (r + g + b) / 3.0; // Basit ortalama
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veri hazırlama hatası: {ex.Message}");
                return null;
            }
            finally
            {
                if (bmpData != null)
                {
                    sourceImage.UnlockBits(bmpData);
                }
            }
            return data;
        }

        private void btnSaveRS_Click(object sender, EventArgs e)
        {
            if (pbRSImage.Image != null)
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Image Files|*.png;*.bmp";
                    saveFileDialog.Title = "R/S Analiz Görselini Kaydet";
                    saveFileDialog.FileName = "RSAnalysis.png";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        pbRSImage.Image.Save(saveFileDialog.FileName);
                    }
                }
            }
            else
            {
                MessageBox.Show("Kaydedilecek bir analiz sonucu yok.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion TAB - R/S Analizi

        #region TAB - Analiz Raporu
        /// <summary>
        /// Analiz Raporu Oluşturma/Kaydetme
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnCreateAnalysisRapor_Click(object sender, EventArgs e)
        {

        }



        private void btnSaveAnalysisRapor_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text Files|*.txt";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, txtLog.Text);
                }
            }
        }
        #endregion TAB - Analiz Raporu

        /// <summary>
        /// Resimlerin boyutunu ayarlamak için kullanılan metodlar
        /// </summary>


        // Seçilen resimleri Picture Box a sığacak şekilde yeniden boyutlandır
        private void SetPictureBoxSizeMode()
        {
            // Resimlerin boyutunu ayarla
            pbOrginal.SizeMode = PictureBoxSizeMode.Zoom;
            pbEncrypted.SizeMode = PictureBoxSizeMode.Zoom;

            // Bit Plane
            pbBitPlaneOrginal.SizeMode = PictureBoxSizeMode.Zoom;
            pbBitPlane.SizeMode = PictureBoxSizeMode.Zoom;

            // Chi-Square
            pbChiSquareOrginal.SizeMode = PictureBoxSizeMode.Zoom;
            pbChiSquare.SizeMode = PictureBoxSizeMode.Zoom;

            // Histogram
            pbHistogramOrginal.SizeMode = PictureBoxSizeMode.Zoom;
            pbHistogram.SizeMode = PictureBoxSizeMode.Zoom;

            // RS
            pbRSOrginal.SizeMode = PictureBoxSizeMode.Zoom;
            pbRSImage.SizeMode = PictureBoxSizeMode.Zoom;
        }
        // Tıklanan resmin tam ekran gösterilmesi
        private void pbOrginal_Click(object sender, EventArgs e)
        {
            ShowImageFullScreen(pbOrginal.Image);
        }


        private void ShowImageFullScreen(Image image)
        {
            Form fullScreenForm = new Form();
            fullScreenForm.FormBorderStyle = FormBorderStyle.None;
            fullScreenForm.WindowState = FormWindowState.Maximized;
            fullScreenForm.BackColor = Color.Black;
            fullScreenForm.BackgroundImage = image;
            fullScreenForm.BackgroundImageLayout = ImageLayout.Zoom;
            fullScreenForm.Click += (s, e) => fullScreenForm.Close();
            fullScreenForm.ShowDialog();
        }

    }
} 