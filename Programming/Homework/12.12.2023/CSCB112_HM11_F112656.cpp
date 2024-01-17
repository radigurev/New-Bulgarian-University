#include <iostream>
#include <cmath>
using namespace std;

double power(double x, int n) {
    double result = 1;
    for (int i = 0; i < n; ++i) {
        result *= x;
    }
    return result;
}

int factorial(int n) {
    int result = 1;
    for (int i = 2; i <= n; ++i) {
        result *= i;
    }
    return result;
}

double seriesSum1(double x, int n) {
    double sum = 0;
    for (int i = 0; i <= n; ++i) {
        double term = power(x, 2 * i + 1) / factorial(2 * i + 1);
        if (i % 2 != 0) term = -term;
        sum += term;
    }
    return sum;
}

double seriesSum2(double x, int n) {
    double sum = 1;
    for (int i = 1; i <= n; ++i) {
        double term = power(x, 2 * i) / factorial(2 * i);
        if (i % 2 != 0) term = -term;
        sum += term;
    }
    return sum;
}

double calculateExpression(int n, int m, double a) {
    double sum = 0;
    for (int i = 1; i <= n; ++i) {
        for (int j = 1; j <= m; ++j) {
            sum += (a + j) / (double)(i + j);
        }
    }
    return sum;
}

int f(int x) {
    return x + 1;
}

int g(int x) {
    return x - 1;
}

void applyFunctionsToMatrix(int** matrix, int size, int (*funcF)(int), int (*funcG)(int)) {
    for (int i = 0; i < size; ++i) {
        for (int j = 0; j < size; ++j) {
            if ((i + j) % 2 == 0) {
                matrix[i][j] = funcF(matrix[i][j]);
            } else {
                matrix[i][j] = funcG(matrix[i][j]);
            }
        }
    }
}

int** createAndFillMatrix(int size) {
    int** matrix = new int*[size];
    for (int i = 0; i < size; ++i) {
        matrix[i] = new int[size];
        for (int j = 0; j < size; ++j) {
            cout << "Enter element [" << i << "][" << j << "]: ";
            cin >> matrix[i][j];
        }
    }
    return matrix;
}

void printMatrix(int** matrix, int size) {
    for (int i = 0; i < size; ++i) {
        for (int j = 0; j < size; ++j) {
            cout << matrix[i][j] << " ";
        }
        cout << endl;
    }
}

int main() {
    double x;
    int n;
    cout << "Task 1: Enter a real number x and a natural number n:" << endl;
    cin >> x >> n;
    cout << "x^n: " << power(x, n) << endl;
    cout << "n!: " << factorial(n) << endl;

    cout << "Task 2: Enter a real number x and a natural number n for series sums:" << endl;
    cin >> x >> n;
    cout << "Series Sum 1: " << seriesSum1(x, n) << endl;
    cout << "Series Sum 2: " << seriesSum2(x, n) << endl;

    int m;
    double a;
    cout << "Task 3: Enter natural numbers n, m, and a real number a:" << endl;
    cin >> n >> m >> a;
    cout << "Calculated Expression: " << calculateExpression(n, m, a) << endl;

    int matrixSize;
    cout << "Task 4: Enter the size of the square matrix: ";
    cin >> matrixSize;

    int** matrix = createAndFillMatrix(matrixSize);
    cout << "Original Matrix:" << endl;
    printMatrix(matrix, matrixSize);

    applyFunctionsToMatrix(matrix, matrixSize, f, g);
    cout << "Modified Matrix:" << endl;
    printMatrix(matrix, matrixSize);

    for (int i = 0; i < matrixSize; ++i) {
        delete[] matrix[i];
    }
    delete[] matrix;

    return 0;
}
