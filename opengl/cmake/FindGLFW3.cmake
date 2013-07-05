# Locate the glfw library
# This module defines the following variables:
# GLFW3_LIBRARY, the name of the library;
# GLFW3_INCLUDE_DIR, where to find glfw include files.
# GLFW3_FOUND, true if both the GLFW3_LIBRARY and GLFW3_INCLUDE_DIR have been found.
#
# To help locate the library and include file, you could define an environment variable called
# GLFW3_ROOT which points to the root of the glfw library installation. This is pretty useful
# on a Windows platform.
#
# Usage example to compile an "executable" target to the glfw library:
#
# FIND_PACKAGE (glfw REQUIRED)
# INCLUDE_DIRECTORIES (${GLFW3_INCLUDE_DIR})
# ADD_EXECUTABLE (executable ${EXECUTABLE_SRCS})
# TARGET_LINK_LIBRARIES (executable ${GLFW3_LIBRARY})
#
# TODO:
# Allow the user to select to link to a shared library or to a static library.

#Search for the include file...
IF(WIN_32)
  FIND_PATH(GLFW3_INCLUDE_DIR GLFW/glfw3.h DOC "Path to GLFW3 include directory."
    PATHS
    ${CMAKE_CURRENT_BINARY_DIR}/glfw/include
    ${CMAKE_CURRENT_BINARY_DIR}/glfw/include/GLFW
  )
  FIND_LIBRARY(GLFW3_LIBRARY DOC "Absolute path to GLFW3 library."
    NAMES glfw GLFW
    PATHS
    /usr/lib64
    /usr/lib
    /usr/local/lib64
    /usr/local/lib
  )
ELSE(WIN_32)
  FIND_PATH(GLFW3_INCLUDE_DIR GLFW/glfw3.h DOC "Path to GLFW3 include directory."
    PATHS
    /usr/include/
    /usr/local/include/
    # By default headers are under GLFW subfolder
    /usr/include/GLFW
    /usr/local/include/GLFW
  )

  FIND_LIBRARY(GLFW3_LIBRARY DOC "Absolute path to GLFW3 library."
    NAMES glfw GLFW
    PATHS
    /usr/lib64
    /usr/lib
    /usr/local/lib64
    /usr/local/lib
  )
ENDIF(WIN_32)

IF (GLFW3_INCLUDE_DIR)
  SET( GLFW3_FOUND 1 CACHE STRING "Set to 1 if GLEW is found, 0 otherwise")
ELSE (GLFW3_INCLUDE_DIR)
  SET( GLFW3_FOUND 0 CACHE STRING "Set to 1 if GLEW is found, 0 otherwise")
ENDIF (GLFW3_INCLUDE_DIR)

MARK_AS_ADVANCED( GLFW3_FOUND )