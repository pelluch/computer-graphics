#ifndef SCENE_H_
#define SCENE_H_

#include <GL/glew.h>
#include <vector>
#include <glm/glm.hpp>
#include <map>
#include "scene/camera.h"
#include "scene/light.h"
#include "model/model.h"
#include "model/material.h"

class Scene {
	private:
		std::vector<Camera> _cameras;
		std::vector<Light> _lights;
		std::vector<Model> _models;
		std::map<std::string, Material> _materials;
		glm::vec3 _ambientLight;
		glm::vec3 _backgroundColor;
		GLuint _shaderProgramId;
		GLuint _lightsId;
		GLuint _numLightsId;
	public:
		Scene();
		Scene(float fov, float near, float far, glm::vec3 eye, glm::vec3 target, glm::vec3 up);
		Scene(Camera & cam);
		Scene(Camera & cam, std::vector<Model> & models, std::map<std::string, Material> & materials, std::vector<Light> & lights, glm::vec3 backgroundColor, glm::vec3 ambientLight);
		glm::mat4 projectionTransform(float aspectRatio, int camIndex = 0);
		glm::mat4 viewTransform(int camIndex = 0);
		void addLight(Light & light);
		void addLights(std::vector<Light> lights);
		void setShaderId(GLuint id);
		void generateIds();
		void bindUniforms();
		void initModelData();
		void draw(GLuint shaderProgramId);
		void setMaterials();
};

#endif