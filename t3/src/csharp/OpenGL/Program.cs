using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Tao.FreeGlut;
using Tao.OpenGl;
using SceneLib;
using System.Drawing;
using System.Drawing.Imaging;

namespace OpenGL
{
  class Program
  {
    private static int WIDTH = 600;
    private static int HEIGHT = 600;

    //Vertex buffer ids
    private static int[] vertexBufferIdsArray;

    //Informacion de vertices
    private static float[] cubeVertices = new float[]
    {
       //Back
       500.0f, 0.0f, 500.0f, 1.0f,
       500.0f, 500.0f, 500.0f, 1.0f,
       0.0f, 500.0f, 500.0f, 1.0f,
                     
       0.0f, 500.0f, 500.0f, 1.0f,
       0.0f,  0.0f, 500.0f, 1.0f,
       500.0f,  0.0f, 500.0f, 1.0f,

       //Front
       500.0f, 0.0f, 0.0f, 1.0f,
       500.0f, 500.0f, 0.0f, 1.0f,
       0.0f, 500.0f, 0.0f, 1.0f,
                     
       0.0f, 500.0f, 0.0f, 1.0f,
       0.0f,  0.0f, 0.0f, 1.0f,
       500.0f,  0.0f, 0.0f, 1.0f,
       
       //Floor
       500.0f, 0.0f, 0.0f, 1.0f,
       500.0f, 0.0f, 500.0f, 1.0f,
       0.0f, 0.0f, 500.0f, 1.0f,
                     
       0.0f, 0.0f, 500.0f, 1.0f,
       0.0f,  0.0f, 0.0f, 1.0f,
       500.0f,  0.0f, 0.0f, 1.0f,

       //Ceiling
       500.0f, 500.0f, 0.0f, 1.0f,
       500.0f, 500.0f, 500.0f, 1.0f,
       0.0f, 500.0f, 500.0f, 1.0f,
                     
       0.0f, 500.0f, 500.0f, 1.0f,
       0.0f,  500.0f, 0.0f, 1.0f,
       500.0f,  500.0f, 0.0f, 1.0f,

       //Right
       0.0f, 0.0f, 0.0f, 1.0f,
       0.0f, 500.0f, 0.0f, 1.0f,
       0.0f, 500.0f, 500.0f, 1.0f,
                     
       0.0f, 500.0f, 500.0f, 1.0f,
       0.0f,  0.0f, 500.0f, 1.0f,
       0.0f,  0.0f, 0.0f, 1.0f,

       //Left
       500.0f, 0.0f, 0.0f, 1.0f,
       500.0f, 500.0f, 0.0f, 1.0f,
       500.0f, 500.0f, 500.0f, 1.0f,
                       
       500.0f, 500.0f, 500.0f, 1.0f,
       500.0f,  0.0f, 500.0f, 1.0f,
       500.0f,  0.0f, 0.0f, 1.0f,
       
       //Back colors
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
       
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
       
       //Front colors
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
       
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
       
       //Floor colors
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
       
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
        
       //Ceiling colors
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
       
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
       
       //Right colors
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
       
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
       
       //Left colors
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
       
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,
       1.0f,  1.0f, 1.0f, 1.0f,

       //Back normals
       0.0f,  0.0f, 1.0f, 0.0f,
       0.0f,  0.0f, 1.0f, 0.0f,
       0.0f,  0.0f, 1.0f, 0.0f,
       
       0.0f,  0.0f, 1.0f, 0.0f,
       0.0f,  0.0f, 1.0f, 0.0f,
       0.0f,  0.0f, 1.0f, 0.0f,
       
       //Front normals
       0.0f,  0.0f, -1.0f, 0.0f,
       0.0f,  0.0f, -1.0f, 0.0f,
       0.0f,  0.0f, -1.0f, 0.0f,
                          
       0.0f,  0.0f, -1.0f, 0.0f,
       0.0f,  0.0f, -1.0f, 0.0f,
       0.0f,  0.0f, -1.0f, 0.0f,
       
       //Floor normals
       0.0f,  -1.0f, 0.0f, 0.0f,
       0.0f,  -1.0f, 0.0f, 0.0f,
       0.0f,  -1.0f, 0.0f, 0.0f,
       
       0.0f,  -1.0f, 0.0f, 0.0f,
       0.0f,  -1.0f, 0.0f, 0.0f,
       0.0f,  -1.0f, 0.0f, 0.0f,
        
       //Ceiling normals
       0.0f,  1.0f, 0.0f, 0.0f,
       0.0f,  1.0f, 0.0f, 0.0f,
       0.0f,  1.0f, 0.0f, 0.0f,
       
       0.0f,  1.0f, 0.0f, 0.0f,
       0.0f,  1.0f, 0.0f, 0.0f,
       0.0f,  1.0f, 0.0f, 0.0f,
       
       //Right normals
       -1.0f,  0.0f, 0.0f, 0.0f,
       -1.0f,  0.0f, 0.0f, 0.0f,
       -1.0f,  0.0f, 0.0f, 0.0f,
       
       -1.0f,  0.0f, 0.0f, 0.0f,
       -1.0f,  0.0f, 0.0f, 0.0f,
       -1.0f,  0.0f, 0.0f, 0.0f,
       
       //Left normals
       1.0f,  0.0f, 0.0f, 0.0f,
       1.0f,  0.0f, 0.0f, 0.0f,
       1.0f,  0.0f, 0.0f, 0.0f,
       
       1.0f,  0.0f, 0.0f, 0.0f,
       1.0f,  0.0f, 0.0f, 0.0f,
       1.0f,  0.0f, 0.0f, 0.0f,

       //Back texture
       0.0f,  0.0f, 0.0f, 0.0f,
       0.0f,  1.0f, 0.0f, 0.0f,
       1.0f,  1.0f, 0.0f, 0.0f,
       
       1.0f,  1.0f, 0.0f, 0.0f,
       1.0f,  0.0f, 0.0f, 0.0f,
       0.0f,  0.0f, 0.0f, 0.0f,
       
       //Front texture
       0.0f,  0.0f, 0.0f, 0.0f,
       0.0f,  1.0f, 0.0f, 0.0f,
       1.0f,  1.0f, 0.0f, 0.0f,
       
       1.0f,  1.0f, 0.0f, 0.0f,
       1.0f,  0.0f, 0.0f, 0.0f,
       0.0f,  0.0f, 0.0f, 0.0f,
       
       //Floor texture
       0.0f,  0.0f, 0.0f, 0.0f,
       0.0f,  1.0f, 0.0f, 0.0f,
       1.0f,  1.0f, 0.0f, 0.0f,
       
       1.0f,  1.0f, 0.0f, 0.0f,
       1.0f,  0.0f, 0.0f, 0.0f,
       0.0f,  0.0f, 0.0f, 0.0f,
        
       //Ceiling texture
       0.0f,  0.0f, 0.0f, 0.0f,
       0.0f,  1.0f, 0.0f, 0.0f,
       1.0f,  1.0f, 0.0f, 0.0f,
       
       1.0f,  1.0f, 0.0f, 0.0f,
       1.0f,  0.0f, 0.0f, 0.0f,
       0.0f,  0.0f, 0.0f, 0.0f,
       
       //Right texture
       0.0f,  0.0f, 0.0f, 0.0f,
       0.0f,  1.0f, 0.0f, 0.0f,
       1.0f,  1.0f, 0.0f, 0.0f,
       
       1.0f,  1.0f, 0.0f, 0.0f,
       1.0f,  0.0f, 0.0f, 0.0f,
       0.0f,  0.0f, 0.0f, 0.0f,
       
       //Left texture
       0.0f,  0.0f, 0.0f, 0.0f,
       0.0f,  1.0f, 0.0f, 0.0f,
       1.0f,  1.0f, 0.0f, 0.0f,
       
       1.0f,  1.0f, 0.0f, 0.0f,
       1.0f,  0.0f, 0.0f, 0.0f,
       0.0f,  0.0f, 0.0f, 0.0f,
         
    };

