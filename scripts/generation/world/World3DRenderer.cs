using Godot;
using InnoRPG.scripts.generation.map.data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Stopwatch stopwatch = new();
            List<Tri> tris = new();

            RenderTerrain(ref tris, currentMap);
            GD.Print($"World3DRenderer generated tris in {stopwatch.ElapsedMilliseconds}ms");
            stopwatch.Restart();

            RenderRivers(ref tris, currentMap);
            GD.Print($"World3DRenderer generated river tris in {stopwatch.ElapsedMilliseconds}ms");
            stopwatch.Restart();

            SurfaceTool surfaceTool = new();
            surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
            tris.ForEach(y => y.points.ForEach(x => {
                surfaceTool.SetUV(new (x.X / currentMap.graphSize, x.Z / currentMap.graphSize));
                surfaceTool.SetColor(y.colour);
                surfaceTool.AddVertex(x);
                }));

            GD.Print($"World3DRenderer added tris in {stopwatch.ElapsedMilliseconds}ms");
            stopwatch.Restart();

            surfaceTool.Index();
            surfaceTool.GenerateNormals();
            surfaceTool.GenerateTangents();
            Mesh mesh = surfaceTool.Commit();

            mesh.SurfaceSetMaterial(0, baseMaterial);

            GD.Print($"World3DRenderer indexed, created normals and committed in {stopwatch.ElapsedMilliseconds}ms");
            stopwatch.Restart();
#if GODOT_PC
            ResourceSaver.Save(mesh, "res://map_render.tres", ResourceSaver.SaverFlags.Compress);
            GD.Print($"World3DRenderer saved mesh in {stopwatch.ElapsedMilliseconds}ms");
#endif
            stopwatch.Stop();

            if (meshInstance == null)
            {
                meshInstance = new();
                CallDeferred("add_child", meshInstance);
            }

            meshInstance.Mesh = mesh;
        }

        private void RenderTerrain(ref List<Tri> tris, Graph graph)
        { //Render terrain on top of mesh, apply colours
            foreach (Centre centre in graph.centres)
            {
                for (int i=0; i < centre.corners.Count; i++)
                {
                    //As a test, create a flat polygon around the centre, then bridge the gap with elevation

                    int j = i + 1;
                    if (j >= centre.corners.Count) j = 0;

                    Color centreColour = centre.waterFlags.HasFlag(WaterFlags.Water) ?
                            GetWaterColour(centre.waterFlags, centre.elevation) :
                            GetElevationColour(centre.elevation);

                    if (!settings.flattenMidPoint)
                    {
                        //Easier to build rivers in here?
                        Corner iCorner = centre.corners[i];
                        Corner jCorner = centre.corners[j];
                         
                        Vector2 iPosition = iCorner.position;
                        Vector2 jPosition = jCorner.position;

                        if (iCorner.river > 0 && jCorner.river > 0)
                        {
                            //TO-DO: Build river support
                        }

                        tris.Add(new(new Vector3[]
                        {
                            ToVector3(centre.position, centre.elevation),
                            ToVector3(iPosition, iCorner.elevation),
                            ToVector3(jPosition, jCorner.elevation)
                        },
                            centreColour
                        ));
                        continue;
                    }

                    //TO-DO: Build river support
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
                    tris.Add(new(new Vector3[] 
                    {
                        ToVector3(centre.position, centre.elevation),
                        midI,
                        midJ 
                    },
                        centreColour
                    ));

                    //The the two tris (square) from mid to corner
                    tris.Add(new(new Vector3[]
                    {
                        midI,
                        ToVector3(centre.corners[i].position, centre.corners[i].elevation),
                        ToVector3(centre.corners[j].position, centre.corners[j].elevation)
                    },
                        centreColour
                    ));

                    tris.Add(new(new Vector3[]
                    {
                        midJ,
                        midI,
                        ToVector3(centre.corners[j].position, centre.corners[j].elevation)
                    },
                        centreColour
                    ));
                }
            }
        }

        private void RenderFloor(ref List<Tri> verts, Graph graph)
        { //Render outskirts of map

        }

        private void RenderRivers(ref List<Tri> verts, Graph graph)
        { //Take river edges, bezel them, eventually draw more polygons on top of riverbeds with a translucent material

        }

        private Vector3 ToVector3(Vector2 position, double elevation) => 
            new Vector3(position.X, (float)elevation * settings.heightFactor, position.Y);

        //Find the difference, multiply it by midPointNormalized and return diff + a
        private Vector3 FindMidPoint(Vector3 a, Vector3 b, float midPointNormalized) =>
            a + ((b - a) * midPointNormalized);

        public Color GetWaterColour(WaterFlags flags, double elevation) => flags switch
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

        public class Tri
        {
            public List<Vector3> points; //Should be 3 only
            public Color colour;

            public Tri(Vector3[] points, Color colour)
            {
                this.points = points.ToList();
                this.colour = colour;
            }
        }

    }
}
