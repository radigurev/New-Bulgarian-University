#include <vector>
#include <stack>
#include <list>
#include <iostream>

using namespace std;

class Graph {
public:
    explicit Graph(size_t n);

    void addEdge(size_t u, size_t v);
    list<size_t> eulerCycle();

private:
    size_t n_;
    vector<vector<int>> A_;

    bool isEuler() const;
    size_t nextEdge(size_t v);
};

Graph::Graph(size_t n)
        : n_(n), A_(n_ + 1, vector<int>(n_ + 1, 0)) {
    for (size_t i = 1; i <= n_; ++i) A_[i][0] = 1;
}

void Graph::addEdge(size_t u, size_t v) {
    ++A_[u][v];
    ++A_[v][u];
}

bool Graph::isEuler() const {
    for (size_t v = 1; v <= n_; ++v) {
        int deg = 0;
        for (size_t u = 1; u <= n_; ++u) deg += A_[v][u];
        if (deg & 1) return false;
    }
    return true;
}

size_t Graph::nextEdge(size_t v) {
    while (A_[v][0] <= n_ && A_[v][A_[v][0]] == 0) ++A_[v][0];
    size_t u = 0;
    if (A_[v][0] <= n_) {
        u = A_[v][0];
        --A_[v][u];
        --A_[u][v];
    }
    return u;
}

list<size_t> Graph::eulerCycle() {
    list<size_t> C;
    if (!isEuler()) return C;

    stack<size_t> S;
    S.push(1);
    while (!S.empty()) {
        size_t v = nextEdge(S.top());
        if (v != 0) S.push(v);
        else { C.push_front(S.top()); S.pop(); }
    }
    return C;
}

int main() {
    size_t n, m;
    if (!(cin >> n >> m)) return 0;
    Graph g(n);
    for (size_t i = 0; i < m; ++i) {
        size_t u, v;  cin >> u >> v;
        g.addEdge(u, v);
    }
    auto cyc = g.eulerCycle();
    if (cyc.empty()) { cout << "no Euler cycle\n"; return 0; }
    for (auto v : cyc) cout << v << ' ';
    cout << '\n';
    return 0;
}
