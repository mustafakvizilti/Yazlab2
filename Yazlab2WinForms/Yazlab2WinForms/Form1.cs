using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Yazlab2WinForms
{
    public partial class Form1 : Form
    {
        private Graph graph = new Graph();
        private List<Node> pathToHighlight = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblInfo.Text = "Graf uygulaması hazır!";
            pictureBox1.BackColor = Color.White;
        }

        // Düğüm ekleme
        private void btnAddNode_Click(object sender, EventArgs e)
        {
            string nodeName = txtNodeName.Text.Trim();

            // Eğer isim boşsa otomatik numara ver
            if (string.IsNullOrEmpty(nodeName))
            {
                int nextNumber = 1;
                while (graph.Nodes.Any(node => node.Name == nextNumber.ToString()))
                    nextNumber++;
                nodeName = nextNumber.ToString();
            }

            Node newNode = new Node(nodeName, new Point(0, 0));
            graph.AddNode(newNode);

            // Tüm düğümlerin pozisyonunu güncelle (daire)
            UpdateNodePositions();

            lblInfo.Text = $"Düğüm eklendi: {nodeName}";
            txtNodeName.Clear();
            pictureBox1.Invalidate();
        }

        private void UpdateNodePositions()
        {
            int count = graph.Nodes.Count;
            if (count == 0) return;

            int radius = Math.Min(pictureBox1.Width, pictureBox1.Height) / 2 - 50;
            Point center = new Point(pictureBox1.Width / 2, pictureBox1.Height / 2);

            for (int i = 0; i < count; i++)
            {
                double angle = 2 * Math.PI * i / count;
                int x = center.X + (int)(radius * Math.Cos(angle)) - 25;
                int y = center.Y + (int)(radius * Math.Sin(angle)) - 25;
                graph.Nodes[i].Position = new Point(x, y);
            }
        }

        // Kenar ekleme
        private void btnAddEdge_Click(object sender, EventArgs e)
        {
            if (graph.Nodes.Count == 0)
            {
                lblInfo.Text = "Önce düğüm ekleyin!";
                return;
            }

            string sourceName = txtSource.Text.Trim();
            string targetName = txtTarget.Text.Trim();

            List<Node> fromNodes;
            List<Node> toNodes;

            // Kaynak boşsa tüm düğümler
            if (string.IsNullOrEmpty(sourceName))
                fromNodes = new List<Node>(graph.Nodes);
            else
            {
                var n = graph.Nodes.FirstOrDefault(node => node.Name == sourceName);
                if (n == null)
                {
                    lblInfo.Text = "Kaynak düğüm bulunamadı!";
                    return;
                }
                fromNodes = new List<Node> { n };
            }

            // Hedef boşsa tüm düğümler
            if (string.IsNullOrEmpty(targetName))
                toNodes = new List<Node>(graph.Nodes);
            else
            {
                var n = graph.Nodes.FirstOrDefault(node => node.Name == targetName);
                if (n == null)
                {
                    lblInfo.Text = "Hedef düğüm bulunamadı!";
                    return;
                }
                toNodes = new List<Node> { n };
            }

            int addedEdges = 0;
            foreach (var from in fromNodes)
            {
                foreach (var to in toNodes)
                {
                    if (from != to && !graph.Edges.Any(a => (a.From == from && a.To == to) || (a.From == to && a.To == from)))
                    {
                        graph.AddEdge(from, to);
                        addedEdges++;
                    }
                }
            }

            lblInfo.Text = $"{addedEdges} kenar eklendi.";
            txtSource.Clear();
            txtTarget.Clear();
            pictureBox1.Invalidate();
        }

        // En kısa yol
        private void btnShortestPath_Click(object sender, EventArgs e)
        {
            Node start = graph.Nodes.FirstOrDefault(n => n.Name == txtSource.Text.Trim());
            Node end = graph.Nodes.FirstOrDefault(n => n.Name == txtTarget.Text.Trim());

            if (start == null || end == null)
            {
                lblInfo.Text = "En kısa yol için geçerli düğümler girin!";
                return;
            }

            // Dijkstra ile en kısa yol
            pathToHighlight = graph.FindShortestPathDijkstra(start, end);

            if (pathToHighlight == null)
                lblInfo.Text = "Yol bulunamadı!";
            else
                lblInfo.Text = "En kısa yol: " + string.Join(" → ", pathToHighlight.Select(n => n.Name));

            pictureBox1.Invalidate();
        }

        // Çizim
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Kenarları çiz (yönsüz)
            foreach (var edge in graph.Edges)
            {
                Pen pen = Pens.Gray;

                if (pathToHighlight != null)
                    for (int i = 0; i < pathToHighlight.Count - 1; i++)
                        if ((edge.From == pathToHighlight[i] && edge.To == pathToHighlight[i + 1]) ||
                            (edge.To == pathToHighlight[i] && edge.From == pathToHighlight[i + 1]))
                            pen = new Pen(Color.Red, 2);

                g.DrawLine(pen,
                    edge.From.Position.X + 25, edge.From.Position.Y + 25,
                    edge.To.Position.X + 25, edge.To.Position.Y + 25);
            }

            // Düğümleri çiz
            foreach (var node in graph.Nodes)
            {
                g.FillEllipse(Brushes.LightBlue, node.Position.X, node.Position.Y, 50, 50);
                g.DrawEllipse(Pens.Black, node.Position.X, node.Position.Y, 50, 50);
                g.DrawString(node.Name, this.Font, Brushes.Black, node.Position.X + 15, node.Position.Y + 15);
            }
        }
    }
}
