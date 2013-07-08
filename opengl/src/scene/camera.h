#ifndef CAMERA_H_
#define CAMERA_H_

#include <glm/glm.hpp>
#include <GL/glew.h>

class Camera
{
public:
	Camera();
	Camera(float fov, float near, float far, glm::vec3 eye,
		glm::vec3 target, glm::vec3 up);
	glm::mat4 viewTransform();
	glm::mat4 perspectiveTransform(float aspectRatio);
	void moveCamera(glm::vec3 translation, glm::vec3 rotation);
	void generateId(GLuint shaderProgramId);
	void assignUniformData();
	void printInfo();
private:
	float _fov;	//Must be in degrees
	float _near;
	float _far;
	glm::vec3 _eye;
	glm::vec3 _target;
	glm::vec3 _up;
	GLuint _id;
};

#endif