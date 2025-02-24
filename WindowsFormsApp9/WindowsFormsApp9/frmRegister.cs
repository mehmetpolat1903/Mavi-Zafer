using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System;
using MaviZafer;

public partial class frmRegister : Form
{
    private TextBox txtUsername;
    private TextBox txtPassword;
    private TextBox txtEmail;
    private TextBox txtConfirmPassword;
    private Button btnRegister;
    private Button btnBack; // Geri butonu
    private Label lblUsername;
    private Label lblPassword;
    private Label lblEmail;
    private Label lblConfirmPassword;

    public frmRegister()
    {
        InitializeComponent();
        InitializeControls();
    }

    private void InitializeComponent() { }

    private void InitializeControls()
    {
        // Form settings
        this.Text = "Kayıt Ol";
        this.WindowState = FormWindowState.Maximized; // Tam ekran
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.StartPosition = FormStartPosition.CenterScreen;

        // Form background settings
        this.BackgroundImage = Image.FromFile(@"C:\Users\MEHMET POLAT\Downloads\bs.png");
        this.BackgroundImageLayout = ImageLayout.Stretch;

        // Username Label and TextBox
        lblUsername = new Label
        {
            Text = "Kullanıcı Adı:",
            Font = new Font("Arial", 16, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            AutoSize = true
        };
        this.Controls.Add(lblUsername);

        txtUsername = new TextBox
        {
            Font = new Font("Arial", 12),
            BackColor = Color.LightGray,
            Size = new Size(300, 30)
        };
        this.Controls.Add(txtUsername);

        // Password Label and TextBox
        lblPassword = new Label
        {
            Text = "Şifre:",
            Font = new Font("Arial", 16, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            AutoSize = true
        };
        this.Controls.Add(lblPassword);

        txtPassword = new TextBox
        {
            Font = new Font("Arial", 12),
            BackColor = Color.LightGray,
            Size = new Size(300, 30),
            UseSystemPasswordChar = true
        };
        this.Controls.Add(txtPassword);

        // Confirm Password Label and TextBox
        lblConfirmPassword = new Label
        {
            Text = "Şifre Tekrarı:",
            Font = new Font("Arial", 14, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            AutoSize = true
        };
        this.Controls.Add(lblConfirmPassword);

        txtConfirmPassword = new TextBox
        {
            Font = new Font("Arial", 12),
            BackColor = Color.LightGray,
            Size = new Size(300, 30),
            UseSystemPasswordChar = true
        };
        this.Controls.Add(txtConfirmPassword);

        // Email Label and TextBox
        lblEmail = new Label
        {
            Text = "E-posta:",
            Font = new Font("Arial", 16, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            AutoSize = true
        };
        this.Controls.Add(lblEmail);

        txtEmail = new TextBox
        {
            Font = new Font("Arial", 12),
            BackColor = Color.LightGray,
            Size = new Size(300, 30)
        };
        this.Controls.Add(txtEmail);

        // Register Button
        btnRegister = new Button
        {
            Text = "Kayıt Ol",
            Font = new Font("Arial", 18, FontStyle.Bold),
            BackColor = Color.Green,
            ForeColor = Color.White,
            Size = new Size(200, 50)
        };
        this.Controls.Add(btnRegister);

        btnBack = new Button
        {
            Text = "Geri",
            Font = new Font("Arial", 14, FontStyle.Bold),
            BackColor = Color.Gray,
            ForeColor = Color.White,
            Size = new Size(200, 50)
        };
        btnBack.MouseEnter += (s, e) => btnBack.BackColor = Color.Red; // Fare üzerine geldiğinde kırmızı
        btnBack.MouseLeave += (s, e) => btnBack.BackColor = Color.Gray; // Fare ayrıldığında tekrar gri
        btnBack.Click += BtnBack_Click;
        this.Controls.Add(btnBack);

        // Event Handlers
        btnRegister.Click += BtnRegister_Click;
        btnBack.Click += BtnBack_Click;

        this.Load += FrmRegister_Load; // Form yüklenirken öğeleri hizala
    }

    private void FrmRegister_Load(object sender, EventArgs e)
    {
        // Ekran ortasına hizalama
        int centerX = this.ClientSize.Width / 2;
        int centerY = this.ClientSize.Height / 2;

        lblUsername.Location = new Point(centerX - txtUsername.Width / 2 - lblUsername.Width - 10, centerY - 150);
        txtUsername.Location = new Point(centerX - txtUsername.Width / 2, lblUsername.Top);

        lblPassword.Location = new Point(lblUsername.Left, lblUsername.Bottom + 20);
        txtPassword.Location = new Point(centerX - txtPassword.Width / 2, lblPassword.Top);

        lblConfirmPassword.Location = new Point(lblUsername.Left, lblPassword.Bottom + 20);
        txtConfirmPassword.Location = new Point(centerX - txtConfirmPassword.Width / 2, lblConfirmPassword.Top);

        lblEmail.Location = new Point(lblUsername.Left, lblConfirmPassword.Bottom + 20);
        txtEmail.Location = new Point(centerX - txtEmail.Width / 2, lblEmail.Top);

        btnRegister.Location = new Point(centerX - btnRegister.Width / 2, txtEmail.Bottom + 40);
        btnBack.Location = new Point(centerX - btnBack.Width / 2, btnRegister.Bottom + 20);
    }

    private void BtnRegister_Click(object sender, EventArgs e)
    {
        string username = txtUsername.Text;
        string password = txtPassword.Text;
        string confirmPassword = txtConfirmPassword.Text;
        string email = txtEmail.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(confirmPassword) || string.IsNullOrWhiteSpace(email))
        {
            MessageBox.Show("Lütfen tüm alanları doldurun.");
            return;
        }

        if (password.Length < 8)
        {
            MessageBox.Show("Lütfen en az 8 karakter uzunluğunda bir şifre girin.");
            return;
        }

        if (password != confirmPassword)
        {
            MessageBox.Show("Şifreler eşleşmiyor. Lütfen tekrar kontrol edin.");
            return;
        }

        if (!email.Contains("@"))
        {
            MessageBox.Show("Lütfen geçerli bir e-posta adresi girin.");
            return;
        }

        string connectionString = "Data Source=DESKTOP-1BSONIC\\SQLEXPRESS;Initial Catalog=Database1;Integrated Security=True;";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO Users (Username, Password, Email) VALUES (@username, @password, @Email)";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@Email", email);

                try
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show("Kayıt başarılı!");

                    frmLogin loginForm = new frmLogin();
                    loginForm.Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Kayıt başarısız: " + ex.Message);
                }
            }
        }
    }

    private void BtnBack_Click(object sender, EventArgs e)
    {
        frmLogin loginForm = new frmLogin();
        loginForm.Show();
        this.Close();
    }
}
