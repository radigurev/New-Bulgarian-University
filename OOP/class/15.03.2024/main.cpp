#include "Point.h"

using namespace std;

int main() {
    Point a(1,2);
    Point b(4,5);

    Point c = a + b;
    a += b;

    Point d = c;
    cout << "X: "<< c.getX() << " Y: " << c.getY() << endl;

    cout << "X: "<< a.getX() << " Y: " << a.getY() << endl;

    cout << "X: "<< d.getX() << " Y: " << d.getY() << endl;
    return 0;
}
