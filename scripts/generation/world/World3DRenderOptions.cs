using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.world
{
    [GlobalClass]
    public partial class World3DRenderOptions : Resource
    {
        [ExportGroup("Settings")]
        [Export] public float heightFactor = 1f;
        [Export] public bool flattenMidPoint = false;
        [Export] public float flatMidPoint = 0.5f;

        [ExportGroup("Water Colours")]
        [Export] public Color landColour = Colors.ForestGreen;
        [Export] public Color lakeColour = Colors.LightCyan;
        [Export] public Color oceanColour = Colors.LightBlue;
        [Export] public Color beachColour = Colors.LightYellow;
        [Export] public Color cliffColour = Colors.Gray;

        [ExportGroup("Elevation Colours")]
        [Export] public Color highColour = Colors.Red;
        [Export] public Color midColour = Colors.Green;
        [Export] public Color lowColour = Colors.Blue;
    }
}
