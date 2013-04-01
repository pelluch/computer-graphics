using SceneLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneLib
{
    public class Ray
    {
        private Vector start;
        private bool isShadow;
        public bool IsShadow
        { get { return isShadow; } }

        public Vector Start
        { get { return start; } }

        private Vector direction;
        private Vector cameraLookDirection;
        public Vector CameraLookDirection
        { get { return cameraLookDirection; } }

        public Vector Direction
        {  get { return direction; }}

        public Ray(Vector eye, Vector rayDirection)
        {
            this.start = eye;
            this.direction = rayDirection;
            isShadow = true;
        }

        public Ray(Vector eye, Vector rayDirection, Vector cameraLookDirection, float near, float far)
        {
            this.start = eye;
            this.cameraLookDirection = cameraLookDirection;
            this.direction = rayDirection;
            isShadow = false;
        }
    }
}
