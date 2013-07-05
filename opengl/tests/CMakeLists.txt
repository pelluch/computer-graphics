ADD_EXECUTABLE(glm_test glm_test.cpp)
TARGET_LINK_LIBRARIES(glm_test /usr/lib/nvidia/libGL.so -lutils_cmdline ${Boost_LIBRARIES} ${GLEW_LIBRARY} ${GLFW3_LIBRARY})