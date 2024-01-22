using InnoRPG.scripts.generation.map.data;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.map.layers.polygons
{
    public partial class RandomPointLayer : MapGenLayer
    {
        public override void ProcessLayer(ref Graph graph, MapGenerationOptions options)
        {
            graph.randomPoints = new();

            //Generate random points
            RandomNumberGenerator random = new();
            random.Seed = graph.mapSeed;
            for (int i = 0; i < options.voronoiPointCount; i++)
            {
                graph.randomPoints.Add(new Vector2(
                    random.RandfRange(0, graph.graphSize),
                    random.RandfRange(0, graph.graphSize)));
            }
        }
    }
}
