#include "Chair.h"

Chair::Chair() {
    this->legs = 0;
    this->height = 0;
}

Chair::Chair(int legs, double height) {
    this->legs = legs;
    this->height = height;
}

int Chair::getLegs() const {
    return this->legs;
}

double Chair::getHeight() const {
    return this->height;
}

void Chair::setLegs(int legs) {
    if (legs > 0)
        this->legs = legs;
}

void Chair::setHeight(double height) {
    if (height > 0)
        this->height = height;
}