#include <iostream>
#include <cstring>
#include <iomanip>

using namespace std;
//task 1, 2
const int maxSize = 100;
const int maxSize10 = 10;

void displayArray(int numbers[], int n) {
    for (int i = 0; i < n; ++i) {
        std::cout << numbers[i] << " ";
    }
    std::cout << std::endl;
}

void deleteElement(int numbers[], int &n, int index) {
    for (int i = index; i < n - 1; ++i) {
        numbers[i] = numbers[i + 1];
    }
    n--;
}

void addElement(int numbers[], int &size, int element) {
    numbers[size++] = element;
}

void addElementAtPosition(int numbers[], int &n, int element, int position) {
    for (int i = n; i > position; --i) {
        numbers[i] = numbers[i - 1];
    }
    numbers[position] = element;
    n++;
}

void changeElementValue(int numbers[], int &n, int index, int newValue) {
    numbers[index] = newValue;
}

void deleteStudent(int numbers[], int &n, int newValue) {
    for (int i = 0; i < n; ++i) {
        if (numbers[i] == newValue)
            deleteElement(numbers, n, i);
    }
}

void findElement(int numbers[], int n, int search) {
    for (int i = 0; i < n; ++i) {
        if (numbers[i] == search) {
            cout << "Found " << search << " at index: " << i;
            return;
        }
    }
    cout << search << " not found.";
}

void changeStudent(int number[], int n, int oldNumber, int newNumber) {
    for (int i = 0; i < n; ++i) {
        if (number[i] == oldNumber) {
            changeElementValue(number, n, i, newNumber);
        }
    }
}
//end

//task 3
void carriagesToPass(int n, int m) {
    const char composition[] = "LBOPKRKSKPMRKKPP";
    const int totalCarriages = sizeof(composition) - 1;
    const char targetCarriage = composition[m - 1];

    std::cout << "Carriages to pass from " << n << " to " << m << ": " << std::endl;

    for (int i = n; i < m; ++i) {
        if (composition[i - 1] != targetCarriage) {
            std::cout << "Carriage " << i << " (" << composition[i - 1] << ")" << std::endl;
        }
    }
}
//end

//task4
void fillZigZag(int **arr, int rows, int cols) {
    int value = 1;
    for (int i = 0; i < rows; ++i) {
        if (i % 2 == 0) {
            for (int j = 0; j < cols; ++j) {
                arr[i][j] = value++;
            }
        } else {
            for (int j = cols - 1; j >= 0; --j) {
                arr[i][j] = value++;
            }
        }
    }
}

void printTwoDimensionalArray(int **arr, int rows, int cols) {
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            std::cout << arr[i][j] << "\t";
        }
        std::cout << std::endl;
    }
}
//end

//task 5
void deleteDigits(char* str) {
    int len = strlen(str);
    int readPtr = 0;
    int writePtr = 0;

    while (readPtr < len) {
        if (str[readPtr] < '0' || str[readPtr] > '9') {
            str[writePtr++] = str[readPtr];
        }
        readPtr++;
    }

    str[writePtr] = '\0';
}
//task 6

void replaceSubstring(char* str, const char* oldSubstr, const char* newSubstr) {
    int oldSubstrLen = strlen(oldSubstr);
    int newSubstrLen = strlen(newSubstr);
    int strLen = strlen(str);

    char* temp = new char[strLen * 2 + 1];
    temp[0] = '\0';

    char* pos = str;
    char* next;

    while ((next = strstr(pos, oldSubstr)) != nullptr) {
        strncat(temp, pos, next - pos);
        strcat(temp, newSubstr);
        pos = next + oldSubstrLen;
    }

    strcat(temp, pos);

    strcpy(str, temp);

    delete[] temp;
}

//task 7

int countIdenticalCharacters(const char* str1, const char* str2) {
    int count = 0;
    int len1 = strlen(str1);
    int len2 = strlen(str2);

    int smallerLength = (len1 < len2) ? len1 : len2;

    for (int i = 0; i < smallerLength; ++i) {
        if (str1[i] == str2[i]) {
            count++;
        }
    }

    return count;
}

//task 8
bool charExists(char* str, char c) {
    while (*str != '\0') {
        if (*str == c) {
            return true;
        }
        str++;
    }
    return false;
}

int countDistinctCharacters( char* str1,  char* str2) {
    int count = 0;
    int len1 = strlen(str1);
    int len2 = strlen(str2);

    for (int i = 0; i < len1; ++i) {
        if (!charExists(str2, str1[i])) {
            count++;
        }
    }
    for (int i = 0; i < len2; ++i) {
        if (!charExists(str1, str2[i])) {
            count++;
        }
    }

    return count;
}

