#include "Student.h"

//
// Created by Radoslav Gurev on 22.03.24.
//
Student::Student(char *name, char *facultyNo, int grades[3]) {
    this->name = new char[strlen(name) + 1];
    strcpy(this->name,name);

    this->facultyNo = new char[strlen(this->facultyNo + 1)];
    strcpy(this->facultyNo,facultyNo);

    for (int i = 0; i < 3; ++i) {
        this->grades[i] = grades[i];
    }
}

Student::Student() {
    this->name = new char[1];
    strcpy(this->name,"");

    this->facultyNo = new char[1];
    strcpy(this->facultyNo,"");

}

char *Student::getName() const {
    return name;
}

void Student::setName(char *name) {
    this->name = name;
}

char *Student::getFacultyNo() const {
    return facultyNo;
}

void Student::setFacultyNo(char *facultyNo) {
    this->facultyNo = facultyNo;
}

const int *Student::getGrades() const {
    return grades;
}

ostream &operator<<(ostream &os, const Student &student) {
    os << "name: " << student.name << " facultyNo: " << student.facultyNo << " grades: ";
    for (int i = 0; i < 3; ++i) {
        os << student.grades[i] <<" ";
    }
    os << endl;

    return os;
}

istream &operator>>(istream &is, Student &student) {
    is >> student.name >> student.facultyNo;

    for (int i = 0; i < sizeof(student.grades) / sizeof(int); ++i) {
        cout << "Enter grade["<<(i)<<"]: ";
        is >> student.grades[i];
    }

    return is;
}

Student::~Student() {
    delete[] name;
    delete[] facultyNo;
}
