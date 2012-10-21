/**
 *  Terrain.cs
 *
 *  Defines an object for the terrain for the game, Beware the Rhino.
 *  Includes methods for automatically generating a terrain with the Plasma
 *  Fractal.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Windows;

using Windows.UI.Input;
using Windows.UI.Core;
using Windows.System;
using Windows.Devices.Sensors;
using CommonDX;

using SharpDX_Windows_8_Abstraction;
namespace SharpDX_Windows_8_Abstraction
{

    public class Terrain : VisibleGameObject
    {
        // keep heights in a 2 dimensional matrix. Allocated on instantiation.
        private float[,] world;
        private int grid_len; // maximum index in the world array.

        // these arrays store the vectors that define the boundaries of square
        // cells in the map. ul, ur, ll, lr are the upper and lower left and right
        // corners accordingly of each square cell.
        float[] floatArray;
        Vertices[] vertices;

        Vector4 red = new Vector4(1.0f, 0f, 0.0f, 1.0f);
        Vector4 brown = new Vector4(0.5f, 0.5f, 0.0f, 1.0f);
        Vector4 green = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
        Vector4 yellow = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);
        Vector4 darkYellow = new Vector4(184/255.0f,134/255.0f,11/255.0f,1.0f);
        Vector4 blue = new Vector4(0.0f, 0.0f, 1.0f, 1.0f);
        Vector4 lightBlue = new Vector4(135/255.0f,206/255.0f,1.0f,1.0f);


        

        float WorldBottom = 100;
        float WorldTop = 0;

        // roughness factor determines what the terrain looks like. The bigger it
        // is, the more we have exaggerated mountains and ravines.
        const float roughness = 1.0f;
        // TODO: what value should this be? Perhaps initialise it in the constructor?

        // random number generator.
        Random rand;

        /**
         *	constructor. Parameter is the side length of the world grid. This ensures
         *	that the array is square.
         */
        public Terrain(Game game, int n) : base(game)
        {

            // TODO: check that n is a power of 2?
            this.world = new float[n + 1, n + 1];
            this.grid_len = n;
            this.rand = new Random();
            this.pos = new Vector3(0, 0, 0);

            // if the world has NxN vertices, there are (N-1)x(N-1) squares.
            this.floatArray = new float[n * n * 72 + 72];
            this.vertices = new Vertices[n * n * 6 + 6];
        }

        /**
         *	wrapper for recursive Plasma Fractal algorithm. Seeds the four corners
         *	of the grid with randomly asigned initial values.
         */
        public void generate()
        {
            // seed the four corners.
            world[0, 0] = displace(grid_len);
            world[0, grid_len] = displace(grid_len);
            world[grid_len, 0] = displace(grid_len);
            world[grid_len, grid_len] = displace(grid_len);

            // recursively generate the terrain.
            plasma_fractal(new point(0, 0), grid_len);
        }

       /**
         *	this is the main part of the Plasma Fractal algorithm. We are given a
         *	square defined by an upper left coordinate (closest to the origin) and
         *	a side length. We will find the midpoints of the edges and assign them
         *	a height interpolated from the adjacent corners, and the center of the
         *	square will be assigned the average of the midpoints plus a random
         *	offset. The range of values from which the offset is selected is dependant
         *	on the side length of the current sector.
         */
        private void
        plasma_fractal(point u_left, int len)
        {
            // midpoint coordinates.
            int x_mid = u_left.x + (len / 2);
            int y_mid = u_left.y + (len / 2);

            // base case is when we are left with a block of four cells, where len
            // is 1. In this case, there is nothing to be done; those four cells
            // have all been assigned their final heights.
            if (len <= 1)
            {
                return;
            }

            // interpolate heights for midpoints of edges.
            // top edge.
            world[x_mid, u_left.y] = (world[u_left.x, u_left.y] + world[u_left.x + len, u_left.y]) / 2;
            // bottom edge.
            world[x_mid, u_left.y + len] = (world[u_left.x, u_left.y + len] + world[u_left.x + len, u_left.y + len]) / 2;
            // left edge.
            world[u_left.x, y_mid] = (world[u_left.x, u_left.y] + world[u_left.x, u_left.y + len]) / 2;
            // right edge.
            world[u_left.x + len, y_mid] = (world[u_left.x + len, u_left.y] + world[u_left.x + len, u_left.y + len]) / 2;

            // interpolate middle cell, then add a random displacement.
            world[x_mid, y_mid] = (world[x_mid, u_left.y] +
              world[x_mid, u_left.y + len] + world[u_left.x, y_mid] +
              world[u_left.x + len, y_mid]) / 4;
            world[x_mid, y_mid] += displace(len);

            if (world[x_mid, y_mid] > WorldTop) {WorldTop = world[x_mid, y_mid]; }
            if (world[x_mid, y_mid] < WorldBottom) { WorldBottom = world[x_mid, y_mid]; }

            // recursively process the four smaller segments.
            plasma_fractal(u_left, len / 2);
            plasma_fractal(new point(x_mid, u_left.y), len / 2);
            plasma_fractal(new point(u_left.x, y_mid), len / 2);
            plasma_fractal(new point(x_mid, y_mid), len / 2);
        }

        /**
         *	calculate a random displacement within a range dependant on the side
         *	length of a sector. One parameter, being that side length.
         */
        private float displace(int side_len)
        {
            // range depends on the size of side_len with reference to the size of
            // the whole grid.
            float range = ((float)side_len / (float)grid_len) * roughness;
            return ((float)rand.Next(0,100) - 0.5f) * range;
        }

        // Return the height value (i.e. "y" value)
        public float getWorldHeight(int x, int z)
        {
            return world[x, z];
        }

        // Return the size value of the map 
        public int getGridlen()
        {
            return grid_len;
        }

