using System;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class MatricksGridViewControl : UserControl
    {
        public static bool isCompleteGraph; // Связен ли граф
        public static bool isDirectedGraph; // Ориентирован ли граф
        public static bool isConnectedGraph; //Связен ли граф
        public static int[,] graphMatrix; // Матрица смежности графа

        public MatricksGridViewControl()
        {
            InitializeComponent();

            MainForm.EvtntDataGridView += Button_Click;
            MainForm.FileGraphVisualized += PopulateAdjacencyMatrix;
        }

        public void Button_Click(object sender, EventArgs e)
        {
            TabPage selectedTabPage = tabControl1.SelectedTab;

            if (selectedTabPage == tabPage1)
            {
                if (dataGridView1.DataSource == null)
                {
                    graphMatrix = ReadDataGridView(dataGridView1);
                    isConnectedGraph = IsConnected(graphMatrix);
                    isDirectedGraph = IsDirectedGraph(graphMatrix);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            FillDataGridView(dataGridView1, (int)numericUpDown1.Value, (int)numericUpDown1.Value);
        }

        private void FillDataGridView(DataGridView dataGridView, int rows, int columns)
        {
            dataGridView.Rows.Clear();
            dataGridView.Columns.Clear();

            // Добавляем столбцы с номерами от 1 до columns
            for (int j = 1; j <= columns; j++)
            {
                dataGridView.Columns.Add($"Column{j}", j.ToString());
            }

            // Добавляем строки с номерами от 1 до rows
            for (int i = 1; i <= rows; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.HeaderCell.Value = i.ToString();

                for (int j = 0; j < columns; j++)
                {
                    row.Cells.Add(new DataGridViewTextBoxCell
                    {
                        Value = 1
                    });
                }

                dataGridView.Rows.Add(row);
            }
        }

        private int[,] ReadDataGridView(DataGridView dataGridView)
        {
            int[,] dataArray = new int[dataGridView.RowCount, dataGridView.ColumnCount];

            for (int row = 0; row < dataGridView.RowCount; row++)
            {
                for (int column = 0; column < dataGridView.ColumnCount; column++)
                {
                    int cellValue;
                    if (int.TryParse(dataGridView[column, row].Value.ToString(), out cellValue))
                    {
                        dataArray[row, column] = cellValue;
                    }
                    else
                    {
                        dataArray[row, column] = 0;
                    }
                }
            }

            return dataArray;
        }

        private bool IsConnected(int[,] graph)
        {
            int n = graph.GetLength(0);
            bool[] visited = new bool[n];

            DFS(graph, visited, 0); // начинаем обход из первой вершины

            // если есть непосещенные вершины, значит граф не связный
            for (int i = 0; i < n; i++)
            {
                if (!visited[i])
                {
                    return false;
                }
            }

            return true;
        }

        private void DFS(int[,] graph, bool[] visited, int vertex)
        {
            visited[vertex] = true;

            // обходим всех соседей вершины
            for (int i = 0; i < graph.GetLength(1); i++)
            {
                if (graph[vertex, i] != 0 && !visited[i])
                {
                    DFS(graph, visited, i);
                }
            }
        }
        // Проверка ориентированности графа
        private bool IsDirectedGraph(int[,] adjacencyMatrix)
        {
            int vertices = adjacencyMatrix.GetLength(0);

            for (int i = 0; i < vertices; i++)
            {
                for (int j = 0; j < vertices; j++)
                {
                    if (adjacencyMatrix[i, j] != adjacencyMatrix[j, i])
                        return true; // Если найдено ребро, несимметричное относительно диагонали, граф ориентированный
                }
            }

            return false;
        }
        // Проверка графа на полноту
        public bool IsCompleteGraph(int[,] adjacencyMatrix)
        {
            int vertices = adjacencyMatrix.GetLength(0);

            // Проверяем каждую пару вершин, кроме диагональных элементов
            for (int i = 0; i < vertices; i++)
            {
                for (int j = i + 1; j < vertices; j++)
                {
                    // Если существует незаполненное ребро (ноль в матрице смежности),
                    // то граф не является полным
                    if (adjacencyMatrix[i, j] == 0)
                    {
                        return false;
                    }
                }
            }

            // Все ребра заполнены, граф является полным
            return true;
        }

        private void PopulateAdjacencyMatrix(object sender, EventArgs e)
        {
            TabPage selectedTabPage = tabControl1.SelectedTab;

            if (selectedTabPage == tabPage1)
            {
                PopulateAdjacencyMatrix_ef(dataGridView1, MainForm.TextGraph.VertexCount);
            }
        }

        public void PopulateAdjacencyMatrix_ef(DataGridView dataGridView, int numVertices)
        {
            // Очищаем DataGridView перед заполнением
            dataGridView.Rows.Clear();
            dataGridView.Columns.Clear();

            // Создаем колонки и строки в DataGridView на основе количества вершин
            for (int i = 1; i <= MainForm.TextGraph.VertexCount; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.HeaderCell.Value = i.ToString();

                dataGridView.Columns.Add(i.ToString(), i.ToString());
                dataGridView.Rows.Add(row);
            }


            foreach (var weightedEdge in MainForm.TextGraph.Edges)
            {
                int source = weightedEdge.Source;
                int destination = weightedEdge.Target;
                int weight = weightedEdge.Weight;

                // Устанавливаем значение веса в соответствующую ячейку DataGridView                
                if (weightedEdge.Direction)
                {
                    dataGridView.Rows[source - 1].Cells[destination - 1].Value = weight;
                }
                else
                {
                    dataGridView.Rows[source - 1].Cells[destination - 1].Value = weight;
                    dataGridView.Rows[destination - 1].Cells[source - 1].Value = weight;
                }
            }

            // Заполняем нулями для отсутствующих ребер
            for (int i = 0; i < numVertices; i++)
            {
                for (int j = 0; j < numVertices; j++)
                {
                    if (dataGridView.Rows[i].Cells[j].Value == null)
                    {
                        dataGridView.Rows[i].Cells[j].Value = 0;
                    }
                }
            }
        }
    }
}