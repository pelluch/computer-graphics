#ifndef SHADER_PARAMS_H
#define SHADER_PARAMS_H

enum SHADER_MODE { PER_VERTEX, PER_PIXEL };
class ShaderParams
{
	public:
		SHADER_MODE mode;
};

#endif