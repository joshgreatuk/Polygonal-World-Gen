﻿using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        public List<Vector2> cornerPositions = new();
        public Dictionary<Vector2, Corner> corners = new();

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
            minElevation = centres.Min(x => x.elevation);
            maxElevation = centres.Where(x => x.elevation != Mathf.Inf).Max(x => x.elevation);

            minTemperature = centres.Min(x => x.temperature);
            maxTemperature = centres.Max(x => x.temperature);

            limitsCalculated = true;
        }
    }
}
