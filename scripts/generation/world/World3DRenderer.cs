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
    public partial class World3DRenderer : Node3D
    {
        //Generates the geometry for the world
        [Export] public World3DRenderOptions settings;
        [Export] public Material baseMaterial;

        private MeshInstance3D meshInstance;
        private Graph currentMap;

        public void SetMap(Graph graph)
        {
            currentMap = graph;
        }
        
        /* 3D rendering notes:
         * 1. Render terrain map
         * 2. Render terrain map + bottom geometry (just outline?)
         * 3. Render sides between terrain an bottom
         * 4. Assign colours to polygon centres
         * 5. Draw rivers (bevel?)
         */

        public void DrawMesh()
        {
            List<Vector3> verts = new();

            RenderTerrain(ref verts, currentMap);

            SurfaceTool surfaceTool = new();
            surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
            verts.ForEach(x => {
                surfaceTool.SetUV(new (x.X / currentMap.graphSize, x.Z / currentMap.graphSize));
                surfaceTool.AddVertex(x);
                });

            surfaceTool.Index();
            surfaceTool.GenerateNormals();
            surfaceTool.GenerateTangents();
            Mesh mesh = surfaceTool.Commit();

            ResourceSaver.Save(mesh, "res://map_render.tres", ResourceSaver.SaverFlags.Compress);

            if (meshInstance == null)
            {
                meshInstance = new();
                AddChild(meshInstance);
            }

            meshInstance.Mesh = mesh;
            meshInstance.MaterialOverride = baseMaterial;
        }

        //TO-DO: Move these verts to a Triangle class, so we can save colour and UV data easily
        private void RenderTerrain(ref List<Vector3> verts, Graph graph)
        { //Render terrain on top of mesh, apply colours
            foreach (Centre centre in graph.centres)
            {
                for (int i=0; i < centre.corners.Count; i++)
                {
                    //As a test, create a flat polygon around the centre, then bridge the gap with elevation

                    int j = i + 1;
                    if (j >= centre.corners.Count) j = 0;

                    //Generate tris to half way then use the 2 we took originally and the 2 we generated to form the outer ring with elevation
                    Vector3 midI = FindMidPoint(
                        ToVector3(centre.position, centre.elevation), 
                        ToVector3(centre.corners[i].position, centre.elevation), 
                        settings.flatMidPoint);

                    Vector3 midJ = FindMidPoint(
                        ToVector3(centre.position, centre.elevation),
                        ToVector3(centre.corners[j].position, centre.elevation),
                        settings.flatMidPoint);

                    //Add the tri from centre to mid
                    verts.Add(ToVector3(centre.position, centre.elevation));
                    verts.Add(midI);
                    verts.Add(midJ);

                    //The the two tris (square) from mid to corner
                    verts.Add(midI);
                    verts.Add(ToVector3(centre.corners[i].position, centre.corners[i].elevation));
                    verts.Add(ToVector3(centre.corners[j].position, centre.corners[j].elevation));

                    verts.Add(midJ);
                    verts.Add(midI);
                    verts.Add(ToVector3(centre.corners[j].position, centre.corners[j].elevation));

                    //verts.Add(ToVector3(centre.position, centre.elevation));
                    //verts.Add(ToVector3(centre.corners[i].position, centre.corners[i].elevation));
                    //verts.Add(ToVector3(centre.corners[j].position, centre.corners[j].elevation));
                }
            }
        }

        private void RenderFloor(ref List<Vector3> verts, Graph graph)
        { //Render outskirts of map

        }

        private void DrawRivers(ref List<Vector3> verts, Graph graph)
        { //Take river edges, bezel them, eventually draw more polygons on top of riverbeds with a translucent material

        }

        private Vector3 ToVector3(Vector2 position, double elevation) => 
            new Vector3(position.X, (float)elevation * settings.heightFactor, position.Y);

        //Find the difference, multiply it by midPointNormalized and return diff + a
        private Vector3 FindMidPoint(Vector3 a, Vector3 b, float midPointNormalized) =>
            a + ((b - a) * midPointNormalized);

        public Color GetWaterColour(World2DRenderOptions.ColourMode2D mode, WaterFlags flags, double elevation) => flags switch
        {
            WaterFlags.Coast => elevation > 1f ? settings.cliffColour : settings.beachColour,
            WaterFlags.Ocean | WaterFlags.Water => settings.oceanColour,
            WaterFlags.Water => settings.lakeColour,
            _ => settings.landColour,
        };

        public Color GetElevationColour(double elevation) => Blend3(settings.lowColour, settings.midColour, settings.highColour,
            (float)((elevation - currentMap.minElevation) / (currentMap.maxElevation - currentMap.minElevation)));

        /// <summary></summary>
        /// <param name="colourA"></param>
        /// <param name="colourB"></param>
        /// <param name="colourC"></param>
        /// <param name="normalisedBlend">Between 0 and 1, at 0.5 the colour will be colourB</param>
        /// <returns></returns>
        public Color Blend3(Color colourA, Color colourB, Color colourC, float normalisedBlend) => normalisedBlend > 0.5f ?
            BlendLerp(colourC, colourB, (normalisedBlend * 2) - 1) :
            BlendLerp(colourB, colourA, normalisedBlend * 2);

        public Color BlendLerp(Color colourA, Color colourB, float blend) => new Color(
            (float)(colourA.R * blend + colourB.R * (1 - blend)),
            (float)(colourA.G * blend + colourB.G * (1 - blend)),
            (float)(colourA.B * blend + colourB.B * (1 - blend)));

    }
}
