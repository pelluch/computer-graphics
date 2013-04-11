using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SceneLib;
using Tao.OpenGl;
using Tao.FreeGlut;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Renderer
{
    class RaytraceRenderer
    {
        private static Random rand = new Random();
        private Scene scene;
        private int width;
        private int height;
        private bool isRaytracing;
        private Vector[][] map;
        private Vector[,] buffer;
        private int y = 0, x = 0;
        private bool antiAlias = true;

        private bool useParallel = true;
        private bool updateRows = false;
        private Stopwatch watch;
        private int imageIndex = 0;
        private float maxTime = 10.0f;

        public RaytraceRenderer(Scene scene, int width, int height)
        {
            this.scene = scene;
            this.width = width;
            this.height = height;
            this.buffer = new Vector[width, height];
            this.map = new Vector[height][];

            for (int y = 0; y < height; y++)
            {
                map[y] = new Vector[width];
                for (int x = 0; x < height; x++)
                {
                    map[y][x] = new Vector(x, y);
                }
            }
            watch = new Stopwatch();
        }

        public void ResetTracer()
        {
            watch.Reset();
            x = 0;
            y = 0;
            isRaytracing = true;
        }


        public void Update()
        {
            if (!watch.IsRunning)
                watch.Start();

            if (!useParallel)
            {
                if (isRaytracing)
                {
                    buffer[x, y] = CalculatePixel(x, y);
                    x++;
                    if (x >= width)
                    {
                        x = 0;
                        y++;
                        if(updateRows)
                            Glut.glutPostRedisplay();
                    }

                    if (y >= height)
                    {
                        watch.Stop();
                        Console.WriteLine((double)watch.ElapsedMilliseconds/1000.0);
                        isRaytracing = false;
                        Glut.glutPostRedisplay();
                        SaveImage(buffer, "image" + imageIndex + ".png");
                        imageIndex++;
                    }

                }
            }
            else
            {
                if (isRaytracing)
                {
                    Vector[] currentRow = map[y];
                    ParallelOptions opts = new ParallelOptions();
                    Parallel.ForEach(currentRow, pos =>
                    {
                        Vector currentPos = pos;
                        buffer[(int)pos.x, (int)pos.y] = CalculatePixel((int)pos.x, (int)pos.y);

                    }
                    );

                    y++;
                    if (updateRows)
                        Glut.glutPostRedisplay();

                    if (y >= height)
                    {
                        watch.Stop();
                        Console.WriteLine((double)watch.ElapsedMilliseconds / 1000.0);
                        isRaytracing = false;
                        Glut.glutPostRedisplay();
                        SaveImage(buffer, "image" + imageIndex + ".png");
                        imageIndex++;
                    }
                    

                }
               
            }
        }

        public void PrintCameraPositions()
        {
            Vector e = scene.Camera.Position;
            Vector up = scene.Camera.Up;
            Vector t = scene.Camera.Target;
            float fov = scene.Camera.FieldOfView;
            Console.Clear();
            Console.WriteLine("e:");
            Console.WriteLine(e);
            Console.WriteLine("up:");
            Console.WriteLine(up);
            Console.WriteLine("t:");
            Console.WriteLine(t);
            Console.WriteLine("fov: " + fov);
        }

        public void Render()
        {
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Gl.glOrtho(0, width, 0, height, 1, -1);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            Gl.glClearColor(0, 0, 1, 0);

            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
            Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            Gl.glBegin(Gl.GL_POINTS);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (buffer[i, j] == null)
                        Utils.DrawPixel(new Vector(i, j), new Vector(0, 0, 0));
                    else
                        Utils.DrawPixel(new Vector(i, j), buffer[i, j]);

                }

            }

            Gl.glEnd();
            Gl.glFlush();

        }

        private object randLock = new object();
        public double GenerateRandom()
        {
            double next = 0.0;
            lock (randLock)
            {
                next = rand.NextDouble();
            }
            return next;
        }
       
        private Vector CalculatePixel(int screenX, int screenY)
        {

            //Console.WriteLine("Drawing pixel " + screenX + ", " + screenY);
            //Naturally, we would have to trace rays in order to find collisions.
            Vector eye = scene.Camera.Position;
            // Console.WriteLine("Eye: " + eye);
            Vector target = scene.Camera.Target;
            // Console.WriteLine("Target: " + target);
            Vector up = scene.Camera.Up;
            //Console.WriteLine("up: " + up);

            //Use radians
            float fov = (float)Math.PI * (scene.Camera.FieldOfView / 180.0f);
            float near = scene.Camera.NearClip;
            //float far = scene.Camera.FarClip;
            float far = float.MaxValue;

            Vector w = CalculateW(eye, target);
            Vector u = CalculateU(up, w);
            Vector v = CalculateV(w, u);

            float tanAngle = (float)Math.Tan(fov / 2.0);
            float top = tanAngle * near;
            float bottom = -top;
            float aspectRatio = width / height;
            float left = aspectRatio * bottom;
            float right = -left;

            int subPixelLevel = 2;
            int cellsPerRow = 2 << (subPixelLevel - 1);
            double cellWidth = 1.0 / (double)cellsPerRow;
            Vector averageColor = new Vector();
            float totalSamples = cellsPerRow * cellsPerRow;

            float currentTime = (float)GenerateRandom();

            if (antiAlias)
            {
                for (int i = 0; i < cellsPerRow; i++)
                {
                    double startX = screenX + i * cellWidth;
                    for (int j = 0; j < cellsPerRow; j++)
                    {
                        
                        double startY = screenY + j * cellWidth;
                        float sampleX = (float)(startX + GenerateRandom() * cellWidth);
                        float sampleY = (float)(startY + GenerateRandom() * cellWidth);

                        float uCoord = (sampleX + 0.5f) * (right - left) / width + left;
                        float vCoord = (sampleY + 0.5f) * (top - bottom) / height + bottom;
                        float wCoord = -near;
                        //(x,y) -> (u, v, w)
                        //Sij es con respecto a la posición de la cámara
                        Vector Sij = uCoord * u + vCoord * v + wCoord * w;
                        Vector rayStart = eye;
                        Vector rayDirection = Sij;
                        List<SceneLight> lights = scene.Lights;
                        rayDirection.Normalize3();

                        Ray ray = new Ray(rayStart, rayDirection, w, near, far);
                        currentTime = (float)GenerateRandom() * maxTime;
                        ray.Time = currentTime;
                        averageColor = averageColor + CalculateColor(ray, near, far, 0);
                    }
                }
            }
            else
            {
                float uCoord = (screenX + 0.5f) * (right - left) / width + left;
                float vCoord = (screenY + 0.5f) * (top - bottom) / height + bottom;
                float wCoord = -near;
                //(x,y) -> (u, v, w)
                //Sij es con respecto a la posición de la cámara
                Vector Sij = uCoord * u + vCoord * v + wCoord * w;
                Vector rayStart = eye;
                Vector rayDirection = Sij;
                List<SceneLight> lights = scene.Lights;
                rayDirection.Normalize3();

                Ray ray = new Ray(rayStart, rayDirection, w, near, far);
                ray.Time = currentTime;
                Vector finalColor = CalculateColor(ray, near, far, 0);
                return finalColor;
            }

            averageColor = averageColor / totalSamples;
            return averageColor;
        }

        private Vector CalculateColor(Ray ray, float minDistance, float maxDistance, int recurseLevel)
        {
            HitRecord record = new HitRecord();
            Vector finalColor = new Vector();
            Vector lightDirection = new Vector();
            bool hitSomething = false;
            Vector surfaceNormal = new Vector();
            Vector rayDirection = ray.Direction;

            foreach (SceneObject sceneObject in scene.Objects)
            {
                
                //First check intersection
                bool intersects = sceneObject.IsHit(ray, record, minDistance, maxDistance);
                //If it intersects, diffuse color is set, so set shading color

                if (intersects)
                {
                    finalColor = new Vector();
                    hitSomething = true;
                    surfaceNormal = sceneObject.SurfaceNormal(record.HitPoint, rayDirection);

                    foreach (SceneLight light in scene.Lights)
                    {
                        Vector currentLightColor = new Vector();
                        lightDirection = light.Position - record.HitPoint;
                        lightDirection.Normalize3();

                        //Get cosine of angle between vectors
                        float similarity = Vector.Dot3(surfaceNormal, lightDirection);
                    
                        Vector lambertColor = new Vector();
                        if (record.Material.TextureImage != null)
                        {
                            lambertColor = Vector.ColorMultiplication(light.Color, record.TextureColor) * Math.Max(0, similarity);
                        }
                        else
                        {
                            lambertColor = Vector.ColorMultiplication(light.Color, record.Material.Diffuse) * Math.Max(0, similarity);
                        }
                       

                        //Get half vector between camera direction and light direction
                        Vector halfVector = -1*rayDirection + lightDirection;
                        halfVector.Normalize3();

                        //Phong shading calculations
                        float normalHalfSimilarity = Vector.Dot3(surfaceNormal, halfVector);
                        Vector phongLightCoefficient = Vector.ColorMultiplication(light.Color, record.Material.Specular);
                        float shininessComponent = (float)Math.Pow(Math.Max(0, normalHalfSimilarity), record.Material.Shininess);
                        Vector phongColor = phongLightCoefficient * shininessComponent;

                        //Add colors and ambient light
                        //Assume no transparency

                        currentLightColor = Vector.LightAdd(lambertColor, phongColor);

                        //Check for shadows
                       
                        Vector shadowStart = record.HitPoint + lightDirection * 0.1f;
                        Ray shadowRay = new Ray(shadowStart, lightDirection);
                        shadowRay.Time = ray.Time;

                        float lightDistance = (light.Position - shadowStart).Magnitude3();
                        shadowRay.MaximumTravelDistance = lightDistance;

                        HitRecord shadowRecord = new HitRecord();
                        foreach (SceneObject shadowObject in scene.Objects)
                        {
                            bool makesShadow = shadowObject.IsHit(shadowRay, shadowRecord, float.MinValue, float.MaxValue);
                            if (makesShadow)
                            {
                                currentLightColor = new Vector();
                                break;
                            }
                        }

                        finalColor = Vector.LightAdd(currentLightColor, finalColor);
                        finalColor.w = 1.0f;
                    }
                }
            }

            if (hitSomething)
            {

                Vector d = record.HitPoint - scene.Camera.Position;
                d.Normalize3();

                //Check for reflections
                Vector reflection = d - 2 * Vector.Dot3(d, surfaceNormal) * surfaceNormal;
                reflection.Normalize3();
                HitRecord reflectionRecord = new HitRecord();

                Ray reflectionRay = new Ray(record.HitPoint + reflection * 0.01f, reflection);
                reflectionRay.Time = ray.Time;

                if (recurseLevel < 20 && !record.Material.Reflective.IsBlack())
                {
                    Vector reflectiveColor = record.Material.Reflective;
                    Vector reflectedObjectColor = CalculateColor(reflectionRay, float.MinValue, float.MaxValue, recurseLevel + 1);
                    return Vector.LightAdd(finalColor, Vector.ColorMultiplication(reflectiveColor, reflectedObjectColor));
                }
                 
            }
            finalColor = Vector.LightAdd(finalColor, scene.Background.AmbientLight);
            return finalColor;
        }

        private Vector CalculateU(Vector up, Vector w)
        {
            Vector u = Vector.Cross3(up, w);
            u.Normalize3();
            return u;
        }

        private Vector CalculateV(Vector w, Vector u)
        {
            Vector v = Vector.Cross3(w, u);
            return v;
        }

        private Vector CalculateW(Vector eye, Vector target)
        {
            // -(t - e)
            Vector gazeDirection = eye - target;
            gazeDirection.Normalize3();
            return gazeDirection;
        }

        private void SaveImage(Vector[,] buffer, string fileName)
        {
            Bitmap image = new Bitmap(buffer.GetLength(0), buffer.GetLength(1));

            for (int i = 0; i < buffer.GetLength(0); i++)
            {
                for (int j = 0; j < buffer.GetLength(1); j++)
                {
                    int r = (int)(255 * buffer[i, j].x);
                    int g = (int)(255 * buffer[i, j].y);
                    int b = (int)(255 * buffer[i, j].z);
                    image.SetPixel(i, buffer.GetLength(1) - 1 - j, Color.FromArgb(r, g, b));
                }
            }
            image.Save(fileName);
            return;
        }

    }
}
