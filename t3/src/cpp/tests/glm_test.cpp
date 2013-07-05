#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/matrix_access.hpp>
#include <iostream>

void printMatrix(glm::mat4 matrix)
{
	std::cout << "[" << std::endl;
	for(int i = 0; i < 4; ++i)
	{
		std::cout << "\t";
		for(int j = 0; j < 4; ++j)
		{

	
		}
		std::cout << std::endl;
	}
}

void printVector(glm::vec4 vector)
{
	for(int i = 0; i < 4; ++i)
		std::cout << vector[i] << ", ";

	std::cout << std::endl;
}

int main(int argc, char** argv)
{
	glm::vec3 Position = glm::vec3(0, 1, 0);
	glm::mat4 translate = glm::translate(glm::mat4(1.0f), glm::vec3(50,0,-10));
	glm::mat4 rotate = glm::rotate(glm::mat4(1.0f), 180.0f, glm::vec3(1,0,0));
	glm::vec4 transformed = rotate*glm::vec4(Position,1);
	printVector(transformed);

	return 0;
}