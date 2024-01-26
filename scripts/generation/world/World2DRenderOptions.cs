using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.world
{
    [GlobalClass]
    public partial class World2DRenderOptions : Resource
    {
        [Flags]
        public enum ColourMode2D
        {
            None = 0,
            Solid = 1,
            Dotted = 2,
            Flush = 4,
            Water = 8,
            Temperature = 16,
            Elevation = 32,
            Linked = 64,
        }

        [Flags]
        public enum TextMode2D
        {
            None = 0,
            Temperature = 1,
            Elevation = 2,
            Water = 4,
            DistanceFromCoast = 8,
        }

        /* Options
         * - Render size
         * - Corner size, border mode, fill mode
         * - Edge size, border mode, fill mode
         * - Centre border mode, fill mode
         */
        [ExportCategory("Settings")]
        [Export] public float renderScale = 1f;
        [Export] public float cornerScale = 1f;
        [Export] public float edgeScale = 1f;
        [Export] public float centreScale = 1f;
        [Export] public float linkScale = 1f;
        [Export] public float riverScale = 0.5f;

        [ExportCategory("Colour Modes")]
        [Export] public ColourMode2D cornerMode;
        [Export] public ColourMode2D edgeMode;
        [Export] public ColourMode2D centreMode;

        [ExportCategory("Text Options")]
        [Export] public TextMode2D centreTextMode;
        [Export] public TextMode2D cornerTextMode;
        [Export] public Color textColour = Colors.Black;
        [Export] public int fontSize = 12;
        [Export] public bool includeWater = false;


        [ExportCategory("Colour Options")]
        [Export] public Color cornerColour = Colors.Blue;
        [Export] public Color edgeColour = Colors.Black;
        [Export] public Color centreColour = Colors.Purple;
        [Export] public Color riverColour = Colors.SkyBlue;

        [ExportGroup("Centre Colours")]
        [Export] public Color linkedColour = Colors.Red;
        [Export] public Color landColour = Colors.ForestGreen;
        [Export] public Color lakeColour = Colors.LightCyan;
        [Export] public Color oceanColour = Colors.LightBlue;
        [Export] public Color beachColour = Colors.LightYellow;
        [Export] public Color cliffColour = Colors.Gray;

        [ExportGroup("Temperature Colours")]
        [Export] public Color hotColour = Colors.Red;
        [Export] public Color temperateColour = Colors.Green;
        [Export] public Color coldColour = Colors.Blue;

        [ExportGroup("Elevation Colours")]
        [Export] public Color highColour = Colors.Red;
        [Export] public Color midColour = Colors.Green;
        [Export] public Color lowColour = Colors.Blue;
    }
}
