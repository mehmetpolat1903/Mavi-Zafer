using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MaviZafer
{
    public class frmLogin : Form
    {
        private Button btnLogin;
        private Button btnSignUp;
        private Button btnHowToPlay;
        private Button btnAbout;
        private Button btnClose;
        private PictureBox backgroundPictureBox;

        public frmLogin()
        {
            InitializeComponent();
        }

        public string conString = "Data Source=DESKTOP-1BSONIC\\SQLEXPRESS;Initial Catalog=Database1;Integrated Security=True;Trust Server Certificate=True";

        private void InitializeComponent()
        {
            this.Text = "MAVİ ZAFER";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.ControlBox = true;
            this.MaximizeBox = true;
            this.MinimizeBox = true;

            backgroundPictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                Image = Image.FromFile("C:\\Users\\MEHMET POLAT\\Downloads\\bs.png"),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            this.Controls.Add(backgroundPictureBox);

            Label lblTitle = new Label
            {
                Text = "MAVİ ZAFER OYUNU",
                Font = new Font("Arial", 36, FontStyle.Bold),
                ForeColor = Color.WhiteSmoke,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.FromArgb(50, 0, 0, 0)
            };
            backgroundPictureBox.Controls.Add(lblTitle);

            btnLogin = CreateTexturedButton("Giriş Yap", 200, Color.FromArgb(64, 64, 64), Color.DarkBlue);
            btnLogin.Click += BtnLogin_Click;

            btnSignUp = CreateTexturedButton("Üye Ol", 300, Color.FromArgb(64, 128, 64), Color.Green);
            btnSignUp.Click += BtnSignUp_Click;

            btnHowToPlay = CreateTexturedButton("Nasıl Oynanır", 400, Color.FromArgb(128, 128, 64), Color.Olive);
            btnHowToPlay.Click += BtnHowToPlay_Click;

            btnAbout = CreateTexturedButton("Hakkında", 500, Color.FromArgb(64, 128, 128), Color.Teal);
            btnAbout.Click += BtnAbout_Click;

            btnClose = CreateTexturedButton("Oyunu Kapat", 600, Color.FromArgb(128, 64, 64), Color.DarkRed);
            btnClose.Click += BtnClose_Click;

            backgroundPictureBox.Controls.Add(btnLogin);
            backgroundPictureBox.Controls.Add(btnSignUp);
            backgroundPictureBox.Controls.Add(btnHowToPlay);
            backgroundPictureBox.Controls.Add(btnAbout);
            backgroundPictureBox.Controls.Add(btnClose);
        }

        private Button CreateTexturedButton(string text, int topPosition, Color startColor, Color endColor)
        {
            Button button = new Button
            {
                Text = text,
                Width = 200,
                Height = 80,
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Top = topPosition,
                Left = (this.ClientSize.Width - 200) / 2,
                FlatAppearance = { BorderSize = 0 }
            };

            // Button Paint event to add gradient and shadow effects
            button.Paint += (sender, e) =>
            {
                // Gradient background
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    new Rectangle(0, 0, button.Width, button.Height),
                    startColor,
                    endColor,
                    LinearGradientMode.ForwardDiagonal))
                {
                    e.Graphics.FillRectangle(brush, 0, 0, button.Width, button.Height);
                }

                // Shadow effect for texture
                for (int i = 0; i < 3; i++)
                {
                    Rectangle shadowRect = new Rectangle(i, i, button.Width - i * 2, button.Height - i * 2);
                    using (Pen shadowPen = new Pen(Color.FromArgb(80 - i * 20, 0, 0, 0), 1))
                    {
                        e.Graphics.DrawRectangle(shadowPen, shadowRect);
                    }
                }

                // Apply elliptical shape
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(0, 0, button.Width, button.Height);
                    button.Region = new Region(path);
                }

                // Draw text
                TextRenderer.DrawText(e.Graphics, text, button.Font, new Rectangle(0, 0, button.Width, button.Height), Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };

            return button;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            var signInForm = new frmSignIn();
            signInForm.Show();
            this.Hide();
        }

        private void BtnSignUp_Click(object sender, EventArgs e)
        {
            frmRegister registerForm = new frmRegister();
            registerForm.Show();
            this.Hide();
        }

        private void BtnHowToPlay_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Mavi Zafer Nasıl Oynanır:\n\n" +
                "1. Oyun ızgara tabanlı bir strateji oyunudur. Oyuncular kendi gemilerini yerleştirir ve rakibin gemilerini bulmaya çalışır.\n" +
                "2. Gemi yerleştirirken çift tıklama ile geminin yönünü değiştirebilirsiniz.\n" +
                "3. Oyuncular sırayla rakibin tahtasına atış yapar. İsabet eden bir atış rakibin gemisine hasar verir.\n" +
                "4. Amaç, rakibin tüm gemilerini batırarak oyunu kazanmaktır.\n" +
                "5. Kazanmak için iyi bir strateji kurmalı, rakibin gemilerinin yerini tahmin etmelisiniz.\n" +
                "6. Oyun sonunda, isabetli atışlara göre puan hesaplanır. Eğer kazanırsanız ekstra puanlar alırsınız.\n\n" +
                "İyi şanslar!",
                "Oyun Nasıl Oynanır");
        }

        private void BtnAbout_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.Show();
            this.Hide();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
