//
// Created by Radoslav Gurev on 21.03.24.
//

#ifndef INC_2024_03_22_MONEY_H
#define INC_2024_03_22_MONEY_H
#include <iostream>

using namespace std;

class Money
{
private:
    int money;
    int coins;

public:
    Money();
    Money(int, int);

    int getMoney() const;

    void setMoney(int);

    int getCoins() const;

    void setCoins(int);

    friend istream& operator>>(istream&, Money&);

    friend ostream& operator<<(ostream&, Money&);

        bool operator!=(const Money&);

    Money operator+(const Money&) const;

    Money operator-(const Money&) const;

    Money operator*(int) const;

    Money operator/(int) const;

    Money operator%(double) const;

    bool operator==(const Money &rhs) const;

    bool operator<(const Money &rhs) const;

    bool operator>(const Money &rhs) const;

    bool operator<=(const Money &rhs) const;

    bool operator>=(const Money &rhs) const;

};
#endif //INC_2024_03_22_MONEY_H
