#include "IntHeader.h"
#include "iostream"
#include <cstring>
using namespace std;

//Int::Int():x(0),name(" ") {
Int::Int():x(0),name(new char [2]) {
    strcpy(this->name," ");
//    this->x = 0;
//    this->name = "";

    cout << "Default constructor" << endl;
}

Int::Int(int x,const char* name):x(x),name(new char[strlen(name)+1]) {
//    this->name = new char[strlen(name)+1];
    strcpy(this->name, name);
//    strcpy(this->name,name);
cout << this->name << " constructor" << endl;
}

Int::Int(unsigned z, const char* name):x(x) {
    strcpy_s(this->name, name);

    cout << this->name << " constructor" << endl;
}

Int::~Int() {
//    delete[] name;
    cout << "Destructor" << endl;
}