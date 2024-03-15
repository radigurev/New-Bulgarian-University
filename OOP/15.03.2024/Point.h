//
// Created by Radoslav Gurev on 15.03.24.
//

#ifndef INC_15_03_2024_POINT_H
#define INC_15_03_2024_POINT_H

#include <iostream>
#include "cmath"

class Point {
private:
    int x;
    int y;
public:
    Point(int, int);

    Point();

#pragma region Getters
    int getX() const;

    int getY() const;
#pragma endregion getters
#pragma region Setters
    void setX(int);

    void setY(int);
#pragma endregion Setters

    Point sum(Point &rhs);
#pragma region Operations
    Point operator+(const Point &);

    Point operator+=(const Point &);

    Point operator=(const Point &);

#pragma endregion Operations
};

#endif //INC_15_03_2024_POINT_H
