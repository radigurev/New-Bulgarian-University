#include <iostream>
#include <string>

using namespace std;

int main() {
    int row, col, currentCol, currentRow, maxBoxesYouCanOpen, prevRow, prevCol,playerBoxRow,playerBoxCol;
    int boxesOpened = 0, maxSumOfBoxes = 0;
    string command;

    cin >> row;
    cin >> col;

    int **room = new int *[row];

    for (int i = 0; i < row; ++i) {
        room[i] = new int[col];

        for (int j = 0; j < col; ++j) {
            cin >> room[i][j];
            maxSumOfBoxes += room[i][j];
        }
    }


    maxBoxesYouCanOpen = row * col;

    cin >> currentRow;
    cin >> currentCol;

    playerBoxRow = currentRow;
    playerBoxCol = currentCol;

    while (cin >> command && command != "yes") {

        if(command == "no")
            cin >> command;

        prevRow = currentRow;
        prevCol = currentCol;

        currentRow = stoi(command);
        cin >> currentCol;

        if ((playerBoxRow == currentRow && playerBoxCol == currentCol) || (maxBoxesYouCanOpen == boxesOpened)) {
            cout << "You won: " << room[currentRow][currentCol] << "lv.";
            break;
        }

        maxSumOfBoxes -= room[currentRow][currentCol];

        boxesOpened++;

        if (boxesOpened % 3 == 0) {
            cout << "The offer is " << (int) (maxSumOfBoxes / (maxBoxesYouCanOpen - boxesOpened)) / 2
                 << "lv. Do you take it" << endl;
        }
    }

    if (command == "yes")
        cout << "You took the offer for "
             << (int) (maxSumOfBoxes / (maxBoxesYouCanOpen - boxesOpened)) / 2
             << "lv. Instead of " << room[playerBoxRow][playerBoxCol]
             << "";

    return 0;
}
