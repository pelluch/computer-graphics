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
#include "renderer/renderer.h"

class Scene {
	private:
		std::vector<Light> _lights;
		std::map<std::string, Material> _materials;
		std::vector<glm::vec3> _ray;
		glm::vec3 _ambientLight;
		glm::vec3 _backgroundColor;
		GLuint _shaderProgramId;
		GLuint _lightsId;
		GLuint _numLightsId;
		GLuint _ambientLightId;
		GLuint _rayBufferId;
		GLuint _rayColorBufferId;
		bool _rayExists;
		int currentCam;
	public:
		std::vector<Model> _models;
		std::vector<Camera> _cameras;
		Scene(Camera & cam, std::vector<Model> & models, std::map<std::string, Material> & materials, std::vector<Light> & lights, glm::vec3 backgroundColor, glm::vec3 ambientLight);
		~Scene();
		glm::mat4 projectionTransform(float aspectRatio, int camIndex = 0);
		glm::mat4 viewTransform(int camIndex = 0);
		void addLight(Light & light);
		void addLights(std::vector<Light> lights);
		void setShaderId(GLuint id);
		void drawRay(glm::vec3 start, glm::vec3 end);
		void generateIds();
		void generateLineIds();
		void bindUniforms();
		void initModelData();
		void draw(GLuint shaderProgramId, Renderer & renderer);
		void drawBoundingBoxes(GLuint shaderProgramId, Renderer & renderer);
		void setMaterials();
		void moveCamera(glm::vec3 translation, glm::vec3 rotation);
};

#endif