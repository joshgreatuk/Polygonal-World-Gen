using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using Corner = InnoRPG.scripts.generation.map.data.Corner;
using System.ComponentModel.DataAnnotations;

namespace InnoRPG.scripts.generation.map.layers.elevation
{
    public class BreadthSearchCornerElevationLayer : MapGenLayer
    {
        public override void ProcessLayer(ref Graph graph, MapGenerationOptions options)
        {
            Queue<Corner> queue = new();
            double minElevation = 1;
            double maxElevation = 1;

            foreach (Corner corner in graph.corners.Values)
            {
                if (corner.waterFlags.HasFlag(WaterFlags.Coast))
                {
                    queue.Enqueue(corner);
                    corner.elevation = 0;
                }
                else
                {
                    corner.elevation = Mathf.Inf;
                }
            }

            RandomNumberGenerator random = new();
            random.Seed = graph.mapSeed;

            while (queue.TryDequeue(out Corner corner))
            {
                int offset = random.RandiRange(0, corner.adjacent.Count);

                for (int i=0; i < corner.adjacent.Count; i++)
                {
                    Edge edge = corner.protrudes[(i + offset) % corner.adjacent.Count];
                    Corner neighbour = edge.v0 == corner ? edge.v1 : edge.v0;
                    double newElevation = (edge.IsEdgeLake() ? 0 : 1) + corner.elevation;

                    if (newElevation < neighbour.elevation)
                    {
                        neighbour.elevation = neighbour.waterFlags.HasFlag(WaterFlags.Ocean) ? 0 : newElevation;
                        neighbour.downSlope = corner;

                        if (neighbour.waterFlags.HasFlag(WaterFlags.Ocean) && newElevation > minElevation) minElevation = newElevation;
                        if (!neighbour.waterFlags.HasFlag(WaterFlags.Ocean) && newElevation > maxElevation) maxElevation = newElevation;

                        if (edge.IsEdgeLake())
                        {
                            queue.Prepend(neighbour);
                        }
                        else
                        {
                            queue.Enqueue(neighbour);
                        }
                    }
                }
            }

            foreach (Corner corner in graph.corners.Values.Where(x => x.elevation == Mathf.Inf))
            {
                corner.elevation = corner.adjacent.Where(x => x.elevation != Mathf.Inf).Average(x => x.elevation);
            }

            //Add randomness
            foreach (Corner corner in graph.corners.Values.Where(x => !x.waterFlags.HasFlag(WaterFlags.Ocean)))
            {
                corner.elevation += random.Randf();
            }

            graph.minElevation = minElevation;
            graph.maxElevation = maxElevation;
        }
    }
}
