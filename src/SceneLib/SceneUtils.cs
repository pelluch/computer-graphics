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

        private object lockObject = new object();
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

            Vector color;
            lock (lockObject)
            {
                Color c = TextureImage.GetPixel(x, y);
                color = new Vector(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f, c.A / 255.0f);
            }
            return color;
        }

        //Interpolation
        public Vector GetTexturePixelColor(float u, float v)
        {
            if (TextureImage == null)
                return null;

            Vector color = new Vector();
            

            lock (lockObject)
            {
                float x = u * (TextureImage.Width - 1);
                float y = v * (TextureImage.Height - 1);

                float left = (float)Math.Floor(x);
                float right = (float)Math.Ceiling(x);
                float top = (float)Math.Floor(y);
                float bottom = (float)Math.Ceiling(y);
                float deltaX = x - left;
                float deltaY = y - top;

                Color c1, c2, c3, c4;

                c1 = TextureImage.GetPixel((int)left, (int)top);
                c2 = TextureImage.GetPixel((int)right, (int)top);
                c3 = TextureImage.GetPixel((int)left, (int)bottom);
                c4 = TextureImage.GetPixel((int)right, (int)bottom);

                Vector v1 = new Vector(c1);
                Vector v2 = new Vector(c2);
                Vector v3 = new Vector(c3);
                Vector v4 = new Vector(c4);

                Vector topColor = v1 * (1 - deltaX) + v2 * deltaX;
                Vector bottomColor = v3 * (1 - deltaX) + v4 * deltaX;
                color = topColor * (1 - deltaY) + bottomColor * deltaY;
                //color = new Vector(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f, c.A / 255.0f);
            }

            return color;
        }       

        public void CreatePointer() 
        {
            BitmapData data = this.TextureImage.LockBits(new Rectangle(0, 0, TextureImage.Width, TextureImage.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            ptrBitmap = data.Scan0;
            this.TextureImage.UnlockBits(data);
        }
               
        
    }

   

    

    public class LightSphere : SceneSphere
    {

        public override Vector SurfaceNormal(Vector point, Vector eyeDirection)
        {
            return -1.0f*base.SurfaceNormal(point, eyeDirection);
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
