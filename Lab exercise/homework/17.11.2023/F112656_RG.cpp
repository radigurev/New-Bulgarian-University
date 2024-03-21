#include <iostream>

using namespace std;

int main() {
//    int x,y;
//    
//    cin >> x >> y;
//
//    if(x > 16 || x < 2 || y < 1 || y > 100) {
//        cout << "Invalid input data!";
//        return 0;
//    }
//
//    int n = 0;
//
//    int tempNumber = y;
//    while (tempNumber > 0) {
//        tempNumber /= x;
//        n++;
//    }
//
//    char* binaryNumber = new char[n];
//
//    tempNumber = y;
//
//    int tempDigit = 0;
//
//    for (int i = n-1; i >= 0; --i) {
//        tempDigit = tempNumber % x;
//        switch (tempDigit) {
//            case 10:  binaryNumber[i] = 'A';
//                break;
//            case 11:  binaryNumber[i] = 'B';
//                break;
//            case 12:  binaryNumber[i] = 'C';
//                break;
//            case 13:  binaryNumber[i] = 'D';
//                break;
//            case 14:  binaryNumber[i] = 'E';
//                break;
//            case 15:  binaryNumber[i] = 'F';
//                break;
//            case 16:  binaryNumber[i] = 'G';
//                break;
//            default: binaryNumber[i] = tempDigit + '0';
//        }
//        tempNumber /= x;
//    }
//
//    for (int i = 0; i < n; i++) {
//        cout<< binaryNumber[i];
//    }
//    

//    int x, y, z;
//    cin >> x >> y >> z;
//
//    if ((x < 20 || x > 300) || (y < 20 || y > 300) || (z < 20 || z > 300)) {
//        cout << "Invalid input data!";
//        return 0;
//    }
//
//    int tempx = x, tempy = y;
//    while (tempx != tempy) {
//        if (tempx > tempy)
//            tempx -= tempy;
//        else
//            tempy -= tempx;
//    }
//    int gcd = tempx;
//
//    tempy = z;
//    while (tempx != tempy) {
//        if (tempx > tempy)
//            tempx -= tempy;
//        else
//            tempy -= tempx;
//    }
//    int result = tempx;
//
//    cout << result;

//    int x, y, z;
//
//    cin >> x >> y >> z;
//
//    if ((x < 10 || x > 100) || (y < 10 || y > 100) || (z < 10 || z > 100)) {
//        cout << "Invalid input data!";
//        return 0;
//    }
//
//    int tempx = x, tempy = y;
//    int tempXY = 0;
//
//    while (tempy != 0) {
//        tempXY = tempy;
//        tempy = tempx % tempy;
//        tempx = tempXY;
//    }
//    int gcd = tempx;
//
//    tempy = z;
//    while (tempy != 0) {
//        tempXY = tempy;
//        tempy = tempx % tempy;
//        tempx = tempXY;
//    }
//    int gcdXYZ = tempx;
//
//    int lcm = (x * y) / gcd;
//    int lcmXYZ = (lcm * z) / gcdXYZ;
//
//    cout << lcmXYZ;

    int x, y, z;
    cin >> x >> y >> z;

    if (x < 10 || x > 100 || y < 10 || y > 100 || z < 10 || z > 100) {
        cout << "Invalid input data!";
        return 0;
    }

    int tempx = x, tempy = y, tempz = z;
    int gcdAB, gcdBC, lcmAB, result;

    while (tempy != 0) {
        int temp = tempy;
        tempy = tempx % tempy;
        tempx = temp;
    }
    gcdAB = tempx;
    lcmAB = (x * y) / gcdAB;

    tempx = lcmAB, tempy = z;

    while (tempy != 0) {
        int temp = tempy;
        tempy = tempx % tempy;
        tempx = temp;
    }
    gcdBC = tempx;
    result = (lcmAB * z) / gcdBC;

    cout << result;


    return 0;
}
