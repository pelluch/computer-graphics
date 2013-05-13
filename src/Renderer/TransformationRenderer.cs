﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SceneLib;
using Tao.OpenGl;
using Tao.FreeGlut;
using System.Drawing;
using System.IO;

namespace Renderer
{
    public class TransformationRenderer : Renderer
    {
        private Scene scene;
        private int width;
        private int height;
        
        private Vector[,] buffer;
        private float[,] zBuffer;

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
        }
                

        public void Update()
        {
            InnerRender();

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

        /// <summary>
        /// Método de prueba
        /// </summary>
        private void InnerRender()
        {

            foreach(SceneObject obj in scene.Objects)
            {

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


            //DrawLine(new Vector(50, 50), new Vector(100, 100), new Vector(0, 0, 1), new Vector(1, 0, 0), true);

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

            //Use radians
            float fov = (float)Math.PI * (scene.Camera.FieldOfView / 180.0f);
            float near = scene.Camera.NearClip;
            //float far = scene.Camera.FarClip;
            float far = float.MaxValue;

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

            int x1 = (int)p1.x, x2 = (int)p2.x, y1 = (int)p1.y, y2 = (int)p2.y; //Obtenemos puntos iniciales
            //Punto de inicio = punto final
            if (x1 == x2 && y1 == y2)
            {
                this.buffer[x1, y1] = c1;
                return;
            }

            int deltax = Math.Abs(x2 - x1), deltay = Math.Abs(y2 - y1); //Obtenemos diferencias entre puntos
            int x = x1, y = y1; //x e y son los puntos actuales que dibujamos.

            int xinc1, xinc2, yinc1, yinc2, den, num, numadd, numpixels;

            if (x2 >= x1)
            {
                xinc1 = 1;
                xinc2 = 1;
            }
            else
            {
                xinc1 = -1;
                xinc2 = -1;
            }

            if (y2 >= y1)
            {
                yinc1 = 1;
                yinc2 = 1;
            }
            else
            {
                yinc1 = -1;
                yinc2 = -1;
            }

            if (deltax >= deltay)  //La pendiente es menor que 1, aumentaremos x de uno en uno
            {
                xinc1 = 0;
                yinc2 = 0;
                den = deltax;
                num = deltax / 2;
                numadd = deltay;
                numpixels = deltax;
            }
            else
            {
                xinc2 = 0;
                yinc1 = 0;
                den = deltay;
                num = deltay / 2;
                numadd = deltax;
                numpixels = deltay;
            }

            for (int pixels = 0; pixels <= numpixels; pixels++)
            {
                this.buffer[x, y] = new Vector(0, 0, 0);
                num += numadd;
                if (num >= den)
                {
                    num -= den;
                    x += xinc1;
                    y += yinc1;
                }
                x += xinc2;
                y += yinc2;
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
            float y1 = p1.y, y2 = p2.y, x1 = p1.x, x2 = p2.x;

            //Punto de inicio = punto final
            if (x1 == y1 && x2 == y2)
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
            }
            //La pendiente es mayor que uno. Si la regla es vertical, ingresa aquí y no se producen errores.
            else
            {
                xinc2 = 0;
                yinc1 = 0;
                numpixels = (int)deltay;
                m = (x2 - x1) / (y2 - y1);
                colorM = (c2 - c1) / (y2 - y1);

            }
            int decreasing;
            if (xinc2 == -1 || yinc2 == -1) decreasing = -1;
            else decreasing = 1;

            float y = y1, x = x1;
            Vector c = new Vector(c1.x, c1.y, c1.z, 1);

            //Al contrario del algoritmo tradicional de Xaolin, no es necesario hacer un caso específico
            //para el primer y último punto de la recta pues estos serán enteros.
            for (int pixels = 0; pixels <= numpixels; pixels++)
            {
                float fractional, invFractional;
                if (deltax >= deltay) invFractional = y - (int)y;
                else invFractional = x - (int)x;

                fractional = 1 - invFractional;

                this.buffer[(int)x, (int)y] = new Vector(c.x, c.y, c.z, fractional);
                this.buffer[(int)(x + xinc1), (int)(y + yinc1)] = new Vector(c.x, c.y, c.z, invFractional);

                c = c + colorM * decreasing;


                x = x + m * xinc1 * yinc2;
                y = y + m * yinc1 * xinc2;

                y = y + yinc2;
                x = x + xinc2;
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
        private void DrawTriangleWire(Vector p1, Vector p2, Vector p3, Vector c1, Vector c2,
            Vector c3, bool antialias)
        {
            DrawLine(p1, p2, c1, c2, antialias);
            DrawLine(p1, p3, c1, c3, antialias);
            DrawLine(p2, p3, c2, c3, antialias);
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
        private void DrawTriangle(Vector p1, Vector p2, Vector p3, Vector c1, Vector c2,
            Vector c3, bool antialias)
        {

            if (antialias)
                DrawTriangleWire(p1, p2, p3, c1, c2, c3, antialias);

            float x1 = p1.x, x2 = p2.x, x3 = p3.x, y1 = p1.y, y2 = p2.y, y3 = p3.y;
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

            for (float x = leftmost; x <= rightmost; x++)
            {
                for (float y = bottommost; y <= topmost; y++)
                {
                    float u = (y2y3 * (x - x3) - x2x3 * (y - y3)) / den1;
                    if (u < 0 || u > 1) continue;

                    float v = (-y1y3 * (x - x3) + x1x3 * (y - y3)) / den2;
                    if (v < 0 || v > 1) continue;

                    float w = 1 - u - v;
                    if (w < 0 || w > 1) continue;

                    this.buffer[(int)x, (int)y] = u * c1 + v * c2 + w * c3;
                }

            }

        }

    }
}
