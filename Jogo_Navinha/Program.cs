using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Joguinho_Nave
{
    internal class Program
    {

        static void Main(string[] args)
        { 

            Console.WriteLine("Insira um número para definir a probabilidade de spawn de inimigos(1 - 40):");
            int prob = 44 - Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Defina o valor para o delay em que os inimigos se aproximam(em ms):");
            int delayTime = Convert.ToInt32(Console.ReadLine());

            Console.CursorVisible = false;

            string[,] screen = getScreenMatrix(20, 8, "", " A");
            int playerPosition = 0;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            long currentTime;
            int totalLifes = 10;

            //controladores de tempo
            long enemyScreenUpdateTime = (long)delayTime;
            long lastEnemyScreenUpdate = 0;

            long playerScreenUpdateTime = 100;
            long lastPlayerScreenUpdate = 0;

            long bulletsScreenUpdateTime = 50;
            long lastBulletScreenUpdate = 0;

            long shotMinimumTime = bulletsScreenUpdateTime;
            long lastShot = 0;

            int restLifes = totalLifes;

            watch.Start();
            Console.Clear();
            showScreen(screen, restLifes, true);

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
                                break;
                        }
                        updateSpecificConsolePart(screen, screen.GetLength(0)+1, playerPosition, true);

                    }
                    lastPlayerScreenUpdate = currentTime;
                }

                if(currentTime - lastBulletScreenUpdate >= bulletsScreenUpdateTime)
                {
                    BulletsUp(screen);
                    lastBulletScreenUpdate = currentTime;
                }

                if(currentTime - lastEnemyScreenUpdate >= enemyScreenUpdateTime)
                {
                    downScreen(screen, 0);
                    spawnEnemys(screen, "{#}", prob);
                    lastEnemyScreenUpdate = currentTime;
                }

            }

        }

        public static void updateSpecificConsolePart(string[,] screen, int lineToUpdate, int lineStart, bool tabText)
        {
            int localLineStart = (lineStart == 0)? 0 : lineStart - 1;

            Console.SetCursorPosition(localLineStart * ((tabText) ? 8 : 1), lineToUpdate); //o 8 representa o espaço ocupado por uma tabulação
            
            for(int i = localLineStart; i < screen.GetLength(1); i++)
            {
                Console.Write(screen[lineToUpdate-2, i] + ((tabText) ? "\t" : ""));
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
            updateSpecificConsolePart(screen, shootInitialHeight + 2, playerPosition, true);

            return hit;
        }



        public static void BulletsUp(string[,] screen)
        {
            bool hasBulletOnLine;

            for (int i = 0; i < screen.GetLength(0); i++)
            {
                hasBulletOnLine = false;

                for (int j = 0; j < screen.GetLength(1); j++)
                {
                    if (screen[i,j] == " |")
                    {
                        hasBulletOnLine = true;

                        if(i == 0)
                        {
                            screen[i, j] = "";
                            continue;
                        }

                        if(screen[i - 1, j] == "{#}")
                        {
                            screen[i - 1, j] = "###";
                        }
                        else {
                            screen[i - 1, j] = screen[i, j];
                        }

                        screen[i, j] = "";
                    }
                }

                if (hasBulletOnLine)
                {
                    updateSpecificConsolePart(screen, i + 2, 0, true);
                    
                    if(i != 0) {
                        updateSpecificConsolePart(screen, i + 1, 0, true);
                    }
                }
            }
        }


        public static void showLastLine(string[,] screen, bool tabText, int atualPosition)
        {
            Console.SetCursorPosition(atualPosition, Console.CursorTop - 1);

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
            //int minimumEnemyPosition = -1;

            for(int j = 0; j < screen.GetLength(1); j++)
            {
                /*if((rand.Next() % prob) == 0)
                {
                    if(minimumEnemyPosition < j) { minimumEnemyPosition = j; }
                    screen[0, j] = enemyStyle;
                }
                else
                {
                    screen[0, j] = "";
                }*/

                screen[0, j] = ((rand.Next() % prob) == 0) ? enemyStyle : "";
            }
            updateSpecificConsolePart(screen, 2, 0, true);
        }



        public static int downScreen(string [,] screen, int exceptLine) // retorna a quantidade de erros do player // -1 para fim de jogo
        {
            string[] lastLine = new string[screen.GetLength(0)];
            int erros = 0;
            string atualValue;

            for (int i = 1; i < screen.GetLength(0); i++)
            {
                for(int j = 0; j < screen.GetLength(1); j++)
                {
                    if (screen[i, j] == " |")
                    {
                        if (lastLine[j] == "{#}")
                        {
                            screen[i , j] = "###";
                        }

                        lastLine[j] = "";

                        continue;
                    }

                    if (screen[i,j] == "###")
                    {
                        screen[i, j] = "";
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
                updateSpecificConsolePart(screen, i+2, 0, true);
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

        public static string[,] getScreenMatrix(int height, int width, string initialValue, string playerStyle)
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
