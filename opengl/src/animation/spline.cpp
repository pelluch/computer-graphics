#include "animation/spline.h"
#include "utils/debugutils.h"
#include <iostream>

Spline::Spline()
{
	_currentT = 0;
	_currentIndex = 0;

	_basis =  glm::mat4(0, 2, 0, 0,
		-1, 0, 1, 0,
		2, -5, 4, -1,
		-1, 3, -3, 1);
}

void Spline::generateSpline(glm::vec3 center, float radius)
{
	radius = 1000.0f;
	float angle = 0;
	float deltaAngle = 2.0f*3.14159265359f/100.0f;
	float goal = 2000.0f;
 	for(int i = 0; i < 200; ++i)
 	{
 		glm::vec3 currentPoint = glm::vec3(center[0] + radius*cos(angle), center[1] + (float)i*goal/200.0f , center[2] + radius*sin(angle));
 		//Debugger::printInfo(currentPoint);
 		splinePoints.push_back(currentPoint);
 		angle += deltaAngle;
 	}
 	for(int i = 199; i >= 0; --i)
 	{
 		glm::vec3 currentPoint = glm::vec3(center[0] + radius*cos(angle), center[1] + (float)i*goal/200.0f , center[2] + radius*sin(angle));
 		//Debugger::printInfo(currentPoint);
 		splinePoints.push_back(currentPoint);
 		angle += deltaAngle;
 	}


}

glm::vec3 Spline::evaluate(float deltaT)
{
	//std::cout << deltaT << std::endl;
	while(_currentT > 1.0f)
	{
		_currentT -= 1.0f;
		_currentIndex = (_currentIndex + 1)%splinePoints.size();
	}
	int numPoints = splinePoints.size();
	glm::vec3 curvePoint = 0.5f * ((2.0f * splinePoints[(_currentIndex+1) % numPoints]) +
	 	(-splinePoints[_currentIndex % numPoints] + splinePoints[(_currentIndex+2) % numPoints]) * _currentT +
	(2.0f*splinePoints[_currentIndex % numPoints] - 5.0f*splinePoints[(_currentIndex+1) % numPoints] + 4.0f*splinePoints[(_currentIndex+2) % numPoints] - 
		splinePoints[(_currentIndex+3) % numPoints]) * _currentT*_currentT +
	(-splinePoints[_currentIndex % numPoints] + 3.0f*splinePoints[(_currentIndex+1) % numPoints] - 3.0f*splinePoints[(_currentIndex+2) % numPoints] +
	 splinePoints[(_currentIndex+3)% numPoints]) * _currentT* _currentT* _currentT);
	//Debugger::printInfo(curvePoint);
	_currentT += deltaT;
	return curvePoint;

	
}