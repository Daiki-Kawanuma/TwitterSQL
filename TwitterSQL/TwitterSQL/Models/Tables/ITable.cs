﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterSQL.Models.Tables
{
    public interface ITable
    {
        string TableName { get; }

        IList<string> Columns { get; }

        IDictionary<string, string> Paramerters { get; set; }

        string SelectPhrase { get; set; }

        string WherePhrase { get; set; }

        string GroupByPhrase { get; set; }

        string HavingPhrase { get; set; }

        string OrderByPhrase { get; set; }

        Task<T> GetResult<T>();
    }
}