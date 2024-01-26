using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.map.layers.rivers
{
    public partial class CalculateDownslopeLayer : MapGenLayer
    {
        public override void ProcessLayer(ref Graph graph, MapGenerationOptions options)
        {
            foreach (Corner corner in graph.corners.Where(x => !x.waterFlags.HasFlag(WaterFlags.Water)))
            {
                Corner downslope = corner;
                foreach (Corner adjacent in corner.adjacent)
                {
                    if (adjacent.elevation <= downslope.elevation)
                    {
                        downslope = adjacent;
                    }
                }
                corner.downSlope = downslope;
            }
        }
    }
}
