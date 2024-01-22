using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoronatorSharp;

namespace InnoRPG.scripts.generation.map.layers.polygons
{
    public partial class LloydRelaxationLayer : MapGenLayer
    {
        public override void ProcessLayer(ref Graph graph, MapGenerationOptions options)
        {
            for (int i = 0; i < options.lloydsAlgorithmIterations; i++)
            {
                //Lloyd relaxation
                Voronator voronoi = new(
                    graph.randomPoints.Select(x => new Vector2(x.X, x.Y)).ToList(),
                    Vector2.zero,
                    new Vector2(options.worldSize, options.worldSize));

                for (int j = 0; j < graph.randomPoints.Count; j++)
                {
                    List<Vector2> region = voronoi.GetClippedPolygon(j);
                    if (region == null) continue;

                    Vector2 point = new();
                    foreach (Vector2 p in region) point += p;
                    point /= region.Count;
                    graph.randomPoints[j] = new Godot.Vector2(point.x, point.y);
                }
            }
        }
    }
}
