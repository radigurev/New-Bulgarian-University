#include <iostream>
using namespace std;

int main() {
    int p, L;
    cin >> p >> L;
    int n = p + 1;
    int q = L / n;
    int r = L % n;
    int sumSq = r * (q + 1) * (q + 1) + (n - r) * q * q;
    int L2 = L * L;
    int area = (L2 - sumSq) / 2;
    cout << area;
    return 0;
}
