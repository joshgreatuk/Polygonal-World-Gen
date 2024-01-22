using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoronatorSharp;

namespace InnoRPG.scripts.generation.map.layers.polygons
{
    public partial class GeneratePolygonLayer : MapGenLayer
    {
        public override void ProcessLayer(ref Graph graph, MapGenerationOptions options)
        {
            //Generate polygons from random points, clear random points
            Voronator voronator = new(
                graph.randomPoints.Select(x => new Vector2(x.X, x.Y)).ToList(),
                Vector2.zero,
                new Vector2(options.worldSize, options.worldSize));

            for (int i = 0; i < options.voronoiPointCount; i++)
            {
                Centre centre = new(graph.randomPoints[i]);
                foreach (Vector2 cornerPos in voronator.GetClippedPolygon(i))
                {
                    if (cornerPos == null) continue;
                    //Clamp corner to world border
                    Vector2 clampedCorner = cornerPos;
                    if (cornerPos.x < 0) clampedCorner.x = 0;
                    if (cornerPos.x > options.worldSize) clampedCorner.x = options.worldSize;
                    if (cornerPos.y < 0) clampedCorner.y = 0;
                    if (cornerPos.y > options.worldSize) clampedCorner.y = options.worldSize;

                    Corner corner = new(new Godot.Vector2(clampedCorner.x, clampedCorner.y), options.worldSize);
                    Corner existingCorner = graph.corners.FirstOrDefault(x => x.Equals(corner));
                    if (existingCorner != null) corner = existingCorner;
                    else graph.corners.Add(corner);

                    centre.corners.Add(corner);
                    corner.touches.Add(centre);
                }
                graph.centres.Add(centre);

                for (int j = 0; j < centre.corners.Count; j++)
                {
                    //Generate edges between corners
                    Edge newEdge = null;
                    if (j == centre.corners.Count - 1)
                    {
                        newEdge = new Edge(centre, null, centre.corners.Last(), centre.corners.First());
                    }
                    else
                    {
                        newEdge = new Edge(centre, null, centre.corners[j], centre.corners[j + 1]);
                    }

                    //Check if the edge already exists
                    Edge existingEdge = graph.edges.FirstOrDefault(x => x.Equals(newEdge));
                    if (existingEdge != null)
                    {
                        newEdge = existingEdge;
                        newEdge.d1 = centre;
                        if (!centre.neighbours.Contains(newEdge.d0)) centre.neighbours.Add(newEdge.d0);
                        if (!newEdge.d0.neighbours.Contains(centre)) newEdge.d0.neighbours.Add(centre);
                    }
                    else
                    {
                        graph.edges.Add(newEdge);
                        newEdge.v0.protrudes.Add(newEdge);
                        newEdge.v1.protrudes.Add(newEdge);
                    }

                    centre.borders.Add(newEdge);
                }
            }
            graph.randomPoints.Clear(); //For memory
        }
    }
}
