using Godot;
using InnoRPG.scripts.generation.map;
using InnoRPG.scripts.generation.map.data;
using InnoRPG.scripts.generation.world;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation
{
    [GlobalClass]
    public partial class WorldGenerator : Node
    {
        [Export] MapGenerator mapGen;
        [Export] World2DRenderer world2DRenderer;

        //The control class for the world generation system
        public override void _Ready()
        {
            GD.Print("Generating map");
            Graph map = mapGen.StartGenerator();

            GD.Print("Rendering map");
            world2DRenderer.SetActiveMap(map);

            GD.Print("Done");
        }

        public override void _UnhandledKeyInput(InputEvent @event)
        {
            if (@event.IsActionReleased("map_reset")) _Ready();
        }
    }
}
