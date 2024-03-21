#include <iostream>
#include <cstring>
using namespace std;

const int MAX_CITIES = 20;

//task1
int findMax(const int arr[], int size) {
    int max = arr[0];
    for (int i = 1; i < size; ++i)
        if (arr[i] > max) max = arr[i];
    return max;
}

int findMin(const int arr[], int size) {
    int min = arr[0];
    for (int i = 1; i < size; ++i)
        if (arr[i] < min) min = arr[i];
    return min;
}
//task2
void replacePositive(int* arr, int size, int a, int b, int c) {
    for (int i = 0; i < size; ++i) {
        if (arr[i] > 0)
            arr[i] = 1 * a + 2 * b + 3 * c;
    }
}

//task3
void sortArray(int* arr, int size, bool ascending) {
    for (int i = 0; i < size - 1; ++i) {
        for (int j = 0; j < size - i - 1; ++j) {
            if (ascending ? arr[j] > arr[j + 1] : arr[j] < arr[j + 1]) {
                // Swap
                int temp = arr[j];
                arr[j] = arr[j + 1];
                arr[j + 1] = temp;
            }
        }
    }
}

//task4
void recursiveStringCopy(char* destination, const char* source) {
    if (*source == '\0') {
        *destination = '\0';
        return;
    }
    *destination = *source;
    recursiveStringCopy(destination + 1, source + 1);
}

//task5
int lexicographicCompare(unsigned int num1, unsigned int num2) {
    char str1[20], str2[20];
    sprintf(str1, "%u", num1);
    sprintf(str2, "%u", num2);
    return strcmp(str1, str2);
}

//task6
int stringToNumber(const char* str) {
    if (*str == '\0') return 0;
    return (str[0] - '0') + 10 * stringToNumber(str + 1);
}

//task7
void replaceCharacter(char* str, char x, char y) {
    if (*str == '\0') return;
    if (*str == x) *str = y;
    replaceCharacter(str + 1, x, y);
}

//task8
bool findRegionHelper(int** grid, int m, int n, int x, int y, int S, int currentSum) {
    if (x < 0 || y < 0 || x >= m || y >= n) return false;
    currentSum += grid[x][y];

    if (currentSum == S) return true;
    if (currentSum > S) return false;

    return findRegionHelper(grid, m, n, x + 1, y, S, currentSum) ||
           findRegionHelper(grid, m, n, x, y + 1, S, currentSum) ||
           findRegionHelper(grid, m, n, x + 1, y + 1, S, currentSum);
}

bool findRegionWithSum(int** grid, int m, int n, int S) {
    for (int i = 0; i < m; ++i) {
        for (int j = 0; j < n; ++j) {
            if (findRegionHelper(grid, m, n, i, j, S, 0)) return true;
        }
    }
    return false;
}

//task9
bool isValidMove(int x, int y, int m, int n) {
    return (x >= 0 && x < m && y >= 0 && y < n);
}

bool canFormTextHelper(char** board, int m, int n, int x, int y, const string& text, int index) {
    if (index == text.length()) return true;
    if (!isValidMove(x, y, m, n)) return false;
    if (board[x][y] != text[index]) return false;

    int knightMoves[8][2] = {{-2, -1}, {-1, -2}, {1, -2}, {2, -1},
                             {-2, 1}, {-1, 2}, {1, 2}, {2, 1}};

    for (auto& move : knightMoves) {
        int nextX = x + move[0];
        int nextY = y + move[1];
        if (canFormTextHelper(board, m, n, nextX, nextY, text, index + 1))
            return true;
    }

    return false;
}

bool canFormText(char** board, int m, int n, const string& text, int startX, int startY) {
    return canFormTextHelper(board, m, n, startX, startY, text, 0);
}

//task10
char* deleteFirst(char* str, const char* findWhat) {
    int n = strlen(str);
    int m = strlen(findWhat);

    for (int i = 0; i <= n - m; i++) {
        int j;
        for (j = 0; j < m; j++)
            if (str[i + j] != findWhat[j])
                break;

        if (j == m) {
            for (int k = i; k <= n - m; k++)
                str[k] = str[k + m];
            break;
        }
    }

    return str;
}

