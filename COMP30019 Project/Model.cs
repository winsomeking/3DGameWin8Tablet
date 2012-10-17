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
    public class Model
    {
        public Shape[] shapeList;

        public void Render(TargetBase render)
        {
            foreach (var shape in shapeList)
            {
                shape.Render(render); 
            }
        }

    }
}
