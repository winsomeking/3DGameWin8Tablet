using System;
using System.Collections.Generic;
using System.Diagnostics;

// SharpDX
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;


namespace SharpDX_Windows_8_Abstraction
{
    public struct Model
    {
        public Shape[] shapeList;
    }

    public struct Shape
    {
        public VertexBufferBinding vertexBinding;
        public PrimitiveTopology topology;
        public ShaderResourceView textureView;
        public Style style;
        public int vertexCount;
    }

    public struct Style
    {
        public InputLayout layout;
        public VertexShader vertexShader;
        public PixelShader pixelShader;
        public int floatsPerVertex;
    }

    public enum StyleType
    {
        Textured, Coloured
    }

    public enum GameObjectType
    {
        None, Player, Enemy
    }
}