//task 9
int minPartition(int weights[], int n) {
    int totalSum = 0;
    for (int i = 0; i < n; ++i) {
        totalSum += weights[i];
    }

    bool dp[n + 1][totalSum + 1];

    for (int i = 0; i <= n; ++i) {
        dp[i][0] = true;
    }

    for (int i = 1; i <= totalSum; ++i) {
        dp[0][i] = false;
    }

    for (int i = 1; i <= n; ++i) {
        for (int j = 1; j <= totalSum; ++j) {
            dp[i][j] = dp[i - 1][j];
            if (weights[i - 1] <= j) {
                dp[i][j] = dp[i][j] || dp[i - 1][j - weights[i - 1]];
            }
        }
    }

    int minDiff = 0;

    for (int j = totalSum / 2; j >= 0; --j) {
        if (dp[n][j]) {
            minDiff = totalSum - 2 * j;
            break;
        }
    }

    return minDiff;
}

//task 10

void fillArray(float arr[maxSize10][maxSize10], int rows, int cols) {
    float value = 3.33;
    float increment = 10.0;

    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            arr[i][j] = value;
            value += increment;
            if (value > 333.33) {
                value = 3.33;
            }
        }
    }
}

// Function to find elements in even order and equal to the maximum product of array elements by columns
void findMaxProductElements(float arr[maxSize10][maxSize10], int rows, int cols) {
    float maxProduct = 1.0;
    for (int j = 0; j < cols; ++j) {
        float product = 1.0;
        for (int i = 0; i < rows; ++i) {
            product *= arr[i][j];
        }
        if (j % 2 == 0 && product >= maxProduct) {
            maxProduct = product;
        }
    }
    std::cout << "Maximum product of elements in even order: " << maxProduct << std::endl;
}

// Function to form a new matrix by calculating the difference between elements of two matrices
void diffBetweenMatrices(float arr1[maxSize10][maxSize10], float arr2[maxSize10][maxSize10], float result[maxSize10][maxSize10], int rows, int cols) {
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            result[i][j] = arr1[i][j] - arr2[i][j];
        }
    }
}

// Function to replace elements below the secondary diagonal with 0
void replaceBelowSecondaryDiagonalWithZero(float arr[maxSize10][maxSize10], int rows, int cols) {
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            if (i + j > rows - 1) {
                arr[i][j] = 0.0;
            }
        }
    }
}

// Function to fill the array with values of the unit matrix
void fillUnitMatrix(float arr[maxSize10][maxSize10], int rows, int cols) {
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            if (i == j) {
                arr[i][j] = 1.0;
            } else {
                arr[i][j] = 0.0;
            }
        }
    }
}

// Function to multiply two matrices
void multiplyMatrices(float arr1[maxSize10][maxSize10], float arr2[maxSize10][maxSize10], float result[maxSize10][maxSize10], int rows1, int cols1, int rows2, int cols2) {
    for (int i = 0; i < rows1; ++i) {
        for (int j = 0; j < cols2; ++j) {
            result[i][j] = 0;
            for (int k = 0; k < cols1; ++k) {
                result[i][j] += arr1[i][k] * arr2[k][j];
            }
        }
    }
}

void displayMatrix(float arr[maxSize10][maxSize10], int rows, int cols) {
    std::cout << "Matrix:" << std::endl;
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            std::cout << std::fixed << std::setprecision(2) << arr[i][j] << " ";
        }
        std::cout << std::endl;
    }
}


