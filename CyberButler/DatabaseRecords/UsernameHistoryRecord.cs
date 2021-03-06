﻿using System;
using System.Collections.Generic;

namespace CyberButler.DatabaseRecords
{
    internal class UsernameHistoryRecord : BaseRecord
    {
        public string Server { get; set; }
        public string UserID { get; set; }
        public string NameBefore { get; set; }
        public string NameAfter { get; set; }
        public string InsertDateTime { get; set; }

        public override void Insert()
        {
            var statement = @"INSERT INTO username_history
                                          (server,
                                           userid,
                                           name_before,
                                           name_after,
                                           insert_datetime)
                              VALUES      (@server,
                                           @userid,
                                           @namebefore,
                                           @nameafter,
                                           @datetime)";

            var parameters = new Dictionary<String, String>
            {
                { "@server", Server },
                { "@userid", UserID },
                { "@namebefore", NameBefore },
                { "@nameafter", NameAfter },
                { "@datetime", InsertDateTime }
            };

            db.NonQuery(statement, parameters);
        }

        public IEnumerable<UsernameHistoryRecord> Select(String _server, String _userid)
        {
            var query = @"SELECT name_before,
                                 name_after
                          FROM   username_history
                          WHERE  server = @server
                                 AND userid = @userid
                          ORDER  BY insert_datetime DESC  ";

            var parameters = new Dictionary<String, String>
            {
                { "@server", _server },
                { "@userid", _userid }
            };

            var dt = db.Select<UsernameHistoryRecord>(query, parameters);

            var result = new Dictionary<String, String>();

            var records = db.Select<UsernameHistoryRecord>(query, parameters);

            return records;
        }
    }
}