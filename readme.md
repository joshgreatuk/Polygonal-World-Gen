# Polygonal World Generation - Godot 4.2 - C#
![image](https://github.com/joshgreatuk/Polygonal-World-Gen/assets/35399690/a6f8941c-0252-4284-9921-3cf7a4b6d9b5)

## World Generation? InnoRPG? Mobile Fantasy RPG?
Yes! The Original idea for this project was a DnD-like fantasy RPG aimed at mobile platforms. It's an interesting concept and one that I'd like to play with more but for now, this project only contains the world generation part of the concept as well as the 2D and 3D renderer that I made to show the generation in Godot 4.2.

## How'd You Do it?
First of all, I wanted to play with procedural generation, so first i did some research. After stumbling on a blog post about polygonal world generation (https://www-cs-students.stanford.edu/~amitp/game-programming/polygon-map-generation/) I was hooked

I originally implemented this concept in an unreleased discord bot where I used ImageSharp to draw the 2D visualisation of the world. I then decided to rebuild it in Godot 4.2 so that I could translate the world into a 3D space, which I have now managed to do.

Before I go through the individual steps of the world generation I'll explain how the code in this repo is formatted.

## Code Format
The project is split into the folders:
- scripts/generation
	- map (contains the actual map generation code)
		- data (the data classes for the map)
		- layers (each step is split into map gen layers, this allows for modularity)
			- elevation (layers are organised into folders to keep the code tidy)
			- experiments
			- polygons
			- rivers
			- temperature
			- water
	- world (contains the code that bridges between the map generation and the frontend (Godot))

Throughout the repo, you may see references to classes starting with "E_", this just means an experimental class that didnt make it into the world generation code as a more optimised, complete layer, or just didnt work at all.

## Now! The World Generation
### Polygonal Generation
First of all we assign the seed for the random generation in SeedLayer, I made this as simple as taking the current system millisecond and using that.

Next up is RandomPointLayer, which as it says in the class name, generates random points. It generates as many points as set in the MapGenerationOptions via the voronoiPointCount field.

Now we use the Lloyd Relaxation algorithm to relax the points, making them more uniform. This is run multiple times as set in the MapGenerationOptions via the lloydsAlgorithmIterations field, the more you run this the less variation you will find in the polygons, since they are random points you could end up with some polygons that are big and some that are very small.

Lastly for the polygon generation we now use VoronatorSharp (https://www-cs-students.stanford.edu/~amitp/game-programming/polygon-map-generation/). This consists of generating the polygons in VoronatorSharp then iterating through them, adding the Centre, Corner and Edge nodes to the Graph. During this stage we also populate the lists on each node, including adjacent corners, neighbour centres and centre edges, this helps to traverse the graph in later steps.

After the polygonal generate I also clear the random point list from the map as It's taking up some memory and won't be used again.
### Water Generation
Next up is the water generation, the point of this world generation system is start by creating good coasts then populate the island from there, so first we assign the water flags. You could use boolean values to track whether each corner or centre has water, is a coast, or is an ocean, however i decided to use an Enum with the System.Flags attribute instead as i believe it is cleaner.

To assign which polygons are water and which are not we use some maths set out in AssignWaterLayer to generate a nice island shape, after that we iterate through the Centre nodes of the map and use an average of how many corners are water to decide if the Centre is water or land.

Lastly for the water generation we use a flood fill algorithm running from the map borders to assign which water Corners are Ocean, since all the ocean polygons will be touching, each adjacent ocean corner will also be ocean, then if we reach a corner that isnt water, it must be a coast. After that we can iterate through the centres again to decide which centres are oceans and coasts.

I decided to do river generation further along the generation as it makes more sense for rivers to start from areas of high elevation and flow down to the ocean.
### Elevation Assignment
Elevation assignment is a more difficult task, I ended up messing with a couple methods and in the end it turned out ok, but for the project i wanted to build this generation into, I would need more flat land.

Firstly I iterated through each Corner on the graph that is not ocean and traversed the graph until i found a coast Corner, measuring the distance from the coast to use as the elevation. Although this worked, I later found this method to be particularly slow when increasing the voronoi point count of the graph. 

For some other algorithms to work instead of elevation being up to an undefined number it must be between 0 and 1, 0 being sea level. This is what RedistributeElevationLayer is for. To redistribute the elevation we calculate the highest elevation using Linq (List.Max) then iterate through each Corner and normalize it using this max elevation value.

The part that i did keep the same through the methods i used was the Centre elevation assignment, much like the water generation i simply iterated over each Centre (this time, just the Centres that arent ocean) and used the average elevation of it's corners.

Although this method worked for prototyping and allowed me to finish the 2D and 3D renderers, i found for more practical use it would need to a be lot faster so i did some more research. I found that a breadth first search algorithm would not only be a lot faster but would achieve the same result. Another pro of this algorithm was that very easy to calculate the downstreams as we traverse uip the downstream in the algorithm. Before i implemented this i had a seperate step which i will explain in the river generation to calculate all the downstreams.

I mentioned above that one of the issues i have with finding the elevation from the distance from the coast is that there is no flat land, this means it becomes very difficult to potentially place forests, towns, roads and other features into the generator. To combat this i tried rounding the elevation to steps such as 0.1 or 0.2, this means that the map ends up with defined flat layers, which I am a fan of.
### Temperature Assignment
Since I didn't need complex biome generation at this point, temperature generation was easy. First of all i created fields in MapGenerationOptions for the north, south and equator temperatures, then i just iterated through every Corner and assigned the temperature based on the Y value of the Corner. After that I iterate through the centres and once again use the average of it's corners to assign it's temperature.
### River Generation
The last step for the world generator currently is the river generation which at first, consisted of 2 steps: downstream calculation and river generation. To calculate the downstreams we can iterate through every corner and find the adjacent corner with the lowest elevation, which would be the downstream.

For generating the rivers, there were multiple methods I could've used. I decided to go for random river generation, which actually ended up providing some very nice rivers. To do this we iterate x number of times (as set in MapGenerationOptions via the riverIterations field), then pick a random Corner that doesn't have the water flag, and add 1 to the river value. Then we traverse down through each Corner's downslope Corner, adding 1 to each river and the edge connecting them, until reaching the coast. As long as the downslopes are set correctly this doesn't have a risk of an infinite loop, although maybe more failsafes could have been put in place.
## So What's Next?
For now I am happy with the world generator in it's current state and it could be ported to any engine without much issue. There is no copyright on this code, meaning you can use it wherever you want with no restrictions. If you do by chance use my code, it would be appreciated if I was credited (or just the author of the orignal blog that walked through the steps required for this generator), but this isn't a requirement. 

This repo is unmaintained by me and I'm unsure if i will come back to it, as such this code is provided as it is and there may or may not be updates in the future.
