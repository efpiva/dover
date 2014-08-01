/*
 *  Dover Framework - OpenSource Development framework for SAP Business One
 *  Copyright (C) 2014  Eduardo Piva
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *  
 *  Contact me at <efpiva@gmail.com>
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dover.Framework.Factory;
using System.IO;

namespace Dover.Framework.DAO
{
    public static class DBResourceExtension
    {
        private static string dbType = null;

        public static string GetSQL(this Object o, string resource)
        {
            string ns = o.GetType().Namespace;
            if (dbType == null)
                dbType = (SAPServiceFactory.CompanyFactory().DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB) ? "hana" : "sql";

            using (var stream = o.GetType().Assembly.GetManifestResourceStream(ns + "." + dbType + "." + resource))
            {
                if (stream != null)
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            return string.Empty;
        }

    }
}
