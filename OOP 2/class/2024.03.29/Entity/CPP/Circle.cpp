//
// Created by Radoslav Gurev on 29.03.24.
//

#include "../Headers/Circle.h"

Circle::Circle(char* name, double r) : Figure(name), r(r) {}

Circle::Circle() {}

double Circle::perimeter() {
    return 2 * this->getR() * 3.14;
}

double Circle::getR() const {
    return r;
}

void Circle::setR(double r) {
    Circle::r = r;
}

double Circle::calculateArea() {
    return 3.14 * r * r;
}