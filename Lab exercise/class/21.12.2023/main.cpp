#include <iostream>
#include <stdlib.h>
#include <math.h>

using namespace std;

void fillArray(int *, int);

void printArray(int *, int);

void toBinary(char *, int, int);

void getBitmask(char **, int);

void printBitmasks(char **, int);

void printAllSubArray(int *, int, int = 0, int = 1);

int main() {

    int n;

    cin >> n;

    int *array = new int[n]();

    fillArray(array, n);

    printArray(array, n);

    printAllSubArray(array, n);

//    char **masks = new char *[n];
//
//    getBitmask(masks, n);
//
//    printBitmasks(masks, n);

    return 0;
}

void fillArray(int *array, int n) {
    int min = 1;
    int max = 10000;
    for (int i = 0; i < n; ++i) {
        array[i] = min + (rand() % (max - min + 1));
    }
}

void printArray(int *array, int n) {
    for (int i = 0; i < n; ++i) {
        cout << array[i] << "\t";
    }
    cout << endl;
}

char *toBinary(char *bin, int &number, int &power, int originalPower) {
    int tmp;

    if (power < 0) return bin;

    tmp = (number >> (power)) & 1;

    bin[originalPower - power] = (char) (tmp + 48);

    power--;
    return toBinary(bin, number, power, originalPower);
}

void getBitmask(char **bitMasks, int power) {
    int rows = pow(2, power);

    int j, tempPower;

    for (int i = 0; i < rows; i++) {
        tempPower = power;
        bitMasks[i] = new char[power + 1];
        j = i;
        bitMasks[i] = toBinary(bitMasks[i], j, tempPower, power);
    }
}

void printBitmasks(char **bitMasks, int power) {
    for (int i = 0; i < pow(2, power); ++i) {
        cout << bitMasks[i] << endl;
    }
}

void printAllSubArray(int *array, int n, int i, int j) {

    if (i == n - 1) return;

    if (j == n) {
        printAllSubArray(array, n, i + 1, i + 2);
        return;
    }

    cout << array[i]  << " " << array[j++] << endl;

    printAllSubArray(array, n, i, j);
}