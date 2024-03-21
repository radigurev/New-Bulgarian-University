#ifndef INC_29_02_2024_PERSON_H
#define INC_29_02_2024_PERSON_H

class Person {
private:
    const char* name;
    double height;
    double weight;
public:
    Person();
    Person(const char*,double,double);
    ~Person();
    const char* getName();

    double getHeight() const;

    double getWeight() const;

    void setName(char*);

    void setHeight(double height);

    void setWeight(double weight);
};

#endif //INC_29_02_2024_PERSON_H
