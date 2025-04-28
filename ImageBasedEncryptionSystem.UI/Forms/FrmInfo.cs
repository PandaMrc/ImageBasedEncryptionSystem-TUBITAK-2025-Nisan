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

namespace ImageBasedEncryptionSystem.UI.Forms
{
    public partial class FrmInfo : Form
    {
        // FrmMenu'ya dönmek için referans
        private FrmMenu _menuForm;
        
        // RichTextBox kontrolü
        private RichTextBox txtInfo;

        public FrmInfo()
        {
            InitializeComponent();
            InitializeInformation();
        }

        public FrmInfo(FrmMenu menuForm)
        {
            InitializeComponent();
            _menuForm = menuForm;
            InitializeInformation();
        }

        private void FrmInfo_Load(object sender, EventArgs e)
        {
            // Modern arka plan oluştur
            try
            {
                Cls_Background.Instance.CreateModernBackground(this);
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Arka plan oluşturulurken hata oluştu: {ex.Message}", 
                    "Arka Plan Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            // Tab içeriklerini ayarla
            SetTabContents();
        }

        private void SetTabContents()
        {
            // Tüm richTextBox kontrollerinde metin rengini ve stil özelliklerini ayarla
            foreach (Control tabPage in tabControl.TabPages)
            {
                foreach (Control control in tabPage.Controls)
                {
                    if (control is RichTextBox rtb)
                    {
                        rtb.BackColor = Color.FromArgb(30, 30, 50);
                        rtb.ForeColor = Color.White;
                        rtb.BorderStyle = BorderStyle.None;
                        rtb.ReadOnly = true;
                        rtb.ScrollBars = RichTextBoxScrollBars.Vertical;
                    }
                }
            }

            // Tab içeriklerini ayarla
            // Bilgilendirme tab içeriği
            rtbBilgilendirme.Text = "Resim Tabanlı Şifreleme Sistemi\r\n\r\n" +
                "Bu uygulama, görsel şifreleme teknikleri kullanarak güvenli bir şifreleme sistemi sağlar. " +
                "Metinleri, belgeleri ve diğer dosyaları bir resim içerisine gizlemenize imkan tanır.\r\n\r\n" +
                "Sistem, stenografi ve kriptografi tekniklerini bir araya getirerek çift katmanlı bir güvenlik sağlar. " +
                "Verileriniz öncelikle güçlü bir algoritma ile şifrelenir, ardından görsel içerisine gömülür.\r\n\r\n" +
                "Temel Özellikler:\r\n" +
                "• Dosya şifreleme ve gizleme\r\n" +
                "• Gelişmiş şifreleme algoritmaları\r\n" +
                "• Şifreli iletişim\r\n" +
                "• Güvenlik analizi\r\n" +
                "• Kullanıcı dostu arayüz\r\n\r\n" +
                "Bu uygulama, yalnızca meşru amaçlar için kullanılmak üzere tasarlanmıştır. " +
                "Kötü niyetli kullanım yasalara aykırı olabilir ve yasal sorumluluk doğurabilir.";

            // Nasıl Kullanılır tab içeriği
            rtbNasilKullanilir.Text = "Kullanım Kılavuzu\r\n\r\n" +
                "1. Şifreleme İşlemi:\r\n" +
                "   a) Ana menüden 'Şifreleme' seçeneğini seçin.\r\n" +
                "   b) Şifrelemek istediğiniz dosyayı veya metni seçin.\r\n" +
                "   c) Taşıyıcı görsel olarak kullanılacak bir resim seçin.\r\n" +
                "   d) Güçlü bir şifre belirleyin. Bu şifreyi hatırlamanız önemlidir.\r\n" +
                "   e) 'Şifrele' butonuna tıklayın.\r\n" +
                "   f) Şifrelenmiş resmi bilgisayarınıza kaydedin.\r\n\r\n" +
                "2. Şifre Çözme İşlemi:\r\n" +
                "   a) Ana menüden 'Şifre Çözme' seçeneğini seçin.\r\n" +
                "   b) Şifrelenmiş resim dosyasını seçin.\r\n" +
                "   c) Daha önce belirlediğiniz şifreyi girin.\r\n" +
                "   d) 'Şifreyi Çöz' butonuna tıklayın.\r\n" +
                "   e) Çözülen dosyayı veya metni kaydedin.\r\n\r\n" +
                "3. Güvenlik İpuçları:\r\n" +
                "   • Karmaşık ve uzun şifreler kullanın.\r\n" +
                "   • Şifrelerinizi güvenli bir yerde saklayın.\r\n" +
                "   • Önemli dosyaları şifrelemeden önce yedekleyin.\r\n" +
                "   • Taşıyıcı resim olarak yüksek çözünürlüklü ve detaylı görseller kullanın.";

            // Nasıl Çalışır tab içeriği
            rtbNasilCalisir.Text = "Sistem Çalışma Prensibi\r\n\r\n" +
                "Resim Tabanlı Şifreleme Sistemi, birkaç temel prensip üzerine kurulmuştur:\r\n\r\n" +
                "1. Şifreleme Aşaması:\r\n" +
                "   • Dosya/metin önce AES-256 şifreleme algoritması ile şifrelenir.\r\n" +
                "   • Şifrelenen veri, Wavelet dönüşümü stenografi yöntemiyle resim içerisine gömülür.\r\n" +
                "   • Bu işlem sırasında resmin dalgacık katsayıları değiştirilerek veri gizlenir.\r\n" +
                "   • Gözle görülmeyen bu değişiklikler, resmin görünümünü etkilemez.\r\n\r\n" +
                "2. Şifre Çözme Aşaması:\r\n" +
                "   • Şifrelenmiş resim okunur ve Wavelet dönüşümü yöntemiyle gizlenmiş veri çıkarılır.\r\n" +
                "   • Çıkarılan veri, doğru şifre kullanılarak AES-256 algoritmasıyla çözülür.\r\n" +
                "   • Orijinal dosya/metin kullanıcıya sunulur.\r\n\r\n" +
                "3. Gizlilik Mekanizması:\r\n" +
                "   • Sistem, çift katmanlı güvenlik sağlar: kriptografik şifreleme ve stenografik gizleme.\r\n" +
                "   • Resmin içinde veri olduğu, gelişmiş analiz yöntemleri olmadan anlaşılamaz.\r\n" +
                "   • Şifre olmadan, resimden veriyi çıkarmak mümkün olsa bile, şifrelenmiş verinin çözülmesi imkansızdır.";

            // Analiz tab içeriği
            rtbAnaliz.Text = "Güvenlik Analizi ve Performans\r\n\r\n" +
                "1. Güvenlik Analizi:\r\n" +
                "   • AES-256 Şifreleme: Günümüzde bilinen en güvenli şifreleme algoritmalarından biridir.\r\n" +
                "   • Brute Force Saldırı Direnci: 2^256 olası anahtar kombinasyonu içerir, bu da mevcut bilgisayar\r\n" +
                "     teknolojileriyle kırılması imkansız denecek kadar zorlaştırır.\r\n" +
                "   • Stenografi Analizi: Resim içinde veri gizlendiğinin tespit edilmesi için özel araçlar gerekir.\r\n" +
                "   • Çift Katmanlı Koruma: Verinin hem şifrelenmesi hem gizlenmesi, tekli korumaya göre çok daha güvenlidir.\r\n\r\n" +
                "2. Performans Analizi:\r\n" +
                "   • Şifreleme Hızı: Dosya boyutuna bağlı olarak saniyeler içinde gerçekleşir.\r\n" +
                "   • Resim Kapasitesi: Bir resmin gizleyebileceği veri miktarı, resmin piksel sayısına bağlıdır.\r\n" +
                "     Örneğin: 1024x768 piksel bir resim, yaklaşık 300KB veri gizleyebilir.\r\n" +
                "   • Çıktı Kalitesi: İşlem sonrası resimde gözle görülür kalite kaybı olmaz.\r\n\r\n" +
                "3. Güvenlik/Performans Dengesi:\r\n" +
                "   • Sistem, yüksek güvenlik sağlarken makul performans sunar.\r\n" +
                "   • Büyük dosyalar için daha büyük resimler veya çoklu resim kullanımı gerekebilir.\r\n" +
                "   • Maksimum güvenlik için, taşıyıcı resmin sıradan ve dikkat çekmeyen bir görsel olması önerilir.";

            // Güvenlik tab içeriği
            rtbGuvenlik.Text = "Güvenlik Önlemleri ve Tavsiyeler\r\n\r\n" +
                "1. Şifre Güvenliği:\r\n" +
                "   • En az 12 karakter uzunluğunda şifreler kullanın.\r\n" +
                "   • Büyük/küçük harfler, rakamlar ve özel karakterler içeren karmaşık şifreler oluşturun.\r\n" +
                "   • Sözlükte bulunan kelimeleri şifre olarak kullanmaktan kaçının.\r\n" +
                "   • Kişisel bilgilerinizi (doğum tarihi, isim vb.) şifrenizde kullanmayın.\r\n" +
                "   • Her önemli veri için farklı şifreler kullanın.\r\n\r\n" +
                "2. Sistem Güvenliği:\r\n" +
                "   • İşletim sisteminizi ve güvenlik yazılımlarınızı güncel tutun.\r\n" +
                "   • Şifrelenmiş resimleri güvenli ortamlarda saklayın ve paylaşın.\r\n" +
                "   • Şifreleme işlemlerini güvendiğiniz cihazlarda gerçekleştirin.\r\n" +
                "   • Önemli verileri şifrelerken internet bağlantınızı kesin.\r\n\r\n" +
                "3. Dikkat Edilmesi Gerekenler:\r\n" +
                "   • Şifrelerinizi unutursanız, verilerinize erişemezsiniz. Bu sistemde 'şifremi unuttum' seçeneği yoktur.\r\n" +
                "   • Şifrelenmiş resim dosyalarını yeniden boyutlandırmak veya düzenlemek, içindeki verilerin bozulmasına neden olabilir.\r\n" +
                "   • Şifrelenmiş dosyalarınızı düzenli olarak yedekleyin.\r\n" +
                "   • Bu sistemi yasadışı amaçlar için kullanmayın.";

            // Hakkında tab içeriği
            rtbHakkinda.Text = "Resim Tabanlı Şifreleme Sistemi Hakkında\r\n\r\n" +
                "Sürüm: 1.0.0\r\n" +
                "Geliştirici: [Geliştirici Adı]\r\n" +
                "Lisans: Tüm Hakları Saklıdır © 2023\r\n\r\n" +
                "Bu uygulama, modern şifreleme ve stenografi tekniklerini kullanarak güvenli veri gizleme ve iletişim sağlamak amacıyla geliştirilmiştir.\r\n\r\n" +
                "Kullanılan Teknolojiler:\r\n" +
                "• .NET Framework\r\n" +
                "• AES-256 Şifreleme\r\n" +
                "• Wavelet Dönüşümü Stenografi\r\n" +
                "• Guna UI Framework\r\n\r\n" +
                "İletişim:\r\n" +
                "E-posta: info@resimsifrele.com\r\n" +
                "Web: www.resimsifrele.com\r\n\r\n" +
                "Geri bildirimleriniz ve önerileriniz için teşekkür ederiz. Uygulamamızı geliştirmemize yardımcı olun.\r\n\r\n" +
                "Not: Bu uygulama eğitim ve meşru güvenlik amaçları için tasarlanmıştır. Yasa dışı faaliyetlerde kullanımı yasaktır.";
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            // Ana menüye dön
            if (_menuForm != null)
            {
                this.Hide();
                _menuForm.Show();
            }
            else
            {
                FrmMenu newMenuForm = new FrmMenu();
                this.Hide();
                newMenuForm.Show();
            }
        }

        private void FrmInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Form kapatıldığında uygulamayı sonlandırma, sadece gizle
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();

                if (_menuForm != null)
                {
                    _menuForm.Show();
                }
                else
                {
                    FrmMenu newMenuForm = new FrmMenu();
                    newMenuForm.Show();
                }
            }
        }

