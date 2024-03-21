#include <iostream>

using namespace std;

int main()
{
//    int firstNumber, secondNumber, temp;
//
//    cout << "First Number: ";
//    cin >> firstNumber;
//
//    cout << "Second Number: ";
//    cin >> secondNumber;
//
//start:
//    temp = firstNumber % secondNumber;
//    if (temp == 0)
//    {
//        goto finish;
//    }
//    else
//    {
//        firstNumber = secondNumber;
//        secondNumber = temp;
//        goto start;
//    }
//
//finish:
//    cout << "GCD: " << secondNumber;

    const int n = 4;

    int *arr = new int[n];
    int *arr2 = new int[n];

    int i;
    for (i = 0; i < n; i++) {
        cout << "Enter arr[" << (i + 1) << "]: ";
        cin >> arr[i];
    }

    for (i = 0; i < n; i++) {
        cout << "Enter arr2[" << (i + 1) << "]: ";
        cin >> arr2[i];
    }

    int arr3[n+1];

    int temp = 0;
    for (i = n; i > 0; i--) {
        int s = arr[i-1] + arr2[i-1] + temp;

        arr3[i] = s % 10;
        temp = s / 10;
    }
    arr3[i] = temp;

    for (i = 0; i <= n; i++) {
        cout << arr3[i] << ", ";
    }
}