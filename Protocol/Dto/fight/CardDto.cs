using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Dto.fight
{
    /// <summary>
    /// 表示卡牌
    /// </summary>
    [Serializable]
 public   class CardDto
    {
        public string Name;
        public int Color;
        public int Weight;

        public CardDto()
        {

        }

        public CardDto(string name,int color,int weight)
        {
            this.Name = name;
            this.Color = color;
            this.Weight = weight;
        }



    }
}
