#include "Person.h"

Person::Person() {
    this->height = 0;
    this->weight = 0;
    this->name = " ";
}

Person::Person(const char *, double, double) {

}

void Person::setWeight(double weight) {
    this->weight = weight;
}

void Person::setHeight(double height)  {
    this->height = height;
}

void Person::setName(char* name) {
    this->name = name;
}

const char *Person::getName() {
    return this->name;
}

double Person::getHeight() const {
    return this->height;
}

double Person::getWeight() const {
    return this->weight;
}