using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.map.layers.experiments.elevation
{
    public class E_StepRounding : MapGenLayer
    {
        public override void ProcessLayer(ref Graph graph, MapGenerationOptions options)
        {
            foreach (Corner corner in graph.corners.Values)
            {
                corner.elevation = Math.Round(corner.elevation / options.stepValue) * options.stepValue;
            }
        }
    }
}
