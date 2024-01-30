using Godot;
using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private LabelSettings labelSettings;

        public override void _Ready()
        {
            Visible = false;
            if (options == null)
            {
                GD.Print("World2DRenderer has no options set");
                ProcessMode = ProcessModeEnum.Disabled;
                return;
            }

            labelSettings = new();
            labelSettings.FontSize = Mathf.RoundToInt(options.fontSize * options.renderScale);
            labelSettings.FontColor = options.textColour;
        }

        public void SetActiveMap(Graph map, bool drawMap=true)
        {
            if (this.map != null)
            {
                foreach (Corner corner in this.map.corners.Values.Where(x => x.cornerLabel != null)) corner.cornerLabel.QueueFree();
                foreach (Centre centre in this.map.centres.Where(x => x.centreLabel != null)) centre.centreLabel.QueueFree();
            }

            this.map = map;
            mapDrawn = false;
            Visible = true;
            if (drawMap) QueueRedraw();
        }

        public override void _Draw()
        {
            //if (mapDrawn || map == null) return;
            if (map == null) return;

            //Draw map
            if (!map.limitsCalculated) map.CalculateGraphLimits();

            DrawCentres(options.centreMode);
            DrawEdges(options.edgeMode);
            DrawCorners(options.cornerMode);
            DrawCentreText(options.centreTextMode);
            DrawCornerText(options.cornerTextMode);

            Scale = new Vector2(options.renderScale, options.renderScale);

            mapDrawn = true;
        }
        #region Corners
        public void DrawCorners(World2DRenderOptions.ColourMode2D mode)
        {
            if (mode is World2DRenderOptions.ColourMode2D.None) return; //TO-DO: Redo this, flags wont work properlyt with dotted, look at DrawEdges()

            foreach (Corner corner in map.corners.Values)
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
                    DrawLine(edge.v0.position, edge.v1.position, renderColour, edgeScale, true);
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

                if (mode.HasFlag(World2DRenderOptions.ColourMode2D.Elevation) && !centre.waterFlags.HasFlag(WaterFlags.Water))
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
                    }
                }

                if (mode.HasFlag(World2DRenderOptions.ColourMode2D.Solid))
                {
                    DrawCircle(centre.position, options.centreScale, options.centreColour);
                }

                if (mode.HasFlag(World2DRenderOptions.ColourMode2D.Flush))
                {
                    DrawArc(centre.position, options.centreScale, 0, 360, 32, options.centreColour, 1);
                }

            }

            //Flush ignored
        }

        public void DrawCornerText(World2DRenderOptions.TextMode2D mode)
        {
            if (mode is World2DRenderOptions.TextMode2D.None) return;

            foreach (Corner corner in options.includeWater ? map.corners.Values : map.corners.Values.Where(x => !x.waterFlags.HasFlag(WaterFlags.Water)))
            {
                string textContents = string.Empty;
                if (mode.HasFlag(World2DRenderOptions.TextMode2D.Temperature))
                {
                    textContents += Mathf.Round(corner.temperature * 100) / 100;
                }

                if (mode.HasFlag(World2DRenderOptions.TextMode2D.Elevation))
                {
                    if (textContents != string.Empty) textContents += "\n";
                    textContents += Mathf.Round(corner.elevation * 100) / 100;
                }

                if (mode.HasFlag(World2DRenderOptions.TextMode2D.Water))
                {
                    if (textContents != string.Empty) textContents += "\n";
                    textContents += corner.waterFlags.ToString();
                }

                if (mode.HasFlag(World2DRenderOptions.TextMode2D.DistanceFromCoast))
                {
                    if (textContents != string.Empty) textContents += "\n";
                    textContents += corner.distanceFromCoast;
                }

                Label textLabel;
                if (corner.cornerLabel != null) textLabel = corner.cornerLabel;
                else
                {
                    textLabel = new Label();
                    textLabel.SetAnchorsPreset(Control.LayoutPreset.Center);
                    textLabel.LabelSettings = labelSettings;
                    textLabel.Name = $"Corner: {corner.position}";
                    textLabel.HorizontalAlignment = HorizontalAlignment.Center;
                    textLabel.VerticalAlignment = VerticalAlignment.Center;
                    corner.cornerLabel = textLabel;
                    AddChild(textLabel);
                }
                
                textLabel.Text = textContents;
                textLabel.Position = corner.position - textLabel.Size/2;
            }
        }

        public void DrawCentreText(World2DRenderOptions.TextMode2D mode)
        {
            if (mode is World2DRenderOptions.TextMode2D.None) return;

            foreach (Centre centre in options.includeWater ? map.centres : map.centres.Where(x => !x.waterFlags.HasFlag(WaterFlags.Water)))
            {
                string textContents = string.Empty;
                if (mode.HasFlag(World2DRenderOptions.TextMode2D.Temperature))
                {
                    textContents += Mathf.Round(centre.temperature * 100) / 100;
                }

                if (mode.HasFlag(World2DRenderOptions.TextMode2D.Elevation))
                {
                    if (textContents != string.Empty) textContents += "\n";
                    textContents += Mathf.Round(centre.elevation * 100) / 100;
                }

                if (mode.HasFlag(World2DRenderOptions.TextMode2D.Water))
                {
                    if (textContents != string.Empty) textContents += "\n";
                    textContents += centre.waterFlags.ToString();
                }

                Label textLabel;
                if (centre.centreLabel != null) textLabel = centre.centreLabel;
                else
                {
                    textLabel = new Label();
                    textLabel.SetAnchorsPreset(Control.LayoutPreset.Center);
                    textLabel.LabelSettings = labelSettings;
                    textLabel.Name = $"Centre: {centre.position}";
                    textLabel.HorizontalAlignment = HorizontalAlignment.Center;
                    textLabel.VerticalAlignment = VerticalAlignment.Center;
                    centre.centreLabel = textLabel;
                    AddChild(textLabel);
                }

                textLabel.Text = textContents;
                textLabel.Position = centre.position - textLabel.Size / 2;
            }
        }
        #endregion
        #region Utils
        public Color GetWaterColour(World2DRenderOptions.ColourMode2D mode, WaterFlags flags, double elevation) => flags switch
        {
            WaterFlags.Coast => elevation > 1f ? options.cliffColour : options.beachColour,
            WaterFlags.Ocean | WaterFlags.Water => options.oceanColour,
            WaterFlags.Water => options.lakeColour,
            _ => options.landColour,  
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
            BlendLerp(colourC, colourB, (normalisedBlend*2)-1) :
            BlendLerp(colourB, colourA, normalisedBlend*2);

        public Color BlendLerp(Color colourA, Color colourB, float blend) => new Color(
            (float)(colourA.R * blend + colourB.R * (1 - blend)),
            (float)(colourA.G * blend + colourB.G * (1 - blend)),
            (float)(colourA.B * blend + colourB.B * (1 - blend)));
        #endregion
    }
}
