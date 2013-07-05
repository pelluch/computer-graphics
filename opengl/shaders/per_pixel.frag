#version 330 core

in vec3 fragmentNormal;
in vec3 fragmentColor;
in vec3 fragmentPosition;

out vec3 color;

uniform int numLights;
uniform vec3 lights[10];
uniform vec3 eyePosition;

void main()
{
	vec3 diffuseColor = vec3(0, 0, 0);
	vec3 specularColor = vec3(0, 0, 0);
	vec3 eyeDirection = normalize(eyePosition - fragmentPosition);

	for(int i = 0; i < numLights; i++)
	{
		vec3 lightDirection = normalize(lights[i] - fragmentPosition);
		diffuseColor = diffuseColor + fragmentColor * max(0, dot(lightDirection, fragmentNormal));
		vec3 half = normalize(eyeDirection + lightDirection);
		specularColor = specularColor + pow( max ( 0, dot(half, fragmentNormal ) ), 100) * vec3(1, 1, 1);

	}

	// Output color = red 
	//gl_FragColor = gl_Color;
	color = diffuseColor + specularColor;

}