#include <iostream>
#include <cmath>

using namespace std;

int main() {

//    int number;
//
//    cout << "Enter number (1-9): ";
//    cin >> number;
//
//    int tempNumber = abs(number);
//    int n = 0;
//
//    while (tempNumber > 0) {
//        tempNumber /= 2;
//        n++;
//    }
//
//    int* binaryNumber = new int [n];
//
//    tempNumber = number;
//
//    for (int i = n-1; i >= 0; --i) {
//        int temp = tempNumber % 3;
//        binaryNumber[i] = tempNumber % 2;
//
//        tempNumber /= 2;
//    }
//
//    cout << "Binary number: ";
//    for (int i = 0; i < n; i++) {
//        cout<< binaryNumber[i];
//    }

//   int number;
//
//    cout << "Enter number: ";
//    cin >> number;
//
//    int tempNumber = abs(number);
//    int n = 0;
//
//    while (tempNumber > 0) {
//        tempNumber /= 3;
//        n++;
//    }
//
//    int* binaryNumber = new int [n];
//
//    tempNumber = number;
//
//    for (int i = n-1; i >= 0; --i) {
//        binaryNumber[i] = tempNumber % 3;
//
//        tempNumber /= 3;
//    }
//
//    cout << "Binary number: ";
//    for (int i = 0; i < n; i++) {
//        cout<< binaryNumber[i];
//    }

//    int a;
//    cout << "Enter a: ";
//    cin >> a;
//
//    int b;
//    cout << "Enter b: ";
//    cin >> b;
//
//    int gcd = 1;
//
//    while (a != 0 && b != 0) {
//        if(a > b) {
//             a %= b;
//        }else {
//            b %= a;
//        }
//    }
//
//        cout << "GCD: " << a+b;

    int i = 1;

    int a;
    cout << "Enter a: ";
    cin >> a;

    int b;
    cout << "Enter b: ";
    cin >> b;

    while (true) {

        if(i % a == 0 && i % b == 0) {
            break;
        }

        i++;
    }

    cout << "NOK: " << i;

    return 0;
}
