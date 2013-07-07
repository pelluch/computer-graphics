#ifndef DEBUG_UTILS_H_
#define DEBUG_UTILS_H_

#include <glm/glm.hpp>
#include "scene/camera.h"
#include "scene/scene.h"
#include "scene/light.h"
#include "model/model.h"
#include "model/material.h"

class Debugger
{
public:
	static void printInfo(Camera & cam);
	static void printInfo(Scene & scene);
	static void printInfo(Material & material);
	static void printInfo(Light & light);
	static void printInfo(Model & model);
	static void printInfo(glm::vec2 vec);
	static void printInfo(glm::vec3 vec);
	static void printInfo(glm::vec4 vec);
	static void printInfo(glm::mat4 mat);
};

#endif