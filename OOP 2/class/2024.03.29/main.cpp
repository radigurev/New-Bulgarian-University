#include "Entity/Headers/Circle.h"

int main() {

    Circle circle("Krug",4);

    cout << circle.perimeter() << endl;
    cout << circle.calculateArea() << endl;

    return 0;
}
