using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneLib
{
    public abstract class SceneObject
    {
        public string Name { get; set; }
        public Vector Scale { get; set; }
        public Vector Position { get; set; }
        public Vector Rotation { get; set; }

        public abstract Vector SurfaceNormal(Vector point, Vector cameraDirection);
        public abstract bool IsHit(Ray ray, HitRecord record, float near, float far);


        public SceneObject()
        {
            Scale = new Vector(1, 1, 1);
            Position = new Vector(0, 0, 0);
            Rotation = new Vector(0, 0, 0);
        }
    }
}
