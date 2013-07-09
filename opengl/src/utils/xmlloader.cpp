#include "utils/xmlloader.h"
#include <map>
#include <iostream>
#include <glm/glm.hpp>
#include "utils/debugutils.h"
#include <string>
#include "utils/objloader.h"

using namespace tinyxml2;


glm::vec3 XmlLoader::loadPosition(const tinyxml2::XMLElement * vecElement)
{
	if(!vecElement) return glm::vec3(0, 0, 0);

	float x = vecElement->FloatAttribute("x");
	float y = vecElement->FloatAttribute("y");
	float z = vecElement->FloatAttribute("z");
	glm::vec3 color(x, y, z);
	return color;
}

glm::vec3 XmlLoader::loadColor(const tinyxml2::XMLElement * vecElement)
{
	if(!vecElement) return glm::vec3(0, 0, 0);

	float x = vecElement->FloatAttribute("red");
	float y = vecElement->FloatAttribute("green");
	float z = vecElement->FloatAttribute("blue");
	glm::vec3 color(x, y, z);
	return color;
}

Camera XmlLoader::loadCamera(const tinyxml2::XMLElement *camElement)
{

	glm::vec3 eye = loadPosition(camElement->FirstChildElement("position"));
	glm::vec3 target = loadPosition(camElement->FirstChildElement("target"));
	glm::vec3 up = loadPosition(camElement->FirstChildElement("up"));

	float fov = camElement->FloatAttribute("fieldOfView");
	float near = camElement->FloatAttribute("nearClip");
	float far = camElement->FloatAttribute("farClip");

	Camera cam(fov, near, far, eye, target, up);
	return cam;
}

std::map<std::string, Material> XmlLoader::loadMaterials(const tinyxml2::XMLElement * materialsElement)
{
	std::map<std::string, Material> materials;
	std::cout << "Iterating " << std::endl;
	for(const XMLElement * materialElement = materialsElement->FirstChildElement("material");
		materialElement; materialElement = materialElement->NextSiblingElement())
	{
		Material mat;
		const std::string materialName = materialElement->Attribute("name");
		const XMLElement * normalElement = materialElement->FirstChildElement("normalmap");
		if(normalElement) mat._normalTexturePath = normalElement->Attribute("filename");

		const XMLElement * textureElement = materialElement->FirstChildElement("texture");
		if(textureElement) mat._texturePath = textureElement->Attribute("filename");

		mat._diffuseColor = loadColor(materialElement->FirstChildElement("diffuse"));
		mat._reflectiveColor = loadColor(materialElement->FirstChildElement("reflective"));
		mat._refractiveColor = loadColor(materialElement->FirstChildElement("refraction_index"));
		const XMLElement * specularElement = materialElement->FirstChildElement("specular");
		if(specularElement)
		{
			glm::vec3 specularColor = loadColor(specularElement);
			mat._shininess = specularElement->FloatAttribute("shininess");
			mat._specularColor = glm::vec4(specularColor[0], specularColor[1], specularColor[2],
				mat._shininess);
			
		}
		materials[materialName] = mat;
		//Debugger::printInfo(mat);
	}

	return materials;
}

glm::vec2 XmlLoader::loadTextureCoords(const tinyxml2::XMLElement *vecElement)
{
	float u = vecElement->FloatAttribute("u");
	float v = vecElement->FloatAttribute("v");
	glm::vec2 uv(u, v);
	return uv;
}

Model XmlLoader::loadModel(const std::string & modelPath)
{
	std::cout << "Attempting to load model " << modelPath << std::endl;
	Model model;

	if(modelPath.find(".xml") != std::string::npos)
	{
		XMLDocument modelDoc;
		modelDoc.LoadFile(modelPath.c_str());
		XMLElement * rootElement = modelDoc.RootElement();
		//const std::string materialName = rootElement->Attribute("material");
		XMLElement * triangleListElement = rootElement->FirstChildElement("triangle_list");
		for(XMLElement * triangleElement = triangleListElement->FirstChildElement("triangle");
			triangleElement; triangleElement = triangleElement->NextSiblingElement())
		{
			for(XMLElement * vertexElement = triangleElement->FirstChildElement("vertex");
				vertexElement; vertexElement = vertexElement->NextSiblingElement())
			{
				glm::vec3 vertexPosition = loadPosition(vertexElement->FirstChildElement("position"));
				glm::vec3 vertexNormal = loadPosition(vertexElement->FirstChildElement("normal"));
				glm::vec2 textureUv = loadTextureCoords(vertexElement->FirstChildElement("texture"));
				model._normals.push_back(vertexNormal);
				model._vertices.push_back(vertexPosition);
				model._uvs.push_back(textureUv);
			}
		}
		//model._materialName = materialName;
	}
	else if(modelPath.find(".obj") != std::string::npos)
	{
		std::vector<glm::vec3> vertices;
		std::vector<glm::vec2> uvs;
		std::vector<glm::vec3> normals;
		bool success = loadOBJ(modelPath.c_str(), vertices, uvs, normals);
		if(!success) std::cerr << "Error loading model" << modelPath << std::endl;

		model._normals = normals;
		model._vertices = vertices;
		model._uvs = uvs;
		//Debugger::printInfo(model);
	}
	else
	{
		std::cerr << "Model with invalid extension: " << modelPath << std::endl;
	}

	

	return model;
}

