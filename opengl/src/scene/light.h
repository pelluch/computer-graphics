#ifndef LIGHT_H_
#define LIGHT_H_


#include <glm/glm.hpp>

class Light
{
	public:
		glm::vec3 _worldPosition;
		glm::vec3 _color;
		float _constantAttenuation;
		float _linearAttenuation;
		float _quadraticAttenuation;
		glm::vec3 _spotDirection;
		Light(glm::vec3 position, glm::vec3 color);
		Light();
};

#endif