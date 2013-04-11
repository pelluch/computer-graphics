using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneLib
{
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
            float similarity = Vector.Dot3(surfaceNormal, -1 * eyeDirection);
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
            if (t >= 0)
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
                            if (record.Material.TextureImage != null)
                            {
                                float alpha = 1 - beta - gamma;
                                float u = alpha * U[0] + beta * U[1] + gamma * U[2];
                                float v = alpha * V[0] + beta * V[1] + gamma * V[2];
                                record.TextureColor = record.Material.GetTexturePixelColor(u, v);
                            }

                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
