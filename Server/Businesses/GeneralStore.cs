using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UE_Shared;

namespace UE_Server.Businesses
{
    public class GeneralStore : Business
    {
        public GeneralStore(string businnessName, Location location, uint blipSprite, int inventoryMax, uint pedhash = 0, string owner = null, bool buyable = true, bool onsale = true) : base(businnessName, location, blipSprite, inventoryMax, pedhash, owner, buyable, onsale)
        {
        }
    }
}
