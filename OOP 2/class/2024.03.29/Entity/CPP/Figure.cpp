//
// Created by Radoslav Gurev on 29.03.24.
//

#include "../Headers/Figure.h"

double Figure::perimeter() {
    return 0;
}

Figure::Figure() {}

Figure::Figure(char *name) {
    this->name = new char[strlen(name) + 1];
    strcpy(this->name,name);
}
