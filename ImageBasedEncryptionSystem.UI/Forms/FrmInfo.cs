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
            
            SetFormattedText(rtbAnalysis, "1. 🔍 Karşılaştırma:", Color.LightGreen, true, 12);
            rtbAnalysis.AppendText("\r\n");
            rtbAnalysis.SelectionColor = Color.White;
            rtbAnalysis.AppendText("   • 🖼️ İçinde veri gizli olan resim ile orijinal resmin karşılaştırılması yapılır.\r\n");
            rtbAnalysis.SelectionColor = Color.White;
            rtbAnalysis.AppendText("   • 👁️ Göz ile görülemeyen farkları kullanıcıya sunar.\r\n");
            rtbAnalysis.SelectionColor = Color.White;
            rtbAnalysis.AppendText("   • 📋 Çeşitli yöntemler kullanılarak şifreli resim analiz edilir ve analiz raporu hazırlanır.\r\n");
            rtbAnalysis.SelectionColor = Color.White;
            rtbAnalysis.AppendText("   ⚠️ NOT: Bu sistem henüz tamamlanmamıştır. Gelecekte eklenecek bazı özellikler belirtilmiştir.\r\n\r\n");
            
            SetFormattedText(rtbAnalysis, "2. ⚡ Performans Analizi:", Color.LightGreen, true, 12);
            rtbAnalysis.AppendText("\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • ⏱️ Şifreleme Hızı: Dosya boyutuna ve resim çözünürlüğüne bağlı olarak saniyeler içinde gerçekleşir.\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 📊 Resim Kapasitesi: Bir resmin gizleyebileceği veri miktarı, resmin piksel sayısına bağlıdır.\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("     📈 Örneğin: 1024x768 piksel bir resim, yaklaşık 300KB veri gizleyebilir.\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 🎨 Çıktı Kalitesi: İşlem sonrası resimde gözle görülür kalite kaybı olmaz.\r\n\r\n");
            
            SetFormattedText(rtbAnalysis, "3. ⚖️ Güvenlik/Performans Dengesi:", Color.LightGreen, true, 12);
            rtbAnalysis.AppendText("\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 🛡️ Sistem, yüksek güvenlik sağlarken makul performans sunar.\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 📝 Büyük metinler için daha büyük resimler gerekebilir.\r\n");
            rtbAnalysis.SelectionColor = Color.LightYellow;
            rtbAnalysis.AppendText("   • 🔒 Maksimum güvenlik için, taşıyıcı resmin sıradan ve dikkat çekmeyen bir görsel olması önerilir.");

            // TÜBİTAK tab içeriği
            SetFormattedText(rtbTubitak, "🔬 TÜBİTAK Projesi 🔬", Color.Cyan, true, 14);
            rtbTubitak.AppendText("\r\n\r\n");
            
            SetFormattedText(rtbTubitak, "1. 📋 Proje Konusu:", Color.LightGreen, true, 12);
            rtbTubitak.AppendText("\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 🏫 Bu proje TÜBİTAK'a sunmak üzere İzmir KANUNİ SULTAN SÜLEYMAN ANADOLU LİSESİ 12/A öğrencileri tarafından hazırlanmıştır.\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 📚 TÜBİTAK konu başlığımız 'Kriptoloji ve Matematiğin İlişkisi'dir.\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 🧮 Bu projede kullanılan şifreleme yöntemleri ve veri gizleme yöntemi ağırlıklı olarak zor matematiksel formüller kullanır.\r\n\r\n");

            SetFormattedText(rtbTubitak, "2. 🔢 Kullanılan Matematiksel Kavramlar:", Color.LightGreen, true, 12);
            rtbTubitak.AppendText("\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            SetFormattedText(rtbTubitak, "   - 🔒 AES Şifrelemesinde Kullanılan Matematiksel Kavramlar:", Color.Aqua, true, 10);
            rtbTubitak.AppendText("\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 🧪 Galois Alanı - GF(2⁸)\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 📐 Polinom Aritmetiği\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • ✖️ Modüler Çarpım\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 📊 Matris Çarpımı\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 🔄 Affine Dönüşümü (Doğrusal Cebir)\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • ⚡ Bit düzeyinde XOR işlemi\r\n\r\n");

            SetFormattedText(rtbTubitak, "   - 🔐 RSA Şifrelemesinde Kullanılan Matematiksel Kavramlar:", Color.Aqua, true, 10);
            rtbTubitak.AppendText("\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 🔢 Asal Sayılar\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 🧮 Modüler Aritmetik\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 📊 Euler'in Totient Fonksiyonu\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 🔄 Modüler Ters Eleman\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 📈 Üstel Mod Hesaplamaları\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 🧩 Sayı Teorisi ve Faktörleme Zorluğu\r\n\r\n");

            SetFormattedText(rtbTubitak, "   - 🖼️ LSB Yönteminde Kullanılan Matematiksel Kavramlar:", Color.Aqua, true, 10);
            rtbTubitak.AppendText("\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 📊 Dizi ve Matris İşlemleri\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 0️⃣1️⃣ İkili Sayı Sistemi\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 🧮 Modüler Aritmetik\r\n\r\n");

            SetFormattedText(rtbTubitak, "   ℹ️ NOT: Bunlar şifreleme ve veri gizleme algoritmalarımızda kullanılan bazı matematiksel kavramlardır. Çok daha fazla kavram mevcuttur, bunlar sadece önemli örneklerdir.", Color.LightYellow, false, 10);
            rtbTubitak.AppendText("\r\n\r\n");

            SetFormattedText(rtbTubitak, "3. 🤔 Neden Bu Projeyi Yaptık:", Color.LightGreen, true, 12);
            rtbTubitak.AppendText("\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 🔒 Bu projeyi yapmamızın amacı, günümüzde veri güvenliğinin önemini vurgulamak ve bu alanda yenilikçi bir çözüm sunmaktır.\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 🛡️ Projemiz, veri güvenliğini artırmak ve kullanıcıların gizliliğini korumak amacıyla tasarlanmıştır.\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 🔐 Projemiz, şifreleme ve veri gizleme yöntemlerini etkin bir şekilde kullanarak Kriptoloji ve Stenografi başlıklarını ön planda tutmaktadır.\r\n\r\n");

            SetFormattedText(rtbTubitak, "4. 🌐 Kullanım Alanları:", Color.LightGreen, true, 12);
            rtbTubitak.AppendText("\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 👤 Bu sistem, kişisel verilerin korunması, gizli iletişim ve güvenli veri saklama gibi alanlarda kullanılabilir.\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 🏢 Özellikle askeri, finansal ve sağlık sektörlerinde veri güvenliğini sağlamak için etkili bir araçtır.\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 🔒 Ayrıca, kişisel gizliliği korumak isteyen bireyler için de faydalı bir çözüm sunmaktadır.\r\n\r\n");

            SetFormattedText(rtbTubitak, "5. 🚀 Gelecek Planları:", Color.LightGreen, true, 12);
            rtbTubitak.AppendText("\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 🔄 Projemizi daha da geliştirmek ve yeni özellikler eklemek için çalışmalara devam edeceğiz.\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 👂 Kullanıcı geri bildirimlerini dikkate alarak sistemimizi sürekli olarak güncellemeyi planlıyoruz.\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 🌐 Gelecekte, sistemimizi daha geniş bir kullanıcı kitlesine ulaştırmak için çeşitli platformlarda yayınlamayı hedefliyoruz.\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 🔒 Sistemimizin güvenliğini artırmak için yeni şifreleme algoritmaları ve veri gizleme yöntemleri üzerinde çalışmayı planlıyoruz.\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 🎨 Kullanıcı deneyimini iyileştirmek için arayüz tasarımını ve kullanım kolaylığını artırmayı hedefliyoruz.\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 📚 Projemizi daha geniş bir kitleye ulaştırmak için eğitim materyalleri ve belgeler hazırlamayı planlıyoruz.\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 📱 Sosyal medya ve diğer platformlarda tanıtım yapmayı planlıyoruz.\r\n");
            rtbTubitak.SelectionColor = Color.LightCoral;
            rtbTubitak.AppendText("   • 🎬 Kullanıcıların sistemimizi daha iyi anlamalarına yardımcı olmak için eğitim videoları ve kılavuzlar oluşturmayı hedefliyoruz.");
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
