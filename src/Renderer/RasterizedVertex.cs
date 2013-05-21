using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SceneLib;

namespace Renderer
{
    public class RasterizedVertex
    {
        public Vector Position;
        public Vector Normal;
        public SceneMaterial Material;
        public float U;
        public float V;
        public Vector BlinnPhongColor;
        public bool HasTexture = false;


        public RasterizedVertex()
        {
        }

        public RasterizedVertex(RasterizedVertex v1, RasterizedVertex v2, float t)
        {
            this.Position = v1.Position + t * (v2.Position - v1.Position);
            this.Normal = v1.Normal + t * (v2.Normal - v1.Normal);
            this.U = v1.U + t * (v2.U - v1.U);
            this.V = v1.V + t * (v2.V - v1.V);
        }
    }
}
