﻿using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.map
{
    using data;
    using layers;
    using layers.elevation;
    using layers.experiments;
    using layers.experiments.elevation;
    using layers.polygons;
    using layers.rivers;
    using layers.temperature;
    using layers.water;
    using System.Diagnostics;

    [GlobalClass]
    public partial class MapGenerator : Node
    {
        [Export] public MapGenerationOptions options;

        private List<Type> mapGenLayers = new()
        {   //Polygons
            typeof(SeedLayer),
            typeof(RandomPointLayer),
            typeof(LloydRelaxationLayer),
            typeof(GeneratePolygonLayer),
            //Water
            typeof(AssignWaterLayer),
            typeof(AssignOceanLayer),

            //Elevation TO-DO: Add noise-based elevation
            //typeof(E_NoiseElevation),

            //Coast-based elevation
            typeof(BreadthSearchCornerElevationLayer), //Best method
            //typeof(AssignCornerElevationLayer),
            typeof(RedistributeElevationLayer), //Not sure if this is needed honestly
            //typeof(E_CurveAdjustment),
            typeof(E_StepRounding),
            typeof(AssignCentreElevationLayer),

            //Temperature
            typeof(AssignCornerTempLayer),
            typeof(AssignCentreTempLayer),
            //Rivers
            //typeof(CalculateDownslopeLayer), //Handled by BreadthSearchCornerElevationLayer
            typeof(GenerateRiversLayer),

            //Experimentation
            //typeof(E_MountainRoadLayer),
            //typeof(E_RandomRoadLayer),
        };

        public Graph StartGenerator()
        {
            Stopwatch stopwatch = new();

            if (options == null)
            {
                GD.Print("Map Generator has no options assigned");
                return null;
            }

            //TO-DO: Multi-thread this into background threads
            Graph graph = new Graph(options.worldSize);

            //Process layers
            foreach (Type layerType in mapGenLayers)
            {
                if (!layerType.IsAssignableTo(typeof(MapGenLayer)))
                {
                    GD.Print($"Map layer type '{layerType.Name}' is not assignable to type MapGenLayer");
                    continue;
                }

                MapGenLayer mapGenLayer = Activator.CreateInstance(layerType) as MapGenLayer;
                //TO-DO: Add logging solution
                GD.Print($"Processing map layer '{layerType.Name}'");
                stopwatch.Restart();
                try
                {
                    mapGenLayer.ProcessLayer(ref graph, options);
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"MapGenLayer '{layerType.Name}' returned error", ex);
                }
                stopwatch.Stop();
                GD.Print($"Map layer '{layerType.Name}' returned in {stopwatch.ElapsedMilliseconds}ms");
            }

            GD.Print($"Map generated");
            graph.CalculateGraphLimits();
            return graph;
        }
    }
}
