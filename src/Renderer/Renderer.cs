using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SceneLib;
using Tao.FreeGlut;

namespace Renderer
{
    public class Renderer
    {

        protected Vector[,] buffer;

        protected Vector CalculateU(Vector up, Vector w)
        {
            Vector u = Vector.Cross3(up, w);
            u.Normalize3();
            return u;
        }

        protected Vector CalculateV(Vector w, Vector u)
        {
            Vector v = Vector.Cross3(w, u);
            return v;
        }

        protected Vector CalculateW(Vector eye, Vector target)
        {
            // -(t - e)
            Vector gazeDirection = eye - target;
            gazeDirection.Normalize3();
            return gazeDirection;
        }

        public Vector ShowColor(Vector position)
        {
            Vector color = buffer[(int)position.x, (int)position.y];
            Console.WriteLine(color);
            //buffer[(int)position.x, (int)position.y] = new Vector(0, 0, 0);
            Glut.glutPostRedisplay();
            return color;

        }
    }
}
