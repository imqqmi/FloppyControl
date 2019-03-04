# build_library.cmake: utilility function for building static
# libraries for the Arduino DUE platform

include(CMakeParseArguments)

# funtion build_library(
#  library SRC_PATHS ... INCLUDE_PATHS ...
# )
function(build_library library)

  set(multi_value_args SRC_PATHS INCLUDE_PATHS)
  cmake_parse_arguments(
    ${library} "" "" "${multi_value_args}" ${ARGN} )

  #message(STATUS "${library}_SRC_PATHS: ${${library}_SRC_PATHS}")
  #message(STATUS "${library}_INCLUDE_PATHS: ${${library}_INCLUDE_PATHS}")

  # adding *.c and *.cpp extensions to paths
  set(compile_paths "")
  foreach(src_path ${${library}_SRC_PATHS})
    if(EXISTS ${src_path})
      set(compile_paths ${compile_paths} ${src_path}/*.c)
      set(compile_paths ${compile_paths} ${src_path}/*.cpp)
    else()
      message(SEND_ERROR "[ERROR] path ${src_path} not found!")
      message(SEND_ERROR "[ERROR] library ${library} cannot be built!")
      return()
    endif()
  endforeach(src_path)

  file(
    GLOB LIB_SOURCE_FILES
    LIST_DIRECTORIES false 
    ${compile_paths}
  )

  #message(STATUS "LIB_SOURCE_FILES (${library}): ${LIB_SOURCE_FILES}")

  add_library(${library} STATIC ${LIB_SOURCE_FILES})
  set_property(
    TARGET ${library} PROPERTY 
    LIBRARY_OUTPUT_DIRECTORY ${CMAKE_CURRENT_LIST_DIR}
  )
  target_compile_options(
    ${library} PUBLIC 
    -c -g -Os -w -ffunction-sections -fdata-sections -nostdlib -fno-threadsafe-statics --param max-inline-insns-single=500 -fno-rtti -fno-exceptions -MMD -mcpu=cortex-m3 -std=gnu++11 -mthumb
  )
  
  target_compile_definitions(
    ${library} PUBLIC
    -Dprintf=iprintf -DF_CPU=84000000L -DARDUINO=10605 -DARDUINO_SAM_DUE -DARDUINO_ARCH_SAM -D__SAM3X8E__ -DUSB_VID=0x2341 -DUSB_PID=0x003e -DUSBCON -DUSB_MANUFACTURER="Unknown" -DUSB_PRODUCT="Arduino Due"
  )

  target_include_directories(
    ${library} PUBLIC
    ${${library}_SRC_PATHS}
    ${${library}_INCLUDE_PATHS}
  )

endfunction(build_library)



