#include <iostream>
#include <vector>
using namespace std;

int factorial(int n) {
    int result = 1;
    for (int i = 1; i <= n; i++) {
        result *= i;
    }
    return result;
}

int combination(int n, int k) {
    if (k > n || k < 0) return 0;
    return factorial(n) / (factorial(k) * factorial(n - k));
}

int main() {
    int n, k;
    cin >> n >> k;

    vector<vector<int> > b(n);
    for (int i = 0; i < n; i++) {
        int j;
        cin >> j;
        b[i].resize(j);
        for (int m = 0; m < j; m++) {
            cin >> b[i][m];
        }
    }

    int totalCombinations = 0;

    for (int mask = 0; mask < (1 << n); mask++) {
        vector<int> count(n, 0);
        int numElements = 0;

        for (int i = 0; i < n; i++) {
            if (mask & (1 << i)) {
                numElements++;
                count[i]++;
            }
        }

        if (numElements == k) {
            int currentCombination = 1;

            for (int i = 0; i < n; i++) {
                if (count[i] > 0) {
                    bool valid = false;
                    for (int j = 0; j < b[i].size(); j++) {
                        if (count[i] == b[i][j]) {
                            valid = true;
                            break;
                        }
                    }

                    if (!valid) {
                        currentCombination = 0;
                        break;
                    }
                    currentCombination *= combination(k, count[i]);
                }
            }

            totalCombinations += currentCombination;
        }
    }

    cout << totalCombinations << endl;
    return 0;
}
