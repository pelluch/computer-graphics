#ifndef CURVE_H_
#define CURVE_H_

#include <glm/glm.hpp>
#include <vector>

class Curve {
	virtual glm::vec3 next(float t) = 0;
private: 
	std::vector<glm::vec3> _points;
	float _minT;
	float _maxT;
};

#endif