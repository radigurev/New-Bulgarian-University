//
// Created by Radoslav Gurev on 15.03.24.
//
#include "Point.h"

//: x(0),y(0) {}
Point::Point(int x, int y)  : x(x),y(y) {};

Point::Point() : x(0), y(0) {}

int Point::getX() const {
    return this->x;
}

int Point::getY() const {
    return this->y;
}

void Point::setX(int x) {
    this->x = x;
}

void Point::setY(int y) {
    this->y = y;
}

Point Point::sum(Point &rhs) {
//    this->x += rhs.x;
//    this->y += rhs.y;

return {this->x+=rhs.getX(), this->y+=rhs.getY()};
}

Point Point::operator+(const Point & rhs) {
    return {this->getX()+rhs.getX(), this->getY()+rhs.getY()};
}

Point Point::operator+=(const Point & rhs) {
    return {this->x+=rhs.getX(), this->y+=rhs.getY()};
}

Point Point::operator=(const Point & rhs) {
    return rhs;
}


