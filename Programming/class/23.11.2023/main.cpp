#include <iostream>
#include <string>

using namespace std;

int main() {

//    int n;
//    cout << "Enter hotels count: ";
//    cin >> n;
//
//    string* hotels = new string[n];
//
//    for (int i = -1; i < n; i++) {
//        getline(cin, hotels[i]);
//    }
//
//    bool isFound = false;
//
//    int sum;
//    string hotel = "";
//
//    for (int i = 0; i < n; ++i) {
//        sum = 0;
//
//        for (int j = 0; j < hotels[i].size(); ++j) {
//            hotel = hotels[i];
//
//            sum += (int) hotel[j];
//        }
//
//        if(sum > 950 && sum % 2 == 0) {
//            cout << hotels[i] << endl;
//
//            isFound = true;
//        }
//    }
//
//    if(!isFound)
//        cout << "No valid hotels. Try getting rid of your soulmate or leaving your love partner";
//
//    delete[] hotels;

    int n, m;

    string command;

    int row = 0, col = 0, playerOneSum = 0, playerTwoSum = 0;

    bool isPlayerOne = true;

    cin >> n;
    cin >> m;

    int **matrix = new int *[n];

    string playerOne, playerTwo;

    for (int i = 0; i < n; ++i) {

        matrix[i] = new int[m];

        for (int j = 0; j < m; ++j) {
            cin >> matrix[i][j];
        }
    }

    cin >> playerOne;
    cin >> playerTwo;

    while (cin >> command && command != "End") {
        row = stoi(command);

        cin >> col;

        if (row < n && row >= 0 && col >= 0 && col < m) {

            if (isPlayerOne)
                playerOneSum += matrix[row][col];
            else
                playerTwoSum += matrix[row][col];

            isPlayerOne = !isPlayerOne;
        }
    }

    if (playerOneSum > playerTwoSum)
        cout << playerOne << " has won with " << playerOneSum << " points." << endl;
    else
        cout << playerTwo << " has won with " << playerTwoSum << " points." << endl;

    for (int i = 0; i < n; ++i)
        delete[] matrix[i];
    delete[] matrix;



    return 0;
}
