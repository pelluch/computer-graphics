#version 330 core

// Input vertex data, different for all executions of this shader.
layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec3 vertexColor;
layout(location = 2) in vec3 vertexNormal;

uniform mat4 MVP;
uniform int numLights;
uniform vec3 lights[10];
uniform vec3 eyePosition;

out vec3 fragmentColor;

void main(){

	gl_Position = MVP * vec4(vertexPosition, 1);
	vec3 diffuseColor = vec3(0, 0, 0);
	vec3 specularColor = vec3(0, 0, 0);
	vec3 eyeDirection = normalize(eyePosition - vertexPosition);

	for(int i = 0; i < numLights; i++)
	{
		vec3 lightDirection = normalize(lights[i] - vertexPosition);
		diffuseColor = diffuseColor + vertexColor * max(0, dot(lightDirection, vertexNormal));
		vec3 half = normalize(eyeDirection + lightDirection);
		specularColor = specularColor + pow( max ( 0, dot(half, vertexNormal ) ), 100) * vec3(1, 1, 1);

	}
    fragmentColor = diffuseColor + specularColor;
}

