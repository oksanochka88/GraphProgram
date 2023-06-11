using System;
using System.Collections.Generic;

namespace popka
{
    public class Edge
    {
        public int City1 { get; set; }
        public int City2 { get; set; }
        public int Distance { get; set; }

        public Edge(int city1, int city2, int distance)
        {
            City1 = city1;
            City2 = city2;
            Distance = distance;
        }
    }

    public class TravelingSalesman
    {
        private int numCities;
        private List<Edge> edges;
        private List<int> bestPath;
        private int bestDistance;
        private int startCity;
        private int endCity;

        public TravelingSalesman(int numCities, List<Edge> edges, int startCity, int endCity)
        {
            this.numCities = numCities;
            this.edges = edges;
            this.bestPath = new List<int>();
            this.bestDistance = int.MaxValue;
            this.startCity = startCity;
            this.endCity = endCity;
        }

        public List<int> Solve()
        {
            List<int> currentPath = new List<int>();
            bool[] visitedCities = new bool[numCities + 1];

            currentPath.Add(startCity);
            visitedCities[startCity] = true;

            TSPRecursive(currentPath, visitedCities);

            if (bestPath.Count == 0)
            {
                throw new Exception("No feasible path exists.");
            }

            return bestPath;
        }

        private void TSPRecursive(List<int> currentPath, bool[] visitedCities)
        {
            if (currentPath.Count == numCities)
            {
                currentPath.Add(endCity);
                int totalDistance = CalculateTotalDistance(currentPath);

                if (totalDistance < bestDistance)
                {
                    bestPath = new List<int>(currentPath);
                    bestDistance = totalDistance;
                }

                currentPath.RemoveAt(currentPath.Count - 1); // Remove the end city
                return;
            }

            int lastCity = currentPath[currentPath.Count - 1];

            foreach (Edge edge in edges)
            {
                if (edge.City1 == lastCity && !visitedCities[edge.City2])
                {
                    currentPath.Add(edge.City2);
                    visitedCities[edge.City2] = true;

                    TSPRecursive(currentPath, visitedCities);

                    visitedCities[edge.City2] = false;
                    currentPath.RemoveAt(currentPath.Count - 1);
                }
                else if (edge.City2 == lastCity && !visitedCities[edge.City1])
                {
                    currentPath.Add(edge.City1);
                    visitedCities[edge.City1] = true;

                    TSPRecursive(currentPath, visitedCities);

                    visitedCities[edge.City1] = false;
                    currentPath.RemoveAt(currentPath.Count - 1);
                }
            }
        }

        public int CalculateTotalDistance(List<int> path)
        {
            int totalDistance = 0;

            for (int i = 0; i < path.Count - 1; i++)
            {
                int city1 = path[i];
                int city2 = path[i + 1];
                totalDistance += GetDistance(city1, city2);
            }

            return totalDistance;
        }

        private int GetDistance(int city1, int city2)
        {
            foreach (Edge edge in edges)
            {
                if ((edge.City1 == city1 && edge.City2 == city2) || (edge.City1 == city2 && edge.City2 == city1))
                {
                    return edge.Distance;
                }
            }

            throw new Exception("Нельзя проложить маршрут");
        }
    }
}