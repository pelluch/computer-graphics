#ifndef CMD_LINE_H_
#define CMD_LINE_H_

#include <string>
#include <map>

enum PARAM_TYPE { INTEGER, FLOAT, STRING, VOID };

class CmdLine
{

private: 
	static std::map<std::string, PARAM_TYPE> _paramTypes;
	static std::map<std::string, std::string> _params;
public:
	static void addParams(const std::string & name, PARAM_TYPE type);
	static int parse(int argc, char ** argv);
};

#endif