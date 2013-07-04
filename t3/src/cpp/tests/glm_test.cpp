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

}

int main(int argc, char** argv)
{
	glm::vec3 Position = glm::vec3(0, 0, 50);
	glm::vec3 up = glm::vec3(0, 1, 0);
	glm::vec3 center = glm::vec3(0, 0, 60);
	glm::mat4 view = glm::lookAt(Position, center, up);
	printMatrix(view);

	glm::vec4 worldPosition = glm::vec4(0, 0, 0, 0);
	glm::vec4 transformed = view*worldPosition;
	printVector(transformed);

	return 0;
}