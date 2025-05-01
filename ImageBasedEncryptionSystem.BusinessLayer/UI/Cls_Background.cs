using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace ImageBasedEncryptionSystem.BusinessLayer.UI
{
    public class Cls_Background
    {
        // Singleton pattern kullanarak sınıfın tek bir örneğini oluşturma
        private static Cls_Background _instance;
        
        public static Cls_Background Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Cls_Background();
                }
                return _instance;
            }
        }
        
        private Cls_Background()
        {
            // Özel constructor
        }
        
        /// <summary>
        /// Herhangi bir form için modern şifreleme temalı arka plan oluşturur
        /// </summary>
        /// <param name="form">Arka plan uygulanacak form</param>
        /// <param name="saveToDisk">Arka planın diske kaydedilip kaydedilmeyeceği</param>
        public void CreateModernBackground(Form form, bool saveToDisk = true)
        {
            try
            {
                // Arka plan resmi oluştur
                using (Bitmap background = new Bitmap(form.Width, form.Height))
                {
                    using (Graphics g = Graphics.FromImage(background))
                    {
                        // Gece mavisi gradient arka plan
                        Rectangle rect = new Rectangle(0, 0, background.Width, background.Height);
                        using (LinearGradientBrush brush = new LinearGradientBrush(
                            rect,
                            Color.FromArgb(25, 25, 50),   // Koyu lacivert
                            Color.FromArgb(10, 10, 30),   // Daha koyu ton
                            LinearGradientMode.ForwardDiagonal))
                        {
                            g.FillRectangle(brush, rect);

                            // Siber güvenlik temalı görsel öğeler ekle
                            
                            // 1. Parıldayan yıldızlar (daha parlak ve daha fazla)
                            Random rnd = new Random();
                            using (SolidBrush starBrush = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
                            {
                                for (int i = 0; i < 300; i++)
                                {
                                    int x = rnd.Next(background.Width);
                                    int y = rnd.Next(background.Height);
                                    int size = rnd.Next(1, 3);
                                    g.FillEllipse(starBrush, x, y, size, size);
                                }
                            }
                            
                            // 2. Dijital şifreleme simgelerini temsil eden hafif ışık çizgileri
                            using (Pen lightPen = new Pen(Color.FromArgb(30, 0, 120, 215), 1.5f))
                            {
                                // Arka planda rastgele yatay çizgiler
                                for (int i = 0; i < 20; i++)
                                {
                                    int y = rnd.Next(background.Height);
                                    int startX = rnd.Next(background.Width / 4);
                                    int endX = startX + rnd.Next(50, 200);
                                    
                                    g.DrawLine(lightPen, startX, y, endX, y);
                                }
                            }
                            
                            // 3. Dijital veri sembolizasyonu (1 ve 0'ları temsil eden şekiller)
                            using (SolidBrush digitBrush = new SolidBrush(Color.FromArgb(40, 100, 200, 255)))
                            {
                                // Rastgele "1" ve "0" rakamları
                                using (Font digitFont = new Font("Consolas", 8, FontStyle.Bold))
                                {
                                    for (int i = 0; i < 150; i++)
                                    {
                                        string digit = rnd.Next(2).ToString(); // 0 veya 1
                                        int x = rnd.Next(background.Width);
                                        int y = rnd.Next(background.Height);
                                        
                                        g.DrawString(digit, digitFont, digitBrush, x, y);
                                    }
                                }
                            }
                            
                            // 4. Şifreli mesaj görünümü
                            using (SolidBrush codeBrush = new SolidBrush(Color.FromArgb(30, 0, 255, 160)))
                            {
                                string[] codeLines = {
                                    "AES-256",
                                    "RSA-3062",
                                    "SHA-512",
                                    "ENCRYPT",
                                    "DECRYPT",
                                    "SECURE",
                                    "ZORLU",
                                    "IBES"
                                };
                                
                                using (Font codeFont = new Font("Courier New", 10, FontStyle.Bold))
                                {
                                    for (int i = 0; i < 10; i++)
                                    {
                                        string code = codeLines[rnd.Next(codeLines.Length)];
                                        int x = rnd.Next(background.Width - 100);
                                        int y = rnd.Next(background.Height - 50);
                                        
                                        g.DrawString(code, codeFont, codeBrush, x, y);
                                    }
                                }
                            }
                        }
                    }

                    // Form arka planı olarak ayarla
                    form.BackgroundImage = new Bitmap(background);
                    form.BackgroundImageLayout = ImageLayout.Stretch;
                    
                    // İsteğe bağlı: arka planı kaydet
                    if (saveToDisk)
                    {
                        SaveBackgroundToDisk(background, form.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Arka plan oluşturulurken bir hata oluştu: {ex.Message}", 
                                "Arka Plan Hatası", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// Arka plan resmini diske kaydeder
        /// </summary>
        private void SaveBackgroundToDisk(Bitmap background, string formName)
        {
            try
            {
                string appPath = Application.StartupPath;
                string resourcesFolder = Path.Combine(appPath, "Resources");
                
                if (!Directory.Exists(resourcesFolder))
                {
                    Directory.CreateDirectory(resourcesFolder);
                }
                
                string backgroundImagePath = Path.Combine(resourcesFolder, $"background_{formName}.png");
                background.Save(backgroundImagePath, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch
            {
                // Kaydetme hatası olursa sessizce devam et
                // Kritik bir işlem değil, kullanıcıya göstermeye gerek yok
            }
        }
    }
}
