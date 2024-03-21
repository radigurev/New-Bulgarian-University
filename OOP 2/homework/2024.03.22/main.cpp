#include <iostream>

#include "Money.h"
#include "BigInteger.h"

int main() {
    Money a;
    cin >> a;
    cout << a;

    Money b;
    cin >> b;
    cout << b;

    Money sum = a + b;
    cout << "Operation +: " << sum;
    Money substract = a-b;
    cout << "Operation -: " << substract;
    Money multiply = a * 5;
    cout << "Operation *: " << multiply;
    Money divide = a * 5;
    cout << "Operation /: " << divide;
    cout << "Operation ==: " << (a == b);
    cout << "Operation >: " << (a > b);
    cout << "Operation <: " << (a < b);
    cout << "Operation <=: " << (a <= b);
    cout << "Operation >=: " << (a >= b);
    Money percent = a % 50;
    cout << "Operation >=: " << percent;

    BigInteger x("123456789");
    BigInteger y("987654321");

    cout << x * y;
    return 0;
}
