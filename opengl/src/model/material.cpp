
#include "model/material.h"
#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <iostream>
#include <gli/gli.hpp>

Material::Material()
{
	_generatedUniform = false;
}

void Material::generateUniformIds(GLuint shaderProgramId)
{

	if(!_generatedUniform)
	{
		this->_diffuseColorId = glGetUniformLocation(shaderProgramId, "materialDiffuse");
		this->_specularColorId = glGetUniformLocation(shaderProgramId, "materialSpecular");
		this->_hasTextureId = glGetUniformLocation(shaderProgramId, "hasTexture");

		if(_texturePath != "")
		{
			_hasTexture = true;
			std::cout << "Loading texture " << _texturePath << std::endl;
			gli::texture2D Texture(gli::loadStorageDDS(_texturePath));
			this->_textureUniformId = glGetUniformLocation(shaderProgramId, "textureSampler");

			assert(!Texture.empty());

			glGenTextures(1, &_textureId);
			glBindTexture(GL_TEXTURE_2D, _textureId);
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_BASE_LEVEL, 0);
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAX_LEVEL, GLint(Texture.levels() - 1));
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_SWIZZLE_R, GL_RED);
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_SWIZZLE_G, GL_GREEN);
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_SWIZZLE_B, GL_BLUE);
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_SWIZZLE_A, GL_ALPHA);

			glTexStorage2D(GL_TEXTURE_2D,
				GLint(Texture.levels()),
				GLenum(gli::internal_format(Texture.format())),
				GLsizei(Texture.dimensions().x),
				GLsizei(Texture.dimensions().y));

			if(gli::is_compressed(Texture.format()))
			{
				for(gli::texture2D::size_type Level = 0; Level < Texture.levels(); ++Level)
				{
					glCompressedTexSubImage2D(GL_TEXTURE_2D,
						GLint(Level),
						0, 0,
						GLsizei(Texture[Level].dimensions().x),
						GLsizei(Texture[Level].dimensions().y),
						GLenum(gli::internal_format(Texture.format())),
						GLsizei(Texture[Level].size()),
						Texture[Level].data());
				}
			}
			else
			{
				for(gli::texture2D::size_type Level = 0; Level < Texture.levels(); ++Level)
				{
					glTexSubImage2D(GL_TEXTURE_2D,
						GLint(Level),
						0, 0,
						GLsizei(Texture[Level].dimensions().x),
						GLsizei(Texture[Level].dimensions().y),
						GLenum(gli::external_format(Texture.format())),
						GLenum(gli::type_format(Texture.format())),
						Texture[Level].data());
				}
			}

		}
		_generatedUniform = true;
	}
	else 
	{
		_hasTexture = false;
		return;
	}
}

void Material::setActiveTexture()
{
	if(_hasTexture)
	{
		// Bind our texture in Texture Unit 0
		glActiveTexture(GL_TEXTURE0);
		glBindTexture(GL_TEXTURE_2D, _textureId);
		// Set our "myTextureSampler" sampler to user Texture Unit 0
		glUniform1i(_textureUniformId, 0);
	}
	else
	{
		glUniform1i(_hasTextureId, _hasTexture);
	}
}