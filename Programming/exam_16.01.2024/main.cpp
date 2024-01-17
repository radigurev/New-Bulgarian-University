#include <iostream>

using namespace std;

const double PI = 3.14;

double calculateArea(double);
double calculatePerimeter(double);
void findAreaAndPerimeter(double);

//int main() {
//    double radius;
//
//    cout << "Enter radius: ";
//    cin >> radius;
//
//    findAreaAndPerimeter(radius);
//}

double calculateArea(double radius) {
    return PI * radius * radius;
}

double calculatePerimeter(double radius) {
    return 2 * PI * radius;
}

void findAreaAndPerimeter(double radius) {
    cout << "Area: " << calculateArea(radius) << endl;
    cout << "Perimeter: " << calculatePerimeter(radius) << endl;
}
