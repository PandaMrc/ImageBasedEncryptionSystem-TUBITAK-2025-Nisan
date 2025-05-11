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
                        rtb.BackColor = Color.FromArgb(40, 40, 40);
                        rtb.ForeColor = Color.White;
                        rtb.BorderStyle = BorderStyle.None;
                        rtb.ReadOnly = true;
                        rtb.ScrollBars = RichTextBoxScrollBars.Vertical;
                    }
                }
            }

            // Tab içeriklerini ayarla
            // Bilgilendirme tab içeriği
            SetFormattedText(rtbInformation, "🔐 Resim Tabanlı Şifreleme Sistemi 🔐", Color.Cyan, true, 14);
            rtbInformation.AppendText("\r\n\r\n");
            rtbInformation.SelectionColor = Color.White;
            rtbInformation.AppendText("Bu uygulama, gelişmiş metin şifreleme ve görsel şifreleme teknikleri kullanarak yüksek güvenlikli bir şifreleme sistemi sağlar. " +
                "Şimdilik sadece metinleri bir resmin içerisine güvenli bir şekilde gizlemenizi sağlar.\r\n\r\n" +
                "Sistem, stenografi ve kriptografi tekniklerini bir araya getirerek çift katmanlı bir güvenlik sağlar. " +
                "Verileriniz öncelikle güçlü bir algoritma ile iki aşamalı olarak şifrelenir, ardından görsel içerisine gömülür.\r\n\r\n");
            
            SetFormattedText(rtbInformation, "✨ Temel Özellikler:", Color.LightGreen, true, 12);
            rtbInformation.AppendText("\r\n");
            rtbInformation.SelectionColor = Color.Yellow;
            rtbInformation.AppendText("• 🛡️ Gelişmiş şifreleme algoritmaları\r\n");
            rtbInformation.SelectionColor = Color.Yellow;
            rtbInformation.AppendText("• 🔒 Şifreli iletişim\r\n");
            rtbInformation.SelectionColor = Color.Yellow;
            rtbInformation.AppendText("• 🖥️ Kullanıcı dostu arayüz\r\n\r\n");
            
            rtbInformation.SelectionColor = Color.Orange;
            rtbInformation.AppendText("⚠️ Bu uygulama, yalnızca meşru amaçlar için kullanılmak üzere tasarlanmıştır. " +
                "Kötü niyetli kullanım yasalara aykırı olabilir ve yasal sorumluluk doğurabilir.");

            // Nasıl Kullanılır tab içeriği
            SetFormattedText(rtbUseing, "📚 Kullanım Kılavuzu 📚", Color.Cyan, true, 14);
            rtbUseing.AppendText("\r\n\r\n");
            
            SetFormattedText(rtbUseing, "1. 🔒 Şifreleme İşlemi:", Color.LightGreen, true, 12);
            rtbUseing.AppendText("\r\n");
            rtbUseing.SelectionColor = Color.White;
            rtbUseing.AppendText("   a) Menüdeki Resim seçme butonuna tıklayarak uygun bir resim seçin.\r\n" +
                "   b) Şifrelemek istediğiniz metni yazın.\r\n" +
                "   c) Şifreleme işleminde kullanılacak güçlü parolayı yazın. Bu parolayı unutmamalısınız, yoksa veriye bir daha erişemeyebilirsiniz.\r\n" +
                "   d) 'Şifrele' butonuna tıklayın.\r\n" +
                "   e) Şifrelenmiş resmi bilgisayarınıza kaydedin.\r\n\r\n");
            
            SetFormattedText(rtbUseing, "2. 🔓 Şifre Çözme İşlemi:", Color.LightGreen, true, 12);
            rtbUseing.AppendText("\r\n");
            rtbUseing.SelectionColor = Color.White;
            rtbUseing.AppendText("   a) Menüdeki resim seçme butonuna tıklayarak içinde veri gizli resmi seçin.\r\n" +
                "   b) Şifreleme esnasında kullanılan parolayı girin.\r\n" +
                "   c) 'Şifre Çöz' butonuna tıklayın.\r\n" +
                "   d) Bütün bilgiler doğru ise çözülen metni göreceksiniz.\r\n\r\n");
            
            SetFormattedText(rtbUseing, "3. 🛠️ Gelişmiş Özellikler:", Color.LightGreen, true, 12);
            rtbUseing.AppendText("\r\n");
            rtbUseing.SelectionColor = Color.White;
            rtbUseing.AppendText("   a) Geliştirici modu ile daha fazla teknik seçenek kullanabilirsiniz.\r\n" +
                "   b) Görsel Analiz işlemleri ile içinde veri gizli olan resim ile orijinal resim arasındaki gözle görülemeyen farkları görebilirsiniz.\r\n\r\n");
            
            SetFormattedText(rtbUseing, "4. 💡 Güvenlik İpuçları:", Color.LightGreen, true, 12);
            rtbUseing.AppendText("\r\n");
            rtbUseing.SelectionColor = Color.Yellow;
            rtbUseing.AppendText("   • Karmaşık ve uzun şifreler kullanın.\r\n" +
                "   • Şifreleri güvenli bir yerde saklayın.\r\n" +
                "   • İçine veri gizlemek istediğiniz resmin yüksek çözünürlüklü ve detaylı bir resim olduğuna dikkat edin.\r\n" +
                "   • Seçilen resmin çözünürlüğü ne kadar yüksekse içine saklanabilecek veri boyutu o kadar yüksektir.");

            // Nasıl Çalışır tab içeriği
            SetFormattedText(rtbWorking, "⚙️ Sistem Çalışma Prensibi ⚙️", Color.Cyan, true, 14);
            rtbWorking.AppendText("\r\n\r\n");
            rtbWorking.SelectionColor = Color.White;
            rtbWorking.AppendText("Resim Tabanlı Şifreleme Sistemi, birkaç temel prensip üzerine kurulmuştur:\r\n\r\n");
            
            SetFormattedText(rtbWorking, "1. 🔒 Şifreleme Aşaması:", Color.LightGreen, true, 12);
            rtbWorking.AppendText("\r\n");
            rtbWorking.SelectionColor = Color.LightSkyBlue;
            rtbWorking.AppendText("   • 🔑 Parola kısmına girilen metin ile bir AES anahtarı oluşturulur.\r\n");
            rtbWorking.SelectionColor = Color.LightSkyBlue;
            rtbWorking.AppendText("   • 🛡️ Oluşturulan anahtar ile metin önce AES-256 şifreleme algoritması ile şifrelenir.\r\n");
            rtbWorking.SelectionColor = Color.LightSkyBlue;
            rtbWorking.AppendText("   • 🔐 AES-256 anahtarı RSA-3072 algoritması ile şifrelenir.\r\n");
            rtbWorking.SelectionColor = Color.LightSkyBlue;
            rtbWorking.AppendText("   • 🖼️ AES ile şifrelenen metin ve RSA-3072 ile şifrelenen AES anahtarı özel LSB algoritmamızla resmin içerisine gizlenir.\r\n");
            rtbWorking.SelectionColor = Color.LightSkyBlue;
            rtbWorking.AppendText("   • 📏 Resim çözünürlüğü ne kadar yüksekse o kadar uzunlukta veriyi içinde saklayabilir ve bununla orantılı olarak bozulma o kadar az olur.\r\n");
            rtbWorking.SelectionColor = Color.LightSkyBlue;
            rtbWorking.AppendText("   • 📊 256x256 çözünürlüğe sahip bir resmin içerisine sadece İngilizce karakter kullanırsak yaklaşık 24.000 karakterlik bir metin sığdırabiliriz.\r\n");
            rtbWorking.SelectionColor = Color.LightSkyBlue;
            rtbWorking.AppendText("   • 📈 2560x1440 çözünürlüğe sahip bir resimde ise bu 1.380.400 karakteri aşabilir.\r\n");
            rtbWorking.SelectionColor = Color.LightSkyBlue;
            rtbWorking.AppendText("   • ℹ️ Bu değerler arka planı şeffaf olmayan görseller için geçerlidir.\r\n");
            rtbWorking.SelectionColor = Color.Orange;
            rtbWorking.AppendText("   • ⚠️ Arka planı şeffaf görsellerde saklanan verinin uzunluğuna bağlı şeffaflıkta bozulma gerçekleşebilir.\r\n\r\n");
            
            SetFormattedText(rtbWorking, "2. 🔓 Şifre Çözme Aşaması:", Color.LightGreen, true, 12);
            rtbWorking.AppendText("\r\n");
            rtbWorking.SelectionColor = Color.LightSkyBlue;
            rtbWorking.AppendText("   • 🔑 Parola kısmına girilen metinle yeni AES anahtarı oluşturulur.\r\n");
            rtbWorking.SelectionColor = Color.LightSkyBlue;
            rtbWorking.AppendText("   • 🖼️ Şifrelenmiş resim okunur ve LSB algoritmamızı kullanarak içindeki veri çıkartılır.\r\n");
            rtbWorking.SelectionColor = Color.LightSkyBlue;
            rtbWorking.AppendText("   • 🔐 Çıkartılan RSA ile şifrelenmiş AES anahtarı çözülür.\r\n");
            rtbWorking.SelectionColor = Color.LightSkyBlue;
            rtbWorking.AppendText("   • ✅ Yeni oluşturulan AES anahtarı ve şifresi çözülen AES anahtarı karşılaştırılır.\r\n");
            rtbWorking.SelectionColor = Color.LightSkyBlue;
            rtbWorking.AppendText("   • 📝 Anahtarlar uyuşursa metin AES anahtarı kullanılarak çözülür ve kullanıcıya gösterilir.\r\n");
            rtbWorking.SelectionColor = Color.LightSkyBlue;
            rtbWorking.AppendText("   ⚠️ NOT: Şifreleme ve şifre çözme esnasında kullanılan Sistem kimliği aynı değilse şifre çözme işlemi başarılı olmaz.\r\n\r\n");
            
            SetFormattedText(rtbWorking, "3. 🕵️ Gizlilik Mekanizması:", Color.LightGreen, true, 12);
            rtbWorking.AppendText("\r\n");
            rtbWorking.SelectionColor = Color.LightSkyBlue;
            rtbWorking.AppendText("   • 🛡️ Sistem, üç katmanlı güvenlik sağlar: 2 defa kriptografik şifreleme ve stenografik gizleme.\r\n");
            rtbWorking.SelectionColor = Color.LightSkyBlue;
            rtbWorking.AppendText("   • 🔍 Resmin içinde veri olduğu, gelişmiş analiz yöntemleri olmadan anlaşılamaz.\r\n");
            rtbWorking.SelectionColor = Color.LightSkyBlue;
            rtbWorking.AppendText("   • 🔒 Şifre olmadan, resimdeki veriyi çıkartmak mümkün olsa bile, şifrelenmiş verinin çözülmesi imkansızdır.");

            // Güvenlik tab içeriği
            SetFormattedText(rtbSecure, "🛡️ Güvenlik Önlemleri ve Tavsiyeler 🛡️", Color.Cyan, true, 14);
            rtbSecure.AppendText("\r\n\r\n");
            
            SetFormattedText(rtbSecure, "1. 🔑 Şifre Güvenliği:", Color.LightGreen, true, 12);
            rtbSecure.AppendText("\r\n");
            rtbSecure.SelectionColor = Color.LightYellow;
            rtbSecure.AppendText("   • 📏 En az 12 karakter uzunluğunda ve ardışık sayılar içermeyen şifreler kullanın.\r\n");
            rtbSecure.SelectionColor = Color.LightYellow;
            rtbSecure.AppendText("   • 🔣 Büyük/küçük harfler, rakamlar, özel karakterler içeren karmaşık şifreler oluşturun.\r\n");
            rtbSecure.SelectionColor = Color.LightYellow;
            rtbSecure.AppendText("   • ⚠️ Kişisel bilgilerinizi (doğum tarihi, isim vb.) kullanmaktan kaçının.\r\n");
            rtbSecure.SelectionColor = Color.LightYellow;
            rtbSecure.AppendText("   • 🔄 Her önemli veri için farklı şifreler kullanın.\r\n\r\n");
            
            SetFormattedText(rtbSecure, "2. 💻 Sistem Güvenliği:", Color.LightGreen, true, 12);
            rtbSecure.AppendText("\r\n");
            rtbSecure.SelectionColor = Color.LightYellow;
            rtbSecure.AppendText("   • 🔄 İşletim sisteminizi ve güvenlik yazılımlarınızı güncel tutun.\r\n");
            rtbSecure.SelectionColor = Color.LightYellow;
            rtbSecure.AppendText("   • 🔒 Şifrelenmiş resimleri güvenli ortamlarda tutun ve paylaşın.\r\n");
            rtbSecure.SelectionColor = Color.LightYellow;
            rtbSecure.AppendText("   • ✅ Şifreleme işlemlerini güvendiğiniz cihazlarda gerçekleştirin.\r\n");
            rtbSecure.SelectionColor = Color.LightYellow;
            rtbSecure.AppendText("   • 📵 Önemli verileri şifrelerken internet bağlantınızı kesin.\r\n\r\n");
            
            SetFormattedText(rtbSecure, "3. ⚠️ Dikkat Edilmesi Gerekenler:", Color.LightGreen, true, 12);
            rtbSecure.AppendText("\r\n");
            rtbSecure.SelectionColor = Color.LightYellow;
            rtbSecure.AppendText("   • 🚫 Şifrelerinizi unutursanız, verilerinize erişemezsiniz. Bu sistemde 'şifremi unuttum' seçeneği yoktur.\r\n");
            rtbSecure.SelectionColor = Color.LightYellow;
            rtbSecure.AppendText("   • 🖼️ Şifrelenmiş resimleri yeniden boyutlandırmak, düzenlemek içerisindeki verilerin bozulmasına neden olabilir.\r\n");
            rtbSecure.SelectionColor = Color.LightYellow;
            rtbSecure.AppendText("   • 💾 Şifrelenmiş resimlerinizi yedekleyin.\r\n");
            rtbSecure.SelectionColor = Color.LightYellow;
            rtbSecure.AppendText("   • 🚫 Bu sistemi yasadışı amaçlar için kullanmayın!");

            // Hakkında tab içeriği
            SetFormattedText(rtbAbout, "ℹ️ Resim Tabanlı Şifreleme Sistemi Hakkında ℹ️", Color.Cyan, true, 14);
            rtbAbout.AppendText("\r\n\r\n");
            
            SetFormattedText(rtbAbout, "🔢 Sürüm: 1.0.0", Color.LightGreen, true, 12);
            rtbAbout.AppendText("\r\n");
            rtbAbout.SelectionColor = Color.LightYellow;
            rtbAbout.AppendText("👨‍💻 Geliştirici: Meriç Zorlu\r\n");
            rtbAbout.SelectionColor = Color.LightYellow;
            rtbAbout.AppendText("©️ Lisans: Tüm hakları saklıdır\r\n\r\n");
            
            SetFormattedText(rtbAbout, "🔬 Bu uygulama, modern şifreleme ve stenografi teknikleri kullanılarak TÜBİTAK'a \"Kriptoloji ve Matematik\" konulu proje olarak sunmak üzere geliştirilmiştir.", Color.LightYellow, false, 10);
            rtbAbout.AppendText("\r\n\r\n");
            
            SetFormattedText(rtbAbout, "🧰 Kullanılan Teknolojiler:", Color.LightGreen, true, 12);
            rtbAbout.AppendText("\r\n");
            rtbAbout.SelectionColor = Color.LightYellow;
            rtbAbout.AppendText("• 🖥️ .NET Framework 4.8.1\r\n");
            rtbAbout.SelectionColor = Color.LightYellow;
            rtbAbout.AppendText("• 🔐 AES-256 Şifreleme\r\n");
            rtbAbout.SelectionColor = Color.LightYellow;
            rtbAbout.AppendText("• 🛡️ RSA-3072 Şifreleme\r\n");
            rtbAbout.SelectionColor = Color.LightYellow;
            rtbAbout.AppendText("• 🖼️ LSB Görüntü Gizleme\r\n");
            rtbAbout.SelectionColor = Color.LightYellow;
            rtbAbout.AppendText("• 🎨 GUNA UI Framework\r\n\r\n");
            
            SetFormattedText(rtbAbout, "📧 İletişim:", Color.LightGreen, true, 12);
            rtbAbout.AppendText("\r\n");
            rtbAbout.SelectionColor = Color.LightYellow;
            rtbAbout.AppendText("✉️ E-Posta: mericzorlu@gmail.com\r\n");
            rtbAbout.SelectionColor = Color.LightYellow;
            rtbAbout.AppendText("🌐 Kaynak: https://github.com/PandaMrc/ImageBasedEncryptionSystem-TUBITAK-2025-Nisan\r\n\r\n");
            
            SetFormattedText(rtbAbout, "⚠️ NOT: Bu uygulama eğitim amaçlı ve TÜBİTAK projesi olarak geliştirilmiştir. Yasadışı faaliyetlerde kullanılması yasaktır. Kullanılması halinde bir sorumluluk kabul etmemekteyiz.", Color.LightYellow, false, 10);
            rtbAbout.AppendText("\r\n\r\n");
            
            SetFormattedText(rtbAbout, "👥 Proje Ekibi:", Color.LightGreen, true, 12);
            rtbAbout.AppendText("\r\n");
            rtbAbout.SelectionColor = Color.LightYellow;
            rtbAbout.AppendText("👨‍💼 Proje Lideri: Yusuf KAÇAR\r\n");
            rtbAbout.SelectionColor = Color.LightYellow;
            rtbAbout.AppendText("👨‍💻 Geliştirici: Meriç ZORLU\r\n");
            rtbAbout.SelectionColor = Color.LightYellow;
            rtbAbout.AppendText("👥 Sunum ve Detaylar: Ayberk GEZGİN, Yıldız KUNAK, Dilay Z. METİN");

            // DevMode tab içeriği
            SetFormattedText(rtbDevMode, "🛠️ Geliştirici Modu 🛠️", Color.Cyan, true, 14);
            rtbDevMode.AppendText("\r\n\r\n");
            
            SetFormattedText(rtbDevMode, "❓ Geliştirici Modu Nedir? Ne İşe Yarar?", Color.LightGreen, true, 12);
            rtbDevMode.AppendText("\r\n");
            rtbDevMode.SelectionColor = Color.White;
            rtbDevMode.AppendText("• 👨‍💻 Admin paneli ve Analiz sayfasına erişmenizi sağlar.\r\n");
            rtbDevMode.SelectionColor = Color.White;
            rtbDevMode.AppendText("• 🔘 Menü sayfasının üst kısmında yer alan 'Geliştirici Modu' özelliğini açıp kapatmaya yarayan bir buton bulunur.\r\n");
            rtbDevMode.SelectionColor = Color.White;
            rtbDevMode.AppendText("• ✅ Geliştirici Modu aktif edildiği zaman Admin Panel ve Analiz ekranına giriş yapabilirsiniz.\r\n\r\n");
            
            SetFormattedText(rtbDevMode, "❓ Nasıl Aktif Edilir:", Color.LightGreen, true, 12);
            rtbDevMode.AppendText("\r\n");
            rtbDevMode.SelectionColor = Color.White;
            rtbDevMode.AppendText("• 🖱️ Menü sayfasında sol üst köşede bulunan uygulama logosuna tıklayarak bir giriş ekranı açılır.\r\n");
            rtbDevMode.SelectionColor = Color.White;
            rtbDevMode.AppendText("• 🔑 Açılan sayfada 'Geliştirici Kimliği' ve 'Parola' girilerek giriş yapılır (varsayılan hesap bilgileri aşağıda).\r\n");
            rtbDevMode.SelectionColor = Color.White;
            rtbDevMode.AppendText("• ✅ Bilgileri doğru girdiğiniz zaman Geliştirici girişi yapmış olursunuz ve bu özelliği artık istediğiniz zaman menü sayfasından açıp kapatabilirsiniz.\r\n\r\n");
            
            SetFormattedText(rtbDevMode, "🔑 Varsayılan Giriş Bilgileri:", Color.LightGreen, true, 12);
            rtbDevMode.AppendText("\r\n");
            rtbDevMode.SelectionColor = Color.White;
            rtbDevMode.AppendText("• 👤 Geliştirici Kimliği: admin\r\n");
            rtbDevMode.SelectionColor = Color.White;
            rtbDevMode.AppendText("• 🔒 Parola: 12345\r\n");
            rtbDevMode.SelectionColor = Color.White;
            rtbDevMode.AppendText("• 👤 Geliştirici Kimliği: developer\r\n");
            rtbDevMode.SelectionColor = Color.White;
            rtbDevMode.AppendText("• 🔒 Parola: dev123\r\n\r\n");
            
            SetFormattedText(rtbDevMode, "⚠️ NOT: Kimlik bilgileri henüz değiştirilebilir veya yeni kullanıcı eklenebilir değil. İlerleyen güncellemelerde eklenmesi planlanıyor.", Color.LightYellow, false, 10);

            // Admin tab içeriği
            SetFormattedText(rtbAdmin, "👨‍💻 Admin Paneli İşlemleri 👨‍💻", Color.Cyan, true, 14);
            rtbAdmin.AppendText("\r\n\r\n");
            
            SetFormattedText(rtbAdmin, "❓ Admin Paneli Nedir? Ne İşe Yarar?", Color.LightGreen, true, 12);
            rtbAdmin.AppendText("\r\n");
            rtbAdmin.SelectionColor = Color.White;
            rtbAdmin.AppendText("• 🛠️ Admin paneli, sistem yöneticilerinin uygulamanın genel ayarlarını yönetmelerine olanak tanır.\r\n");
            rtbAdmin.SelectionColor = Color.White;
            rtbAdmin.AppendText("• 🔄 Kullanıcıların şifreleme ve şifre çözme işlemlerinin yapılmasını sağlayan sistem kimliğini değiştirerek RSA anahtarlarının değiştirilebilmesini sağlar.\r\n\r\n");
            
            SetFormattedText(rtbAdmin, "✨ Admin Paneli Özellikleri:", Color.LightGreen, true, 12);
            rtbAdmin.AppendText("\r\n");
            rtbAdmin.SelectionColor = Color.White;
            rtbAdmin.AppendText("• 🔑 Sistem Kimliği: Mevcut sistem kimliğini görebilir, değiştirebilir ve sıfırlayabilirsiniz.\r\n");
            rtbAdmin.SelectionColor = Color.White;
            rtbAdmin.AppendText("• 🔐 RSA Anahtarları: Sistem kimliklerine özel olarak oluşturulan RSA anahtarlarını görebilirsiniz.\r\n\r\n");
            
            SetFormattedText(rtbAdmin, "📝 Admin Paneli Kullanımı:", Color.LightGreen, true, 12);
            rtbAdmin.AppendText("\r\n");
            rtbAdmin.SelectionColor = Color.White;
            rtbAdmin.AppendText("• 🔑 Admin paneline giriş yapmak için yönetici kimlik bilgilerinizi kullanın.\r\n");
            rtbAdmin.SelectionColor = Color.White;
            rtbAdmin.AppendText("• ✅ Giriş yaptıktan sonra Geliştirici Modunu aktif ederek Admin paneline girin.\r\n");
            rtbAdmin.SelectionColor = Color.White;
            rtbAdmin.AppendText("• 📝 Admin panelini açtıktan sonra 'Yeni Sistem Kimliği' bölümüne istediğiniz sistem kimliğini yazıp 'Kaydet' butonuna tıklayabilirsiniz.\r\n");
            rtbAdmin.SelectionColor = Color.White;
            rtbAdmin.AppendText("• ⚠️ Her sistem kimliğine özel RSA anahtarları kullanılır! Aynı sistem kimliği ile şifrelenmemiş verileri açmanız imkansızdır.\r\n");
            rtbAdmin.SelectionColor = Color.White;
            rtbAdmin.AppendText("• 🚨 Sistem kimliğinizi değiştirirseniz unutmamanız çok önemlidir. Unutursanız geri dönüşü olmaz!\r\n\r\n");
            
            SetFormattedText(rtbAdmin, "💡 Admin Paneli İpuçları:", Color.LightGreen, true, 12);
            rtbAdmin.AppendText("\r\n");
            rtbAdmin.SelectionColor = Color.White;
            rtbAdmin.AppendText("• ⚠️ Admin panelini kullanırken dikkatli olun. Yanlış sistem kimliği ile veri çözmeye çalışırsanız hata alırsınız.\r\n\r\n");
            
            SetFormattedText(rtbAdmin, "🚨 ÖNEMLİ NOTLAR:", Color.LightGreen, true, 12);
            rtbAdmin.AppendText("\r\n");
            rtbAdmin.SelectionColor = Color.LightYellow;
            rtbAdmin.AppendText("• 🚧 Admin Paneli daha geliştirme aşamasındadır.\r\n");
            rtbAdmin.SelectionColor = Color.LightYellow;
            rtbAdmin.AppendText("• ✅ Sadece sorunsuz çalışan özellikler belirtilmektedir ve kullanılabilmektedir.\r\n");
            rtbAdmin.SelectionColor = Color.LightYellow;
            rtbAdmin.AppendText("• 🔄 Yeni güncellemelerde Admin Paneline daha yeni özellikler eklenecektir ve geliştirilecektir.\r\n");
            rtbAdmin.SelectionColor = Color.LightYellow;
            rtbAdmin.AppendText("• ⚠️ Sistem Kimliğini değiştirirken dikkatli olun! Eğer unutursanız geri dönüşü olmaz. Unuttuğunuz kimlikle şifrelenen verileri KURTARAMAZSINIZ!");

            // Analiz tab içeriği
            SetFormattedText(rtbAnalysis, "📊 Güvenlik Analizi ve Performans 📊", Color.Cyan, true, 14);
            rtbAnalysis.AppendText("\r\n\r\n");
            
            SetFormattedText(rtbAnalysis, "1. 🔍 Steganaliz İşlemleri:", Color.LightGreen, true, 12);
            rtbAnalysis.AppendText("\r\n");
            rtbAnalysis.SelectionColor = Color.White;
            rtbAnalysis.AppendText("   • 🖼️ İçinde veri gizli olan resim ile orijinal resmin karşılaştırılması yapılır.\r\n");
            rtbAnalysis.SelectionColor = Color.White;
            rtbAnalysis.AppendText("   • 👁️ Göz ile görülemeyen farkları kullanıcıya sunar.\r\n");
            rtbAnalysis.SelectionColor = Color.White;
            rtbAnalysis.AppendText("   • 📋 Çeşitli yöntemler kullanılarak şifreli resim analiz edilir ve kapsamlı analiz raporu hazırlanır.\r\n\r\n");
            
            SetFormattedText(rtbAnalysis, "2. 🧪 Kullanılan Analiz Yöntemleri:", Color.LightGreen, true, 12);
            rtbAnalysis.AppendText("\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 📊 RS (Regular-Singular) Analizi: Görüntü istatistiklerindeki anormallikleri tespit eder ve tahmini veri gizleme oranını hesaplar.\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 📈 SPA (Sample Pair Analysis): Piksel çiftleri arasındaki ilişkileri inceleyerek gizli veri varlığını tespit eder.\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 🗺️ Değişim Haritası: Orijinal ve şifreli görüntü arasındaki farklılıkları görselleştirir, piksel düzeyinde renklendirerek gösterir.\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 🧩 Bit Düzlemi Analizi: Her renk kanalının (R, G, B) bit düzlemlerini ayrı ayrı inceleyerek gizli verinin izlerini tespit eder.\r\n\r\n");
            
            SetFormattedText(rtbAnalysis, "3. 📋 Analiz Sonuçlarını Yorumlama:", Color.LightGreen, true, 12);
            rtbAnalysis.AppendText("\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 📏 Tahmini Gömme Oranı: 0.5'e yakın değerler (%50) rastgele gömme, 0'a yakın değerler düşük veri yoğunluğunu gösterir.\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 📉 RS Analiz Grafiği: Düzenli (Regular) ve Tekil (Singular) grup sayılarının yakınlığı steganografi kullanıldığını gösterir.\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 🔍 SPA Analiz Sonuçları: 0.5'e yakın değerler şüpheli, 0'a yakın değerler temiz görüntüyü gösterir.\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 🔢 Değiştirilen Piksel Sayısı: Yüksek sayılar, büyük miktarda veri gizlendiğini gösterir.\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 📊 PSNR Değeri: 30dB altındaki değerler gözle görülebilir değişiklikleri, 50dB üstü değerler çok küçük değişimleri gösterir.\r\n\r\n");
            
            SetFormattedText(rtbAnalysis, "4. ⚡ Performans Analizi:", Color.LightGreen, true, 12);
            rtbAnalysis.AppendText("\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • ⏱️ Şifreleme Hızı: Dosya boyutuna ve resim çözünürlüğüne bağlı olarak saniyeler içinde gerçekleşir.\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 📊 Resim Kapasitesi: Bir resmin gizleyebileceği veri miktarı, resmin piksel sayısına bağlıdır.\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("     📈 Örneğin: 1024x768 piksel bir resim, yaklaşık 300KB veri gizleyebilir.\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 🎨 Çıktı Kalitesi: İşlem sonrası resimde gözle görülür kalite kaybı olmaz.\r\n\r\n");
            
            SetFormattedText(rtbAnalysis, "5. 🔄 Analiz İşlemlerini Başlatma:", Color.LightGreen, true, 12);
            rtbAnalysis.AppendText("\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 🖱️ Ana ekrandaki 'Analiz' butonuna tıklayın (Geliştirici modu etkin olmalıdır).\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 📁 Analiz edilecek şifreli görüntüyü seçin.\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 📂 İsterseniz, karşılaştırma için orijinal görüntüyü de seçin.\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • ⚙️ Analiz parametrelerini ayarlayın (renk kanalı, grup boyutu vb.).\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • ▶️ 'Analiz Başlat' butonuna tıklayarak işlemi başlatın.\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 📊 Analiz raporunu inceleyerek görüntü hakkında bilgi edinin.\r\n\r\n");
            
            SetFormattedText(rtbAnalysis, "6. ⚖️ Güvenlik/Performans Dengesi:", Color.LightGreen, true, 12);
            rtbAnalysis.AppendText("\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 🛡️ Sistem, yüksek güvenlik sağlarken makul performans sunar.\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 📝 Büyük metinler için daha büyük resimler gerekebilir.\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 🔒 Maksimum güvenlik için, taşıyıcı resmin karmaşık doku ve renklere sahip, sıradan ve dikkat çekmeyen bir görsel olması önerilir.\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 📸 JPEG formatı gibi kayıplı sıkıştırma formatları yerine PNG gibi kayıpsız formatların kullanılması gereklidir.");

            // TÜBİTAK tab içeriği
            SetFormattedText(rtbTubitak, "🔬 TÜBİTAK Projesi 🔬", Color.Cyan, true, 14);
            rtbTubitak.AppendText("\r\n\r\n");
            
            SetFormattedText(rtbTubitak, "1. 📋 Proje Konusu:", Color.LightGreen, true, 12);
            rtbTubitak.AppendText("\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🏫 Bu proje TÜBİTAK'a sunmak üzere İzmir KANUNİ SULTAN SÜLEYMAN ANADOLU LİSESİ 12/A öğrencileri tarafından hazırlanmıştır.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 📚 TÜBİTAK konu başlığımız 'Kriptoloji ve Matematiğin İlişkisi'dir.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🧮 Bu projede kullanılan şifreleme yöntemleri ve veri gizleme yöntemi ağırlıklı olarak zor matematiksel formüller kullanır.\r\n\r\n");

            SetFormattedText(rtbTubitak, "2. ❓ Kriptoloji Nedir?", Color.LightGreen, true, 12);
            rtbTubitak.AppendText("\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🔐 Kriptoloji, bilgilerin güvenli bir şekilde iletilmesi ve saklanması için şifreleme ve şifre çözme tekniklerini içeren bilim dalıdır.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 📚 İki ana alt dalı vardır: Kriptografi (şifreleme bilimi) ve Kriptoanaliz (şifre çözme/analiz bilimi).\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🏛️ Tarihsel olarak askeri haberleşmeden başlayıp, günümüzde internet güvenliği, elektronik ticaret ve kişisel mahremiyet için kritik öneme sahiptir.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🛡️ Veri bütünlüğü, kimlik doğrulama, inkâr edilemezlik ve gizlilik gibi güvenlik hedeflerini sağlar.\r\n\r\n");

            SetFormattedText(rtbTubitak, "3. ❓ Steganografi Nedir?", Color.LightGreen, true, 12);
            rtbTubitak.AppendText("\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🙈 Steganografi, gizli bilgileri başka bir veri içerisine (genellikle görsel, ses veya video dosyaları) göze çarpmayacak şekilde gizleme sanatıdır.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🧩 Şifrelemeden farklı olarak, mesajın varlığını bile gizleyerek daha derin bir güvenlik katmanı sağlar.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🖼️ Dijital steganografi, görüntülerin piksel değerlerindeki küçük değişiklikler gibi insan duyularıyla algılanamayan yöntemler kullanır.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🔍 Steganaliz, steganografi ile gizlenmiş verileri tespit etme ve çıkarma amacı taşıyan karşı tekniklerdir.\r\n\r\n");

            SetFormattedText(rtbTubitak, "4. ❓ RSA Şifreleme Nedir?", Color.LightGreen, true, 12);
            rtbTubitak.AppendText("\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 👨‍🔬 RSA, 1977'de Ron Rivest, Adi Shamir ve Leonard Adleman tarafından geliştirilen, asimetrik şifreleme algoritmasıdır.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🔑 Bir çift anahtar kullanır: Herkese açık olan 'public key' ve gizli tutulan 'private key'.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🧮 Büyük asal sayıların çarpımının faktörizasyonunun zorluğu ilkesine dayanır.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 📝 Dijital imzalar, anahtar değişimi ve güvenli veri iletimi için kullanılır.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • ⚙️ İşlem yükü yüksek olduğundan, genellikle küçük veriler (örn. anahtar) şifrelemek için kullanılır.\r\n\r\n");

            SetFormattedText(rtbTubitak, "5. ❓ AES Şifreleme Nedir?", Color.LightGreen, true, 12);
            rtbTubitak.AppendText("\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🔄 AES (Advanced Encryption Standard), blok şifreleme yöntemi kullanan simetrik bir şifreleme algoritmasıdır.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🏆 2001 yılında ABD hükümeti tarafından standart olarak kabul edilmiştir ve dünya çapında yaygın kullanılır.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🔢 128, 192 veya 256 bit anahtar uzunlukları destekler. Projemizde en güvenli olan AES-256 kullanılmıştır.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🔄 SubBytes, ShiftRows, MixColumns ve AddRoundKey adımlarından oluşan dönüşüm turları kullanır.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • ⚡ Hem yazılım hem donanım uygulamalarında etkili performans gösterir, günümüzde bilinen pratik saldırılara karşı dayanıklıdır.\r\n\r\n");

            SetFormattedText(rtbTubitak, "6. ❓ LSB (Least Significant Bit) Nedir?", Color.LightGreen, true, 12);
            rtbTubitak.AppendText("\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 0️⃣1️⃣ LSB, dijital verilerde her baytın en sağdaki (en az önemli) bitidir ve değiştirildiğinde verinin görsel/işitsel kalitesinde en az bozulmaya neden olur.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🖼️ Görüntü steganografisinde, piksellerin renk değerlerinin LSB'leri değiştirilerek veri gizlenir.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 📊 Örneğin, RGB renk formatında her piksel için 3 bayt (R, G, B) kullanılır, her piksel potansiyel olarak 3 bit bilgi saklayabilir.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🧪 Projemizdeki optimizasyonlar, bit değişimlerinin desenini daha rastgele hale getirerek tespit edilmesini zorlaştırır.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • ⚠️ JPEG gibi kayıplı sıkıştırma formatları LSB değişikliklerini yok edebilir, bu yüzden PNG gibi kayıpsız formatlar tercih edilmelidir.\r\n\r\n");

            SetFormattedText(rtbTubitak, "7. 🔢 Kriptoloji ve Matematik İlişkisi:", Color.LightGreen, true, 12);
            rtbTubitak.AppendText("\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🧮 Matematik, kriptolojinin temelidir. Güvenli şifreleme sistemleri matematiksel zorluklara dayanır.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🔢 Sayı teorisi: RSA'da büyük asal sayıların çarpımı ve faktörizasyon zorluğu kullanılır.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 📊 Cebir: AES'te Galois alanları ve matris işlemleri kullanılır.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 📉 Olasılık teorisi: Şifreleme algoritmalarının gücü ve kırılabilirliği analiz edilir.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 📈 İstatistik: Kriptoanaliz teknikleri, mesaj örüntülerini tespit etmek için istatistiksel analizler kullanır.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🔍 Karmaşıklık teorisi: Şifreleme algoritmalarının hesaplama zorluğunu değerlendirmek için kullanılır.\r\n\r\n");

            SetFormattedText(rtbTubitak, "8. 🔢 Kullanılan Matematiksel Kavramlar:", Color.LightGreen, true, 12);
            rtbTubitak.AppendText("\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            SetFormattedText(rtbTubitak, "   - 🔒 AES Şifrelemesinde Kullanılan Matematiksel Kavramlar:", Color.Aqua, true, 10);
            rtbTubitak.AppendText("\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🧪 Galois Alanı - GF(2⁸): Sonlu cisim matematiği, 8-bitlik bloklarla işlem yapar\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 📐 Polinom Aritmetiği: Baytların polinom olarak temsil edilip işlenmesi\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • ✖️ Modüler Çarpım: Galois alanında çarpma işlemleri\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 📊 Matris Transformasyonları: ShiftRows ve MixColumns işlemlerinde kullanılır\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🔄 Doğrusal Dönüşümler: Veri karıştırma ve yayma işlemleri\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • ⚡ XOR İşlemi: Bit düzeyinde 'dışlayıcı veya' işlemi\r\n\r\n");

            SetFormattedText(rtbTubitak, "   - 🔐 RSA Şifrelemesinde Kullanılan Matematiksel Kavramlar:", Color.Aqua, true, 10);
            rtbTubitak.AppendText("\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🔢 Asal Sayılar: Sadece kendisi ve 1'e bölünebilen sayılar\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🧮 Modüler Aritmetik: Bir sayının bölümünden kalan üzerinde yapılan işlemler\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 📊 Euler'in Totient Fonksiyonu φ(n): n ile aralarında asal olan n'den küçük pozitif tam sayıların sayısı\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🔄 Modüler Ters: a·a⁻¹ ≡ 1 (mod n) olacak şekildeki a⁻¹ değeri\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 📈 Modüler Üs Alma: büyük sayıların modüler üssünü etkili şekilde hesaplama\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🧩 Faktorizasyon Zorluğu: büyük sayıları asal çarpanlarına ayırmanın hesaplama zorluğu\r\n\r\n");

            SetFormattedText(rtbTubitak, "   - 🖼️ LSB Yönteminde Kullanılan Matematiksel Kavramlar:", Color.Aqua, true, 10);
            rtbTubitak.AppendText("\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 📊 Dizi ve Matris İşlemleri: Görüntülerin 2D matris olarak işlenmesi\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 0️⃣1️⃣ İkili Sayı Sistemi: Bit manipülasyonu ve bit maskeleme işlemleri\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🔍 Bit Düzlemi Ayrışımı: Görüntünün bit düzlemlerine ayrılarak analiz edilmesi\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 📐 İstatistiksel Analizler: Piksel değerlerinin dağılımının incelenmesi\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            SetFormattedText(rtbTubitak, "   • 🔢 Kullanılan Matematiksel Kavramlar:\n", Color.Aqua, true, 10);
            rtbTubitak.AppendText("\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🧪 Galois Alanı - GF(2⁸): Sonlu cisim matematiği, 8-bitlik bloklarla işlem yapar\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 📐 Polinom Aritmetiği: Baytların polinom olarak temsil edilip işlenmesi\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • ✖️ Modüler Çarpım: Galois alanında çarpma işlemleri\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 📊 Matris Transformasyonları: ShiftRows ve MixColumns işlemlerinde kullanılır\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🔄 Doğrusal Dönüşümler: Veri karıştırma ve yayma işlemleri\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • ⚡ XOR İşlemi: Bit düzeyinde 'dışlayıcı veya' işlemi\r\n\r\n");

            SetFormattedText(rtbTubitak, "9. 🚀 Projemizin Kriptoloji Alanına Katkıları:", Color.LightGreen, true, 12);
            rtbTubitak.AppendText("\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🔒 Çok katmanlı şifreleme yaklaşımı ile güvenlik seviyesinin artırılması\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🧪 Optimizasyonlar ile LSB steganografisinin tespit edilebilirliğinin azaltılması\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 📊 Kapsamlı steganaliz araçları ile steganografi tekniklerinin etkinliğinin ölçülmesi\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 👤 Kullanıcı dostu arayüz tasarımı ile kriptoloji ve steganografi yöntemlerinin erişilebilirliğinin artırılması\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🧩 Farklı güvenlik tekniklerinin (AES, RSA, LSB) entegrasyonu ile daha güçlü bir güvenlik çözümü oluşturulması\r\n\r\n");

            SetFormattedText(rtbTubitak, "10. 🔮 Kriptolojinin Geleceği:", Color.LightGreen, true, 12);
            rtbTubitak.AppendText("\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🧮 Kuantum Bilgisayarlar: Mevcut kriptografik sistemlere tehdit oluşturabilir (özellikle RSA) ve yeni post-kuantum kriptografi yöntemleri geliştirilmektedir.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🌐 Blockchain: Dağıtık yapıda, kriptografik olarak güvenli veri saklama teknolojileri yaygınlaşmaktadır.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🤖 Yapay Zeka: Hem kriptografik sistemleri güçlendirmek hem de kriptoanaliz için kullanımı artmaktadır.\r\n");
            rtbTubitak.SelectionColor = Color.LightYellow;
            rtbTubitak.AppendText("   • 🔐 Homomorfik Şifreleme: Şifreli veriler üzerinde işlem yapabilme imkanı sağlayan yöntemler geliştirilmektedir.\r\n");
        }

        // Başlıkları formatlamak için yardımcı metot
        private void SetFormattedText(RichTextBox rtb, string text, Color color, bool isBold, float size)
        {
            // Mevcut stilleri saklayın
            Font originalFont = rtb.SelectionFont;
            Color originalColor = rtb.SelectionColor;
            
            // Yeni stil uygulayın
            FontStyle style = isBold ? FontStyle.Bold : FontStyle.Regular;
            rtb.SelectionFont = new Font(rtb.Font.FontFamily, size, style);
            rtb.SelectionColor = color;
            
            // Metni ekleyin
            rtb.AppendText(text);
            
            // Stilleri sıfırlayın (ya da diğer yazılacak metin için varsayılan beyaz renk ayarlayın)
            rtb.SelectionFont = originalFont;
            rtb.SelectionColor = originalColor;
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {

        }

        private void FrmInfo_FormClosing(object sender, FormClosingEventArgs e)
        {

        }



    }
}