//task 11
char* deleteLast(char* str, const char* findWhat) {
    int n = strlen(str);
    int m = strlen(findWhat);
    int lastOccur = -1;

    for (int i = 0; i <= n - m; i++) {
        int j;
        for (j = 0; j < m; j++)
            if (str[i + j] != findWhat[j])
                break;

        if (j == m)
            lastOccur = i;
    }

    if (lastOccur != -1) {
        for (int i = lastOccur; i < n - m; i++)
            str[i] = str[i + m];
        str[n - m] = '\0';
    }

    return str;
}

//task12
char* deleteAll(char* str, const char* findWhat) {
    int n = strlen(str);
    int m = strlen(findWhat);
    int newLength = n;

    for (int i = 0; i < n; i++) {
        int j;
        for (j = 0; j < m && i + j < n; j++)
            if (str[i + j] != findWhat[j])
                break;

        if (j == m) {
            newLength -= m;
            for (int k = i; k < newLength; k++)
                str[k] = str[k + m];
            i--;
        }
    }
    str[newLength] = '\0';

    return str;
}

//task13
char* common(char* s1, char* s2) {
    int len1 = strlen(s1), len2 = strlen(s2);
    int minLen = min(len1, len2);
    char* result = new char[minLen + 1];

    // Find common prefix
    int i = 0;
    for (; i < minLen; ++i) {
        if (s1[i] != s2[i]) break;
    }
    strncpy(result, s1, i);
    result[i] = '\0';

    return result;
}
//task 14
int countChar(char* s1, char* s2) {
    int count = 0;
    int freq1[256] = {0}, freq2[256] = {0};

    for (int i = 0; s1[i]; i++) freq1[(unsigned char)s1[i]]++;
    for (int i = 0; s2[i]; i++) freq2[(unsigned char)s2[i]]++;

    for (int i = 0; i < 256; i++)
        count += min(freq1[i], freq2[i]);

    return count;
}
//task 15
int longestNonDecreasingSubsequence(int* arr, int n) {
    int* dp = new int[n];
    int maxLength = 0;

    for (int i = 0; i < n; ++i) {
        dp[i] = 1;
        for (int j = 0; j < i; ++j) {
            if (arr[j] <= arr[i])
                dp[i] = max(dp[i], dp[j] + 1);
        }
        maxLength = max(maxLength, dp[i]);
    }

    delete[] dp;
    return maxLength;
}

//task 16
bool isSafe(int city, int graph[MAX_CITIES][MAX_CITIES], int path[], int pos) {
    for (int i = 0; i < pos; i++) {
        if (graph[path[i]][city] == 1) {
            return false;
        }
    }
    return true;
}

void findMaxSubsetUtil(int graph[MAX_CITIES][MAX_CITIES], int n, int path[], int pos, int bestPath[], int &bestSize) {
    if (pos > bestSize) {
        bestSize = pos;
        for (int i = 0; i < pos; i++) {
            bestPath[i] = path[i];
        }
    }

    for (int v = 0; v < n; v++) {
        if (isSafe(v, graph, path, pos)) {
            path[pos] = v;
            findMaxSubsetUtil(graph, n, path, pos + 1, bestPath, bestSize);
            path[pos] = -1; // Backtrack
        }
    }
}

void findMaxSubset(int graph[MAX_CITIES][MAX_CITIES], int n, int bestPath[], int &bestSize) {
    int path[MAX_CITIES];
    for (int i = 0; i < MAX_CITIES; i++) path[i] = -1;

    bestSize = 0;
    findMaxSubsetUtil(graph, n, path, 0, bestPath, bestSize);
}


