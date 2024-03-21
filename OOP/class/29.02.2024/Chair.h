#ifndef INC_29_02_2024_CHAIR_H
#define INC_29_02_2024_CHAIR_H
#include <iostream>

class Chair {
private:
    int legs;
    double height;

public:
    Chair(int, double);
    Chair();

    int getLegs() const;

    double getHeight() const;

    void setLegs(int);

    void setHeight(double);
};

#endif //INC_29_02_2024_CHAIR_H
