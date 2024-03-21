#include <iostream>
#include <cstdlib>
#include <cmath>
#include <algorithm>

using namespace std;

int main() {

    //task 1
    const int min = -12, max = 122;

    int rows, cols;

    cout << "Enter the number of rows: ";
    cin >> rows;

    cout << "Enter the number of columns: ";
    cin >> cols;

    int **taskOneArray = new int *[rows];

    cout << "Task 1 array" << endl;
    for (int i = 0; i < rows; ++i) {
        taskOneArray[i] = new int[cols];

        for (int j = 0; j < cols; ++j) {
            taskOneArray[i][j] = min + (rand() % (max - min + 1));

            cout << taskOneArray[i][j] << " ";
        }

        cout << endl;
    }

//    for(int i = 0; i < rows; ++i)
//        delete [] array[i];
//    delete [] array;

//task 2
    int **taskTwoArray = new int *[rows];

    for (int i = 0; i < rows; ++i) {
        taskTwoArray[i] = new int[cols];
        for (int j = 0; j < cols; ++j)
            taskTwoArray[i][j] = taskOneArray[i][j];
    }

    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols - i - 1; ++j) {
            taskTwoArray[i][j] = 0;
        }
    }

    cout << "Task 2 array: " << endl;
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            cout << taskTwoArray[i][j] << " ";
        }
        cout << endl;
    }

//    for(int i = 0; i < rows; ++i)
//        delete [] taskTwoArray[i];
//    delete [] taskTwoArray;

//task 3
    int **taskThreeArray = new int *[rows];

    for (int i = 0; i < rows; ++i) {
        taskThreeArray[i] = new int[cols];
        for (int j = 0; j < cols; ++j)
            taskThreeArray[i][j] = taskOneArray[i][j];
    }
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j)
            if (j <= i)
                taskThreeArray[i][j] = pow(i + j, i + 1);
    }

    cout << "Task 3 array: " << endl;
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            cout << taskThreeArray[i][j] << " ";
        }
        cout << endl;
    }

    //task 4
    int **taskFourArray = new int *[rows];

    for (int i = 0; i < rows; ++i) {
        taskFourArray[i] = new int[cols];
        for (int j = 0; j < cols; ++j)
            taskFourArray[i][j] = taskOneArray[i][j];
    }
    cout << "Task 4 spiral array: " << endl;
    int startRow = 0, startCol = 0;
    while (startRow < rows && startCol < cols) {
        for (int i = startCol; i < cols; ++i) {
            cout << taskFourArray[startRow][i] << " ";
        }

        startRow++;

        for (int i = startRow; i < rows; ++i) {
            cout << taskFourArray[i][cols - 1] << " ";
        }

        cols--;

        if (startRow < rows) {
            for (int i = cols - 1; i >= startCol; --i) {
                cout << taskFourArray[rows - 1][i] << " ";
            }

            rows--;
        }

        if (startCol < cols) {
            for (int i = rows - 1; i >= startRow; --i) {
                cout << taskFourArray[i][startCol] << " ";
            }

            startCol++;
        }
    }

    for (int i = 0; i < rows; ++i) {
        delete[] taskOneArray[i];
        delete[] taskTwoArray[i];
        delete[] taskThreeArray[i];
        delete[] taskFourArray[i];
    }
    delete[] taskOneArray;
    delete[] taskTwoArray;
    delete[] taskThreeArray;
    delete[] taskFourArray;

    cout << endl;

    //task 5
    int n, p, q;
    cout << "Enter n: ";
    cin >> n;
    int *arr = new int[n];
    cout << "Enter the array: ";
    for (int i = 0; i < n; i++) {
        cin >> arr[i];
    }
    cout << "Enter the interval p - q: ";
    cin >> p >> q;

    int count = 0;
    for (int i = 0; i < n; i++) {
        if (i % 2 == 0 && arr[i] % 2 == 0 && arr[i] >= p && arr[i] <= q) {
            count++;
        }
    }

    cout << "Task 5 final result is: " << count << endl;

    //task 6
    delete[] arr;

    cout << "Enter n: ";
    cin >> n;

    arr = new int[n];

    cout << "Enter the array: ";
    for (int i = 0; i < n; i++) {
        cin >> arr[i];
    }

    cout << "Enter the p-th positive element to find: ";
    cin >> p;

    int firstPositive = -1, lastPositive = -1, pthPositive = -1;
    count = 0;

    for (int i = 0; i < n; i++) {
        if (arr[i] > 0) {
            count++;
            if (firstPositive == -1) firstPositive = i;
            if (count == p) pthPositive = i;
            lastPositive = i;
        }
    }

    cout << "Task 6: " << endl;

    cout << "First positive element at index: " << firstPositive << endl;

    cout << "Last positive element at index: " << lastPositive << endl;

    cout << "The index of the p-th(" << p << ") positive element is: " << pthPositive << endl;

    delete[] arr;

    //task 7
    cout << "Enter n: ";
    cin >> n;

    arr = new int[n];

    cout << "Enter the array: ";
    for (int i = 0; i < n; i++) {
        cin >> arr[i];
    }

    cout << "Task 7: ";
    for (int i = 1; i < n; ++i) {
        if (arr[i] % i == 0) {
            cout << arr[i] << " ";
        }
    }
    cout << endl;

    //task 8
    cout << "Enter n: ";
    cin >> n;

    arr = new int[n];

    cout << "Enter the array: ";
    for (int i = 0; i < n; i++) {
        cin >> arr[i];
    }

    for (int i = 1; i < n; ++i) {
        if (arr[i] < i) {
            arr[i] *= arr[i];
        } else if (arr[i] == i) {
            arr[i] = -arr[i];
        } else if (arr[i] > i) {
            arr[i] -= 1;
        }
    }

    cout << "Task 8: ";
    for (int i = 1; i < n; ++i) {
        cout << arr[i] << " ";
    }
    cout << endl;

    //task 9
    cout << "Enter n: ";
    cin >> n;

    arr = new int[n];

    cout << "Enter the array: ";
    for (int i = 0; i < n; i++) {
        cin >> arr[i];
    }

    cout << "Reverse order: ";
    for(int i = n-1; i >= 0; i--) {
        cout << arr[i] << " ";
    }
    cout << endl;

    cout << "Even positions first then odd: ";
    for(int i = 0; i < n; i+=2) {
        cout << arr[i] << " ";
    }
    for(int i = 1; i < n; i+=2) {
        cout << arr[i] << " ";
    }
    cout << endl;

    unsigned seed = chrono::system_clock::now().time_since_epoch().count();

    for(int i = n-1; i > 0; i--) {
        int j = rand() % (i+1);
        swap(arr[i], arr[j]);
    }
    cout << "Random order: ";
    for(int i = 0; i < n; i++) {
        cout << arr[i] << " ";
    }
    cout << endl;

    //task 10

    arr = new int[n];

    cout << "Enter the array: ";
    for (int i = 0; i < n; i++) {
        cin >> arr[i];
    }

    sort(arr, arr + n);

    // Remove duplicates
    int newSize = distance(arr, unique(arr, arr + n));

    cout << "Array after removing duplicates: ";
    for(int i = 0; i < newSize; i++) {
        cout << arr[i] << " ";
    }
    cout << endl;

    
    return 0;
}