int main() {
    const int ROWS = 3;
    const int COLS = 4;
    int arr1[ROWS][COLS] = {
            {1, 2, 3, 4},
            {5, 6, 7, 8},
            {9, 10, 11, 12}
    };

    cout << "Task 1: Max/Min in 2D Array Rows" << endl;
    for (int i = 0; i < ROWS; ++i) {
        if (i % 2 == 0)
            cout << "Max in row " << i << ": " << findMax(arr1[i], COLS) << endl;
        else
            cout << "Min in row " << i << ": " << findMin(arr1[i], COLS) << endl;
    }

    const int SIZE = 5;
    int arr2[SIZE] = { -1, 3, 4, -2, 5 };
    int a = 1, b = 2, c = 3;

    cout << "Task 2: Replace Positive Elements in Array" << endl;
    cout << "Original array: ";
    for (int i = 0; i < SIZE; ++i) cout << arr2[i] << " ";
    cout << endl;

    replacePositive(arr2, SIZE, a, b, c);

    cout << "Modified array: ";
    for (int i = 0; i < SIZE; ++i) cout << arr2[i] << " ";
    cout << endl;

    int arr3[SIZE] = {5, 3, 1, 4, 2};

    cout << "Task 3: Sort Array" << endl;
    cout << "Original array: ";
    for (int i = 0; i < SIZE; ++i) cout << arr3[i] << " ";
    cout << endl;

    sortArray(arr3, SIZE, true);
    cout << "Sorted ascending: ";
    for (int i = 0; i < SIZE; ++i) cout << arr3[i] << " ";
    cout << endl;

    sortArray(arr3, SIZE, false);
    cout << "Sorted descending: ";
    for (int i = 0; i < SIZE; ++i) cout << arr3[i] << " ";
    cout << endl;

    cout << "Task 4: Recursive String Copy" << endl;
    char source[] = "Hello, World!";
    char destination[50];

    recursiveStringCopy(destination, source);
    cout << "Original String: " << source << endl;
    cout << "Copied String: " << destination << endl;

    cout << "Task 5: Lexicographic Comparison of Two Numbers" << endl;
    unsigned int num1 = 1234, num2 = 2345;

    int result = lexicographicCompare(num1, num2);
    cout << "Comparing " << num1 << " and " << num2 << ": ";
    if (result < 0) cout << num1 << " is lexicographically smaller than " << num2 << endl;
    else if (result > 0) cout << num1 << " is lexicographically greater than " << num2 << endl;
    else cout << num1 << " and " << num2 << " are lexicographically equal" << endl;

    cout << "Task 6: Recursive String to Number Conversion" << endl;
    char numberString[] = "12345";

    int number = stringToNumber(numberString);
    cout << "String: " << numberString << endl;
    cout << "Number: " << number << endl;

    cout << "Task 7: Replace Characters in a String Recursively" << endl;
    char str1[] = "example string with x characters";
    char findChar = 'x', replaceChar = 'y';

    cout << "Original String: " << str1 << endl;
    replaceCharacter(str1, findChar, replaceChar);
    cout << "Modified String: " << str1 << endl;

    cout << "Task 8: Find Region with Sum S in a Grid" << endl;
    const int GRID_ROWS = 3;
    const int GRID_COLS = 3;
    int** grid = new int*[GRID_ROWS];
    for (int i = 0; i < GRID_ROWS; ++i) {
        grid[i] = new int[GRID_COLS];
    }

// Pravq go taka za da moga da izpolzvam dinamichen masiv
    int gridData[3][3] = {{1, 2, 3}, {4, 5, 6}, {7, 8, 9}};
    for (int i = 0; i < GRID_ROWS; ++i) {
        for (int j = 0; j < GRID_COLS; ++j) {
            grid[i][j] = gridData[i][j];
        }
    }
    int S = 15;

    bool found = findRegionWithSum(grid, GRID_ROWS, GRID_COLS, S);
    cout << "Region with sum " << S << (found ? " is found." : " is not found.") << endl;

    cout << "Task 9: Knight Move on Checkerboard" << endl;
    const int BOARD_ROWS = 5;
    const int BOARD_COLS = 5;
    char** board = new char*[BOARD_ROWS];
    for (int i = 0; i < BOARD_ROWS; ++i) {
        board[i] = new char[BOARD_COLS];
    }

// Initialize the board
    char boardData[5][5] = {
            {'a', 'b', 'c', 'd', 'e'},
            {'f', 'g', 'h', 'i', 'j'},
            {'k', 'l', 'm', 'n', 'o'},
            {'p', 'q', 'r', 's', 't'},
            {'u', 'v', 'w', 'x', 'y'}
    };
    for (int i = 0; i < BOARD_ROWS; ++i) {
        for (int j = 0; j < BOARD_COLS; ++j) {
            board[i][j] = boardData[i][j];
        }
    }
    string text = "abcd";
    int startX = 0, startY = 0;  // Starting position on the board

    bool canRead = canFormText(board, BOARD_ROWS, BOARD_COLS, text, startX, startY);
    cout << "Can read \"" << text << "\" starting from (" << startX << ", " << startY << "): "
         << (canRead ? "Yes" : "No") << endl;

    cout << "Task 10: Delete First Occurrence in String" << endl;
    char* str10 = new char[50];
    strcpy(str10, "This is a test string for testing.");
    const char* findWhat10 = "test";

    cout << "Original String: " << str10 << endl;
    deleteFirst(str10, findWhat10);
    cout << "String after deleting first occurrence of '" << findWhat10 << "': " << str10 << endl;

    delete[] str10;

    cout << "Task 11: Delete Last Occurrence in String" << endl;
    char* str11 = new char[50];
    strcpy(str11, "Test string with test for testing.");
    const char* findWhat11 = "test";

    cout << "Original String: " << str11 << endl;
    deleteLast(str11, findWhat11);
    cout << "String after deleting last occurrence of '" << findWhat11 << "': " << str11 << endl;

    delete[] str11;

    cout << "Task 12: Delete All Occurrences in String" << endl;
    char* str12 = new char[50];
    strcpy(str12, "Test string for test and testing.");
    const char* findWhat12 = "test";

    cout << "Original String: " << str12 << endl;
    deleteAll(str12, findWhat12);
    cout << "String after deleting all occurrences of '" << findWhat12 << "': " << str12 << endl;

    delete[] str12;

    cout << "Task 13: Find Common Prefix or Suffix" << endl;
    char s1_13[] = "beginning";
    char s2_13[] = "begging";

    char* commonPrefix = common(s1_13, s2_13);
    cout << "Common prefix of \"" << s1_13 << "\" and \"" << s2_13 << "\": " << commonPrefix << endl;
    delete[] commonPrefix;  // Remember to delete dynamically allocated memory

    cout << "Task 14: Count Identical Characters" << endl;
    char s1_14[] = "character";
    char s2_14[] = "architect";

    int count = countChar(s1_14, s2_14);
    cout << "Number of identical characters in \"" << s1_14 << "\" and \"" << s2_14 << "\": " << count << endl;

    cout << "Task 15: Longest Nondecreasing Subsequence" << endl;
    int arr_15[] = {3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5};
    int n_15 = sizeof(arr_15)/sizeof(arr_15[0]);

    int length = longestNonDecreasingSubsequence(arr_15, n_15);
    cout << "Length of the longest nondecreasing subsequence: " << length << endl;

    int n = 5;
    int cityConnections[MAX_CITIES][MAX_CITIES] = {
            {0, 1, 0, 0, 0},
            {1, 0, 1, 0, 0},
            {0, 1, 0, 1, 0},
            {0, 0, 1, 0, 1},
            {0, 0, 0, 1, 0}
    };

    int bestPath[MAX_CITIES];
    int bestSize;

    findMaxSubset(cityConnections, n, bestPath, bestSize);

    cout << "Maximal Subset of Non-connected Cities: ";
    for (int i = 0; i < bestSize; i++) {
        cout << bestPath[i] << " ";
    }
    cout << endl;
    return 0;
}
