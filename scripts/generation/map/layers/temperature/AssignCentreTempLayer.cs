using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.map.layers.temperature
{
    public partial class AssignCentreTempLayer : MapGenLayer
    {
        public override void ProcessLayer(ref Graph graph, MapGenerationOptions options)
        {
            foreach (Centre centre in graph.centres)
            {
                double totalTemp = 0;
                foreach (Corner corner in centre.corners) totalTemp += corner.temperature;
                centre.temperature = totalTemp / centre.corners.Count;
            }
        }
    }
}
