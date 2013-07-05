using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneLib
{
    class Plane : SceneObject
    {
        public Vector PlaneNormal
        {
            get;
            set;
        }

        private SceneTriangle[] triangles;
        public Vector L1
        { get; set; }
        public Vector L2
        { get; set; }
        public Vector Center
        { get; set; }
        public float Height
        { get; set; }
        public List<Vector> Vertex
        { get; set; }

        public float Width
        { get; set; }

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

        public override Vector SurfaceNormal(Vector point, Vector cameraDirection)
        {
            Vector surfaceNormal = PlaneNormal;
            float similarity = Vector.Dot3(surfaceNormal, -cameraDirection);
            if (similarity < 0)
            {
                surfaceNormal = -surfaceNormal;
            }
            return surfaceNormal;
        }

        public void Initialize(List<Vector> vertex)
        {
            triangles = new SceneTriangle[2];
            Vector center = new Vector();
            foreach (Vector v in vertex)
                center += v;

            center = center / 4.0f;
            this.Center = center;

            this.PlaneNormal = Vector.Cross3(vertex[1] - vertex[0], vertex[2] - vertex[0]);
            this.PlaneNormal.Normalize3();

            SceneTriangle t1 = new SceneTriangle();
            SceneTriangle t2 = new SceneTriangle();
            triangles[0] = t1;
            triangles[1] = t2;
            t1.Vertex = new List<Vector>();
            t2.Vertex = new List<Vector>();

            t1.Vertex.Add(vertex[0]);
            t1.Vertex.Add(vertex[1]);
            t1.Vertex.Add(vertex[2]);

            t2.Vertex.Add(vertex[2]);
            t2.Vertex.Add(vertex[3]);
            t2.Vertex.Add(vertex[0]);

            t1.Materials = new List<SceneMaterial>();
            t2.Materials = new List<SceneMaterial>();
            t1.U = new List<float>();
            t2.U = new List<float>();
            t1.V = new List<float>();
            t2.V = new List<float>();
            for (int i = 0; i < 3; i++)
            {
                t1.Materials.Add(this.material);
                t2.Materials.Add(this.material);
                t1.U.Add(0);
                t1.V.Add(0);
                t2.U.Add(0);
                t2.V.Add(0);
            }
            
            this.Vertex = vertex;
        }

        public void Initialize()
        {
            L1.Normalize3();
            L1 = L1 * Width;

            L2.Normalize3();
            L2 = L2 * Height;

            PlaneNormal = Vector.Cross3(L1, L2);
            PlaneNormal.Normalize3();
            Vertex = new List<Vector>();
            triangles = new SceneTriangle[2];
            Vector A = Center + L1 + L2;
            Vector B = Center + L1 - L2;
            Vector C = Center - L1 - L2;
            Vector D = Center - L1 + L2;

            Vertex.Add(A);
            Vertex.Add(B);
            Vertex.Add(C);
            Vertex.Add(D);

            SceneTriangle t1 = new SceneTriangle();
            SceneTriangle t2 = new SceneTriangle();
            triangles[0] = t1;
            triangles[1] = t2;
            t1.Vertex = new List<Vector>();
            t2.Vertex = new List<Vector>();

            t1.Vertex.Add(A);
            t1.Vertex.Add(B);
            t1.Vertex.Add(C);

            t2.Vertex.Add(C);
            t2.Vertex.Add(D);
            t2.Vertex.Add(A);

            t1.Materials = new List<SceneMaterial>();
            t2.Materials = new List<SceneMaterial>();
            t1.U = new List<float>();
            t2.U = new List<float>();
            t1.V = new List<float>();
            t2.V = new List<float>();
            for (int i = 0; i < 3; i++)
            {
                t1.Materials.Add(this.material);
                t2.Materials.Add(this.material);
                t1.U.Add(0);
                t1.V.Add(0);
                t2.U.Add(0);
                t2.V.Add(0);
            }
            
        }

        public override bool IsHit(Ray ray, HitRecord record, float near, float far)
        {
            bool isHit = false;
            
            foreach (SceneTriangle triangle in triangles)
            {
                isHit = triangle.IsHit(ray, record, near, far) || isHit;    
            }
            if (isHit)
            {
                record.ObjectName = this.Name;
                if (Material.TextureImage != null)
                {
                    Vector l1 = Vertex[1] - Vertex[0];
                    Vector l2 = Vertex[3] - Vertex[0];
                    float f = l1.x;
                    float g = l2.x;
                    float h = record.HitPoint.x;
                    float i = Vertex[0].x;

                    float j = l1.y;
                    float k = l2.y;
                    float l = record.HitPoint.y;
                    float m = Vertex[0].y;

                    float n = l1.z;
                    float o = l2.z;
                    float p = record.HitPoint.z;
                    float q = Vertex[0].z;

                    float det = g * j - f * k;
                    if (det == 0)
                    {
                        det = o * j - n * k;
                        if (det == 0)
                        {
                            det = g * n - f * o;
                            j = n;
                            k = o;
                            l = p;
                            m = q;
                        }
                        else
                        {
                            f = n;
                            h = o;
                            h = p;
                            i = q;
                        }
                    }
                    if (det != 0)
                    {
                        float alpha = (g * l - g * m - h * k + i * k) / det;
                        float beta = (-f * l + f * m + h * j - i * j) / det;
                        //if (RenderingParameters.showMouse)
                        //{
                        //Console.WriteLine("alpha: " + alpha + "\tbeta: " + beta);

                        record.TextureColor = this.Material.GetTexturePixelColor(alpha, beta);
                    }
                   // }
                }
                
            }           
            return isHit;
        }
    }
}
