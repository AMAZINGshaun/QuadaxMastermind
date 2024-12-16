using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadaxMastermind
{
    internal class Mastermind
    {
        public int NumTurns { get; private set; }

        private int[] SecretCode { get; set; }

        private ConsoleColor DefaultFontColor { get; set; } = ConsoleColor.Gray;
        private ConsoleColor TurnLabelColor { get; set; } = ConsoleColor.Cyan;

        /// <summary>
        /// Default Constructor to initiate a 10 turn game
        /// </summary>
        public Mastermind()
        {
            NumTurns = 10;
            SecretCode = GenerateSecretCode();
        }

        /// <summary>
        /// A Constructor to initiate the game with a non-default number of turns
        /// </summary>
        /// <param name="numTurns">The number of turns the game should continue for. Default value is 10 turns</param>
        public Mastermind(int numTurns = 10)
        {
            NumTurns = numTurns;
            SecretCode = GenerateSecretCode();
        }

        /// <summary>
        /// Begin the game
        /// 
        /// This method with handle the main game loop
        /// As part of that, we'll make calls to handle game intro text and win/loss condition checking with appropriate text displayed to the user
        /// </summary>
        public void Play()
        {
            CreateIntroductionText();

            // If this was a real game, I wouldn't include this
            Console.Write($"The secret code is currently: ");
            DisplaySecretCode();
            Console.WriteLine();

            // The Game Loop
            int currentTurn = 0;
            bool playerHasWon = false;
            do
            {
                Console.ForegroundColor = TurnLabelColor;
                Console.WriteLine($"Turn {currentTurn + 1}:");

                Console.ForegroundColor = DefaultFontColor;
                playerHasWon = GetUserInputWithValidation();

                if (playerHasWon == true)
                {
                    DisplayWinTextAndReset(currentTurn + 1);
                }
                else
                {
                    if (currentTurn + 1 == NumTurns)
                    {
                        DisplayLossTextAndReset();
                        break;
                    }

                    currentTurn++;
                }
            }
            while (currentTurn < NumTurns || playerHasWon == true);
        }

        /// <summary>
        /// Reset does the following
        /// 1) Re-initializes the main game variables (NumTurns/SecretCode)
        /// 2) Clears the Console of all existing text
        /// 3) Restarts the game loop
        /// </summary>
        /// <param name="numTurns">The number of turns the game should be reset with. Default value is 10 turns</param>
        public void Reset (int numTurns = 10)
        {
            NumTurns = numTurns;
            SecretCode = GenerateSecretCode();

            Console.Clear();
            Play();
        }

#region PRIVATE METHODS
        /// <summary>
        /// Generate a secret code
        /// </summary>
        /// <returns>An integer array containing a 4-digit secret code with each digit between values 1 and 6</returns>
        private int[] GenerateSecretCode()
        {
            Random rnd = new Random();
            var code = new int[4];
            for (int i = 0; i < 4; i++)
            {
                code[i] = rnd.Next(1, 7);
            }
            return code;
        }

        /// <summary>
        /// A helper method to display our integer array
        /// </summary>
        private void DisplaySecretCode()
        {
            foreach (int i in SecretCode)
            {
                Console.Write(i);
            }
        }

        /// <summary>
        /// A helper method to consolidate game introduction text
        /// </summary>
        private void CreateIntroductionText()
        {
            Console.WriteLine("Welcome to Mastermind!");
            Console.WriteLine("The secret code has been generated.");
            Console.WriteLine($"You have {NumTurns} to solve the code. Good luck!");
            Console.WriteLine();
        }

        /// <summary>
        /// A helper method to handle everything surrounding user input
        /// This includes:
        ///    1) User text
        ///    2) Handling user input from the Console
        ///    3) Passing the input off for validation
        ///    4) Re-prompting for input if the value failed validation, or
        ///    5) Passing the guess off for win/loss checking and Hint generation (if applicable)
        /// </summary>
        /// <returns>true/false for a win or lose condition</returns>
        private bool GetUserInputWithValidation()
        {
            Console.Write("Your guess: ");
            Console.ForegroundColor = ConsoleColor.White;
            string? userGuess = Console.ReadLine();

            Console.ForegroundColor = DefaultFontColor;
            int[]? userGuessInt;
            bool isValid = ValidateUserInput(userGuess, out userGuessInt);
            if (isValid)
            {
                if (userGuessInt != null)
                { 
                    //return CheckGuessAndDisplayHint(userGuessInt);
                    return CheckGuessAndDisplayHintAlternate(userGuessInt); // See Disclaimer in CheckGuessAndDisplayHint method body
                }
                else
                {
                    // We shouldn't ever fall into here (as userGuessInt is only set to null on failed validation, which should fall into the other logic block),
                    // but it never hurts to be extra careful
                    Console.WriteLine("The input value was not valid. Please enter a new value.");
                    GetUserInputWithValidation();
                }
            }
            else
            {
                Console.WriteLine("The input value was not valid. Please enter a new value.");
                GetUserInputWithValidation();
            }
            return false;
        }

        /// <summary>
        /// Check the user's guess against the secret code and perform the logic to generate any applicable hint
        /// </summary>
        /// <param name="userGuess">The user's guess as an integer array</param>
        /// <returns>true if the guess matches the secret code or 
        /// false if not</returns>
        private bool CheckGuessAndDisplayHint(int[] userGuess)
        {
            // Check for an exact match
            if (userGuess.SequenceEqual(SecretCode))
            {
                return true;
            }

            var correctDigitCorrectPosition = 0;
            var correctDigitIncorrectPosition = 0;

            // We're going to create a temporary shallow copy because we'll be modifying values to assist with our Phase 2 checks
            var tempGuess = (int[]) userGuess.Clone();

            // Phase 1: Look for correct digits in the correct position
            for (int i = 0; i < 4; i++)
            {
                if (tempGuess[i] == SecretCode[i])
                {
                    correctDigitCorrectPosition++;
                    tempGuess[i] = 0; // 0 is a safe value to use, because our code can't use this value
                }
            }

            // Phase 2: Look for correct digits in the incorrect position
            for (int i = 0; i < 4; i++)
            {
                if (tempGuess[i] == 0)
                {
                    continue;
                }

                for (int j = 0; j < 4; j++)
                {
                    if (tempGuess[i] == SecretCode[j])
                    {
                        correctDigitIncorrectPosition++;
                        tempGuess[i] = 0;
                        break;
                    }
                }
            }

            // Disclaimer: I believe that the logic above provides a hint as per the requirements provided (Secret Code: 1234, Guess: 4233, Hint: ++--)
            // I left the code as is here, as I was unsure if this was intentional or not.
            // Under normal circumstances I would clarify the acceptance criteria and perhaps get additional data points
            // to compare against, but in this situation that isn't available (that I'm aware of).
            // This becomes more apparent in a scenario like this: (Secret Code: 1234, Guess: 1111. The hint becomes +---)
            // I provided a "fixed" version in the method CheckGuessAndDisplayHintAlternate(). That is the method that is currently being used in the game when it runs

            // And again, this is just an observation I made while working through the exercise and I might be completely overthinking this!
            // I hope I didn't ruin any potential chances of moving forward in the interview process by making this decision, but I hope my reasoning makes sense.
            // I'd love to talk it over with the hiring manager(s) as well, if necessary.

            Console.Write($"Hint: ");
            Console.Write(new string('+', correctDigitCorrectPosition));
            Console.Write(new string('-', correctDigitIncorrectPosition));
            Console.WriteLine();
            return false;
        }

        /// <summary>
        /// Check the user's guess against the secret code and perform the logic to generate any applicable hint
        /// </summary>
        /// <param name="userGuess">The user's guess as an integer array</param>
        /// <returns>true if the guess matches the secret code or 
        /// false if not</returns>
        private bool CheckGuessAndDisplayHintAlternate(int[] userGuess)
        {
            // This version also tracks used digits in the Secret Code
            // By tracking the used digits in the Secret Code as well, we don't re-use digits that have already been accounted for in either number. This now gives us the following:
            // Secret Code: 1234
            // Guess: 4233
            // Hint: ++-

            // With no 4th hint element, as the user didn't guess a 1 in any position.

            // Check for an exact match
            if (userGuess.SequenceEqual(SecretCode))
            {
                return true;
            }

            var correctDigitCorrectPosition = 0;
            var correctDigitIncorrectPosition = 0;

            // We're going to create a temporary shallow copy because we'll be modifying values to assist with our Phase 2 checks
            var tempGuess = (int[]) userGuess.Clone();
            int[] tempSecret = (int[]) SecretCode.Clone();
            // Phase 1: Look for correct digits in the correct position
            for (int i = 0; i < 4; i++)
            {
                if (tempGuess[i] == tempSecret[i])
                {
                    correctDigitCorrectPosition++;
                    tempGuess[i] = 0; // 0 is a safe value to use, because our code can't use this value
                    tempSecret[i] = 0;
                }
            }

            // Phase 2: Look for correct digits in the incorrect position
            for (int i = 0; i < 4; i++)
            {
                if (tempGuess[i] == 0)
                {
                    continue;
                }

                for (int j = 0; j < 4; j++)
                {
                    if (tempSecret[j] == 0)
                    {
                        continue;
                    }

                    if (tempGuess[i] == tempSecret[j])
                    {
                        correctDigitIncorrectPosition++;
                        tempGuess[i] = 0;
                        tempSecret[j] = 0;
                        break;
                    }
                }
            }

            Console.Write($"Hint: ");
            Console.Write(new string('+', correctDigitCorrectPosition));
            Console.Write(new string('-', correctDigitIncorrectPosition));
            Console.WriteLine();
            return false;
        }

        /// <summary>
        /// Test the given input against a series of validation checks
        /// </summary>
        /// <param name="userGuess">The user's raw input from the Console</param>
        /// <param name="userGuessInt">A valid 4-digit secret code in integer array format. Array will be null on failed validation.</param>
        /// <returns>true/false depending on successful validation</returns>
        private bool ValidateUserInput(string? userGuess, out int[]? userGuessInt)
        {
            // Null/empty check
            if (userGuess == null || userGuess.Length == 0)
            {
                userGuessInt = null;
                return false;
            }

            // Correct number of digits
            if (userGuess.Length != 4)
            {
                userGuessInt = null;
                return false;
            }

            // Check for certain special characters
            if (userGuess.Contains('-') ||
                userGuess.Contains('+'))
            {
                userGuessInt = null;
                return false;
            }

            int tempGuess;
            // Convert the string to an integer
            // Failure should cover all remaining non-numeric values
            if (Int32.TryParse(userGuess, out tempGuess))
            {
                // This will work because we know each digit can only be 1 through 6
                // This uses character code math to get the value that we want
                userGuessInt = userGuess.Select(c => c - '0').ToArray();
                return true;
            }
            else
            {
                // falling into this block should cover remaining letters/special characters for fail cases
                userGuessInt = null;
                return false;
            }
        }

        /// <summary>
        /// A helper method to display the win text to the player.
        /// Additionally allow the user to press Enter/Return to reset the game and continue playing
        /// </summary>
        /// <param name="turnNum">The winning turn number</param>
        private void DisplayWinTextAndReset(int turnNum)
        {
            Console.WriteLine();
            Console.WriteLine($"Congratulations! You have guessed the code in {turnNum} " + $"{(turnNum == 1 ? "turn" : "turns")}!");
            Console.WriteLine();
            Console.WriteLine("Press Enter/Return to restart the game!");
            Console.ReadLine();
            // We could catch the input and allow the user to end the game by entering a certain button/word/phrase
            // For now this will always just Reset the game and continue until the Console is closed.
            Reset(NumTurns);
        }

        /// <summary>
        /// A helper method to display the loss text to the player
        /// Additionally allow the user to press Enter/Return to reset the game and continue playing
        /// </summary>
        private void DisplayLossTextAndReset()
        {
            Console.WriteLine();
            Console.WriteLine($"You have failed to guess the code in {NumTurns} " + $"{(NumTurns == 1 ? "turn" : "turns")}.");
            Console.Write("The correct code was ");
            DisplaySecretCode();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Press Enter/Return to restart the game and try again!");
            Console.ReadLine();
            // We could catch the input and allow the user to end the game by entering a certain button/word/phrase
            // For now this will always just Reset the game and continue until the Console is closed.
            Reset(NumTurns);
        }
        #endregion PRIVATE METHODS

    }
}
