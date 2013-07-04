#ifndef LIGHT_H_
#define LIGHT_H_


#include <glm/glm.hpp>

class Light
{
	public:
		glm::vec3 _worldPosition;
		glm::vec3 _color;
		glm::vec3 _ambient;
		glm::vec3 _diffuse;
		glm::vec3 _specular;
		float _constantAttenuation;
		float _linearAttenuation;
		float _quadraticAttenuation;
		glm::vec3 _spotDirection;
		Light(glm::vec3 position, glm::vec3 color);
		Light();
};

#endif