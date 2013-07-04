#ifndef LIGHT_H_
#define LIGHT_H_


#include <glm/glm.hpp>

class Light
{
	public:
		glm::vec3 _worldPosition;
		glm::vec3 _color;
		Light(glm::vec3 position, glm::vec3 color);
};

#endif