using GrafTextJson;
using Newtonsoft.Json;
using popka;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class MainForm : Form
    {
        public static GrafTextJson.Graph TextGraph = new Graph(); // Граф экспорт/импорт-ируемый в/из файл

        public static event EventHandler EvtntDataGridView;
        public static event EventHandler FileGraphVisualized;

        protected virtual void EtntDataGridView(EventArgs e)
        {
            EvtntDataGridView?.Invoke(this, e);
        }

        protected virtual void EventFileGraphVisualized(EventArgs e)
        {
            FileGraphVisualized?.Invoke(this, e);
        }

        public MainForm()
        {
            InitializeComponent();

            radioButton3_CheckedChanged(null, null);
        }
        // SPP
        private void SPPSearchButton_Click(object sender, EventArgs e)
        {
            int startNode = 0;
            int endNode = 0;

            try
            {
                startNode = Convert.ToInt32(textBox6.Text);
                endNode = Convert.ToInt32(textBox7.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            textBox4.Clear();
            //+
            if (radioButton4.Checked)
            {
                EtntDataGridView(EventArgs.Empty);

                int[,] adjacencyMatrix = MatricksGridViewControl.graphMatrix;
                int p = adjacencyMatrix.GetLength(0);
                List<int> path = new List<int>();

                if (startNode < 1 || startNode > p || endNode < 1 || endNode > p || startNode == endNode)
                {
                    MessageBox.Show("Вершины выбраны некорректно");
                    return;
                }

                Dijkstra graph = new Dijkstra(p + 1);

                for (int i = 0; i < p; i++)
                {
                    for (int j = 0; j < p; j++)
                    {
                        int weight = adjacencyMatrix[i, j];
                        if (weight != 0)
                        {
                            graph.AddEdge(i + 1, j + 1, weight);
                        }
                    }
                }

                try
                {
                    path = graph.ShortestPath(startNode, endNode);
                }
                catch
                {
                    MessageBox.Show("Такого пути не существует", "Warnigs!");
                    return;
                }

                textBox4.Text += "ShortedPath: \r\n";

                textBox4.Text += (string.Join(" -> ", path));
            }
            //+
            if (radioButton3.Checked)
            {
                if (startNode < 1 || startNode > VisualGraphControl.graphNodes.Count || endNode < 1 || endNode > VisualGraphControl.graphNodes.Count || startNode == endNode)
                {
                    MessageBox.Show("Вершины выбраны некорректно");
                    return;
                }
                List<int> path = new List<int>();

                Dijkstra graph = new Dijkstra(VisualGraphControl.graphNodes.Count + 1);

                foreach (GraphEdge edge in VisualGraphControl.graphEdges)
                {
                    if (edge._edgeDirection)
                        graph.AddEdge(edge.StartNode.ID, edge.EndNode.ID, edge._weight);
                    else
                    {
                        graph.AddEdge(edge.StartNode.ID, edge.EndNode.ID, edge._weight);
                        graph.AddEdge(edge.EndNode.ID, edge.StartNode.ID, edge._weight);
                    }
                }

                try
                {
                    path = graph.ShortestPath(startNode, endNode);

                    textBox4.Text += "ShortedPath: \r\n";
                    textBox4.Text += (string.Join(" -> ", path));
                }
                catch
                {
                    MessageBox.Show("Такого пути не существует");
                }
            }
        }
        // MST (+/-), граф неориентированный, связный
        private void MSTSearchButton_Click(object sender, EventArgs e)
        {
            textBox4.Clear();

            if (radioButton4.Checked)
            {
                EtntDataGridView(EventArgs.Empty);

                if (MatricksGridViewControl.isConnectedGraph == false || MatricksGridViewControl.isDirectedGraph == true)
                {
                    MessageBox.Show("Граф не соответствует требованиям");
                    return;
                }

                int totalsum = 0;
                List<Kruskal.Edge> mst = new List<Kruskal.Edge>();
                List<Kruskal.Edge> edges = new List<Kruskal.Edge>();

                int[,] adjacencyMatrix = MatricksGridViewControl.graphMatrix;
                if (adjacencyMatrix == null)
                    return;
                int p = adjacencyMatrix.GetLength(0);

                List<GrafTextJson.Edge> edges_3 = GetUniqueDirectedEdgesFromAdjacencyMatrix(adjacencyMatrix);

                foreach (GrafTextJson.Edge edge in edges_3)
                {
                    Kruskal.Edge edge_por = new Kruskal.Edge(edge.Source, edge.Target, edge.Weight);
                    edges.Add(edge_por);

                }

                // Создаем объект класса KruskalAlgorithm и вызываем метод Kruskal
                Kruskal.KruskalAlgorithm kruskal = new Kruskal.KruskalAlgorithm();
                try
                {
                    mst = kruskal.Kruskal(p + 1, edges);
                }
                catch
                {
                    MessageBox.Show("Error");
                }

                textBox4.Text += "MST: \r\n";

                foreach (Kruskal.Edge edge in mst)
                {
                    textBox4.Text += (edge.source + " - " + edge.dest + " (" + edge.weight + ") \r\n");
                    totalsum += edge.weight;
                }
                textBox4.Text += "Total sum - " + Convert.ToString(totalsum);
            }

            if (radioButton3.Checked)
            {
                if (VisualGraphControl.IsDirectedGraph() == false || VisualGraphControl.IsGraphConnected(VisualGraphControl.graphEdges, VisualGraphControl.graphNodes.Count) == false)
                {
                    MessageBox.Show("Граф не соответствует требемому формату, для нахождения MST он должент быть: \r\n 1) Неориентированный \r\n 2) Связный", "Warnings!");

                    return;
                }

                List<Kruskal.Edge> mst = new List<Kruskal.Edge>();
                List<Kruskal.Edge> edges = new List<Kruskal.Edge>();

                foreach (GraphEdge edge in VisualGraphControl.graphEdges)
                {
                    Kruskal.Edge edge_f = new Kruskal.Edge(edge.StartNode.ID, edge.EndNode.ID, edge._weight);

                    edges.Add(edge_f);
                }

                Kruskal.KruskalAlgorithm kruskal = new Kruskal.KruskalAlgorithm();

                try
                {
                    mst = kruskal.Kruskal(VisualGraphControl.graphNodes.Count + 1, edges);
                }
                catch
                {
                    MessageBox.Show("Error");
                }


                int totalsum = 0;

                textBox4.Text += "MST: \r\n";

                foreach (Kruskal.Edge edge in mst)
                {
                    textBox4.Text += (edge.source + " - " + edge.dest + " (" + edge.weight + ") \r\n");
                    totalsum += edge.weight;
                }

                textBox4.Text += "Total sum - " + Convert.ToString(totalsum);
            }
        }
        // TSP, симметричная
        private void TSPSearchButton_Click(object sender, EventArgs e)
        {
            textBox4.Clear();

            int statNode = 0;

            try
            {
                statNode = Convert.ToInt32(textBox5.Text);
            }
            catch (Exception v)
            {
                MessageBox.Show(v.Message);
                return;
            }

            if (radioButton4.Checked)
            {
                EtntDataGridView(EventArgs.Empty);

                int[,] adjacencyMatrix = MatricksGridViewControl.graphMatrix;
                if (adjacencyMatrix == null)
                    return;
                int numVertices = adjacencyMatrix.GetLength(0);
                List<popka.Edge> edges = new List<popka.Edge>();

                if (MatricksGridViewControl.isConnectedGraph == false || MatricksGridViewControl.isDirectedGraph == true)
                {
                    MessageBox.Show("Граф не соответствует требованиям");
                    return;
                }
                if (statNode < 1 || statNode > numVertices)
                {
                    MessageBox.Show("Вершина выбрана некорректно!");
                    return;
                }

                for (int i = 0; i < numVertices; i++)
                {
                    for (int j = 0; j < numVertices; j++)
                    {
                        int weight = adjacencyMatrix[i, j];

                        if (weight != 0)
                        {
                            edges.Add(new popka.Edge(i + 1, j + 1, weight));
                        }
                    }
                }
                try
                {
                    TravelingSalesman tsp = new TravelingSalesman(numVertices, edges, statNode, statNode);
                    List<int> bestPath = tsp.Solve();

                    textBox4.Text += ("Best path: " + string.Join(" -> ", bestPath) + "\r\n");
                    textBox4.Text += ("Total distance: " + tsp.CalculateTotalDistance(bestPath));
                }
                catch (Exception ex)
                {
                    textBox4.Text += ("Error: " + ex.Message);
                }

                // создаем экземпляр класса NearestNeighborSolver
                //NearestNeighborSolver.NearestNeighborSolver solver = new NearestNeighborSolver.NearestNeighborSolver(edges, numVertices);

                //// решаем задачу коммивояжера, начиная с вершины 1
                //List<int> path = solver.Solve(statNode);

                //// выводим оптимальный путь
                //textBox4.Text += ("NearestNeighborSolver:" + "\r\n");
                //textBox4.Text += ("Optimal path: " + string.Join(" -> ", path));
            }

            if (radioButton3.Checked)
            {
                if (VisualGraphControl.IsDirectedGraph() == false || VisualGraphControl.IsGraphConnected(VisualGraphControl.graphEdges, VisualGraphControl.graphNodes.Count) == false)
                {
                    MessageBox.Show("Граф не соответствует требемому формату, он должент быть: \r\n 1) Неориентированный \r\n 2) Связный", "Warnings!");

                    return;
                }

                if (statNode < 1 || statNode > VisualGraphControl.graphNodes.Count)
                {
                    MessageBox.Show("Вершина выбрана некорректно!");
                    return;
                }

                List<popka.Edge> edges = new List<popka.Edge>();

                foreach (GraphEdge edge in VisualGraphControl.graphEdges)
                {
                    edges.Add(new popka.Edge(edge.StartNode.ID, edge.EndNode.ID, edge._weight));
                }

                try
                {
                    TravelingSalesman tsp = new TravelingSalesman(VisualGraphControl.graphNodes.Count, edges, statNode, statNode);
                    List<int> bestPath = tsp.Solve();

                    textBox4.Text += ("Best path: " + string.Join(" -> ", bestPath) + "\r\n");
                    textBox4.Text += ("Total distance: " + tsp.CalculateTotalDistance(bestPath));
                }
                catch (Exception ex)
                {
                    textBox4.Text += ("Error: " + ex.Message);
                }
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            visualGraphControl1.Enabled = true;
            matricksGridViewControl1.Enabled = false;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            matricksGridViewControl1.Enabled = true;
            visualGraphControl1.Enabled = false;
        }

        private GrafTextJson.Graph ReadGraphFromJson(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                GrafTextJson.Graph graph = JsonConvert.DeserializeObject<Graph>(json);
                return graph;
            }
            catch (JsonReaderException ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private void WriteGraphToJson(Graph graph, string jsonFilePath)
        {
            string json = JsonConvert.SerializeObject(graph, Formatting.Indented);
            try
            {
                File.WriteAllText(jsonFilePath, json);
            }
            catch (JsonReaderException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ImportGrahToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON Files (*.json)|*.json";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    TextGraph = ReadGraphFromJson(filePath);

                    MessageBox.Show("Граф успешно загружен!");

                    if (TextGraph.VertexCount <= 50)
                        EventFileGraphVisualized(EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при чтении файла: " + ex.Message);
                }
            }
        }

        private void ExportGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextGraph.Nodes.Clear();
            TextGraph.Edges.Clear();

            if (radioButton3.Checked)
            {
                TextGraph.VertexCount = VisualGraphControl.graphNodes.Count;

                foreach (GraphNode node in VisualGraphControl.graphNodes)
                {
                    TextGraph.Nodes.Add(new GrafTextJson.Node { Id = node.ID });
                }

                foreach (GraphEdge edge in VisualGraphControl.graphEdges)
                {
                    TextGraph.Edges.Add(new GrafTextJson.Edge { Source = edge.StartNode.ID, Target = edge.EndNode.ID, Direction = edge._edgeDirection, Weight = edge._weight });
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "JSON Files (*.json)|*.json";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    try
                    {
                        WriteGraphToJson(TextGraph, filePath);
                        MessageBox.Show("Граф успешно сохранен!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при записи файла: " + ex.Message);
                    }
                }
            }

            if (radioButton4.Checked)
            {
                EtntDataGridView(EventArgs.Empty);
                int[,] adjacencyMatrix = MatricksGridViewControl.graphMatrix;
                if (adjacencyMatrix == null)
                    return;

                int p = adjacencyMatrix.GetLength(0);
                TextGraph.VertexCount = p;

                for (int i = 1; i <= p; i++)
                {
                    TextGraph.Nodes.Add(new GrafTextJson.Node { Id = i });
                }

                List<GrafTextJson.Edge> edges = GetUniqueDirectedEdgesFromAdjacencyMatrix(adjacencyMatrix);

                foreach (GrafTextJson.Edge edge in edges)
                {
                    TextGraph.Edges.Add(new GrafTextJson.Edge { Source = edge.Source, Target = edge.Target, Direction = edge.Direction, Weight = edge.Weight });
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "JSON Files (*.json)|*.json";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    try
                    {
                        WriteGraphToJson(TextGraph, filePath);
                        MessageBox.Show("Граф успешно сохранен!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при записи файла: " + ex.Message);
                    }
                }
            }
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
            "Программа разработана студентом IVT3_21 \r\nСтрекаловым Романом Павловичем, в рамках курсового проекта на тему: Решение крмбинаторно-оптимизационых задач.",
            "О программе");
        }
        // Вроде работает
        public List<GrafTextJson.Edge> GetUniqueDirectedEdgesFromAdjacencyMatrix(int[,] adjacencyMatrix)
        {
            List<GrafTextJson.Edge> edges = new List<GrafTextJson.Edge>();

            int numVertices = adjacencyMatrix.GetLength(0);

            for (int i = 0; i < numVertices; i++)
            {
                for (int j = 0; j < numVertices; j++)
                {
                    int weight = adjacencyMatrix[i, j];

                    if (weight != 0)
                    {
                        if (adjacencyMatrix[i, j] != adjacencyMatrix[j, i] && adjacencyMatrix[i, j] == 0 || adjacencyMatrix[j, i] == 0)
                        {
                            GrafTextJson.Edge edge = new GrafTextJson.Edge()
                            {
                                Source = i + 1, // Вершины начинаются с 1
                                Target = j + 1, // Вершины начинаются с 1
                                Weight = weight,
                                Direction = true // Ориентированное ребро
                            };
                            edges.Add(edge);
                        }
                        else
                        {
                            GrafTextJson.Edge edge = new GrafTextJson.Edge()
                            {
                                Source = i + 1, // Вершины начинаются с 1
                                Target = j + 1, // Вершины начинаются с 1
                                Weight = weight,
                                Direction = false // Ориентированное ребро
                            };

                            GrafTextJson.Edge foundItem1 = edges.Find(item => item.Source == i + 1 && item.Target == j + 1);
                            GrafTextJson.Edge foundItem2 = edges.Find(item => item.Source == j + 1 && item.Target == i + 1);

                            if (foundItem1 == null && foundItem2 == null)
                            {
                                edges.Add(edge);
                            }
                        }

                    }
                }
            }
            //foreach (GrafTextJson.Edge edgewqw in edges)
            //{

            //}
            return edges;
        }
    }
}