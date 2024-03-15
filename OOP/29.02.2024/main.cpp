#include <iostream>
#include "Chair.h"

using namespace std;

int main() {

    int legs = 4;
    double height = 0.9;

    Chair chair(legs,height);

    cout << chair.getHeight() << " " << chair.getLegs() << endl;

    chair.setHeight(1.6);
    chair.setLegs(-44);

    cout << chair.getHeight() << " " << chair.getLegs() << endl;

    return 0;
}