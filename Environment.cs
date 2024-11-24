using System;
using System.ComponentModel;


namespace FiftyOne
{
    /// <summary>
    /// カードの種類の列挙型
    /// </summary>
    public enum Cards
    {
        dA, d2, d3, d4, d5,
        hA, h2, h3, h4, h5,
        J
    }

    /// <summary>
    /// Deque
    /// </summary>
    public class Deque<T>
    {
        private LinkedList<T> list = new LinkedList<T>();

        // 前から追加
        public void AddFirst(T item)
        {
            list.AddFirst(item);
        }

        // 後ろから追加
        public void AddLast(T item)
        {
            list.AddLast(item);
        }

        // 前から削除
        public T RemoveFirst()
        {
            if (list.Count == 0)
                throw new InvalidOperationException("Deque is empty.");
            var value = list.First.Value;
            list.RemoveFirst();
            return value;
        }

        // 後ろから削除
        public T RemoveLast()
        {
            if (list.Count == 0)
                throw new InvalidOperationException("Deque is empty.");
            var value = list.Last.Value;
            list.RemoveLast();
            return value;
        }

        // 前の要素を見る
        public T PeekFirst()
        {
            if (list.Count == 0)
                throw new InvalidOperationException("Deque is empty.");
            return list.First.Value;
        }

        // 後ろの要素を見る
        public T PeekLast()
        {
            if (list.Count == 0)
                throw new InvalidOperationException("Deque is empty.");
            return list.Last.Value;
        }

        // 要素数を取得
        public int Count => list.Count;
    }

    public class Environment
    {
        //最大所持カード枚数
        public static int MAX_CARD = 2;
        //同じマークのカードが何枚あるか
        public static int MAX_NUMBER_OF_CARD = 5;

        Random random = new Random();

        //ゲームで使用するカードのセットを入れておく
        private List<Cards> allCards = new List<Cards> {
            Cards.dA, Cards.d2, Cards.d3, Cards.d4, Cards.d5,
            Cards.hA, Cards.h2, Cards.h3, Cards.h4, Cards.h5,
            Cards.J
        };

        private Deque<Cards> stock = new Deque<Cards>();
        private Cards[] ownCard = new Cards[2];
        private Cards[] tableCard = new Cards[2];
        private Cards[] cpuCard = new Cards[2];

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            //山札
            stock = ShuffledStock();

            //配る順番(場→CPU→自分)
            for (int i = 0; i < MAX_CARD; i++)
            {
                tableCard[i] = stock.RemoveFirst();
            }
            for (int i = 0; i < MAX_CARD; i++)
            {
                cpuCard[i] = stock.RemoveFirst();
            }
            for (int i = 0; i < MAX_CARD; i++)
            {
                ownCard[i] = stock.RemoveFirst();
            }
        }

        /// <summary>
        /// Action：カード交換、左から0,1,2で数える
        /// </summary>
        public int ChangeCard(int playerCardNum, int tableCardNum, bool isCpu)
        {
            if (isCpu)
            {
                Cards tmp = cpuCard[playerCardNum];
                cpuCard[playerCardNum] = tableCard[tableCardNum];
                tableCard[tableCardNum] = tmp;
            }
            else
            {
                Cards tmp = ownCard[playerCardNum];
                ownCard[playerCardNum] = tableCard[tableCardNum];
                tableCard[tableCardNum] = tmp;
            }
            return 0;
        }

        /// <summary>
        /// Action: 流す
        /// </summary> <summary>
        public int ClearTableCards()
        {
            for (int i = 0; i < MAX_CARD; i++)
            {
                Cards tmp = tableCard[i];
                tableCard[i] = stock.RemoveFirst();
                stock.AddLast(tmp);
            }
            return 0;
        }

        /// <summary>
        /// Action: パス
        /// </summary>
        public int Pass()
        {
            return 0;
        }

