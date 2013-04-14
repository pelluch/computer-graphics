using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneLib
{
    public class HitRecord
    {
  
        public float T { get; set; }
        public Vector HitPoint { get; set; }
        public Vector LightVector { get; set; }
        public float Distance { get; set; }
        public Vector SurfaceNormal { get; set; }
        public SceneMaterial Material { get; set; }
        public Vector TextureColor  { get; set; }
        public List<Vector> ShadedColors { get; set; }

        public void ClearVectors()
        {
            ShadedColors.Clear();
        }

        public HitRecord()
        {
            Distance = float.MaxValue;
            Material = new SceneMaterial();
            TextureColor = new Vector();
            ShadedColors = new List<Vector>();
        }

       
    }
}
