// Copyright (c) 2010-2012 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommonDX;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Devices.Sensors;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace SharpDX_Windows_8_Abstraction
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        DeviceManager deviceManager;
        SwapChainBackgroundPanelTarget target;
        Assets assets;
        Game game;
        MainPage swapchainPanel;
        static private List<IDisposable> disposableList;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
       public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        
        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {

            disposableList = new List<IDisposable> { };

            // Safely dispose any previous instance
            // Creates a new DeviceManager (Direct3D, Direct2D, DirectWrite, WIC)
            deviceManager = new DeviceManager();
      
            // Place the frame in the current Window and ensure that it is active
            swapchainPanel = new MainPage();
            Window.Current.Content = swapchainPanel;
            Window.Current.Activate();

            //// Use CoreWindowTarget as the rendering target (Initialize SwapChain, RenderTargetView, DepthStencilView, BitmapTarget)
            //target = new SwapChainBackgroundPanelTarget(swapchainPanel);

            //// Initializes the Swap Chain target, game and game assets when the device manager is initialized
            //deviceManager.OnInitialize += (dm) =>
            //    {
            //        target.Initialize(dm);
            //        assets = new Assets(dm);
            //        game = new Game(swapchainPanel, assets, dm);
            //    };

            //// Initialize the device manager and all registered deviceManager.OnInitialize 
            //deviceManager.Initialize(DisplayProperties.LogicalDpi);

            //// Render the game objects within the CoreWindow
            //target.OnRender += game.Render;

            //// Setup rendering callback
            //CompositionTarget.Rendering += CompositionTarget_Rendering;

            //// Callback on DpiChanged
            //DisplayProperties.LogicalDpiChanged += DisplayProperties_LogicalDpiChanged;
        }

        void DisplayProperties_LogicalDpiChanged(object sender)
        {
            deviceManager.Dpi = DisplayProperties.LogicalDpi;
        }

        //  This is the rendering loop.  It is called for every new frame 
        void CompositionTarget_Rendering(object sender, object e)
        {
            // Update game logic for current frame
            game.Update();

            // Render all shapes to the backbuffer
            target.RenderAll();

            // Swap buffers
            target.Present();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        void OnSuspending(object sender, SuspendingEventArgs e)
        {
            
        }
    }
}
