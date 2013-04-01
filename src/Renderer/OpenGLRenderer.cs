using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;
using SceneLib;
using Tao.FreeGlut;

namespace Renderer
{
	class OpenGLRenderer
	{
		private Scene scene;
		private int width;
		private int height;

		public OpenGLRenderer(Scene scene, int width, int height)
		{
			this.scene = scene;
			this.width = width;
			this.height = height;
			
		}

		public void Render()
		{
            
            /*
            Console.WriteLine("Position");
            Console.WriteLine(this.scene.Camera.Position);
            Console.WriteLine("Target");
            Console.WriteLine(this.scene.Camera.Target);
             */
			if (scene != null)
			{
				// Prepare Render States
				SetRenderStates();

				// Get the Camera
				SceneCamera sceneCam = scene.Camera;

				Gl.glMatrixMode(Gl.GL_PROJECTION);
				Gl.glLoadIdentity();
				Glu.gluPerspective(sceneCam.FieldOfView, (double)width / (double)height, sceneCam.NearClip, sceneCam.FarClip);

				Gl.glMatrixMode(Gl.GL_MODELVIEW);
				Gl.glLoadIdentity();
				Glu.gluLookAt(sceneCam.Position.x, sceneCam.Position.y, sceneCam.Position.z,
							sceneCam.Target.x, sceneCam.Target.y, sceneCam.Target.z,
							sceneCam.Up.x, sceneCam.Up.y, sceneCam.Up.z);

				
				// Get the Background
				SceneBackground sceneBg = scene.Background;
				Gl.glClearColor(sceneBg.Color.x, sceneBg.Color.y, sceneBg.Color.z, 0);
				Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, MathUtils.GetVector3Array(sceneBg.AmbientLight));
				Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
				

				// Get the Lights (OpenGL will only support up to 8)
				int numLights = scene.Lights.Count;
				for (int n = 0; n < numLights && n < 8; n++)
				{
					SceneLight sceneLight = scene.Lights[n];
			
					Gl.glLightfv(Gl.GL_LIGHT0 + n, Gl.GL_SPECULAR, MathUtils.GetVector4Array(sceneLight.Color));
					Gl.glLightf (Gl.GL_LIGHT0 + n, Gl.GL_CONSTANT_ATTENUATION, sceneLight.AtenuationConstant);
					Gl.glLightf (Gl.GL_LIGHT0 + n, Gl.GL_LINEAR_ATTENUATION, sceneLight.AtenuationLinear);
					Gl.glLightf (Gl.GL_LIGHT0 + n, Gl.GL_QUADRATIC_ATTENUATION, sceneLight.AtenuationQuadratic);
					Gl.glLightfv(Gl.GL_LIGHT0 + n, Gl.GL_DIFFUSE, MathUtils.GetVector4Array(sceneLight.Color));
					Gl.glLightfv(Gl.GL_LIGHT0 + n, Gl.GL_POSITION, MathUtils.GetVector4Array(sceneLight.Position));
				}

				

				// Get the Objects and Render them
				int numObjs = scene.Objects.Count;
				for (int n = 0; n < numObjs; n++)
				{
					SceneObject sceneObj = scene.Objects[n];

					// Store the current Transformation Matrix
					Gl.glPushMatrix ();

                    if (sceneObj.GetType().Equals(typeof(SceneModel)))
                    {
                        SceneModel sceneModel = ((SceneModel)sceneObj);
                        Gl.glTranslatef(sceneModel.Position.x, sceneModel.Position.y, sceneModel.Position.z);
                        Gl.glRotatef(sceneModel.Rotation.x, 1.0f, 0.0f, 0.0f);
                        Gl.glRotatef(sceneModel.Rotation.y, 0.0f, 1.0f, 0.0f);
                        Gl.glRotatef(sceneModel.Rotation.z, 0.0f, 0.0f, 1.0f);
                        Gl.glScalef(sceneModel.Scale.x, sceneModel.Scale.y, sceneModel.Scale.z);

                        SetTextures(sceneModel.Triangles[0].Materials[0], true);
                        foreach (SceneTriangle subTriangle in sceneModel.Triangles)
                            ProcessTriangle(subTriangle, true);
                    }
					else if (sceneObj.GetType().Equals(typeof(SceneTriangle)))
					{
                        SceneTriangle tr = (SceneTriangle)sceneObj;
                        SetTextures(tr.Materials[0], true);
                        ProcessTriangle(tr, true);
					}
					else if (sceneObj.GetType().Equals(typeof(SceneSphere)))
					{
						// Set the Material
						SceneSphere sceneSphere = ((SceneSphere)sceneObj);
						SceneMaterial sceneMat = sceneSphere.Material;
						Gl.glColor3fv(MathUtils.GetVector3Array(sceneMat.Diffuse));
						Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, MathUtils.GetVector3Array(sceneMat.Specular));
						Gl.glMaterialf(Gl.GL_FRONT, Gl.GL_SHININESS, sceneMat.Shininess);

						// Set Transformations
						Vector translate = sceneSphere.Position + sceneSphere.Center;
						Gl.glTranslatef(translate.x, translate.y, translate.z);
						Gl.glRotatef(sceneSphere.Rotation.x, 1.0f, 0.0f, 0.0f);
						Gl.glRotatef(sceneSphere.Rotation.y, 0.0f, 1.0f, 0.0f);
						Gl.glRotatef(sceneSphere.Rotation.z, 0.0f, 0.0f, 1.0f);
						Gl.glScalef(sceneSphere.Scale.x, sceneSphere.Scale.y, sceneSphere.Scale.z);

						// Draw the Sphere
						Glut.glutSolidSphere(((SceneSphere)sceneObj).Radius, 16, 16);
					  
					}
					
					// Restore the Transformation Matrix
					Gl.glPopMatrix ();
				}

