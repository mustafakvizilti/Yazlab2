using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using Yazlab2WinForms.Models;

namespace Yazlab2WinForms.DataAccess
{
    public class GraphData
    {
        private JavaScriptSerializer _serializer = new JavaScriptSerializer();

        // KAYDETME METODU
        public void SaveToJson(Graph graph, string filePath)
        {
            var dataToSave = new
            {
                Nodes = graph.Nodes.Select(n => new { n.Name, X = n.Position.X, Y = n.Position.Y, n.Aktiflik }).ToList(),
                Edges = graph.Edges.Select(e => new {
                    From = e.From.Name,
                    To = e.To.Name,
                    Type = e.Type.ToString(),
                    e.Weight
                }).ToList(),
                AdjacencyMatrix = graph.GetAdjacencyMatrix()
            };

            string json = _serializer.Serialize(dataToSave);
            File.WriteAllText(filePath, json);
        }

        // YÜKLEME METODU
        public Graph LoadFromJson(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            string json = File.ReadAllText(filePath);
            dynamic data = _serializer.Deserialize<dynamic>(json);

            Graph newGraph = new Graph();

            // 1. Düðümleri oluþtur
            foreach (var n in data["Nodes"])
            {
                string name = n["Name"].ToString();
                int x = Convert.ToInt32(n["X"]);
                int y = Convert.ToInt32(n["Y"]);
                double aktif = n.ContainsKey("Aktiflik") ? Convert.ToDouble(n["Aktiflik"]) : 0.5;

                Node newNode = new Node(name, new Point(x, y), aktif);
                newGraph.AddNode(newNode);
            }

            // 2. Kenarlarý oluþtur
            foreach (var e in data["Edges"])
            {
                Node fromNode = newGraph.Nodes.FirstOrDefault(n => n.Name == e["From"].ToString());
                Node toNode = newGraph.Nodes.FirstOrDefault(n => n.Name == e["To"].ToString());

                if (fromNode != null && toNode != null)
                {
                    RelationType rType = (RelationType)Enum.Parse(typeof(RelationType), e["Type"].ToString());
                    int weight = Convert.ToInt32(e["Weight"]);
                    newGraph.AddEdge(fromNode, toNode, rType, weight); 
                }
            }

            return newGraph;
        }
    }
}