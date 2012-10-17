using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDX;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.IO;
using Windows.UI.Core;

namespace SharpDX_Windows_8_Abstraction
{
    public class Shape
    {
        public VertexBufferBinding vertexBinding;
        public PrimitiveTopology topology;
        public ShaderResourceView textureView;
        public Style style;
        public int vertexCount;
        
        public Shape(VertexBufferBinding vertexBinding, PrimitiveTopology topology, ShaderResourceView textureView, Style style, int vertexCount)
        {
            this.vertexBinding = vertexBinding;
            this.topology = topology;
            this.textureView = textureView;
            this.style = style;
            this.vertexCount = vertexCount;
        }

        // Renders a shape to the backbuffer
        public void Render(TargetBase render)
        {
            var d3dContext = render.DeviceManager.ContextDirect3D;

            // Bind the vertex buffer and input layout
            d3dContext.InputAssembler.SetVertexBuffers(0, vertexBinding);
            d3dContext.InputAssembler.InputLayout = style.layout;

            // Set the topology if different from the previous topology
            if (d3dContext.InputAssembler.PrimitiveTopology != topology)
            {
                d3dContext.InputAssembler.PrimitiveTopology = topology;
            }

            // Set the vertex shader if different from the current vertex shader
            if (d3dContext.VertexShader.Get() != style.vertexShader)
            {
                d3dContext.VertexShader.Set(style.vertexShader);
            }

            // Set the pixel shader if different from the current pixel shader
            if (d3dContext.PixelShader.Get() != style.pixelShader)
            {
                d3dContext.PixelShader.Set(style.pixelShader);
            }
            d3dContext.PixelShader.SetShaderResource(0, textureView);

            // Draw the actual shape
            d3dContext.Draw(vertexCount, 0);
        }

    }
}
