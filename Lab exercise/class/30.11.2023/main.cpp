#include <iostream>

using namespace std;

int main() {

    int a, b;

    int rows = 4, columnLength = -1;

    do {
        cout << "Enter a: ";
        cin >> a;
    } while (a == 0);

    do {
        cout << "Enter b: ";
        cin >> b;
    } while (b == 0);

    int **matrix = new int *[4];

    int tempX = 0;
    int tempY = 0;

    if (b > a) {
        a = a ^ b;
        b = a ^ b;
        a = a ^ b;
    }

    tempX = a;
    tempY = b;

    int tmp = 0;

    while (tempX > 0) {
        tempX -= tempY;
        tmp = tempY;
        tempY = tempX;
        tempX = tmp;
        columnLength++;
    }

    if (columnLength == 0) return 0;

    for (int i = 0; i < rows; ++i) {
        matrix[i] = new int[columnLength];
    }

    matrix[0][0] = a;
    matrix[0][1] = b;

    matrix[rows - 1][0] = 0;
    matrix[rows - 1][1] = 1;

    matrix[rows - 2][0] = 1;
    matrix[rows - 2][1] = 0;

    tempX = a;
    tempY = b;


    int rowIndex = 1;
    for (int i = 2; i <= columnLength; ++i) {

        matrix[rowIndex][i - 1] = (int) tempX / tempY;

        int xx = matrix[rows - 3][i - 1] * matrix[rows - 2][i - 1] + matrix[rows - 2][i - 2];

        int xy = matrix[rows - 3][i - 1] * matrix[rows - 1][i - 1] + matrix[rows - 1][i - 2];


        if (i == columnLength) break;

        matrix[rows - 2][i] = xx;

        matrix[rows - 1][i] = xy;

        tempX -= tempY;
        tmp = tempX;

        matrix[0][i] = tmp;

        tempX = tempY;
        tempY = tmp;

    }

//    for (int i = 0; i < rows; ++i) {
//        for (int j = 0; j < columnLength; ++j) {
//            cout << matrix[i][j] << " ";
//        }
//        cout << endl;
//    }

    for (int i = 0; i < columnLength; ++i) {
        cout << matrix[0][i] << " = " << pow(-1, i) << " * "
        << a << " * " << matrix[rows - 2][i]
             << " + " << pow(-1, i + 1)
             << " * " << matrix[rows - 1][i] << " * " << b << endl;
    }
    return 0;
}
