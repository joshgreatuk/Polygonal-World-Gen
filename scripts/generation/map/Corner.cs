using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.map
{
    public class Corner
    {
        public Vector2 position;
        public List<Centre> touches = new();
        public List<Edge> protrudes = new();
        public List<Corner> adjacent = new();

        public WaterFlags waterFlags;

        public double elevation = int.MaxValue;
        public double temperature = int.MaxValue;

        public Corner downSlope;
        public int river = 0;

        private float borderPos;

        public Corner(Vector2 position, float borderPos)
        {
            this.position = position;
            this.borderPos = borderPos;
        }

        public override bool Equals(object? obj)
        {
            if (obj != null && obj is Corner other)
            {
                return position.X == other.position.X && position.Y == other.position.Y;
            }
            return false;
        }

        public bool IsBorder() => position.X == 0 || position.Y == 0 || position.X == borderPos || position.Y == borderPos;
    }
}
