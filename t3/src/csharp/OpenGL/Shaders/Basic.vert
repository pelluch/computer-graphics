//Vertex shader 2:

//Atributos de entrada: posicion, color y normal del vertice
in vec4 position;
in vec4 color;
in vec4 normal;
in vec4 uv;

//Atributos de salida: color del vertice y coordenadas de textura, indicando modificador
smooth out vec4 theColor;
smooth out vec2 textureUV;

//Matrix view projection definida comom uniform: variable comun a todos los vertices
uniform mat4 viewProjection;
uniform mat4 modelTransform;

//Sampler de textura
uniform sampler2D vertexTex;
uniform vec2 offset;

//Light parameters
const vec4 lightPos = vec4(-250, 1250, -800, 1);
const vec4 lightColor = vec4(0.7, 0.7, 0.7, 1);
const vec4 ambientColor = vec4(0.3, 0.3, 0.3, 1);

const vec4 waterColor = vec4(0,0,1,1);
const vec4 groundColor = vec4(0.1,1,0,1);

void main()
{
    vec4 texColor = texture2D(vertexTex, offset);
    vec4 heightPosition = vec4(position.x,position.y*(1 + 3 * texColor.x),position.z,position.w);
    mat4 mvp = viewProjection * modelTransform;
    vec4 positionInClipping =  mvp * heightPosition;
    gl_Position = positionInClipping;
    
    //Vertex shading (lambert+ambient model)
    vec4 lightDir = lightPos - position;
    lightDir = normalize(lightDir);
    vec4 baseColor = mix(waterColor, groundColor, texColor.x);
    theColor = color *  ambientColor + baseColor * lightColor * max (0, dot(lightDir, normal));

    textureUV = uv.xy;
    
}
