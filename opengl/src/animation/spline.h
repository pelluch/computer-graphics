#ifndef SPLINE_H_
#define SPLINE_H_

#include <vector>
#include <glm/glm.hpp>

class Spline {

public:
	Spline();
	std::vector<glm::vec3> splinePoints;
	const glm::mat4 basis = glm::mat4(0, 2, 0, 0,
		-1, 0, 1, 0,
		2, -5, 4, -1,
		-1, 3, -3, 1);
	float currentT;
	int currentIndex = 0;
	void generateSpline(glm::vec3 center, float radius);
	glm::vec3 evaluate(float t);

};

#endif