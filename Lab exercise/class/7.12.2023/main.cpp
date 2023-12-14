#include <iostream>

using namespace std;

void encode(char *orig, char *coded) {
    char *ptr = orig;

    int i = 0;

    while (*ptr != '\0') {
        if ((*ptr >= 'A' && *ptr <= 'Z') ||
            (*ptr >= 'a' && *ptr <= 'z')) {

            if ((*ptr >= 'A' && *ptr <= 'X') ||
                (*ptr >= 'a' && *ptr <= 'x')) {
                coded[i++] = *ptr + 3;
            } else {
                coded[i++] = *ptr - 23;
            }
        }
        ptr++;
    }

    coded[i] = '\0';
}

void decode(char *encoded, char *decoded) {
    char *ptr = encoded;

    int i = 0;

    while (*ptr != '\0') {
        if ((*ptr >= 'A' && *ptr <= 'Z') ||
            (*ptr >= 'a' && *ptr <= 'z')) {

            if ((*ptr >= 'C' && *ptr <= 'Z') ||
                (*ptr >= 'c' && *ptr <= 'z')) {
                decoded[i++] = *ptr - 3;
            } else {
                decoded[i++] = *ptr + 23;
            }
        }
        ptr++;
    }

    decoded[i] = '\0';
}

bool checkISBN(char *isbn) {
    int sum = 0;
    int i = 11;
    *isbn--;
    while (i != 0) {
        *isbn++;

        if (*isbn == '-' || *isbn == '\0') continue;

        i--;

        if (*isbn == 'X') {
            sum += 10 * i;
            continue;
        }

        int digit = *isbn - '0';

        sum += i * digit;
    }
    return sum % 11 == 0;
}

void fixCode(char *isbn) {
    int i = 11;
    int sum = 0;

    bool isIncorrect = false;

    *isbn--;

    while (*isbn != '\0') {
        *isbn++;

        if (*isbn == '-' || *isbn == '\0') continue;

        i--;

        if (*isbn == 'X') {
            sum += 10 * i;
        }else {
            sum += i * (*isbn - '0');
        }

        if(sum % 11 != 0) {
            isIncorrect = true;
            break;
        }
    }

    cout << "The ISBN is " << (isIncorrect ? "correct" : "incorrect at position " + to_string(i));
}

int main() {
//    string name;
//
//    cout << "What is your name? ";
//    getline(cin,name);
//
//    cout << "Hello, " << name << endl;

//    char str[1000];
//    char *encodedText = new char[strlen(str)+1];
//    char *decodedText = new char[strlen(str)+1];
//
//    cin.getline(str, 999);
//
//    encode(str,encodedText);
//
//    cout << "Encoded: " << encodedText << endl;
//
//    decode(encodedText,decodedText);
//
//    cout << "Decoded: " << decodedText << endl;

    char str[15];

    cin.getline(str, 14);


//    cout << "The entered ISBN is " << (checkISBN(str) ? "correct" : "incorrect");

//    fixCode(str);

    return 0;
}