    //Identificador del programa asociado a los shaders
    private static int shaderProgramId;

    //Camera y matriz mundo-clipping
    private static SceneCamera camera;
    private static Matrix viewProjection;

    //Identificador para referirse a matriz mundo-clipping y matriz de modelo
    private static int viewProjectionId;
    private static int modelTransformId;

    //Identificador para el atributo offset
    private static int offsetId;

    //Identificadores texturas
    private static int[] grassTextureId;
    private static Bitmap grassTexture;
    private static int[] mazeTextureId;
    private static Bitmap mazeTexture;
    private static int grassTextureUniformId;
    private static int mazeTextureUniformId;

    //Tamaño del mapa
    private static int worldSize = 64;

    static void Main(string[] args)
    {

      //Inicialización y seteo de buffers de display a utilizar
      Glut.glutInit();
      Glut.glutInitDisplayMode(Glut.GLUT_RGBA | Glut.GLUT_DEPTH | Glut.GLUT_DOUBLE);
		
      //Creación de ventana para desplegar rendering
      Glut.glutInitWindowSize(WIDTH, HEIGHT);
      Glut.glutInitWindowPosition(100, 100);
      Glut.glutCreateWindow("Hello OpenGL");

      //Definicion de callbacks
      Glut.glutDisplayFunc(new Glut.DisplayCallback(Draw)); //Callback de dibujo
      Glut.glutReshapeFunc(new Glut.ReshapeCallback(Resize)); //Callback de resize
      Glut.glutKeyboardFunc(new Glut.KeyboardCallback(KeyboardHandler));

      Glut.glutIdleFunc(new Glut.IdleCallback(Update));
      Glut.glutPassiveMotionFunc(new Glut.PassiveMotionCallback(MouseMove));
      Glut.glutMouseFunc(new Glut.MouseCallback(MouseClick));
      Glut.glutMouseWheelFunc(new Glut.MouseWheelCallback(MouseWheel));

      Init();

      //Inicio de loop principal de aplicacion: programa se queda pegado aqui
      Glut.glutMainLoop();
    }

