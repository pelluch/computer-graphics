#version 330 core

in vec3 fragmentNormal;
in vec3 fragmentWorldPosition;
in vec2 fragmentUV;

out vec3 color;

uniform int numLights;
uniform vec3 lights[10];
uniform vec3 eyePosition;
uniform vec3 ambientLight;
//uniform vec3 materialDiffuse;
uniform vec4 materialSpecular;
uniform sampler2D textureSampler;

void main()
{
	vec3 diffuseColor = vec3(0, 0, 0);
	vec3 specularColor = vec3(0, 0, 0);
	vec3 eyeDirection = normalize(eyePosition - fragmentWorldPosition);
	vec3 colorMultiplier = texture2D(textureSampler, fragmentUV).rgb;
	for(int i = 0; i < numLights; i++)
	{
		vec3 lightDirection = normalize(lights[i] - fragmentWorldPosition);
		diffuseColor = diffuseColor + colorMultiplier * max(0, dot(lightDirection, fragmentNormal));
		vec3 half = normalize(eyeDirection + lightDirection);
		specularColor = specularColor + materialSpecular.xyz * pow( max ( 0, dot(half, fragmentNormal ) ), materialSpecular.w);

	}

	// Output color = red 
	//gl_FragColor = gl_Color;
	color = diffuseColor + specularColor + ambientLight * texture2D(textureSampler, fragmentUV).rgb;

}