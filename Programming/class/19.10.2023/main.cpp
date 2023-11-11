#include <iostream>
using namespace std;

int main() {

    double TRANSFER_TO_LITERS_COEFFICIENT = 0.0295735;

    const double MINERAL_OIL_DENSITY = 0.87;
    const double LITHIUM_STEARATE_DENSITY = (double) 723 / 1000;
    const double SECRET_ADDITION_DENSITY = (0.0022 * 0.453592) / (0.0338 * TRANSFER_TO_LITERS_COEFFICIENT);

    double mineralOilWeight = 0;
    double lithiumStearateWeight = 0;
    double secretAdditionWeight = 0;

    double maxSpeed = (330 * 300000 / 1000) / (29.5 * 24);

    double mineralOil = 33.8140227 * TRANSFER_TO_LITERS_COEFFICIENT;

    double lithiumStearate = 2.339 * TRANSFER_TO_LITERS_COEFFICIENT;

    double secretAddition = 5.072 * TRANSFER_TO_LITERS_COEFFICIENT;

    cout << "Mineral Oil: " << mineralOil << " L" << endl;
    cout << "Lithium Stearate: " << lithiumStearate << " L" << endl;
    cout << "Secret Addition: " << secretAddition << " L" << endl;

    mineralOilWeight += mineralOil * MINERAL_OIL_DENSITY;

    lithiumStearateWeight += lithiumStearate * LITHIUM_STEARATE_DENSITY;

    secretAdditionWeight += secretAddition * SECRET_ADDITION_DENSITY;

    cout << "Mineral Oil Weight: " << mineralOilWeight;
    cout << "Lithium Stearate Weight: " << lithiumStearateWeight;
    cout << "Secret Addition Weight: " << secretAdditionWeight;

    return 0;
}
