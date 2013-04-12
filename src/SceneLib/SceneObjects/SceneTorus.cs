using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneLib.SceneObjects
{
    class SceneTorus : SceneObject
    {
        public override Vector SurfaceNormal(Vector point, Vector cameraDirection)
        {
            throw new NotImplementedException();
        }

        public override bool IsHit(Ray ray, HitRecord record, float near, float far)
        {
            throw new NotImplementedException();
        }
    }
}
