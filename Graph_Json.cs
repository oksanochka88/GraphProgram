using System.Collections.Generic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace GrafTextJson
{
    public class Graph
    {
        public int VertexCount { get; set; }
        public List<Node> Nodes { get; set; }
        public List<Edge> Edges { get; set; }
        public Graph()
        {
            Nodes = new List<Node>();
            Edges = new List<Edge>();
        }
    }

    public class Node
    {
        public int Id { get; set; }
    }

    public class Edge
    {
        public int Source { get; set; }
        public int Target { get; set; }
        public bool Direction { get; set; } 
        public int Weight { get; set; }
    }
}