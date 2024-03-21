//
// Created by Radoslav Gurev on 21.03.24.
//
#include "BigInteger.h"

string BigInteger::sumNumbers(const std::string &num1, const std::string &num2) const {

    std::string result = "";
    int carry = 0;

    int i = num1.length() - 1, j = num2.length() - 1;

    while (i >= 0 || j >= 0 || carry) {

        int digit1 = i >= 0 ? num1[i] - '0' : 0;
        int digit2 = j >= 0 ? num2[j] - '0' : 0;

        int sum = digit1 + digit2 + carry;
        carry = sum / 10;
        result += (sum % 10) + '0';

        i--; j--;
    }

    std::reverse(result.begin(), result.end());

    return result;
}

string BigInteger::subtractNumbers(const std::string &num1, const std::string &num2) const {
    string result = "";

    int i = num1.length() - 1, j = num2.length() - 1, borrow = 0;

    while (i >= 0 || j >= 0) {
        int digit1 = i >= 0 ? num1[i] - '0' : 0;
        int digit2 = j >= 0 ? num2[j] - '0' + borrow : 0 + borrow;

        if (digit1 < digit2) {
            digit1 += 10;
            borrow = 1;
        } else {
            borrow = 0;
        }

        result += (digit1 - digit2) + '0';

        i--; j--;
    }

    while (result.length() > 1 && result.back() == '0') {
        result.pop_back();
    }

    reverse(result.begin(), result.end());
    return result;
}

string BigInteger::multiplyNumbers(const std::string &num1, const std::string &num2) const {
    if (num1 == "0" || num2 == "0") return "0";

    vector<int> result(num1.size() + num2.size(), 0);

    for (int i = num1.size() - 1; i >= 0; i--) {
        for (int j = num2.size() - 1; j >= 0; j--) {
            int mul = (num1[i] - '0') * (num2[j] - '0');
            int sum = result[i + j + 1] + mul;

            result[i + j + 1] = sum % 10;
            result[i + j] += sum / 10;
        }
    }

    string resultStr;
    for (int num : result) {
        if (!(resultStr.empty() && num == 0)) {
            resultStr.push_back(num + '0');
        }
    }

    return resultStr.empty() ? "0" : resultStr;
}

BigInteger::BigInteger(const std::string &num) {
    this->number = num;
}

BigInteger BigInteger::operator*(const BigInteger &rhs) const {
    return {multiplyNumbers(this->number,rhs.number)};
}

BigInteger BigInteger::operator+(const BigInteger &rhs) const {
    return {sumNumbers(this->number,rhs.number)};
}

BigInteger BigInteger::operator-(const BigInteger &rhs) const {
    return {subtractNumbers(this->number,rhs.number)};
}

ostream &operator<<(ostream &os, const BigInteger &bi) {
    os << bi.number;
    return os;
}