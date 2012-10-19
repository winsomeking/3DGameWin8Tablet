using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;

//Reference : Introduction to 
namespace SharpDX_Windows_8_Abstraction
{
    class CameraController
    {

        private Vector3 mPosition;
        private Vector3 mRight;
        private Vector3 mUp;
        private Vector3 mLook;

        private float mNearZ;
        private float mFarZ;
        private float mAspect;
        private float mFovY;
        private float mNearWindowHeight;
        private float mFarWindowHeight;

        private Matrix mView;
        private Matrix mProj;

        public CameraController()
        {
            mPosition = new Vector3(0.0f,0.0f,0.0f);
            mRight = new Vector3(1.0f,0.0f,0.0f);
            mUp = new Vector3(0.0f,1.0f,0.0f);
            mLook = new Vector3(0.0f,0.0f,1.0f);

            setLens((float)(0.25f*Math.PI),1.0f,1.0f,1000.0f);

        }

        public void setLens(float fovY, float aspect, float zn, float zf)
        {
            mFovY = fovY;
            mAspect = aspect;
            mNearZ = zn;
            mFarZ = zf;

            mNearWindowHeight = (float)(2.0f * mNearZ * Math.Tan(0.5f * mFovY));
            mFarWindowHeight  = (float)(2.0f*mFarZ*Math.Tan(0.5f*mFovY));

            Matrix p = Matrix.PerspectiveFovLH(mFovY,mAspect,mNearZ,mFarZ);

             mProj = p;

        }

        public void lookAt(Vector3 pos, Vector3 target, Vector3 up)
        {
            Vector3 temp_L = Vector3.Normalize(Vector3.Subtract(target,pos));
            Vector3 temp_R = Vector3.Normalize(Vector3.Cross(up,temp_L));
            Vector3 temp_U = Vector3.Cross(temp_L,temp_R);

            mPosition = pos;
            mLook = temp_L;
            mRight = temp_R;
            mUp = temp_U;
        }

        public Matrix getView()
        {

             return mView;
           
        }
        public Matrix getProj()
        {

            return mProj;
        }

        public Matrix getViewProj()
        {
            return Matrix.Multiply(getView(),getProj());

        }
        public Vector4 getPos()
        {
            Vector4 pos = new Vector4(mPosition.X,mPosition.Y,mPosition.Z,1.0f);
            return pos;
        }
        public void strafe(float dist)
        {

            Vector3 temp_s = new Vector3(dist);
            Vector3 temp_r = mRight;
            Vector3 temp_p = mPosition;

            mPosition = Vector3.Add(Vector3.Modulate(temp_s, temp_r), temp_p);

        }

        public void walk(float dist)
        {
            Vector3 temp_s = new Vector3(dist);
            Vector3 temp_l = mLook;
            Vector3 temp_p = mPosition;

            mPosition = Vector3.Add(Vector3.Modulate(temp_s, temp_l), temp_p);

        }

        public void pitch(float angle)
        {
            Matrix R = Matrix.RotationAxis(mRight,angle);
            
            mUp = Vector3.TransformNormal(mUp,R);
            mLook = Vector3.TransformNormal(mLook,R);

        }

        public void rotateY(float angle)
        {
            Matrix R = Matrix.RotationY(angle);

            mRight = Vector3.TransformNormal(mRight,R);
            mUp = Vector3.TransformNormal(mUp,R);
            mLook = Vector3.TransformNormal(mLook,R);

        }

        public void updateViewMatrix()
        {
            Vector3 R = mRight;
            Vector3 U = mUp;
            Vector3 L = mLook;
            Vector3 P = mPosition;

            L = Vector3.Normalize(L);
            U = Vector3.Normalize(Vector3.Cross(L,R));

            R = Vector3.Cross(U,L);

            float x = -Vector3.Dot(P,R);
            float y = -Vector3.Dot(P,U);
            float z = -Vector3.Dot(P,L);

            mRight = R;
            mUp = U;
            mLook = L;

            mView.M11 = mRight.X;
            mView.M21 = mRight.Y;
            mView.M31 = mRight.Z;
            mView.M41 = x;

            mView.M12 = mUp.X;
            mView.M22 = mUp.Y;
            mView.M32 = mUp.Z;
            mView.M42 = y;

            mView.M13 = mLook.X;
            mView.M23 = mLook.Y;
            mView.M33 = mLook.Z;
            mView.M43 = z;

            mView.M14 = 0.0f;
            mView.M24 = 0.0f;
            mView.M34 = 0.0f;
            mView.M44 = 1.0f;

        }

    }
}
