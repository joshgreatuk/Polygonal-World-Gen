using Godot;
using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoronatorSharp;
using Vector2 = VoronatorSharp.Vector2;
using Corner = InnoRPG.scripts.generation.map.data.Corner;

namespace InnoRPG.scripts.generation.map.layers.polygons
{
    public partial class GeneratePolygonLayer : MapGenLayer
    {
        public override void ProcessLayer(ref Graph graph, MapGenerationOptions options)
        {
            //Generate polygons from random points, clear random points
            Stopwatch debugStopwatch = new();
            debugStopwatch.Start();
            Voronator voronator = new(
                graph.randomPoints.Select(x => new Vector2(x.X, x.Y)).ToList(),
                Vector2.zero,
                new Vector2(options.worldSize, options.worldSize));

            GD.Print($"GeneratePolygonLayer Voronator returned in {debugStopwatch.ElapsedMilliseconds}ms");
            debugStopwatch.Restart();

            for (int i = 0; i < options.voronoiPointCount; i++)
            {
                Centre centre = new(graph.randomPoints[i]);
                foreach (Vector2 cornerPos in voronator.GetClippedPolygon(i))
                {
                    if (cornerPos == null) continue;
                    //Clamp corner to world border
                    Vector2 clampedCorner = ClampVector(cornerPos, 0, options.worldSize);

                    Corner corner = new(new Godot.Vector2(clampedCorner.x, clampedCorner.y), options.worldSize);
                    Corner existingCorner = graph.corners.FirstOrDefault(x => x.Equals(corner)); //Querying graph.corners is slow asf
                    if (existingCorner != null) corner = existingCorner;
                    else graph.corners.Add(corner);

                    centre.corners.Add(corner);
                    corner.touches.Add(centre);
                }
                graph.centres.Add(centre);

                for (int j = 0; j < centre.corners.Count; j++)
                {
                    //Generate edges between corners
                    int k = j + 1;
                    if (k >= centre.corners.Count) k = 0;
                    Edge newEdge = new Edge(centre, null, centre.corners[j], centre.corners[k]);

                    //Check if the edge already exists
                    Edge existingEdge = centre.corners.SelectMany(x => x.protrudes).FirstOrDefault(x => x.Equals(newEdge));
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

            GD.Print($"GeneratePolygonLayer polygon extraction took {debugStopwatch.ElapsedMilliseconds}ms");
            debugStopwatch.Restart();

            foreach (Corner corner in graph.corners)
            {
                foreach (Edge edge in corner.protrudes)
                {
                    Corner adjacent = null;
                    if (edge.v1.Equals(corner)) adjacent = edge.v0;
                    else if (edge.v0.Equals(corner)) adjacent = edge.v1;

                    corner.adjacent.Add(adjacent);
                }
            }

            GD.Print($"GeneratePolygonLayer adjacent collection took {debugStopwatch.ElapsedMilliseconds}ms");
            debugStopwatch.Stop();
        }

        private Vector2 ClampVector(Vector2 vec, double min, double max) =>
            new Vector2((float)Mathf.Clamp(vec.x, min, max), (float)Mathf.Clamp(vec.y, min, max));
    }
}
