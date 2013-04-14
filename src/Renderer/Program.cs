using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Tao.FreeGlut;
using Tao.OpenGl;
using Renderer;
using SceneLib;

namespace Renderer
{
    enum RendererType
    {
        Raytracer,
        OpenGL,
        RasterizerWireframe,
        Rasterizer
    }

    enum NumberKeys
    {
        One = 49,
        Two,
        Three,
        Four
    }

    enum ArrowKeys
    {
        Left = 100,
        Up,
        Right,
        Down
    }


    class Program
    {
        private static int WIDTH = 800;
        private static int HEIGHT = 800;

        private static RendererType mode = RendererType.OpenGL;
        
        private static OpenGLRenderer openGLrenderer;
        private static RaytraceRenderer raytraceRenderer;
        private static Scene scene;

         static void Main(string[] args)
         {
             Glut.glutInit();
             Glut.glutInitDisplayMode(Glut.GLUT_SINGLE | Glut.GLUT_RGB);

             Glut.glutInitWindowSize(WIDTH, HEIGHT);
             Glut.glutInitWindowPosition(100, 100);
             Glut.glutCreateWindow("Renderer");
  
             Init();
  
             Glut.glutDisplayFunc(new Glut.DisplayCallback(Draw));
             Glut.glutIdleFunc(new Glut.IdleCallback(Update));

             Glut.glutKeyboardFunc(new Glut.KeyboardCallback(KeyboardHandle));
             Glut.glutSpecialFunc(new Glut.SpecialCallback(SpecialKeysHandler));
             Glut.glutMouseFunc(new Glut.MouseCallback(MouseHandler));
             Glut.glutMainLoop();
  
         }

         public static void MouseHandler(int button, int state, int x, int y)
         {
             if (state == 1)
             {

                 Console.WriteLine("x: " + x);
                 Console.WriteLine("y: " + y);
                 raytraceRenderer.ShowMouse(x+10, 785-y);
                 raytraceRenderer.CalculatePixel(x+10, 785-y);
                 
             }             
         }

         static void Init()
         {
             scene = new Scene(WIDTH, HEIGHT);
             scene.Load(@"Scenes/cornellBox2.xml");
             openGLrenderer = new OpenGLRenderer(scene, WIDTH, HEIGHT);
             raytraceRenderer = new RaytraceRenderer(scene, WIDTH, HEIGHT);
         }

        
         static void Draw()
         {
             switch (mode)
             {
                 case RendererType.Raytracer:
                     raytraceRenderer.Render();
                     break;
                 case RendererType.OpenGL:
                     openGLrenderer.Render();
                     break;
                 default:
                     break;
             }
         }

         static void Update()
         {
             if (mode == RendererType.Raytracer)
             {
                 raytraceRenderer.Update();
             }
         }

         static void SpecialKeysHandler(int key, int x, int y)
         {
             if (key == (int)ArrowKeys.Left)
             {
                 scene.Lights[0].Position += new Vector(10, 0, 0);
                 raytraceRenderer.ResetTracer();
                 Glut.glutPostRedisplay();
             }
             else if (key == (int)ArrowKeys.Up)
             {
                 scene.Lights[0].Position += new Vector(0, 0, 10);
                 raytraceRenderer.ResetTracer();
                 Glut.glutPostRedisplay();
             }
             else if (key == (int)ArrowKeys.Right)
             {
                 scene.Lights[0].Position += new Vector(-10, 0, 0);
                 raytraceRenderer.ResetTracer();                 
                 Glut.glutPostRedisplay();
             }
             else if (key == (int)ArrowKeys.Down)
             {
                 scene.Lights[0].Position += new Vector(0, 0, -10);
                 raytraceRenderer.ResetTracer();
                 Glut.glutPostRedisplay();
             }
 
         }

