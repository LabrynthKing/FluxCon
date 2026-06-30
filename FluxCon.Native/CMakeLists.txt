cmake_minimum_required(VERSION 4.2)

if (UNIX)
    set(CMAKE_TOOLCHAIN_FILE ${CMAKE_CURRENT_SOURCE_DIR}/deps/RE-UE4SS/cmake/toolchains/xwin-clang-cl-toolchain.cmake)
endif ()

project(FluxCon.Native LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 23)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

# Ignore Warnings
add_compile_options("-w")

add_subdirectory(deps EXCLUDE_FROM_ALL)

set(TARGET FluxCon.Native)

add_library(${TARGET} SHARED
        # Main Files
        src/Flux.cpp
        src/Init.cpp
        src/FluxConAPI_Impl.hpp
        src/FluxConAPI_Impl.cpp
        src/Handlers/LuaHandler.cpp
        src/Handlers/PipeHandler.cpp
        src/Handlers/ModHandler.cpp
        src/Handlers/LogHandler.cpp

        # Headers
        include/FluxCon.h
        include/Init.hpp
        include/Types.hpp
        include/API/FluxCon.hpp
        include/Handlers/LuaHandler.hpp
        include/Handlers/PipeHandler.hpp
        include/Handlers/ModHandler.hpp
        include/Handlers/LogHandler.hpp
        include/Utils/ByteWriter.hpp

)

target_compile_definitions(${TARGET}
        PRIVATE
        NOMINMAX
)

target_include_directories(${TARGET}
        PUBLIC
        ${CMAKE_CURRENT_SOURCE_DIR}/include
)

target_link_libraries(${TARGET}
        PRIVATE
        FluxConDeps
)

set_target_properties(${TARGET} PROPERTIES
        OUTPUT_NAME "main"
)
