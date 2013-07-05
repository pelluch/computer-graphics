using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneLib
{
    class SceneCylinder : SceneObject
    {
        private enum IntersectionType { Cylinder, Base, End };

        public Vector BasePoint { get; set; }
        private Vector endPoint;
        public Vector EndPoint 
        {
            get { return endPoint; }
            set 
            { 
                HeightDirection = value - BasePoint;
                HeightDirection.Normalize3();
                Tangent = Vector.Cross3(HeightDirection, BasePoint);
                Tangent.Normalize3();
                endPoint = value;
            }
        }
        public Vector Tangent
        { get; set; }
        public float Radius { get; set; }
        public Vector HeightDirection
        {
            get;
            set;
        }
        private float Height
        {
            get { return (EndPoint - BasePoint).Magnitude3(); }
        }
        private int textureWidth;
        private int textureHeight;
        private SceneMaterial material;
        public SceneMaterial Material
        {
            get;
            set;
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

        public Vector PlaneNormal(Vector point, Vector cameraDirection)
        {
            Vector normal = this.HeightDirection;
            float similarity = Vector.Dot3(normal, -cameraDirection);
            if (similarity < 0)
            {
                normal = -normal;
            }
            return normal;
        }

        public bool IsInside(Vector point)
        {
            float insideBase = Vector.Dot3(HeightDirection, point - this.BasePoint);
            float insideEnd = Vector.Dot3(HeightDirection, point - this.EndPoint);
            if (insideBase < 0 || insideEnd > 0)
               return false;
           else
               return true;

        }

        public bool IsCap(Vector capPoint, Vector intersection)
        {
            Vector diff = capPoint - intersection;
            float radius = Vector.Dot3(diff, diff);
            if (radius > this.Radius*this.Radius)
                return false;
            else
                return true;
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
            float t1 = 0, t2 = 0, first_t = float.MaxValue;
            List<float> candidates = new List<float>();
            IntersectionType intersectionType = IntersectionType.Cylinder;

            if (discriminant >= 0)
            {
                t1 = (-B - (float)Math.Sqrt(discriminant)) / (2 * A);
                t2 = (-B + (float)Math.Sqrt(discriminant)) / (2 * A);

                if (t1 >= 0 && IsInside(ray.Start + ray.Direction*t1))
                {
                    first_t = t1;
                }
                if (t2 >= 0 && IsInside(ray.Start + ray.Direction*t2) && t2 < first_t)
                {
                    first_t = t2;
                }
            }

            float hDotd = Vector.Dot3(this.HeightDirection, ray.Direction);
            if (hDotd != 0)
            {
                float basePlaneT = Vector.Dot3(this.HeightDirection, this.BasePoint) - Vector.Dot3(this.HeightDirection, ray.Start);
                basePlaneT = basePlaneT / hDotd;
                if (basePlaneT > 0 && IsCap(this.BasePoint, basePlaneT * ray.Direction + ray.Start) && basePlaneT < first_t)
                {
                    intersectionType = IntersectionType.Base;
                    first_t = basePlaneT;
                }

                float endPlaneT = Vector.Dot3(this.HeightDirection, this.EndPoint) - Vector.Dot3(this.HeightDirection, ray.Start);
                endPlaneT = endPlaneT / hDotd;
                if (endPlaneT > 0 && IsCap(this.EndPoint, endPlaneT * ray.Direction + ray.Start) && endPlaneT < first_t)
                {
                    intersectionType = IntersectionType.End;
                    first_t = endPlaneT;
                }
            }

            if (first_t == float.MaxValue)
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
                if (intersectionType == IntersectionType.Cylinder)
                {
                    record.SurfaceNormal = SurfaceNormal(record.HitPoint, ray.Direction);
                    if (Material.TextureImage != null)
                    {
                        float heightReached = Vector.Dot3(HeightDirection, record.HitPoint - this.BasePoint);
                        float u = heightReached / Height;
                        Vector circCenter = BasePoint + HeightDirection * heightReached;
                        Vector projection = record.HitPoint - circCenter;
                        projection.Normalize3();
                        float similarity = Vector.Dot3(projection, Tangent);
                        float theta = (float)Math.Acos(similarity);
                        float v = (float)((Math.PI - theta) / Math.PI);
                        v = v < 0 ? -v : v;
                        record.TextureColor = this.Material.GetTexturePixelColor(u, v);
                         // float v = 
                    }
                }
                else
                {
                    record.SurfaceNormal = PlaneNormal(record.HitPoint, ray.Direction);
                    if (Material.TextureImage != null)
                    {
                        Vector circCenter = intersectionType == IntersectionType.Base ? BasePoint : EndPoint;
                        Vector projection = record.HitPoint - circCenter;
                        float length = projection.Magnitude3();
                        projection.Normalize3();
                        float similarity = Vector.Dot3(projection, Tangent);
                        float theta = (float)Math.Acos(similarity);
                        float u = length / Radius;
                        float v = (float)((Math.PI - theta) / Math.PI);
                        v = v < 0 ? -v : v;
                        record.TextureColor = this.Material.GetTexturePixelColor(u, v);
                        // float v = 
                    }
                }
               // record.SurfaceNormal = SurfaceNormal(record.HitPoint, ray.Direction);
                return true;
            }
            return false;
        }
    }
}
