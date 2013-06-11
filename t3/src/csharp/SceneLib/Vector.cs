using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneLib
{
    public class Vector
    {
        public float x { get; private set; }
        public float y { get; private set; }
        public float z { get; private set; }
        public float w { get; set; }

        public Vector(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Vector(float x, float y, float z) 
            : this(x,y,z,1)
        {
        }

        public Vector(float x, float y)
            : this(x, y, 0, 1)
        {
        }

        public Vector()
            : this(0, 0, 0, 1)
        {
        }

        /// <summary>
        /// Returns the magnitude of a 3-dimensional vector
        /// </summary>
        /// <returns></returns>
        public float Magnitude3()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        /// <summary>
        /// Normalizes a 3-dimensional vector
        /// </summary>
        public Vector Normalize3()
        {
            float currentMagnitude = Magnitude3();
            x /= currentMagnitude;
            y /= currentMagnitude;
            z /= currentMagnitude;
            return this;
        }

        public Vector Clamp3()
        {
            x = Math.Min(x, 1);
            y = Math.Min(y, 1);
            z = Math.Min(z, 1);
            return this;
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z, v1.w + v2.w);
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z, v1.w - v2.w);
        }

        public static Vector operator *(float scalar, Vector v)
        {
            return new Vector(scalar * v.x, scalar * v.y, scalar * v.z, scalar*v.w);
        }

        public static Vector operator *(Vector v, float scalar)
        {
            return new Vector(scalar * v.x, scalar * v.y, scalar * v.z, scalar * v.w);
        }

        public static Vector operator *(Vector v1, Vector v2)
        {
            return new Vector(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z, v1.w * v2.w);
        }

        public static Vector operator /(Vector v, float scalar)
        {
            return new Vector(v.x / scalar, v.y / scalar, v.z / scalar, v.w / scalar);
        }

        public static Vector operator /(Vector v1, Vector v2)
        {
            return new Vector(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z, v1.w / v2.w);
        }

        public static Vector Cross3(Vector v1, Vector v2)
        {
            Vector crossVec = new Vector();
            crossVec.x = v1.y * v2.z - v2.y * v1.z;
            crossVec.y = v2.x * v1.z - v1.x * v2.z;
            crossVec.z = v1.x * v2.y - v2.x * v1.y;
            crossVec.w = 0.0f;
            return crossVec;
        }

        /// <summary>
        /// Dot product of a 3-dimensional vector
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static float Dot3(Vector v1, Vector v2)
        {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }

        public override string ToString()
        {
            return "x: " + this.x + ", y: " + this.y + ", z: " + this.z + ", w: " + this.w;
        }
    }
}
