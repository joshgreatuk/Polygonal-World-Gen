using Godot;
using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corner = InnoRPG.scripts.generation.map.data.Corner;

namespace InnoRPG.scripts.generation.map.layers.rivers
{
    public partial class GenerateRiversLayer : MapGenLayer
    {
        public override void ProcessLayer(ref Graph graph, MapGenerationOptions options)
        {
            RandomNumberGenerator random = new();
            random.Seed = graph.mapSeed;

            for (int i=0; i < options.riverIterations; i++)
            {
                Corner corner = graph.corners[random.RandiRange(0, graph.corners.Count - 1)];
                if (corner.waterFlags.HasFlag(WaterFlags.Ocean) || corner.elevation < 0.3 || corner.elevation < 0.9) continue;

                while (!corner.waterFlags.HasFlag(WaterFlags.Coast))
                { 
                    if (corner == corner.downSlope || corner.downSlope == null)
                    {
                        break;
                    }

                    Edge edge = corner.protrudes.FirstOrDefault(x => x.v0 == corner.downSlope | x.v1 == corner.downSlope);

                    edge.river++;
                    corner.river++;
                    corner.downSlope.river++;
                    corner = corner.downSlope;
                }
            }
        }
    }
}
