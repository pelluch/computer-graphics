#version 330 core

//in vec3 fragmentNormal;
//in vec3 fragmentWorldPosition;
//in vec2 fragmentUV;

out vec3 color;

uniform int numLights;
uniform vec3 lights[10];
uniform vec3 eyePosition;
uniform vec3 ambientLight;
uniform vec3 materialDiffuse;
uniform vec4 materialSpecular;

uniform sampler2D diffuseTextureSampler;
uniform sampler2D normalTextureSampler;
uniform bool hasTexture;
uniform bool hasNormalTexture;

const vec3 lightColor = vec3(1, 1, 1);
in vec3 fragmentWorldPosition;
in vec2 fragmentUV;
in vec3 lightDirectionTangents[10];
in vec3 eyeDirectionTangent;
in vec3 eyeDirectionCamera;
in vec3 lightDirectionCameras[10];
in vec3 fragmentNormal;

void main()
{

	float lightPower = 40.0;
	vec3 materialDiffuseColor = texture2D(diffuseTextureSampler, fragmentUV).rgb;
	vec3 materialAmbientColor = ambientLight * materialDiffuseColor;
	vec3 materialSpecularColor = materialSpecular.rgb;
	float shininess = materialSpecular.a;
	vec3 lightDirection = normalize(lights[0] - fragmentWorldPosition);

	vec3 textureNormalTangent = normalize(texture2D(normalTextureSampler,
		vec2(fragmentUV.x - fragmentUV.y)) .rgb * 2.0 - 1.0);

	//float distance = length(lights[0] - fragmentWorldPosition);

	vec3 n = textureNormalTangent;
	vec3 l = normalize(lightDirectionTangents[0]);
	float cosTheta = clamp(dot(n,l),0,1);
	vec3 E = normalize(eyeDirectionTangent);
	vec3 R = reflect(-l,n);
	float cosAlpha = clamp( dot(E, R), 0, 1);

	
	if(hasTexture && hasNormalTexture)
		color =
			materialAmbientColor +
			materialDiffuseColor * 2000 * lightColor * dot(l, n);
	else
	{
		vec3 diffuseComponent = vec3(0,0,0);
		if(hasTexture) diffuseComponent = materialDiffuseColor;
		else diffuseComponent = materialDiffuse;

		color = ambientLight * diffuseComponent + diffuseComponent * lightColor * dot(lightDirection,fragmentNormal);
	}

	/*vec3 diffuseColor = vec3(0, 0, 0);
	vec3 specularColor = vec3(0, 0, 0);
	vec3 eyeDirection = normalize(eyePosition - fragmentWorldPosition);
	vec3 colorMultiplier = hasTexture ? texture2D(diffuseTextureSampler, fragmentUV).rgb : materialDiffuse;
	for(int i = 0; i < numLights; i++)
	{
		vec3 lightDirection = normalize(lights[i] - fragmentWorldPosition);
		diffuseColor = diffuseColor + colorMultiplier * max(0, dot(lightDirection, fragmentNormal));
		vec3 half = normalize(eyeDirection + lightDirection);
		specularColor = specularColor + materialSpecular.xyz * pow( max ( 0, dot(half, fragmentNormal ) ), materialSpecular.w);

	}

	// Output color = red 
	//gl_FragColor = gl_Color;
	color = diffuseColor + specularColor + ambientLight * texture2D(diffuseTextureSampler, fragmentUV).rgb;
	*/
	//color = materialDiffuse;

}