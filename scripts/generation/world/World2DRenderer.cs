using Godot;
using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corner = InnoRPG.scripts.generation.map.data.Corner;

namespace InnoRPG.scripts.generation.world
{
    [GlobalClass]
    public partial class World2DRenderer : Node2D
    {
        [Export] public World2DRenderOptions options;

        private Graph map;
        private bool mapDrawn = false;

        public override void _Ready()
        {
            Visible = false;
            if (options == null)
            {
                GD.Print("World2DRenderer has no options set");
                ProcessMode = ProcessModeEnum.Disabled;
                return;
            }
        }

        public void SetActiveMap(Graph map, bool drawMap=true)
        {
            this.map = map;
            mapDrawn = false;
            if (drawMap) QueueRedraw();
        }

        public override void _Draw()
        {
            if (mapDrawn || map == null) return;

            //Draw map
            if (!map.limitsCalculated) map.CalculateGraphLimits();

            DrawCentres(options.centreMode);
            DrawEdges(options.edgeMode);
            DrawCorners(options.cornerMode);

            mapDrawn = true;
        }
        #region Corners
        public void DrawCorners(World2DRenderOptions.ColourMode2D mode)
        {
            if (mode is World2DRenderOptions.ColourMode2D.None) return; //TO-DO: Redo this, flags wont work properlyt with dotted, look at DrawEdges()

            foreach (Corner corner in map.corners)
            {
                if (mode.HasFlag(World2DRenderOptions.ColourMode2D.Solid))
                {
                    DrawCircle(corner.position, options.cornerScale, options.cornerColour);
                }

                if (mode.HasFlag(World2DRenderOptions.ColourMode2D.Dotted))
                {
                    DrawArc(corner.position, options.cornerScale, 0, 360, 32, options.cornerColour, 1f);
                }

                if (mode.HasFlag(World2DRenderOptions.ColourMode2D.Water) && corner.waterFlags is not WaterFlags.None || corner.river > 0)
                {
                    DrawCircle(corner.position, options.riverScale, GetWaterColour(mode, corner.waterFlags, corner.elevation));
                }

                if (mode.HasFlag(World2DRenderOptions.ColourMode2D.Temperature))
                {
                    DrawCircle(corner.position, options.riverScale, GetTempColour(corner.temperature));
                }

                if (mode.HasFlag(World2DRenderOptions.ColourMode2D.Elevation))
                {
                    DrawCircle(corner.position, options.riverScale, GetElevationColour(corner.elevation));
                }

                //Flush and Linked flags skipped
            }
        }
        #endregion
        #region Edges
        public void DrawEdges(World2DRenderOptions.ColourMode2D mode)
        {
            if (mode is World2DRenderOptions.ColourMode2D.None) return;

            foreach (Edge edge in map.edges)
            {
                Color renderColour = options.edgeColour;
                float edgeScale = options.edgeScale;

                if (mode.HasFlag(World2DRenderOptions.ColourMode2D.Flush))
                {
                    renderColour = edge.d0.renderedColour;
                }

                if (mode.HasFlag(World2DRenderOptions.ColourMode2D.Water))
                {
                    renderColour = edge.river > 0 ? options.riverColour : options.edgeColour;
                    edgeScale = options.riverScale * edge.river;
                }

                if (mode.HasFlag(World2DRenderOptions.ColourMode2D.Dotted))
                {
                    DrawDashedLine(edge.v0.position, edge.v1.position, renderColour, edgeScale);
                }
                else
                {
                    DrawLine(edge.v0.position, edge.v1.position, renderColour, edgeScale);
                }
            }

            //Temperature, Elevation, Linked skipped
        }
        #endregion
        #region Centres
        public void DrawCentres(World2DRenderOptions.ColourMode2D mode)
        {
            if (mode is World2DRenderOptions.ColourMode2D.None) return;

            foreach (Centre centre in map.centres)
            {
                Color renderColour = options.landColour;

                if (mode.HasFlag(World2DRenderOptions.ColourMode2D.Water))
                {
                    renderColour = GetWaterColour(mode, centre.waterFlags, centre.elevation);
                }

                if (mode.HasFlag(World2DRenderOptions.ColourMode2D.Temperature))
                {
                    renderColour = GetTempColour(centre.temperature);
                }

                if (mode.HasFlag(World2DRenderOptions.ColourMode2D.Elevation))
                {
                    renderColour = GetElevationColour(centre.elevation);
                }

                DrawColoredPolygon(centre.corners.Select(x => x.position).ToArray(), renderColour);

                if (mode.HasFlag(World2DRenderOptions.ColourMode2D.Linked))
                {
                    //Draw lines between polygon centres with dot
                    foreach(Centre adjacent in centre.neighbours)
                    {
                        if (mode.HasFlag(World2DRenderOptions.ColourMode2D.Dotted))
                        {
                            DrawDashedLine(centre.position, adjacent.position, options.linkedColour, options.linkScale);
                        }
                        else
                        {
                            DrawLine(centre.position, adjacent.position, options.linkedColour, options.linkScale);
                        }

                        DrawCircle(centre.position, options.linkScale, options.linkedColour);
                    }
                }
            }

            //Solid, Dotted, Flush ignored
        }
        #endregion
        #region Utils
        public Color GetWaterColour(World2DRenderOptions.ColourMode2D mode, WaterFlags flags, double elevation) => flags switch
        {
            WaterFlags.Coast => elevation > 1f ? options.cliffColour : options.beachColour,
            WaterFlags.Ocean => options.oceanColour,
            WaterFlags.Water => options.lakeColour,
            _ => options.riverColour,  
        };

        public Color GetTempColour(double temperature) => Blend3(options.coldColour, options.temperateColour, options.hotColour,
            (float)((temperature - map.minTemperature) / (map.maxTemperature - map.minTemperature)));

        public Color GetElevationColour(double elevation) => Blend3(options.lowColour, options.midColour, options.highColour,
            (float)((elevation - map.minElevation) / (map.maxElevation - map.minElevation)));

        /// <summary></summary>
        /// <param name="colourA"></param>
        /// <param name="colourB"></param>
        /// <param name="colourC"></param>
        /// <param name="normalisedBlend">Between 0 and 1, at 0.5 the colour will be colourB</param>
        /// <returns></returns>
        public Color Blend3(Color colourA, Color colourB, Color colourC, float normalisedBlend) => normalisedBlend > 0.5f ?
            BlendLerp(colourB, colourC, (normalisedBlend*2)-1) :
            BlendLerp(colourA, colourB, normalisedBlend*2);

        public Color BlendLerp(Color colourA, Color colourB, float blend) => new Color(
            (float)(colourA.R * blend + colourB.R * (1 - blend)),
            (float)(colourA.G * blend + colourB.G * (1 - blend)),
            (float)(colourA.B * blend + colourB.B * (1 - blend)));
        #endregion
    }
}
