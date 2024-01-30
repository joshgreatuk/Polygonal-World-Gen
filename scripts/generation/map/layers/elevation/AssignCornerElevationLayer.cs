using Godot;
using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corner = InnoRPG.scripts.generation.map.data.Corner;

namespace InnoRPG.scripts.generation.map.layers.elevation
{
    public partial class AssignCornerElevationLayer : MapGenLayer
    {
        public override void ProcessLayer(ref Graph graph, MapGenerationOptions options)
        {
            foreach (Corner corner in graph.corners.Values.Where(x => x.IsBorder || x.waterFlags.HasFlag(WaterFlags.Ocean)))
            {
                corner.elevation = 0;
            }

            RandomNumberGenerator random = new();
            random.Seed = graph.mapSeed;

            foreach (Corner corner in graph.corners.Values.Where(x => !x.waterFlags.HasFlag(WaterFlags.Ocean)))
            {
                Queue<Corner> queue = new();
                Queue<Corner> nextQueue = new();
                List<Corner> checkedCorners = new();
                bool coastFound = false;
                double distanceFromCoast = 1;

                queue.Enqueue(corner);

                int iterationsNeeded = 0;

                while (!coastFound)
                {
                    if (queue.Count < 1)
                    {
                        if (nextQueue.Count < 1) break;
                        foreach (Corner nextCorner in nextQueue) queue.Enqueue(nextCorner);
                        nextQueue.Clear();
                        distanceFromCoast++;
                    }

                    Corner currentCorner = queue.Dequeue();
                    if (currentCorner.waterFlags.HasFlag(WaterFlags.Coast))
                    {
                        coastFound = true;
                        break;
                    }

                    foreach (Corner adjacent in currentCorner.adjacent)
                    {
                        if (!checkedCorners.Contains(adjacent))
                        {
                            nextQueue.Enqueue(adjacent);
                        }
                    }

                    checkedCorners.Add(currentCorner);
                    iterationsNeeded++;
                }
                GD.Print($"iterationsNeeded: {iterationsNeeded}");
                //Add randomness
                corner.distanceFromCoast = distanceFromCoast;
                distanceFromCoast += random.Randf();

                corner.elevation = distanceFromCoast;
            }
        }
    }
}
