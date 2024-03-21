#include <iostream>
using namespace std;

int main() {
    const float SHOE_WEIGHT = 0.4;
    const float CLOTHES_WEIGHT = 1;

    float mass;
    char gender;
    int p;

    cout << "Please enter you mass: ";

    cin >> mass;

    cout << "Mass: " << mass << endl;

    cout << "A or F: ";

    cin >> gender;

    p = (int) gender - 68;

    mass += p;

    cout << "Korigirana masa: " << mass << endl;

    mass -=SHOE_WEIGHT * 2 + CLOTHES_WEIGHT;

    cout << "Final mass: " << mass << endl;

    float height;

    cout << "Viso4ina v metri: ";
    cin >> height;

    float BMI = mass / pow(height,2);

    cout << "BMI: "<<BMI<<endl;
    return 0;
}
