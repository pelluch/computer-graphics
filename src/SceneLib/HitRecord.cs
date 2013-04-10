using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneLib
{
    public class HitRecord
    {
        private float t;
        public float T
        {
            get { return t; }
            set { t = value; }
        }


        private Vector hitPoint;        
        public Vector HitPoint
        {
            get { return hitPoint; }
            set { hitPoint = value; }
        }

        private Vector lightVector;
        public Vector LightVector
        {
            get { return lightVector; }
            set { lightVector = value; }
        }

        private float distance;
        public float Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        private SceneMaterial material;
        public SceneMaterial Material
        {
            get { return material; }
            set { material = value; }
        }

        private Vector textureColor;
        public Vector TextureColor
        {
            get { return textureColor; }
            set { textureColor = value; }
        }
        public HitRecord()
        {
            this.distance = float.MaxValue;
            this.material = new SceneMaterial();
            this.textureColor = new Vector();
        }

       
    }
}
