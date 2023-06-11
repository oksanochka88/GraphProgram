using System.Drawing;

public partial class GraphNode
{
    public int ID { get; set; }
    public Point Location { get; set; }
}

public partial class GraphEdge
{
    public int _weight { get; set; }

    public bool _edgeDirection { get; set; } 

    public GraphNode StartNode { get; set; }

    public GraphNode EndNode { get; set; }
}