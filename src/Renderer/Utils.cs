using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SceneLib;
using Tao.OpenGl;
using System.Drawing;

namespace Renderer
{
    class Utils
    {

        public static void DrawPixel(Vector position, Vector color)
        {
            Gl.glColor4f(color.x, color.y, color.z, color.w);
            Gl.glVertex2i((int)position.x, (int)position.y);
        }

        
    }
}
