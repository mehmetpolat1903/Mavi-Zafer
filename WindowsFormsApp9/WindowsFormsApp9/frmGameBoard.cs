using System;
using System.Drawing;
using System.Windows.Forms;

namespace MaviZafer
{
    public partial class frmGameBoard : Form
    {
        private readonly string playerName;
        private readonly int gridSize;
        private readonly int userId; // Kullanıcı ID'si
        private readonly Panel[,] cells;
        private readonly bool[,] boardState;
        private PictureBox selectedShip = null;
        private bool isHorizontal = true; // Geminin yönü
        private int shipsPlaced = 0;
        private const int CellSize = 40;
        private Button btnStart;
        private Button btnBack;
        private Button btnInfo; // Soru işareti butonu

        public frmGameBoard(string playerName, int gridSize, int userId)
        {
            this.playerName = playerName;
            this.gridSize = gridSize;
            this.userId = userId;
            cells = new Panel[gridSize, gridSize];
            boardState = new bool[gridSize, gridSize];

            InitializeComponent();
        }

        private void InitializeComponent()
        {
           
            this.Text = "Oyun Tahtası - Gemi Yerleştirme";
            this.FormBorderStyle = FormBorderStyle.None; // Pencere kenarlığını kaldırıyoruz
            this.WindowState = FormWindowState.Maximized; // Tam ekran yapıyoruz
            this.ClientSize = new Size(gridSize * CellSize + 250, gridSize * CellSize + 70);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Arka plan resmini ayarlayın
            this.BackgroundImage = Image.FromFile(@"C:\\Users\\MEHMET POLAT\\Downloads\\bs.png");
            this.BackgroundImageLayout = ImageLayout.Stretch; // Resmi pencereye uyacak şekilde genişletir

            CreateGameGrid();
            CreateShipArea();

            // Soru işareti butonu (info butonu)
            btnInfo = new Button
            {
                Text = "?",
                Width = 35,
                Height = 35,
                BackColor = Color.Green,
                ForeColor = Color.White,
                Font = new Font("Arial", 18, FontStyle.Bold),
                Location = new Point(this.ClientSize.Width - 60, 10),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnInfo.FlatAppearance.BorderSize = 0;
            btnInfo.Click += BtnInfo_Click; // Butona tıklandığında bilgi gösterme
            this.Controls.Add(btnInfo);

            // Başlat butonu
            btnStart = new Button
            {
                Text = "Başla",
                Enabled = false,
                Width = 100,
                Height = 40,
                BackColor = Color.Green,
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(gridSize * CellSize + 20, this.ClientSize.Height - 70)
            };
            btnStart.Click += BtnStart_Click;
            this.Controls.Add(btnStart);

            // Geri butonu
            btnBack = new Button
            {
                Text = "Geri",
                Width = 100,
                Height = 40,
                BackColor = Color.Red,
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(gridSize * CellSize + 130, this.ClientSize.Height - 70)
            };
            btnBack.Click += BtnBack_Click;
            this.Controls.Add(btnBack);
        }

        private void BtnInfo_Click(object sender, EventArgs e)
        {
            // Butona tıklanırsa bilgi mesajı gösterilsin
            MessageBox.Show("Gemi yerleştirmek için sürükleyin.\nDöndürmek için çift tıklayın.", 
                            "Yardım", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Information);
        }

        private void CreateGameGrid()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Panel cell = new Panel
                    {
                        Size = new Size(CellSize, CellSize),
                        Location = new Point(j * CellSize + 10, i * CellSize + 10),
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.LightSkyBlue,
                        Tag = new Point(i, j)
                    };


                    cells[i, j] = cell;
                    this.Controls.Add(cell);
                }
            }
        }

        private void CreateShipArea()
        {
            PictureBox[] ships = new PictureBox[]
            {
                CreateShip("Gemi5", 5),
                CreateShip("Gemi4", 4),
                CreateShip("Gemi3", 3),
                CreateShip("Gemi2", 2)
            };

            int offsetX = gridSize * CellSize + 20;
            int offsetY = 50;

            foreach (var ship in ships)
            {
                ship.Location = new Point(offsetX, offsetY);
                this.Controls.Add(ship);
                offsetY += ship.Height + 20;
            }
        }

        private PictureBox CreateShip(string name, int size)
        {
            PictureBox pbShip = new PictureBox
            {
                Name = name,
                Tag = size,
                Size = new Size(CellSize * size, CellSize), // Gemi kutucuk boyutlarına tam oturacak
                BorderStyle = BorderStyle.Fixed3D,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent,
                Image = GetShipImage(name),
                SizeMode = PictureBoxSizeMode.StretchImage, // Resmi kutucuklara tam sığdır
                Margin = new Padding(0),  // Boşlukları kaldır
                Padding = new Padding(0)
            };

            // Her gemiye özel isHorizontal bayrağını store etme
            pbShip.Tag = new { Size = size, IsHorizontal = true }; // Geminin boyutunu ve yönünü sakla

            pbShip.MouseDown += Ship_MouseDown;
            pbShip.DoubleClick += (sender, e) => Ship_DoubleClick(sender, e, pbShip); // Her gemiye özel çift tıklama event handler
            pbShip.MouseMove += Ship_MouseMove;
            pbShip.MouseUp += Ship_MouseUp;
            return pbShip;
        }


        private Image GetShipImage(string shipName)
        {
            try
            {
                // Gemi ismine göre resim dosyası belirleniyor
                switch (shipName)
                {
                    case "Gemi5":
                        return Image.FromFile(@"C:\\Users\\MEHMET POLAT\\Downloads\\bsp5.png");
                    case "Gemi4":
                        return Image.FromFile(@"C:\\Users\\MEHMET POLAT\\Downloads\\bsp4.png");
                    case "Gemi3":
                        return Image.FromFile(@"C:\\Users\\MEHMET POLAT\\Downloads\\bsp3.png");
                    case "Gemi2":
                        return Image.FromFile(@"C:\\Users\\MEHMET POLAT\\Downloads\\bsp2.png");
                    default:
                        return Image.FromFile(@"C:\\Users\\MEHMET POLAT\\Downloads\\default_ship.png"); // Varsayılan bir resim
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Resim yüklenirken bir hata oluştu: {ex.Message}");
                return null;
            }
        }

        private void Ship_MouseDown(object sender, MouseEventArgs e)
        {
            selectedShip = sender as PictureBox;
            selectedShip.BringToFront();
        }

        private void Ship_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedShip != null && e.Button == MouseButtons.Left)
            {
                selectedShip.Left += e.X - selectedShip.Width / 2;
                selectedShip.Top += e.Y - selectedShip.Height / 2;
            }
        }

        private void Ship_MouseUp(object sender, MouseEventArgs e)
        {
            if (selectedShip != null)
            {
                Point nearestCell = GetNearestCell(selectedShip.Location);

                // selectedShip.Tag'deki boyut ve yön bilgilerini almak için dynamic türünden veri kullanıyoruz
                dynamic shipTag = selectedShip.Tag;
                int shipSize = shipTag.Size;
                bool isHorizontal = shipTag.IsHorizontal;

                // Gemiyi yerleştirme koşulunu kontrol et
                if (nearestCell.X >= 0 && CanPlaceShip(nearestCell, shipSize, isHorizontal))
                {
                    PlaceShip(nearestCell, shipSize, isHorizontal);
                    selectedShip.Visible = false;
                    shipsPlaced++;
                    if (shipsPlaced == 4)
                    {
                        btnStart.Enabled = true;
                    }
                }
                else
                {
                    selectedShip.Location = new Point(gridSize * CellSize + 20, selectedShip.Top); // Gemi yerleşmediyse orijinal konumuna geri döner
                }

                selectedShip = null;
            }
        }


        private void Ship_DoubleClick(object sender, EventArgs e, PictureBox ship)
        {
            // Gemiye özgü yön değişikliğini kontrol et
            dynamic shipTag = ship.Tag;
            bool isHorizontal = shipTag.IsHorizontal;

            // Yönü değiştir
            isHorizontal = !isHorizontal;

            // Resmi ve boyutları değiştir
            if (isHorizontal)
            {
                ship.Width = (int)shipTag.Size * CellSize;
                ship.Height = CellSize;
                ship.Image.RotateFlip(RotateFlipType.Rotate270FlipNone); // Yatay yön için döndür
            }
            else
            {
                ship.Width = CellSize;
                ship.Height = (int)shipTag.Size * CellSize;
                ship.Image.RotateFlip(RotateFlipType.Rotate90FlipNone); // Dikey yön için döndür
            }

            // Yeni yönü kaydet
            ship.Tag = new { Size = shipTag.Size, IsHorizontal = isHorizontal };

            // Resmin yenilenmesi için
            ship.Invalidate();
        }



        private Point GetNearestCell(Point location)
        {
            int x = (location.Y - 10 + CellSize / 2) / CellSize;
            int y = (location.X - 10 + CellSize / 2) / CellSize;
            return x >= 0 && x < gridSize && y >= 0 && y < gridSize ? new Point(x, y) : new Point(-1, -1);
        }

        private bool CanPlaceShip(Point start, int shipSize, bool isHorizontal)
        {
            for (int i = 0; i < shipSize; i++)
            {
                int x = isHorizontal ? start.X : start.X + i;
                int y = isHorizontal ? start.Y + i : start.Y;

                if (x >= gridSize || y >= gridSize || boardState[x, y])
                {
                    return false;
                }
            }
            return true;
        }

        private void PlaceShip(Point start, int shipSize, bool isHorizontal)
        {
            for (int i = 0; i < shipSize; i++)
            {
                int x = isHorizontal ? start.X : start.X + i;
                int y = isHorizontal ? start.Y + i : start.Y;

                cells[x, y].BackColor = Color.Blue;
                boardState[x, y] = true;
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            frmSecim secimForm = new frmSecim(userId, playerName); // Parametreler eklendi
            secimForm.Show();
            this.Close();
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            frmBattle battleForm = new frmBattle(boardState, gridSize, userId, playerName);
            battleForm.Show();
            this.Hide();
        }
    }
}
