//
// Created by Radoslav Gurev on 29.03.24.
//

#ifndef INC_2024_03_29_CIRCLE_H
#define INC_2024_03_29_CIRCLE_H
#include "Figure.h"
#include "Area.h"

class Circle : public Figure, public Area{
private:
    double r;
public:
    Circle(char *name, double r);

    Circle();

    double perimeter() override;

    double getR() const;

    void setR(double r);

    double calculateArea() override;
};
#endif //INC_2024_03_29_CIRCLE_H
