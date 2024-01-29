using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.map.layers.elevation
{
    public partial class AssignCentreElevationLayer : MapGenLayer
    {
        public override void ProcessLayer(ref Graph graph, MapGenerationOptions options)
        {
            if (options.allowCliffs)
            {
                foreach (Centre centre in graph.centres)
                {
                    double totalElevation = 0;
                    foreach (Corner corner in centre.corners) totalElevation += corner.elevation;
                    centre.elevation = totalElevation / (double)centre.corners.Count;
                }
            }
            else
            {
                foreach (Centre centre in graph.centres)
                {
                    double lowestElevation = int.MaxValue;
                    foreach (Corner corner in centre.corners)
                    {
                        if (corner.elevation < lowestElevation)
                        {
                            lowestElevation = corner.elevation;
                        }
                    }
                    centre.elevation = lowestElevation;
                }
            }
        }
    }
}
