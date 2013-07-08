#ifndef MATERIAL_H_
#define MATERIAL_H_

#include <GL/glew.h>
#include <glm/glm.hpp>
#include <string>
#include "model/texture.h"

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

		~Material();
		GLuint _diffuseColorId;
		GLuint _specularColorId;
		GLuint _hasTextureId;
		void generateUniformIds(GLuint shaderProgramId);
		void setActiveTexture();
		
	private:
		bool _hasTexture;
		bool _generatedUniform;
		Texture * _texture;
};

#endif