using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneLib
{
    class SceneCylinder : SceneObject
    {

        public Vector BasePoint { get; set; }
        public float Radius { get; set; }
        private Vector heightDirection; 
        public Vector HeightDirection 
        {
            get { return heightDirection; }
            set
            {
                heightDirection = value;
                heightDirection.Normalize3();
            }
        }
        public float Height { get; set; }

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

        public override Vector SurfaceNormal(Vector point, Vector cameraDirection)
        {
            Vector center = Vector.Dot3(point - this.BasePoint, this.HeightDirection) * this.HeightDirection + this.BasePoint;
            Vector normal = point - center;
            normal.Normalize3();

            float similarity = Vector.Dot3(normal, -cameraDirection);
            if (similarity < 0)
            {
                normal = -normal;
            }
            return normal;
        }

        public override bool IsHit(Ray ray, HitRecord record, float near, float far)
        {
            //(d.h)h
            Vector dDotHTimesH = Vector.Dot3(ray.Direction, HeightDirection) * HeightDirection;
            Vector deltaP = ray.Start - this.BasePoint;
            Vector deltaPDotHTimesH = Vector.Dot3(deltaP, HeightDirection) * HeightDirection;
            Vector deltaPMinusdeltaPDotHTimesH = deltaP - deltaPDotHTimesH;
            Vector dMinusdDotHTimesH = ray.Direction - dDotHTimesH;
            
            float B = 2 * Vector.Dot3(dMinusdDotHTimesH, deltaPMinusdeltaPDotHTimesH);
            float A = Vector.Dot3(dMinusdDotHTimesH, dMinusdDotHTimesH);
            float C = Vector.Dot3(deltaPMinusdeltaPDotHTimesH, deltaPMinusdeltaPDotHTimesH) - this.Radius * this.Radius;
            Vector diff, worldCoords;
            float discriminant = B * B - 4 * A * C;
            float t1 = 0, t2 = 0, first_t = 0;
            float pointHeight = 0.0f;

            if (discriminant >= 0)
            {
                t1 = (-B - (float)Math.Sqrt(discriminant)) / (2 * A);
                t2 = (-B + (float)Math.Sqrt(discriminant)) / (2 * A);

                if (t1 >= 0)
                {
                    first_t = t1;
                      // t*d
                    diff = ray.Direction * first_t;
                    // e + t*d
                    worldCoords = diff + ray.Start;
                    pointHeight = Vector.Dot3(worldCoords - this.BasePoint, this.HeightDirection);
                    if (pointHeight < 0 || pointHeight > this.Height)
                    {
                        first_t = t2;
                    }
                }
                else if (t2 >= 0)
                    first_t = t2;
                else
                    return false;
            }
            else
                return false;

            
            // t*d
            diff = ray.Direction * first_t;
            // e + t*d
            worldCoords = diff + ray.Start;
            pointHeight = Vector.Dot3(worldCoords - this.BasePoint, this.HeightDirection);
            if (pointHeight < 0 || pointHeight > this.Height)
                return false;

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
                return true;
            }
            return false;
        }
    }
}
