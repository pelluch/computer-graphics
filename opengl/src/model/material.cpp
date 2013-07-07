#include "model/material.h"

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
		_generatedUniform = true;
	}
	else 
		return;
}