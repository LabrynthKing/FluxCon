add_subdirectory(RE-UE4SS EXCLUDE_FROM_ALL)

add_library(FluxConDeps INTERFACE)

target_link_libraries(FluxConDeps INTERFACE UE4SS)
