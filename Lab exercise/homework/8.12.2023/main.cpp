#include <iostream>
#include <string>

using namespace std;

bool isLeapYear(int year) {
    return (year % 4 == 0 && (year % 100 != 0 || year % 400 == 0));
}

bool isValidDate(int year, int month, int day) {
    if (year < 1800 || year > 2024 || month < 1 || month > 12 || day < 1) {
        return false;
    }

    int daysInMonth[] = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
    if (month == 2 && isLeapYear(year)) {
        daysInMonth[1] = 29;
    }

    return day <= daysInMonth[month - 1];
}

bool isEGNValid(const string& egn) {

    int year = stoi(egn.substr(0, 2));
    int month = stoi(egn.substr(2, 2));
    int day = stoi(egn.substr(4, 2));

    if (month > 40) {
        year += 2000;
        month -= 40;
    } else if (month > 20) {
        year += 1800;
        month -= 20;
    } else {
        year += 1900;
    }

    int weights[] = {2, 4, 8, 5, 10, 9, 7, 3, 6};
    int sum = 0;
    for (int i = 0; i < 9; ++i) {
        sum += (egn[i] - '0') * weights[i];
    }
    int checkDigit = sum % 11;
    if (checkDigit == 10) checkDigit = 0;

    return checkDigit == (egn[9] - '0');
}

int main() {
    string egn;
    cin >> egn;

    if (egn.length() != 10) {
        return 0;
    }

    if (isEGNValid(egn)) {
        cout << "1" << endl;
    } else {
        cout << "0" << endl;
    }

    return 0;
}