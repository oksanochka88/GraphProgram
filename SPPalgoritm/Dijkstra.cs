using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3
{
    class Dijkstra
    {
        public int size;
        private List<Tuple<int, int>>[] edges;

        public Dijkstra(int size)
        {
            this.size = size;
            edges = new List<Tuple<int, int>>[size];
            
            for (int i = 0; i < size; i++)
            {
                edges[i] = new List<Tuple<int, int>>();
            }
        }

        public void AddEdge(int u, int v, int weight)
        {
            edges[u].Add(new Tuple<int, int>(v, weight));
        }

        public List<int> ShortestPath(int start, int end)
        {
            int[] distances = new int[size];
            int[] previous = new int[size];
            bool[] visited = new bool[size];

            for (int i = 0; i < size; i++)
            {
                distances[i] = int.MaxValue;
                previous[i] = -1;
            }

            distances[start] = 0;

            for (int i = 0; i < size - 1; i++)
            {
                int u = MinDistance(distances, visited);
                visited[u] = true;

                foreach (Tuple<int, int> edge in edges[u])
                {
                    int v = edge.Item1;
                    int weight = edge.Item2;
                    int distanceThroughU = distances[u] + weight;

                    if (distanceThroughU < distances[v])
                    {
                        distances[v] = distanceThroughU;
                        previous[v] = u;
                    }
                }
            }

            return GetPath(previous, start, end);
        }

        private int MinDistance(int[] distances, bool[] visited)
        {
            int minDistance = int.MaxValue;
            int minIndex = -1;

            for (int i = 0; i < size; i++)
            {
                if (!visited[i] && distances[i] <= minDistance)
                {
                    minDistance = distances[i];
                    minIndex = i;
                }
            }

            return minIndex;
        }

        private List<int> GetPath(int[] previous, int start, int end)
        {
            List<int> path = new List<int>();
            int current = end;

            while (current != start)
            {
                path.Insert(0, current);
                current = previous[current];
            }

            path.Insert(0, start);
            return path;
        }
    }
}