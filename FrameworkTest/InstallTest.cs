using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dover.Framework.DAO;
using Dover.Framework.Form;
using System;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using SAPbobsCOM;
using SAPbouiCOM;
using Dover.LicenseGenerator;
using Dover.Framework.Model.License;
using System.Collections.Generic;
using Dover.Framework.Service;

namespace FrameworkTest
{
    class UpdateDbValue
    {
        internal string Type { get; set; }
        internal string Value { get; set; }
    }

    [TestClass]
    public class InstallTests
    {
        private Dover.Framework.Application app;
        private Application b1App;
        private SAPbobsCOM.Company b1Company;

        [TestInitialize]
        public void Initialize()
        {
            app = DoverSetup.CleanDover(false);
            DoverSetup.bootDover(app);
            b1Company = app.Resolve<SAPbobsCOM.Company>();
            b1App = app.Resolve<SAPbouiCOM.Application>();
            CleanAddins();
        }

        private void CleanAddins()
        {
            string[] udos = {"TestUDO"};
            string[] tables = {"TEST", "TEST_LINES", "TEST_LINES2"};

            removeUDOs(udos);
            removeTables(tables);
            // removeFields(userFields);
        }

        private void removeTables(string[] tables)
        {
            SAPbouiCOM.Application b1App = app.Resolve<SAPbouiCOM.Application>();
            UserTablesMD userTableMD = (UserTablesMD)b1Company.GetBusinessObject(BoObjectTypes.oUserTables);
            foreach (var table in tables)
            {
                DoverSetup.removeTable(userTableMD, table, b1App, b1Company);
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(userTableMD);
        }

        private void removeFields(dynamic[] userFields)
        {
            SAPbouiCOM.Application b1App = app.Resolve<SAPbouiCOM.Application>();
            UserFieldsMD userFieldMD = (UserFieldsMD)b1Company.GetBusinessObject(BoObjectTypes.oUserFields);
            foreach (var field in userFields)
            {
                DoverSetup.removeField(userFieldMD, field.Tablename, field.Field, b1App, b1Company);
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(userFieldMD);
        }

        private void removeUDOs(string[] udos)
        {
            SAPbouiCOM.Application b1App = app.Resolve<SAPbouiCOM.Application>();
            UserObjectsMD userObjectsMD = (UserObjectsMD)b1Company.GetBusinessObject(BoObjectTypes.oUserObjectsMD);
            foreach (var udo in udos)
            {
                DoverSetup.removeUDO(userObjectsMD, udo, b1App, b1Company);
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(userObjectsMD);
        }

        [TestCleanup]
        public void Cleanup()
        {
            DoverSetup.shutdownDover();
        }

        [TestMethod]
        public void InstallDover()
        {
            BusinessOneDAO b1dao = app.Resolve<BusinessOneDAO>();
            Assert.AreEqual( b1dao.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_MODULES\""), 11);
            Assert.IsTrue( b1dao.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_MODULES_BIN\"") > 0);
            Assert.IsTrue( b1dao.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_MODULES_DEP\"") > 0);
            Assert.AreEqual( b1dao.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_MODULES_USER\""), 0);
            Assert.AreEqual( b1dao.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_LOGS\""), 0);
            Assert.AreEqual( b1dao.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_LICENSE\""), 0);
            Assert.AreEqual( b1dao.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_LICENSE_BIN\""), 0);
        }

        [TestMethod]
        public void InstallAndRestartDover()
        {
            BusinessOneDAO b1dao = app.Resolve<BusinessOneDAO>();
            Assert.AreEqual( b1dao.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_MODULES\""), 11);
            Assert.IsTrue( b1dao.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_MODULES_BIN\"") > 0);
            Assert.IsTrue( b1dao.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_MODULES_DEP\"") > 0);
            Assert.AreEqual( b1dao.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_MODULES_USER\""), 0);
            Assert.AreEqual( b1dao.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_LOGS\""), 0);
            Assert.AreEqual( b1dao.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_LICENSE\""), 0);
            Assert.AreEqual( b1dao.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_LICENSE_BIN\""), 0);
            
            DoverSetup.shutdownDover();
            DoverSetup.bootDover(app);
            b1Company = app.Resolve<SAPbobsCOM.Company>();
            b1App = app.Resolve<SAPbouiCOM.Application>();
            b1dao = app.Resolve<BusinessOneDAO>();
            Assert.AreEqual( b1dao.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_MODULES\""), 11);
            Assert.IsTrue( b1dao.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_MODULES_BIN\"") > 0);
            Assert.IsTrue( b1dao.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_MODULES_DEP\"") > 0);
            Assert.AreEqual( b1dao.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_MODULES_USER\""), 0);
            Assert.AreEqual( b1dao.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_LOGS\""), 0);
            Assert.AreEqual( b1dao.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_LICENSE\""), 0);
            Assert.AreEqual( b1dao.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_LICENSE_BIN\""), 0);
        }

        [TestMethod]
        public void InstallI18NAddinWithLicenseControl()
        {
            InstallAddin("DOVER_WL", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                                "..\\..\\..\\Examples\\testAddins\\withLicense\\I18NExample.dover"),
                "I18NExample",
                new List<UpdateDbValue>() {
					new UpdateDbValue() { Type=Dover.Framework.Service.Messages.UDO, Value="[TestUDO]"}, 
					new UpdateDbValue() { Type=Dover.Framework.Service.Messages.UserTable, Value="[TEST]"}, 
					new UpdateDbValue() { Type=Dover.Framework.Service.Messages.UserTable, Value="[TEST_LINES]"}, 
					new UpdateDbValue() { Type=Dover.Framework.Service.Messages.UserTable, Value="[TEST_LINES2]"}, 
					new UpdateDbValue() { Type=Dover.Framework.Service.Messages.UserField, Value="[@TEST].[CardCode]"}, 
					new UpdateDbValue() { Type=Dover.Framework.Service.Messages.UserField, Value="[@TEST].[CardName]"}, 
					new UpdateDbValue() { Type=Dover.Framework.Service.Messages.UserField, Value="[@TEST_LINES].[ItemCode]"}, 
					new UpdateDbValue() { Type=Dover.Framework.Service.Messages.UserField, Value="[@TEST_LINES].[Quantity]"}, 
					new UpdateDbValue() { Type=Dover.Framework.Service.Messages.UserField, Value="[@TEST_LINES].[ItemName]"}, 
					new UpdateDbValue() { Type=Dover.Framework.Service.Messages.UserField, Value="[@TEST_LINES2].[SlpCode]"}, 
					new UpdateDbValue() { Type=Dover.Framework.Service.Messages.UserField, Value="[@TEST_LINES2].[Percentage]"}, 
                });
        }

        [TestMethod]
        public void InstallI18NAddin()
        {
            InstallAddin("DOVER_NL", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                            "..\\..\\..\\Examples\\testAddins\\withoutLicense\\I18NExample.dover"),
                 "I18NExample",
                new List<UpdateDbValue>() {
					new UpdateDbValue() { Type=Dover.Framework.Service.Messages.UDO, Value="[TestUDO]"}, 
					new UpdateDbValue() { Type=Dover.Framework.Service.Messages.UserTable, Value="[TEST]"}, 
					new UpdateDbValue() { Type=Dover.Framework.Service.Messages.UserTable, Value="[TEST_LINES]"}, 
					new UpdateDbValue() { Type=Dover.Framework.Service.Messages.UserTable, Value="[TEST_LINES2]"}, 
					new UpdateDbValue() { Type=Dover.Framework.Service.Messages.UserField, Value="[@TEST].[CardCode]"}, 
					new UpdateDbValue() { Type=Dover.Framework.Service.Messages.UserField, Value="[@TEST].[CardName]"}, 
					new UpdateDbValue() { Type=Dover.Framework.Service.Messages.UserField, Value="[@TEST_LINES].[ItemCode]"}, 
					new UpdateDbValue() { Type=Dover.Framework.Service.Messages.UserField, Value="[@TEST_LINES].[Quantity]"}, 
					new UpdateDbValue() { Type=Dover.Framework.Service.Messages.UserField, Value="[@TEST_LINES].[ItemName]"}, 
					new UpdateDbValue() { Type=Dover.Framework.Service.Messages.UserField, Value="[@TEST_LINES2].[SlpCode]"}, 
					new UpdateDbValue() { Type=Dover.Framework.Service.Messages.UserField, Value="[@TEST_LINES2].[Percentage]"}, 
                });
        }
        
        [TestMethod]
        public void InstallAddinsWithDifferentNamespace()
        {
            InstallI18NAddin();
            InstallI18NAddinWithLicenseControl();
        }

        [TestMethod]
        public void InstallAddinsWithDifferentNamespaceAndRestart()
        {
            InstallLicense();
            InstallI18NAddin();
            InstallI18NAddinWithLicenseControl();

            DoverSetup.shutdownDover();
            DoverSetup.bootDover(app);
            b1Company = app.Resolve<SAPbobsCOM.Company>();
            b1App = app.Resolve<SAPbouiCOM.Application>();

            Form adminForm = UIHelper.GetFormAfterAction("dover.formAdmin", b1App, () => b1App.Menus.Item("doverAdmin").Activate());
            string dtxml = UIHelper.ExportDTXML(adminForm, "modDT");
            XDocument xdoc = XDocument.Parse(dtxml);

            CheckAddinStatus("DOVER_WL", "I18NExample", "Y", "R", xdoc);
            CheckAddinStatus("DOVER_NL", "I18NExample", "Y", "R", xdoc);
            AssertNoFrameworkError();
        }

        private void InstallAddin(string addinNamespace, string filePath, string addinName, List<UpdateDbValue> updateDbList)
        {
            Form adminForm = UIHelper.GetFormAfterAction("dover.formAdmin", b1App, () => b1App.Menus.Item("doverAdmin").Activate() );
            EditText fileEdit = (SAPbouiCOM.EditText)adminForm.Items.Item("edArquivo").Specific;
            Item installButton = adminForm.Items.Item("btInst");
            
            if (!File.Exists(filePath))
            {
                Assert.Fail(string.Format("addin {0} does not exists", filePath));
            }

            fileEdit.Value = filePath;
            Form dbChangeForm = UIHelper.GetFormAfterAction("Dover.dbchange", b1App, () => installButton.Click());
            string dtxml = UIHelper.ExportDTXML(dbChangeForm, "dbchange");
            XDocument xdoc = XDocument.Parse(dtxml);
            foreach (var value in updateDbList)
            {
                CheckDBChangeValue(value.Type, value.Value, xdoc);
            }
            Assert.AreEqual(updateDbList.Count, CheckDBChangeCount(xdoc));

            dbChangeForm.Items.Item("btWarn").Click();
            dtxml = UIHelper.ExportDTXML(adminForm, "modDT");
            xdoc = XDocument.Parse(dtxml);
            CheckAddinStatus(addinNamespace, addinName, "N", "S", xdoc);
            adminForm.Close();
            AssertNoFrameworkError();
        }


        [TestMethod]
        public void InstallI18NAddinAndRestart()
        {
            InstallI18NAddin();
            DoverSetup.shutdownDover();
            DoverSetup.bootDover(app);
            b1App = app.Resolve<SAPbouiCOM.Application>();

            Form adminForm = UIHelper.GetFormAfterAction("dover.formAdmin", b1App, () => b1App.Menus.Item("doverAdmin").Activate() );
            string dtxml = UIHelper.ExportDTXML(adminForm, "modDT");
            XDocument xdoc = XDocument.Parse(dtxml);

            CheckAddinStatus("DOVER_NL", "I18NExample", "Y", "R", xdoc);
            AssertNoFrameworkError();
        }

        [TestMethod]
        public void InstallI18NAddinAndStart()
        {
            InstallI18NAddin();
            Form adminForm = UIHelper.GetFormAfterAction("dover.formAdmin", b1App, () => b1App.Menus.Item("doverAdmin").Activate() );
            Item startButton = adminForm.Items.Item("btStart");
            DataTable adminDT = adminForm.DataSources.DataTables.Item("modDT");
            Grid adminGrid = (Grid)adminForm.Items.Item("gridArq").Specific;

            for (int i = 0; i < adminDT.Rows.Count; ++i)
            {
                if ((string)adminDT.GetValue("Name", i) == "I18NExample" && (string)adminDT.GetValue("Namespace", i) == "DOVER_NL")
                {
                    adminGrid.Rows.SelectedRows.Add(i);
                    startButton.Enabled = true;
                    startButton.Click();
                    string dtxml = UIHelper.ExportDTXML(adminForm, "modDT");
                    XDocument xdoc = XDocument.Parse(dtxml);

                    CheckAddinStatus("DOVER_NL", "I18NExample", "Y", "R", xdoc);
                    return;
                }
            }
            Assert.Fail("No I18NExample module found");
            AssertNoFrameworkError();
        }

        [TestMethod]
        public void InstallI18NAddinAndStartStop()
        {
            InstallI18NAddin();
            Form adminForm = UIHelper.GetFormAfterAction("dover.formAdmin", b1App, () => b1App.Menus.Item("doverAdmin").Activate() );
            Item startButton = adminForm.Items.Item("btStart");
            Item stopButton = adminForm.Items.Item("btStop");
            DataTable adminDT = adminForm.DataSources.DataTables.Item("modDT");
            Grid adminGrid = (Grid)adminForm.Items.Item("gridArq").Specific;

            for (int i = 0; i < adminDT.Rows.Count; ++i)
            {
                if ((string)adminDT.GetValue("Name", i) == "I18NExample" && (string)adminDT.GetValue("Namespace", i) == "DOVER_NL")
                {
                    adminGrid.Rows.SelectedRows.Add(i);
                    startButton.Enabled = true;
                    startButton.Click();
                    string dtxml = UIHelper.ExportDTXML(adminForm, "modDT");
                    XDocument xdoc = XDocument.Parse(dtxml);

                    CheckAddinStatus("DOVER_NL", "I18NExample", "Y", "R", xdoc);
                    adminGrid.Rows.SelectedRows.Add(i);
                    stopButton.Enabled = true;
                    stopButton.Click();

                    dtxml = UIHelper.ExportDTXML(adminForm, "modDT");
                    xdoc = XDocument.Parse(dtxml);

                    CheckAddinStatus("DOVER_NL", "I18NExample", "Y", "S", xdoc);
                    return;
                }
            }
            Assert.Fail("No I18NExample module found");
            AssertNoFrameworkError();
        }

        [TestMethod]
        public void InstallI18NAddinAndInstallAndStart()
        {
            InstallI18NAddin();
            Form adminForm = UIHelper.GetFormAfterAction("dover.formAdmin", b1App, () => b1App.Menus.Item("doverAdmin").Activate() );
            Item startButton = adminForm.Items.Item("btStart");
            Item initializeButton = adminForm.Items.Item("btInstall");
            DataTable adminDT = adminForm.DataSources.DataTables.Item("modDT");
            Grid adminGrid = (Grid)adminForm.Items.Item("gridArq").Specific;

            for (int i = 0; i < adminDT.Rows.Count; ++i)
            {
                if ((string)adminDT.GetValue("Name", i) == "I18NExample" && (string)adminDT.GetValue("Namespace", i) == "DOVER_NL")
                {
                    adminGrid.Rows.SelectedRows.Add(i);
                    initializeButton.Enabled = true;
                    initializeButton.Click();
                    string dtxml = UIHelper.ExportDTXML(adminForm, "modDT");
                    XDocument xdoc = XDocument.Parse(dtxml);

                    CheckAddinStatus("DOVER_NL", "I18NExample", "Y", "S", xdoc);
                    startButton.Enabled = true;
                    adminGrid.Rows.SelectedRows.Add(i);
                    startButton.Click();
                    
                    dtxml = UIHelper.ExportDTXML(adminForm, "modDT");
                    xdoc = XDocument.Parse(dtxml);
                    CheckAddinStatus("DOVER_NL", "I18NExample", "Y", "R", xdoc);

                    return;
                }
            }
            Assert.Fail("No I18NExample module found");
            AssertNoFrameworkError();
        }

        [TestMethod]
        public void InstallLicense()
        {
            LicenseHeader header = new LicenseHeader()
            {
                InstallNumber = b1App.Company.InstallationId,
                SystemNumber = b1App.Company.SystemId,
                Items = new List<LicenseModule>()
                {
                    new LicenseModule() { Description = "I18NExample", Name = "I18NExample", ExpirationDate = DateTime.Today.AddDays(3) },
                    new LicenseModule() { Description = "Foo", Name = "Foo", ExpirationDate = DateTime.Today.AddDays(3) },
                },
                LicenseNamespace = "DOVER_WL"
            };
            LicenseService service = new LicenseService();
            var privateKey = CreateKeys.CreateKeyPair();
            string xmlLicense = service.GenerateLicense(header, privateKey);
            string licensePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "license.xml");
            File.WriteAllText(licensePath, xmlLicense);

            Form licenseForm = UIHelper.GetFormAfterAction("Dover.license", b1App, () => b1App.Menus.Item("doverLicense").Activate() );
            EditText licensePathEditText = (EditText)licenseForm.Items.Item("moduleEd").Specific;
            licensePathEditText.Value = licensePath;
            licenseForm.Items.Item("importBt").Click();
            AssertNoFrameworkError();
            BusinessOneDAO b1DAO = app.Resolve<BusinessOneDAO>();

            Assert.AreEqual(b1DAO.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_LICENSE\""), 1);
            Assert.AreEqual(b1DAO.ExecuteSqlForObject<string>("select \"U_namespace\" from \"@DOVER_LICENSE\""), "DOVER_WL");
        }

        [TestMethod]
        public void InstallLicenseAndI18NAddinAndRestart()
        {
            InstallLicense();
            InstallI18NAddinWithLicenseControl();

            DoverSetup.shutdownDover();
            DoverSetup.bootDover(app);
            b1Company = app.Resolve<SAPbobsCOM.Company>();
            b1App = app.Resolve<SAPbouiCOM.Application>();

            Form adminForm = UIHelper.GetFormAfterAction("dover.formAdmin", b1App, () => b1App.Menus.Item("doverAdmin").Activate());
            string dtxml = UIHelper.ExportDTXML(adminForm, "modDT");
            XDocument xdoc = XDocument.Parse(dtxml);

            CheckAddinStatus("DOVER_WL", "I18NExample", "Y", "R", xdoc);
        }

        [TestMethod]
        public void InstallI18NAddinWithLicenseControlAndRestart()
        {
            InstallI18NAddinWithLicenseControl();

            DoverSetup.shutdownDover();
            DoverSetup.bootDover(app);
            b1Company = app.Resolve<SAPbobsCOM.Company>();
            b1App = app.Resolve<SAPbouiCOM.Application>();

            Form adminForm = UIHelper.GetFormAfterAction("dover.formAdmin", b1App, () => b1App.Menus.Item("doverAdmin").Activate());
            string dtxml = UIHelper.ExportDTXML(adminForm, "modDT");
            XDocument xdoc = XDocument.Parse(dtxml);

            CheckAddinStatus("DOVER_WL", "I18NExample", "N", "S", xdoc);
        }

        private void CheckAddinStatus(string addinNamespace, string addinName, string installed, string status, XDocument xdoc)
        {
            bool hasElement = (from dt in xdoc.Elements("DataTable")
                        from rows in dt.Elements("Rows")
                        from row in rows.Elements("Row")
                        from cells in row.Elements("Cells")
                        where 
                            (from cell in cells.Elements("Cell")
                                where (string)cell.Element("ColumnUid") == "Name"
                                   && (string)cell.Element("Value") == addinName
                            select cell
                            ).Any()
                        &&
                            (from cell in cells.Elements("Cell")
                                where (string)cell.Element("ColumnUid") == "Installed"
                                   && (string)cell.Element("Value") == installed
                            select cell
                            ).Any()
                        &&
                            (from cell in cells.Elements("Cell")
                                where (string)cell.Element("ColumnUid") == "Status"
                                   && (string)cell.Element("Value") == status
                            select cell
                            ).Any()
                        &&
                            (from cell in cells.Elements("Cell")
                                where (string)cell.Element("ColumnUid") == "Namespace"
                                   && (string)cell.Element("Value") == addinNamespace
                            select cell
                            ).Any()
                        select dt).Any();
            Assert.IsTrue(hasElement, 
                string.Format("CheckAddinSatatus: Namespace {3}, addin {0}, installed {1} and status {2} not found on datatable",
                addinName, installed, status, addinNamespace));
        }

        private int CheckDBChangeCount(XDocument xdoc)
        {
            return (from dt in xdoc.Elements("DataTable")
             from rows in dt.Elements("Rows")
             from row in rows.Elements("Row")
             from cells in row.Elements("Cells")
             select cells).Count();
        }

        private void CheckDBChangeValue(string type, string key, XDocument xdoc)
        {
            bool hasElement = (from dt in xdoc.Elements("DataTable")
                        from rows in dt.Elements("Rows")
                        from row in rows.Elements("Row")
                        from cells in row.Elements("Cells")
                        where 
                            (from cell in cells.Elements("Cell")
                                where (string)cell.Element("ColumnUid") == "type"
                                   && (string)cell.Element("Value") == type
                            select cell
                            ).Any()
                        &&
                            (from cell in cells.Elements("Cell")
                                where (string)cell.Element("ColumnUid") == "key"
                                   && (string)cell.Element("Value") == key
                            select cell
                            ).Any()
                        select dt).Any();
            Assert.IsTrue(hasElement, "DBChange: Did not found " + key + " - " + type);
        }

        private void AssertNoFrameworkError()
        {
            BusinessOneDAO b1dao = app.Resolve<BusinessOneDAO>();
            Assert.AreEqual( b1dao.ExecuteSqlForObject<int>("select count(*) from \"@DOVER_LOGS\""), 0);
        }

    }
}
