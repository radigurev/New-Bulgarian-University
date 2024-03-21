
#include <iostream>
#include <list>

using namespace std;


int main() {

	//1
	int taskOneNumber;
	int count = 0;

	cout << "Enter a number: ";
	cin >> taskOneNumber;

	do {
		taskOneNumber /= 10;
		count++;
	} while (taskOneNumber != 0);

	cout << "Number of digits: " << count << endl;

	//2
	int taskTwoNumber, original;
	int sum = 0;

	cout << "Enter an integer: ";
	cin >> taskTwoNumber;

	original = taskTwoNumber;

	while (taskTwoNumber != 0) {
		int digit = taskTwoNumber % 10;
		sum += digit;
		taskTwoNumber /= 10;
	}

	cout << "The sum of the digits of " << original << " is: " << sum << endl;

	//3
	int taskThreeNumber, reversedNum = 0;

	cout << "Enter an integer: ";
	cin >> taskThreeNumber;

	while (taskThreeNumber != 0) {
		int digit = taskThreeNumber % 10;
		reversedNum = reversedNum * 10 + digit;
		taskThreeNumber /= 10;
	}

	cout << "Reversed integer: " << reversedNum << endl;

	//4
	int taskFourNumber;

	cout << "Enter an integer: ";
	cin >> taskFourNumber;

	if (taskFourNumber < 0) {
		taskFourNumber = -taskFourNumber;
	}

	int count0 = 0, count1 = 0, count2 = 0, count3 = 0, count4 = 0, count5 = 0, count6 = 0, count7 = 0, count8 = 0, count9 = 0;

	while (taskFourNumber != 0) {
		int digit = taskFourNumber % 10;
		switch (digit) {
		case 0:
			count0++;
			break;
		case 1:
			count1++;
			break;
		case 2:
			count2++;
			break;
		case 3:
			count3++;
			break;
		case 4:
			count4++;
			break;
		case 5:
			count5++;
			break;
		case 6:
			count6++;
			break;
		case 7:
			count7++;
			break;
		case 8:
			count8++;
			break;
		case 9:
			count9++;
			break;
		}
		taskFourNumber /= 10;
	}

	cout << "Digit/Count" << endl;
	cout << "0/" << count0 << ", 1/" << count1 << ", 2/" << count2 << ", 3/" << count3 << ", 4/" << count4 << ", 5/" << count5 << ", 6/" << count6 << ", 7/" << count7 << ", 8/" << count8 << ", 9/" << count9 << endl;

	//5

	int numLines;
	char symbol;

	cout << "Enter the number of lines: ";
	cin >> numLines;

	cout << "Enter the symbol to print: ";
	cin >> symbol;

	for (int i = 1; i <= numLines; i++) {

		for (int j = 1; j <= numLines - i; j++) {
			cout << " ";
		}

		for (int k = 1; k <= 2 * i - 1; k++) {
			cout << symbol;
		}

		cout << endl;
	}
	//6
	int rows;

	do {
		cout << "Enter the number for rectangle: ";
		cin >> rows;
	} while (rows > 9 && rows < 1);

	for (int i = 0; i <= rows;i++) {
		for (int k = 1; k <= rows;k++) {
			if (i == 0 || k > i) {
				cout << "0";
				continue;
			}
			cout << k;
		}

		for (int k = rows; k > 0; k--) {
			if (i == 0 || k > i) {
				cout << "0";
				continue;
			}
			cout << k;

		}
		cout << endl;

	}


	//6*
	int numRows;
	do {
		cout << "Enter the number for another rectangle: ";
		cin >> numRows;
	} while (numRows < 0);

	for (int i = 0; i <= numRows; i++) {
		for (int j = 1; j <= numRows; j++) {
			if (i == 0 || j > i) {
				cout << "0";
				continue;
			}
			cout << j;
		}
		for (int k = numRows - 1; k > 0; k--) {
			if (i == 0 || k > i) {
				cout << "0";
				continue;
			}
			cout << k;
		}
		cout << endl;
	}
	//7
	double a;
	double m_value;
	double n_value;
	double sumTaskSeven = 0;
	double product = 1;

	cout << "Enter a: ";
	cin >> a;
	cout << "Enter m: ";
	cin >> m_value;
	cout << "Enter n: ";
	cin >> n_value;

	for (int i = 1; i <= n_value; i++) {
		for (int j = 1; j <= m_value; j++) {
			product *= (a + j) / (i + j);
		}
		sumTaskSeven += product;
	}

	cout << "Final: " << product << endl;
	cout << "The result is: " << sumTaskSeven << endl;


	//8
	int n;

	cout << "Enter the number of positive integers: ";
	cin >> n;

	int gcd, nextNumber;

	cout << "Enter the first positive integer: ";
	cin >> gcd;


	for (int i = 2; i <= n; i++) {
		cout << "Enter the next positive integer: ";
		cin >> nextNumber;

		while (nextNumber != 0) {
			int temp = nextNumber;
			nextNumber = gcd % nextNumber;
			gcd = temp;
		}
	}

	cout << "The GCD of the " << n << " positive integers is: " << gcd << endl;

	//9
	int start, end;

	cout << "Enter the starting number (0 < start < end): ";
	cin >> start;

	cout << "Enter the ending number (start < end): ";
	cin >> end;

	cout << "Integers with unique digits in the interval [" << start << ", " << end << "]: " << endl;

	for (int num = start; num <= end; num++) {
		int temp = num;
		bool unique = true;
		list<int> digitList;

		while (temp > 0) {
			int digit = temp % 10;
			if (find(digitList.begin(), digitList.end(), digit) != digitList.end()) {
				unique = false;
				break;
			}
			digitList.push_back(digit);
			temp /= 10;
		}

		if (unique) {
			cout << num << " ";
		}
	}

	cout << endl;


	// 10
	int desiredCount;

	cout << "Enter the desired number of prime numbers (desiredCount > 1): ";
	cin >> desiredCount;

	cout << "The first " << desiredCount << " prime numbers that start with 3 are: " << endl;

	int primeCount = 0;
	int candidate = 3;
	int numberToIncrement = 3;

	while (primeCount < desiredCount) {
		
		int currentNumber = numberToIncrement;
		while (currentNumber > 0)
		{
			int digit = currentNumber % 10;
			currentNumber /= 10;

			if (currentNumber == 0 && digit == 3) {
				cout << numberToIncrement << " ";
				primeCount++;
			}
		}

		numberToIncrement++;
	}

	return 0;
}

