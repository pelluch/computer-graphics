#include "animation/spline.h"
#include "utils/debugutils.h"
#include <iostream>

Spline::Spline()
{

}

void Spline::generateSpline(glm::vec3 center, float radius)
{
	radius = 1000.0f;
	float angle = 0;
	float deltaAngle = 2.0f*3.14159265359f/100.0f;
	
 	for(int i = 0; i < 100; ++i)
 	{
 		glm::vec3 currentPoint = glm::vec3(center[0] + radius*cos(angle), center[1], center[2] + radius*sin(angle));
 		Debugger::printInfo(currentPoint);
 		splinePoints.push_back(currentPoint);
 		angle += deltaAngle;
 	}

 	currentT = 0;
 	currentIndex = 0;
}

glm::vec3 Spline::evaluate(float deltaT)
{
	std::cout << deltaT << std::endl;
	if(currentT > 1.0f)
	{
		currentT = 0;
		currentIndex = (currentIndex + 1)%splinePoints.size();
	}
	glm::vec3 curvePoint = 0.5f * ((2.0f * splinePoints[currentIndex+1]) +
	 	(-splinePoints[currentIndex] + splinePoints[currentIndex+2]) * currentT +
	(2.0f*splinePoints[currentIndex] - 5.0f*splinePoints[currentIndex+1] + 4.0f*splinePoints[currentIndex+2] - 
		splinePoints[currentIndex+3]) * currentT*currentT +
	(-splinePoints[currentIndex] + 3.0f*splinePoints[currentIndex+1] - 3.0f*splinePoints[currentIndex+2] +
	 splinePoints[currentIndex+3]) * currentT* currentT* currentT);
	Debugger::printInfo(curvePoint);
	currentT += deltaT;
	return curvePoint;

	
}