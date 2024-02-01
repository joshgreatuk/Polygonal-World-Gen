using Godot;
using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corner = InnoRPG.scripts.generation.map.data.Corner;

namespace InnoRPG.scripts.generation.map.layers.experiments.elevation
{
    public class E_NoiseElevation : MapGenLayer
    {
        public override void ProcessLayer(ref Graph graph, MapGenerationOptions options)
        {
            options.noise.Seed = (int)graph.mapSeed;

            foreach (Corner corner in graph.corners.Values.Where(x => !x.waterFlags.HasFlag(WaterFlags.Ocean)))
            {
                if (corner.waterFlags.HasFlag(WaterFlags.Coast))
                {
                    //Decide if this coast will be beach or cliff
                    return;
                }

                
            }
        }
    }
}