        // Form içeriğini hazırla
        private void InitializeInformation()
        {
            // txtInfo kontrolünü oluştur
            txtInfo = new RichTextBox();
            txtInfo.Size = new Size(600, 400);
            txtInfo.Location = new Point(12, 40);
            txtInfo.BackColor = Color.FromArgb(30, 30, 50);
            txtInfo.ForeColor = Color.White;
            txtInfo.BorderStyle = BorderStyle.None;
            txtInfo.ReadOnly = true;

            // Program adı ve özelliklerini ekle
            DisplayTitle();
            DisplayGeneralDescription();
            DisplayWorkflowDescription();
            DisplayKeyFeatures();
            DisplaySecurityFeatures();
            DisplayUsageTips();
            DisplayVersion();
        }

        // Program başlığı ve yazarlar
        private void DisplayTitle()
        {
            txtInfo.SelectionFont = new Font(txtInfo.Font.FontFamily, 16, FontStyle.Bold);
            txtInfo.SelectionAlignment = HorizontalAlignment.Center;
            txtInfo.AppendText("IMAGE BASED ENCRYPTION SYSTEM\r\n\r\n");

            txtInfo.SelectionFont = new Font(txtInfo.Font.FontFamily, 10, FontStyle.Italic);
            txtInfo.SelectionAlignment = HorizontalAlignment.Center;
            txtInfo.AppendText("Geliştiriciler: Meriç Zorlu & Erçin Berk\r\n\r\n");
        }