        /// <summary>
        /// コール
        /// </summary>
        public int Call(bool isCallFromCpu)
        {
            // 得点計算から勝敗判定
            int cpuScore = CalculateScore(true);
            int ownScore = CalculateScore(false);

            ShowResult();
            Console.WriteLine("------------------");
            Console.WriteLine(String.Format("相手:{0}点　自分:{1}点", cpuScore, ownScore));
            Console.WriteLine(isWin() ? "相手の勝ち" : "自分の勝ち");

            if (isCallFromCpu)
            {
                //CpuがコールしてPlayerが勝利(?): 敗北(?)
                //return !isWin() ? 10 : -10;
            }
            else
            {
                //Playerがコールして勝利(10): 敗北(-10)
                return isWin() ? 10 : -10;
            }

            return 0;
        }


        /// <summary>
        /// Playerが勝ったか否か
        /// </summary>
        public bool isWin()
        {
            return CalculateScore(isCpu: false) >= CalculateScore(isCpu: true);
        }

        /// <summary>
        /// 報酬
        /// </summary>
        /// <param name="isCpu">Cpuならtrue、Playerならfalse</param>
        public int getReward(bool isCpu)
        {
            return isCpu ? CalculateScore(isCpu: true) : CalculateScore(isCpu: false);
        }

        /// <summary>
        /// 場札の取得
        /// </summary>
        public Cards[] getTableCards()
        {
            return tableCard;
        }

        /// <summary>
        /// 手札の取得
        /// </summary>
        public Cards[] getOwnCards()
        {
            return ownCard;
        }

        /// <summary>
        /// 得点計算
        /// </summary>
        private int CalculateScore(bool isCpu)
        {
            int score = 0;
            Cards firstCard = isCpu ? cpuCard[0] : ownCard[0];

            for (int i = 0; i < MAX_CARD; i++)
            {
                if (isSameMark(firstCard, isCpu ? cpuCard[i] : ownCard[i]))
                {
                    score += cardScore(isCpu ? cpuCard[i] : ownCard[i]);
                }
                else
                {
                    //同じマークでないものが一つでもあれば0を返す
                    score = 0;
                    break;
                }
            }
            return Math.Min(score, 10);
        }

        /// <summary>
        /// 単一カードの得点
        /// </summary>
        private int cardScore(Cards card)
        {
            int cardNum = (int)card;
            if (card == Cards.J)
            {
                return 6; //とりあえず6返しとく（Joker = 5,6）
            }
            else if (card == Cards.dA || card == Cards.hA)
            {
                return 5; //A = 5
            }
            else
            {
                return cardNum % MAX_NUMBER_OF_CARD + 1;
            }
        }

        /// <summary>
        /// 同じマークのカードかどうか判定
        /// </summary>
        private bool isSameMark(Cards card1, Cards card2)
        {
            if (card1 == Cards.J || card2 == Cards.J)
            {
                return true;
            }
            return (int)card1 / MAX_NUMBER_OF_CARD == (int)card2 / MAX_NUMBER_OF_CARD;
        }

        /// <summary>
        /// すべてのカードをシャッフルする
        /// </summary>
        private Deque<Cards> ShuffledStock()
        {
            List<Cards> allCardsCopy = new List<Cards>(allCards);
            Deque<Cards> shuffled = new Deque<Cards>();
            while (allCardsCopy.Count > 0)
            {
                int num = random.Next(0, allCardsCopy.Count);
                shuffled.AddFirst(allCardsCopy[num]);
                allCardsCopy.RemoveAt(num);
            }
            return shuffled;
        }

        private void ShowResult()
        {
            Console.WriteLine($"相手: {cpuCard[0]} {cpuCard[1]}");
            Console.WriteLine($"場札: {tableCard[0]} {tableCard[1]}");
            Console.WriteLine($"自分: {ownCard[0]} {ownCard[1]}");
            Console.WriteLine($"点数: {CalculateScore(false)}");
        }

    }
}