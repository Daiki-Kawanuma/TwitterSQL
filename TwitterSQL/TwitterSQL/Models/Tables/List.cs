﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using CoreTweet;

namespace TwitterSQL.Models.Tables
{
    public class List : ITable
    {
        public string TableName => "List(Slug: , OwnerUserName: , Count: 20)";
        public IList<string> Columns => new List<string>
        {
            "Tweet"
        };

        public IDictionary<string, string> Paramerters { get; set; }
        public string SelectPhrase { get; set; }
        public string WherePhrase { get; set; }
        public string GroupByPhrase { get; set; }
        public string HavingPhrase { get; set; }
        public string OrderByPhrase { get; set; }

        public List()
        {
            Paramerters = new Dictionary<string, string>();
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
            var slug = Paramerters["Slug"];
            var ownerUserName = Paramerters["OwnerUserName"];
            var count = int.Parse(Paramerters["Count"]);

            var tokens = await TokenGenerator.GenerateTokens();
            var result = await tokens.Lists.StatusesAsync(slug: slug, owner_screen_name: ownerUserName, count: count);

            return result.ToList();
        }
    }
}