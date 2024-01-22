﻿using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.generation.map
{
    using data;
    using InnoRPG.scripts.generation.map.layers;
    using InnoRPG.scripts.generation.map.layers.elevation;
    using InnoRPG.scripts.generation.map.layers.polygons;
    using InnoRPG.scripts.generation.map.layers.temperature;
    using InnoRPG.scripts.generation.map.layers.water;

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
            typeof(AssignCornerElevationLayer),
            typeof(RedistributeElevationLayer),
            typeof(AssignCentreElevationLayer),
            //Temperature
            typeof(AssignCornerTempLayer),
            typeof(AssignCentreTempLayer),
            //Rivers
        };

        public Graph StartGenerator()
        {
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
                try
                {
                    mapGenLayer.ProcessLayer(ref graph, options);
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"MapGenLayer '{layerType.Name}' returned error", ex);
                }
            }

            GD.Print("Map generated");
            graph.CalculateGraphLimits();
            return graph;
        }
    }
}
