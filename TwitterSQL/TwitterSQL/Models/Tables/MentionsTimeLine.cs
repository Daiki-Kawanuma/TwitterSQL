﻿using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;

namespace TwitterSQL.Models.Tables
{
    public class MentionsTimeLine : ITable
    {
        public string TableName => "MentionsTimeLine(Count: 20)";
        public IList<string> Columns => new List<string>
        {
            "Tweet"
        };

        public IDictionary<string, string> Parameters { get; set; }
        public string SelectPhrase { get; set; }
        public string WherePhrase { get; set; }
        public string GroupByPhrase { get; set; }
        public string HavingPhrase { get; set; }
        public string OrderByPhrase { get; set; }

        public MentionsTimeLine()
        {
            Parameters = new Dictionary<string, string>();
        }

        public async Task<T> GetResult<T>()
        {
            var list = await GetRawResult();

            if (!string.IsNullOrEmpty(WherePhrase))
                list = list.AsQueryable().Where(WherePhrase).ToList();

            if (!string.IsNullOrEmpty(OrderByPhrase))
                list = list.AsQueryable().OrderBy(OrderByPhrase).ToList();

            if (!string.IsNullOrEmpty(SelectPhrase) && !SelectPhrase.Equals("Tweet"))
            {
                return (T)list.AsQueryable().Select($"new({SelectPhrase})");
            }
            else
            {
                return (T)list;
            }
        }

        private async Task<IList<CoreTweet.Status>> GetRawResult()
        {
            var count = int.Parse(Parameters["Count"]) > 800 ? 800 : int.Parse(Parameters["Count"]);

            var tokens = await TokenGenerator.GenerateAccessTokens();
            var result = await tokens.Statuses.MentionsTimelineAsync(count: count > 200 ? 200 : count);

            var returnList = new List<CoreTweet.Status>();
            returnList.AddRange(result.ToList());

            while (returnList.Count < count && returnList.Last().Id != 0)
            {
                var requestCount = (count - returnList.Count) % 201;
                result = await tokens.Statuses.MentionsTimelineAsync(count: requestCount, max_id: returnList.Last().Id);
                returnList.AddRange(result.ToList());
            }

            return returnList;
        }
    }
}
