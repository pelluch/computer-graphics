cmake_minimum_required (VERSION 2.6) 
project (opengl_game)

include(ExternalProject)
set(CMAKE_MODULE_PATH ${CMAKE_MODULE_PATH} ${CMAKE_SOURCE_DIR}/cmake )


find_package(GLEW REQUIRED)
find_package(GLFW3 REQUIRED)
find_package(Boost COMPONENTS system program_options)

include_directories(${GLEW_INCLUDE_DIR} ${GLFW3_INCLUDE_DIR})

add_executable(01_simple tests/01_simple.cpp)
target_link_libraries(01_simple ${GLEW_LIBRARY} ${GLFW3_LIBRARY})

add_executable(02_triangles tests/02_triangles.cpp)
target_link_libraries(02_triangles /usr/lib/nvidia/libGL.so ${GLEW_LIBRARY} ${GLFW3_LIBRARY})

file(GLOB_RECURSE ICLS ${CMAKE_CURRENT_SOURCE_DIR}/src/*.h)
file(GLOB_RECURSE SRCS ${CMAKE_CURRENT_SOURCE_DIR}/src/*.cpp)
include_directories(src)

include_directories(/home/pablo/src/utils/include)
link_directories(/home/pablo/src/utils/lib)

add_executable(main src/main.cpp ${ICLS} ${SRCS})
target_link_libraries(main /usr/lib/nvidia/libGL.so -lutils_cmdline ${Boost_LIBRARIES} ${GLEW_LIBRARY} ${GLFW3_LIBRARY})