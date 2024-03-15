//
// Created by Radoslav Gurev on 15.03.24.
//
#include "Triangle.h"

Triangle::Triangle() : a(Point()), b(Point()), c(Point()) {};

Triangle::Triangle(Point a, Point b, Point c) : a(a), b(b), c(c) {};

void Triangle::setA(const Point &a) {
    this->a = a;
}

void Triangle::setB(const Point &b) {
    this->b = b;
}

void Triangle::setC(const Point &c) {
    this->c = c;
}

const Point &Triangle::getA() const {
    return a;
}

const Point &Triangle::getB() const {
    return b;
}

const Point &Triangle::getC() const {
    return c;
}

Triangle::~Triangle() {

}

double Triangle::sideLength(Point a, Point b) {
    return sqrt(pow(b.getX() - a.getX(), 2) +
                pow(b.getY() - a.getY(), 2));
}

double Triangle::area() {
    double a, b, c;

    a = this->sideLength(this->a, this->b);
    b = this->sideLength(this->b, this->c);
    c = this->sideLength(this->a, this->c);

    double s = this->permieter() / 2;

    return sqrt(s * (s - a) * (s - b) * (s - c));
}

double Triangle::permieter() {
    double a, b, c;

    a = this->sideLength(this->a, this->b);
    b = this->sideLength(this->b, this->c);
    c = this->sideLength(this->a, this->c);

    return a + b + c;
}





