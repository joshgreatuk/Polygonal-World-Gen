using Godot;
using InnoRPG.scripts.generation.map;
using InnoRPG.scripts.generation.map.data;
using InnoRPG.scripts.generation.world;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation
{
    [GlobalClass]
    public partial class WorldGenerator : Node
    {
        [Export] public MapGenerator mapGen;
        [Export] public World2DRenderer world2DRenderer;
        [Export] public World3DRenderer world3DRenderer;

        public enum WorldGenMode { Gen2D, Gen3D }
        [Export] public WorldGenMode mode;

        //The control class for the world generation system
        public override void _Ready()
        {
            Task.Run(GenerateWorld);
        }

        public void GenerateWorld()
        {
            Stopwatch stopwatch = new();
            float stopwatchTotal = 0;
            stopwatch.Start();
            GD.Print("Generating map");
            Graph map = mapGen.StartGenerator();
            GD.Print($"Generator returned in {stopwatch.ElapsedMilliseconds}ms");
            stopwatchTotal += stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();
            GD.Print("Rendering map");
            if (mode is WorldGenMode.Gen2D)
            {
                world2DRenderer.SetActiveMap(map);
            }
            else if (mode is WorldGenMode.Gen3D)
            {
                world3DRenderer.SetMap(map);
                world3DRenderer.DrawMesh();
            }
            GD.Print($"Renderer returned in {stopwatch.ElapsedMilliseconds}ms");
            stopwatchTotal += stopwatch.ElapsedMilliseconds;
            stopwatch.Stop();
            GD.Print($"Done in {stopwatchTotal}ms");
        }

        public override void _UnhandledKeyInput(InputEvent @event)
        {
            if (@event.IsActionReleased("map_reset")) _Ready();
        }
    }
}
