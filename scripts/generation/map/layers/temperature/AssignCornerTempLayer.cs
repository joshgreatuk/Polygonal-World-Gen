using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.map.layers.temperature
{
    public partial class AssignCornerTempLayer : MapGenLayer
    {
        public override void ProcessLayer(ref Graph graph, MapGenerationOptions options)
        {
            //TO-DO: Add equator position
            foreach (Corner corner in graph.corners)
            {
                double yFactor = corner.position.Y / (options.worldSize / 2);
                if (yFactor < 1)
                {
                    if (options.nTemp < options.equatorTemp)
                    {
                        corner.temperature = options.nTemp + ((options.equatorTemp - options.nTemp) * yFactor);
                    }
                    else
                    {
                        yFactor = 1 - yFactor;
                        corner.temperature = options.equatorTemp + ((options.nTemp - options.equatorTemp) * yFactor);
                    }
                }
                else
                {
                    yFactor -= 1;
                    if (options.sTemp < options.equatorTemp)
                    {
                        yFactor = 1 - yFactor;
                        corner.temperature = options.sTemp + ((options.equatorTemp - options.sTemp) * yFactor);
                    }
                    else
                    {
                        corner.temperature = options.equatorTemp + ((options.sTemp - options.equatorTemp) * yFactor);
                    }
                }
            }
        }
    }
}
