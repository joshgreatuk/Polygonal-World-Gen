using Godot;
using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corner = InnoRPG.scripts.generation.map.data.Corner;

namespace InnoRPG.scripts.generation.map.layers.water
{
    public partial class AssignWaterLayer : MapGenLayer
    {
        public override void ProcessLayer(ref Graph graph, MapGenerationOptions options)
        {
            float waterTileCornerPercentageRequirement = 0.2f;

            RandomNumberGenerator random = new();
            random.Seed = graph.mapSeed;

            int bumps = random.RandiRange(1, 6);
            double startAngle = random.RandfRange(0, 2 * (float)Math.PI);
            double dipAngle = random.RandfRange(0, 2 * (float)Math.PI);
            double dipWidth = random.RandfRange(0.2f, 0.7f);

            foreach (Corner corner in graph.corners)
            {
                float halfSize = options.worldSize / 2;
                Vector2 normalPos = (corner.position - new Vector2(halfSize, halfSize)) / halfSize;

                double angle = Math.Atan2(normalPos.Y, normalPos.X);
                double length = 0.5 * (Math.Max(Math.Abs(normalPos.X), Math.Abs(normalPos.Y)) + normalPos.Length());

                double r1 = 0.5 + 0.4 * Math.Sin(startAngle + bumps * angle + Math.Cos((bumps + 3) * angle));
                double r2 = 0.7 - 0.2 * Math.Sin(startAngle + bumps * angle - Math.Sin((bumps + 2) * angle));
                if (Math.Abs(angle - dipAngle) < dipWidth
                    || Math.Abs(angle - dipAngle + 2 * Math.PI) < dipWidth
                    || Math.Abs(angle - dipAngle - 2 * Math.PI) < dipWidth)
                {
                    r1 = r2 = 0.2;
                }
                corner.waterFlags = !(length < r1 || length > r1 * options.islandFactor && length < r2) 
                    ? corner.waterFlags |= WaterFlags.Water 
                    : corner.waterFlags &= ~WaterFlags.Water;
            }

            foreach (Centre centre in graph.centres)
            {
                float waterCorners = 0f;
                foreach (Corner corner in centre.corners) if (corner.waterFlags.HasFlag(WaterFlags.Water)) waterCorners++;

                waterCorners /= centre.corners.Count;

                if (waterCorners > waterTileCornerPercentageRequirement) centre.waterFlags |= WaterFlags.Water;
            }
        }
    }
}
