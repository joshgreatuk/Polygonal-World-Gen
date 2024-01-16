using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.map
{
    public class Edge
    {
        public Centre d0;
        public Centre d1;

        public Corner v0;
        public Corner v1;

        public Edge(Centre d0, Centre d1, Corner v0, Corner v1)
        {
            this.d0 = d0;
            this.d1 = d1;
            this.v0 = v0;
            this.v1 = v1;
        }

        public Corner[] Corners { get { return new[] { v0, v1 }; } }

        public override bool Equals(object? obj)
        {
            if (obj != null && obj is Edge edge)
            {
                return edge.v0.Equals(v0) && edge.v1.Equals(v1) || edge.v1.Equals(v0) && edge.v0.Equals(v1);
            }
            return false;
        }
    }
}
