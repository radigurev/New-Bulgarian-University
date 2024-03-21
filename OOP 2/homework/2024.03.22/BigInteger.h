//
// Created by Radoslav Gurev on 21.03.24.
//

#ifndef INC_2024_03_22_BIGINTEGER_H
#define INC_2024_03_22_BIGINTEGER_H

#include "iostream"

using namespace std;

class BigInteger {
private:
    string number;

    string sumNumbers(const string &num1, const string &num2) const;

    string subtractNumbers(const string &num1, const string &num2) const;

    string multiplyNumbers(const string &num1, const string &num2) const;

public:
    BigInteger(const string &num);

    BigInteger operator+(const BigInteger &rhs) const;

    BigInteger operator-(const BigInteger &rhs) const;

    BigInteger operator*(const BigInteger &rhs) const;

    friend ostream &operator<<(ostream &os, const BigInteger &bi);
};

#endif //INC_2024_03_22_BIGINTEGER_H
