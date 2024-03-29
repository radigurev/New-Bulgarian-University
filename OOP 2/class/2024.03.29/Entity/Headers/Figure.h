//
// Created by Radoslav Gurev on 29.03.24.
//

#ifndef INC_2024_03_29_FIGURE_H
#define INC_2024_03_29_FIGURE_H

#include "iostream"
using namespace std;

class Figure {
private:
    char* name;
public:
    virtual double perimeter();

    Figure();

    Figure(char *name);
};

#endif //INC_2024_03_29_FIGURE_H
