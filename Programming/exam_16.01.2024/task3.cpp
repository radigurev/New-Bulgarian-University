#include <iostream>

using namespace std;

void splitSentence(char*, char*,char**,int&,char*&);
void checkWord(char*,char*,char**,int& ,char*&);
bool startsWith(char*, char*);
int getLength(char*);
char* substr(char*, int, int);

int main() {
    char* sentence = new char[100];
    char** allWords = new char*[100];
    char* longestWord = new char[20];
    int wordsCount = 0;

    cin.getline(sentence,100);

    char* syllable = new char[5];

    cin >> syllable;

    splitSentence(sentence,syllable,allWords,wordsCount,longestWord);

    cout << "All words: ";

    for (int i = 0; i < wordsCount; ++i) {
        cout << "\t" << allWords[i];
    }
    cout << endl;
    cout << "Longest word: " << longestWord;
}

void splitSentence(char* sentence, char* syllable, char** words,int& wordsCount,char* &longestWord) {

    if(*sentence == '\0') return;

    char* word = new char [100];

    int i = 0;
    while (*sentence != ' ' && *sentence != '\0') {
        word[i] = *sentence;

        i++;
        sentence++;
    }

    checkWord(word,syllable,words,wordsCount,longestWord);

    sentence++;

    splitSentence(sentence,syllable,words,wordsCount,longestWord);

}

void checkWord(char* word, char* syllable, char** words,int& wordsCount,char* &longestWord) {
    if(strstr(word,syllable)) {
        if(strlen(longestWord) < strlen(word)) {
            longestWord = word;
        }
    }

    if(startsWith(word,syllable)) {
        words[wordsCount] = word;
        wordsCount++;
    }
}

bool startsWith(char* word, char* syllable) {
    int syllableLen = strlen(syllable);

    return strcmp(substr(word,0,syllableLen),syllable) == 0;
}

int getLength(char* word) {
    int i = 0;
    while (*word != '\0') {
        i++;
    }

    return i;
}

char* substr(char* arr, int begin, int len)
{
    char* res = new char[len + 1];
    for (int i = 0; i < len; i++)
        res[i] = *(arr + begin + i);
    res[len] = 0;
    return res;
}