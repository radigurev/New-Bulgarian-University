#include "Building.h"

int main() {

    Building building("adres",5,5,5,5);

    cout << 5 + building << endl;
    cout << building + 5 << endl;

    building += 5;
    cout << building << endl;

    5 += building;
    cout<< building << endl;
    return 0;
}
