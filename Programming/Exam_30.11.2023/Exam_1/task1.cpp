#include <iostream>
#include <string>

using namespace std;

int main() {

    string loveSentence, command, value;

    int numberOfCommands;

    getline(cin, loveSentence);

    cin >> numberOfCommands;

    for (int i = 0; i < numberOfCommands; ++i) {
        cin >> command;

        if (command == "add") {
            cin >> value;

            loveSentence += value;
        } else if (command == "multiply") {
            cin >> value;
            string originalSentence = loveSentence;

            for (int j = 0; j < stoi(value) - 1; ++j) {
                loveSentence += originalSentence;
            }

        } else if (command == "delete") {
            cin >> value;

            int valueSize = stoi(value);
            int sentenceSize = loveSentence.size() - 1;

            for (int j = sentenceSize; j >= sentenceSize - valueSize; --j) {
                loveSentence = loveSentence.substr(0, loveSentence.size() - 1);
            }

        } else if (command == "divide") {
            cin >> value;

            int maxIndex = loveSentence.size() / stoi(value);

            loveSentence = loveSentence.substr(0, maxIndex);
        }

    }

    cout << loveSentence;

    return 0;
}
