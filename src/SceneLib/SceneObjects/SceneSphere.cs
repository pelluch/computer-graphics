using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneLib
{
    public class SceneSphere : SceneObject
    {
        public Vector Center { get; set; }
        public float Radius { get; set; }
        public Vector Speed { get; set; }
        private int textureWidth;
        private int textureHeight;
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
                if (value.TextureImage != null)
                {
                    textureWidth = value.TextureImage.Width;
                    textureHeight = value.TextureImage.Height;
                }
            }

        }

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
            Vector newCenter;
            if (Speed.IsBlack())
                newCenter = Center;
            else
            {
                newCenter = this.Center + ray.Time * this.Speed;
            }

           
            //e - c
            Vector eMinusC = ray.Start - newCenter;
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
                        double cos = (record.HitPoint.z - newCenter.z) / Radius;
                        if (cos < -1)
                            cos = -1;
                        else if (cos > 1)
                            cos = 1;

                        double theta = Math.Acos(cos);
                        double phi = Math.Atan2(record.HitPoint.y - newCenter.y, record.HitPoint.x - newCenter.x);
                        if (phi < 0)
                            phi += 2 * Math.PI;

                        float u = (float)(phi / (2 * Math.PI));
                        float v = (float)((Math.PI - theta) / Math.PI);
                        //Console.WriteLine(this.textureWidth);
                        //Console.WriteLine(this.Material.TextureImage.Height);
                        //int i = (int)(u * (this.textureWidth - 1) + 0.5);
                        //int j = (int)(v * (this.textureHeight - 1) + 0.5);
                        record.TextureColor = this.Material.GetTexturePixelColor(u, v);
                    }

                    return true;
                }

            }
            return false;
        }

    }
}
