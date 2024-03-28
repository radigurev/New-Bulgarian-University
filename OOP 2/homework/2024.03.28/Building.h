//
// Created by Radoslav Gurev on 28.03.24.
//

#ifndef INC_2024_03_28_BUILDING_H
#define INC_2024_03_28_BUILDING_H

#include "iostream"

using namespace std;

class Building {
private:
    string address;
    int plannedLevels;
    int plannedEntrances;
    int buildLevels;
    int buildEntrances;
public:

#pragma region getters

    int getPlannedLevels() const;

    int getPlannedEntrances() const;

    int getBuildLevels() const;

    int getBuildEntrances() const;

    const string &getAddress() const;

#pragma endregion

#pragma region setters

    void setPlannedEntrances(int);

    void setAddress(const string &);

    void setPlannedLevels(int);

    void setBuildLevels(int);

    void setBuildEntrances(int);

#pragma endregion

#pragma region constructors
    ~Building();

    Building();

    Building(const string &, int, int, int, int);

    Building(const Building&);

#pragma endregion

#pragma region operators
    friend Building operator+(int,Building&);

    friend Building operator+(Building&, int);

    friend Building operator+=(Building&, int);

    friend Building operator+=(int,Building&);

    friend ostream &operator<<(ostream &os, const Building &building);

    friend istream& operator>>(istream&, Building&);
#pragma endregion
};

#endif //INC_2024_03_28_BUILDING_H
