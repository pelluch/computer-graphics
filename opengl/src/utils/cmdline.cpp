#include "utils/cmdline.h"
#include <string>
#include <map>
#include <iostream>

std::map<std::string, std::string> CmdLine::_params;
std::map<std::string, PARAM_TYPE> CmdLine::_paramTypes;


void CmdLine::addParams(const std::string &name, PARAM_TYPE type)
{
	if(_paramTypes.find(name) == _paramTypes.end() )
		_paramTypes[name] = type;
	else
		std::cerr << "Parameter " << name << " was already added." << std::endl;
}

int CmdLine::parse(int argc, char **argv)
{
	for(int i = 1; i < argc; ++i)
	{

	}
	return 0;
}