        // Genel program açıklaması
        private void DisplayGeneralDescription()
        {
            txtInfo.SelectionFont = new Font(txtInfo.Font.FontFamily, 12, FontStyle.Bold);
            txtInfo.SelectionAlignment = HorizontalAlignment.Left;
            txtInfo.AppendText("PROGRAM HAKKINDA\r\n\r\n");

            txtInfo.SelectionFont = new Font(txtInfo.Font.FontFamily, 10, FontStyle.Regular);
            txtInfo.SelectionAlignment = HorizontalAlignment.Left;
            txtInfo.AppendText(
                "Image Based Encryption System, metin tabanlı verileri görüntüler içerisine güvenli bir şekilde gizleyebilen " +
                "ve şifreli olarak saklayabilen gelişmiş bir steganografi uygulamasıdır. Program, " +
                "çoklu şifreleme algoritmaları kullanarak steganografi işlemi öncesinde metninizi " +
                "şifreler ve böylece maksimum güvenlik sağlar.\r\n\r\n");
        }

        // Çalışma akışı açıklaması
        private void DisplayWorkflowDescription()
        {
            txtInfo.SelectionFont = new Font(txtInfo.Font.FontFamily, 12, FontStyle.Bold);
            txtInfo.AppendText("ÇALIŞMA PRENSİBİ\r\n\r\n");

            txtInfo.SelectionFont = new Font(txtInfo.Font.FontFamily, 10, FontStyle.Regular);
            txtInfo.AppendText("Şifreleme İşlemi Akışı:\r\n");
            txtInfo.AppendText(
                "   • Kullanıcı bir metin, bir parola ve bir resim dosyası seçer.\r\n" +
                "   • Parola AES şifreleme anahtarına dönüştürülür.\r\n" +
                "   • Metin, AES algoritması ile şifrelenir.\r\n" +
                "   • AES anahtarı RSA ile şifrelenir.\r\n" +
                "   • Şifrelenen veri, Wavelet dönüşümü stenografi yöntemiyle resim içerisine gömülür.\r\n" +
                "   • Sonuç olarak görsel açıdan normal görünen ama içerisinde şifreli veri taşıyan bir resim oluşur.\r\n\r\n");

            txtInfo.AppendText("Şifre Çözme İşlemi Akışı:\r\n");
            txtInfo.AppendText(
                "   • Kullanıcı şifreli resim ve parolayı girer.\r\n" +
                "   • Şifrelenmiş resim okunur ve Wavelet dönüşümü yöntemiyle gizlenmiş veri çıkarılır.\r\n" +
                "   • Parola AES anahtarına dönüştürülür ve doğrulanır.\r\n" +
                "   • AES anahtarı kullanılarak şifreli metin çözülür.\r\n" +
                "   • Orijinal metin kullanıcıya gösterilir.\r\n\r\n");
        }

