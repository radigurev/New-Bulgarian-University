#include <iostream>
using namespace std;

int main() {
    //1
//int fingers;
//
//cin >> fingers;
//
//    switch (fingers) {
//        case 1:
//            cout<< "Monday";
//            break;
//        case 2:
//            cout<< "Thursday";
//            break;
//        case 3:
//            cout<< "Wednesday";
//            break;
//        case 4:
//            cout<< "Thursday";
//            break;
//        case 5:
//            cout<< "Friday";
//            break;
//        case 6:
//            cout<< "Saturday";
//            break;
//        case 7:
//            cout<< "Sunday";
//            break;
//        default:
//            cout<< "Error";
//            break;
//    }

//    string dayOfTheWeek;
//
//    cout << "Enter day of the week (Monday - Sunday): ";
//    cin >> dayOfTheWeek;
//
//    if (dayOfTheWeek == "Monday" || dayOfTheWeek == "Tuesday" ||
//    dayOfTheWeek == "Wednesday" || dayOfTheWeek == "Thursday" ||
//    dayOfTheWeek == "Friday")
//        cout << "Working day";
//    else if (dayOfTheWeek == "Saturday" || dayOfTheWeek == "Sunday")
//        cout << "Weekday";
//    else
//        cout << "Error";

//    string dayOfTheWeek;
//    int hourOfTheWeek;
//
//    cin >> hourOfTheWeek;
//    cin >> dayOfTheWeek;
//
//    //S for saturday or sunday
//    if (hourOfTheWeek > 9 && hourOfTheWeek < 19
//        && toupper(dayOfTheWeek.at(0) != 'S')) {
//        cout << "open";
//    } else {
//        cout << "closed";
//    }

    enum TrafficLights {
        RED,
        YELLOW,
        GREEN
    };

    string command;

    cin >> command;

    TrafficLights currentColor;
    TrafficLights nextColor;

    if (command == "Go") {
        currentColor = GREEN;
        nextColor = YELLOW;
    } else if (command == "Slow Down") {
        currentColor = YELLOW;
        nextColor = RED;
    } else {
        currentColor = RED;
        nextColor = YELLOW;
    }

    cout << "Current traffic light state: ";

    switch (currentColor) {
        case RED:
            cout << "Red";
            break;
        case GREEN:
            cout << "Green";
            break;
        case YELLOW:
            cout << "Yellow";
            break;
    }

    cout << endl;

    cout << "Next traffic light: ";

    switch (nextColor) {
        case RED:
            cout << "Red";
            break;
        case GREEN:
            cout << "Green";
            break;
        case YELLOW:
            cout << "Yellow";
            break;
    }

    return 0;
}