         static void KeyboardHandle(byte key, int x, int y)
         {
             SceneCamera sceneCam = scene.Camera;
             Vector dir = sceneCam.Target - sceneCam.Position;
             float tar_posDist = dir.Magnitude3();
             dir.Normalize3();
             Vector right = Vector.Cross3(dir, sceneCam.Up);
             float camSpeed = 10f;
             Vector tempVec;
             float turnSpeed = 0.01f;

             if (key == (int)NumberKeys.One)
             {
                 mode = RendererType.OpenGL;
                 Glut.glutPostRedisplay();
             }
             else if (key == (int)NumberKeys.Two)
             {
                 mode = RendererType.Raytracer;
                 raytraceRenderer.ResetTracer();
                 Glut.glutPostRedisplay();
             }
             else if (key == (int)NumberKeys.Three)
             {
                 mode = RendererType.RasterizerWireframe;
                 Glut.glutPostRedisplay();
             }
             else if (key == (int)NumberKeys.Four)
             {
                 mode = RendererType.Rasterizer;
                 Glut.glutPostRedisplay();
             }
             else if (key == (byte)'w')
             {
                 scene.Camera.Position += dir * camSpeed;
                 scene.Camera.Target += dir * camSpeed;
                 raytraceRenderer.ResetTracer();
                 Glut.glutPostRedisplay();
             }
             else if (key == (byte)'s')
             {
                 scene.Camera.Position -= dir * camSpeed;
                 scene.Camera.Target -= dir * camSpeed;
                 raytraceRenderer.ResetTracer();
                 Glut.glutPostRedisplay();
             }
             else if (key == (byte)'a')
             {
                 scene.Camera.Position -= right * camSpeed;
                 scene.Camera.Target -= right * camSpeed;
                 raytraceRenderer.ResetTracer();
                 Glut.glutPostRedisplay();
             }
             else if (key == (byte)'d')
             {
                 scene.Camera.Position += right * camSpeed;
                 scene.Camera.Target += right * camSpeed;
                 raytraceRenderer.ResetTracer();
                 Glut.glutPostRedisplay();
             }
             else if (key == (byte)'i')
             {
                 tempVec = dir + scene.Camera.Up * turnSpeed;
                 tempVec.Normalize3();
                 scene.Camera.Target = scene.Camera.Position + tempVec * tar_posDist;
                 scene.Camera.Up = Vector.Cross3(right, tempVec);
                 raytraceRenderer.ResetTracer();
                 Glut.glutPostRedisplay();
             }
             else if (key == (byte)'k')
             {
                 tempVec = dir - scene.Camera.Up * turnSpeed;
                 tempVec.Normalize3();
                 scene.Camera.Target = scene.Camera.Position + tempVec * tar_posDist;
                 scene.Camera.Up = Vector.Cross3(right, tempVec);
                 raytraceRenderer.ResetTracer();
                 Glut.glutPostRedisplay();
             }
             else if (key == (byte)'j')
             {
                 tempVec = dir - right * turnSpeed;
                 tempVec.Normalize3();
                 scene.Camera.Target = scene.Camera.Position + tempVec * tar_posDist;
                 raytraceRenderer.ResetTracer();
                 Glut.glutPostRedisplay();
             }
             else if (key == (byte)'l')
             {
                 tempVec = dir + right * turnSpeed;
                 tempVec.Normalize3();
                 scene.Camera.Target = scene.Camera.Position + tempVec * tar_posDist;
                 raytraceRenderer.ResetTracer();
                 Glut.glutPostRedisplay();
             }
             else if (key == (byte)'x')
             {
                 raytraceRenderer.renderingParameters.EnableRefractions = !raytraceRenderer.renderingParameters.EnableRefractions;
                 raytraceRenderer.ResetTracer();
                 Glut.glutPostRedisplay();
             }
             else if (key == (byte)'z')
             {
                 raytraceRenderer.renderingParameters.EnableShadows = !raytraceRenderer.renderingParameters.EnableShadows;
                 raytraceRenderer.ResetTracer();
                 Glut.glutPostRedisplay();
             }
             else if (key == (byte)'c')
             {
                 raytraceRenderer.renderingParameters.EnableAntialias = !raytraceRenderer.renderingParameters.EnableAntialias;
                 raytraceRenderer.ResetTracer();
                 Glut.glutPostRedisplay();
             }


         }

       
    }
}
