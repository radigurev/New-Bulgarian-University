cmake_minimum_required(VERSION 3.27)
project(2024_03_29)

set(CMAKE_CXX_STANDARD 17)

add_executable(2024_03_29 main.cpp
        Entity/Headers/Figure.h
        Entity/CPP/Figure.cpp
        Entity/Headers/Circle.h
        Entity/CPP/Circle.cpp
        Entity/Headers/Area.h
        Entity/CPP/Area.cpp)
