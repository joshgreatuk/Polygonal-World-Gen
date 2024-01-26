using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace InnoRPG.scripts.generation.map.data
{
    public class Corner
    {
        public Vector2 position;
        public List<Centre> touches = new();
        public List<Edge> protrudes = new();
        public List<Corner> adjacent = new();

        public WaterFlags waterFlags;

        public double elevation = 0;
        public double temperature = 0;

        public Corner downSlope;
        public int river = 0;
        public double distanceFromCoast = 0;

        private float borderPos;

        public Label cornerLabel;

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

        private bool? isBorder = null;
        public bool IsBorder
        {
            get
            {
                if (isBorder == null) isBorder = position.X == 0 || position.Y == 0 || position.X == borderPos || position.Y == borderPos;
                return (bool)isBorder;
            }
        }
    }
}
