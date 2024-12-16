# QuadaxMastermind
A coding exercise to create a variation of the logic game Mastermind

The prompt:

Create a C# console application that is a simple version of Mastermind. The randomly generated answer
should be four (4) digits in length, with each digit between the numbers 1 and 6. After the player enters
a combination, a minus (-) sign should be printed for every digit that is correct but in the wrong position,
and a plus (+) sign should be printed for every digit that is both correct and in the correct position. Print
all plus signs first, all minus signs second, and nothing for incorrect digits. The player has ten (10)
attempts to guess the number correctly before receiving a message that they have lost.

Example:

If the secret answer were: 1234

And the user guessed: 4233

The hint should be: ++--
