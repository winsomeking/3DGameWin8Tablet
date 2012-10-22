using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Windows.UI.Input;
using Windows.UI.Core;
using Windows.Devices.Sensors;
using CommonDX;

namespace SharpDX_Windows_8_Abstraction
{
    // Super class for all game objects.
    public enum GameObjectType
    {
        None, Player, Enemy
    }
    public class GameObject
    {
        public GameObjectType type = GameObjectType.None;
        public Game game;

        public GameObject(Game game)
        {
            this.game = game;
        }

        public virtual void Tapped(GestureRecognizer sender, TappedEventArgs args)
        {

        }
       

        public virtual void OnManipulationStarted(GestureRecognizer sender, ManipulationStartedEventArgs args)
        {

        }

        public virtual void OnManipulationUpdated(GestureRecognizer sender, ManipulationUpdatedEventArgs args)
        {

        }

        public virtual void OnManipulationCompleted(GestureRecognizer sender, ManipulationCompletedEventArgs args)
        {

        }

        public virtual void KeyDown(KeyEventArgs arg)
        {

        }

        public virtual void KeyUp(KeyEventArgs arg)
        {

        }

        public virtual void Update(float timeDelta)
        {
            
        }

        public virtual void Render(TargetBase render, float rotX, float rotY, float rotZ)
        {

        }
    }
}