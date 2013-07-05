using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SceneLib;

namespace Renderer
{
    public class Fragment
    {
        public Vector RasterizedPosition;
        public Vector WorldPosition;
        public Vector Normal;
        public SceneMaterial Material;
        public float U;
        public float V;
        public Vector BlinnPhongColor;
        public bool HasTexture = false;


        public Fragment()
        {
        }

        public Fragment(Fragment v1, Fragment v2, float t)
        {
            this.RasterizedPosition = v1.RasterizedPosition + t * (v2.RasterizedPosition - v1.RasterizedPosition);
            this.Normal = v1.Normal + t * (v2.Normal - v1.Normal);
            this.U = v1.U + t * (v2.U - v1.U);
            this.V = v1.V + t * (v2.V - v1.V);
        }
    }
}
