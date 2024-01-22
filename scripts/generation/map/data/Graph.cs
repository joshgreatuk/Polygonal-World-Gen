using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.map.data
{
    public class Graph
    {
        public List<Vector2> randomPoints;

        public ulong mapSeed;
        public int graphSize;

        public List<Centre> centres = new();
        public List<Edge> edges = new();
        public List<Corner> corners = new();

        public bool limitsCalculated = false;

        public double minElevation;
        public double maxElevation;

        public double minTemperature;
        public double maxTemperature;

        public Graph(int graphSize)
        {
            this.graphSize = graphSize;
        }

        public void CalculateGraphLimits()
        {
            minElevation = corners.Min(x => x.elevation);
            maxElevation = corners.Max(x => x.elevation);

            minTemperature = corners.Min(x => x.temperature);
            maxTemperature = corners.Max(x => x.temperature);

            limitsCalculated = true;
        }
    }
}
