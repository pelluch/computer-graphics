add_executable(glm_test glm_test.cpp)
target_link_libraries(glm_test /usr/lib/nvidia/libGL.so -lutils_cmdline ${Boost_LIBRARIES} ${GLEW_LIBRARY} ${GLFW3_LIBRARY})