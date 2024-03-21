#include <iostream>

using namespace std;

int main() {


    char text[100];
    char printText[100];

    cin.getline(text,100);

    char startLetter = 96;

    int i = 0;

    for (char letter : text) {
        if(letter == '\0') break;

        while (startLetter != letter) {

            if(letter == ' ') {
                printText[i] = ' ';
                break;
            }

            startLetter++;

            if(startLetter == 123) startLetter = 65;

            printText[i] = startLetter;

            cout << printText << endl;

        }

        startLetter = 96;
        i++;
    }

    return 0;
}
