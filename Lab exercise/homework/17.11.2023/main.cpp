#include <iostream>

using namespace std;
int main() {
    int x,y;
    
    cin >> x >> y;

    if(x > 16 || x < 2 || y < 1 || y > 100) {
        cout << "Invalid input data!";
        return 0;
    }

    int n = 0;

    int tempNumber = y;
    while (tempNumber > 0) {
        tempNumber /= x;
        n++;
    }

    char* binaryNumber = new char[n];

    tempNumber = y;

    int tempDigit = 0;

    for (int i = n-1; i >= 0; --i) {
        tempDigit = tempNumber % x;
        switch (tempDigit) {
            case 10:  binaryNumber[i] = 'A';
                break;
            case 11:  binaryNumber[i] = 'B';
                break;
            case 12:  binaryNumber[i] = 'C';
                break;
            case 13:  binaryNumber[i] = 'D';
                break;
            case 14:  binaryNumber[i] = 'E';
                break;
            case 15:  binaryNumber[i] = 'F';
                break;
            case 16:  binaryNumber[i] = 'G';
                break;
            default: binaryNumber[i] = tempDigit + '0';
        }
        tempNumber /= x;
    }

    for (int i = 0; i < n; i++) {
        cout<< binaryNumber[i];
    }
    
    return 0;
}