//        public Vector4[] rectangle()
        public void rectangle()
        {
            for (int i = 0; i < grid_len; i++)
            {
                for (int j = 0; j < grid_len; j++)
                {
                    // Terrain facing toward ground
                    /*
                    vertices[(i * grid_len + j) * 6] = new Vertices(j,world[j,i],i,ColorChoose(world[j, i]));
                    vertices[(i * grid_len + j) * 6 + 1] = new Vertices(j+1,world[j+1,i],i,ColorChoose(world[j+1, i]));
                    vertices[(i * grid_len + j) * 6 + 2] = new Vertices(j,world[j,i+1],i+1,ColorChoose(world[j, i+1]));
                    vertices[(i * grid_len + j) * 6 + 3] = new Vertices(j,world[j,i+1],i+1,ColorChoose(world[j, i+1]));
                    vertices[(i * grid_len + j) * 6 + 4] = new Vertices(j+1,world[j+1,i],i,ColorChoose(world[j+1, i]));
                    vertices[(i * grid_len + j) * 6 + 5] = new Vertices(j+1, world[j+1, i+1], i+1, ColorChoose(world[j+1, i+1]));
                    */

                    // Terrain facing toward sky
                    vertices[(i * grid_len + j) * 6] = new Vertices(j, world[j, i], i, ColorChoose(world[j, i]));
                    vertices[(i * grid_len + j) * 6 + 1] = new Vertices(j, world[j, i + 1], i + 1, ColorChoose(world[j, i + 1]));
                    vertices[(i * grid_len + j) * 6 + 2] = new Vertices(j + 1, world[j + 1, i], i, ColorChoose(world[j + 1, i]));
                    vertices[(i * grid_len + j) * 6 + 3] = new Vertices(j, world[j, i + 1], i + 1, ColorChoose(world[j, i + 1]));
                    vertices[(i * grid_len + j) * 6 + 4] = new Vertices(j + 1, world[j + 1, i + 1], i + 1, ColorChoose(world[j + 1, i + 1]));
                    vertices[(i * grid_len + j) * 6 + 5] = new Vertices(j + 1, world[j + 1, i], i, ColorChoose(world[j + 1, i]));

                }
            }

            /*
            // Making a plane of square on y=0
            vertices[grid_len * grid_len * 6] = new Vertices(0, 0, 0, red);
            vertices[grid_len * grid_len * 6 + 1] = new Vertices(0, 0, grid_len, red);
            vertices[grid_len * grid_len * 6 + 2] = new Vertices(grid_len, 0, 0, red);
            vertices[grid_len * grid_len * 6 + 3] = new Vertices(0, 0, grid_len, red);
            vertices[grid_len * grid_len * 6 + 4] = new Vertices(grid_len, 0, grid_len, red);
            vertices[grid_len * grid_len * 6 + 5] = new Vertices(grid_len, 0, 0, red);
            */

            //FIX ME: Add normal calculation here. It should be normalised.
            




            
            for (int i = 0; i < (grid_len * grid_len * 6 /*+ 6*/)/3; i++)
            {
                Vector3 faceNormal = new Vector3();

                Vector3 p0 = new Vector3(vertices[i].x,vertices[i].y,vertices[i].z);
                Vector3 p1 = new Vector3(vertices[i+1].x, vertices[i+1].y, vertices[i+1].z);
                Vector3 p2 = new Vector3(vertices[i+2].x, vertices[i+2].y, vertices[i+2].z);

                faceNormal = computeFaceNormal(p0, p1, p2);

                floatArray[i * 36] = (float)vertices[i * 3].x;
                floatArray[i * 36 + 1] = (float)vertices[i * 3].y;
                floatArray[i * 36 + 2] = (float)vertices[i * 3].z;
                floatArray[i * 36 + 3] = (float)vertices[i * 3].m;
                floatArray[i * 36 + 4] = vertices[i * 3].color[0];
                floatArray[i * 36 + 5] = vertices[i * 3].color[1];
                floatArray[i * 36 + 6] = vertices[i * 3].color[2];
                floatArray[i * 36 + 7] = vertices[i * 3].color[3];
                floatArray[i * 36 + 8] = faceNormal.X;  //vertices[i].nx;
                floatArray[i * 36 + 9] = faceNormal.Y;  //vertices[i].ny;
                floatArray[i * 36 + 10] = faceNormal.Z; //vertices[i].nz;
                floatArray[i * 36 + 11] = 1.0f;          //vertices[i].nm;

                floatArray[i * 36 + 12] = (float)vertices[i * 3 + 1].x;
                floatArray[i * 36 + 13] = (float)vertices[i * 3 + 1].y;
                floatArray[i * 36 + 14] = (float)vertices[i * 3 + 1].z;
                floatArray[i * 36 + 15] = (float)vertices[i * 3 + 1].m;
                floatArray[i * 36 + 16] = vertices[i * 3 + 1].color[0];
                floatArray[i * 36 + 17] = vertices[i * 3 + 1].color[1];
                floatArray[i * 36 + 18] = vertices[i * 3 + 1].color[2];
                floatArray[i * 36 + 19] = vertices[i * 3 + 1].color[3];
                floatArray[i * 36 + 20] = faceNormal.X; //vertices[i].nx;
                floatArray[i * 36 + 21] = faceNormal.Y; //vertices[i].ny;
                floatArray[i * 36 + 22] = faceNormal.Z; //vertices[i].nz;
                floatArray[i * 36 + 23] = 1.0f;          //vertices[i].nm;

                floatArray[i * 36 + 24] = (float)vertices[i * 3 + 2].x;
                floatArray[i * 36 + 25] = (float)vertices[i * 3 + 2].y;
                floatArray[i * 36 + 26] = (float)vertices[i * 3 + 2].z;
                floatArray[i * 36 + 27] = (float)vertices[i * 3 + 2].m;
                floatArray[i * 36 + 28] = vertices[i * 3 + 2].color[0];
                floatArray[i * 36 + 29] = vertices[i * 3 + 2].color[1];
                floatArray[i * 36 + 30] = vertices[i * 3 + 2].color[2];
                floatArray[i * 36 + 31] = vertices[i * 3 + 2].color[3];
                floatArray[i * 36 + 32] = faceNormal.X; //vertices[i].nx;
                floatArray[i * 36 + 33] = faceNormal.Y; //vertices[i].ny;
                floatArray[i * 36 + 34] = faceNormal.Z; //vertices[i].nz;
                floatArray[i * 36 + 35] = 1.0f;          //vertices[i].nm;

            }
            
           
            
            
            model = game.assets.CreateTerrain("terrain", floatArray);
        }

        private Vector3 computeFaceNormal(Vector3 p0, Vector3 p1, Vector3 p2)
        {

            Vector3 u = p1 - p0;
            Vector3 v = p2 - p0;

            Vector3 tempU = new Vector3(u.X,u.Y,u.Z);
            Vector3 tempV = new Vector3(v.X,v.Y,v.Z);

            Vector3 normalisedVector = Vector3.Normalize(Vector3.Cross(tempU, tempV));
            //Vector4 normalisedVector = new Vector4(normalisedVectorTemp.X,normalisedVectorTemp.Y,normalisedVectorTemp.Z,1);
            return normalisedVector;
        }

        protected Vector4 ColorChoose(float height)
        {
            // This is using forced color on different section of height
            
            if (height < 10.0f)
            {
                return blue;
            }
            else if (height < 30.0f)
            {
                return lightBlue;
            }
            else if (height < 40.0f)
            {
                return yellow;
            }
            else if (height < 50.0f)
            {
                return darkYellow;
            }
            else if (height < 60.0f)
            {
                return green;
            }
            else if (height < 90.0f)
            {
                return brown;
            }
            else
            {
                return red;
            }
            

            //This is using color based on height
            //return new Vector4(height/WorldTop,0.5f - (height-50)/WorldTop,1.0f - (height / WorldTop),1.0f);
        }
    }

    

    /**
     *  class defining a point in the grid.
     */
    public class point
    {
        // x and y coordinates.
        public int x, y;

        // constructor method. Initialise coordinates.
        public point(int new_x, int new_y)
        {
            this.x = new_x;
            this.y = new_y;
        }
    }

    /**
     *  class defining a point in the grid.
     */
    public class Vertices
    {
        // x and y coordinates.
        public int x, z;
        public float y;
        public Vector4 color;
        public int m = 1;

        // constructor method. Initialise coordinates.
        public Vertices(int x, float y, int z, Vector4 color)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.color = color;
        }
    }
}
