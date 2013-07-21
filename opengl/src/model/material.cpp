
#include "model/material.h"
#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <iostream>

Material::Material()
{
	_generatedUniform = false;
	_texture = NULL;
	_normalTexture = NULL;
	_hasTexture = false;
	_hasNormalTexture = false;
	//std::shared_ptr<Material> asdf;
}

void Material::generateUniformIds(GLuint shaderProgramId)
{

	if(!_generatedUniform)
	{
		this->_diffuseColorId = glGetUniformLocation(shaderProgramId, "materialDiffuse");
		this->_specularColorId = glGetUniformLocation(shaderProgramId, "materialSpecular");
		this->_hasTextureId = glGetUniformLocation(shaderProgramId, "hasTexture");
		this->_hasNormalTextureId = glGetUniformLocation(shaderProgramId, "hasNormalTexture");

		if(_texturePath != "")
		{
			_hasTexture = true;
			this->_texture = new Texture(_texturePath);
			_texture->initTexture(shaderProgramId);
		}
		if(_normalTexturePath != "")
		{
			std::cout << "Loading normal tex" << std::endl;
			_hasNormalTexture = true;
			this->_normalTexture = new Texture(_normalTexturePath, TEX_NORMAL);
			_normalTexture->initTexture(shaderProgramId);

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
	if(_hasNormalTexture)
	{
		_normalTexture->bindUniform();
	}
	glUniform1i(_hasTextureId, _hasTexture);
	glUniform1i(_hasNormalTextureId, _hasNormalTexture);
}

Material::~Material()
{
	if(_texture)
		delete _texture;
	if(_normalTexture)
		delete _normalTexture;
}