
#include "scene/light.h"

Light::Light(glm::vec3 position, glm::vec3 color)
{
	this->_color = color;
	this->_worldPosition = position;

}
