//
// Created by Radoslav Gurev on 21.03.24.
//
#include "Money.h"
using namespace std;
Money::Money(): money(0),coins(0){}

Money::Money(int money, int coins):money(money),coins(coins){}

#pragma region getters
int Money::getMoney() const
{
    return this->money;
}

int Money::getCoins() const
{
    return this->coins;
}
#pragma endregion
#pragma region setters
void Money::setMoney(int money)
{
    this->money = money;
}

void Money::setCoins(int coins)
{
    this->coins = coins;
}
#pragma endregion
#pragma region operations
istream& operator>>(istream& is, Money& c)
{
    is >> c.money >> c.coins;
    return is;
}

ostream& operator<<(ostream& os, Money& c)
{
    os << "Money: " << c.getMoney() << "Coins: " << c.getCoins() << endl;
    return os;
}

bool Money::operator!=(const Money& other)  {
    return !(*this == other);
}

Money Money::operator+(const Money& other) const {
    int totalcoins = this->getMoney() * 100 + this->getCoins()
            + other.getMoney() * 100 + other.getCoins();
    return {totalcoins / 100, totalcoins % 100};
}

Money Money::operator-(const Money& other) const {
    int totalcoins = this->getMoney() * 100 + this->getCoins()
            - (other.getMoney() * 100 + other.getCoins());
    return {totalcoins / 100, totalcoins % 100};
}

Money Money::operator*(int multiplier) const {
    int totalcoins = (this->getMoney() * 100 + this->getCoins()) * multiplier;
    return {totalcoins / 100, totalcoins % 100};
}

Money Money::operator/(int divisor) const {
    int totalcoins = (this->getMoney() * 100 + this->getCoins()) / divisor;
    return {totalcoins / 100, totalcoins % 100};
}

Money Money::operator%(double percent) const {
    double total = this->getMoney() + this->getCoins() / 100.0;
    total *= percent / 100.0;
    return {(int)(total), (int)((total - (int)(total)) * 100)};
}

bool Money::operator==(const Money &rhs) const {
    return money == rhs.money &&
           coins == rhs.coins;
}

bool Money::operator<(const Money &rhs) const {
    if(this->getMoney() == rhs.getMoney())
    return coins < rhs.coins;

    return this->getMoney() < rhs.getMoney();
}

bool Money::operator>(const Money &rhs) const {
    return rhs < *this;
}

bool Money::operator<=(const Money &rhs) const {
    return !(rhs < *this);
}

bool Money::operator>=(const Money &rhs) const {
    return !(*this < rhs);
}

#pragma endregion