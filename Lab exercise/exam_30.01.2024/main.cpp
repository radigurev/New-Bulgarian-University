#include <iostream>
using namespace std;

int sum(int start,int end,int n,int* set) {
    int sum = start + end;

    for (int i = 1; i < n-1; ++i) {
        if(set[i] >= start && set[i] <= end) sum += set[i];
    }

    return sum;
}

int main() {

    int n, start = 0, end = 0;

    cout << "Enter set count: ";
    cin >> n;

    int* set = new int[n];

    for (int i = 0; i < n; ++i) cin >> set[i];

    start = set[0];
    end = set[n-1];

    cout << sum(start,end,n,set);
    return 0;
}