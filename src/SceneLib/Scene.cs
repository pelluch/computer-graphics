using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Globalization;
using System.IO;
using System.Drawing;

namespace SceneLib
{
    public class Scene
    {
        public CultureInfo CultureInfo { get; set; }

        private int width, height;
        private SceneBackground background;
        private Dictionary<string, SceneMaterial> materialsTable;
        private List<SceneObject> objects;
        private List<SceneLight> lights;
        private SceneCamera camera;       

        public List<SceneObject> Objects { get { return objects; } }
        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public SceneCamera Camera { get { return camera; } }
        public SceneBackground Background { get { return background; } }
        public List<SceneLight> Lights { get { return lights; } }

        public Matrix Transform { get; set; }


        public Scene(int width, int height)
        {
            this.width = width;
            this.height = height;
            materialsTable = new Dictionary<string, SceneMaterial>();
            objects = new List<SceneObject>();
            lights = new List<SceneLight>();
            background = new SceneBackground();
            camera = new SceneCamera();

            CultureInfo = new CultureInfo("en-US");
        }

        public void AddObject(SceneObject obj)
        {
            objects.Add(obj);
        }

        public SceneMaterial GetMaterial(string name)
        {
            if (materialsTable.ContainsKey(name))
                return materialsTable[name];
            return null;
        }

