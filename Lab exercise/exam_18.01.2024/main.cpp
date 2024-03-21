#include <iostream>

using namespace std;

//task 1
void encode(char*);
void decode(char*);

//task 2
void enterA(int*, int);

void enterB(int*, int);

void enterC(int*, int);

bool isPrime(int);

bool isEven(int);

void generateMask(char*,int*, int);

void compareMasks(char*,int, char*,int);

//task 3
void enterArray(char*, int);

bool checkIfSubsets(char*,char*);

//task 3

int main() {

    //task 1

//    char* sentence = new char[100];
//
//    cin.getline(sentence,100);
//
//    encode(sentence);
//
//    cout << sentence << endl;
//
//    decode(sentence);
//
//    cout << sentence;


//task 2
//    int lengthA, lengthB, lengthC;
//
//    cout << "Enter length for A: ";
//    cin >> lengthA;
//
//    int* A = new int[lengthA];
//
//    enterA(A,lengthA);
//
//    cout << "Enter length for B: ";
//    cin >> lengthB;
//
//    int* B = new int[lengthB];
//
//    enterB(B,lengthB);
//
//    cout << "Enter length for C: ";
//    cin >> lengthC;
//
//    int* C = new int[lengthC];
//
//    enterC(C,lengthC);
//
//    int D[] = {1, 2};
//
//    int E[] = {1};
//
//    char* maskA = new char[lengthA+1] {'0'};
//    char* maskB = new char[lengthB+1] {'0'};
//    char* maskC = new char[lengthC+1] {'0'};
//    char* maskD = new char[3] {'0'};
//    char* maskE = new char[2] {'0'};
//
//    generateMask(maskA,A,lengthA);
//    generateMask(maskB,B,lengthB);
//    generateMask(maskC,C,lengthC);
//    generateMask(maskD,D,2);
//    generateMask(maskD,D,1);
//
//    cout << "Mask A and mask B" << endl;
//    compareMasks(maskA,lengthA,maskB,lengthB);
//    cout << "Mask A and mask C" << endl;
//    compareMasks(maskA,lengthA,maskC,lengthC);
//    cout << "Mask A and mask D" << endl;
//    compareMasks(maskA,lengthA,maskD,2);
//    cout << "Mask A and mask E" << endl;
//    compareMasks(maskA,lengthA,maskE,1);
//
//    cout << "Mask B and mask C" << endl;
//    compareMasks(maskB,lengthB,maskC,lengthC);
//    cout << "Mask B and mask D" << endl;
//    compareMasks(maskB,lengthB,maskD,2);
//    cout << "Mask B and mask E" << endl;
//    compareMasks(maskB,lengthB,maskE,1);
//
//    cout << "Mask C and mask D" << endl;
//    compareMasks(maskC,lengthC,maskD,2);
//    cout << "Mask C and mask E" << endl;
//    compareMasks(maskC,lengthC,maskE,1);
//
//    cout << "Mask D and mask E" << endl;
//    compareMasks(maskE,2,maskE,1);

//task 3
    int aLength, bLength;

    cout << "Enter length for A: ";
    cin >> aLength;

    cout << "Enter length for B: ";
    cin >> bLength;

    char* arrayA = new char[aLength];
    char* arrayB = new char [bLength];

    enterArray(arrayA,aLength);
    enterArray(arrayB,bLength);

    bool isSubsets = checkIfSubsets(arrayA,arrayB);

    cout << (isSubsets ? "They are subsets" : "They are not subsets");

    return 0;
}
//task 1
void encode(char* sentence) {

    int sentenceLength = (int) strlen(sentence);

    for (int i = 0; i < sentenceLength; ++i) {

        if(isdigit(sentence[i])) continue;

        switch (sentence[i]) {
            case 'z':
            case 'Z':
                sentence[i] = ' ';
                break;
            case ' ':
                if(sentenceLength - 1 == i) {
                    sentence[i] = 'A';
                    break;
                }

                if(isupper(sentence[i + 1]))
                    sentence[i] = 'A';
                else
                    sentence[i] = 'a';

                break;
            default:
                sentence[i] = ++sentence[i];
                break;
        }
    }
}

void decode(char* encodedSentence) {
    int sentenceLength = (int) strlen(encodedSentence);

    bool shouldSkip = false;

    for (int i = 0; i < sentenceLength; ++i) {

        if(isdigit(encodedSentence[i])) continue;

        switch (encodedSentence[i]) {
            case 'a':
            case 'A':
                encodedSentence[i] = ' ';
                break;
            case ' ':
                if(sentenceLength - 1 == i || encodedSentence[i] == ' ') {
                    encodedSentence[i] = 'z';
                    break;
                }

                if(isupper(encodedSentence[i + 1]))
                    encodedSentence[i] = 'Z';
                else
                    encodedSentence[i] = 'z';

                break;
            default:
                encodedSentence[i] = --encodedSentence[i];
                break;
        }
    }
}

//task 2

void enterA(int* A, int n) {

    int entry = 0;

    for (int i = 0; i < n; ++i) {
        do {
            cout << "Enter A["<< i + 1 <<"]:";
            cin >> entry;
        } while (entry < 1 || entry > 10);
    }
}

void enterB(int* B, int n) {
    int entry = 0;

    for (int i = 0; i < n; ++i) {
        do {
            cout << "Enter B["<< i + 1 <<"]:";
            cin >> entry;
        } while (!isPrime(entry));
    }
}

void enterC(int* C, int n) {
    int entry = 0;

    for(int i = 0; i < n; i++) {
        do {
            cout << "Enter C["<< i + 1 <<"]:";
            cin >> entry;
        } while ( entry > 1 && entry <= 6 && !isEven(entry));
    }
}

bool isPrime(int entry) {
    int i;

    for (i = 2; i <= entry/2; ++i) {
        if (entry % i == 0) {
            return false;
        }
    }

    return true;
}

bool isEven(int entry) {
    if(entry % 2 == 0) return true;

    return false;
}

void generateMask(char* arr,int* numbers, int n) {
    for (int i = 0; i < n; ++i) {
        arr[numbers[i]] = '1';
    }
}

void compareMasks(char* A,int lengthA, char* B, int lengthB) {
    char* temp;
    int tempLength;

    if(lengthA < lengthB) {
        temp = A;
        tempLength = lengthA;

        lengthA = lengthB;
        lengthB = tempLength;

        A = B;
        B = temp;
    }

    for(int i=0;i < lengthB; i++) {
        if(B[i] != A[i]) {

            cout << "They are not subsets";
            return;
        }
    }

    cout << "They are subsets" << endl;
}

//task 3

void enterArray(char* arr, int n) {
    for (int i = 0; i < n; ++i) {
        cout << "Enter arr["<< i+1 << "]=";
        cin >> arr[i];
    }
}

bool checkIfSubsets(char* A, char* B) {

    int lengthA = (int) strlen(A);
    int lengthB = (int) strlen(B);

    if(lengthB > lengthA) {
        char* temp = A;
        int lengthTemp = lengthA;

        lengthA = lengthB;
        lengthB = lengthTemp;

        A = B;
        B = temp;
    }

    bool isNotSubsets = true;

    for (int i = 0; i < lengthB; ++i) {
        for (int j = 0; j < lengthA; ++j) {
            if(A[j] == B[i]) {
                cout << B[i] << endl;
                isNotSubsets = true;
                break;
            }else {
                isNotSubsets = false;
            }
        }
    }

    return isNotSubsets;

}