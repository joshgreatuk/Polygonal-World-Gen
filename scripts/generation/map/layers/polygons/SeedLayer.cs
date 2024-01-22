using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.map.layers.polygons
{
    public partial class SeedLayer : MapGenLayer
    {
        public override void ProcessLayer(ref Graph graph, MapGenerationOptions options)
        {
            graph.mapSeed = (ulong)DateTime.Now.Millisecond;
        }
    }
}
