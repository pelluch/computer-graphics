#include "renderer/params.h"

std::string RenderingParams::fragmentShaderPath = "";
std::string RenderingParams::vertexShaderPath = "";
SHADER_MODE RenderingParams::mode = PER_PIXEL;
bool RenderingParams::antiAlias = true;
int RenderingParams::verticalInterval = 1;
bool RenderingParams::paused = false;