using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FiftyOne
{
    class MainClass
    {

        public static Random random = new Random();
        public int MAX_CYCLE = 1000;

        public static void Main(string[] args)
        {
                for (int cycle = 0; cycle < 1; cycle++)
                {
                    Environment env = new Environment();
                    env.Init();

                    env.Call();
                    /*int episode = 0;
                    int action = 0;
                    do {
                        action = SelectAction();
                        episode += 1;
                    } while ( action != 6);
                    */
                }
        }

        /// <summary>
        /// 0(LL),1(LR),2(RL),3(RR) = ChangeCard
        /// 4 = clearTable
        /// 5 = Pass
        /// 6 = Call 
        /// </summary>
        private static int SelectAction(){
            int rnd = random.Next(0,7);
            return rnd;
        }
    }
}