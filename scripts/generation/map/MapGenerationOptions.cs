using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.map
{
    [GlobalClass]
    public partial class MapGenerationOptions : Resource
    {
        [Export] public int voronoiPointCount = 1000;
        [Export] public int worldSize = 1024;
        [Export] public int lloydsAlgorithmIterations = 3;
        [Export] public float elevationScaleFactor = 1.1f;

        [Export] public float islandFactor = 1.07f;
        [Export] public float waterLevel = 0.15f;
        [Export] public bool allowCliffs = true;

        [Export] public float nTemp = 20;
        [Export] public float equatorTemp = 50;
        [Export] public float sTemp = 0;

        [Export] public float equatorPosition = 0.6f;

        [Export] public int riverIterations = 200;

        [Export] public Curve elevationCurve;
    }
}
