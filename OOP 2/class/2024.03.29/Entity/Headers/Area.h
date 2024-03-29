//
// Created by Radoslav Gurev on 29.03.24.
//

#ifndef INC_2024_03_29_AREA_H
#define INC_2024_03_29_AREA_H

class Area {
private:
    double myArea;
public:
    virtual double calculateArea();

    double getMyArea() const;

    void setMyArea(double myArea);
};

#endif //INC_2024_03_29_AREA_H
