using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SceneLib;

namespace OpenGL
{
  public static class RenderUtils
  {
    public static void ComputeUVW(SceneCamera camera)
    {
      camera.W = camera.Target - camera.Position;
      camera.W.Normalize3();
      camera.W *= -1;

      camera.U = Vector.Cross3(camera.Up, camera.W);
      camera.U.Normalize3();

      camera.V = Vector.Cross3(camera.W, camera.U);
    }

    public static float[] ComputeImageDimensions(SceneCamera camera, int width, int height, float nearClip)
    {
      float[] bounds = new float[4];
      float t = bounds[0] = (float)(Math.Abs(nearClip) * Math.Tan(((camera.FieldOfView / 2) / 180.0) * Math.PI));
      float b = bounds[1] = -t;
      float r = bounds[2] = t * width / height;
      float l = bounds[3] = -r;

      return bounds;
    }

    public static Matrix PerspectiveProjectionMatrix(SceneCamera sceneCam, int width, int height)
    {
      Matrix projectionMatrix = Matrix.Identity();
      float[] bounds = ComputeImageDimensions(sceneCam, width, height, sceneCam.NearClip);
      float t = bounds[0];
      float b = bounds[1];
      float r = bounds[2];
      float l = bounds[3];
      float n = -sceneCam.NearClip;
      float f = -sceneCam.FarClip;

      projectionMatrix.MatrixData[0, 0] = -2*n/(r-l);
      projectionMatrix.MatrixData[0, 2] = (r + l) / (r - l);
      projectionMatrix.MatrixData[1, 1] = -2*n/(t-b);
      projectionMatrix.MatrixData[1, 2] = (t+b) / (t - b);
      projectionMatrix.MatrixData[2, 2] = (n + f)/(n-f);
      projectionMatrix.MatrixData[2, 3] = -(2 * n * f) / (n - f);
      projectionMatrix.MatrixData[3, 2] = -1;
      projectionMatrix.MatrixData[3, 3] = 0;


      return projectionMatrix;
    }  

    public static Matrix CameraMatrix(SceneCamera sceneCam)
    {
      ComputeUVW(sceneCam);

      Matrix baseRotationMatrix = Matrix.Identity();
      baseRotationMatrix.MatrixData[0, 0] = sceneCam.U.x;
      baseRotationMatrix.MatrixData[0, 1] = sceneCam.U.y;
      baseRotationMatrix.MatrixData[0, 2] = sceneCam.U.z;
      baseRotationMatrix.MatrixData[1, 0] = sceneCam.V.x;
      baseRotationMatrix.MatrixData[1, 1] = sceneCam.V.y;
      baseRotationMatrix.MatrixData[1, 2] = sceneCam.V.z;
      baseRotationMatrix.MatrixData[2, 0] = sceneCam.W.x;
      baseRotationMatrix.MatrixData[2, 1] = sceneCam.W.y;
      baseRotationMatrix.MatrixData[2, 2] = sceneCam.W.z;

      Matrix OriginTranslationMatrix = Matrix.Translation(new Vector(-sceneCam.Position.x, -sceneCam.Position.y, -sceneCam.Position.z));

      return baseRotationMatrix * OriginTranslationMatrix;

    }
  }
}
