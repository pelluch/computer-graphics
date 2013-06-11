using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneLib
{
    public class MathUtils
    {
        public static float[] GetVector3Array(Vector v)
        {
            float[] array = new float[3];
            array[0] = v.x;
            array[1] = v.y;
            array[2] = v.z;
            return array;
        }

        public static float[] GetVector4Array(Vector v)
        {
            float[] array = new float[4];
            array[0] = v.x;
            array[1] = v.y;
            array[2] = v.z;
            array[3] = 1;
            return array;
        }        
    }
}
