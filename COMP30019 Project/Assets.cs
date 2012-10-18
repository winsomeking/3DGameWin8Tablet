using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Windows;

//SharpDX
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using CommonDX;
using SharpDX.IO;

namespace SharpDX_Windows_8_Abstraction
{
    /*
     * A manager class for different vertex buffers organised into Shapes and Models.
     * Encapsulates the InputLayout and Shaders as Styles that are currently predefined as an enum in StyleType.
     * Provides utility functions for creating generic box shapes with textures on each side.
     */
    public enum StyleType
    {
        Textured, Coloured
    }
    public class Assets
    {
        private DeviceManager deviceManager;

        // Dictionary of currently loaded models.
        // New/existing models are loaded by calling GetModel(modelName, modelMaker).
        public Dictionary<String, Model> modelDict = new Dictionary<String, Model>();

        // Dictionary of currently loaded textures.
        // New/existing textures are loaded by calling GetTexture(textureName).
        public Dictionary<String, ShaderResourceView> textureDict = new Dictionary<String, ShaderResourceView>();

        // Dictionary of InputLayout & Shader styles.
        // Gets preloaded when the asset manager is created.
        public Dictionary<StyleType, Style> styleDict = new Dictionary<StyleType, Style>();



        public Assets(DeviceManager deviceManager)
        {
            // Establish reference to program.
            this.deviceManager = deviceManager;

            // Initialise styles.
            styleDict[StyleType.Textured] = createTexturedStyle();
            styleDict[StyleType.Coloured] = createColouredStyle();
        }

        // Create a shape that's ready to be drawn with the pipeline.
        public Shape createShape(float[] floatArray, PrimitiveTopology topology, String textureName, Style style)
        {
            var vertices = Buffer.Create(deviceManager.DeviceDirect3D, BindFlags.VertexBuffer, floatArray);
            var vertexBufferBinding = new VertexBufferBinding(vertices, Utilities.SizeOf<float>() * style.floatsPerVertex, 0);
            return new Shape(vertexBufferBinding, topology, GetTexture(textureName), style, floatArray.Length / style.floatsPerVertex);
        }

        // Load a texture from the texture dictionary.
        // If the texture hasn't been loaded before it will be loaded from images/<texture name>.png
        // If the texture name is null then null will be returned.
        public ShaderResourceView GetTexture(String textureName){
            if (textureName == null) { return null; }
            if (!textureDict.ContainsKey(textureName)) {
                // Load texture and create sampler
                using (var bitmap = TextureLoader.LoadBitmap(deviceManager.WICFactory, "images/" + textureName))
                using (var texture2D = TextureLoader.CreateTexture2DFromBitmap(deviceManager.DeviceDirect3D, bitmap))
                    textureDict[textureName] = new ShaderResourceView(deviceManager.DeviceDirect3D, texture2D);
            }
            return textureDict[textureName];
        }

        // Load a model from the model dictionary.
        // If the model name hasn't been loaded before then modelMaker will be called to generate the model.
        public delegate Model ModelMaker ();
        public Model GetModel(String modelName, ModelMaker modelMaker)
        {
            if (!modelDict.ContainsKey(modelName))
            {
                modelDict[modelName] = modelMaker();
            }
            return modelDict[modelName];
        }

        // Generate an input style suitable for using with textured shapes.
        private Style createTexturedStyle()
        {
            
            //FIX ME: add a new input element to describe normal elements.
            return new Style(
                "shaders\\VS_TEXTURED.cso",
                "shaders\\PS_TEXTURED.cso",
                new[]
                {
                    new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                    new InputElement("TEXCOORD", 0, Format.R32G32_Float, 12, 0),
                },
                5,
                deviceManager
            );
        }

