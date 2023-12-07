#include <iostream>
using namespace std;
int main() {
//    int n;
//    cin >> n;
//
//    if (n < 2 || n > 2000) {
//        cout << "Invalid input data!";
//        return 0;
//    }
//
//    int count = 1;
//
//    for (int i = 2; i * i <= n; ++i) {
//        int exponent = 0;
//        while (n % i == 0) {
//            n /= i;
//            exponent++;
//        }
//        count *= (exponent + 1);
//    }
//
//    if (n > 1) {
//        count *= 2;
//    }
//
//    cout << count;

//    int n;
//    cin >> n;
//
//    if (n < 2 || n > 2000) {
//        cout << "Invalid input data!";
//        return 0;
//    }
//
//    int sum = 0;
//
//    for (int i = 2; i <= n; ++i) {
//        if (n % i == 0) {
//            bool isPrime = true;
//            for (int j = 2; j * j <= i; ++j) {
//                if (i % j == 0) {
//                    isPrime = false;
//                    break;
//                }
//            }
//            if (isPrime) {
//                sum += i;
//            }
//        }
//    }
//
//    cout << sum;

    int n;
    cin >> n;

    if (n < 2 || n > 2000) {
        cout << "Invalid input data!";
        return 0;
    }

    int sum = 0;

    for (int i = 2; i < n; ++i) {
        bool isPrime = true;
        for (int j = 2; j * j <= i; ++j) {
            if (i % j == 0) {
                isPrime = false;
                break;
            }
        }
        if (n % i == 0 && !isPrime) {
            sum += i;
        }
    }

    cout << sum;
    return 0;
}