    static void Init()
    {
      //Iniciamos los parametros de la escena
      InitScene();

      //Iniciamos la geometría de la escena copiandola a un VBO
      InitVBO();

      //Cargado de texturas
      InitTextures();

      //Seteo de parametros de OpenGL
      SetRenderingParameters();

      //Cargamos los shaders
      SetShaders(@"Shaders/Basic");

    }

    static void InitScene()
    {
      //Definimos nuestra camara
      camera = new SceneCamera();
      camera.NearClip = 10f;
      camera.FarClip = 35000;
      camera.FieldOfView = 45.0f;
      camera.Up = new Vector(0, 1, 0);
      camera.Target = new Vector(250, 550, 250);
      camera.Position = new Vector(-500, 1250, -500);
    }

    static void InitVBO()
    {
      //Ocuparemos un buffer, por lo que el arreglo de buffers tiene el valor 0 (indice del unico buffer)
      vertexBufferIdsArray = new int[] { 0 };

      //Creamos el buffer que será ocupado en la GPU
      Gl.glGenBuffers(1, vertexBufferIdsArray);

      //Antes de hacer operaciones con un buffer hay que hacer un bind
      Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, vertexBufferIdsArray[0]);

      //Una vez bindeado, le asigamos los datos a este buffer
      Gl.glBufferData(Gl.GL_ARRAY_BUFFER,
           (IntPtr)(cubeVertices.Length * sizeof(float)),
           cubeVertices, Gl.GL_STATIC_DRAW);

      Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, 0);
      
    }

    static void InitTextures()
    {
      //Id para textura del fragment shader
      grassTextureId = new int[] { 0 };
      Gl.glGenTextures(1, grassTextureId);

      //Cargamos textura
      grassTexture = (Bitmap)Bitmap.FromFile("Images/whiteGrass.jpg");
      //Obtenemos puntero a textura
      IntPtr ptrTexture = GetTexturePointer(grassTexture);
      //Binding de imagen a textura
      Gl.glActiveTexture(Gl.GL_TEXTURE0);
      Gl.glBindTexture(Gl.GL_TEXTURE_2D, grassTextureId[0]);
      Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
      Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
      Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, grassTexture.Width, grassTexture.Height, 0, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, ptrTexture);
      Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);

      //Id para textura del vertex shader, i.e. heightmap
      mazeTextureId = new int[] { 0 };
      Gl.glGenTextures(1, mazeTextureId);

      mazeTexture = (Bitmap)Bitmap.FromFile("Images/heightMap.jpg");
      //Obtenemos puntero a textura
      IntPtr mazePtrTexture = GetTexturePointer(mazeTexture);
      //Binding de imagen a textura
      Gl.glActiveTexture(Gl.GL_TEXTURE1);
      Gl.glBindTexture(Gl.GL_TEXTURE_2D, mazeTextureId[0]);
      Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
      Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
      Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, mazeTexture.Width, mazeTexture.Height, 0, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, mazePtrTexture);
      Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
    }

    //Se habilita el Z-Buffer y uso de Texturas
    static void SetRenderingParameters()
    {
      Gl.glEnable(Gl.GL_DEPTH_TEST);
      Gl.glEnable(Gl.GL_TEXTURE_2D);
    }

    //Carga y compilacion  de shaders
    static void SetShaders(string shader)
    {
      // Cargamos y compilamos el vertex shader
      int vs = Gl.glCreateShader(Gl.GL_VERTEX_SHADER);
      string[] vsSource = new string[] { System.IO.File.ReadAllText(shader + ".vert") };
      Gl.glShaderSource(vs, 1, vsSource, null);
      Gl.glCompileShader(vs);

      // Cargamos y compilamos el fragment shader
      int fs = Gl.glCreateShader(Gl.GL_FRAGMENT_SHADER);
      string[] fsSource = new string[] { System.IO.File.ReadAllText(shader + ".frag") };
      Gl.glShaderSource(fs, 1, fsSource, null);
      Gl.glCompileShader(fs);

      // Linkeamos los shaders a un programa entendible por OpenGL
      shaderProgramId = Gl.glCreateProgram();
      Gl.glAttachShader(shaderProgramId, vs);
      Gl.glAttachShader(shaderProgramId, fs);
      Gl.glLinkProgram(shaderProgramId);

      //Asociamos los indices de los atributos a nombres de variable en el shader
      Gl.glBindAttribLocation(shaderProgramId, 0, "position");
      Gl.glBindAttribLocation(shaderProgramId, 1, "color");
      Gl.glBindAttribLocation(shaderProgramId, 2, "normal");
      Gl.glBindAttribLocation(shaderProgramId, 3, "uv");

      //Obtenemos los identificadores de los uniforms que se ocuparan en ambos shaders
      viewProjectionId = Gl.glGetUniformLocation(shaderProgramId, "viewProjection");
      modelTransformId = Gl.glGetUniformLocation(shaderProgramId, "modelTransform");
      offsetId = Gl.glGetUniformLocation(shaderProgramId, "offset");
      grassTextureUniformId = Gl.glGetUniformLocation(shaderProgramId, "fragmentTex");
      mazeTextureUniformId = Gl.glGetUniformLocation(shaderProgramId, "vertexTex");
    }


    
    //Callback de display, acá hacemos todo el código de dibujo
    static void Draw()
    {
      //Define el color de fondo del framebuffer
      Gl.glClearColor(0, 0, 0, 1);
      
      //Limpia los buffers indicados
      Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

      //Le decimos a OpenGL que use nuestros shaders
      Gl.glUseProgram(shaderProgramId);
      
      //Obtenemos la transformacion de mundo a clipping (viewProjection)
      Matrix cameraMatrix = RenderUtils.CameraMatrix(camera);
      Matrix projectionMatrix = RenderUtils.PerspectiveProjectionMatrix(camera, WIDTH, HEIGHT);
      viewProjection = projectionMatrix * cameraMatrix;
      float[] matrixArray = viewProjection.GetMatrixArray();
      Gl.glUniformMatrix4fv(viewProjectionId, 1, Gl.GL_TRUE, matrixArray);

      //Antes de hacer operaciones con un buffer hay que hacer un bind
      Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, vertexBufferIdsArray[0]);

      //Indicamos que vamos a habiltar cuatro atributos del vertex shader
      Gl.glEnableVertexAttribArray(0);
      Gl.glEnableVertexAttribArray(1);
      Gl.glEnableVertexAttribArray(2);
      Gl.glEnableVertexAttribArray(3);

      //Indicamos que la ubicación y descripcion de ambos atributos en el vertex buffer 
      Gl.glVertexAttribPointer(0, 4, Gl.GL_FLOAT, Gl.GL_FALSE, 0, IntPtr.Zero);
      Gl.glVertexAttribPointer(1, 4, Gl.GL_FLOAT, Gl.GL_FALSE, 0, new IntPtr((cubeVertices.Length / 4) *  sizeof(float)));
      Gl.glVertexAttribPointer(2, 4, Gl.GL_FLOAT, Gl.GL_FALSE, 0, new IntPtr(2 * (cubeVertices.Length / 4) * sizeof(float)));
      Gl.glVertexAttribPointer(3, 4, Gl.GL_FLOAT, Gl.GL_FALSE, 0, new IntPtr(3 * (cubeVertices.Length / 4) * sizeof(float)));

      //Habiltamos dos unidades de texturas y asociamos los uniform correspondientes
      Gl.glActiveTexture(Gl.GL_TEXTURE0);
      Gl.glUniform1i(grassTextureUniformId, 0);
      Gl.glBindTexture(Gl.GL_TEXTURE_2D, grassTextureId[0]);
      Gl.glActiveTexture(Gl.GL_TEXTURE1);
      Gl.glUniform1i(mazeTextureUniformId, 1);
      Gl.glBindTexture(Gl.GL_TEXTURE_2D, mazeTextureId[0]);      
      
      //Model transform
      Matrix scale = Matrix.Scaling(new Vector(0.1f, 0.1f, 0.1f));
      for (int xOffset = 0; xOffset < worldSize; xOffset++)
      {
        for (int zOffset = 0; zOffset < worldSize; zOffset++)
        {
          Matrix translate = Matrix.Translation(new Vector(xOffset * 50, 0, zOffset * 50));
          Matrix model = translate * scale;
          float[] modelTransformArray = model.GetMatrixArray();
          Gl.glUniformMatrix4fv(modelTransformId, 1, Gl.GL_TRUE, modelTransformArray);
          Gl.glUniform2f(offsetId, (float)(xOffset) / 32.0f, (float)(zOffset) / 32.0f );
          //Dibujamos cubo
          Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, cubeVertices.Length / (4 * 4));
        }
        
      }
      //Deshabilitamos y desbindeamos todo lo usado
      Gl.glDisableVertexAttribArray(0);
      Gl.glDisableVertexAttribArray(1);
      Gl.glDisableVertexAttribArray(2);
      Gl.glDisableVertexAttribArray(3);

      Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
      Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, 0);
      Gl.glUseProgram(0);

      //Intercambio de los dos buffers del double buffer
      Glut.glutSwapBuffers();
    }

    private static IntPtr GetTexturePointer(Bitmap texture)
    {
      BitmapData data = texture.LockBits(new Rectangle(0, 0, texture.Width, texture.Height),
                                          ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
      IntPtr ptrTexture = data.Scan0;
      texture.UnlockBits(data);
      return ptrTexture;
    }

    //Llamado cada vez que se cambia el tamaño de la ventana con el nuevo tamaño
    static void Resize(int w, int h)
    {
      //Actualiza la transformacion de Viewport
      Gl.glViewport(0, 0, w, h);
    }

    
    static void KeyboardHandler(byte key, int x, int y)
    {
      Vector dir = camera.Target - camera.Position;
      float tar_posDist = dir.Magnitude3();
      dir.Normalize3();
      Vector right = Vector.Cross3(dir, camera.Up);
      float camSpeed = 100f;
      Vector tempVec;
      float turnSpeed = 0.1f;

      if (key == (byte)'w')
      {
        camera.Position += dir * camSpeed;
        camera.Target += dir * camSpeed;
        Glut.glutPostRedisplay();
      }
      else if (key == (byte)'s')
      {
        camera.Position -= dir * camSpeed;
        camera.Target -= dir * camSpeed;
        Glut.glutPostRedisplay();
      }
      else if (key == (byte)'a')
      {
        camera.Position -= right * camSpeed;
        camera.Target -= right * camSpeed;
        Glut.glutPostRedisplay();
      }
      else if (key == (byte)'d')
      {
        camera.Position += right * camSpeed;
        camera.Target += right * camSpeed;
        Glut.glutPostRedisplay();
      }
      else if (key == (byte)'i')
      {
        tempVec = dir + camera.Up * turnSpeed;
        tempVec.Normalize3();
        camera.Target = camera.Position + tempVec * tar_posDist;
        camera.Up = Vector.Cross3(right, tempVec);
        Glut.glutPostRedisplay();
      }
      else if (key == (byte)'k')
      {
        tempVec = dir - camera.Up * turnSpeed;
        tempVec.Normalize3();
        camera.Target = camera.Position + tempVec * tar_posDist;
        camera.Up = Vector.Cross3(right, tempVec);
        Glut.glutPostRedisplay();
      }
      else if (key == (byte)'j')
      {
        tempVec = dir - right * turnSpeed;
        tempVec.Normalize3();
        camera.Target = camera.Position + tempVec * tar_posDist;
        Glut.glutPostRedisplay();
      }
      else if (key == (byte)'l')
      {
        tempVec = dir + right * turnSpeed;
        tempVec.Normalize3();
        camera.Target = camera.Position + tempVec * tar_posDist;
        Glut.glutPostRedisplay();
      }
    }

    static void Update()
    {
    }

    static void MouseMove(int x, int y)
    {
      Console.WriteLine("x: " + x + ", y: " + y);
    }

    static void MouseClick(int state, int button, int x, int y)
    {
      Console.WriteLine("state: " + state + ", button: " + button + ", x: " + x + ", y: " + y);
    }

    static void MouseWheel(int wheel, int direction, int x, int y)
    {
      Console.WriteLine("wheel: " + wheel + ", direction: " + direction + ", x: " + x + ", y: " + y);
    }
  }
}
