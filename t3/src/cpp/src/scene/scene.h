#ifndef SCENE_H_
#define SCENE_H_

#include <vector>
#include "camera.h"

class Scene {
	private:
		std::vector<Camera> _cameras;
	public:
		Scene();
		Scene(float fov, float near, float far, glm::vec3 eye, glm::vec3 target, glm::vec3 up);
		Scene(Camera & cam);
		glm::mat4 projectionTransform(float aspectRatio, int camIndex = 0);
		glm::mat4 viewTransform(int camIndex = 0);
};

#endif