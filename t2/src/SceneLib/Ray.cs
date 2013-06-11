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
        private bool useBounds;
        public bool UseBounds
        { get { return useBounds; } }

        public Vector Start
        { get { return start; } }

        private Vector direction;
        private Vector cameraLookDirection;
        public Vector CameraLookDirection
        { get { return cameraLookDirection; } }

        public Vector Direction
        {  get { return direction; }}

        public float Time
        { get; set; }
        private float maximumTravelDistance;
        public float MaximumTravelDistance
        {
            set { maximumTravelDistance = value; }
             get { return maximumTravelDistance; }
        }

        public Ray(Vector eye, Vector rayDirection)
        {
            this.start = eye;
            this.direction = rayDirection;
            useBounds = false;
            maximumTravelDistance = float.MaxValue;
        }

        public Ray(Vector eye, Vector rayDirection, Vector cameraLookDirection, float near, float far)
        {
            this.start = eye;
            this.cameraLookDirection = cameraLookDirection;
            this.direction = rayDirection;
            useBounds = true;
            maximumTravelDistance = float.MaxValue;
        }
    }
}
