﻿using System;
using System.Data;

namespace CyberButler.DatabaseRecords
{
    class RestaurantRecord : BaseRecord
    {
        public string Server { get; set; }
        public string Restaurant { get; set; }

        public override void Insert()
        {
            db.Insert($"insert into restaurant (server, restaurant) values ('{Server}', '{Restaurant}')");
        }

        public String SelectRandom()
        {
            DataTable dt = db.Select("select restaurant from restaurant order by random() limit 1");
            return dt.Rows[0]["restaurant"].ToString();
        }
    }
}