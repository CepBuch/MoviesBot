using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot.Data.TelegramBotData.Enums
{
    public enum QueryType
    {
        SearchingMovie,
        SelectingMovie,
        WaitingTrailer,
        ChoosingRandomMovie,
        SearchingPerson,
        ChosingGenre
    }
}
