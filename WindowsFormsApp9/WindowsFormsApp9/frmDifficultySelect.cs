using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MaviZafer
{
    public partial class frmDifficultySelect : Form
    {
        private string playerName;
        private int userId; // Kullanıcı ID'si
        private Button btnEasy;
        private Button btnMedium;
        private Button btnHard;
        private Button btnBack; // Geri butonu
        private int selectedGridSize; // Seçilen tahta boyutunu tutacak

        public frmDifficultySelect(string playerName, int userId)
        {
            this.playerName = playerName;
            this.userId = userId; // Kullanıcı ID'sini alıyoruz
            InitializeComponent();
            this.Resize += FrmDifficultySelect_Resize; // Dinamik yeniden boyutlandırma
            CenterButtons(); // İlk merkezleme
        }

        private void InitializeComponent()
        {
            this.Text = "Zorluk Seçimi";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black;

            string backgroundPath = @"C:\Users\MEHMET POLAT\Downloads\bs.png";
            if (System.IO.File.Exists(backgroundPath))
            {
                this.BackgroundImage = Image.FromFile(backgroundPath);
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }

            // Zorluk seviyelerine göre butonlar
            btnEasy = CreateButton("Kolay", Color.LightGreen, (s, e) => SetDifficulty(7)); // 7x7 boyutunda
            btnMedium = CreateButton("Orta", Color.Orange, (s, e) => SetDifficulty(10)); // 10x10 boyutunda
            btnHard = CreateButton("Zor", Color.Red, (s, e) => SetDifficulty(15)); // 15x15 boyutunda

            btnBack = CreateBackButton(); // Geri butonu oluştur

            this.Controls.Add(btnEasy);
            this.Controls.Add(btnMedium);
            this.Controls.Add(btnHard);
            this.Controls.Add(btnBack);

            // Form kapanma işlemi
            this.FormClosing += (s, e) =>
            {
                Application.Exit();
            };
        }

        private Button CreateButton(string text, Color color, EventHandler onClick)
        {
            var button = new Button()
            {
                Text = text,
                Width = 200,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;
            button.Click += onClick;

            ApplyButtonStyle(button);
            return button;
        }

        private Button CreateBackButton()
        {
            var button = new Button()
            {
                Text = "Geri",
                Width = 200,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Gray,
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;

            button.MouseEnter += (s, e) => button.BackColor = Color.Red;
            button.MouseLeave += (s, e) => button.BackColor = Color.Gray;

            button.Click += (s, e) =>
            {
                frmSecim secimForm = new frmSecim(userId, playerName); // Kullanıcı adı ve ID'siyle frmSecim formunu açıyoruz
                secimForm.Show();
                this.Hide(); // Zorluk seçimi ekranını gizliyoruz
            };

            return button;
        }


        private void FrmDifficultySelect_Resize(object sender, EventArgs e)
        {
            CenterButtons(); // Form boyutları değiştiğinde butonları yeniden hizala
        }

        private void CenterButtons()
        {
            int totalHeight = (btnEasy.Height + 10) * 3; // Butonlar arasındaki mesafe
            int startingTop = (this.ClientSize.Height - totalHeight) / 2; // Yüksekliğe göre başlangıç

            int centerLeft = (this.ClientSize.Width - btnEasy.Width) / 2; // Yatayda ortalama

            // Butonların pozisyonlarını ayarlama
            btnEasy.Top = startingTop;
            btnEasy.Left = centerLeft;

            btnMedium.Top = btnEasy.Bottom + 10;
            btnMedium.Left = centerLeft;

            btnHard.Top = btnMedium.Bottom + 10;
            btnHard.Left = centerLeft;

            btnBack.Top = btnHard.Bottom + 10;
            btnBack.Left = centerLeft;
        }

        private void ApplyButtonStyle(Button button)
        {
            button.Region = new Region(CreateRoundRectPath(button.ClientRectangle, 15));
        }

        private static GraphicsPath CreateRoundRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void SetDifficulty(int gridSize)
        {
            // Zorluk seviyesi belirlendikten sonra oyun tahtası açılır
            selectedGridSize = gridSize;
            OpenGameBoard();
        }

        private void OpenGameBoard()
        {
            // Seçilen zorluk seviyesine göre oyun tahtasını başlatıyoruz
            frmGameBoard gameBoardForm = new frmGameBoard(playerName, selectedGridSize, userId); // userId ekledik
            gameBoardForm.Show();
            this.Hide(); // Zorluk seçimi ekranını gizle
        }
    }
}
