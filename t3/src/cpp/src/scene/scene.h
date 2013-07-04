#ifndef SCENE_H_
#define SCENE_H_

#include <GL/glew.h>
#include <vector>
#include "scene/camera.h"
#include "scene/light.h"

class Scene {
	private:
		std::vector<Camera> _cameras;
		std::vector<Light> _lights;
		GLuint _shaderProgramId;
		GLuint _lightsId;
		GLuint _numLightsId;
	public:
		Scene();
		Scene(float fov, float near, float far, glm::vec3 eye, glm::vec3 target, glm::vec3 up);
		Scene(Camera & cam);
		glm::mat4 projectionTransform(float aspectRatio, int camIndex = 0);
		glm::mat4 viewTransform(int camIndex = 0);
		void addLight(Light & light);
		void addLights(std::vector<Light> lights);
		void setShaderId(GLuint id);
		void generateIds();
		void bindUniforms();
};

#endif