using System;
using System.Collections.Generic;

/// Хороший понятный алгоритм, граф неориентированный, взвешенный.

namespace Kruskal
{
    public class Edge : IComparable<Edge>
    {
        public int source;
        public int dest;
        public int weight;

        public Edge(int source, int dest, int weight)
        {
            this.source = source;
            this.dest = dest;
            this.weight = weight;
        }

        // Метод для сравнения ребер по весу
        public int CompareTo(Edge other)
        {
            return this.weight - other.weight;
        }
    }

    public class KruskalAlgorithm
    {
        private int[] parent;

        // Метод для нахождения корня дерева
        private int Find(int vertex)
        {
            while (vertex != parent[vertex])
            {
                parent[vertex] = parent[parent[vertex]];
                vertex = parent[vertex];
            }
            return vertex;
        }

        // Метод для объединения двух деревьев
        private void Union(int source, int dest)
        {
            int rootSource = Find(source);
            int rootDest = Find(dest);

            parent[rootSource] = rootDest;
        }

        // Метод для построения минимального остовного дерева
        public List<Edge> Kruskal(int n, List<Edge> edges)
        {
            List<Edge> mst = new List<Edge>();
            parent = new int[n];

            // Инициализируем каждый узел как отдельное дерево
            for (int i = 0; i < n; i++)
            {
                parent[i] = i;
            }

            // Сортируем ребра по возрастанию веса
            edges.Sort();

            // Проходим по каждому ребру в отсортированном списке
            foreach (Edge edge in edges)
            {
                int source = edge.source;
                int dest = edge.dest;

                // Если ребро не создает цикл, то добавляем его в остовное дерево
                if (Find(source) != Find(dest))
                {
                    mst.Add(edge);
                    Union(source, dest);
                }
            }

            return mst;
        }
    }
}