#include <iostream>
using namespace std;

int gcd_result; // Global variable to store the result

int gcd(int a, int b) {
    // On descent
    cout << "Entering gcd(" << a << ", " << b << ")" << endl;
    if (b == 0) {
        gcd_result = a;
        // On ascent
        cout << "Returning from gcd(" << a << ", " << b << ") with result " << a << endl;
        return a;
    } else {
        int result = gcd(b, a % b);
        // On ascent
        cout << "Returning from gcd(" << a << ", " << b << ") with result " << result << endl;
        return result;
    }
}

int main() {
    int a, b;
    cout << "Enter two integers: ";
    cin >> a >> b;
    gcd(a, b);
    cout << "GCD of " << a << " and " << b << " is " << gcd_result << endl;
    return 0;
}