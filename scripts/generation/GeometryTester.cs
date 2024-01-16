using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Array = Godot.Collections.Array;

namespace InnoRPG.scripts.generation
{
    [GlobalClass]
    public partial class GeometryTester : Node3D
    {
        [ExportCategory("Generation")]
        [Export] public Material material;
        [Export] public int sides = 5;
        [Export] public int iterations = 1;
        [Export] public float polygonRadius = 1f;
        [Export] public float polygonHeight = 1f;

        public override void _Ready()
        {
            //TO-DO: Add iterations

            //Array surfaceArray = new();
            //surfaceArray.Resize((int)Mesh.ArrayType.Max);

            List<Vector3> verts = new();
            //List<Vector2> uvs = new();
            //List<Vector3> normals = new();
            //List<int> indices = new();


            //Create the top and bottom
            //verts.AddRange(Generate2DPolygon(sides, Vector3.Zero));
            //verts.AddRange(Generate2DPolygon(sides, Vector3.Up * polygonHeight).Reverse<Vector3>());

            //Create the polygon
            verts.AddRange(Generate3DPolygon(sides, Vector3.Zero));

            //Commit Mesh
            //surfaceArray[(int)Mesh.ArrayType.Vertex] = verts.ToArray();
            ////surfaceArray[(int)Mesh.ArrayType.TexUV] = uvs.ToArray();
            ////surfaceArray[(int)Mesh.ArrayType.Normal] = normals.ToArray();
            ////surfaceArray[(int)Mesh.ArrayType.Index] = indices.ToArray();

            //ArrayMesh mesh = new();
            //mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);
            //mesh.SurfaceSetMaterial(0, material);

            SurfaceTool surfaceTool = new();
            surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
            verts.ForEach(x => surfaceTool.AddVertex(x));

            surfaceTool.Index();
            surfaceTool.GenerateNormals();
            surfaceTool.GenerateTangents();
            Mesh mesh = surfaceTool.Commit();

            ResourceSaver.Save(mesh, "res://polygon.tres", ResourceSaver.SaverFlags.Compress);

            MeshInstance3D meshInstance = new();
            meshInstance.Mesh = mesh;

            AddChild(meshInstance);
        }

        public Vector2 RotateVector(Vector2 vector, float degrees, Vector2? centre=null)
        {
            if (centre != null) vector -= (Vector2)centre;
            float radians = Mathf.DegToRad(degrees);
            //Vector2 newVector = new Vector2(
            //    Mathf.Sqrt(Mathf.Cos(radians*vector.X)-Mathf.Sin(radians*vector.Y)),
            //    Mathf.Sqrt(Mathf.Sin(radians*vector.X)+Mathf.Cos(radians*vector.Y)));
            Vector2 newVector = vector.Rotated(radians);
            return centre == null ? newVector : newVector + (Vector2)centre;
        }

        public Vector3 ToVector3(Vector2 vector) => new Vector3(vector.X, 0, vector.Y);

        public List<Vector3> Generate2DPolygon(int sides, Vector3 centre)
        {
            List<Vector3> verts = new();
            float angleStep = 360f / sides;
            for (int i = 0; i < sides; i++)
            {
                //Bottom polygon, anti-clockwise
                /*
                 * [0] = (0,0)
                 * [1] = right hand vertex
                 * [2] = left hand vertex
                 */
                verts.Add(centre);
                verts.Add(centre + ToVector3(RotateVector(Vector2.Up * polygonRadius, angleStep * (i + 1))));
                verts.Add(centre + ToVector3(RotateVector(Vector2.Up * polygonRadius, angleStep * i)));
            }
            return verts;
        }

        public List<Vector3> Generate3DPolygon(int sides, Vector3 bottomCentre)
        {
            List<Vector3> bottomVerts = new();
            List<Vector3> sideVerts = new();
            List<Vector3> topVerts = new();

            float angleStep = 360f / sides;
            for (int i = 0; i < sides; i++)
            {
                List<Vector3> bottomTri = GeneratePolygonTriangle(bottomCentre, polygonRadius, i * angleStep, angleStep);
                List<Vector3> topTri = GeneratePolygonTriangle(bottomCentre + (Vector3.Up * polygonHeight), polygonRadius, i * angleStep, angleStep);

                List<Vector3> sideTris = new()
                {
                    bottomTri[2], topTri[1], bottomTri[1],
                    topTri[1], bottomTri[2], topTri[2]
                };

                bottomVerts.AddRange(bottomTri);
                sideVerts.AddRange(sideTris);
                topVerts.AddRange(topTri);
            }

            List<Vector3> verts = new();
            verts.AddRange(bottomVerts.Reverse<Vector3>());
            verts.AddRange(sideVerts);
            verts.AddRange(topVerts);
            return verts;
        }

        public List<Vector3> GeneratePolygonTriangle(Vector3 centre, float radius, float angleA, float angleStep)
        {
            return new() { centre, centre + ToVector3(RotateVector(Vector2.Up*radius, angleA)), centre + ToVector3(RotateVector(Vector2.Up*radius, angleA+angleStep)) };
        }
    }
}