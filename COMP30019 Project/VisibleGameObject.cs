using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

using SharpDX;
using CommonDX;

namespace SharpDX_Windows_8_Abstraction
{
    // Super class for visible game objects that have a position, a model and a local transformation.
    public class VisibleGameObject : GameObject
    {
        public Model model;
        public Vector3 pos;
        public Matrix transformation;

        public VisibleGameObject(Game game) : base(game)
        {
            this.game = game;
            transformation = Matrix.Identity;
        }

        // Draw the model in the position having undergone the local transformation.
        public override void Render(TargetBase render, float rotX, float rotY, float rotZ)
        {
            game.PushWorldMatrix();
            game.world *= transformation * Matrix.RotationX(rotX) * Matrix.RotationZ(rotZ) * Matrix.RotationY(rotY) * Matrix.Translation(pos);

            game.updateConstantBuffer();

            model.Render(render);
            game.PopWorldMatrix();
        }
    }
}