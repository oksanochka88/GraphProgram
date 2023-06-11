using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class VisualGraphControl : UserControl
    {
        private const int MAX_GRAPH_NODE = 50;

        public event EventHandler MyEvent;

        int graphCount = 1;

        private bool buttonAddNode = false;
        private bool buttonDeleteNotall = false;
        private bool buttonAddEdgeNaprv = false;
        private bool buttonAddEdgeDontNaprv = false;
        private bool buttonHand = true;

        private readonly int _width = 30;  // Ширина вершины
        private readonly int _height = 30; // Высота вершины

        public static List<GraphNode> graphNodes;
        public static List<GraphEdge> graphEdges;
        private GraphNode _selectedNode;

        private Point _offset;

        List<GraphNode> vertices = new List<GraphNode>();

        public VisualGraphControl()
        {
            InitializeComponent();

            MainForm.FileGraphVisualized += DrawingZadannyiMassivGrapha;

            graphNodes = new List<GraphNode>();
            graphEdges = new List<GraphEdge>();
        }

        protected virtual void OnMyEvent(EventArgs e)
        {
            MyEvent?.Invoke(this, e);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (graphNodes != null)
            {
                // Отрисовываем все ребра
                foreach (GraphEdge edge in graphEdges)
                {
                    if (edge.StartNode == edge.EndNode)
                    {
                        Pen pen = new Pen(Color.Aquamarine, 2);
                        drawPetlz_2(e.Graphics, pen, edge.StartNode.Location.X, edge.StartNode.Location.Y);
                    }
                    else
                    {
                        Pen pen = new Pen(Color.Aquamarine, 2);

                        DrawLineGraphEdge(e.Graphics, pen, edge.StartNode.Location.X, edge.StartNode.Location.Y, edge.EndNode.Location.X, edge.EndNode.Location.Y, edge._edgeDirection, edge._weight);
                    }

                }

                // Отрисовываем все узлы
                foreach (GraphNode node in graphNodes)
                {
                    DrawVertex(node.Location, e.Graphics, node.ID);
                }
            }

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (buttonHand)
            {
                foreach (GraphNode node in graphNodes)
                {
                    Rectangle nodeBounds = new Rectangle(node.Location.X - 15, node.Location.Y - 15, _width, _height);
                    if (nodeBounds.Contains(e.Location))
                    {
                        // Выбираем узел
                        _selectedNode = node;
                        // Запоминаем смещение относительно левого верхнего угла узла
                        _offset = new Point(node.Location.X - e.Location.X, node.Location.Y - e.Location.Y);
                        break;
                    }
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_selectedNode != null && buttonHand)
            {
                _selectedNode.Location = new Point(e.Location.X + _offset.X, e.Location.Y + _offset.Y);
                // Перерисовываем PictureBox
                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (buttonHand)
            {
                _selectedNode = null;
            }
            // Снимаем выделение с узла
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (buttonAddNode)
            {
                if (graphCount <= MAX_GRAPH_NODE)
                {
                    graphNodes.Add(new GraphNode { ID = graphCount, Location = new Point(e.X, e.Y) });
                    graphCount++;

                    pictureBox1.Invalidate();
                }
                else
                    MessageBox.Show("Достигнут лимит вершин!", "Warnings!");
            }

            if (buttonDeleteNotall)
            {
                GraphNode selectedNode = null;

                foreach (GraphNode node in graphNodes)
                {
                    Rectangle nodeBounds = new Rectangle(node.Location.X - 10, node.Location.Y - 10, _width, _height);
                    if (nodeBounds.Contains(e.Location))
                    {
                        // Выбираем узел
                        selectedNode = node;

                        for (int i = graphEdges.Count - 1; i >= 0; i--)
                        {
                            if (graphEdges[i].StartNode == selectedNode || graphEdges[i].EndNode == selectedNode)
                            {
                                graphEdges.RemoveAt(i);
                            }
                        }

                        graphNodes.Remove(selectedNode);

                        graphCount--;

                        break;
                    }
                }

                foreach (GraphEdge edge in graphEdges)
                {
                    if (IsEdgeClicked(e.X, e.Y, edge.StartNode.Location.X, edge.StartNode.Location.Y, edge.EndNode.Location.X, edge.EndNode.Location.Y, 3))
                    {
                        graphEdges.Remove(edge);

                        break;
                    }
                }
                pictureBox1.Invalidate();
            }

            if (buttonAddEdgeNaprv)
            {
                foreach (GraphNode node in graphNodes)
                {
                    Rectangle nodeBounds = new Rectangle(node.Location.X - 15, node.Location.Y - 15, _width, _height);
                    if (nodeBounds.Contains(e.Location))
                    {
                        vertices.Add(node);

                        if (vertices.Count == 2)
                        {
                            graphEdges.Add(new GraphEdge { StartNode = vertices[0], EndNode = vertices[1], _edgeDirection = true, _weight = 1 }); ;
                            // очищаем список вершин для дальнейшего использования
                            vertices.Clear();

                            pictureBox1.Invalidate();
                        }
                    }
                }
            }

            if (buttonAddEdgeDontNaprv)
            {
                foreach (GraphNode node in graphNodes)
                {
                    Rectangle nodeBounds = new Rectangle(node.Location.X - 15, node.Location.Y - 15, _width, _height);
                    if (nodeBounds.Contains(e.Location))
                    {
                        vertices.Add(node);

                        if (vertices.Count == 2)
                        {
                            graphEdges.Add(new GraphEdge { StartNode = vertices[0], EndNode = vertices[1], _weight = 1 });
                            // очищаем список вершин для дальнейшего использования
                            vertices.Clear();

                            pictureBox1.Invalidate();
                        }
                    }
                }
            }
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (buttonHand)
            {
                foreach (GraphEdge edge in graphEdges)
                {
                    if (IsEdgeClicked(e.X, e.Y, edge.StartNode.Location.X, edge.StartNode.Location.Y, edge.EndNode.Location.X, edge.EndNode.Location.Y, 3))
                    {
                        string input = Microsoft.VisualBasic.Interaction.InputBox("Введите вес ребра:",
                           "Изменение веса ребра",
                           "1",
                           0,
                           0);

                        try
                        {
                            int bop = Convert.ToInt32(input);
                            if (bop >= 1)
                                edge._weight = bop;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

                        pictureBox1.Invalidate();
                        break;
                    }
                }
            }

        }
        // Добавление вершины
        private void button1_Click(object sender, EventArgs e)
        {
            deltgeteOtv();
            buttonAddNode = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            deltgeteOtv();
            buttonAddEdgeDontNaprv = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            deltgeteOtv();
            buttonAddEdgeNaprv = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            deltgeteOtv();
            buttonHand = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            deltgeteOtv();
            buttonDeleteNotall = true;

        }
        // Удаление всего графа
        private void button6_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            //
            graphNodes.Clear();
            graphEdges.Clear();

            graphCount = 1;
        }
        // Отрисовка ребра
        private void DrawLineGraphEdge(Graphics g, Pen pen, int x_i, int y_i, int x_j, int y_j, bool edgeDirection, int weight)
        {
            Point startPoint = new Point(x_i, y_i);
            Point endPoint = new Point(x_j, y_j);

            // рассчитываем длину прямой
            double length = Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2));

            // задаем значение, на которое нужно уменьшить длину прямой
            double shortenValue = 15;

            // рассчитываем новую длину прямой
            double newLength = length - shortenValue;

            // рассчитываем новые координаты конечной точки прямой
            Point newEndPoint = new Point((int)(startPoint.X + (newLength / length) * (endPoint.X - startPoint.X)),
                                          (int)(startPoint.Y + (newLength / length) * (endPoint.Y - startPoint.Y)));

            // рисуем новую прямую с укороченным отрезком
            //g.DrawLine(pen, startPoint, newEndPoint);

            x_j = newEndPoint.X;
            y_j = newEndPoint.Y;

            // рассчитываем угол между вектором и осью X
            double angle = Math.Atan2(y_j - y_i, x_j - x_i) * 180.0 / Math.PI;

            // рассчитываем координаты вершин треугольника на конце вектора
            PointF[] arrowPoints = new PointF[3];
            float arrowLength = 20;  // длина треугольника
            float arrowAngle = 20;   // угол между линиями треугольника и вектором (в градусах)
            arrowPoints[0] = new PointF(x_j, y_j);    // конец вектора
            arrowPoints[1] = new PointF(
                (float)(x_j - arrowLength * Math.Cos((angle + arrowAngle) * Math.PI / 180.0)),
                (float)(y_j - arrowLength * Math.Sin((angle + arrowAngle) * Math.PI / 180.0)));   // точка левого угла треугольника
            arrowPoints[2] = new PointF(
                (float)(x_j - arrowLength * Math.Cos((angle - arrowAngle) * Math.PI / 180.0)),
                (float)(y_j - arrowLength * Math.Sin((angle - arrowAngle) * Math.PI / 180.0)));   // точка правого угла треугольника


            Font font = new Font("Arial", 10);
            Brush brush = new SolidBrush(Color.Black);
            int x_text = (x_i + x_j) / 2;
            int y_text = (y_i + y_j) / 2;

            if (edgeDirection)
            {
                g.DrawLine(pen, startPoint, newEndPoint);
                g.FillPolygon(Brushes.Blue, arrowPoints);

                g.DrawString(weight.ToString(), font, brush, x_text, y_text);
            }
            else
            {
                g.DrawLine(pen, startPoint, newEndPoint);
                g.DrawString(weight.ToString(), font, brush, x_text, y_text);
            }

        }

        private void DrawVertex(Point vertex, Graphics graphics, int weiht)
        {
            // вычисляем координаты прямоугольника для текста подписи
            int Radius = 15;
            Font font = new Font("Arial", 12);
            SizeF textSize = graphics.MeasureString(weiht.ToString(), font);
            PointF textLocation = new PointF(
                vertex.X - textSize.Width / 2,
                vertex.Y - textSize.Height / 2
            );
            RectangleF textRect = new RectangleF(textLocation, textSize);

            // рисуем круг для вершины
            Brush brush = new SolidBrush(Color.White);
            Pen pen = new Pen(Color.Black, (float)1.2);
            graphics.FillEllipse(brush, vertex.X - Radius, vertex.Y - Radius, Radius * 2, Radius * 2);
            graphics.DrawEllipse(pen, vertex.X - Radius, vertex.Y - Radius, Radius * 2, Radius * 2);

            // рисуем текст подписи вершины
            graphics.DrawString(weiht.ToString(), font, Brushes.Black, textRect);
        }

        private bool IsEdgeClicked(int x, int y, int x1, int y1, int x2, int y2, int margin)
        {

            // Вычисляем длину ребра
            double edgeLength = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

            // Вычисляем расстояние от точки (x, y) до ребра
            double distanceToEdge = Math.Abs((y2 - y1) * x - (x2 - x1) * y + x2 * y1 - y2 * x1) / edgeLength;

            // Проверяем, находится ли точка на ребре с отступом margin
            if (distanceToEdge <= margin && x >= Math.Min(x1, x2) - margin && x <= Math.Max(x1, x2) + margin &&
                y >= Math.Min(y1, y2) - margin && y <= Math.Max(y1, y2) + margin)
            {
                return true;
            }

            return false;
        }

        private void drawPetlz_2(Graphics g, Pen pen, int x_i, int y_i)
        {
            int R = 15;
            // координаты начала и конца петли
            g.DrawArc(pen, (x_i - 2 * R), (y_i - 2 * R), 2 * R, 2 * R, 90, 270);
        }

        private void deltgeteOtv()
        {
            buttonAddNode = false;
            buttonDeleteNotall = false;
            buttonAddEdgeNaprv = false;
            buttonAddEdgeDontNaprv = false;
            buttonHand = false;
        }
        //Замена
        private bool IsCircleValid(Point center, int radius)
        {
            //graphnode
            foreach (GraphNode circle in graphNodes)
            {
                double distance = Math.Sqrt(Math.Pow(center.X - circle.Location.X, 2) + Math.Pow(center.Y - circle.Location.Y, 2));
                if (distance < radius + 30)
                {
                    return false; // Найдено пересечение с другой окружностью
                }
            }

            return true;
        }

        private Point GenerateRandomPoint()
        {
            Point point = new Point();
            int width = pictureBox1.Width;
            int height = pictureBox1.Height;

            while (true)
            {
                Random random = new Random();

                int x = random.Next(30, width - 30);
                int y = random.Next(30, height - 30);

                point.X = x;
                point.Y = y;
                if (IsCircleValid(point, 15))
                    return new Point(x, y);
            }
        }
        // Отрисовка графа из файла
        private void DrawingZadannyiMassivGrapha(object sender, EventArgs e)
        {
            if (MainForm.TextGraph.Nodes == null || MainForm.TextGraph.Edges == null)
                return;

            graphNodes.Clear();
            graphEdges.Clear();

            foreach (GrafTextJson.Node node in MainForm.TextGraph.Nodes)
            {
                Point ef = GenerateRandomPoint();
                graphNodes.Add(new GraphNode { ID = node.Id, Location = new Point(ef.X, ef.Y) });
            }

            foreach (GrafTextJson.Edge edge in MainForm.TextGraph.Edges)
            {
                GraphNode foundItem1 = graphNodes.Find(item => item.ID == edge.Source);
                GraphNode foundItem2 = graphNodes.Find(item => item.ID == edge.Target);

                graphEdges.Add(new GraphEdge { StartNode = foundItem1, EndNode = foundItem2, _edgeDirection = edge.Direction, _weight = edge.Weight }); ;
            }

            graphCount = MainForm.TextGraph.VertexCount + 1;

            pictureBox1.Invalidate();
        }

        // Проверка графа на связность
        public static bool IsGraphConnected(List<GraphEdge> edges, int numVertices)
        {
            // Создаем список смежности на основе списка ребер
            Dictionary<int, List<int>> adjacencyList = new Dictionary<int, List<int>>();
            foreach (GraphEdge edge in edges)
            {
                if (!adjacencyList.ContainsKey(edge.StartNode.ID))
                    adjacencyList[edge.StartNode.ID] = new List<int>();
                if (!adjacencyList.ContainsKey(edge.EndNode.ID))
                    adjacencyList[edge.EndNode.ID] = new List<int>();

                adjacencyList[edge.StartNode.ID].Add(edge.EndNode.ID);
                adjacencyList[edge.EndNode.ID].Add(edge.StartNode.ID);
            }

            // Обход графа в глубину (DFS)
            HashSet<int> visited = new HashSet<int>();
            DFS(adjacencyList, 1, visited);

            // Проверяем, были ли все вершины посещены
            return visited.Count == numVertices;
        }

        private static void DFS(Dictionary<int, List<int>> adjacencyList, int vertex, HashSet<int> visited)
        {
            visited.Add(vertex);
            if (adjacencyList.ContainsKey(vertex))
            {
                foreach (int neighbor in adjacencyList[vertex])
                {
                    if (!visited.Contains(neighbor))
                    {
                        DFS(adjacencyList, neighbor, visited);
                    }
                }
            }
        }
        // true - неориентированный
        public static bool IsDirectedGraph()
        {
            foreach (GraphEdge edge in graphEdges)
            {
                if (edge._edgeDirection == true)
                    return false;
            }

            return true;
        }
    }
}