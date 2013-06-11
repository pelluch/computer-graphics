using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace SceneLib
{
  /// <summary>
  /// Several useful scene related objects are listed below
  /// </summary>

  /// <summary>
  /// Stores the background parameters of the scene
  /// </summary>
  public class SceneBackground
  {
    public Vector Color { get; set; }
    public Vector AmbientLight { get; set; }

  }

  public enum LightType
  {
    Point = 0,
    Directional,
    Spot
  }

  /// <summary>
  /// Represents a point light
  /// </summary>
  public class SceneLight
  {
    public float AtenuationConstant { get; set; }
    public float AtenuationLinear { get; set; }
    public float AtenuationQuadratic { get; set; }
    public Vector Color { get; set; }
    public Vector Position { get; set; }
    public Vector Direction { get; set; }
    public float FieldOfLight { get; set; }
    public Vector PhotonTarget { get; set; }
    public Vector PhotonUp { get; set; }
    public Vector A { get; set; }
    public Vector B { get; set; }
    public bool IsPhoton { get; set; }
    public bool CanCreatePhotonMap { get; set; }

    public LightType TypeOfLight { get; set; }

  }

  /// <summary>
  /// Represents a camera
  /// </summary>
  public class SceneCamera
  {
    public Vector Position { get; set; }
    public Vector Target { get; set; }
    public Vector Up { get; set; }
    public Vector U { get; set; }
    public Vector V { get; set; }
    public Vector W { get; set; }
    public float FieldOfView { get; set; }
    public float NearClip { get; set; }
    public float FarClip { get; set; }
  }

  /// <summary>
  /// Represents a material
  /// </summary>
  public class SceneMaterial
  {
    //Texture map image
    public Bitmap TextureImage;
    private IntPtr ptrBitmap;

    //Bump mapping image
    public Bitmap BumpImage;
    private IntPtr ptrBumpBitmap;

    //Normal mapping image
    public Bitmap NormalImage;
    private IntPtr ptrNormalBitmap;

    //Normal mapping image
    public Bitmap EnvironmentMapImage;
    private IntPtr ptrEnvironmentMapBitmap;

    public string Name { get; set; }
    public string TextureFile { get; set; }
    public string BumpFile { get; set; }
    public string NormalFile { get; set; }
    public string EnvironmentMapFile { get; set; }
    public Vector Diffuse { get; set; }
    public Vector Specular { get; set; }
    public Vector Transparent { get; set; }
    public Vector Reflective { get; set; }
    public Vector RefractionIndex { get; set; }
    public float Shininess { get { return Specular.w; } set { Specular.w = value; } }
    public IntPtr PtrBitmap { get { return ptrBitmap; } }
    public IntPtr PtrBumpBitmap { get { return ptrBumpBitmap; } }
    public IntPtr PtrNormalBitmap { get { return ptrNormalBitmap; } }
    public IntPtr PtrEnironemtnMapBitmap { get { return ptrEnvironmentMapBitmap; } }

    public SceneMaterial()
    {
      Specular = new Vector(0, 0, 0);
      Diffuse = new Vector(0, 0, 0);
    }

    /// <summary>
    /// Returns the color of the texture associated to this pixel
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector GetTexturePixelColor(int x, int y)
    {
      if (TextureImage == null)
        return null;

      Color c = TextureImage.GetPixel(x, y);
      Vector color = new Vector(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f, c.A / 255.0f);
      return color;
    }

    public Vector GetFilteredTexturePixelColor(float u, float v, SceneTextureMode textureFilter)
    {
      if (TextureImage == null)
        return null;
      if (u < 0 || u > 1 || v < 0 || v > 1)
        return null;
      float x = (TextureImage.Width - 1) * u;
      float y = (TextureImage.Height - 1) * v;
      Color Cij = TextureImage.GetPixel((int)x, (int)y);
      Vector color;
      if (textureFilter == SceneTextureMode.Bilinear)
      {
        float uPrima = x - (int)x;
        float vPrima = y - (int)y;
        Color Ci1j = TextureImage.GetPixel((int)x + 1, (int)y);
        Color Cij1 = TextureImage.GetPixel((int)x, (int)y + 1);
        Color Ci1j1 = TextureImage.GetPixel((int)x + 1, (int)y + 1);
        float R = (1 - uPrima) * (1 - vPrima) * Cij.R +
                   uPrima * (1 - vPrima) * Ci1j.R +
                   (1 - uPrima) * vPrima * Cij1.R +
                   uPrima * vPrima * Ci1j1.R;
        float G = (1 - uPrima) * (1 - vPrima) * Cij.G +
                   uPrima * (1 - vPrima) * Ci1j.G +
                   (1 - uPrima) * vPrima * Cij1.G +
                   uPrima * vPrima * Ci1j1.G;
        float B = (1 - uPrima) * (1 - vPrima) * Cij.B +
                   uPrima * (1 - vPrima) * Ci1j.B +
                   (1 - uPrima) * vPrima * Cij1.B +
                   uPrima * vPrima * Ci1j1.B;
        color = new Vector(R / 255.0f, G / 255.0f, B / 255.0f, 1.0f);
      }
      else
        color = new Vector(Cij.R / 255.0f, Cij.G / 255.0f, Cij.B / 255.0f, 1.0f);
      return color;
    }


    /// <summary>
    /// Creates an image pointer (for OpenGL use)
    /// </summary>
    public void CreatePointer()
    {
      BitmapData data = this.TextureImage.LockBits(new Rectangle(0, 0, TextureImage.Height, TextureImage.Width),
      ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
      ptrBitmap = data.Scan0;
      this.TextureImage.UnlockBits(data);
    }

  }

  /// <summary>
  /// Represent an abstract object in the scene
  /// </summary>
  public abstract class SceneObject
  {
    public string Name { get; set; }
    public Vector Scale { get; set; }
    public Vector Position { get; set; }
    public Vector Rotation { get; set; }

    public SceneObject()
    {
      Scale = new Vector(1, 1, 1);
      Position = new Vector(0, 0, 0);
      Rotation = new Vector(0, 0, 0);
    }

    public virtual Vector PositionInTime(float deltaTime) { return Position; }
  }

  /// <summary>
  /// Corresponds to a sphere geometrical object
  /// </summary>
  public class SceneSphere : SceneObject
  {
    public Vector Center { get; set; }
    public float Radius { get; set; }
    public SceneMaterial Material { get; set; }

    public Vector Velocity { get; set; }

    public SceneSphere()
    {

    }

    public override Vector PositionInTime(float deltaTime)
    {
      return Center + deltaTime * Velocity;
    }
  }

  public interface IHaveTriangles
  {
    List<SceneTriangle> Triangles { get; }
    Vector Scale { get; set; }
    Vector Position { get; set; }
    Vector Rotation { get; set; }
  }

  /// <summary>
  /// Corresponds to a triangle geometrical object
  /// </summary>
  public class SceneTriangle : SceneObject, IHaveTriangles
  {
    public List<Vector> Vertices { get; set; }
    public List<Vector> VerticesClip { get; set; }
    public List<Vector> VerticesView { get; set; }
    public List<Vector> VerticesShadow { get; set; }
    public List<Vector> LightDirectionsView { get; set; }
    public List<Vector> HalfDirectionsView { get; set; }
    public List<Vector> Normal { get; set; }
    public List<Vector> NormalModelView { get; set; }
    public List<Vector> Colors { get; set; }
    public List<SceneMaterial> Materials { get; set; }
    public List<float> U { get; set; }
    public List<float> V { get; set; }

    public bool InClipCoords = false;

    private List<SceneTriangle> thisTriangleList = new List<SceneTriangle>();

    public SceneTriangle()
    {
      Vertices = new List<Vector>();
      VerticesClip = new List<Vector>();
      VerticesView = new List<Vector>();
      VerticesShadow = new List<Vector>();
      Normal = new List<Vector>();
      Materials = new List<SceneMaterial>();
      Colors = new List<Vector>();
      U = new List<float>();
      V = new List<float>();
      LightDirectionsView = new List<Vector>();
      HalfDirectionsView = new List<Vector>();
      NormalModelView = new List<Vector>();
      thisTriangleList.Add(this);
    }

    public List<SceneTriangle> Triangles
    {
      get { return thisTriangleList; }
    }
  }

  /// <summary>
  /// Correspond to a mesh object
  /// </summary>
  public class SceneModel : SceneObject, IHaveTriangles
  {
    public string FileName { get; set; }
    public List<SceneTriangle> Triangles { get; set; }

    public int NumTriangles { get { return Triangles == null ? 0 : Triangles.Count; } }

    public SceneModel(string name, string filename)
    {
      Name = name;
      Triangles = new List<SceneTriangle>();
      FileName = filename;
    }

    public SceneTriangle GetTriangle(int index)
    {
      return Triangles == null || index < 0 || index > Triangles.Count ? null : Triangles[index];
    }
  }
}
