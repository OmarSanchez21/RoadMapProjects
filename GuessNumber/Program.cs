using System;
using System.Diagnostics;
namespace GuessNumber
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Number Guessing Game!");
            bool playAgain = true;
            int highScore = int.MaxValue;

            while(playAgain)
            {
                Console.WriteLine("Please select the difficulty level:");
                Console.WriteLine("1. Easy (10 chances)");
                Console.WriteLine("2. Medium (5 chances)");
                Console.WriteLine("3. Hard (3 chances)");
                int chances = GetDifficultyLevel();
                int targetNumber = new Random().Next(1,101);
                int attempts = 0;
                bool isCorrect = false;

                Console.WriteLine("\nI'm thinking of a number between 1 and 100");
                Console.WriteLine($"You have {chances} chances to guess the correct number.");

                Stopwatch timer = Stopwatch.StartNew();  
                while (chances > 0)
                {
                    Console.Write("Enter your guess: ");
                    int guess = GetValidGuess();

                    attempts++;
                    chances--;

                    if (guess == targetNumber)
                    {
                        timer.Stop();
                        Console.WriteLine($"\nCongratulations! You guessed the correct number in {attempts} attempts.");
                        Console.WriteLine($"It took you {timer.Elapsed.Seconds} seconds.");

                        if (attempts < highScore)
                        {
                            highScore = attempts;
                            Console.WriteLine("New high score!");
                        }
                        isCorrect = true;
                        break;
                    }
                    else if (guess > targetNumber)
                    {
                        Console.WriteLine("Incorrect! The number is less than " + guess + ".");
                    }
                    else
                    {
                        Console.WriteLine("Incorrect! The number is greater than " + guess + ".");
                    }

                    Console.WriteLine($"You have {chances} chances left.");
                }

                if (!isCorrect)
                {
                    Console.WriteLine($"\nSorry, you've run out of chances. The correct number was {targetNumber}.");
                }

                Console.Write("\nDo you want to play again? (y/n): ");
                playAgain = Console.ReadLine()?.ToLower() == "y";
            }

            Console.WriteLine("Thanks for playing! Goodbye!");
        }

        static int GetDifficultyLevel()
        {
            while (true)
            {
                Console.Write("Enter your choice (1, 2, 3): ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        Console.WriteLine("\nGreat! You have selected the Easy difficulty level.");
                        return 10;
                    case "2":
                        Console.WriteLine("\nGreat! You have selected the Medium difficulty level.");
                        return 5;
                    case "3":
                        Console.WriteLine("\nGreat! You have selected the Hard difficulty level.");
                        return 3;
                    default:
                        Console.WriteLine("Invalid choice. Please choose a valid difficulty level.");
                        break;
                }
            }
        }

        static int GetValidGuess()
        {
            while (true)
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out int guess) && guess >= 1 && guess <= 100)
                {
                    return guess;
                }
                else
                {
                    Console.WriteLine("Please enter a valid number between 1 and 100.");
                }
            }
        }
    }
}