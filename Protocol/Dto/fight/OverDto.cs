using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Dto.fight
{
    [Serializable]
   public class OverDto
    {
        public int WinIdentity;
        public List<int> WinUIdList;
        public List<int> LeaveUIdList;
        public List<int> tempUIdList;
        public int BeenCount;

        public OverDto()
        {
            BeenCount = 0;
        }
    }
}
