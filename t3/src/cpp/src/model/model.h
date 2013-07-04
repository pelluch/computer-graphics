#ifndef MODEL_H_
#define MODEL_H_

#include <glm/glm.hpp>
#include <vector>

class Model
{
	public:
		Model();
		~Model();
		int getNumVertex();
		float * getVertexPtr();
		float * getNormalPtr();
		float * getColorPtr();
		float * getUvPtr();
	private:
		float * _vertexPtr;
		float * _normalPtr;
		float * _colorPtr;
		float * _uvPtr;
		
		std::vector<glm::vec3> _vertex;
		std::vector<glm::vec3> _normals;
		std::vector<glm::vec3> _colors;
		std::vector<glm::vec2> _uv;
};

#endif