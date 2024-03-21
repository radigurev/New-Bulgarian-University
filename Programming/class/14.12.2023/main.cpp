#include <iostream>

using namespace std;

enum roles {
    CREATOR, USER
};

int checkIfAlreadyExists(string usernames[], string username, int n) {

    int i;
    for (i = 0; i < n; ++i) {
        if (usernames[i] == username) break;
        else if (i == n - 1) return -1;
    }

    return i;
}

void registerUser(string usernames[], string passwords[],
                  roles types[], string username, string password,
                  roles type, int n, int &currentPosition) {

    if (checkIfAlreadyExists(usernames, username, n) != -1) {
        cout << username << " already exists!" << endl;
        return;
    }

    usernames[currentPosition] = username;
    passwords[currentPosition] = password;
    types[currentPosition] = type;

    cout << username << " registered successfully" << endl;

    currentPosition++;
}

void loginUser(string usernames[], string passwords[],
               string username, string password,
               int n) {
    int index = checkIfAlreadyExists(usernames, username, n);

    if (index == -1 || passwords[index] != password) {
        cout << "Username or password is invalid." << endl;
        return;
    }
    cout << username << " logged in." << endl;
}

void FollowCreator(string usernames[], roles types[], string userOne, string userTwo, int n) {

    int userOneIndex = checkIfAlreadyExists(usernames, userOne, n);

    if(userOneIndex == -1) return;

    int userTwoIndex = checkIfAlreadyExists(usernames, userTwo, n);

    if(userTwoIndex == -1) return;

    roles user = USER;

    if(types[userOneIndex] == user && types[userTwoIndex] == user) {
        cout << "Normal users can follow only creators" << endl;
        return;
    }

    cout << userOne << ", " << userTwo << endl;

}

int main() {

    int n;

    cin >> n;

    string *usernames = new string[n];
    string *passwords = new string[n];
    roles *types = new roles[n];
    int currentPosition = 0;
    roles userRole = USER;
    roles creatorRole = CREATOR;

    registerUser(usernames, passwords, types, "NoBalloonsUnused96", "pass", userRole, n, currentPosition);

    registerUser(usernames, passwords, types, "Ho_logram_hunter00", "pass2", userRole, n, currentPosition);

    registerUser(usernames, passwords, types, "Cardi B", "pass3", userRole, n, currentPosition);

    registerUser(usernames, passwords, types, "Cardi B", "pass3", creatorRole, n, currentPosition);

    registerUser(usernames, passwords, types, "The Rock", "pass4", creatorRole, n, currentPosition);

    loginUser(usernames, passwords, "asda", "aaa", n);

    loginUser(usernames, passwords, "NoBalloonsUnused96", "aaa", n);

    loginUser(usernames, passwords, "NoBalloonsUnused96", "pass", n);

    FollowCreator(usernames,types,"The Rock","Cardi B",n);

    FollowCreator(usernames,types,"NoBalloonsUnused96","Ho_logram_hunter00",n);

    FollowCreator(usernames,types,"NoBalloonsUnused96","The Rock",n);

    return 0;
}
