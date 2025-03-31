using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Questao2
{
    public class FootballMatchesResult
    {
        public int Page { get; set; }
        public int Per_page { get; set; }
        public int Total { get; set; }
        public int Total_pages { get; set; }
        public List<FootballMatch>? Data { get; set; }
    }

    public class FootballMatch
    {
        public string Competition { get; set; }
        public int Year { get; set; }
        public string Round { get; set; }
        public string Team1 { get; set; }
        public string Team2 { get; set; }
        public int Team1goals { get; set; }
        public int Team2goals { get; set; }
    }
}
