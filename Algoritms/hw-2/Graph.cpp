//
// Created by Radoslav Gurev on 6.06.25.
//

#pragma once
#include <vector>
#include <list>

using namespace std;

class Graph {
public:
    explicit Graph(std::size_t n);

    void addEdge(std::size_t u, std::size_t v);
    std::list<std::size_t> eulerCycle();

private:
    std::size_t n_;
    std::vector<std::vector<int>> A_;

    bool isEuler() const;
    std::size_t nextEdge(std::size_t v);
};