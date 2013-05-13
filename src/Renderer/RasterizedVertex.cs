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

  
    }
}
