using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SceneLib;
using Tao.OpenGl;
using Tao.FreeGlut;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Renderer
{
    public class TransformationRenderer : Renderer
    {
        private Scene scene;
        private int width;
        private int height;
        private bool isDrawing = false;
        private Vector[,] buffer;
        private float[,] zBuffer;
        private Stopwatch watch;
        private Queue<Vector> pointsToDraw = new Queue<Vector>();
        private Queue<Vector> colorsToDraw = new Queue<Vector>();

        private int imageIndex = 0;
        public RenderingParameters renderingParameters;

        public TransformationRenderer(Scene scene, RenderingParameters rendParams)
        {
            this.scene = scene;
            this.renderingParameters = rendParams;
            this.width = rendParams.Width;
            this.height = rendParams.Height;
            this.buffer = new Vector[width, height];
            this.zBuffer = new float[width, height];
            for (int i = 0; i < zBuffer.GetLength(0); ++i)
            {
                for (int j = 0; j < zBuffer.GetLength(1); ++j)
                {
                    zBuffer[i, j] = float.MinValue;
                    buffer[i, j] = rendParams.BackgroundColor;
                }
            }
            watch = new Stopwatch();
            this.isDrawing = true;
        }

        public void ResetRenderer()
        {
            if (renderingParameters.Interactive && !renderingParameters.FinishedInteracting) return;

            this.buffer = new Vector[width, height];
            this.zBuffer = new float[width, height];
            for (int i = 0; i < zBuffer.GetLength(0); ++i)
            {
                for (int j = 0; j < zBuffer.GetLength(1); ++j)
                {
                    buffer[i, j] = renderingParameters.BackgroundColor;
                    zBuffer[i, j] = float.MinValue;
                }
            }
            isDrawing = true;
            watch.Reset();
        }
        public void Update()
        {
            InnerRender();

        }

        public void StepOnce()
        {
            Vector coords = pointsToDraw.Dequeue();
            Vector color = colorsToDraw.Dequeue();
            buffer[(int)coords.x, (int)coords.y] = color;
            Console.WriteLine("Coords: " + coords);
            Console.WriteLine("Color: " + color);
            if (pointsToDraw.Count == 0)
            {
                renderingParameters.FinishedInteracting = true;
                renderingParameters.Interactive = false;
            }
            if((int)coords.x == 289 && (int)coords.y == 187)
            {
                Console.WriteLine(buffer[(int)coords.x, (int)coords.y]);
            }
        }

        public void Render()
        {
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Gl.glOrtho(0, width, 0, height, 1, -1);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            Gl.glClearColor(0, 0, 0.5f, 0);

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
                        Utils.DrawPixel(new Vector(i, j), new Vector(1.0f, 1.0f, 1.0f));
                    else
                        Utils.DrawPixel(new Vector(i, j), buffer[i, j]);

                }
                
            }

            Gl.glEnd();
            Gl.glFlush();

        }

        private Matrix ViewPortMatrix()
        {
            float nx = renderingParameters.Width;
            float ny = renderingParameters.Height;

            float[] viewData = new float[]{ nx/2.0f, 0, 0, (nx-1)/2.0f,
                0, ny/2.0f, 0, (ny-1)/2.0f,
                0, 0, 1, 0,
                0, 0, 0, 1};

            Matrix viewMatrix = new Matrix(viewData);
            return viewMatrix;
        }

        private Matrix PerspectiveMatrix()
        {
            float far = -scene.Camera.FarClip;
            //float far = float.MaxValue;

            float near = -scene.Camera.NearClip;
            float[] perspectiveData = new float[] { near, 0, 0, 0,
                0, near, 0, 0,
                0, 0, near+far, -far*near,
                0, 0, 1, 0};
            Matrix perspectiveMatrix = new Matrix(perspectiveData);
            return perspectiveMatrix;

        }

        public void TestRaster()
        {
            if (!renderingParameters.Interactive || renderingParameters.FinishedInteracting)
            {
                if (renderingParameters.Interactive) renderingParameters.FinishedInteracting = false;

                for (int i = 0; i < 16; i++)
                {
                    Vector center = new Vector(200, 150);
                    Vector pos = center + new Vector((float)(150 * Math.Cos(Math.PI / 16.0 + Math.PI * 2 * i / 16)), (float)(150 * Math.Sin(Math.PI / 16.0 + Math.PI * 2 * i / 16)));
                    //Console.WriteLine(pos);
                    DrawLine(new Vector(200, 150), pos, new Vector(1, 0, 0), new Vector(0, 1, 0), renderingParameters.EnableAntialias);

                    Vector pos2 = center + new Vector((float)(150 * Math.Cos(Math.PI * 2 * i / 16)), (float)(150 * Math.Sin(Math.PI * 2 * i / 16)));
                    //Console.WriteLine(pos);
                    DrawLine(new Vector(200, 150), pos2, new Vector(1, 0, 0), new Vector(0, 0, 1), renderingParameters.EnableAntialias);
                    //Glut.glutPostRedisplay();
                }

                List<RasterizedVertex> rasterized = new List<RasterizedVertex>();

                RasterizedVertex v1 = new RasterizedVertex();
                v1.BlinnPhongColor = new Vector(1, 0, 0);
                v1.Position = new Vector(80, 80);
                rasterized.Add(v1);

                RasterizedVertex v2 = new RasterizedVertex();
                v2.BlinnPhongColor = new Vector(0, 1, 0);
                v2.Position = new Vector(250, 270);
                rasterized.Add(v2);

                RasterizedVertex v3 = new RasterizedVertex();
                v3.BlinnPhongColor = new Vector(0, 0, 1);
                v3.Position = new Vector(260, 40);
                rasterized.Add(v3);


                DrawTriangle(rasterized);
            }
        }

        /// <summary>
        /// Método de prueba
        /// </summary>
        private void InnerRender()
        {
            if (!watch.IsRunning)
                watch.Start();

            Vector w = CalculateW(scene.Camera.Position, scene.Camera.Target);
            Vector q = scene.Camera.Position - scene.Camera.NearClip * w;
            //Para efectos del clipping plane
            float D = -Vector.Dot3(w, scene.Camera.Position) + scene.Camera.NearClip * Vector.Dot3(w, w);

            //if (isDrawing)
            //{
            //    TestRaster();
            //    isDrawing = false;
            //    Glut.glutPostRedisplay();
            //    return;

            //}
            if (isDrawing)
            {
                Matrix cameraMatrix = CameraMatrix();
                Matrix orthogonalProjection = OrthogonalMatrix();
                Matrix perspectiveMatrix = PerspectiveMatrix();
                Matrix viewMatrix = ViewPortMatrix();

                Console.WriteLine(orthogonalProjection);
                List<List<RasterizedVertex>> allRasterized = new List<List<RasterizedVertex>>();
                foreach (SceneObject obj in scene.Objects)
                {
                    
                    SceneTriangle triangle = obj as SceneTriangle;
                    List<Vector> points = new List<Vector>();
                    List<Vector> colors = new List<Vector>();
                    List<RasterizedVertex> rasterizedVertex = new List<RasterizedVertex>();


                    List<Vector> outside = new List<Vector>();
                    List<Vector> inside = new List<Vector>();

                    for (int i = 0; i < 3; ++i)
                    {
                        float planeDistance = Vector.Dot3(w, triangle.Vertex[i]) + D;
                        if (planeDistance > 0)
                        {
                            outside.Add(triangle.Vertex[i]);                            
                        }
                        else
                        {
                            inside.Add(triangle.Vertex[i]);
                        }
                    }
                    if (outside.Count == 3)
                        continue;
                    else if (outside.Count == 2)
                    {
                        float t1 = (Vector.Dot3(w, inside[0]) + D) / (Vector.Dot3(w, inside[0] - outside[0]));
                        Vector inter1 = inside[0] + t1 * (outside[0] - inside[0]);
                        float t2 = (Vector.Dot3(w, inside[0]) + D) / (Vector.Dot3(w, inside[0] - outside[1]));
                        Vector inter2 = inside[0] + t2 * (outside[1] - inside[0]); 
                    }
                    else if (outside.Count == 1)
                    {
                        float t1 = (Vector.Dot3(w, inside[0]) + D) / (Vector.Dot3(w, inside[0] - outside[0]));
                        Vector inter1 = inside[0] + t1 * (outside[0] - inside[0]);
                        float t2 = (Vector.Dot3(w, inside[1]) + D) / (Vector.Dot3(w, inside[1] - outside[0]));
                        Vector inter2 = inside[1] + t2 * (outside[0] - inside[1]);

                        SceneTriangle triangle1 = new SceneTriangle();
                        SceneTriangle triangle2 = new SceneTriangle();
                        Vector bar1 = GetBaricentricCoords(triangle.Vertex, inter1, true);
                        Vector bar2 = GetBaricentricCoords(triangle.Vertex, inter2, true);
                    }

                    for(int i = 0; i < 3; ++i)
                    {
                        Vector vertex = triangle.Vertex[i];
                        //colors[i] = triangle.Materials[i].Diffuse;

                        Vector cameraSpace = cameraMatrix * vertex;
                        Vector perspectiveSpace = perspectiveMatrix * cameraSpace;
                        Vector perspectiveSpaceDivided = perspectiveSpace / perspectiveSpace.w;
                        Vector portSpace = orthogonalProjection * perspectiveSpace;
                        //Vector portSpaceDivided = portSpace / portSpace.w;
                        Vector screenSpace = viewMatrix * portSpace;
                        //screenSpace = screenSpace / screenSpace.w;
                     

                        rasterizedVertex.Add(new RasterizedVertex());
                        rasterizedVertex[i].U = triangle.U[i];
                        rasterizedVertex[i].V = triangle.V[i];
                        rasterizedVertex[i].Position = screenSpace;
                        rasterizedVertex[i].Material = triangle.Materials[i];
                        rasterizedVertex[i].Normal = triangle.Normal[i];

                        if (triangle.Materials[i] != null && triangle.Materials[i].TextureFile != null && triangle.Materials[i].TextureFile != "")
                            rasterizedVertex[i].HasTexture = true;

                        Vector surfaceNormal = triangle.Normal[i];
                        Vector lightDirection;
                        Vector vertexColor = new Vector();

                        Vector diffuse = new Vector();
                        if (rasterizedVertex[i].HasTexture)
                        {
                            diffuse = new Vector(1, 1, 1, 1);
                        }
                        else
                        {
                            diffuse = triangle.Materials[i].Diffuse;
                        }

                        foreach (SceneLight light in scene.Lights)
                        {
                            Vector currentLightColor = new Vector();
                            lightDirection = light.Position - vertex;
                            lightDirection.Normalize3();

                            //Get cosine of angle between vectors
                            float similarity = Vector.Dot3(surfaceNormal, lightDirection);
                            Vector lambertColor = new Vector();
                            lambertColor = Vector.ColorMultiplication(light.Color, diffuse) * Math.Max(0, similarity);


                            //Get half vector between camera direction and light direction
                            Vector eyeDirection = scene.Camera.Position - vertex;
                            eyeDirection.Normalize3();
                            Vector halfVector = eyeDirection + lightDirection;
                            halfVector.Normalize3();

                            //Phong shading calculations
                            float normalHalfSimilarity = Vector.Dot3(surfaceNormal, halfVector);
                            Vector phongLightCoefficient = Vector.ColorMultiplication(light.Color, triangle.Materials[i].Specular);
                            float shininessComponent = (float)Math.Pow(Math.Max(0, normalHalfSimilarity), triangle.Materials[i].Shininess);
                            Vector phongColor = phongLightCoefficient * shininessComponent;

                            //Add colors and ambient light
                            //Assume no transparency

                            currentLightColor = Vector.LightAdd(lambertColor, phongColor);
                            vertexColor = Vector.LightAdd(vertexColor, currentLightColor);

                            //currentLightColor = Vector.LightAdd(lambertColor, phongColor);
                            //record.ShadedColors.Add(currentLightColor);
                        }
                        
                        colors.Add(vertexColor);
                        rasterizedVertex[i].BlinnPhongColor = vertexColor;
                    }


                    //for (int i = 0; i < 3; ++i)
                    //{
                    //    float planeDistance = rasterizedVertex[i].Position.z + scene.Camera.NearClip * rasterizedVertex[i].Position.w;
                    //    if (planeDistance < 0)
                    //    {
                    //        Console.WriteLine("On other side!");
                    //    }
                
                    //}
                    DrawTriangle(rasterizedVertex);
                    allRasterized.Add(rasterizedVertex);
                }
                foreach (List<RasterizedVertex> triangle in allRasterized)
                {
                    DrawTriangleWire(triangle);
                }
                //for (int i = 0; i < 16; i++)
                //{
                //    Vector center = new Vector(200, 150);
                //    Vector pos = center + new Vector((float)(150 * Math.Cos(Math.PI / 16.0 + Math.PI * 2 * i / 16)), (float)(150 * Math.Sin(Math.PI / 16.0 + Math.PI * 2 * i / 16)));
                //    DrawLine(new Vector(200, 150), pos, new Vector(1, 0, 0), new Vector(0, 1, 0), true);

                //    Vector pos2 = center + new Vector((float)(150 * Math.Cos(Math.PI * 2 * i / 16)), (float)(150 * Math.Sin(Math.PI * 2 * i / 16)));
                //    DrawLine(new Vector(200, 150), pos2, new Vector(1, 0, 0), new Vector(0, 0, 1), true);
                //}

                //DrawTriangle(new Vector(80, 80), new Vector(250, 270), new Vector(260, 40), new Vector(1, 0, 0), new Vector(0, 1, 0),
                //    new Vector(0, 0, 1), true);

                Console.WriteLine();
                Glut.glutPostRedisplay();
                isDrawing = false;
                watch.Stop();
                Console.WriteLine((double)watch.ElapsedMilliseconds / 1000.0);
                //DrawLine(new Vector(50, 50), new Vector(100, 100), new Vector(0, 0, 1), new Vector(1, 0, 0), true);
            }
        }


        private void SaveImage(Vector[,] buffer, string fileName)
        {
            Bitmap image = new Bitmap(buffer.GetLength(0), buffer.GetLength(1));

            for (int i = 0; i < buffer.GetLength(0); i++)
            {
                for (int j = 0; j < buffer.GetLength(1); j++)
                { 
                    int r = (int)( 255*buffer[i,j].x);
                    int g = (int)( 255*buffer[i,j].y);
                    int b = (int)( 255*buffer[i,j].z);
                    image.SetPixel(i, buffer.GetLength(1) - 1 - j, Color.FromArgb(r, g, b));
                }
            }
            image.Save(fileName);
            return;
        }

        private Matrix OrthogonalMatrix()
        {
            float far = -scene.Camera.FarClip;
            //float far = float.MaxValue;

            float near = -scene.Camera.NearClip;

            //Use radians
            float fov = (float)Math.PI * (scene.Camera.FieldOfView / 180.0f);
            float tanAngle = (float)Math.Tan(fov / 2.0);
            float top = tanAngle * Math.Abs(near);
            float bottom = -top;
            float aspectRatio = width / height;
            float left = aspectRatio * bottom;
            float right = -left;

            float[] matrixData = new float[]{2.0f/(right-left), 0, 0, -(right+left)/(right-left),
                0, 2.0f/(top-bottom), 0, -(top+bottom)/(top-bottom),
                0, 0, 2.0f/(near-far), -(near+far)/(near-far),
                0, 0, 0, 1};
            Matrix orthogonalMatrix = new Matrix(matrixData);
            return orthogonalMatrix;
        }
        private Matrix CameraMatrix()
        {
            //Console.WriteLine("Drawing pixel " + screenX + ", " + screenY);
            //Naturally, we would have to trace rays in order to find collisions.
            Vector eye = scene.Camera.Position;
            // Console.WriteLine("Eye: " + eye);
            Vector target = scene.Camera.Target;
            // Console.WriteLine("Target: " + target);
            Vector up = scene.Camera.Up;
            //Console.WriteLine("up: " + up);

            float near = scene.Camera.NearClip;

            Vector w = CalculateW(eye, target);
            Vector u = CalculateU(up, w);
            Vector v = CalculateV(w, u);

            float[] coeffs = new float[] { u.x, u.y, u.z, 0.0f, v.x, v.y, v.z, 0.0f, 
            w.x, w.y, w.z, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f};
            Matrix m1 = new Matrix(coeffs);
            float[] coeffs2 = new float[] { 1, 0, 0, -eye.x, 0, 1, 0, -eye.y, 
                0, 0, 1, -eye.z, 0, 0, 0, 1};
            Matrix m2 = new Matrix(coeffs2);
            Matrix cameraMatrix = m1 * m2;

            return cameraMatrix;
        }

        /// <summary>
        /// Dibuja Linea entre los puntos p1 y p2. En caso de lineas sin antialias, estas
        /// se deben pintar en negro (0,0,0) En caso de lineas con antialias
        /// se deben pintar la línea con variación de colores entre c1 y c2.
        /// </summary>
        /// <param name="p1">Punto 1</param>
        /// <param name="p2">Punto 2</param>
        /// <param name="c1">Color 1 (solo ocupado en modo antialias)</param>
        /// <param name="c2">Color 2 (solo ocupado en modo antialias)</param>
        /// <param name="antialias">Define con o sin antialias.</param>
        private void DrawLine(Vector p1, Vector p2, Vector c1, Vector c2, bool antialias)
        {
            //Ocupamos el algoritmo de Bresenham
            if (!antialias)
            {
                DrawLineBresenham(p1, p2, c1, c2);
            }
            else
            {
                DrawLineXiaolin(p1, p2, c1, c2);
            }
        }

        /// <summary>
        /// Dibuja Linea entre los puntos p1 y p2. En caso de lineas sin antialias, estas
        /// se deben pintar en negro (0,0,0) En caso de lineas con antialias
        /// se deben pintar la línea con variación de colores entre c1 y c2.
        /// </summary>
        /// <param name="p1">Punto 1</param>
        /// <param name="p2">Punto 2</param>
        /// <param name="c1">Color 1 (solo ocupado en modo antialias)</param>
        /// <param name="c2">Color 2 (solo ocupado en modo antialias)</param>
        private void DrawLineBresenham(Vector p1, Vector p2, Vector c1, Vector c2)
        {

            Vector c = c1;
            int x1 = (int)p1.x, x2 = (int)p2.x, y1 = (int)p1.y, y2 = (int)p2.y; //Obtenemos puntos iniciales
            //Punto de inicio = punto final
            if (x1 == x2 && y1 == y2)
            {
                this.buffer[x1, y1] = c1;
                return;
            }

            Vector colorM;
            int deltax = Math.Abs(x2 - x1), deltay = Math.Abs(y2 - y1); //Obtenemos diferencias entre puntos
            int x = x1, y = y1; //x e y son los puntos actuales que dibujamos.

            int xinc1, xinc2, yinc1, yinc2, den, num, numadd, numpixels;

            
            if (x2 >= x1) //Left to right
            {
                xinc1 = 1;
                xinc2 = 1;
            }
            else //Right to left
            {
                xinc1 = -1;
                xinc2 = -1;
            }

            if (y2 >= y1) //Top to bottom
            {
                yinc1 = 1;
                yinc2 = 1;
            }
            else //Bottom to top
            {
                yinc1 = -1;
                yinc2 = -1;
            }

            if (deltax >= deltay)  //m < 1 -> increase x
            {
                xinc1 = 0;
                yinc2 = 0;
                den = deltax;
                num = deltax >> 1;
                numadd = deltay;
                numpixels = deltax;
                colorM = (c2 - c1) / (x2 - x1);
            }
            else //m > 1 -> increase y
            {
                xinc2 = 0;
                yinc1 = 0;
                den = deltay;
                num = deltay >> 1;
                numadd = deltax;
                numpixels = deltay;
                colorM = (c2 - c1) / (y2 - y1);
            }

            for (int pixels = 0; pixels <= numpixels; pixels++)
            {
                
                if (renderingParameters.Interactive)
                {
                    pointsToDraw.Enqueue(new Vector(x, y));
                    colorsToDraw.Enqueue(c);
                }
                else this.buffer[x, y] = c;

                //this.buffer[x + 1,y] = new Vector(0, 0, 1);
                num += numadd;
                if (num >= den)
                {
                    num -= den;
                    x += xinc1;
                    y += yinc1;
                }
                x += xinc2;
                y += yinc2;
                c = c + colorM;
            }
        }

        /// <summary>
        /// Dibuja Linea entre los puntos p1 y p2. En caso de lineas sin antialias, estas
        /// se deben pintar en negro (0,0,0) En caso de lineas con antialias
        /// se deben pintar la línea con variación de colores entre c1 y c2.
        /// </summary>
        /// <param name="p1">Punto 1</param>
        /// <param name="p2">Punto 2</param>
        /// <param name="c1">Color 1 (solo ocupado en modo antialias)</param>
        /// <param name="c2">Color 2 (solo ocupado en modo antialias)</param>
        private void DrawLineXiaolin(Vector p1, Vector p2, Vector c1, Vector c2)
        {
            //Algoritmo de Xaolin
            float y1 = p1.y / p1.w, y2 = p2.y / p2.w, x1 = p1.x / p1.w, x2 = p2.x / p2.w;
            float z1 = p1.z / p1.w, z2 = p2.z / p2.w;

            //Punto de inicio = punto final
            if (x1 == x2 && y1 == y2)
            {
                this.buffer[(int)x1, (int)y1] = c1;
                return;
            }

            float deltax = Math.Abs(x2 - x1), deltay = Math.Abs(y2 - y1);
            Vector colorM;

            //Indica cuál de estos tenemos que actualizar con la pendiente.
            //1 = aumenta con la pendiente, 0 en otro caso
            int xinc1 = 1, yinc1 = 1;

            //Indica cuál de las variables aumentamos de uno a uno en el for (variable independiente).
            //1 = aumentamos de uno en uno
            //-1 = disminuimos de uno en uno
            //0 = no cambia (variable dependiente
            int xinc2, yinc2;

            //Cantidad de pixeles que recorremos en el for, calculado como Max(deltax, deltay).
            int numpixels;

            //Pendiente de la recta.
            float m;
            float zm;
            //Si la recta va de izquierda a derecha...
            if (x2 >= x1)
            {
                xinc2 = 1;
            }
            else
            {
                xinc2 = -1;
            }

            //Si la recta va de abajo hacia arriba...
            if (y2 >= y1)
            {
                yinc2 = 1;
            }
            else
            {
                yinc2 = -1;
            }


            //La pendiente es menor que 1, aumentaremos x de uno en uno.
            if (deltax >= deltay)
            {
                xinc1 = 0;
                yinc2 = 0;
                numpixels = (int)deltax;
                m = (y2 - y1) / (x2 - x1);
                colorM = (c2 - c1) / (x2 - x1);
                zm = (z2 - z1) / (x2 - x1);
            }
            //La pendiente es mayor que uno. Si la regla es vertical, ingresa aquí y no se producen errores.
            else
            {
                xinc2 = 0;
                yinc1 = 0;
                numpixels = (int)deltay;
                m = (x2 - x1) / (y2 - y1);
                colorM = (c2 - c1) / (y2 - y1);
                zm = (z2 - z1) / (y2 - y1);

            }
            int decreasing;
            if (xinc2 == -1 || yinc2 == -1) decreasing = -1;
            else decreasing = 1;

            float y = y1, x = x1, z = z1;
            Vector c = new Vector(c1.x, c1.y, c1.z, 1);

            //Al contrario del algoritmo tradicional de Xaolin, no es necesario hacer un caso específico
            //para el primer y último punto de la recta pues estos serán enteros.
            for (int pixels = 0; pixels <= numpixels; pixels++)
            {
                float fractional, invFractional;
                if (deltax >= deltay) invFractional = y - (int)y;
                else invFractional = x - (int)x;

                fractional = 1 - invFractional;

                //if(this.renderingParameters.WireFrame || this.buffer[(int)x, (int)y].IsBlack())
                Vector fractionalColor = c * fractional + this.buffer[(int)x, (int)y] * invFractional;
                fractionalColor.w = 1.0f;
                this.buffer[(int)x, (int)y] = fractionalColor;

                float minZ = this.zBuffer[(int)(x + xinc1), (int)(y + yinc1)];
                if (z > minZ)
                {
                    this.zBuffer[(int)(x + xinc1), (int)(y + yinc1)] = z;
                    Vector invFractionalColor = c*invFractional + this.buffer[(int)(x + xinc1), (int)(y + yinc1)] * fractional;
                    invFractionalColor.w = 1.0f;

                    ////if (this.renderingParameters.WireFrame || this.buffer[(int)(x + xinc1), (int)(y + yinc1)].IsBlack())
                    this.buffer[(int)(x + xinc1), (int)(y + yinc1)] = invFractionalColor;
                }

                c = c + colorM * decreasing;


                x = x + m * xinc1 * yinc2;
                y = y + m * yinc1 * xinc2;

                y = y + yinc2;
                x = x + xinc2;
                z = z + zm;
            }
        }


        /// <summary>
        /// Método que dibuja un wireframe de un triángulo 2D
        /// </summary>
        /// <param name="p1">Vertice 1</param>
        /// <param name="p2">Vertice 2</param>
        /// <param name="p3">Vertice 3</param>
        /// <param name="c1">Color asociado al vertice 1</param>
        /// <param name="c2">Color asociado al vertice 2</param>
        /// <param name="c3">Color asociado al vertice 3</param>
        /// <param name="antialias">Define con o sin antialias.</param>
        private void DrawTriangleWire(List<RasterizedVertex> vertices)
        {
            float h0 = vertices[0].Position.w;
            float h1 = vertices[1].Position.w;
            float h2 = vertices[2].Position.w;

            DrawLine(vertices[0].Position/h0, vertices[1].Position/h1, vertices[0].BlinnPhongColor, vertices[0].BlinnPhongColor, renderingParameters.EnableAntialias);
            DrawLine(vertices[0].Position/h0, vertices[2].Position/h2, vertices[0].BlinnPhongColor, vertices[2].BlinnPhongColor, renderingParameters.EnableAntialias);
            DrawLine(vertices[1].Position/h1, vertices[2].Position/h2, vertices[1].BlinnPhongColor, vertices[2].BlinnPhongColor, renderingParameters.EnableAntialias);
        }

        private Vector GetBaricentricCoords(List<Vector> vertices, Vector trianglePoint, bool homogeneize)
        {
            Vector h = new Vector(1, 1, 1);
            if (homogeneize)
                h = new Vector(vertices[0].w, vertices[1].w, vertices[2].w);

            float x1 = vertices[0].x / h.x, x2 = vertices[1].x / h.y, x3 = vertices[2].x / h.z;
            float y1 = vertices[0].y / h.x, y2 = vertices[1].y / h.y, y3 = vertices[2].y / h.z;
            float x = trianglePoint.x, y = trianglePoint.y;

            float y1y3 = y1 - y3, y1y2 = y1 - y2, y2y3 = y2 - y3;
            float x1x3 = x1 - x3, x1x2 = x1 - x2, x2x3 = x2 - x3;

            float den1 = (y2y3 * x1x3 - x2x3 * y1y3);
            float den2 = (-y1y3 * x2x3 + x1x3 * y2y3);

            float alpha = (y2y3 * (x - x3) - x2x3 * (y - y3)) / den1;
            if (alpha < 0 || alpha > 1) return null;

            float betta = (-y1y3 * (x - x3) + x1x3 * (y - y3)) / den2;
            if (betta < 0 || betta > 1) return null;

            float gamma = 1 - alpha - betta;
            if (gamma < 0 || gamma > 1) return null;

            Vector bar = new Vector(alpha, betta, gamma);
            return bar;
        }

        /// <summary>
        /// Método que dibuja un triángulo 2D, pintando su superficie interpolando los colores de sus vértices.
        /// </summary>
        /// <param name="p1">Vertice 1</param>
        /// <param name="p2">Vertice 2</param>
        /// <param name="p3">Vertice 3</param>
        /// <param name="c1">Color asociado al vertice 1</param>
        /// <param name="c2">Color asociado al vertice 2</param>
        /// <param name="c3">Color asociado al vertice 3</param>
        /// <param name="antialias">Define con o sin antialias.</param>
        private void DrawTriangle(List<RasterizedVertex> rasterized)
        {
            //bool antiAlias = renderingParameters.EnableAntialias;
            //if (renderingParameters.WireFrame)
            //{
            //    DrawTriangleWire(rasterized);
            //    return;
            //}
            //if (renderingParameters.EnableAntialias)
            //{
            //    for (int i = 0; i < 3; i++)
            //    {
            //        DrawLineXiaolin(rasterized[i].Position, rasterized[(i + 1) % 3].Position, rasterized[i].BlinnPhongColor, rasterized[(i + 1) % 3].BlinnPhongColor);
            //    }
            //}
   

            float h0 = rasterized[0].Position.w;
            float h1 = rasterized[1].Position.w;
            float h2 = rasterized[2].Position.w;

            float x1 = rasterized[0].Position.x / rasterized[0].Position.w, x2 = rasterized[1].Position.x / rasterized[1].Position.w, x3 = rasterized[2].Position.x / rasterized[2].Position.w;
            float y1 = rasterized[0].Position.y / rasterized[0].Position.w, y2 = rasterized[1].Position.y / rasterized[1].Position.w, y3 = rasterized[2].Position.y / rasterized[2].Position.w;

            float y1y3 = y1 - y3, y1y2 = y1 - y2, y2y3 = y2 - y3;
            float x1x3 = x1 - x3, x1x2 = x1 - x2, x2x3 = x2 - x3;

            float leftmost = x1;
            if (x2 < leftmost) leftmost = x1;
            if (x3 < leftmost) leftmost = x3;

            float rightmost = x1;
            if (x2 > rightmost) rightmost = x2;
            if (x3 > rightmost) rightmost = x3;

            float topmost = y1;
            if (y2 > topmost) topmost = y2;
            if (y3 > topmost) topmost = y3;

            float bottommost = y1;
            if (y2 < bottommost) bottommost = y2;
            if (y3 < bottommost) bottommost = y3;

            float den1 = (y2y3 * x1x3 - x2x3 * y1y3);
            float den2 = (-y1y3 * x2x3 + x1x3 * y2y3);
            
        
                for (float x = leftmost; x <= rightmost && x < renderingParameters.Width; x++)
                {
                    if (x < 0)
                        continue;

                    for (float y = bottommost; y <= renderingParameters.Height; y++)
                    {
                        if (y < 0)
                            continue;


                        float alpha = (y2y3 * (x - x3) - x2x3 * (y - y3)) / den1;
                        if (alpha < 0 || alpha > 1) continue;

                        float betta = (-y1y3 * (x - x3) + x1x3 * (y - y3)) / den2;
                        if (betta < 0 || betta > 1) continue;

                        float gamma = 1 - alpha - betta;
                        if (gamma < 0 || gamma > 1) continue;

                        float z = alpha * rasterized[0].Position.z / rasterized[0].Position.w + betta * rasterized[1].Position.z / rasterized[1].Position.w + gamma * rasterized[2].Position.z / rasterized[2].Position.w;
                        float minZ = this.zBuffer[(int)x, (int)y];
                        //z es negativo, por eso el >
                        if (z > minZ)
                        {
                            if (rasterized[0].HasTexture)
                            {                            
                                minZ = z;
                                this.zBuffer[(int)x, (int)y] = z;
                                float d = h1 * h2 + h2 * betta * (h0 - h1) + h1 * gamma * (h0 - h2);
                                betta = h0 * h2 * betta / d;
                                gamma = h0 * h1 * gamma / d;
                                alpha = (1 - betta - gamma);

                                float uCoord = alpha * rasterized[0].U + betta * rasterized[1].U + gamma * rasterized[2].U;
                                float vCoord = alpha * rasterized[0].V + betta * rasterized[1].V + gamma * rasterized[2].V;
                                Vector textureColor = rasterized[0].Material.GetTexturePixelColor(uCoord, vCoord);

                                this.buffer[(int)x, (int)y] = alpha * Vector.ColorMultiplication(rasterized[0].BlinnPhongColor, textureColor) +
                                    betta * Vector.ColorMultiplication(rasterized[1].BlinnPhongColor, textureColor) +
                                    gamma * Vector.ColorMultiplication(rasterized[2].BlinnPhongColor, textureColor);
                            }
                            else
                            {
                                minZ = z;
                                this.zBuffer[(int)x, (int)y] = z;
                                this.buffer[(int)x, (int)y] = alpha * rasterized[0].BlinnPhongColor + betta * rasterized[1].BlinnPhongColor + gamma * rasterized[2].BlinnPhongColor;
                            }
                        }


                    }
            }

            
        }

    }
}
