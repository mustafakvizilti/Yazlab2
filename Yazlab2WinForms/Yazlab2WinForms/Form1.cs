using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Yazlab2WinForms
{
    public partial class Form1 : Form
    {
        List<string> nodes = new List<string>();
        List<(string, string)> edges = new List<(string, string)>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblInfo.Text = "Graf uygulaması hazır!";
        }

        private void txtNodeName_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnAddNode_Click(object sender, EventArgs e)
        {
            string node = txtNodeName.Text.Trim();

            if (string.IsNullOrEmpty(node))
            {
                lblInfo.Text = "Lütfen bir düğüm adı girin!";
                return;
            }

            if (nodes.Contains(node))
            {
                lblInfo.Text = "Bu düğüm zaten var!";
                return;
            }

            nodes.Add(node);
            lblInfo.Text = $"Düğüm eklendi: {node}";
            txtNodeName.Clear();
        }


        private void txtSource_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTarget_TextChanged(object sender, EventArgs e)
        {

        }

        private void picGraph_Click(object sender, EventArgs e)
        {

        }

        private void btnAddEdge_Click(object sender, EventArgs e)
        {
            string s = txtSource.Text.Trim();
            string t = txtTarget.Text.Trim();

            if (!nodes.Contains(s) || !nodes.Contains(t))
            {
                lblInfo.Text = "Bağlantı için her iki düğüm de var olmalı!";
                return;
            }

            edges.Add((s, t));
            lblInfo.Text = $"Kenar eklendi: {s} → {t}";
            txtSource.Clear();
            txtTarget.Clear();
        }

private void picGraph_Paint(object sender, PaintEventArgs e)
{
    Graphics g = e.Graphics;
    int x = 20, y = 20;

    foreach (var node in nodes)
    {
        g.FillEllipse(Brushes.LightBlue, x, y, 50, 50);
        g.DrawEllipse(Pens.Black, x, y, 50, 50);
        g.DrawString(node, this.Font, Brushes.Black, x+15, y+15);
        y += 70;
    }
}

    }
}
