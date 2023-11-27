#include <iostream>
#include <cstring>

using namespace std;

int main() {
    int n;
//1.
    int startindex;
    bool isPalindrom= true;

    do {
        cout << "Enter n(n < 20): ";
        cin >> n;
    } while (n > 19 || n < 0);

    char* word = new char[n];

    cin >> word;

    for (int i = n-1; i >= 0; --i) {
        startindex = n-i-1;

        if(word[i] != word[startindex])  {
            isPalindrom = false;
            break;
        }
    }

    if(isPalindrom)
        cout << "Is palindrom";
    else
        cout << "Is not plaindrom";

//2.
    n = 0;

    bool isMonotonicallyDecreasing = true;

    do {
        cout << "Enter n(n < 10): ";
        cin >> n;
    } while (n > 10 || n < 0);

    char *oldWord = new char[n];
    char *newWord = new char[n];

    cin >> oldWord;
    for (int i = 0; i < n - 1; ++i) {
        cin >> newWord;
        if (strcmp(oldWord, newWord) < 0) {
            isMonotonicallyDecreasing = false;
            break;
        }
        oldWord = newWord;
    }

    if(isMonotonicallyDecreasing)
        cout << "The array is monotonically decreasing" << endl;
    else
        cout << "The array is not monotonically decreasing" << endl;

//3.
    n = 0;

    int symmetricRowIndex = 0;
    int symmetricColIndex = 0;

    bool isSymmetrical = true;

    do {
        cout << "Enter n(n < 9): ";
        cin >> n;
    } while (n > 9 || n < 0);

    int **matrix = new int *[n];

    for (int i = 0; i < n; ++i) {
        matrix[i] = new int[n];
        for (int j = 0; j < n; ++j) {
            cin >> matrix[i][j];
        }
    }

    for (int i = 0; i < n; ++i) {
        symmetricRowIndex = n - i - 1;
        for (int j = 0; j < n; ++j) {
            if (i == j) continue;

            symmetricColIndex = n - j - 1;
            if (matrix[i][j] != matrix[symmetricRowIndex][symmetricColIndex]) {
                isSymmetrical = false;
                break;
            }
        }

        if(!isSymmetrical) break;
    }

    if(isSymmetrical)
        cout << "The matrix is symmetrical";
    else
        cout << "The matrix is not symmetrical";

    cout << endl;

//4.
    n = 0;
    cin >> n;

    char* number = new char[10];

    int sum = 0;

    for (int i = 0; i < n; ++i) {
        cin >> number;
        sum += stoi(number);
    }

    cout << "Average: " << (double) sum / n << endl;

//5.

    do {
        cout << "Enter n (1 < n < 20): ";
        cin >> n;
    } while (n > 20 || n < 1);

    char matrixOfWords[7][n*n+1];
    for (int i = 0; i < n*n; i++) {
            cin >> matrixOfWords[i];
    }

    char *s = new char[7];
    cout << "Enter the word to search for: ";
    cin >> s;

    int index = 0;

    bool found = false;
    for (int i = 0; i < n; i++) {
        for (int j = n - i - 1; j >= 0; j--) {
            index = n * i + j;
            if (strcmp(matrixOfWords[index], s) == 0) {
                found = true;
                break;
            }
        }
        if (found) break;
    }

    if (found) {
        cout << "The word '" << s << "' is above or on the secondary main diagonal." << endl;
    } else {
        cout << "The word '" << s << "' is not above or on the secondary main diagonal." << endl;
    }

    do {
        cout << "Enter n (1 < n < 20): ";
        cin >> n;
    } while (n > 20 || n < 1);

    char arr[n + 1][n + 1][7];

    bool match = true;

    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            cin >> arr[i][j];
        }
    }

    for (int i = 0; i < n; i++) {
        if (strcmp(arr[n - i - 1][n - i - 1], arr[i][n - i - 1]) != 0) {
            match = false;
        }
    }

    if (match) {
        cout << "The diagonals match.\n";
    } else {
        cout << "The diagonals do not match.\n";
    }
}
