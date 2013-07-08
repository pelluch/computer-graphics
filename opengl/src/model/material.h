#ifndef MATERIAL_H_
#define MATERIAL_H_

#include <GL/glew.h>
#include <glm/glm.hpp>
#include <string>


class Material
{
	public:
		glm::vec3 _diffuseColor;
		glm::vec4 _specularColor;
		glm::vec3 _reflectiveColor;
		glm::vec3 _refractiveColor;
		Material();
		float _shininess;
		std::string _texturePath;
		std::string _normalTexturePath;
		std::string _bumpTexturePath;

		GLuint _textureId;
		GLuint _textureUniformId;
		GLuint _diffuseColorId;
		GLuint _specularColorId;

		void generateUniformIds(GLuint shaderProgramId);
		void setActiveTexture();
		
	private:
		bool _generatedUniform;
};

#endif