        // Anahtar özellikleri
        private void DisplayKeyFeatures()
        {
            txtInfo.SelectionFont = new Font(txtInfo.Font.FontFamily, 12, FontStyle.Bold);
            txtInfo.AppendText("ANAHTAR ÖZELLİKLER\r\n\r\n");

            txtInfo.SelectionFont = new Font(txtInfo.Font.FontFamily, 10, FontStyle.Regular);
            txtInfo.AppendText(
                "• Çoklu Şifreleme\r\n" +
                "  - AES-256 şifreleme (metin için)\r\n" +
                "  - RSA-2048 şifreleme (anahtar koruma için)\r\n\r\n" +
                "• Wavelet Dönüşümü Steganografi\r\n" +
                "  - Farklı Wavelet aileleri desteği (Haar, Daubechies)\r\n" +
                "  - Çoklu ayrışım seviyesi desteği\r\n" +
                "  - Görünmez veri saklama\r\n\r\n" +
                "• Kullanıcı Dostu Arayüz\r\n" +
                "  - Basit ve sezgisel kullanım\r\n" +
                "  - Adım adım rehberlik\r\n\r\n" +
                "• Geliştirici Modu\r\n" +
                "  - İleri seviye özellikler\r\n" +
                "  - Analiz araçları\r\n" +
                "  - İşlem geçmişi\r\n\r\n");
        }

