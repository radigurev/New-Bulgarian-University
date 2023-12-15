#include <iostream>
#include <stdlib.h>

using namespace std;

void printArray(int *arr, int sz) {
    for (int i = 0; i < sz; ++i) {
        cout << arr[i] << '\t';
    }
    cout << endl;
}

void newArray(int *arr, int sz) {
    for (int i = 0; i < sz; ++i) {
        arr[i] = rand() % 10;
    }
    cout << endl;
}

bool isMultiplicity(int *arr, int sz) {
    for (int i = 0; i < sz - 1; ++i)
        for (int j = i + 1; j < sz; ++j)
            if (arr[i] == arr[j]) return false;

    return true;
}

int fixMultiplicity(int *arr, int sz) {

    bool isRepeated = false;
    int *temp = new int[sz];
    int currentPosition = 0;

    for (int i = 0; i < sz; ++i) {
        isRepeated = false;
        for (int j = i + 1; j < sz; ++j) {
            if (arr[i] == arr[j]) {
                isRepeated = true;
                break;
            }
        }
        if (!isRepeated) {
            temp[currentPosition++] = arr[i];
        }
    }

    arr = new int[currentPosition];

    for (int i = 0; i < currentPosition; ++i) {
        arr[i] = temp[i];
    }

    return currentPosition;
}

void findArrayByBitMask(int arr[], char *mask, int sz) {
    for (int i = 0; i < sz; ++i) {
        if (mask[i] == '1') {
            cout << arr[i] << '\t';
        }
    }

    cout << endl;
}

void findMaskBySubMultiplicity(int arr[], int subMultiplicity[], int sz, int szSub) {
    bool isInSubMultiplicity;

    for (int i = 0; i < sz; ++i) {
        isInSubMultiplicity = false;
        for (int j = 0; j < szSub; ++j) {
            if (arr[i] == subMultiplicity[j]) {
                isInSubMultiplicity = true;
                break;
            }
        }
        cout << (isInSubMultiplicity ? "1" : "0");
    }

    cout << endl;
}

bool findElementInMultiplicity(int arr[], int element, int sz) {
    for (int i = 0; i < sz; ++i) {
        if (arr[i] == element) return true;
    }

    return false;
}

bool findIsItsSubMultiplicity(int arr[], int subMultiplicity[], int sz, int szMultiplicity) {

    bool notFound = true;

    for (int i = 0; i < szMultiplicity; ++i) {
        notFound = true;
        for (int j = 0; j < sz; ++j) {
            if (subMultiplicity[i] == arr[j]) {
                notFound = false;
                break;
            }
        }
        if (notFound) return false;
    }

    return true;
}

int main() {

    int n;

    do {
        cout << "Enter n(1-10): ";
        cin >> n;
    } while (n > 10 || n < 1);

    int *array = new int[n];

    newArray(array, n);

    cout << "Generated array:\t";
    printArray(array, n);

    bool multiplicity = isMultiplicity(array, n);

    cout << "The array is " << (multiplicity ? "" : "not") << " multiplicity: " << endl;

    if (!multiplicity) {
        n = fixMultiplicity(array, n);
        cout << "After fixing: " << endl;
        printArray(array, n);
    }

//    char *bitMask = new char[n + 1];
//
//    cout << "Bit mask(Length: " << n << "): ";
//    cin >> bitMask;
//
//    findArrayByBitMask(array, bitMask, n);
//
    int sizeOfSubMultiplicity;
//
    cout << "Enter size of sub multiplicity: ";
    cin >> sizeOfSubMultiplicity;
//
    int *subMultiplicity = new int[sizeOfSubMultiplicity];
//
//    for (int i = 0; i < sizeOfSubMultiplicity; ++i) {
//        cin >> subMultiplicity[i];
//    }
//
//    findMaskBySubMultiplicity(array, subMultiplicity, n, sizeOfSubMultiplicity);
//
//    int elementToSearch;
//
//    cout << "Enter element to search for: ";
//    cin >> elementToSearch;
//
//    cout << elementToSearch << " is "
//         << (findElementInMultiplicity(array, elementToSearch, n) ? "" : "not")
//         << " in the multiplicity";
//
//    cout << "Enter size of sub multiplicity: ";
//    cin >> sizeOfSubMultiplicity;
//
//    subMultiplicity = new int[sizeOfSubMultiplicity];
//
//    for (int i = 0; i < sizeOfSubMultiplicity; ++i) {
//        cin >> subMultiplicity[i];
//    }
//
//    cout << "The sub multiplicity is "
//    << (findIsItsSubMultiplicity(array,subMultiplicity,n, sizeOfSubMultiplicity) ? "" : "not ")
//    << "a sub multiplicity of: ";
//    printArray(array, n);

    int sizeOfSubSubMultiplicity;

    cout << "Enter size of A: ";
    cin >> sizeOfSubMultiplicity;

    cout << "Enter size of B: ";
    cin >> sizeOfSubSubMultiplicity;

    subMultiplicity = new int[sizeOfSubMultiplicity];

    int *subSubMultiplicity = new int[sizeOfSubMultiplicity];

    cout << "Enter sub Multiplicity A: ";

    for (int i = 0; i < sizeOfSubMultiplicity; ++i) {
        cin >> subMultiplicity[i];
    }

    cout << "Enter sub Multiplicity B: ";

    for (int i = 0; i < sizeOfSubSubMultiplicity; ++i) {
        cin >> subSubMultiplicity[i];
    }

    cout << "The sub multiplicity is "
         << (findIsItsSubMultiplicity(subMultiplicity, subSubMultiplicity, sizeOfSubMultiplicity, sizeOfSubSubMultiplicity) ? "" : "not ")
         << "a sub multiplicity of: ";
    printArray(array, n);

    return 0;
}
