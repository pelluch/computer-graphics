cmake_minimum_required (VERSION 2.6) 
project (opengl_game)



set(CMAKE_MODULE_PATH ${CMAKE_MODULE_PATH} ${CMAKE_SOURCE_DIR}/cmake )
if(UNIX)
    SET(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wall -std=gnu++0x")
    include_directories(/usr/include/bullet)
   	SET(LINK_FLAGS /usr/lib/nvidia/libGL.so -ltinyxml2 -lGLEW -lglfw
    -lBulletDynamics -lBulletCollision -lLinearMath -lBulletSoftBody)  
else(UNIX)
	find_package(OpenGL REQUIRED)
	SET(BULLET_INCLUDE_DIR C:/Users/Pablo/src/bullet-2.81)
	SET(BULLET_LIBRARY_DIR C:/Users/Pablo/src/bullet-2.81/build/vs2012/lib)
	SET(GLEW_INCLUDE_DIR C:/Users/Pablo/src/glew-1.9.0/include)
	SET(GLEW_LIBRARY_DIR C:/Users/Pablo/src/glew-1.9.0/build/vc11)
	SET(GLFW3_INCLUDE_DIR C:/Users/Pablo/src/glfw-3.0.1/include)
	SET(GLFW3_LIBRARY_DIR C:/Users/Pablo/src/glfw-3.0.1/build/src)
	SET(TINYXML2_INCLUDE_DIR C:/Users/Pablo/src/tinyxml2)
	SET(TINYXML2_LIBRARY_DIR C:/Users/Pablo/src/tinyxml2/build/)
	SET(GLI_INCLUDE_DIR C:/Users/Pablo/src/gli-0.4.1.0)
	SET(GLM_INCLUDE_DIR C:/Users/Pablo/src/glm-0.9.4.4)
	include_directories(${BULLET_INCLUDE_DIR} ${GLEW_INCLUDE_DIR} 
		${GLFW3_INCLUDE_DIR} ${TINYXML2_INCLUDE_DIR} ${GLI_INCLUDE_DIR}
		${GLM_INCLUDE_DIR})
	link_directories(${BULLET_LIBRARY_DIR} ${GLEW_LIBRARY_DIR} 
		${GLFW3_LIBRARY_DIR} ${TINYXML2_LIBRARY_DIR})
	 SET(LINK_FLAGS tinyxml2.lib glew.lib glfw3.lib opengl32 glu32)
	 SET(BULLET_RELEASE_FLAGS optimized BulletDynamics.lib optimized BulletCollision.lib optimized LinearMath.lib optimized BulletSoftBody.lib)
	 SET(BULLET_DEBUG_FLAGS debug BulletDynamics_Debug.lib debug BulletCollision_Debug.lib debug LinearMath_Debug.lib debug BulletSoftBody_Debug.lib)
endif(UNIX)

include_directories(src)
file(GLOB_RECURSE ICLS ${CMAKE_CURRENT_SOURCE_DIR}/src/*.h)
file(GLOB_RECURSE SRCS ${CMAKE_CURRENT_SOURCE_DIR}/src/*.cpp)
add_executable(main src/main.cpp ${ICLS} ${SRCS})
target_link_libraries(main ${LINK_FLAGS})

execute_process(COMMAND ${CMAKE_COMMAND} -E create_symlink 
	${CMAKE_SOURCE_DIR}/models ${CMAKE_BINARY_DIR}/models)

execute_process(COMMAND ${CMAKE_COMMAND} -E create_symlink 
	${CMAKE_SOURCE_DIR}/scenes ${CMAKE_BINARY_DIR}/scenes)

execute_process(COMMAND ${CMAKE_COMMAND} -E create_symlink 
	${CMAKE_SOURCE_DIR}/textures ${CMAKE_BINARY_DIR}/textures)

execute_process(COMMAND ${CMAKE_COMMAND} -E create_symlink 
	${CMAKE_SOURCE_DIR}/shaders ${CMAKE_BINARY_DIR}/shaders)
#add_subdirectory(tests)
IF(NOT "${CMAKE_BUILD_TYPE}" STREQUAL "Debug")
message("not")
	add_custom_target(
	make_debug ALL
    COMMAND make -C ${CMAKE_SOURCE_DIR}/debug)
endif()
