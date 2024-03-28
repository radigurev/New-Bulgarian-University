//
// Created by Radoslav Gurev on 28.03.24.
//

#include "Building.h"

#pragma region getters
const string &Building::getAddress() const {
    return this->address;
}

int Building::getPlannedLevels() const {
    return this->plannedLevels;
}

int Building::getPlannedEntrances() const {
    return this->plannedEntrances;
}

int Building::getBuildLevels() const {
    return this->buildLevels;
}

int Building::getBuildEntrances() const {
    return this->buildEntrances;
}
#pragma endregion

#pragma region setters
void Building::setAddress(const string &address) {
    this->address = address;
}

void Building::setPlannedLevels(int plannedLevels) {
    this->plannedLevels = plannedLevels;
}

void Building::setPlannedEntrances(int plannedEntrances) {
    this->plannedEntrances = plannedEntrances;
}

void Building::setBuildLevels(int buildLevels) {
    this->buildLevels = buildLevels;
}

void Building::setBuildEntrances(int buildEntrances) {
    this->buildEntrances = buildEntrances;
}

Building::~Building() {
}

Building::Building() {}

Building::Building(const string &address, int plannedLevels, int plannedEntrances, int buildLevels, int buildEntrances)
        : address(address), plannedLevels(plannedLevels), plannedEntrances(plannedEntrances), buildLevels(buildLevels),
          buildEntrances(buildEntrances) {}

          Building::Building(const Building& building)
          : address(building.getAddress()), plannedLevels(building.getPlannedLevels()), plannedEntrances(building.getPlannedEntrances())
          , buildLevels(building.getBuildLevels()), buildEntrances(building.getBuildEntrances()){}

Building operator+(int number,Building & building) {
    Building a(building);
    a.setBuildEntrances(a.getBuildEntrances()+number);

    return a;
}

Building operator+( Building & building, int number) {
    Building a(building);
    a.setBuildLevels(a.getBuildLevels()+number);

    return a;
}

ostream &operator<<(ostream &os, const Building &building) {
    os << "address: " << building.address << " plannedLevels: " << building.plannedLevels << " plannedEntrances: "
       << building.plannedEntrances << " buildLevels: " << building.buildLevels << " buildEntrances: "
       << building.buildEntrances;
    return os;
}

istream &operator>>(istream& is, Building& building) {
    cin >> building.address >> building.plannedLevels
    >> building.buildLevels >> building.plannedEntrances
    >> building.buildEntrances;
}

Building operator+=(Building & building,int number) {
    building.setBuildLevels(building.getBuildLevels() + number);

    return building;
}

Building operator+=(int number, Building & building) {
    building.setBuildEntrances(building.getBuildEntrances() + number);

    return building;
}


#pragma endregion
