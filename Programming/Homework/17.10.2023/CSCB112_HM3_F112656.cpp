#include <iostream>

using namespace std;

int main() {

    //1
    int arabicNumber;

    do {
        cout << "Enter a Arabic numeral (1 - 3999): ";
        cin >> arabicNumber;
    } while (arabicNumber > 4000 || arabicNumber < 0);

    cout << "The roman equivalent numeral: ";
    switch (arabicNumber / 1000) {
        case 1:
            cout << "M";
            break;
        case 2:
            cout << "MM";
            break;
        case 3:
            cout << "MMM";
            break;
    }

    switch (arabicNumber / 100 % 10) {
        case 9:
            cout << "CM";
            break;
        case 8:
            cout << "DCCC";
            break;
        case 7:
            cout << "DCC";
            break;
        case 6:
            cout << "DC";
            break;
        case 5:
            cout << "D";
            break;
        case 4:
            cout << "CD";
            break;
        case 3:
            cout << "CCC";
            break;
        case 2:
            cout << "CC";
            break;
        case 1:
            cout << "C";
            break;
    }

    switch (arabicNumber / 10 % 10) {
        case 9:
            cout << "XC";
            break;
        case 8:
            cout << "LXXX";
            break;
        case 7:
            cout << "LXX";
            break;
        case 6:
            cout << "LX";
            break;
        case 5:
            cout << "L";
            break;
        case 4:
            cout << "XL";
            break;
        case 3:
            cout << "XXX";
            break;
        case 2:
            cout << "XX";
            break;
        case 1:
            cout << "X";
            break;
    }
    switch (arabicNumber % 10) {
        case 9:
            cout << "IX";
            break;
        case 8:
            cout << "VIII";
            break;
        case 7:
            cout << "VII";
            break;
        case 6:
            cout << "VI";
            break;
        case 5:
            cout << "V";
            break;
        case 4:
            cout << "IV";
            break;
        case 3:
            cout << "III";
            break;
        case 2:
            cout << "II";
            break;
        case 1:
            cout << "I";
            break;
    }

    //2
    cout << endl << "Enter a Roman numeral (1 - 3999): ";

    string roman;
    int decimal = 0;

    cin >> roman;

    for (int i = 0; i < roman.length(); i++) {
        switch (roman[i]) {
            case 'M':
                decimal += 1000;
                break;
            case 'D':
                decimal += 500;
                break;
            case 'C':
                if (i + 1 < roman.length() && (roman[i + 1] == 'M' || roman[i + 1] == 'D')) {
                    decimal -= 100;
                } else {
                    decimal += 100;
                }
                break;
            case 'L':
                decimal += 50;
                break;
            case 'X':
                if (i + 1 < roman.length() && (roman[i + 1] == 'C' || roman[i + 1] == 'L')) {
                    decimal -= 10;
                } else {
                    decimal += 10;
                }
                break;
            case 'V':
                decimal += 5;
                break;
            case 'I':
                if (i + 1 < roman.length() && (roman[i + 1] == 'X' || roman[i + 1] == 'V')) {
                    decimal -= 1;
                } else {
                    decimal += 1;
                }
                break;
        }
    }

    cout << "The arabic equivalent numeral:" << decimal << endl;
    //3
    const double MIN = 10.0;
    const double MAX = 20.0;

    cout << "Enter a Real number: ";

    int realNumber;
    cin >> realNumber;

    if (realNumber >= MIN && realNumber <= MAX) {
        cout << "The number belongs to the interval [" << MIN << ", " << MAX << "]." << endl;
    } else {
        cout << "The number does not belong to the interval [" << MIN << ", " << MAX << "]." << endl;
    }
    //4
    cout << "Enter random number: ";
    int userNumber;
    cin >> userNumber;

    while (userNumber > 0) {
        int digit = userNumber % 10;
        cout << digit << " ";
        userNumber /= 10;
    }
    //5
    int minimumInterval;
    int maximumInterval;
    cout << endl << "Enter the minimum value of the interval: ";
    cin >> minimumInterval;
    cout << "Enter the maximum value of the interval: ";
    cin >> maximumInterval;

    cout << endl << "Random generated number: " << endl;
    int randomNum = minimumInterval + (rand() % (maximumInterval - minimumInterval + 1));

    if (randomNum < 1000) {
        while (randomNum > 0) {
            int digit = randomNum % 10;
            cout << digit << " " << endl;
            randomNum /= 10;
        }
    }
    //6
    int minInterval;
    int maxInterval;

    cout << "Enter the minimum value of the interval: ";
    cin >> minInterval;
    cout << "Enter the maximum value of the interval: ";
    cin >> maxInterval;

    double random = minInterval + (rand() % (maxInterval - minInterval + 1));

    cout << "Randomly generated number in the interval " << minInterval << " - " << maxInterval << " is " << random
         << endl;

    return 0;
}
