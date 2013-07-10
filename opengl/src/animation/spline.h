#ifndef SPLINE_H_
#define SPLINE_H_

#include <vector>
#include <glm/glm.hpp>

class Spline {

private:
	glm::mat4 _basis;
	std::vector<glm::vec3> _splinePoints;
	float _currentT;
	int _currentIndex;
public:
	Spline();
	std::vector<glm::vec3> splinePoints;
	void generateSpline(glm::vec3 center, float radius);
	glm::vec3 evaluate(float t);

};

#endif