        public void Load(string file)
        {
            XDocument xmlDoc = XDocument.Load(file);
            XElement xmlScene = xmlDoc.Elements("scene").First();

            //Loading background
            XElement xmlBackground = xmlScene.Elements("background").First();
            background.AmbientLight = LoadColor(xmlBackground.Elements("ambientLight").First());
            background.Color = LoadColor(xmlBackground.Elements("color").First());

            //Loading camera
            XElement xmlCamera = xmlScene.Elements("camera").First();
            camera.FieldOfView = LoadFloat(xmlCamera, "fieldOfView");
            camera.NearClip = LoadFloat(xmlCamera, "nearClip");
            camera.FarClip = LoadFloat(xmlCamera, "farClip");
            camera.Position = LoadXYZ(xmlCamera.Elements("position").First());
            camera.Target = LoadXYZ(xmlCamera.Elements("target").First());
            camera.Up = LoadXYZ(xmlCamera.Elements("up").First());

            XElement xmlLights = xmlScene.Elements("light_list").First();
            foreach (XElement lightNode in xmlLights.Elements("light"))
            {
                SceneLight lightObj = new SceneLight();
                lightObj.Position = LoadXYZ(lightNode.Elements("position").First());
                if (lightNode.Elements("a").Any())
                    lightObj.A = LoadXYZ(lightNode.Elements("a").First());
                if (lightNode.Elements("b").Any())
                    lightObj.B = LoadXYZ(lightNode.Elements("b").First());
                lightObj.Color = LoadColor(lightNode.Elements("color").First());
                XElement atenuationNode = lightNode.Elements("attenuation").First();
                lightObj.AtenuationConstant = LoadFloat(atenuationNode, "constant");
                lightObj.AtenuationLinear = LoadFloat(atenuationNode, "linear");
                lightObj.AtenuationQuadratic = LoadFloat(atenuationNode, "quadratic");

                lights.Add(lightObj);
            }

            XElement xmlMaterials = xmlScene.Elements("material_list").First();
            foreach (XElement materialNode in xmlMaterials.Elements("material"))
            {
                string name = materialNode.Attribute("name").Value;
                SceneMaterial material = new SceneMaterial();
                if (materialNode.Elements("texture").Any())
                    material.TextureFile = materialNode.Elements("texture").First().Attribute("filename").Value;
                if (material.TextureFile != null && material.TextureFile != String.Empty && File.Exists(material.TextureFile))
                    material.TextureImage = (Bitmap)Bitmap.FromFile(material.TextureFile);
                if (materialNode.Elements("bumpmap").Any())
                    material.BumpFile = materialNode.Elements("bumpmap").First().Attribute("filename").Value;
                if (material.BumpFile != null && material.BumpFile != String.Empty && File.Exists(material.BumpFile))
                    material.BumpImage = (Bitmap)Bitmap.FromFile(material.BumpFile);
                if (materialNode.Elements("normalmap").Any())
                    material.NormalFile = materialNode.Elements("normalmap").First().Attribute("filename").Value;
                if (material.NormalFile != null && material.NormalFile != String.Empty && File.Exists(material.NormalFile))
                    material.NormalImage = (Bitmap)Bitmap.FromFile(material.NormalFile);
                if (materialNode.Elements("specular").Any())
                    material.Specular = LoadSpecular(materialNode.Elements("specular").First());
                if (materialNode.Elements("diffuse").Any())
                    material.Diffuse = LoadColor(materialNode.Elements("diffuse").First());
                if (materialNode.Elements("transparent").Any())
                    material.Transparent = LoadColor(materialNode.Elements("transparent").First());
                if (materialNode.Elements("reflective").Any())
                    material.Reflective = LoadColor(materialNode.Elements("reflective").First());
                if (materialNode.Elements("refraction_index").Any())
                    material.RefractionIndex = LoadColor(materialNode.Elements("refraction_index").First());
                materialsTable.Add(name, material);
            }

            XElement xmlObjects = xmlScene.Elements("object_list").First();

            foreach (XElement modelNode in xmlObjects.Elements("model"))
            {
                SceneModel model = new SceneModel(modelNode.Attribute("name").Value, modelNode.Attribute("path").Value);
                XDocument xmlModel = XDocument.Load(model.FileName);
                XElement xmlSceneModel = xmlModel.Elements("model").First();

                model.Scale = LoadXYZ(modelNode.Elements("scale").First());
                model.Position = LoadXYZ(modelNode.Elements("position").First());
                model.Rotation = LoadXYZ(modelNode.Elements("rotation").First());

                XElement xmlTriangles = xmlSceneModel.Elements("triangle_list").First();

                foreach (XElement triangleNode in xmlTriangles.Elements("triangle"))
                {
                    SceneTriangle triangle = new SceneTriangle();
                    triangle.Scale = LoadXYZ(triangleNode.Elements("scale").First());
                    triangle.Position = LoadXYZ(triangleNode.Elements("position").First());
                    triangle.Rotation = LoadXYZ(triangleNode.Elements("rotation").First());

                    foreach (XElement vertexNode in triangleNode.Elements("vertex"))
                    {
                        triangle.Materials.Add(materialsTable[vertexNode.Attribute("material").Value]);
                        triangle.Vertex.Add(LoadXYZ(vertexNode.Elements("position").First()));
                        triangle.Normal.Add(LoadXYZ(vertexNode.Elements("normal").First()));
                        XElement textNode = vertexNode.Elements("texture").First();
                        triangle.U.Add(LoadFloat(textNode, "u"));
                        triangle.V.Add(LoadFloat(textNode, "v"));
                    }
                    model.Triangles.Add(triangle);
                }

                objects.Add(model);
            }

            foreach (XElement triangleNode in xmlObjects.Elements("triangle"))
            {
                SceneTriangle triangle = new SceneTriangle();
                triangle.Scale = LoadXYZ(triangleNode.Elements("scale").First());
                triangle.Position = LoadXYZ(triangleNode.Elements("position").First());
                triangle.Rotation = LoadXYZ(triangleNode.Elements("rotation").First());

                foreach (XElement vertexNode in triangleNode.Elements("vertex"))
                {
                    triangle.Materials.Add(materialsTable[vertexNode.Attribute("material").Value]);
                    triangle.Vertex.Add(LoadXYZ(vertexNode.Elements("position").First()));
                    triangle.Normal.Add(LoadXYZ(vertexNode.Elements("normal").First()));
                    XElement textNode = vertexNode.Elements("texture").First();
                    triangle.U.Add(LoadFloat(textNode, "u"));
                    triangle.V.Add(LoadFloat(textNode, "v"));
                }
                objects.Add(triangle);
            }

            foreach (XElement sphereNode in xmlObjects.Elements("sphere"))
            {
                SceneSphere sphere = new SceneSphere();
                sphere.Radius = LoadFloat(sphereNode, "radius");
                sphere.Material = materialsTable[sphereNode.Attribute("material").Value];
                sphere.Scale = LoadXYZ(sphereNode.Elements("scale").First());
                sphere.Position = LoadXYZ(sphereNode.Elements("position").First());
                sphere.Rotation = LoadXYZ(sphereNode.Elements("rotation").First());
                sphere.Center = LoadXYZ(sphereNode.Elements("center").First());
                sphere.Speed = LoadXYZ(sphereNode.Elements("speed").First());
                objects.Add(sphere);
            }

            foreach (XElement sphereNode in xmlObjects.Elements("lightsphere"))
            {
                LightSphere sphere = new LightSphere();
                sphere.Radius = LoadFloat(sphereNode, "radius");
                sphere.Material = materialsTable[sphereNode.Attribute("material").Value];
                sphere.Scale = LoadXYZ(sphereNode.Elements("scale").First());
                sphere.Position = LoadXYZ(sphereNode.Elements("position").First());
                sphere.Rotation = LoadXYZ(sphereNode.Elements("rotation").First());
                sphere.Center = LoadXYZ(sphereNode.Elements("center").First());
                objects.Add(sphere);
            }

            foreach (XElement sphereNode in xmlObjects.Elements("cylinder"))
            {
                SceneCylinder cylinder = new SceneCylinder();
                cylinder.Radius = LoadFloat(sphereNode, "radius");
                cylinder.Height = LoadFloat(sphereNode, "height");
                cylinder.Material = materialsTable[sphereNode.Attribute("material").Value];
                cylinder.Scale = LoadXYZ(sphereNode.Elements("scale").First());
                cylinder.Position = LoadXYZ(sphereNode.Elements("position").First());
                cylinder.Rotation = LoadXYZ(sphereNode.Elements("rotation").First());
                cylinder.BasePoint = LoadXYZ(sphereNode.Elements("base").First());
                cylinder.HeightDirection = LoadXYZ(sphereNode.Elements("axis").First());
                objects.Add(cylinder);
            }
        }

        private float LoadFloat(XElement node, string attribute)
        {
            if (node == null)
                return 0;
            float value = 0;
            float.TryParse(node.Attribute(attribute).Value, NumberStyles.Number, CultureInfo, out value);
            return value;
        }

        private Vector LoadXYZ(XElement node)
        {
            return new Vector(LoadFloat(node, "x"), LoadFloat(node, "y"), LoadFloat(node, "z"));
        }

        private Vector LoadColor(XElement node)
        {
            return new Vector(LoadFloat(node, "red"), LoadFloat(node, "green"), LoadFloat(node, "blue"));
        }

        private Vector LoadSpecular(XElement node)
        {
            return new Vector(LoadFloat(node, "red"), LoadFloat(node, "green"), LoadFloat(node, "blue"), LoadFloat(node, "shininess"));
        }
    }
}
