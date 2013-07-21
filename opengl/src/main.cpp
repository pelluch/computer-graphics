#include "game/gameengine.h"

using namespace std;

int main(int argc, char ** argv)
{
	GameEngine * game = new GameEngine();
	game->start();
	delete game;

	return 0;
}