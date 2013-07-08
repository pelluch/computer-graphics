
#include "model/material.h"
#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <iostream>

Material::Material()
{
	_generatedUniform = false;
	_texture = NULL;
	_hasTexture = false;
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
			this->_texture = new Texture(_texturePath);
			_texture->initTexture(shaderProgramId);
		}
		_generatedUniform = true;
	}
}

void Material::setActiveTexture()
{

	if(_hasTexture)
	{
		_texture->bindUniform();
	}

	glUniform1i(_hasTextureId, _hasTexture);
}

Material::~Material()
{
	if(_texture)
	{
		delete _texture;
	}
}