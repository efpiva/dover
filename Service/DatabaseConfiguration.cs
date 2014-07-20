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
using Dover.Framework.DAO;
using Dover.Framework.Model.SAP;
using System.Reflection;
using System.Xml.Serialization;
using SAPbobsCOM;

namespace Dover.Framework.Service
{
    public class DatabaseConfiguration
    {
        private BusinessOneDAO b1DAO;
        private const string DBTABLES_XML = "Dover.Framework.DatabaseTables.xml";
        private const string DBFIELDS_XML = "Dover.Framework.DatabaseFields.xml";

        public DatabaseConfiguration(BusinessOneDAO b1DAO)
        {
            this.b1DAO = b1DAO;
        }

        internal void PrepareDatabase()
        {
            UserTableBOM tables;
            UserFieldBOM fields;

            using (var tableStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(DBTABLES_XML))
            using (var fieldStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(DBFIELDS_XML))
            {
                tables = b1DAO.GetBOMFromXML<UserTableBOM>(tableStream);
                fields = b1DAO.GetBOMFromXML<UserFieldBOM>(fieldStream);
            }

            b1DAO.SaveBOMIfNotExists(tables);
            b1DAO.SaveBOMIfNotExists(fields);
        }
    }
}