Model XmlLoader::loadTriangle(const tinyxml2::XMLElement * triangleElement)
{
	Model model;
	model._scale = glm::vec3(1, 1, 1);
	model._modelName = triangleElement->Attribute("name");
	for(const XMLElement * vertexElement = triangleElement->FirstChildElement("vertex");
			vertexElement; vertexElement = vertexElement->NextSiblingElement())
	{
		const std::string materialName = vertexElement->Attribute("material");
		model._materialName = materialName;
		glm::vec3 vertexPosition = loadPosition(vertexElement->FirstChildElement("position"));
		glm::vec3 vertexNormal = loadPosition(vertexElement->FirstChildElement("normal"));
		glm::vec2 textureUv = loadTextureCoords(vertexElement->FirstChildElement("texture"));
		model._normals.push_back(vertexNormal);
		model._vertices.push_back(vertexPosition);
		model._uvs.push_back(textureUv);
	}

	return model;

}

std::vector<Model> XmlLoader::loadModels(const tinyxml2::XMLElement * objectListElement)
{
	std::vector<Model> models;

	for(const XMLElement * objectElement = objectListElement->FirstChildElement();
		objectElement; objectElement = objectElement->NextSiblingElement())
	{
		const std::string elementName = objectElement->Name();
		if(elementName == "model")
		{
			const std::string modelPath = objectElement->Attribute("path");
			glm::vec3 position = XmlLoader::loadPosition(objectElement->FirstChildElement("position"));
			glm::vec3 scale = XmlLoader::loadPosition(objectElement->FirstChildElement("scale"));
			glm::vec3 rotation = XmlLoader::loadPosition(objectElement->FirstChildElement("rotation"));
			std::string matName = objectElement->Attribute("material");

			Model model = loadModel(modelPath);
			model._modelName = objectElement->Attribute("name");
			model._worldPosition = position;
			model._scale = scale;
			model._worldRotation = rotation;
			model._materialName = matName;

			models.push_back(model);
			//Debugger::printInfo(model);
		}
		else if(elementName == "triangle")
		{
			Model model = loadTriangle(objectElement);
			//Debugger::printInfo(model);
			models.push_back(model);
		}
	}

	return models;
}

std::vector<Light> XmlLoader::loadLights(const tinyxml2::XMLElement * lightListElement)
{
	std::vector<Light> lights;
	for(const XMLElement * lightElement = lightListElement->FirstChildElement("light");
		lightElement; lightElement = lightElement->NextSiblingElement())
	{
		Light light;
		glm::vec3 lightColor = loadColor(lightElement->FirstChildElement("color"));
		glm::vec3 position = loadPosition(lightElement->FirstChildElement("position"));
		const XMLElement * attenuationElement = lightElement->FirstChildElement("attenuation");
		float constantAttenuation = attenuationElement->FloatAttribute("constant");
		float linearAttenuation = attenuationElement->FloatAttribute("linear");
		float quadraticAttenuation = attenuationElement->FloatAttribute("quadratic");

		light._constantAttenuation = constantAttenuation;
		light._linearAttenuation = linearAttenuation;
		light._quadraticAttenuation = quadraticAttenuation;
		light._color = lightColor;
		light._worldPosition = position;

		lights.push_back(light);
		//Debugger::printInfo(light);
	}
	return lights;
}
/*
 <material name="Green">
      <texture filename=""/>
      <diffuse red="0.156" green="0.426" blue="0.107"/>
      <specular red="0.0" green="0.0" blue="0.0" shininess="1.0"/>
      <!--<reflective red="1" green="1" blue="1"/>-->
    </material>

    <material name="Translucent">
      <texture filename=""/>
      <diffuse red="0.7" green="0.7" blue="0.9"/>
      <specular red="0.0" green="0.0" blue="0.0" shininess="1.0"/>
      <refraction_index red="1.03" green="1.03" blue="1.03"/>
   </material>
*/
Scene * XmlLoader::loadScene(const std::string &xmlPath)
{
	Camera cam;

	std::map<std::string, Material> materials;
	std::vector<Model> models;
	std::vector<Light> lights;

	glm::vec3 ambientLight, backgroundColor;
	XMLDocument modelDoc;
	int loaded = modelDoc.LoadFile(xmlPath.c_str());
	if(loaded != XML_NO_ERROR)
		std::cerr << "Could not load file " << xmlPath << std::endl;

	std::cout << "Getting main elements" << std::endl;
	XMLElement * rootElement = modelDoc.RootElement();
	XMLElement * cameraElement = rootElement->FirstChildElement("camera");
	XMLElement * backgroundElement = rootElement->FirstChildElement("background");
	XMLElement * lightListElement = rootElement->FirstChildElement("light_list");
	XMLElement * materialListElement = rootElement->FirstChildElement("material_list");
	XMLElement * objectListElement = rootElement->FirstChildElement("object_list");

	std::cout << "Loading background information" << std::endl;
	//Parse background information
	ambientLight = loadColor(backgroundElement->FirstChildElement("ambientLight"));
	//Debugger::printInfo(ambientLight);
	backgroundColor = loadColor(backgroundElement->FirstChildElement("color"));
	//Debugger::printInfo(backgroundColor);
	//Parse camera information
	std::cout << "Parsing camera information" << std::endl;
	cam = loadCamera(cameraElement);
	//Debugger::printInfo(cam);
	std::cout << "Parsing materials information" << std::endl;
	materials = loadMaterials(materialListElement);
	std::cout << "Parsing object list" << std::endl;
	models = loadModels(objectListElement);

	std::cout << "Parsing lights information" << std::endl;
	lights = loadLights(lightListElement);

	Scene * scene = new Scene(cam, models, materials, lights, backgroundColor, ambientLight);
	return scene;
}