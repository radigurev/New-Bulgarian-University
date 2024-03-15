//
// Created by Radoslav Gurev on 15.03.24.
//

#ifndef INC_15_03_2024_TRIANGLE_H
#define INC_15_03_2024_TRIANGLE_H

#include "Point.h"

class Triangle {
private:
    Point a;
    Point b;
    Point c;
public:
    Triangle();

    Triangle(Point, Point, Point);

//    ~Triangle();
    double sideLength(Point, Point);

    double area();

    double permieter();

    void setA(const Point &a);

    void setB(const Point &b);

    void setC(const Point &c);

    const Point &getA() const;

    const Point &getB() const;

    const Point &getC() const;

    virtual ~Triangle();
};

#endif //INC_15_03_2024_TRIANGLE_H
