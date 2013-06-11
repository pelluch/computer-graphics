//Fragment shader 2

//Atributo de entrada del fragmento: color
smooth in vec4 theColor;
//Coordenadas de textura
smooth in vec2 textureUV; //Perspective correction

//Sampler de fragmentTex
uniform sampler2D fragmentTex;

//Atributo de salida del fragmento: color
out vec4 outputColor;

void main()
{
   outputColor = theColor * texture2D(fragmentTex, textureUV);   
}


