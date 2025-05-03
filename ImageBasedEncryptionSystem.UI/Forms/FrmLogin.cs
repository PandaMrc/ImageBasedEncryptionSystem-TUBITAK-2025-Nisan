using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using ImageBasedEncryptionSystem.TypeLayer;
using ImageBasedEncryptionSystem.BusinessLayer;

namespace ImageBasedEncryptionSystem.UI.Forms
{
    public partial class FrmLogin : Form
    {
        // Ana menüye referans
        private FrmMenu parentForm;
        
        // Geliştirici modu nesnesi
        private Cls_DeveloperMode devMode;
        
        // Title bar için sürükleme işlevselliği
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        
        
        /// <summary>
        /// Varsayılan kurucu
        /// </summary>
        public FrmLogin()
        {
            Console.WriteLine(Debug.DEBUG_LOGIN_FORM_INITIALIZE_STARTED);
            InitializeComponent();
            
            // FormClosing olayını ekle
            this.FormClosing += FrmLogin_FormClosing;
            Console.WriteLine(Debug.DEBUG_LOGIN_FORM_CLOSING_EVENT_BOUND);
            
            // Geliştirici modu nesnesini başlat
            devMode = new Cls_DeveloperMode();
            Console.WriteLine(Debug.DEBUG_DEV_MODE_OBJECT_INITIALIZED);
        }
        
        /// <summary>
        /// Ana menü referansı ile kurucu
        /// </summary>
        /// <param name="parentForm">Ana menü formu</param>
        public FrmLogin(FrmMenu parentForm) : this()
        {
            this.parentForm = parentForm;
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine(Debug.DEBUG_LOGIN_FORM_LOADED);
                // Form ayarları
                this.TopMost = false; // Ana menünün arkasına gitmemesi için false yapıldı
                Console.WriteLine(Debug.DEBUG_FORM_SETTINGS_APPLIED);
                
                // Form başlama konumunu ayarla - eğer ana menü varsa onun ortasında aç
                if (parentForm != null)
                {
                    this.StartPosition = FormStartPosition.CenterParent;
                    Console.WriteLine(Debug.DEBUG_LOGIN_FORM_POSITION_SET);
                }
                
                // Modern arka planı ayarla
                BusinessLayer.UI.Cls_Background.Instance.CreateModernBackground(this);
                Console.WriteLine(Debug.DEBUG_LOGIN_FORM_BACKGROUND_CREATED);
                
                // Başarı mesajı yazdır
                Console.WriteLine(Success.LOGIN_FORM_SUCCESS);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Debug.DEBUG_SYSTEM_IDENTITY_RECEIVED, ex.Message));
                MessageBox.Show(string.Format(Errors.ERROR_LOGIN_BACKGROUND, ex.Message), "Hata", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Enter tuşuna basıldığında giriş yap
            if (e.KeyChar == (char)Keys.Enter)
            {
                Console.WriteLine(Debug.DEBUG_LOGIN_ENTER_KEY_PRESSED);
                e.Handled = true; // Tuş olayını işlenmiş olarak işaretle
                btnLogin_Click(sender, e); // Giriş butonuna tıklama olayını çağır
            }
        }
        
        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine(Debug.DEBUG_LOGIN_PROCESS_STARTED);
                // Giriş bilgilerini al
                string devId = txtDeveloperId.Text.Trim();
                string password = txtPassword.Text.Trim();
                
                Console.WriteLine(string.Format(Debug.DEBUG_LOGIN_CREDENTIALS_OBTAINED, devId));
                
                // Giriş işlemini gerçekleştir
                string result = devMode.Login(devId, password);
                
                // Sonuca göre işlem yap
                if (result == Success.LOGIN_SUCCESS)
                {
                    Console.WriteLine(Debug.DEBUG_LOGIN_RESULT_SUCCESSFUL);
                    // Giriş başarılı - Ana menüye bildir (parentForm null check)
                    if (parentForm != null)
                    {
                        parentForm.ActivateDeveloperMode(devMode);
                        Console.WriteLine(Debug.DEBUG_LOGIN_PARENT_FORM_NOTIFIED);
                    }
                    else
                    {
                        // Eğer parent form yoksa burada başarı mesajı göster
                        MessageBox.Show(Success.DEV_MODE_LOGIN_SUCCESS, "Başarılı", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                        
                    // Formu kapat
                    this.DialogResult = DialogResult.OK;
                    Console.WriteLine(Debug.DEBUG_LOGIN_FORM_CLOSING);
                    this.Close();
                }
                else
                {
                    // Hata durumunda mesaj göster
                    MessageBox.Show(result, "Giriş Hatası", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        
                    // Şifre alanını temizle ve odakla
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Debug.DEBUG_SYSTEM_IDENTITY_RECEIVED, ex.Message));
                MessageBox.Show(string.Format(Errors.ERROR_LOGIN_FAILED, ex.Message), "Hata", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine(Debug.DEBUG_LOGIN_CANCEL_PROCESS_STARTED);
                // İptal durumunda formu kapat
                this.DialogResult = DialogResult.Cancel;
                MessageBox.Show(Success.LOGIN_CANCEL_SUCCESS, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Debug.DEBUG_SYSTEM_IDENTITY_RECEIVED, ex.Message));
                MessageBox.Show(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), "Hata", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void FrmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Console.WriteLine(Debug.DEBUG_LOGIN_FORM_CLOSING_HANDLER);
                // Uygulamayı tamamen kapatmak yerine, sadece bu formu kapat
                if (parentForm != null)
                {
                    e.Cancel = false; // Formun kapanmasını engelleme
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Debug.DEBUG_SYSTEM_IDENTITY_RECEIVED, ex.Message));
                MessageBox.Show(string.Format(Errors.ERROR_LOGIN_FORM_CLOSING, ex.Message), "Hata", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