        // Güvenlik özellikleri
        private void DisplaySecurityFeatures()
        {
            txtInfo.SelectionFont = new Font(txtInfo.Font.FontFamily, 12, FontStyle.Bold);
            txtInfo.AppendText("GÜVENLİK ÖZELLİKLERİ\r\n\r\n");

            txtInfo.SelectionFont = new Font(txtInfo.Font.FontFamily, 10, FontStyle.Regular);
            txtInfo.AppendText(
                "• Parola Gereksinimi\r\n" +
                "  - Şifrelenmemiş veri asla görüntüye gömülmez\r\n" +
                "  - Görsel analiz ile veri tespiti neredeyse imkansız\r\n\r\n" +
                "• Çoklu Katman Güvenliği\r\n" +
                "  - Veri önce AES ile şifrelenir\r\n" +
                "  - AES anahtarı RSA ile korunur\r\n" +
                "  - Şifreli veri Wavelet dönüşümü ile gizlenir\r\n\r\n" +
                "• Güvenli Şifre Yönetimi\r\n" +
                "  - Anahtarlar bellek içinde güvenli şekilde işlenir\r\n" +
                "  - Şifre geçerlilik kontrolü\r\n\r\n");
        }

        // Kullanım önerileri
        private void DisplayUsageTips()
        {
            txtInfo.SelectionFont = new Font(txtInfo.Font.FontFamily, 12, FontStyle.Bold);
            txtInfo.AppendText("KULLANIM ÖNERİLERİ\r\n\r\n");

            txtInfo.SelectionFont = new Font(txtInfo.Font.FontFamily, 10, FontStyle.Regular);
            txtInfo.AppendText(
                "• Yüksek çözünürlüklü görseller kullanın (ideal: 1024x1024 veya daha yüksek).\r\n" +
                "• Karmaşık desenlere sahip görseller tercih edin.\r\n" +
                "• Güçlü parolalar oluşturun (en az 8 karakter, sayı ve özel karakter içeren).\r\n" +
                "• Çok büyük metinler için görsel boyutunu uygun seçin.\r\n" +
                "• Şifrelenmiş görseli kayıpsız formatta (PNG) saklayın.\r\n" +
                "• Görsele filtre uygulamayın veya yeniden boyutlandırmayın.\r\n\r\n");
        }

        // Versiyon bilgisi
        private void DisplayVersion()
        {
            txtInfo.SelectionFont = new Font(txtInfo.Font.FontFamily, 9, FontStyle.Italic);
            txtInfo.SelectionAlignment = HorizontalAlignment.Center;
            txtInfo.AppendText("Versiyon 2.0.0 | © 2023 Tüm Hakları Saklıdır\r\n");
        }

        // Geri dönüş butonu
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
