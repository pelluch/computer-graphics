using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneLib
{
    public class Matrix
    {
        private const int MATRIX_SIZE = 4;
        public float[] data;
        public float[] Data
        {
            get { return data; }
        }

        public Matrix()
        {
            data = new float[MATRIX_SIZE*MATRIX_SIZE];
        }

        public Matrix(float[] coeffs)
        {
            data = coeffs;
        }

        public Matrix(Vector[] colVectors)
        {
            data = new float[MATRIX_SIZE * MATRIX_SIZE];
            for (int row = 0; row < MATRIX_SIZE; ++row)
            {
                for (int col = 0; col < MATRIX_SIZE; ++col)
                {
                    data[row * MATRIX_SIZE + col] = colVectors[col][row];
                }
            }
        }

        public float this[int row, int col]
        {
            get
            {
                float coeff = data[row * MATRIX_SIZE + col];
                return coeff;
            }
            set
            {
                data[row * MATRIX_SIZE + col] = value;
            }
        }

        public override string ToString()
        {
            for (int row = 0; row < MATRIX_SIZE; ++row)
            {
                for (int col = 0; col < MATRIX_SIZE; ++col)
                {
                    Console.Write(data[row * MATRIX_SIZE + col] + " ");
                }
                Console.WriteLine("");
            }
            return base.ToString();
        }
        public static Vector operator *(Matrix M, Vector v)
        {
            float x = M[0,0] * v.x + M[0,1] * v.y + M[0,2] * v.z + M[0,3] * v.w;
            float y = M[1, 0] * v.x + M[1, 1] * v.y + M[1, 2] * v.z + M[1, 3] * v.w;
            float z = M[2, 0] * v.x + M[2, 1] * v.y + M[2, 2] * v.z + M[2, 3] * v.w;
            float w = M[3, 0] * v.x + M[3, 1] * v.y + M[3, 2] * v.z + M[3, 3] * v.w;

            Vector transformedVector = new Vector(x, y, z, w);
            return transformedVector;
        }

        public static Matrix operator *(float scalar, Matrix M)
        {
            float[] newData = new float[MATRIX_SIZE*MATRIX_SIZE];
            for (int i = 0; i < MATRIX_SIZE * MATRIX_SIZE; i++)
                newData[i] = M.Data[i] * scalar;
            Matrix M2 = new Matrix(newData);
            return M2;
        }

        public static Matrix operator *(Matrix M1, Matrix M2)
        {  
            Vector v1 = new Vector(M2[0,0], M2[1,0], M2[2,0], M2[3,0]);
            Vector v2 = new Vector(M2[0, 1], M2[1, 1], M2[2, 1], M2[3, 1]);
            Vector v3 = new Vector(M2[0, 2], M2[1, 2], M2[2, 2], M2[3, 2]);
            Vector v4 = new Vector(M2[0, 3], M2[1, 3], M2[2, 3], M2[3, 3]);

            Vector v1Result = M1 * v1;
            Vector v2Result = M1 * v2;
            Vector v3Result = M1 * v3;
            Vector v4Result = M1 * v4;
            Vector[] colVectors = new Vector[] { v1Result, v2Result, v3Result, v4Result };
            Matrix result = new Matrix(colVectors);
            return result;
        }

        public static Matrix Identity()
        {
            float[] newData = new float[] { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 };
            Matrix result = new Matrix(newData);
            return result;
        }

    }
}