        // Generate an input style suitable for using with coloured shapes.
        private Style createColouredStyle()
        {
            return new Style(
                "shaders\\VS_COLOURED.cso",
                "shaders\\PS_COLOURED.cso",
                new[]
                {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                    new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0),
                    new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 32, 0),
                },
                12,
                deviceManager
            );
        }




        // Create a cube with one texture for all faces.
        public Model CreateTexturedCube(String textureName, float size)
        {
            return CreateTexturedBox(textureName, new Vector3(size, size, size));
        }

        // Create an arbitrarily sized box (rectangular prism) with individual textures for each face.
        // Note that it is far more efficient to create fewer, more complex objects (i.e. a single cube rather than 6 squares)
        // and this should be done whenever possible
        public Model CreateTexturedBox(String textureName, Vector3 size)
        {
            Style style = styleDict[StyleType.Textured];

            // Vertex definitions for a shape
            var shapeArray = new float[] {
                    -1.0f, -1.0f, -1.0f,     0.0f, 1.0f,
                    -1.0f,  1.0f, -1.0f,     0.0f, 0.0f,
                     1.0f,  1.0f, -1.0f,     1.0f, 0.0f,
                    -1.0f, -1.0f, -1.0f,     0.0f, 1.0f,
                     1.0f,  1.0f, -1.0f,     1.0f, 0.0f,
                     1.0f, -1.0f, -1.0f,     1.0f, 1.0f,

                    -1.0f, -1.0f,  1.0f,     1.0f, 1.0f,
                     1.0f,  1.0f,  1.0f,     0.0f, 0.0f,
                    -1.0f,  1.0f,  1.0f,     1.0f, 0.0f,
                    -1.0f, -1.0f,  1.0f,     1.0f, 1.0f,
                     1.0f, -1.0f,  1.0f,     0.0f, 1.0f,
                     1.0f,  1.0f,  1.0f,     0.0f, 0.0f,

                    -1.0f, 1.0f, -1.0f,      0.0f, 1.0f,
                    -1.0f, 1.0f,  1.0f,      0.0f, 0.0f,
                     1.0f, 1.0f,  1.0f,      1.0f, 0.0f,
                    -1.0f, 1.0f, -1.0f,      0.0f, 1.0f,
                     1.0f, 1.0f,  1.0f,      1.0f, 0.0f,
                     1.0f, 1.0f, -1.0f,      1.0f, 1.0f,

                    -1.0f,-1.0f, -1.0f,      0.0f, 0.0f,
                     1.0f,-1.0f,  1.0f,      1.0f, 1.0f,
                    -1.0f,-1.0f,  1.0f,      0.0f, 1.0f,
                    -1.0f,-1.0f, -1.0f,      0.0f, 0.0f,
                     1.0f,-1.0f, -1.0f,      1.0f, 0.0f,
                     1.0f,-1.0f,  1.0f,      1.0f, 1.0f,

                    -1.0f, -1.0f, -1.0f,     1.0f, 1.0f,
                    -1.0f, -1.0f,  1.0f,     0.0f, 1.0f,
                    -1.0f,  1.0f,  1.0f,     0.0f, 0.0f,
                    -1.0f, -1.0f, -1.0f,     1.0f, 1.0f,
                    -1.0f,  1.0f,  1.0f,     0.0f, 0.0f,
                    -1.0f,  1.0f, -1.0f,     1.0f, 0.0f,

                     1.0f, -1.0f, -1.0f,     0.0f, 1.0f,
                     1.0f,  1.0f,  1.0f,     1.0f, 0.0f,
                     1.0f, -1.0f,  1.0f,     1.0f, 1.0f,
                     1.0f, -1.0f, -1.0f,     0.0f, 1.0f,
                     1.0f,  1.0f, -1.0f,     0.0f, 0.0f,
                     1.0f,  1.0f,  1.0f,     1.0f, 0.0f,
            };

            // Scale all the vertices by the size.
            {
                for (int i = 0; i < shapeArray.Length; i += style.floatsPerVertex)
                {
                    shapeArray[i + 0] *= size.X / 2;
                    shapeArray[i + 1] *= size.Y / 2;
                    shapeArray[i + 2] *= size.Z / 2;
                }
            }
            
            // Return the model with all six sides.
            return new Model{shapeList = new Shape[]{
                createShape(shapeArray, PrimitiveTopology.TriangleList, textureName, style),
            }};
        }

        ///////////////////////////////////////////////////////////////////////
        public Model CreateTerrain(String textureName, float[] floatArray)
        {
            Style style = styleDict[StyleType.Coloured];

            // Vertex definitions for a shape
            

            // Scale all the vertices by the size.
            

            // Return the model with all six sides.
            return new Model
            {
                shapeList = new Shape[]{
                createShape(floatArray, PrimitiveTopology.TriangleList, null, style),
            }
            };
        }
        ///////////////////////////////////////////////////////////////////////
        
        public Model CreateSurface(String textureName, Vector3 size)
        {
            Style style = styleDict[StyleType.Textured];

            // Vertex definitions for a shape
            var surfaceArray = new float[] {
                     1.0f, -1.0f, -1.0f,     0.0f, 1.0f,
                     1.0f,  1.0f,  1.0f,     1.0f, 0.0f,
                     1.0f, -1.0f,  1.0f,     1.0f, 1.0f,
                     1.0f, -1.0f, -1.0f,     0.0f, 1.0f,
                     1.0f,  1.0f, -1.0f,     0.0f, 0.0f,
                     1.0f,  1.0f,  1.0f,     1.0f, 0.0f,
            };

            // Scale all the vertices by the size.
            {
                for (int i = 0; i < surfaceArray.Length; i += style.floatsPerVertex)
                {
                    surfaceArray[i + 0] *= size.X / 2;
                    surfaceArray[i + 1] *= size.Y / 2;
                    surfaceArray[i + 2] *= size.Z / 2;
                }
            }

            // Return the model with all six sides.
            return new Model
            {
                shapeList = new Shape[]{
                createShape(surfaceArray, PrimitiveTopology.TriangleList, textureName, style),
            }
            };
        }

    }

}