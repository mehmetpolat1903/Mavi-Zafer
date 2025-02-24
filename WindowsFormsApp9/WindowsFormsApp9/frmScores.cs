using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MaviZafer
{
    public partial class frmScores : Form
    {
        public frmScores(List<PlayerScore> scores)
        {
            InitializeComponent();
            LoadScores(scores);
        }

        private void InitializeComponent()
        {
            this.Text = "Skor Tablosu";
            this.Size = new System.Drawing.Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            DataGridView dgvScores = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            this.Controls.Add(dgvScores);

            dgvScores.Columns.Add("PlayerName", "Oyuncu Adı");
            dgvScores.Columns.Add("Score", "Puan");
        }

        private void LoadScores(List<PlayerScore> scores)
        {
            DataGridView dgvScores = (DataGridView)this.Controls[0];

            foreach (var score in scores)
            {
                dgvScores.Rows.Add(score.PlayerName, score.Score);
            }
        }
    }
}
