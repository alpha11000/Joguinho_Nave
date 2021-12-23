using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Jogo_Navinha
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[,] screen = getStringMatrix(8, 30, "");

            Console.WriteLine("Insira um número para definir a probabilidade de spawn de inimigos(1 - 40):");
            int prob = 44 - Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Defina o valor para o delay em que os inimigos se aproximam(em ms):");
            int delayTime = Convert.ToInt32(Console.ReadLine());


            for (int i = 0; i < 1000; i++)
            {
                downScreen(screen, 15);
                spawnEnemys(screen, "{#}", prob);
                Console.Clear();
                showScreen(screen, true);
                
                Thread.Sleep(delayTime);
            }

        }

        public static void spawnEnemys(string [,] screen, string enemyStyle, int prob)
        {
            Random rand = new Random();

            for(int j = 0; j < screen.GetLength(1); j++)
            {
                screen[0, j] = ((rand.Next() % prob) == 0) ? enemyStyle : "";
            }

        }

        public static void downScreen(string [,] screen, int exceptLine)
        {
            string[] lastLine = new string[screen.GetLength(0)]; ;

            for (int i = 1; i < screen.GetLength(0); i++)
            {
                for(int j = 0; j < screen.GetLength(1); j++)
                {
                    string tempLastLine = lastLine[j];
                    lastLine[j] = screen[i, j];
                    screen[i, j] = (i == 1) ? screen[i - 1, j] : tempLastLine;
                }
            }


        }

        public static void showScreen(String[,] screen, bool tabTexts){
            for (int i = 0; i < screen.GetLength(0); i++)
            {
                for(int j = 0; j < screen.GetLength(1); j++)
                {
                    Console.Write(screen[i, j]);
                    Console.Write(tabTexts ? "\t" : "");
                }
                Console.WriteLine();
            }
        }

        public static string[,] getStringMatrix(int width, int height, string initialValue)
        {
            string[,] textMatrix = new string[height, width];

            for (int i = 0; i < textMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < textMatrix.GetLength(1); j++)
                {
                    textMatrix[i, j] = initialValue;
                }
            }

            return textMatrix;
        }
    }
}
