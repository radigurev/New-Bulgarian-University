#include <iostream>

using namespace std;

int main() {
    const int LIMIT_MAX = 240;

    string name;
    int courses, creditsSum = 0,creditByCourse;

    cout << "Student name: ";
    cin >> name;

    cout << "Number of courses: ";
    cin >> courses;

    for (int i = 0; i < courses; ++i) {
        cin >> creditByCourse;
        creditsSum += creditByCourse;
    }

    if(creditsSum >= LIMIT_MAX)
        cout << name << " gets diploma from NBU with " << creditsSum;
    else
        cout << "As expected, " << name
        << "started a job and left NBU behind. "
        << LIMIT_MAX - creditsSum
        << " credits needed to be granted";

    int numberLimit;

    cout << "Enter a number: ";
    cin >> numberLimit;

    int first = 0;
    int second = 1;


    while (second <= numberLimit && first <= numberLimit) {
            cout << first << " ";

            first += second;

            second = first - second;
    }

}
