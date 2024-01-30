using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.map.layers.elevation
{
    public partial class RedistributeElevationLayer : MapGenLayer
    {
        public override void ProcessLayer(ref Graph graph, MapGenerationOptions options)
        {
            double scaleFactor = options.elevationScaleFactor;

            Corner[] cornerList = graph.corners.Values.Where(x => !x.waterFlags.HasFlag(WaterFlags.Water)).OrderBy(x => x.elevation).ToArray();
            for (int i = 0; i < cornerList.Length; i++)
            {
                double y = (double)i / (cornerList.Length - 1);
                double x = 1.04880885 - Math.Sqrt(scaleFactor * (1 - y));
                if (x > 1.0) x = 1.0;
                cornerList[i].elevation = x;
            }
        }
    }
}
