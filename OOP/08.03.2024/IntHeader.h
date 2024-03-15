#ifndef INC_08_03_2024_INTHEADER_H
#define INC_08_03_2024_INTHEADER_H

class Int {
private:
//    const char* name;
    char* name;
    int x;
public:
    Int();
    Int(int,const char* = "Int");
    Int(unsigned ,const char* = "Unsigned");
    ~Int();

    int getX() const {
        return this->x;
    }


};

#endif //INC_08_03_2024_INTHEADER_H
