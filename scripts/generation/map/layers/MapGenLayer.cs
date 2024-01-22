using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.map.layers
{
    public abstract partial class MapGenLayer
    {
        public abstract void ProcessLayer(ref Graph graph, MapGenerationOptions options);
    }
}
