using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.map.layers.experiments
{
    public partial class E_MountainRoadLayer : MapGenLayer
    {
        public override void ProcessLayer(ref Graph graph, MapGenerationOptions options)
        {
            float targetRoadElevation = 6f;
            Corner startCorner = GetClosestTargetElevation(graph.corners.Values.Where(x => x.waterFlags == WaterFlags.None).ToList(), targetRoadElevation);
            List<Corner> usedCorners = new();

            Corner currentCorner = startCorner;
            while (currentCorner != null)
            {
                currentCorner.river = 1;
                usedCorners.Add(currentCorner);

                //Find next corner, if startCorner we finish
                currentCorner = GetClosestTargetElevation(currentCorner.adjacent.Where(x => !usedCorners.Contains(x)).ToList(), targetRoadElevation);

                if (currentCorner == null) break;

                Edge commonEdge = GetCommonEdge(currentCorner, usedCorners.Last());
                commonEdge.river = 1;

                if (currentCorner == startCorner) currentCorner = null;
            }
        }

        public Corner GetClosestTargetElevation(List<Corner> corners, float targetElevation) =>
            corners.Aggregate(null, (Corner x, Corner n) => x = x == null ? n 
                : MathF.Abs((float)n.elevation - targetElevation) < MathF.Abs((float)x.elevation - targetElevation) ? n : x);

        public Edge GetCommonEdge(Corner a, Corner b) =>
            a.protrudes.FirstOrDefault(x => b.protrudes.Contains(x));
    }
}
