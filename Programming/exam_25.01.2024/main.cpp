#include <iostream>

using namespace std;

namespace commands {
    char story[] = "Tell a personal story";
    char task[] = "Create a task";
    char exam[] = "Do a exam";
    char TS[] = "aylor Swift";
    char CCT[] = "ancer Culture Task";
    char RS[] = "Random Stuff";
}

void enterStudents(string *, int &);

void printArray(string *);

void transferStudents(string *, string *, string,int&,int&);

void enterStory(string *, string *, int&, int&);

bool contains(string *, int, string &);

void removeStudent(string *, int &, string);

void addStudent(string *, int &, string);

void createTask(string *, string *, int&, int&);

void doExam(string*,string*,int&,int&);

int main() {
    string *students = new string[22]{"\0"};
    string *motivated = new string[22]{"\0"};
    string *coming = new string[22]{"\0"};
    string *business = new string[22]{"\0"};
    int motivatedSize = 0, comingSize = 0, businessSize = 0, studentSize = 0;

    enterStudents(students, studentSize);

//    printArray(students);

    string command;
    cin.ignore();
    getline(cin,command);
    while (command != "End") {
        if (commands::story == command) {
            enterStory(students, motivated, motivatedSize, studentSize);
        } else if (commands::task == command ) {
            createTask(students,coming,comingSize,studentSize);
        } else if (commands::exam == command) {
            doExam(students,business,businessSize,studentSize);
        }
        getline(cin,command);
    }

    cout << "Motivated: ";
    printArray(motivated);
    cout << endl;

    cout << "Just coming: ";
    printArray(coming);
    cout << endl;

    cout << "Studying Business: ";
    printArray(business);
    cout << endl;
    return 0;
}

void enterStudents(string *students, int &studentSize) {

    string command = "";

    while (cin >> command && command != "End") {
        students[studentSize] = command;

        studentSize++;
    }
}

void printArray(string *array) {
    int i = 0;

    while (true) {
        if (array[i].empty()) return;
        cout << array[i] << " ";
        i++;
    }
}

void enterStory(string *students, string *motivated, int& motivatedSize, int& studentSize) {
    string command;

    while (cin >> command && command != "End") {
        if (contains(students, studentSize, command)) {
            transferStudents(students, motivated, command,studentSize,motivatedSize);
        }
    }
}

void createTask(string *students, string *coming, int& comingSize, int& studentSize) {
    string command;
    cin.ignore();
    getline(cin,command);
    while (command != "End") {

        if (command == commands::TS) {
            addStudent(coming, comingSize, students[studentSize - 1]);
            addStudent(coming, comingSize, students[studentSize - 2]);

            students[studentSize - 1] = "";
            students[studentSize - 2] = "";
        } else if (command == commands::CCT) {
            addStudent(coming, comingSize, students[studentSize - 1]);
            addStudent(coming, comingSize, students[studentSize - 2]);

            int cnt = 0;
            int i = 0;
            while (cnt != 2) {
                if (!students[i].empty()) {
                    cnt++;
                    students[i] = "";
                }
                i++;
            }
        }
        getline(cin,command);
    }
}

void doExam(string *students, string *business, int& businessSize, int& studentSize) {
    string command;

    while (cin >> command && command != "End") {
        if(!contains(students,studentSize,command)) continue;

        addStudent(business,businessSize,command);
        removeStudent(students,studentSize,command);
    }
}

void transferStudents(string *transferFrom, string *transferTo, string name, int& sizeFrom, int& sizeTo) {
    addStudent(transferTo, sizeTo, name);
    removeStudent(transferFrom, sizeFrom, name);
}

bool contains(string *array, int size, string &target) {
    for (int i = 0; i < size; ++i) {
        if (array[i] == target) {
            return true;
        }
    }
    return false;
}

void addStudent(string *array, int &size, string name) {
    array[size] = name;
    size++;
}

void removeStudent(string *array, int &size, string name) {
    for (int i = 0; i < size; ++i) {
        if (array[size] == name) {
            array[size] = "";
            return;
        }
    }
}