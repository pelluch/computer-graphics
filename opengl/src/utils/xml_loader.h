#ifndef XML_LOADER_H_
#define XML_LOADER_H_

#include <tinyxml2.h>
#include "scene/scene.h"
#include "model/model.h"
#include "scene/camera.h"
#include "model/material.h"
#include "scene/light.h"
#include <string>
#include <map>

class XmlLoader 
{

public:
	static std::map<std::string, Material> loadMaterials(const tinyxml2::XMLElement * materialsElement);
	static glm::vec2 loadTextureCoords(const tinyxml2::XMLElement * vecElement);
	static glm::vec3 loadColor(const tinyxml2::XMLElement * vecElement);
	static glm::vec3 loadPosition(const tinyxml2::XMLElement * vecElement);
	static Scene loadScene(const std::string & xmlPath);
	static Camera loadCamera(const tinyxml2::XMLElement * element);
	static std::vector<Model> loadModels(const tinyxml2::XMLElement * element);
	static std::vector<Light> loadLights(const tinyxml2::XMLElement * element);
	static Model loadModel(const std::string & modelPath);
	static Model loadTriangle(const tinyxml2::XMLElement * element);
};

#endif