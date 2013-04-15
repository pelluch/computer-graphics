using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Globalization;
using System.IO;
using System.Drawing;
using SceneLib.SceneObjects;

namespace SceneLib
{
    /// <summary>
    /// Represents a Scene to be rendered
    /// </summary>
    public class Scene
    {

        public CultureInfo CultureInfo { get; set; } //Used to read XML scene file

        private int width, height;
        private SceneBackground background;
        private Dictionary<string, SceneMaterial> materialsTable; //Assocaites names to materials
        private List<SceneObject> objects; //list of objects in scene
        private List<SceneLight> lights; //list of lights
        private List<SceneCamera> cameras; //list of cameras
        private int currentCamera = 0;

        public List<SceneObject> Objects { get { return objects; } }
        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public SceneCamera Camera { get { return cameras[currentCamera]; } }  //First camera
        public SceneBackground Background { get { return background; } }
        public List<SceneLight> Lights { get { return lights; } }
        public List<SceneCamera> Cameras
        {
            get { return cameras; }
            set { cameras = value; }
        }

        public Matrix Transform { get; set; }

        public RenderingParameters RenderParams { get; set; }

        public Scene(int width, int height)
        {
            this.width = width;
            this.height = height;
            materialsTable = new Dictionary<string, SceneMaterial>();
            objects = new List<SceneObject>();
            lights = new List<SceneLight>();
            cameras = new List<SceneCamera>();
            background = new SceneBackground();
            RenderParams = new RenderingParameters();

            CultureInfo = new CultureInfo("en-US");
        }

        public void NextCamera()
        {
            currentCamera = (currentCamera + 1) % cameras.Count;
        }

        //Adds a new object to the scene
        public void AddObject(SceneObject obj)
        {
            objects.Add(obj);
        }

        //Gets a material based on a name
        public SceneMaterial GetMaterial(string name)
        {
            if (materialsTable.ContainsKey(name))
                return materialsTable[name];
            return null;
        }

        //Loads a scene from an XML file
        public void Load(string file)
        {
            XDocument xmlDoc = XDocument.Load(file);
            XElement xmlScene = xmlDoc.Elements("scene").First();

            //Loading background
            XElement xmlBackground = xmlScene.Elements("background").First();
            background.AmbientLight = LoadColor(xmlBackground.Elements("ambientLight").First());
            background.Color = LoadColor(xmlBackground.Elements("color").First());

            //Loading camera
            XElement xmlCamera = xmlScene.Elements("camera").FirstOrDefault();
            if (xmlCamera != null)
            {
                SceneCamera camera = new SceneCamera();
                camera.FieldOfView = LoadFloat(xmlCamera, "fieldOfView");
                camera.NearClip = LoadFloat(xmlCamera, "nearClip");
                camera.FarClip = LoadFloat(xmlCamera, "farClip");
                camera.Position = LoadXYZ(xmlCamera.Elements("position").First());
                camera.Target = LoadXYZ(xmlCamera.Elements("target").First());
                camera.Up = LoadXYZ(xmlCamera.Elements("up").First());
                cameras.Add(camera);
            }

            XElement xmlCameras = xmlScene.Elements("camera_list").FirstOrDefault();
            if (xmlCameras != null)
            {
                foreach (XElement cameraNode in xmlCameras.Elements("camera"))
                {
                    SceneCamera camera = new SceneCamera();
                    camera.FieldOfView = LoadFloat(cameraNode, "fieldOfView");
                    camera.NearClip = LoadFloat(cameraNode, "nearClip");
                    camera.FarClip = LoadFloat(cameraNode, "farClip");
                    camera.Position = LoadXYZ(cameraNode.Elements("position").First());
                    camera.Target = LoadXYZ(cameraNode.Elements("target").First());
                    camera.Up = LoadXYZ(cameraNode.Elements("up").First());
                    cameras.Add(camera);
                }
            }

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
                material.Name = name;
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
                if (materialNode.Elements("refractiveness").Any())
                    material.Refractiveness = LoadColor(materialNode.Elements("refractiveness").First());
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
                sphere.Velocity = LoadXYZ(sphereNode.Elements("velocity").FirstOrDefault());
                objects.Add(sphere);
            }

            foreach (XElement cylinderNode in xmlObjects.Elements("cylinder"))
            {
                SceneCylinder cylinder = new SceneCylinder();
                cylinder.Radius = LoadFloat(cylinderNode, "radius");
                cylinder.Material = materialsTable[cylinderNode.Attribute("material").Value];
                cylinder.Scale = LoadXYZ(cylinderNode.Elements("scale").First());
                cylinder.Position = LoadXYZ(cylinderNode.Elements("position").First());
                cylinder.Rotation = LoadXYZ(cylinderNode.Elements("rotation").First());
                cylinder.BasePoint = LoadXYZ(cylinderNode.Elements("base").First());
                cylinder.EndPoint = LoadXYZ(cylinderNode.Elements("end").First());
                //cylinder.Velocity = LoadXYZ(cylinderNode.Elements("velocity").FirstOrDefault());
                objects.Add(cylinder);
            }

            foreach (XElement planeNode in xmlObjects.Elements("plane"))
            {
                Plane plane = new Plane();
                plane.Material = materialsTable[planeNode.Attribute("material").Value];
                plane.Height = LoadFloat(planeNode, "height");
                plane.Width = LoadFloat(planeNode, "width");
                plane.Scale = LoadXYZ(planeNode.Elements("scale").First());
                plane.Position = LoadXYZ(planeNode.Elements("position").First());
                plane.Rotation = LoadXYZ(planeNode.Elements("rotation").First());
                plane.Center = LoadXYZ(planeNode.Elements("center").First());
                plane.L1 = LoadXYZ(planeNode.Elements("l1").First());
                plane.L2 = LoadXYZ(planeNode.Elements("l2").First());
                plane.Initialize();
                objects.Add(plane);
            }

            foreach (XElement tableNode in xmlObjects.Elements("table"))
            {
                SceneTable table = new SceneTable();
                table.Material = materialsTable[tableNode.Attribute("material").Value];
                table.Height = LoadFloat(tableNode, "height");
                table.Width = LoadFloat(tableNode, "width");
                table.Scale = LoadXYZ(tableNode.Elements("scale").First());
                table.Position = LoadXYZ(tableNode.Elements("position").First());
                table.Rotation = LoadXYZ(tableNode.Elements("rotation").First());
                table.Radius = LoadFloat(tableNode, "radius");
                XElement planeNode = tableNode.Elements("plane").First();
                Plane plane = new Plane();
                plane.Material = table.Material;
                plane.Height = LoadFloat(planeNode, "height");
                plane.Width = LoadFloat(planeNode, "width");
                plane.Scale = LoadXYZ(planeNode.Elements("scale").First());
                plane.Position = LoadXYZ(planeNode.Elements("position").First());
                plane.Rotation = LoadXYZ(planeNode.Elements("rotation").First());
                plane.Center = LoadXYZ(planeNode.Elements("center").First());
                plane.L1 = LoadXYZ(planeNode.Elements("l1").First());
                plane.L2 = LoadXYZ(planeNode.Elements("l2").First());
                plane.Initialize();
                table.Initialize(plane);

                objects.Add(table);
            }
        }

        #region LoadHelpers
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
            if (node == null)
                return new Vector();
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
        #endregion
    }
}