				// Restore Render States
				UnsetRenderStates ();
			}
		}

        private void ProcessTriangle(SceneTriangle sceneTriangle, bool bilinear)
        {
            SceneMaterial sceneMat;
            
            // Set Transformations
            Gl.glTranslatef(sceneTriangle.Position.x, sceneTriangle.Position.y, sceneTriangle.Position.z);
            Gl.glRotatef(sceneTriangle.Rotation.x, 1.0f, 0.0f, 0.0f);
            Gl.glRotatef(sceneTriangle.Rotation.y, 0.0f, 1.0f, 0.0f);
            Gl.glRotatef(sceneTriangle.Rotation.z, 0.0f, 0.0f, 1.0f);
            Gl.glScalef(sceneTriangle.Scale.x, sceneTriangle.Scale.y, sceneTriangle.Scale.z);

            //Gl.glEnable(Gl.GL_TEXTURE_2D);

            // Specular Properties set before glBegin ()
            sceneMat = sceneTriangle.Materials[0];
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, MathUtils.GetVector3Array(sceneMat.Specular));
            Gl.glMaterialf(Gl.GL_FRONT, Gl.GL_SHININESS, sceneMat.Shininess);


            // Draw the Triangle and set materials for each vertex as well
            Gl.glBegin(Gl.GL_TRIANGLES);
            Gl.glColor3fv(MathUtils.GetVector3Array(sceneMat.Diffuse));
            Gl.glTexCoord2f(sceneTriangle.U[0], sceneTriangle.V[0]);
            Gl.glNormal3f(sceneTriangle.Normal[0].x, sceneTriangle.Normal[0].y,
                sceneTriangle.Normal[0].z);
            Gl.glVertex3f(sceneTriangle.Vertex[0].x, sceneTriangle.Vertex[0].y,
                sceneTriangle.Vertex[0].z);


            sceneMat = sceneTriangle.Materials[1];
            Gl.glColor3fv(MathUtils.GetVector3Array(sceneMat.Diffuse));
            Gl.glTexCoord2f(sceneTriangle.U[1], sceneTriangle.V[1]);
            Gl.glNormal3f(sceneTriangle.Normal[1].x, sceneTriangle.Normal[1].y,
                sceneTriangle.Normal[1].z);
            Gl.glVertex3f(sceneTriangle.Vertex[1].x, sceneTriangle.Vertex[1].y,
                sceneTriangle.Vertex[1].z);

            sceneMat = sceneTriangle.Materials[2];
            Gl.glColor3fv(MathUtils.GetVector3Array(sceneMat.Diffuse));
            Gl.glTexCoord2f(sceneTriangle.U[2], sceneTriangle.V[2]);
            Gl.glNormal3f(sceneTriangle.Normal[2].x, sceneTriangle.Normal[2].y,
                sceneTriangle.Normal[2].z);
            Gl.glVertex3f(sceneTriangle.Vertex[2].x, sceneTriangle.Vertex[2].y,
               sceneTriangle.Vertex[2].z);
            Gl.glEnd();

        }

        private void SetTextures(SceneMaterial sceneMat, bool bilinear)
        {
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            if (sceneMat.TextureImage != null)
            {
                sceneMat.CreatePointer();
                if (bilinear)
                {
                    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
                }
                else
                {
                    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
                    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
                }

                Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, 3, sceneMat.TextureImage.Width, sceneMat.TextureImage.Height, 0, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, sceneMat.PtrBitmap);
            }
            else { Gl.glDisable(Gl.GL_TEXTURE_2D); }
        }

		private void SetRenderStates ()
		{
			Gl.glEnable(Gl.GL_LIGHTING);
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			Gl.glShadeModel(Gl.GL_SMOOTH);
			Gl.glEnable(Gl.GL_LIGHT0);
			Gl.glEnable(Gl.GL_LIGHT1);
			Gl.glEnable(Gl.GL_LIGHT2);
			Gl.glEnable(Gl.GL_LIGHT3);
			Gl.glEnable(Gl.GL_LIGHT4);
			Gl.glEnable(Gl.GL_LIGHT5);
			Gl.glEnable(Gl.GL_LIGHT6);
			Gl.glEnable(Gl.GL_LIGHT7);
			Gl.glEnable(Gl.GL_COLOR_MATERIAL);
			Gl.glColorMaterial(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT_AND_DIFFUSE);
			Gl.glEnable(Gl.GL_NORMALIZE);
            Gl.glEnable(Gl.GL_TEXTURE_2D);
		}

		private void UnsetRenderStates ()
		{
			Gl.glDisable(Gl.GL_CULL_FACE);
			Gl.glDisable(Gl.GL_LIGHTING);
			Gl.glDisable(Gl.GL_LIGHT0);
			Gl.glDisable(Gl.GL_LIGHT1);
			Gl.glDisable(Gl.GL_LIGHT2);
			Gl.glDisable(Gl.GL_LIGHT3);
			Gl.glDisable(Gl.GL_LIGHT4);
			Gl.glDisable(Gl.GL_LIGHT5);
			Gl.glDisable(Gl.GL_LIGHT6);
			Gl.glDisable(Gl.GL_LIGHT7);
			Gl.glDisable(Gl.GL_COLOR_MATERIAL);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
		}
	}
}
