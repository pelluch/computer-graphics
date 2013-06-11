using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneLib
{
  public class Matrix
  {
    private float[,] matrixData = new float[4, 4];

    public float[,] MatrixData
    {
      get { return matrixData; }
      set { matrixData = value; }
    }

    public float[] GetMatrixArray()
    {
      float[] array = new float[16];
      for (int i = 0, arrayIndex = 0; i < 4; i++)
      {
        for (int j = 0; j < 4; j++, arrayIndex++)
        {
          array[arrayIndex] = matrixData[i, j];
        }
      }
      return array;
    }

    public float M11 { get { return matrixData[0, 0]; } set { matrixData[0, 0] = value; } }
    public float M12 { get { return matrixData[0, 1]; } set { matrixData[0, 1] = value; } }
    public float M13 { get { return matrixData[0, 2]; } set { matrixData[0, 2] = value; } }
    public float M14 { get { return matrixData[0, 3]; } set { matrixData[0, 3] = value; } }
    public float M21 { get { return matrixData[1, 0]; } set { matrixData[1, 0] = value; } }
    public float M22 { get { return matrixData[1, 1]; } set { matrixData[1, 1] = value; } }
    public float M23 { get { return matrixData[1, 2]; } set { matrixData[1, 2] = value; } }
    public float M24 { get { return matrixData[1, 3]; } set { matrixData[1, 3] = value; } }
    public float M31 { get { return matrixData[2, 0]; } set { matrixData[2, 0] = value; } }
    public float M32 { get { return matrixData[2, 1]; } set { matrixData[2, 1] = value; } }
    public float M33 { get { return matrixData[2, 2]; } set { matrixData[2, 2] = value; } }
    public float M34 { get { return matrixData[2, 3]; } set { matrixData[2, 3] = value; } }
    public float M41 { get { return matrixData[3, 0]; } set { matrixData[3, 0] = value; } }
    public float M42 { get { return matrixData[3, 1]; } set { matrixData[3, 1] = value; } }
    public float M43 { get { return matrixData[3, 2]; } set { matrixData[3, 2] = value; } }
    public float M44 { get { return matrixData[3, 3]; } set { matrixData[3, 3] = value; } }

    public static Vector operator *(Matrix M, Vector v)
    {
      float x = M.M11 * v.x + M.M12 * v.y + M.M13 * v.z + M.M14 * v.w;
      float y = M.M21 * v.x + M.M22 * v.y + M.M23 * v.z + M.M24 * v.w;
      float z = M.M31 * v.x + M.M32 * v.y + M.M33 * v.z + M.M34 * v.w;
      float w = M.M41 * v.x + M.M42 * v.y + M.M43 * v.z + M.M44 * v.w;

      Vector transformedVector = new Vector(x, y, z, w);
      return transformedVector;
    }

    public static Matrix operator *(float scalar, Matrix M)
    {
      Matrix result = new Matrix();
      result.M11 = M.M11 * scalar;
      result.M12 = M.M12 * scalar;
      result.M13 = M.M13 * scalar;
      result.M14 = M.M14 * scalar;
      result.M21 = M.M21 * scalar;
      result.M22 = M.M22 * scalar;
      result.M23 = M.M23 * scalar;
      result.M24 = M.M24 * scalar;
      result.M31 = M.M31 * scalar;
      result.M32 = M.M32 * scalar;
      result.M33 = M.M33 * scalar;
      result.M34 = M.M34 * scalar;
      result.M41 = M.M41 * scalar;
      result.M42 = M.M42 * scalar;
      result.M43 = M.M43 * scalar;
      result.M44 = M.M44 * scalar;
      return result;
    }

    public static Matrix operator *(Matrix M1, Matrix M2)
    {
      Vector v1 = new Vector(M2.M11, M2.M21, M2.M31, M2.M41);
      Vector v2 = new Vector(M2.M12, M2.M22, M2.M32, M2.M42);
      Vector v3 = new Vector(M2.M13, M2.M23, M2.M33, M2.M43);
      Vector v4 = new Vector(M2.M14, M2.M24, M2.M34, M2.M44);

      Vector v1Result = M1 * v1;
      Vector v2Result = M1 * v2;
      Vector v3Result = M1 * v3;
      Vector v4Result = M1 * v4;

      Matrix result = new Matrix();
      result.M11 = v1Result.x;
      result.M12 = v2Result.x;
      result.M13 = v3Result.x;
      result.M14 = v4Result.x;
      result.M21 = v1Result.y;
      result.M22 = v2Result.y;
      result.M23 = v3Result.y;
      result.M24 = v4Result.y;
      result.M31 = v1Result.z;
      result.M32 = v2Result.z;
      result.M33 = v3Result.z;
      result.M34 = v4Result.z;
      result.M41 = v1Result.w;
      result.M42 = v2Result.w;
      result.M43 = v3Result.w;
      result.M44 = v4Result.w;
      return result;
    }

    public static Matrix Identity()
    {
      Matrix result = new Matrix();
      result.M11 = 1;
      result.M12 = 0;
      result.M13 = 0;
      result.M14 = 0;
      result.M21 = 0;
      result.M22 = 1;
      result.M23 = 0;
      result.M24 = 0;
      result.M31 = 0;
      result.M32 = 0;
      result.M33 = 1;
      result.M34 = 0;
      result.M41 = 0;
      result.M42 = 0;
      result.M43 = 0;
      result.M44 = 1;
      return result;
    }

    public static Matrix Translation(Vector translation)
    {
      Matrix transMat = new Matrix();

      transMat.M11 = 1;
      transMat.M12 = 0;
      transMat.M13 = 0;
      transMat.M14 = translation.x;
      transMat.M21 = 0;
      transMat.M22 = 1;
      transMat.M23 = 0;
      transMat.M24 = translation.y;
      transMat.M31 = 0;
      transMat.M32 = 0;
      transMat.M33 = 1;
      transMat.M34 = translation.z;
      transMat.M41 = 0;
      transMat.M42 = 0;
      transMat.M43 = 0;
      transMat.M44 = 1;

      return transMat;
    }

    public static Matrix Scaling(Vector scaling)
    {
      Matrix scalingMat = new Matrix();
      scalingMat.M11 = scaling.x;
      scalingMat.M12 = 0;
      scalingMat.M13 = 0;
      scalingMat.M14 = 0;
      scalingMat.M21 = 0;
      scalingMat.M22 = scaling.y;
      scalingMat.M23 = 0;
      scalingMat.M24 = 0;
      scalingMat.M31 = 0;
      scalingMat.M32 = 0;
      scalingMat.M33 = scaling.z;
      scalingMat.M34 = 0;
      scalingMat.M41 = 0;
      scalingMat.M42 = 0;
      scalingMat.M43 = 0;
      scalingMat.M44 = 1;
      return scalingMat;
    }

    public static Matrix Rotation(Vector rotation)
    {
      float xRadAngle = (float)(rotation.x * Math.PI / 180.0f);
      float yRadAngle = (float)(rotation.y * Math.PI / 180.0f);
      float zRadAngle = (float)(rotation.z * Math.PI / 180.0f);

      Matrix rotMatX = new Matrix();
      rotMatX.M11 = 1;
      rotMatX.M12 = 0;
      rotMatX.M13 = 0;
      rotMatX.M14 = 0;
      rotMatX.M21 = 0;
      rotMatX.M22 = (float)Math.Cos(xRadAngle);
      rotMatX.M23 = -(float)Math.Sin(xRadAngle);
      rotMatX.M24 = 0;
      rotMatX.M31 = 0;
      rotMatX.M32 = (float)Math.Sin(xRadAngle); ;
      rotMatX.M33 = (float)Math.Cos(xRadAngle);
      rotMatX.M34 = 0;
      rotMatX.M41 = 0;
      rotMatX.M42 = 0;
      rotMatX.M43 = 0;
      rotMatX.M44 = 1;

      Matrix rotMatY = new Matrix();
      rotMatY.M11 = (float)Math.Cos(yRadAngle);
      rotMatY.M12 = 0;
      rotMatY.M13 = (float)Math.Sin(yRadAngle);
      rotMatY.M14 = 0;
      rotMatY.M21 = 0;
      rotMatY.M22 = 1;
      rotMatY.M23 = 0;
      rotMatY.M24 = 0;
      rotMatY.M31 = -(float)Math.Sin(yRadAngle);
      rotMatY.M32 = 0;
      rotMatY.M33 = (float)Math.Cos(yRadAngle);
      rotMatY.M34 = 0;
      rotMatY.M41 = 0;
      rotMatY.M42 = 0;
      rotMatY.M43 = 0;
      rotMatY.M44 = 1;

      Matrix rotMatZ = new Matrix();
      rotMatZ.M11 = (float)Math.Cos(zRadAngle);
      rotMatZ.M12 = -(float)Math.Sin(zRadAngle);
      rotMatZ.M13 = 0;
      rotMatZ.M14 = 0;
      rotMatZ.M21 = (float)Math.Sin(zRadAngle);
      rotMatZ.M22 = (float)Math.Cos(zRadAngle);
      rotMatZ.M23 = 0;
      rotMatZ.M24 = 0;
      rotMatZ.M31 = 0;
      rotMatZ.M32 = 0;
      rotMatZ.M33 = 1;
      rotMatZ.M34 = 0;
      rotMatZ.M41 = 0;
      rotMatZ.M42 = 0;
      rotMatZ.M43 = 0;
      rotMatZ.M44 = 1;

      return rotMatX * rotMatY * rotMatZ;
    }

  }
}
