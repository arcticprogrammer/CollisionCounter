/* ===========================
 * CollisionCounter.cs
 * written by Arcticprogrammer
 * 16 Feb 2023
 * 
 * CollisionCounter generates a user-defined set of integers over
 * multiple generations and counts the number of times a collision
 * occurs within each set of generated integers. It then displays
 * the percentage of conflicting integers across all generated 
 * sets. 
 * 
 * This program provides the option to use either the Random 
 * object or the RandomNumberGenerator interface.
 * 
 * Individual integer collisions can be displayed by uncommenting 
 * the relevant lines of code.
 */

using System;
using System.Collections;
using System.Security.Cryptography;

namespace CollisionCounter
{
    class Program
    {
        private static readonly Random R = new Random();

        static void Main(string[] args)
        {

            GetInputs();
            Console.ReadKey();
        }

        static void GetInputs()
        {
            bool validInput = false;
            int upperBound;
            int repeats;
            int totalCollisions = 0;
            string input;

            //get upper bound
            do
            {
                Console.WriteLine("How many numbers to generate?");
                upperBound = Convert.ToInt32(Console.ReadLine());
                if (upperBound < 0)
                    Console.WriteLine("Invalid input: cannot be less than 0!");
                else if (upperBound == 0)
                    Console.WriteLine("Invalid input: cannot generate 0 numbers!");
                else validInput = true;
            } while (validInput == false);

            //get how many times to repeat
            validInput = false;
            do
            {
                Console.WriteLine("How many times?");
                repeats = Convert.ToInt32(Console.ReadLine());
                if (repeats < 0)
                    Console.WriteLine("Invalid input: cannot be less than 0!");
                else validInput = true;
            } while (validInput == false);

            //use pseudo or secure random?
            validInput = false;
            bool useSecureRandom = false;
            do
            {
                Console.Write("Use SecureRandom? (Y/N) ");
                input = Console.ReadLine();
                if (input.Equals("Y") || input.Equals("y"))
                {
                    useSecureRandom = true;
                    validInput = true;
                }

                if (input.Equals("N") || input.Equals("n"))
                {
                    //useSecureRandom = false;
                    validInput = true;
                }

                if (validInput == false)
                    Console.WriteLine("Invalid input. Must be Y or N.");

            } while (validInput == false);


            //run findCollisions using the array list from generateNumList, i number of times
            int i = 0;
            ArrayList numList;

            do
            {
                numList = GenerateNumList(upperBound, useSecureRandom);
                totalCollisions += FindCollisions(numList);
                i++;
            } while (i < repeats);

            Console.WriteLine("Average collision percentage is: " + AverageCollisionPercentage(totalCollisions, upperBound, repeats) + "%");
        }


        static ArrayList GenerateNumList(int upperBound, bool useSecureRandom)
        {
            //decl
            ArrayList numList = new ArrayList();

            //generate numbers
            if (useSecureRandom == true)
            {
                byte[] bytes = new byte[4];
                int number;
                RandomNumberGenerator rng = RandomNumberGenerator.Create();
                for (int i = 0; i < upperBound; i++)
                {
                    rng.GetBytes(bytes);
                    number = BitConverter.ToInt32(bytes) % upperBound;
                    numList.Add(number);
                }
            }
            else
            {
                for (int i = 0; i < upperBound; i++)
                    numList.Add(R.Next(upperBound));
            }

            PrintNumList(numList);
            return numList;

        }

        static int FindCollisions(ArrayList numList)
        {
            //sort arraylist
            numList.Sort();
            //printNumList(numList);

            //get index of number and its neighbor
            int heldIndex = 0;
            int comparedIndex = heldIndex;
            int localCollisions;
            int totalCollisions = 0;

            //1.1 largest collisions
            int largestCollisionValue = 0;
            int largestCollisionCount = 0;

            do
            {
                int numberInHeldIndex = (int)numList[heldIndex];
                int numberInComparedIndex = (int)numList[comparedIndex];

                //if numbers in both indicies are same, move comparedIndex 1 spot
                if (numberInHeldIndex == numberInComparedIndex)
                    comparedIndex++;

                //special case: if final number is the same as previous number
                if (comparedIndex == numList.Count)
                {
                    localCollisions = CollisionCount(comparedIndex, heldIndex);
                    totalCollisions += localCollisions;

                    //then break out of do-while
                    break;
                }

                //if numbers are different, subtract comparedIndex with heldIndex for number of elements
                if (numberInHeldIndex != numberInComparedIndex)
                {
                    localCollisions = CollisionCount(comparedIndex, heldIndex);
                    totalCollisions += localCollisions;

                    //then go to next number
                    heldIndex = comparedIndex++;
                    comparedIndex = heldIndex;
                }

                //special case: if final number, AND it is not the same as the previous number, print it with 0 collisions
                if (heldIndex == numList.Count && numberInHeldIndex != (int)numList[heldIndex - 1])
                {
                    localCollisions = CollisionCount(comparedIndex, heldIndex);
                    totalCollisions += localCollisions;
                }

                //if current largest collision count is less than compared number's collision count, update largest count to new number
                if (largestCollisionCount < CollisionCount(comparedIndex, heldIndex))
                {
                    largestCollisionValue = numberInHeldIndex;
                    largestCollisionCount = CollisionCount(comparedIndex, heldIndex);
                }

                //repeat while (heldIndex + 1) is not out of bounds
            } while (heldIndex < numList.Count);

            PrintL2CollisionPrompt(totalCollisions, largestCollisionCount, largestCollisionValue);

            return totalCollisions;
        }

        //QOL
        static void PrintNumList(ArrayList numList)
        {
            int i = 1;
            //numList.Sort();
            foreach (int num in numList)
            {
                Console.WriteLine(i + ": " + num);
                i++;
            }
            Console.WriteLine("There are " + numList.Count + " numbers in this list.");
        }

        static void PrintL1CollisionPrompt(int num, int collisions)
        {
            Console.WriteLine("Num: " + num + ", Collisions: " + collisions);
            //return null;
        }

        static void PrintL2CollisionPrompt(int totalCollisions, int largestCollisionCount, int largestCollisionValue)
        {
            Console.WriteLine("Collisions: " + totalCollisions + ". Largest number of collisions: " + largestCollisionCount + " for the number " + largestCollisionValue);
            //return null;
        }

        static int CollisionCount(int upperIndex, int lowerIndex)
        {
            return upperIndex - lowerIndex - 1;
        }

        static double AverageCollisionPercentage(int totalCollisions, int upperBound, int repeats)
        {
            double average = (double)totalCollisions / (double)(repeats * upperBound) * 100.00;
            return average;
        }
    }
}
