using Godot;
using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corner = InnoRPG.scripts.generation.map.data.Corner;

namespace InnoRPG.scripts.generation.map.layers.water
{
    public partial class AssignOceanLayer : MapGenLayer
    {
        public override void ProcessLayer(ref Graph graph, MapGenerationOptions options)
        {
            foreach (Centre centre in graph.centres.Where(x => x.IsBorder))
            {
                if (centre.waterFlags.HasFlag(WaterFlags.Ocean) || !centre.waterFlags.HasFlag(WaterFlags.Water)) continue;

                List<Centre> q = new();
                q.Add(centre);
                while (q.Count > 0)
                {
                    Centre n = q.First();
                    q.RemoveAt(0);

                    if (n.waterFlags.HasFlag(WaterFlags.Ocean)) continue;

                    if (n.waterFlags.HasFlag(WaterFlags.Water))
                    {
                        n.waterFlags |= WaterFlags.Ocean;
                        q.AddRange(n.neighbours);
                    }
                    else
                    {
                        n.waterFlags |= WaterFlags.Coast;
                    }
                }
            }

            foreach (Corner corner in graph.corners.Values)
            {
                if (corner.touches.All(x => x.waterFlags.HasFlag(WaterFlags.Ocean)))
                {
                    corner.waterFlags |= WaterFlags.Ocean;
                }
                else if (corner.touches.Any(x => x.waterFlags.HasFlag(WaterFlags.Ocean))
                    && corner.touches.Any(x => !x.waterFlags.HasFlag(WaterFlags.Water)))
                {
                    corner.waterFlags |= WaterFlags.Coast;
                    corner.waterFlags &= ~WaterFlags.Water;
                }
                else
                {
                    corner.waterFlags &= ~WaterFlags.Water;
                }
            }
        }
    }
}
