using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.map.layers.experiments
{
    public class E_CurveAdjustment : MapGenLayer
    {
        public override void ProcessLayer(ref Graph graph, MapGenerationOptions options)
        {
            foreach (Corner corner in graph.corners.Where(x => !x.waterFlags.HasFlag(WaterFlags.Ocean)))
            {
                float factor = 0.5f + (float)(corner.elevation / graph.maxElevation);
                corner.elevation *= options.elevationCurve.SampleBaked(factor);
            }
        }
    }
}
