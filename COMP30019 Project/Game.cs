using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.UI.Xaml;
using SharpDX;
using SharpDX.Direct3D11;
using CommonDX;
using Windows.UI.Input;
using Windows.UI.Core;
using Windows.System;
using Windows.Devices.Sensors;
using Windows.UI.Xaml.Controls;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace SharpDX_Windows_8_Abstraction
{
    public class Game
    {

        // User Interface
        public MainPage mainPage;
        Console console;

        // Graphical assets.
        public Assets assets;

        // Random number generator
        public Random random;

        // Time keeping.
        public Stopwatch clock;
        public float time;
        public float game_time_limit;
        private float previousTime;

        // Transformation matrices.
        public Matrix world;
        public Matrix view;
        public Matrix proj;
        public Matrix worldViewProj;

        // World boundaries that indicate where the edge of the screen is for the camera.
        public float boundaryLeft;
        public float boundaryRight;
        public float boundaryTop;
        public float boundaryBottom;

        // Running game objects.
        public List<GameObject> gameObjects;
        private Stack<GameObject> addedGameObjects;
        private Stack<GameObject> removedGameObjects;

        GameInput input = new GameInput();


        //For Push and Popping of Matrices
        public Stack<Matrix> worldMatrices = new Stack<Matrix>();
        public Stack<Matrix> viewMatrices = new Stack<Matrix>();
        public Stack<Matrix> projMatrices = new Stack<Matrix>();

        private DeviceManager deviceManager;
        private Buffer constantBuffer;

        private Player player;
        private Terrain game_terrain;

        private CameraController cameraController = new CameraController();
        
        private double cameraNextPosX;
        private double cameraNextPosZ;
        private float differenceXZ;
        private float differenceY;
        private float prevCameraAngleXZ = 0;
        private float prevCameraAngleY = 0;
        private float prevDifferenceY;


        const int MAP_SIZE = 256;

        // Set up the light, a grey ambient light and a white positional light
        // The positional light is currently located above and behind the cube
        // (because it's + in Y and + in Z)
        Vector4 lightAmbCol = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
        Vector4 lightPntCol = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        Vector4 lightPntPos = new Vector4((float)MAP_SIZE / 2.0f, 300.0f, (float)MAP_SIZE / 2.0f, 1.0f);

        //cbuffer structure which is used by shaders
        struct S_SHADER_GLOBALS
        {
            Matrix projectionMatrix;
            Vector4 cameraPosition;
            Vector4 ambientColour;
            Vector4 pointPosition;
            Vector4 pointColour;
            
            public S_SHADER_GLOBALS(Matrix pM, Vector4 cP, Vector4 aC, Vector4 pP, Vector4 pC)
            {
                this.projectionMatrix = pM;
                this.cameraPosition = cP;
                this.ambientColour = aC;
                this.pointPosition = pP;
                this.pointColour = pC;
            }
        }



        // Constructor/initaliser for Game.
        public Game(MainPage mainPage, Assets assets, DeviceManager deviceManager)
        {
            // Set references to 
            game_time_limit = 90.0f; // 1 minute time limit
            this.mainPage = mainPage;
            this.assets = assets;
            this.deviceManager = deviceManager;
            this.game_terrain = new Terrain(this, MAP_SIZE);
            game_terrain.generate();
            game_terrain.rectangle();
          
            // Initialise random.
            random = new Random();

            // Initialise clock and time;
            clock = new Stopwatch();
            time = 0f;
            previousTime = 0f;
            clock.Start();

            // Set boundaries.
            boundaryLeft = 0.0f; // In x-axis
            boundaryRight = (float)MAP_SIZE; // In x-axis
            boundaryTop = 100f;
            boundaryBottom = -100f;

            // Initialise game object containers.
            gameObjects = new List<GameObject>();
            addedGameObjects = new Stack<GameObject>();
            removedGameObjects = new Stack<GameObject>();

            // Initialise event handling.
            input.window.KeyDown += KeyDown;
            input.window.KeyUp += KeyUp;
            input.gestureRecognizer.Tapped += Tapped;
            input.gestureRecognizer.ManipulationStarted += OnManipulationStarted;
            input.gestureRecognizer.ManipulationUpdated += OnManipulationUpdated;
            input.gestureRecognizer.ManipulationCompleted += OnManipulationCompleted;

            // Initialise the transformation matrices and constant buffer.
            world = Matrix.Identity;
            view = Matrix.Identity;
            proj = Matrix.Identity;
            worldViewProj = Matrix.Identity;
            constantBuffer = new SharpDX.Direct3D11.Buffer(deviceManager.DeviceDirect3D, Utilities.SizeOf<S_SHADER_GLOBALS>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            deviceManager.ContextDirect3D.VertexShader.SetConstantBuffer(0, constantBuffer);

            // Set camera
            // view = Matrix.LookAtLH(new Vector3(0, 0, -10), Vector3.Zero, Vector3.UnitY);
            view = cameraController.getViewProj();
            //Matrix.LookAtLH(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            proj =  cameraController.getProj();
            
            //Matrix.PerspectiveFovLH(1, 1, 0.001f, 100);

            // Set console as layout.
            console = new Console();
            mainPage.SetLayout(console);

            // Create game objects.
            player = new Player(this);
            lightPntPos = new Vector4(player.pos.X, player.pos.Y + 30, player.pos.Z - 20, 1.0f);
            cameraController.lookAt(new Vector3(player.pos.X, player.pos.Y + 30, player.pos.Z - 20), new Vector3(player.pos.X, player.pos.Y, player.pos.Z), new Vector3(0, 1, 0));
            Add(player);
            Add(game_terrain);
            Add(new EnemyController(this));

        }


        public void OnManipulationStarted(GestureRecognizer sender, ManipulationStartedEventArgs args)
        {
            // Pass Manipulation events to the game objects.
            foreach (var obj in gameObjects)
            {
                obj.OnManipulationStarted(sender, args);
            }
        }

        public void Tapped(GestureRecognizer sender, TappedEventArgs args)
        {
            // Pass Manipulation events to the game objects.
            foreach (var obj in gameObjects)
            {
                obj.Tapped(sender, args);
            }
        }

        public void OnManipulationUpdated(GestureRecognizer sender, ManipulationUpdatedEventArgs args)
        {
            // Pass Manipulation events to the game objects.
            foreach (var obj in gameObjects)
            {
                obj.OnManipulationUpdated(sender, args);
            }
        }

        public void OnManipulationCompleted(GestureRecognizer sender, ManipulationCompletedEventArgs args)
        {
            // Pass Manipulation events to the game objects.
            foreach (var obj in gameObjects)
            {
                obj.OnManipulationCompleted(sender, args);
            }
        }

        // Handler for key down event.
        public void KeyDown(object s, KeyEventArgs arg)
        {
            // Close the game if the player presses escape.

            if (arg.VirtualKey == VirtualKey.Escape)
            {
                Exit();
            }

            else if (arg.VirtualKey == VirtualKey.U)
            {
                cameraController.walk(-1f);

            }
            else if (arg.VirtualKey == VirtualKey.H)
            {

                cameraController.walk(1f);
            }
            else if (arg.VirtualKey == VirtualKey.I)
            {

                cameraController.strafe(-1f);

            }
            else if (arg.VirtualKey == VirtualKey.J)
            {
                cameraController.strafe(1f);

            }
            else if (arg.VirtualKey == VirtualKey.Down)
            {
                cameraController.pitch(0.005f);
            }
            else if (arg.VirtualKey == VirtualKey.Up)
            {
                cameraController.pitch(-0.005f);
            }
            else if (arg.VirtualKey == VirtualKey.T)
            {

                cameraController.rotateY(-0.005f);
            }
            else if (arg.VirtualKey == VirtualKey.R)
            {
                cameraController.rotateY(0.005f);
            }
            else if (arg.VirtualKey == VirtualKey.S)
            {
                var rootFrame = new Frame();
                rootFrame.Navigate(typeof(EndPage));

                // Place the frame in the current Window and ensure that it is active
                Window.Current.Content = rootFrame;
                Window.Current.Activate();

            }

            // Pass key events to the game objects.
            foreach (var obj in gameObjects)
            {
                obj.KeyDown(arg);
            }
        }

        // Handler for key up event.
        public void KeyUp(object s, KeyEventArgs arg)
        {
            // Pass key events to the game objects.
            foreach (var obj in gameObjects)
            {
                obj.KeyUp(arg);
            }
        }

        // Main loop that executes every frame.
        public void Update()
        {
          //FIX ME: uncomment this line of code to keep tracking the player. 

            cameraNextPosX = player.pos.X + 5 * Math.Cos(player.getAngleXZ());
            cameraNextPosZ = player.pos.Z + 5 * Math.Sin(player.getAngleXZ());

            if(cameraNextPosX < 0)
            {
                cameraNextPosX += MAP_SIZE;
            }
            else if (cameraNextPosX > MAP_SIZE)
            {
                cameraNextPosX -= MAP_SIZE;
            }

            if (cameraNextPosZ < 0)
            {
                cameraNextPosZ += MAP_SIZE;
            }
            else if (cameraNextPosZ > MAP_SIZE)
            {
                cameraNextPosZ -= MAP_SIZE;
            }

            differenceXZ = player.getAngleXZ() - prevCameraAngleXZ;
            differenceY = game_terrain.getWorldHeight((int)(cameraNextPosX), (int)(cameraNextPosZ)) - game_terrain.getWorldHeight((int)player.pos.X, (int)player.pos.Z);

            //if (player.pos.Y < game_terrain.getWorldHeight((int)(cameraNextPosX), (int)(cameraNextPosZ)))
            //{
            cameraController.lookAt(new Vector3((float)(player.pos.X - 30.0f * (float)Math.Cos(prevCameraAngleXZ)), player.pos.Y + 20.0f - 10.0f * ((float)Math.Exp(-Math.Abs(prevCameraAngleY))), (float)(player.pos.Z - 30.0f * (float)Math.Sin(prevCameraAngleXZ))), new Vector3(player.pos.X, player.pos.Y, player.pos.Z), new Vector3(0, 1, 0));
            //}
            //else
            //{
            //    cameraController.lookAt(new Vector3((float)(player.pos.X - 30.0f * (float)Math.Cos(prevCameraAngleXZ)), player.pos.Y + 20.0f, (float)(player.pos.Z - 30.0f * (float)Math.Sin(prevCameraAngleXZ))), new Vector3(player.pos.X, player.pos.Y, player.pos.Z), new Vector3(0, 1, 0));
            //}

            prevCameraAngleXZ += differenceXZ / 10.0f;
            prevCameraAngleY += differenceY / 100.0f;

            cameraController.updateViewMatrix();
            view = cameraController.getView();
                
            // Calculate timeDelta.
            time = clock.ElapsedMilliseconds / 1000f;
            var timeDelta = time - previousTime;
            previousTime = time;

            // Game time limiter
            game_time_limit -= timeDelta;
            if (game_time_limit <= 0.0f)
            {
                // Time is up, game is finished
                Remove(player);
            }

            flushAddedAndRemovedGameObjects();

            // Update game objects.
            foreach (var obj in gameObjects)
            {
                obj.Update(timeDelta);
            }
        }

        // Method to draw all game objects.
        public void Render(TargetBase render)
        {
            // Get the context (pipeline) from the TargetBase.
            var context = render.DeviceManager.ContextDirect3D;
            context.OutputMerger.SetTargets(render.DepthStencilView, render.RenderTargetView);

            // Clear depth buffer.
            context.ClearDepthStencilView(render.DepthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);
            context.ClearRenderTargetView(render.RenderTargetView, Colors.Black);

            S_SHADER_GLOBALS shaderGlobals = new S_SHADER_GLOBALS(worldViewProj, cameraController.getPos(), lightAmbCol, lightPntPos, lightPntCol);
            context.UpdateSubresource(ref shaderGlobals, constantBuffer);

            //var worldInvTrp = world;
            //worldInvTrp.Invert();
            //worldInvTrp.Transpose();
            //world.Transpose();

            //S_SHADER_GLOBALS shaderGlobals = new S_SHADER_GLOBALS(world, worldInvTrp, cameraController.getView(), cameraController.getPos(), lightAmbCol, lightPntPos, lightPntCol);
            //context.UpdateSubresource(ref shaderGlobals, constantBuffer);
                
            // Update game objects.
            foreach (var obj in gameObjects)
            {
                if (obj.type == GameObjectType.Player)
                {
                    obj.Render(render, player.getAngleYZ(), -player.getAngleXZ(), player.getAngleYX());
//                    obj.Render(render, 0, -player.getAngleXZ(), player.getAngleYX());
                }
                else
                {
                    obj.Render(render, 0, 0, 0);
                }
            }
        }


        // Add a new game object.
        public void Add(GameObject obj)
        {
            if (!gameObjects.Contains(obj) && !addedGameObjects.Contains(obj))
            {
                addedGameObjects.Push(obj);
            }
        }

        // Remove a game object.
        public void Remove(GameObject obj)
        {
            if (gameObjects.Contains(obj) && !removedGameObjects.Contains(obj))
            {
                removedGameObjects.Push(obj);
            }
        }

        // Count the number of game objects.
        public int Count()
        {
            return gameObjects.Count;
        }

        // Count the number of game objects for a certain type.
        public int Count(GameObjectType type)
        {
            int count = 0;
            foreach (var obj in gameObjects)
            {
                if (obj.type == type) { count++; }
            }
            return count;
        }

        // Return all game objects of a certain type.
        public List<T> Filter<T>() where T : GameObject
        {
            var list = new List<T>();
            foreach (var obj in gameObjects)
            {
                if (obj is T)
                {
                    list.Add((T)obj);
                }
            }
            return list;
        }

        // Process the buffers of game objects that need to be added/removed.
        private void flushAddedAndRemovedGameObjects()
        {
            while (addedGameObjects.Count > 0) { gameObjects.Add(addedGameObjects.Pop()); }
            while (removedGameObjects.Count > 0) { gameObjects.Remove(removedGameObjects.Pop()); }
        }

        // Quit the game.
        public void Exit()
        {
            Remove(player);
            //Application.Current.Exit();
        }

        // Push the current world matrix onto the world matrix stack.
        public void PushWorldMatrix()
        {
            worldMatrices.Push(Matrix.Identity * this.world);
        }

        // Push the current view matrix onto the view matrix stack.
        public void PushViewMatrix()
        {
            viewMatrices.Push(Matrix.Identity * this.view);
        }

        // Push the current proj matrix onto the proj matrix stack.
        public void PushProjMatrix()
        {
            projMatrices.Push(Matrix.Identity * this.proj);
        }

        // Pop the top matrix off the world matrix stack and set it as the current world matrix.
        public void PopWorldMatrix()
        {
            this.world = worldMatrices.Pop();
        }

        // Pop the top matrix off the view matrix stack and set it as the current view matrix.
        public void PopViewMatrix()
        {
            this.view = viewMatrices.Pop();
        }

        // Pop the top matrix off the proj matrix stack and set it as the current proj matrix.
        public void PopProjMatrix()
        {
            this.proj = projMatrices.Pop();
        }

        public void updateConstantBuffer()
        {
           //worldViewProj = world * view * proj;
            worldViewProj = world * cameraController.getView() * cameraController.getProj();
            worldViewProj.Transpose();
            deviceManager.ContextDirect3D.UpdateSubresource(ref worldViewProj, constantBuffer, 0);
        }

        public Terrain getTerrain()
        {
            return game_terrain;
        }

        public float getAccelX()
        {
            return input.getAccelReading("x");
        }

        public void resetTimeLimit()
        {
            game_time_limit = 90.0f;
        }
    }
}
