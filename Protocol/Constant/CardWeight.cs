using Protocol.Dto.fight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Constant
{
    /// <summary>
    /// 卡牌权值
    /// </summary>
 public   class CardWeight
    {
       /* CLUB 
        HEART 
        SPADE 
        SQUARE*/


        public const int THREE = 3;
        public const int FOUR = 4;
        public const int FIVE = 5;
        public const int SIX =6;
        public const int SEVEN = 7;
        public const int EIGHT = 8;
        public const int NINE =9;
        public const int TEN = 10;

        public const int JACK = 11;
        public const int QUEEN = 12;
        public const int KING = 13;

        public const int ONE = 14;
        public const int TWO = 15;

        public const int SJOKER = 16;
        public const int LJOKER = 17;

        /// <summary>
        /// 获取卡牌的权值
        /// </summary>
        /// <param name="cardList">选中的卡牌</param>
        /// <param name="cardType">出牌类型</param>
        /// <returns></returns>
        public static int GetWeight(List<CardDto> cardList,int cardType)
        {
            int totalWeight = 0;
            if(cardType==CardType.THREE_ONE||cardType==CardType.THREE_TWO)
            {
                //如果是三带一 或者是三带二
                //3335 4443
                for(int i=0;i<cardList.Count-2;i++)
                {
                    if(cardList[i].Weight==cardList[i+1].Weight&& cardList[i].Weight == cardList[i + 2].Weight)
                    {
                        totalWeight += (cardList[i].Weight * 3);
                    }
                }
            }
            else
            {
                for(int i=0;i<cardList.Count;i++)
                {
                    totalWeight += cardList[i].Weight;
                }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
            }
            return totalWeight;
        }

    }
}
