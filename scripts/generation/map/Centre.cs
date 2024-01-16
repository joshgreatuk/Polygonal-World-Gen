using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.map
{
    public class Centre
    {
        public Vector2 position;
        public List<Centre> neighbours = new();
        public List<Edge> borders = new();
        public List<Corner> corners = new();

        public WaterFlags waterFlags;

        public double elevation;
        public double temperature;

        public Centre(Vector2 position)
        {
            this.position = position;
        }

        public override bool Equals(object? obj)
        {
            if (obj != null && obj is Centre centre)
            {
                return centre.position.X == position.X && centre.position.Y == position.Y;
            }
            return false;
        }

        public bool IsBorder() => corners.Any(x => x.IsBorder());
    }

    [Flags]
    public enum WaterFlags
    {
        None = 0,
        Water = 1,
        Ocean = 2,
        Coast = 4
    }
}
