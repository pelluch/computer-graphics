using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace SceneLib
{
    public class SceneBackground
    {
        public Vector Color {get; set;}
        public Vector AmbientLight { get; set; }

    }

    public class SceneLight
    {
        public float AtenuationConstant { get; set; }
        public float AtenuationLinear { get; set; }
        public float AtenuationQuadratic { get; set; }
        public Vector Color { get; set; }
        public Vector Position { get; set; }
        public Vector A { get; set; }
        public Vector B { get; set; }

    }

    public class SceneCamera
    {
        public Vector Position { get; set; }
        public Vector Target { get; set; }
        public Vector Up { get; set; }
        public Vector U { get; set; }
        public Vector V { get; set; }
        public Vector W { get; set; }
        public float FieldOfView { get; set; }
        public float NearClip { get; set; }
        public float FarClip { get; set; }        
    }

    public class SceneMaterial
    {
        public Bitmap TextureImage;
        private IntPtr ptrBitmap;

        public Bitmap BumpImage;
        private IntPtr ptrBumpBitmap;

        public Bitmap NormalImage;
        private IntPtr ptrNormalBitmap;

        public string Name { get; set; }
        public string TextureFile { get; set; }
        public string BumpFile { get; set; }
        public string NormalFile { get; set; }
        public Vector Diffuse { get; set; }
        public Vector Specular { get; set; }
        public Vector Transparent { get; set; }
        public Vector Reflective { get; set; }
        public Vector RefractionIndex { get; set; }
        public float Shininess { get { return Specular.w; } set { Specular.w = value; } }
        public IntPtr PtrBitmap {get { return ptrBitmap; } }
        public IntPtr PtrBumpBitmap { get { return ptrBumpBitmap; } }
        public IntPtr PtrNormalBitmap { get { return ptrNormalBitmap; } }

        public SceneMaterial()
        {
            Specular = new Vector(0, 0, 0);
            Diffuse = new Vector(0, 0, 0);
            Reflective = new Vector(0, 0, 0);
        }

        public Vector GetTexturePixelColor(int x, int y)
        {
            if (TextureImage == null)
                return null;
           
            Color c = TextureImage.GetPixel(x, y);
            Vector color = new Vector(c.R/255.0f, c.G/255.0f, c.B/255.0f, c.A/255.0f);
            return color;
        }        

        public void CreatePointer() 
        {
            BitmapData data = this.TextureImage.LockBits(new Rectangle(0, 0, TextureImage.Height, TextureImage.Width),
            ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            
            ptrBitmap = data.Scan0;

            this.TextureImage.UnlockBits(data);

        }
               
        
    }

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

    public class SceneSphere : SceneObject
    {
        public Vector Center { get; set; }
        public float Radius { get; set; }
        public SceneMaterial Material { get; set; }

        public override Vector SurfaceNormal(Vector point, Vector eyeDirection)
        {
            Vector surfaceNormal = point - this.Center;
            surfaceNormal.Normalize3();
            float similarity = Vector.Dot3(surfaceNormal, -eyeDirection);
            if (similarity < 0)
            {
                surfaceNormal = -surfaceNormal;
            }
            return surfaceNormal;
        }

       
        public override bool IsHit(Ray ray, HitRecord record, float near, float far)
        {
            //e - c
            Vector eMinusC = ray.Start - this.Center;
            //d.(e-c)
            float dDotEMinusC = Vector.Dot3(ray.Direction, eMinusC);
            float dDotD = Vector.Dot3(ray.Direction, ray.Direction);
            //d.(e-c)^2 - (d.d)((e-c).(e-c) - R^2))
            float discriminant = dDotEMinusC * dDotEMinusC - dDotD * (Vector.Dot3(eMinusC, eMinusC) - this.Radius * this.Radius);
            //Intersection occurs
            float t1 = 0, t2 = 0, first_t = 0;
            if (discriminant >= 0)
            {
                t1 = (-dDotEMinusC + (float)Math.Sqrt(discriminant)) / dDotD;
                t2 = (-dDotEMinusC - (float)Math.Sqrt(discriminant)) / dDotD;
                Vector diff;
                Vector worldCoords;

                if (t2 >= 0)
                    first_t = t2;
                else if (t1 >= 0)
                    first_t = t1;
                else
                    return false;

                // t*d
                diff = ray.Direction * first_t;
                // e + t*d
                worldCoords = diff + ray.Start;
                float distance = diff.Magnitude3();

                if (distance <= record.Distance && distance <= ray.MaximumTravelDistance)
                {

                    if (ray.UseBounds)
                    {
                        float cameraOrthogonalDistance = Math.Abs(Vector.Dot3(diff, ray.CameraLookDirection));
                        if (cameraOrthogonalDistance > far && cameraOrthogonalDistance < near)
                        {
                            return false;
                        }
                    }
                    
                    record.T = first_t;
                    record.HitPoint = ray.Start + diff;
                    record.Distance = distance;
                    record.Material = this.Material;

                    if (Material.TextureImage != null)
                    {
                        double theta = Math.Abs(Math.Acos((record.HitPoint.z - this.Center.z) / Radius));
                        double phi = Math.Abs(Math.Atan2(record.HitPoint.y - Center.y, record.HitPoint.x - Center.x));
                        float u = (float)(phi / (2 * Math.PI));
                        float v = (float)((Math.PI - theta) / Math.PI);
                        int i = (int)(u * (Material.TextureImage.Width - 1) + 0.5);
                        int j = (int)(v * (Material.TextureImage.Height - 1) + 0.5);
                        record.TextureColor = this.Material.GetTexturePixelColor(i, j);
                    }

                    return true;
                }
                
            }
        return false;
        }
        
    }

    public class LightSphere : SceneSphere
    {
        public override bool IsHit(Ray ray, HitRecord record, float near, float far)
        {
            if (!ray.UseBounds)
                return false;

            return base.IsHit(ray, record, near, far);
        }

        public override Vector SurfaceNormal(Vector point, Vector eyeDirection)
        {
            return -1.0f*base.SurfaceNormal(point, eyeDirection);
        }
    }

    public class SceneTriangle : SceneObject
    {
        public List<Vector> Vertex { get; set; }
        public List<Vector> VertexClip { get; set; }
        public List<Vector> Normal { get; set; }
        public List<Vector> Colors { get; set; }
        public List<SceneMaterial> Materials { get; set; }
        public List<float> U { get; set; }
        public List<float> V { get; set; }

        public bool InClipCoords = false;
     

        public SceneTriangle()
        {
            Vertex = new List<Vector>();
            VertexClip = new List<Vector>();
            Normal = new List<Vector>();
            Materials = new List<SceneMaterial>();
            Colors = new List<Vector>();
            U = new List<float>();
            V = new List<float>();
        }

        //Returns non-barometric surface normal
        public override Vector SurfaceNormal(Vector point, Vector eyeDirection)
        {
            //Triangles have to be correctly set!
            Vector surfaceNormal = Vector.Cross3(Vertex[2] - Vertex[0], Vertex[1] - Vertex[0]);
            surfaceNormal.Normalize3();
            float similarity = Vector.Dot3(surfaceNormal, -1*eyeDirection);
            if (similarity < 0)
            {
                surfaceNormal = surfaceNormal * (-1.0f);
            }
            return surfaceNormal;
        }

        public override bool IsHit(Ray ray, HitRecord record, float near, float far)
        {
            //Vamos precalculando variables auxiliares para facilitar el calculo y aumentar eficiencia
            //Convencion de nombres usada es la misma del libro "Fundamentals of Computer Graphics":
            /*
             * | a d g | | beta  |   | j |
             * | b e h | | gamma | = | k |
             * | c f i | |   t   | = | l |
            */

            float a = Vertex[0].x - Vertex[1].x;
            float b = Vertex[0].y - Vertex[1].y;
            float c = Vertex[0].z - Vertex[1].z;

            float d = Vertex[0].x - Vertex[2].x;
            float e = Vertex[0].y - Vertex[2].y;
            float f = Vertex[0].z - Vertex[2].z;

            float g = ray.Direction.x;
            float h = ray.Direction.y;
            float i = ray.Direction.z;

            float j = Vertex[0].x - ray.Start.x;
            float k = Vertex[0].y - ray.Start.y;
            float l = Vertex[0].z - ray.Start.z;

            float eiMinushf = e * i - h * f;
            float gfMinusdi = g * f - d * i;
            float dhMinuseg = d * h - e * g;

            float akMinusjb = a * k - j * b;
            float jcMinusal = j * c - a * l;
            float blMinuskc = b * l - k * c;

            float M = a * eiMinushf + b * gfMinusdi + c * dhMinuseg;
            //e + td = punto triangulo
            float t = -(f * akMinusjb + e * jcMinusal + d * blMinuskc) / M;
            if (t >= 0 )
            {
                float gamma = (i * akMinusjb + h * jcMinusal + g * blMinuskc) / M;
                if (gamma >= 0)
                {
                    float beta = (j * eiMinushf + k * gfMinusdi + l * dhMinuseg) / M;
                    //alpha = 1 - beta - gamma
                    if (beta >= 0 && 1 - beta - gamma >= 0)
                    {
                        Vector diff = t * ray.Direction;
                        float distance = diff.Magnitude3();
                        if (distance <= record.Distance && distance <= ray.MaximumTravelDistance)
                        {

                            if (ray.UseBounds)
                            {
                                float cameraOrthogonalDistance = Math.Abs(Vector.Dot3(diff, ray.CameraLookDirection));
                                if (cameraOrthogonalDistance > far && cameraOrthogonalDistance < near)
                                {
                                    return false;
                                }
                            }
                            record.T = t;
                            record.HitPoint = ray.Start + diff;
                            record.Distance = distance;
                            record.Material = Materials[0];

                            return true;
                        }
                    }
                }
            }
            return false;
        }      
    }

    public class SceneModel : SceneObject
    {
        public string FileName { get; set; }
        public List<SceneTriangle> Triangles { get; set; }

        public int NumTriangles { get { return Triangles == null ? 0 : Triangles.Count; } }

        public SceneModel(string name, string filename)
        {
            Name = name;
            Triangles = new List<SceneTriangle>();
            FileName = filename;
        }

        public override Vector SurfaceNormal(Vector point, Vector eyeDirection)
        {
            throw new NotImplementedException();
        }

        public SceneTriangle GetTriangle(int index)
        {
            return Triangles == null || index < 0 || index > Triangles.Count ? null : Triangles[index];
        }
        public override bool IsHit(Ray ray, HitRecord record, float near, float far)
        {
            throw new NotImplementedException();
        }
    }
}
