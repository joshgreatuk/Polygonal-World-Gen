using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.map
{
    public class Graph
    {
        public int graphSize;

        public List<Centre> centres = new();
        public List<Edge> edges = new();
        public List<Corner> corners = new();
        
        public Graph(int graphSize)
        {
            this.graphSize = graphSize;
        }
    }
}
