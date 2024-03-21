
#include <iostream>

using namespace std;

int main() {

    //1
    cout << "Radoslav Georgiev Gurev" << endl << endl;

    //2
    cout << "University: New Bulgarian University" << endl;
    cout << "Program: Informatics" << endl << endl;

    //3
    cout << "Key words: auto double break else case enum"
            " char extern const float continue for default"
            " goto do if int long register return short signed"
            " sizeof static struct switch typedef union unsigned"
            " void volatile while asm bool catch class const_cast"
            " delete dynamic_cast explicit false friend inline mutable"
            " namespace new operator private protected public reinterpret_cast"
            " static_cast template this throw true try typeid typename using virtual wchar_t" << endl << endl;

    //4
    cout << "Lexemes: From the formations of language comes the construction of words through specific rules." << endl;

    cout << "Literal: A literal is a symbol that represents a specific value, such as a number, text or letter.  During "
            "compilation, literals are transformed into an internal binary representation of their corresponding values." << endl;

    cout << "Command: A sequence of lexemes, formed according to the rules, ending with ';' as a separating character.";

    return  0;
}

//5

/*
1. #define: It is used to define preprocessor macros. When the preprocessor encounters this directive, it replaces any occurrence of the identifier in the rest of the code by the replacement[^1^][1][^2^][2].

2. #undef: This directive is used to undefine a macro. A macro lasts until it is undefined with the #undef preprocessor directive[^1^][1][^3^][9].

3. #include: This directive instructs the preprocessor to include a section of standard C++ code, known as header, that allows performing standard input and output operations[^4^][14].

4. #if: This directive serves to specify some condition to be met in order for the portion of code they surround to be compiled. The condition that follows #if can only evaluate constant expressions, including macro expressions[^1^][1].

5. #ifdef: This directive allows a section of a program to be compiled only if the macro that is specified as the parameter has been defined, no matter what its value is[^1^][1].

6. #ifndef: This directive allows a section of a program to be compiled only if the macro that is specified as the parameter has not been defined[^1^][1].

7. #else: This directive is used in conjunction with #if or #ifdef or #ifndef to specify a condition when the condition for #if or #ifdef or #ifndef is not met[^1^][1].

8. #elif: This directive (i.e., "else if") serves to specify some condition to be met in order for the portion of code they surround to be compiled[^1^][1].

9. #endif: This directive indicates the end of conditional preprocessor block started by #if, #ifdef or #ifndef[^1^][1].

10. #line: This directive alters the compiler's perception of line number and filename, but it does not change the actual contents of your files[^5^][16].

11. #error: This directive aborts compilation if it is encountered, outputting an error message you supply.

12. #pragma: This directive specifies diverse options to the compiler. These options are specific for the platform and the compiler you use.
*/