int main() {
    int n, command, command2;
//    cout << "Enter n: ";
//    cin >> n;
//
//    int *numbers = new int[n];
//    cout << "Enter array: " << endl;
//    for (int i = 0; i < n; ++i) {
//        std::cin >> numbers[i];
//    }
//
//    std::cout << "Original array: ";
//    displayArray(numbers, n);
//
//    cout << "Choose index to delete: ";
//    cin >> command;
//    deleteElement(numbers, n, command);
//    std::cout << "array after deleting element: ";
//    displayArray(numbers, n);
//
//    cout << "Choose number to add: ";
//    cin >> command;
//    addElement(numbers, n, command);
//    std::cout << "array after new element: ";
//    displayArray(numbers, n);
//
//    cout << "Choose number to add: ";
//    cin >> command;
//    cout << "Choose number to add at position: ";
//    cin >> command2;
//    addElementAtPosition(numbers, n, command, command2);
//    std::cout << "array after new element: ";
//    displayArray(numbers, n);
//
//    cout << "Choose number to add: ";
//    cin >> command;
//    cout << "Choose number to change at position: ";
//    cin >> command2;
//    changeElementValue(numbers, n, command, command2);
//    std::cout << "array after changing element: ";
//    displayArray(numbers, n);
//
//    delete[] numbers;
//
    //task 2
//    int facultyNumbers[maxSize];
//
//    cout << "Enter n: ";
//    cin >> n;
//
//    cout << "Enter array with faculty numbers: ";
//    for (int i = 0; i < n; ++i) {
//        cin >> facultyNumbers[i];
//    }
//
//    cout << "Enter new faculty number: ";
//    cin >> command;
//    addElement(facultyNumbers, n, command);
//    displayArray(facultyNumbers, n);
//
//    cout << "Delete student faculty: ";
//    cin >> command;
//    deleteStudent(facultyNumbers, n, command);
//    displayArray(facultyNumbers, n);
//
//    cout << "Change faculty number: ";
//    cin >> command >> command2;
//    changeStudent(facultyNumbers, n, command, command2);
//    displayArray(facultyNumbers, n);
//
//    cout << "Search for student: ";
//    cin >> command;
//    findElement(facultyNumbers, n, command);
//    cout << endl;
//    displayArray(facultyNumbers,n);
//    delete[] facultyNumbers;

//task 3
//    int startCarriage, endCarriage;
//
//    cout << "Enter start: ";
//    cin >> startCarriage;
//
//    cout << "Enter end: ";
//    cin >> endCarriage;
//
//    carriagesToPass(startCarriage, endCarriage);

//task 4
//    int rows, cols;
//
//    std::cout << "Enter number of rows: ";
//    std::cin >> rows;
//    std::cout << "Enter number of columns: ";
//    std::cin >> cols;
//
//    int **arr = new int *[rows];
//    for (int i = 0; i < rows; ++i) {
//        arr[i] = new int[cols];
//    }
//
//    fillZigZag(arr, rows, cols);
//
//    std::cout << "Array filled in a zigzag pattern:" << std::endl;
//    printTwoDimensionalArray(arr,rows,cols);
//
//    for (int i = 0; i < rows; ++i) {
//        delete[] arr[i];
//    }
//    delete[] arr;

//task 5

//    cout << "Enter n: ";
//    cin >> n;
//    char letter[n];
//
//    cout << "Enter letter (max " << n << " characters): ";
//    cin >> letter;
//
//    deleteDigits(letter);
//
//    cout << "After deleting digits: " << letter;

//task 6

//    char sentence[maxSize];
//    char oldSubstring[maxSize];
//    char newSubstring[maxSize];
//
//    std::cout << "Enter a sentence: ";
//    std::cin.getline(sentence, maxSize);
//
//    std::cout << "Enter old substring : ";
//    std::cin.getline(oldSubstring, maxSize);
//
//    std::cout << "Enter  new substring: ";
//    std::cin.getline(newSubstring, maxSize);
//
//    replaceSubstring(sentence,oldSubstring,newSubstring);
//
//    cout << "Sentence: " << sentence;

//task 7

//    char sentenceOne[maxSize];
//    char sentenceTwo[maxSize];
//
//    std::cout << "Enter the first sentence: ";
//    std::cin.getline(sentenceOne, maxSize);
//
//    std::cout << "Enter the second sentence: ";
//    std::cin.getline(sentenceTwo, maxSize);
//
//    cout << "Count: " << countIdenticalCharacters(sentenceOne, sentenceTwo) << endl;

//task 8

//    char sentenceOne[maxSize];
//    char sentenceTwo[maxSize];
//
//    std::cout << "Enter the first sentence: ";
//    std::cin.getline(sentenceOne, maxSize);
//
//    std::cout << "Enter the second sentence: ";
//    std::cin.getline(sentenceTwo, maxSize);
//
//    cout << "Count: " << countDistinctCharacters(sentenceOne, sentenceTwo) << endl;

//task 9
//    int weights[maxSize];
//
//    std::cout << "Enter n: ";
//    std::cin >> n;
//
//    std::cout << "Enter weights:" << std::endl;
//    for (int i = 0; i < n; ++i) {
//        std::cin >> weights[i];
//    }
//
//    int minDifference = minPartition(weights, n);
//    std::cout << "The minimum difference is: " << minDifference << std::endl;

//task 10
    int rows, cols;

    std::cout << "Enter rows and columns (max 10): ";
    std::cin >> rows >> cols;

    float matrix[maxSize10][maxSize10];

    fillArray(matrix, rows, cols);

    findMaxProductElements(matrix, rows, cols);

    float diffMatrix[maxSize10][maxSize10];
    fillArray(diffMatrix, rows, cols);
    diffBetweenMatrices(matrix, matrix, diffMatrix, rows, cols);

    replaceBelowSecondaryDiagonalWithZero(matrix, rows, cols);

    float unitMatrix[maxSize10][maxSize10];
    fillUnitMatrix(unitMatrix, rows, cols);

    float resultMatrix[maxSize10][maxSize10];
    multiplyMatrices(matrix, unitMatrix, resultMatrix, rows, cols, rows, cols);

    std::cout << "\nResults:\n";
    displayMatrix(diffMatrix, rows, cols);
    displayMatrix(matrix, rows, cols);
    displayMatrix(resultMatrix, rows, cols);

    return 0;
}