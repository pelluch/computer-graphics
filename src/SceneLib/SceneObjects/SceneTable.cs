using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneLib.SceneObjects
{
    class SceneTable : SceneObject
    {

        private SceneMaterial material;
        public SceneMaterial Material
        {
            get
            {
                return material;
            }
            set
            {
                material = value;
            }

        }

        private List<SceneObject> components;
        public float Width
        { get; set; }

        public float Height
        { get; set; }

        public float Radius
        { get; set; }

        public void Initialize(Plane mainPlane)
        {
            mainPlane.Name = "Top plane";
            components = new List<SceneObject>();
            mainPlane.Initialize();
            components.Add(mainPlane);
            List<Vector> topVertex = mainPlane.Vertex;
            List<Vector> bottomVertex = new List<Vector>();

            for (int i = 0; i < topVertex.Count; i++)
            {
                Vector A = topVertex[i];
                Vector B = topVertex[(i + 1)%4];
                Vector C = A + mainPlane.PlaneNormal*Width;
                bottomVertex.Add(C);
                Vector D = B + mainPlane.PlaneNormal*Width;
                List<Vector> newVertices = new List<Vector>();
                newVertices.Add(A);
                newVertices.Add(B);
                newVertices.Add(D);
                newVertices.Add(C);
                Plane p = new Plane();
                p.Name = "Plane number " + i;
                p.Material = this.Material;
                p.Initialize(newVertices);
                if(i == 3 || i == 2 || i == 1 || i == 0)
                 components.Add(p);

                SceneCylinder cylinder = new SceneCylinder();
                cylinder.Name = "Cylinder " + i;
                cylinder.Radius = this.Radius;
                cylinder.Material = this.Material;
                Vector diff = mainPlane.Center - A;
                diff.Normalize3();

                cylinder.BasePoint = C + diff * Radius*2;
                cylinder.EndPoint = C + diff * Radius*2 + mainPlane.PlaneNormal * Height;
                components.Add(cylinder);
            }
            Plane bottomPlane = new Plane();
            bottomPlane.Name = "Bottom plane";
            bottomPlane.Material = this.Material;
            bottomPlane.Initialize(bottomVertex);
            components.Add(bottomPlane);
        }

        public override Vector SurfaceNormal(Vector point, Vector cameraDirection)
        {
            throw new NotImplementedException();
        }

        public override bool IsHit(Ray ray, HitRecord record, float near, float far)
        {
            bool isHit = false;
            foreach (SceneObject obj in components)
            {
                bool currentHit  =  obj.IsHit(ray, record, float.MinValue, float.MaxValue);
                
                isHit = isHit || currentHit;
            }
            if (RenderingParameters.showMouse && isHit)
            {
                Console.WriteLine("Hit by " + record.ObjectName);
                Console.WriteLine("t: " + record.T);
                Console.WriteLine("Distance: " + record.Distance);
            }
            return isHit;
        }
    }
}
