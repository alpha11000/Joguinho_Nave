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
            Console.WriteLine("Insira um número para definir a probabilidade de spawn de inimigos(1 - 40):");
            int prob = 44 - Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Defina o valor para o delay em que os inimigos se aproximam(em ms):");
            int delayTime = Convert.ToInt32(Console.ReadLine());

            string[,] screen = getScreenMatrix(8, 15, "", " A");
            int playerPosition = 0;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            long currentTime;
            int totalLifes = 10;

            //controladores de tempo
            long enemyScreenUpdateTime = (long)delayTime;
            long lastEnemyScreenUpdate = 0;

            long playerScreenUpdateTime = 100;
            long lastPlayerScreenUpdate = 0;

            long bulletsScreenUpdateTime = 200;
            long lastBulletScreenUpdate = 0;

            int restLifes = totalLifes;

            watch.Start();

            while(true)
            {
                currentTime = watch.ElapsedMilliseconds;

                if(currentTime - lastPlayerScreenUpdate >= playerScreenUpdateTime)
                {
                    if (Console.KeyAvailable)
                    {
                        var pressedKey = Console.ReadKey(true);

                        switch (pressedKey.Key)
                        {
                            case ConsoleKey.LeftArrow:
                                playerPosition = movePlayer(true, screen, playerPosition);
                                break;

                            case ConsoleKey.RightArrow:
                                playerPosition = movePlayer(false, screen, playerPosition);
                                break;

                            case ConsoleKey.Spacebar:
                                shoot(screen, playerPosition);
                                Console.Clear();
                                showScreen(screen, restLifes, true);
                                break;
                        
                        }
                        showLastLine(screen, true);
                        //showScreen(screen, restLifes, true);

                    }
                    lastPlayerScreenUpdate = currentTime;
                }

                if(currentTime - lastBulletScreenUpdate >= bulletsScreenUpdateTime)
                {
                    if (moveBulletsUp(screen))
                    {
                        moveBulletsUp(screen);
                        Console.Clear();
                        showScreen(screen, restLifes, true);
                    }
                    
                }

                if(currentTime - lastEnemyScreenUpdate >= enemyScreenUpdateTime)
                {

                    restLifes -= downScreen(screen, 15);
                    spawnEnemys(screen, "{#}", prob);
                    Console.Clear();
                    showScreen(screen, restLifes, true);

                    lastEnemyScreenUpdate = currentTime;
              
                }

            }

        }

        public static int movePlayer(bool left, string[,] screen, int atualPosition)
        {

            if((atualPosition >= screen.GetLength(1) - 1 && !left)|| atualPosition <= 0 && left)
            {
                return atualPosition;
            }

            int direction = (left) ? -1 : 1;

            int screenLastLine = screen.GetLength(0) - 1;

            screen[screenLastLine , atualPosition + direction] = screen[screenLastLine, atualPosition];
            screen[screenLastLine , atualPosition] = "";

            return atualPosition + direction;
        }

        public static bool shoot(string [ , ] screen, int playerPosition)
        {

            int shootInitialHeight = screen.GetLength(0) - 2;
            string bullet = " |";
            bool hit = false;

            if(screen[shootInitialHeight , playerPosition] == "{#}")
            {
                hit = true;
                bullet = "###";
            }

            screen[shootInitialHeight, playerPosition] = bullet;

            return hit;
        }

        public static bool moveBulletsUp(string[,] screen)
        {
            bool hasBullet = false;

            for(int i = 1; i < screen.GetLength(0) - 1; i++)
            {
                for(int j = 0; j < screen.GetLength(1); j++)
                {
                    if(screen[i, j] == " |")
                    {

                        hasBullet = true;

                        if(screen[i - 1, j] == "{#}")
                        {
                            screen[i - 1, j] = "###";
                            screen[i, j] = "";
                        }
                        else
                        {
                            screen[i - 1, j] = screen[i, j];
                            screen[i, j] = "";
                        }
                    }
                }
            }
            return hasBullet;
        }

        public static void showLastLine(string[,] screen, bool tabText)
        {
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - 1);

            for(int i = 0; i < screen.GetLength(1); i++)
            {
                Console.Write(screen[screen.GetLength(0) - 1, i]);
                Console.Write((tabText) ? "\t" : "");
            }

            Console.WriteLine();
        }

        public static void spawnEnemys(string [,] screen, string enemyStyle, int prob)
        {
            Random rand = new Random();

            for(int j = 0; j < screen.GetLength(1); j++)
            {
                screen[0, j] = ((rand.Next() % prob) == 0) ? enemyStyle : "";
            }

        }

        public static int downScreen(string [,] screen, int exceptLine) // retorna a quantidade de erros do player // -1 para fim de jogo
        {
            string[] lastLine = new string[screen.GetLength(0)];
            int erros = 0;

            for (int i = 1; i < screen.GetLength(0); i++)
            {
                for(int j = 0; j < screen.GetLength(1); j++)
                {
                    if(screen[i,j] == " |" || screen[i,j] == "###")
                    {
                        lastLine[j] = "";
                        continue;
                    }

                

                    if(screen[i, j] == " A") { 
                        if(screen[i - 1, j] == "{#}")
                        {
                            erros = 1000000000;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    string tempLastLine = lastLine[j];
                    lastLine[j] = screen[i, j];
                    screen[i, j] = (i == 1) ? screen[i - 1, j] : tempLastLine;

                    if(i == screen.GetLength(0) - 1 && screen[i , j].CompareTo("{#}") == 0)
                    {
                        erros++;
                        screen[i, j] = "###";
                    }
                }
            }

            return erros;
        }

        public static void showScreen(String[,] screen, int restLifes, bool tabTexts){

            Console.WriteLine("Vidas restantes:" + restLifes + "\n");

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

        public static string[,] getScreenMatrix(int width, int height, string initialValue, string playerStyle)
        {
            string[,] textMatrix = new string[height, width];

            for (int i = 0; i < textMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < textMatrix.GetLength(1); j++)
                {
                    textMatrix[i, j] = initialValue;
                }
            }

            textMatrix[textMatrix.GetLength(0) - 1, 0] = playerStyle;

            return textMatrix;
        }
    }
}
