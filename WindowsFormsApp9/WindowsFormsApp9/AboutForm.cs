using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MaviZafer
{
    public class AboutForm : Form
    {
        private Label lblAbout;
        private Timer scrollTimer;

        public AboutForm()
        {
            // Form ayarları
            this.Text = "Hakkında";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Arka plan resmi
            string imagePath = @"C:\Users\MEHMET POLAT\Downloads\bs.png";
            this.BackgroundImage = Image.FromFile(imagePath);
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // Şeffaflık için panel ayarı (yuvarlatılmış köşelerle)
            Panel panel = new Panel
            {
                BackColor = Color.FromArgb(76, Color.Black), // %70 şeffaflık
                Width = 800,
                Height = 400,
                Location = new Point((this.ClientSize.Width - 800) / 2, (this.ClientSize.Height - 400) / 2 - 60),
                Anchor = AnchorStyles.None,
                AutoScroll = false // AutoScroll'u kapattık çünkü manuel kaydırma yapacağız
            };
            this.Controls.Add(panel);

            // Panelin kenarlarını yuvarlatma
            int cornerRadius = 25; // Yuvarlaklık miktarı
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, cornerRadius, cornerRadius, 180, 90);
            path.AddArc(panel.Width - cornerRadius, 0, cornerRadius, cornerRadius, 270, 90);
            path.AddArc(panel.Width - cornerRadius, panel.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
            path.AddArc(0, panel.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
            path.CloseAllFigures();
            panel.Region = new Region(path);

            // Hakkında metni (Label)
            lblAbout = new Label
            {
                Text =  "Proje Hakkında:\n\n" +
        "Mavi Zafer, klasik Battle Ship oyununun modern ve dijital bir yorumudur. Oyun, .NET Framework kullanılarak C# dilinde geliştirilmiştir ve SQL Server tabanlı bir veri tabanı entegrasyonu içermektedir.\n\n" +
        "Proje, kullanıcı giriş/çıkış işlemleri, oyun zorluk seviyeleri (Kolay, Orta, Zor), puanlama sistemi ve kullanıcıların önceki skorlarını kaydedip görüntüleme özelliklerini kapsamaktadır.\n\n" +
        "Geliştiriciler:\n\n" +
        "1. **Mehmet Polat**\n" +
        "   - Program Adı     : Bilgisayar Programcılığı \n" +
        "   - Telefon Numarası: +90-553-072-5524        \n" +
        "   - E-Posta         : mpolat1785@gmail.com    \n\n" +
        "2. **Alperen Şen**\n" +
        "   - Program Adı     : Bilgisayar Programcılığı \n" +
        "   - Telefon Numarası: +90-501-084-8401         \n" +
        "   - E-Posta         : alperensen100@gmail.com\n\n" +
        "Projeye dair diğer özellikler:\n" +
        "- Kullanıcı dostu bir arayüz\n" +
        "- SQL tabanlı kullanıcı ve skor yönetimi\n" +
        "- Özelleştirilebilir oyun ayarları ve dinamik gemi yerleştirme özelliği\n\n" +
        "Tüm hakları saklıdır. Bu proje eğitim ve kişisel gelişim amaçlı geliştirilmiştir.",
                Font = new Font("Montserrat", 18, FontStyle.Bold), // Kalın yazı tipi
                ForeColor = Color.White, // Beyaz renk ile daha belirgin
                TextAlign = ContentAlignment.TopCenter,
                AutoSize = false,
                Width = 800, // Panelin genişliğine göre sınırlama
                Height = 1000, // Yüksekliği artırarak yazının kaymasını sağlama
                Padding = new Padding(24) // İç boşluk ekleyerek metnin panel içinde rahat görünmesini sağlama
            };

            // Label'i panele ekle
            panel.Controls.Add(lblAbout);

            // Kaydırma işlemi için Timer ayarları
            scrollTimer = new Timer
            {
                Interval = 30 // 40 ms aralıklarla kaydırma yapılacak
            };
            scrollTimer.Tick += ScrollTimer_Tick;
            scrollTimer.Start();

            // Geri butonu
            Button btnBack = new Button
            {
                Text = "Geri",
                Width = 120,
                Height = 40,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Gray, // Başlangıçta gri renk
                ForeColor = Color.White,
                Font = new Font("Montserrat", 16, FontStyle.Bold)
            };

            // Buton kenarlarını yuvarlatma
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Paint += (sender, e) =>
            {
                using (GraphicsPath buttonPath = new GraphicsPath())
                {
                    int radius = 15;
                    buttonPath.AddArc(0, 0, radius, radius, 180, 90);
                    buttonPath.AddArc(btnBack.Width - radius, 0, radius, radius, 270, 90);
                    buttonPath.AddArc(btnBack.Width - radius, btnBack.Height - radius, radius, radius, 0, 90);
                    buttonPath.AddArc(0, btnBack.Height - radius, radius, radius, 90, 90);
                    buttonPath.CloseAllFigures();
                    btnBack.Region = new Region(buttonPath);
                }
            };

            // Fare üzerine gelince rengi değiştirme
            btnBack.MouseEnter += (sender, e) =>
            {
                btnBack.BackColor = Color.Red; // Fare üzerine gelince kırmızı
            };
            btnBack.MouseLeave += (sender, e) =>
            {
                btnBack.BackColor = Color.Gray; // Fare ayrılınca tekrar gri
            };

            // Geri butonu tıklama olayı
            btnBack.Click += (sender, e) =>
            {
                this.Close();
                var loginForm = new frmLogin();
                loginForm.Show();
            };

            // Butonu forma ekleme
            btnBack.Location = new Point((this.ClientSize.Width - btnBack.Width) / 2, panel.Bottom + 20);
            btnBack.Anchor = AnchorStyles.None;
            this.Controls.Add(btnBack);
        }

        private void ScrollTimer_Tick(object sender, EventArgs e)
        {
            // Yazıyı kaydırma
            if (lblAbout.Top < -lblAbout.Height)
            {
                // Eğer yazı tamamen kaymışsa
                scrollTimer.Stop(); // Timer'ı durdur
                this.Close(); // Mevcut formu kapat
                var loginForm = new frmLogin(); // Yeni bir giriş formu oluştur
                loginForm.Show(); // Giriş formunu göster
            }
            else
            {
                // Yazıyı kaydır
                lblAbout.Top -= 1;
            }
        }
    }
}
