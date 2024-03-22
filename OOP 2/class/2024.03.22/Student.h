//
// Created by Radoslav Gurev on 22.03.24.
//

#ifndef INC_2024_03_22_STUDENT_H
#define INC_2024_03_22_STUDENT_H


#include <iostream>
using namespace std;
#include <cstring>

class Student {
private:
    char* name;
    char* facultyNo;
    int grades[3];
public:
    Student();

    Student(char*, char*, int*);

    ~Student();

    char *getName() const;

    char *getFacultyNo() const;

    const int *getGrades() const;

    void setName(char *name);

    void setFacultyNo(char*);

    friend ostream &operator<<(ostream&, const Student&);
    friend istream &operator>>(istream&, Student& );
};

#endif //INC_2024_03_22_STUDENT_H
