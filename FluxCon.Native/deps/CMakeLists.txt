add_subdirectory(RE-UE4SS)

add_library(FluxConDeps INTERFACE)

target_link_libraries(FluxConDeps INTERFACE UE4SS)
