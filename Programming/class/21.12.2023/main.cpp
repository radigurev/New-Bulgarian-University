#include <iostream>

using namespace std;

int recursiveFibonacci(int &firstNumber, int &secondNumber, int timesToSum, int repeated) {

    if (timesToSum <= 1) return secondNumber;
    if (repeated >= timesToSum) return secondNumber;

    repeated++;
    firstNumber += secondNumber;
    firstNumber = firstNumber ^ secondNumber;
    secondNumber = firstNumber ^ secondNumber;
    firstNumber = firstNumber ^ secondNumber;

    return recursiveFibonacci(firstNumber, secondNumber, timesToSum, repeated);
}

int recursiveMathBug(int &firstNumber) {
    if (firstNumber == 1) return firstNumber;

    if (firstNumber % 2 == 0) {
        firstNumber /= 2;
    } else {
        firstNumber *= 3;
        firstNumber++;
    }

    cout << firstNumber << endl;

    return recursiveMathBug(firstNumber);
}

int findYourOtherHalf(char** matrix,int& currentPosition,int otherHalfPosition) {


}

int main() {

    int firstNumber = 0;
//    int secondNumber = 1;
//    int timesRepeated = 1;
//
//    int timesToSum;
//
//    cin >> timesToSum;
//
//    cout << recursiveFibonacci(firstNumber, secondNumber, timesToSum, timesRepeated);

//    cin >> firstNumber;
//
//    recursiveMathBug(firstNumber);

    char you, her;
    int n;

    cin >> n;
    cin >> you >> her;

    char** matrix = new char * [n+1];

    cin.ignore();

    for (int i = 0; i < n; ++i) {
        matrix[i] = new char[n+1];

        cin.getline(matrix[i],n+1);
    }

//    for (int i = 0; i < n; ++i) {
//        for (int j = 0; j < n; ++j) {
//            cout << matrix[i][j];
//        }
//        cout << endl;
//    }
    int i = 0;


    return 0;
}
