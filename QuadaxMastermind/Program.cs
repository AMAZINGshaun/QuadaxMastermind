namespace QuadaxMastermind
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // This could be extended to allow for more than 10 turns using the overloaded constructor
            Mastermind theGame = new Mastermind();
            theGame.Play();
        }
    }
}
