using System;
using System.Collections.Generic;
using System.Diagnostics;

// SharpDX
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using SharpDX.DXGI;
using CommonDX;
using Windows.UI.Core;
using SharpDX.IO;

namespace SharpDX_Windows_8_Abstraction
{
    public class Style
    {
        public InputLayout layout;
        public VertexShader vertexShader;
        public PixelShader pixelShader;
        public int floatsPerVertex;

        public Style(String vertexShaderFilename, String pixelShaderFilename, InputElement[] layoutElements, int floatsPerVertex, DeviceManager deviceManager)
        {
            var path = Windows.ApplicationModel.Package.Current.InstalledLocation.Path; 

            // Read pre-compiled shader byte code relative to current directory
            var vertexShaderByteCode = NativeFile.ReadAllBytes(path + "\\" + vertexShaderFilename);
            this.pixelShader = new PixelShader(deviceManager.DeviceDirect3D, NativeFile.ReadAllBytes(path + "\\" + pixelShaderFilename));
            this.vertexShader = new VertexShader(deviceManager.DeviceDirect3D, vertexShaderByteCode);

            // Specify the input layout for the new style
            this.layout = new InputLayout(deviceManager.DeviceDirect3D, vertexShaderByteCode, layoutElements);
            this.floatsPerVertex = floatsPerVertex;
        }
    }
}
