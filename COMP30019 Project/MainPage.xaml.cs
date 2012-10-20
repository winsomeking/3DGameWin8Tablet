using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Graphics.Display;

using CommonDX;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SharpDX_Windows_8_Abstraction
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : SwapChainBackgroundPanel
    {
        DeviceManager deviceManager;
        SwapChainBackgroundPanelTarget target;
        Assets assets;
        Game game;
        MainPage swapchainPanel;
        static private List<IDisposable> disposableList;

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Used to set the layout overlayed over the SharpDX game screen.
        /// </summary>
        public void SetLayout(UIElement element)
        {
            Children.Clear();
            Children.Add(element);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            disposableList = new List<IDisposable> { };

            // Safely dispose any previous instance
            // Creates a new DeviceManager (Direct3D, Direct2D, DirectWrite, WIC)
            deviceManager = new DeviceManager();

            // Place the frame in the current Window and ensure that it is active
            swapchainPanel = new MainPage();
            Window.Current.Content = swapchainPanel;
            Window.Current.Activate();

            // Use CoreWindowTarget as the rendering target (Initialize SwapChain, RenderTargetView, DepthStencilView, BitmapTarget)
            target = new SwapChainBackgroundPanelTarget(swapchainPanel);

            // Initializes the Swap Chain target, game and game assets when the device manager is initialized
            deviceManager.OnInitialize += (dm) =>
            {
                target.Initialize(dm);
                assets = new Assets(dm);
                game = new Game(swapchainPanel, assets, dm);
            };

            // Initialize the device manager and all registered deviceManager.OnInitialize 
            deviceManager.Initialize(DisplayProperties.LogicalDpi);

            // Render the game objects within the CoreWindow
            target.OnRender += game.Render;

            // Setup rendering callback
            CompositionTarget.Rendering += CompositionTarget_Rendering;

            // Callback on DpiChanged
            DisplayProperties.LogicalDpiChanged += DisplayProperties_LogicalDpiChanged;

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
    }
}
