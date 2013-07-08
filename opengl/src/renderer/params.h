#ifndef PARAMS_H
#define PARAMS_H

#include <string>
#include <vector>

enum SHADER_MODE { PER_VERTEX, PER_PIXEL };
class RenderingParams
{
	private:
		static int _width;
		static int _height;
	public:
		static std::string vertexShaderPath;
		static std::string fragmentShaderPath;
		static SHADER_MODE mode;
		static bool antiAlias;
		static int verticalInterval;
		static bool paused;
		static void setWindowSize(int width, int height);	
		static float getAspectRatio();	
};

#endif