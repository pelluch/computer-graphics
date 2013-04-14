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
        private Vector[,] normals;
        private SceneMaterial[,] materials;
        public RenderingParameters renderingParameters;
        private Vector[,] buffer;
        private int y = 0, x = 0;
        //private bool updateRows = false;
        private Stopwatch watch;
        private int imageIndex = 0;
        private int current_x = 0, current_y = 0;
        public void ShowMouse(int x, int y)
        {
            current_x = x;
            current_y = y;
            RenderingParameters.showMouse = true;
        }

        public RaytraceRenderer(Scene scene, RenderingParameters rendParams)
        {
            this.scene = scene;
            this.renderingParameters = rendParams;
            this.width = rendParams.Width;
            this.height = rendParams.Height;
            this.buffer = new Vector[width, height];
            this.normals = new Vector[width, height];
            this.materials = new SceneMaterial[width, height];

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

            if (!renderingParameters.EnableParallelization)
            {
                if (isRaytracing)
                {
                    buffer[x, y] = CalculatePixel(x, y);
                    x++;
                    if (x >= width)
                    {
                        x = 0;
                        y++;
                        //if (updateRows)
                           // Glut.glutPostRedisplay();
                    }

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
                  //  if (updateRows)
                      //  Glut.glutPostRedisplay();

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

        public Vector CalculatePixel(int screenX, int screenY)
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



            if (renderingParameters.EnableAntialias)
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
                        float currentTime = (float)GenerateRandom() * renderingParameters.MaxTime;
                        ray.Time = currentTime;
                        averageColor = averageColor + CalculateColor(ray, near, far, 0, 0, 1.0f);
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

                float currentTime = (float)GenerateRandom() * renderingParameters.MaxTime;
                Ray ray = new Ray(rayStart, rayDirection, w, near, far);
                ray.Time = currentTime;
                Vector finalColor = CalculateColor(ray, near, far, 0, 0, 1.0f);
                return finalColor;
            }

            averageColor = averageColor / totalSamples;
            return averageColor;
        }

        private Ray CastRefractionRay(Ray ray, HitRecord record, float refractive)
        {
            Vector rayDirection = ray.Direction;
            Vector surfaceNormal = record.SurfaceNormal;
            Vector refractedColor = new Vector();

            float newRefractive = record.Material.RefractionIndex.x;
            if (newRefractive == refractive)
            {
                newRefractive = 1.0f;
            }
            //rayDirection = -surfaceNormal;
            float dDotn = Vector.Dot3(ray.Direction, surfaceNormal);
            float sqrtArgs = 1.0f - (refractive * refractive * (1 - dDotn * dDotn)) / (newRefractive * newRefractive);
            Vector refractiveDir = new Vector();
            Ray refractionRay;

            if (sqrtArgs > 0.0f)
            {
                Vector nSqrt = surfaceNormal * (float)Math.Sqrt(sqrtArgs);
                refractiveDir = (refractive * (rayDirection - surfaceNormal * dDotn)) / newRefractive - nSqrt;
                refractiveDir.Normalize3();
                //refractiveDir = rayDirection;

                refractionRay = new Ray(record.HitPoint + refractiveDir * 0.1f, refractiveDir);
                refractionRay.Time = ray.Time;
                if (RenderingParameters.showMouse)
                {
                    //Console.WriteLine("Refracting");
                //    Console.WriteLine("Ray direction: ");
                //    Console.WriteLine(rayDirection);
                //    Console.WriteLine("Refraction direction: ");
                //    Console.WriteLine(refractiveDir);
                //    Console.WriteLine();
                }

                return refractionRay;
            }
            else
                return ray;
        }

        private bool MakesShadow(Ray shadowRay, SceneLight light, int reflections, int refractions, float refractive, float time)
        {
            if (refractions > 5)
                return false;

            bool makesShadow = false;
            shadowRay.Time = time;
            float lightDistance = (light.Position - shadowRay.Start).Magnitude3();
            shadowRay.MaximumTravelDistance = lightDistance;
            ////////////////////////////////////////////////////////////////

            HitRecord shadowRecord = new HitRecord();
            foreach (SceneObject shadowObject in scene.Objects)
            {
                makesShadow = shadowObject.IsHit(shadowRay, shadowRecord, float.MinValue, float.MaxValue) || makesShadow;
            }

            if (makesShadow)
            {
                if (shadowRecord.Material.RefractionIndex.x != 0.0f)
                {
                    Ray refractedShadow = CastRefractionRay(shadowRay, shadowRecord, refractive);
                    refractive = shadowRecord.Material.RefractionIndex.x;
                    makesShadow = MakesShadow(refractedShadow, light, reflections, refractions + 1, shadowRecord.Material.RefractionIndex.x, time);

                    if (RenderingParameters.showMouse && makesShadow)
                    {
                        Console.WriteLine("Shadow made after getting out from traslucent material.");
                    }
                    return makesShadow;
                }
                else
                {
                    if (RenderingParameters.showMouse)
                        Console.WriteLine("Made shadow from " + shadowRecord.Material.Name);
                    return true;
                }
            }

            return makesShadow;
        }

        private Vector CalculateColor(Ray ray, float minDistance, float maxDistance, int reflections, int refractions, float refractive)
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
                    hitSomething = true;
                    surfaceNormal = record.SurfaceNormal;

                    if (record.Material.RefractionIndex.x > 0.0f)
                    {
                        continue;
                    }
                    record.ShadedColors.Clear();
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
                        Vector halfVector = -1 * rayDirection + lightDirection;
                        halfVector.Normalize3();

                        //Phong shading calculations
                        float normalHalfSimilarity = Vector.Dot3(surfaceNormal, halfVector);
                        Vector phongLightCoefficient = Vector.ColorMultiplication(light.Color, record.Material.Specular);
                        float shininessComponent = (float)Math.Pow(Math.Max(0, normalHalfSimilarity), record.Material.Shininess);
                        Vector phongColor = phongLightCoefficient * shininessComponent;

                        //Add colors and ambient light
                        //Assume no transparency

                        currentLightColor = Vector.LightAdd(lambertColor, phongColor);
                        record.ShadedColors.Add(currentLightColor);
                    }
                }
            }


            if (hitSomething)
            {
                if (RenderingParameters.showMouse)
                {
                //    Console.WriteLine("Reflections: " + reflections);
                //    Console.WriteLine("Refractions: " + refractions);
                    Console.WriteLine("Hit material " + record.Material.Name);
                //    Console.WriteLine("Normal: " + record.SurfaceNormal);
                }
                finalColor = new Vector();
                //////////////////////////////////////////////////////////////
                ///////////////////////SHADOWS///////////////////////////////
                /////////////////////////////////////////////////////////////
                if (record.Material.RefractionIndex.x == 0.0f) //Not refractive surface
                {
                    for (int i = 0; i < scene.Lights.Count; i++) //For each light
                    {
                        if (renderingParameters.EnableShadows)
                        {
                            Vector direction = scene.Lights[i].Position - record.HitPoint;
                            direction.Normalize3();
                            Vector shadowStart = record.HitPoint + direction * 0.1f;
                            Ray shadowRay = new Ray(shadowStart, direction);
                            bool makesShadow = MakesShadow(shadowRay, scene.Lights[i], reflections, refractions, refractive, ray.Time);
                            if (makesShadow)
                            {
                                record.ShadedColors[i] = new Vector();
                            }
                        }
                        finalColor = Vector.LightAdd(finalColor, record.ShadedColors[i]);
                        finalColor.w = 1.0f;
                    }
                }
                ///////////////////////////////////////////////////////////////
                //////////////////////REFRACTIONS/////////////////////////////
                //////////////////////////////////////////////////////////////     
                if (renderingParameters.EnableRefractions && !record.Material.RefractionIndex.IsBlack())
                {
                    Ray refractedRay = CastRefractionRay(ray, record, refractive);
                    if (RenderingParameters.showMouse)
                        Console.WriteLine("REFRACTING");

                    finalColor = 0.8f * CalculateColor(refractedRay, float.MinValue, float.MaxValue, reflections, refractions + 1, record.Material.RefractionIndex.x);
                }

                ///////////////////////////////////////////////////////////////
                //////////////////////REFLECTIONS/////////////////////////////
                //////////////////////////////////////////////////////////////                
                if (renderingParameters.EnableReflections && reflections < 20 && !record.Material.Reflective.IsBlack() )
                {
                    Vector d = rayDirection;
                    //Check for reflections
                    Vector reflection = d -2* Vector.Dot3(d, surfaceNormal) * surfaceNormal;
                    reflection.Normalize3();
                    HitRecord reflectionRecord = new HitRecord();

                    Ray reflectionRay = new Ray(record.HitPoint + reflection * 0.01f, reflection);
                    reflectionRay.Time = ray.Time;
                    //if (showMouse)
                    //{
                    //    Console.WriteLine("Reflection direction: ");
                    //    Console.WriteLine(reflection);
                    //    Console.WriteLine();
                    //}
                    Vector reflectiveColor = record.Material.Reflective;
                    if (RenderingParameters.showMouse)
                    {
                        Console.WriteLine("REFLECTING - NORMAL: " + surfaceNormal + "\tReflection: " + reflection);
                    }
                    Vector reflectedObjectColor = CalculateColor(reflectionRay, float.MinValue, float.MaxValue, reflections + 1, refractions, refractive);
                    finalColor = Vector.LightAdd(finalColor, Vector.ColorMultiplication(reflectiveColor, reflectedObjectColor));
                    finalColor = Vector.LightAdd(finalColor, scene.Background.AmbientLight);
                }

            }

            if (reflections == 0 && refractions == 0)
            {
                //finalColor = Vector.LightAdd(finalColor, scene.Background.AmbientLight);
                if (RenderingParameters.showMouse)
                {
                    RenderingParameters.showMouse = false;
                    Console.WriteLine("-------------------------------------------------------");
                    Console.WriteLine("-------------------------------------------------------");
                }
            }

            finalColor.w = 1.0f;
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
            //StreamWriter normals = new StreamWriter(new FileStream(fileName + ".normals.txt", FileMode.Create));
            //StreamWriter mats = new StreamWriter(new FileStream(fileName + ".mats.txt", FileMode.Create));

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
