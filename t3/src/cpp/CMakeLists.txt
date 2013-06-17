cmake_minimum_required (VERSION 2.6) 
project (opengl_game)

set(CMAKE_MODULE_PATH ${CMAKE_MODULE_PATH} ${CMAKE_SOURCE_DIR}/cmake )
find_package(GLEW REQUIRED)
find_package(GLFW3 REQUIRED)

include_directories(${GLEW_INCLUDE_DIR} ${GLFW3_INCLUDE_DIR})

add_executable(01_simple tests/01_simple.cpp)
target_link_libraries(01_simple ${GLEW_LIBRARY} ${GLFW3_LIBRARY})