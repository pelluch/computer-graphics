cmake_minimum_required (VERSION 2.6) 
project (opengl_game)


set(CMAKE_MODULE_PATH ${CMAKE_MODULE_PATH} ${CMAKE_SOURCE_DIR}/cmake )

if(UNIX)
    SET(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wall -std=gnu++0x")
endif()

find_package(GLEW REQUIRED)
find_package(GLFW3 REQUIRED)
#find_package(Boost COMPONENTS system program_options filesystem)

include_directories(${GLEW_INCLUDE_DIR} ${GLFW3_INCLUDE_DIR})
include_directories(src)
include_directories(/home/pablo/src/utils/include)
include_directories(${CMAKE_SOURCE_DIR}/external/gli-0.4.1.0)
include_directories(/usr/include/bullet)
#include_directories(${CMAKE_SOURCE_DIR}/external/glsdk_0.5.0/glimg/include)
#include_directories(${CMAKE_SOURCE_DIR}/external/glsdk_0.5.0/glload/include)

file(GLOB_RECURSE ICLS ${CMAKE_CURRENT_SOURCE_DIR}/src/*.h)
file(GLOB_RECURSE SRCS ${CMAKE_CURRENT_SOURCE_DIR}/src/*.cpp)

link_directories(/home/pablo/src/utils/lib)
#link_directories(${CMAKE_SOURCE_DIR}/external/glsdk_0.5.0/glimg/lib)
#link_directories(${CMAKE_SOURCE_DIR}/external/glsdk_0.5.0/glload/lib)
#link_directories(${CMAKE_SOURCE_DIR}/external/glsdk_0.5.0/glloadgen/lib)
#link_directories(${CMAKE_SOURCE_DIR}/external/glsdk_0.5.0/glutil/lib)

add_executable(main src/main.cpp ${ICLS} ${SRCS})
target_link_libraries(main /usr/lib/nvidia/libGL.so -lutils_cmdline -lutils_filesystem -ltinyxml2 ${GLEW_LIBRARY} ${GLFW3_LIBRARY}
	-lBulletDynamics -lBulletCollision -lLinearMath -lBulletSoftBody )
#add_subdirectory(tests)