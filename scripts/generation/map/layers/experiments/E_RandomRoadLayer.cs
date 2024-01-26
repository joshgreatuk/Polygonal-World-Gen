using Godot;
using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.map.layers.experiments
{
    public partial class E_RandomRoadLayer : MapGenLayer
    {
        public override void ProcessLayer(ref Graph graph, MapGenerationOptions options)
        {
            int iterations = 300;
            //List<Edge> possibleEdges = new();

            //Edge currentEdge = graph.edges.Where(x => !x.v0.waterFlags.HasFlag(WaterFlags.Water)).First();
            //possibleEdges.Add(currentEdge);

            RandomNumberGenerator random = new();
            random.Seed = graph.mapSeed;

            List<Edge> possibleEdges = graph.edges.Where(
                x => !x.v0.waterFlags.HasFlag(WaterFlags.Water) 
                & !x.v1.waterFlags.HasFlag(WaterFlags.Water))
                .ToList();

            for (int i=0; i < iterations; i++)
            {
                //if (possibleEdges.Count < 1)
                //{
                //    break;
                //}

                //currentEdge.river = 1;
                //if (possibleEdges.Contains(currentEdge)) possibleEdges.Remove(currentEdge);

                //possibleEdges.AddRange(currentEdge.v0.protrudes.Where(x => x != currentEdge && x.river == 0));
                //possibleEdges.AddRange(currentEdge.v1.protrudes.Where(x => x != currentEdge && x.river == 0));

                //currentEdge = possibleEdges[random.RandiRange(0, possibleEdges.Count-1)];

                possibleEdges[random.RandiRange(0, possibleEdges.Count-1)].river = 1;
            }
        }   
    }
}
