#include <iostream>

using namespace std;

int main() {

    //1.
    cout << "First task" << endl;
    int integerVariable;
    char charVariable;
    bool boolVariable;
    double doubleVariable;
    float floatVariable;

    cout << "Enter int value: ";
    cin >> integerVariable;

    cout << "Enter char value: ";
    cin >> charVariable;

    cout << "Enter bool value: ";
    cin >> boolVariable;

    cout << "Enter double value: ";
    cin >> doubleVariable;

    cout << "Enter float value: ";
    cin >> floatVariable;

    cout << "Int value: " << integerVariable << endl;
    cout << "Chat value: " << charVariable << endl;
    cout << "Bool value: " << boolVariable << endl;
    cout << "Double value: " << doubleVariable << endl;
    cout << "Float value: " << floatVariable << endl;

    //2.
    cout << "Second task" << endl;

    int firstVariable;
    int secondVariable;

    cout << "Enter first variable: ";
    cin >> firstVariable;
    cout << "Enter second variable: ";
    cin >> secondVariable;

    cout << "Operation + : " << firstVariable + secondVariable << endl;

    cout << "Operation - : " << firstVariable - secondVariable << endl;

    cout << "Operation * : " << firstVariable * secondVariable << endl;

    cout << "Operation / : " << firstVariable / secondVariable << endl;

    cout << "Operation % : " << firstVariable % secondVariable << endl;

    //3.
    cout << "Third task" << endl;

    double variableOne;
    double variableTwo;

    cout << "Enter first variable: ";
    cin >> variableOne;

    cout << "Enter second variable: ";
    cin >> variableTwo;

    variableOne += variableTwo;
    cout << "a += b -> " << variableOne << endl;

    variableOne *= variableTwo;
    cout << "a *= b -> " << variableOne << endl;

    variableOne -= variableTwo;
    cout << "a -= b -> " << variableOne << endl;

    variableOne /= variableTwo;
    cout << "a /=b -> " << variableOne << endl;

    //4.
    cout << "Fourth task" << endl;

    int variableA;
    int variableB;

    cout << "Enter first variable: ";
    cin >> variableA;

    cout << "Enter second variable: ";
    cin >> variableB;

    variableA = variableA ^ variableB;
    variableB = variableA ^ variableB;
    variableA = variableA ^ variableB;

    cout << "Values after change: " << endl;
    cout << "A value: " << variableA << endl;
    cout << "B value: " << variableB << endl;
    return 0;
}

/*
& - The process of the bitwise AND operation involves combining two binary numbers by iterating through each bit.
It then generates a new binary number where each bit is assigned a value of 1 only if
  both corresponding bits in the original numbers are 1.

| - The OR operation combines binary numbers bit by bit, resulting in
a new binary number where each bit is 1 if at least one corresponding bit in the original numbers is 1.

^ - The XOR operation combines two binary numbers, bit by bit.
It produces a new binary number with each corresponding bit set to 1 if the bits in the original
 numbers are different (i.e. one is 1 and the other is 0).

~ - The not bitwise operation converts each bit in a binary number, transforming 0s into 1s and 1s into 0s.

<< - Shifting the bits in a binary number to the left through left shift results in multiplying the number by powers of 2.

>> - The right shift operation moves bits to the right, effectively dividing the number by powers of 2.
 */