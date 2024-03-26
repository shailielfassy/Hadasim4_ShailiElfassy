//Shaili Elfassy
//hadasim 4.0 Ex2
#include <iostream>
#include<cmath>
#include "Hadasim4Ex2.h"
using namespace std;

//the function calculates the amount of odd numbers between 1 and given width
int countOddNumBetween2(int width) {
	int count=0;
	for (int i = width; i > 1; i=i-2) 
		count++;
	return count-1;
}

//the height of the tower must be bigger or equal 2 
void checkHeight(int& height)
{
	while (height < 2) {
		cout << "height must be bigger or equal 2" << endl << "enter valid height" << endl;
		cin >> height;
	}
}

int main() {
	int num, num2, height, width;
	cout << "Choose your option:" << endl
		<< "1. Press 1 to build a rectangular tower" << endl
		<< "2. Press 2 to build a triangular tower" << endl
		<< "3. Press 3 to exit" << endl;
	cin >> num;
	while (num != 3) {
		switch (num)
		{
		case 1: //rectangle
			cout << "enter the height of the rectangle" << endl;
			cin >> height;
			checkHeight(height);
			cout << "enter the width of the rectangle" << endl;
			cin >> width;
			//if it's a square or a rectangle with sides difference that are greater than 5 then print rectangle's area
			if (height == width || abs(height - width) > 5) 
				cout << "the area of the rectangle is: "<< width * height <<endl;
			else //otherwise print the rectangle's perimeter
				cout << "the perimeter of the rectangle is:" << 2 * width + 2 * height << endl;
			break;

		case 2: //triangle
			cout << "enter the height of the triangle" << endl;
			cin >> height;
			checkHeight(height);
			cout << "enter the width of the triangle" << endl;
			cin >> width;
			cout << "Choose your option:" << endl
				<< "1. Press 1 to get triangle's perimeter" << endl
				<< "2. Press 2 to print the triangle" << endl;
			cin >> num2;
			if (num2 == 1) { //print the triangle's perimeter
				double edge = sqrt(pow(width / 2, 2) + pow(height, 2)); //calculate the edge according to the Pythagorean theorem 
				cout << "the perimeter of the triangle is:" << (2 * edge) + width << endl;
			}
			else { 
				//if the width is even or that the width is greater than twice the height we can't print the triangle
				if (width % 2 == 0 || width > 2 * height)
					cout << "can't print the triangle" << endl;
				else { //print the triangle
					int count = countOddNumBetween2(width);
					for (int s = count + 1; s > 0; s--) //print spaces before the first line
						cout << " ";
					cout << "*" << endl; //print first line

					int space = count - 1;
					if (count == 0)
						count = 1;
					int x = (height - 2) / count; //x lines from each number (but 3) between 1 and width
					int print3 = (height - 2) - x * (count - 1); //print3 is the number of lines of 3
					for (int i = 0; i < print3; i++) {
						for (int s = space+1; s > 0; s--) //print spaces
							cout << " ";
						cout << "***" << endl;
					}

					for (int i = 5; i < width ; i=i+2) { //from 5 till width-2
						for (int j = 0; j < x; j++) {
							for (int s = space; s > 0; s--) //print spaces
								cout << " ";
							for (int t = 0; t < i; t++)
								cout << "*";
							cout << endl;
						}
						space -= 1;
					}

					for(int i=0; i< width; i++) //print last line
						cout << "*";
					cout << endl;
				}
			}
			break;
		default:
			cout << "enter a valid number" << endl;
			break;
		}
		cout << "Choose your option:" << endl
			<< "1. Press 1 to build a rectangular tower" << endl
			<< "2. Press 2 to build a triangular tower" << endl
			<< "3. Press 3 to exit" << endl;
		cin >> num;
	}
	return 0;
}