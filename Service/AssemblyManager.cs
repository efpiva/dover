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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Dover.Framework.Attribute;
using Dover.Framework.DAO;
using Dover.Framework.Log;
using Dover.Framework.Model;
using Dover.Framework.Monad;
using Castle.Core.Logging;
using ICSharpCode.SharpZipLib.Zip;
using Dover.Framework.Factory;

namespace Dover.Framework.Service
{
    internal enum AssemblySource
    {
        Core,
        AddIn
    }

    public class AssemblyManager
    {
        private string[] addinsAssemblies = {
        };

        private string[] coreAssemblies = {
            "Framework.dll",
            "Dover.exe"
        };
        private AssemblyDAO asmDAO;
        private LicenseManager licenseManager;
        private I18NService i18nService;
        public ILogger Logger { get; set; }


        public AssemblyManager(AssemblyDAO asmDAO, LicenseManager licenseManager, I18NService i18nService)
        {
            this.asmDAO = asmDAO;
            this.licenseManager = licenseManager;
            this.i18nService = i18nService;
        }

        internal void RemoveAddIn(string moduleName)
        {
            // TODO: reload appDomain!
            asmDAO.RemoveAssembly(moduleName);
            Logger.Info(string.Format(Messages.RemoveAddinSuccess, moduleName));
        }

        /// <summary>
        /// Return true if the addin is valid.
        /// </summary>
        /// <param name="path">Path name for the intended addin</param>
        /// <param name="comments">Comments to be displayed to the end user, linke tabled that are going to be created.</param>
        /// <returns>true if addin is valid</returns>
        internal bool AddInIsValid(string path, out string comments)
        {
            string extension = Path.GetExtension(path);
            AppDomain testDomain = null;
            string mainDll = string.Empty;
            bool ret;

            // check if it's a DLL or ZIP.
            if (extension != null && extension == ".zip")
            {
                testDomain = CreateTestDomain();
                mainDll = UnzipFile(path, testDomain.BaseDirectory);
            }
            else if (extension != null && (extension == ".dll" || extension == ".exe"))
            {
                testDomain = CreateTestDomain();
                string destination = Path.Combine(testDomain.BaseDirectory, Path.GetFileName(path));
                File.Copy(path, destination);
                mainDll = Path.GetFileName(path);
            }
            else
            {
                throw new ArgumentException(Messages.InvalidAddInExtension);
            }
            mainDll = mainDll.Substring(0, mainDll.Length - 4);
            testDomain.SetData("assemblyName", mainDll); // Used to get current AssemblyName for logging and reflection
            Application testApp = (Application)testDomain.CreateInstanceAndUnwrap("Framework", "Dover.Framework.Application");
            SAPServiceFactory.PrepareForInception(testDomain);
            var addinManager = testApp.Resolve<AddinManager>();
            ret = addinManager.CheckAddinConfiguration(mainDll, out comments);
            testApp.ShutDownApp();
            AppDomain.Unload(testDomain); // TODO: clean up temp directory.
            return ret;
        }

        /// <summary>
        /// Unzip a zip file in a specific path.
        /// </summary>
        /// <param name="path">path for the zip file</param>
        /// <param name="destinationFolder">destination folder to unzip</param>
        /// <returns>main DLL found on zip archive.</returns>
        private string UnzipFile(string path, string destinationFolder)
        {
            string mainDll = null;
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(path)))
            {

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {

                    Console.WriteLine(theEntry.Name);

                    string baseDirName = Path.GetDirectoryName(theEntry.Name);
                    string directoryName = Path.Combine(destinationFolder,  baseDirName);
                    string fileName = Path.GetFileName(theEntry.Name);

                    // create directory
                    if (baseDirName.Length > 0)
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                    else if (fileName.EndsWith(".dll"))
                    {
                        if (mainDll != null)
                        {
                            throw new ArgumentException(Messages.PackageContainsMoreThanOneDLL);
                        }
                        mainDll = fileName; // root main DLL.
                    }

                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(Path.Combine(destinationFolder, theEntry.Name)))
                        {

                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return mainDll;
        }

        private AppDomain CreateTestDomain()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);

            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationName = "Addin.test";
            setup.ApplicationBase = tempDirectory;

            AppDomain testDomain = AppDomain.CreateDomain("Addin.test", null, setup);
            UpdateAssemblies(AssemblySource.Core, tempDirectory);

            return testDomain;
        }

        /// <summary>
        /// Register a valid addin. WARNING: this method does not check if the addin is valid, it just
        /// save it to the database with the correct data structure, such as version and MD5SUM. It's important
        /// to call AddInIsValid if you're not sure on what is being passed as path, otherwise there will
        /// be errors during addin startup.
        /// </summary>
        /// <param name="path">path for the file to be saved</param>
        internal void SaveAddIn(string path)
        {
            if (path == null || path.Length < 4)
            {
                Logger.Error(string.Format(Messages.SaveAddInError, path.Return( x => x, String.Empty)));
            }
            else
            {
                try
                {
                    string directory;
                    string fileName = Path.GetFileName(path);
                    string addInName = fileName.Substring(0, fileName.Length - 4);
                    bool hasi18n = false;
                    byte[] asmBytes;

                    AssemblyInformation existingAsm = asmDAO.GetAssemblyInformation(addInName, "A");

                    if (fileName.EndsWith(".zip"))
                    {
                        directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                        Directory.CreateDirectory(directory);
                        fileName = UnzipFile(path, directory);
                        hasi18n = true;
                    }
                    else
                    {
                        directory = Path.GetDirectoryName(path);
                    }

                    AssemblyInformation newAsm = GetNewAsm(directory, fileName, addInName, "A", out asmBytes);
                    AssemblyInformation savedAsm = SaveIfNotExistsOrDifferent(existingAsm, newAsm, asmBytes);
                    if (hasi18n)
                    {
                        SaveAddinI18NResources(directory, addInName, savedAsm.Code);
                    }

                    licenseManager.BootLicense(); // reload licenses to include added license.
                    Logger.Info(string.Format(Messages.SaveAddInSuccess, path));
                }
                catch (Exception e)
                {
                    Logger.Error(string.Format(Messages.SaveAddInError, path), e);
                }
            }

        }

        private void SaveAddinI18NResources(string directory, string addInName, string moduleCode)
        {
            string[] i18nDirectories = Directory.GetDirectories(directory);
            foreach (string i18nPath in i18nDirectories)
            {
                string i18n = Path.GetFileName(i18nPath);
                string resourceAsm = Path.Combine(directory, i18n, addInName + ".resources.dll");
                
                if (i18nService.IsValidi18NCode(i18n) && File.Exists(resourceAsm))
                {
                    asmDAO.SaveAssemblyI18N(moduleCode, i18n, File.ReadAllBytes(resourceAsm));
                }
            }
        }

        internal void UpdateAssemblies(AssemblySource assemblyLocation, string appFolder)
        {
            List<AssemblyInformation> asms;
            Logger.Debug(DebugString.Format(Messages.UpdatingAssembly, assemblyLocation));
            if (assemblyLocation == AssemblySource.Core)
            {
                asms = InitializeCoreAssemblies(appFolder);
            }
            else
                asms = InitializeAddInAssemblies(appFolder);

            foreach(var asm in asms)
            {
                string fullPath = Path.Combine(appFolder, asm.FileName);
                if (IsDifferent(asm, appFolder, fullPath))
                {
                    UpdateAssembly(asm, fullPath);
                    UpdateI18NAssembly(asm, appFolder);
                }
            }
        }

        private List<AssemblyInformation> InitializeAddInAssemblies(string appFolder)
        {
            List<AssemblyInformation> addinsAsms = asmDAO.getAssembliesInformation("A");
            GenericInitialize(addinsAsms, "A", addinsAssemblies);
            return addinsAsms;
        }

        private List<AssemblyInformation> InitializeCoreAssemblies(string appFolder)
        {
            List<AssemblyInformation> coreAsms = asmDAO.getAssembliesInformation("C");
            GenericInitialize(coreAsms, "C", coreAssemblies);
            return coreAsms;
        }

        private void GenericInitialize(List<AssemblyInformation> asms, string type,
            string[] defaultAsms)
        {
            HashSet<string> dbAsms = new HashSet<string>();
            byte[] asmBytes;

            foreach(var asm in asms)
            {
                if (asmDAO.AutoUpdateEnabled(asm))
                {
                    try
                    {
                        AssemblyInformation newAsm = GetNewAsm(Environment.CurrentDirectory, asm.FileName, asm.Name, type, out asmBytes);
                        AssemblyInformation savedAsm = SaveIfNotExistsOrDifferent(asm, newAsm, asmBytes);
                        if (savedAsm.MD5 != asm.MD5)
                        {
                            SaveAddinI18NResources(Environment.CurrentDirectory, asm.Name, asm.Code);
                            asm.MD5 = savedAsm.MD5; // update MD5Sum, so AppData is updated latter.
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        // Ignore it, use DB version.
                        Logger.Warn(string.Format(Messages.FileMissing, asm.Name, "?"));
                    }
                }
                dbAsms.Add(asm.Name);
            }
            foreach (string asmFile in defaultAsms)
            {
                string asmName = asmFile.Substring(0, asmFile.Length - 4);
                if (!dbAsms.Contains(asmName)) // first upload, do not check if AutoUpdateEnabled and if filenotfound, force error (no try-catch).
                {
                    AssemblyInformation newAsm = GetNewAsm(Environment.CurrentDirectory, asmFile, asmName, type, out asmBytes);
                    AssemblyInformation savedAsm = SaveIfNotExistsOrDifferent(null, newAsm, asmBytes);
                    SaveAddinI18NResources(Environment.CurrentDirectory, asmName, savedAsm.Code);
                    asms.Add(savedAsm); // do not need to check if is valid, it's default asm.
                }
            }
        }

        private AssemblyInformation GetNewAsm(string directory, string filename, string name, string type,
            out byte[] asmBytes)
        {
            string path = Path.Combine(directory, filename);
            AssemblyInformation newAsm = new AssemblyInformation();
            newAsm.Name = name;
            newAsm.FileName = filename;
            asmBytes = File.ReadAllBytes(path);
            GetAssemblyInfoFromBin(asmBytes, newAsm);
            newAsm.MD5 = MD5Sum(asmBytes);
            newAsm.Size = asmBytes.Length;
            newAsm.Date = DateTime.Now;
            newAsm.Type = type;

            return newAsm;
        }

        private AssemblyInformation SaveIfNotExistsOrDifferent(AssemblyInformation existingAsm,
            AssemblyInformation newAsm, byte[] asmBytes)
        {
            if (existingAsm != null)
                newAsm.Code = existingAsm.Code; // Prepare for update.

            if (existingAsm == null || newAsm.Version.CompareTo(existingAsm.Version) == 1
                || (newAsm.Version == existingAsm.Version && newAsm.MD5 != existingAsm.MD5))
            {
                asmDAO.SaveAssembly(newAsm, asmBytes);
                return newAsm;
            }
            else
            {
                return existingAsm;
            }

        }

        private void GetAssemblyInfoFromBin(byte[] asmBytes, AssemblyInformation asmInfo)
        {
            bool found = false;
            Assembly asm = AppDomain.CurrentDomain.Load(asmBytes);
            var version = asm.GetName().Version;
            asmInfo.Version = version.Major.ToString() + "." + version.Minor.ToString() + "." + version.Build.ToString()
                    + "." + version.Revision;

            var types = (from type in asm.GetTypes()
                        where type.IsClass
                        select type);

            foreach (var type in types)
            {
                var attrs = type.GetCustomAttributes(true);
                foreach (var attr in attrs)
                {
                    if (attr is AddInAttribute)
                    {
                        var addInAttribute = ((AddInAttribute)attr);
                        if (!string.IsNullOrEmpty(addInAttribute.i18n))
                        {
                            asmInfo.Description = i18nService.GetLocalizedString(addInAttribute.i18n, asm);
                        }
                        else
                        {
                            asmInfo.Description = ((AddInAttribute)attr).Description;
                        }
                        found = true;
                        break;
                    }
                }
                if (found)
                    break;
            }

        }

        private void UpdateAssembly(AssemblyInformation asmMeta, string fullPath)
        {
            try
            {
                byte[] asmBytes = asmDAO.GetAssembly(asmMeta);
                if (asmBytes != null)
                {
                    File.WriteAllBytes(fullPath, asmBytes);
                    Logger.Info(String.Format(Messages.FileUpdated, asmMeta.Name, asmMeta.Version));
                }
                else
                {
                    Logger.Warn(String.Format(Messages.FileMissing, asmMeta.Name, asmMeta.Version));
                }
            }
            catch (Exception e)
            {
                Logger.Error(String.Format(Messages.FileError, asmMeta.Name, asmMeta.Version), e);
            }
        }

        private void UpdateI18NAssembly(AssemblyInformation asm, string appFolder)
        {
            List<string> supportedI18N = asmDAO.GetSupportedI18N(asm);
            foreach (var i18n in supportedI18N)
            {
                Directory.CreateDirectory(Path.Combine(appFolder, i18n));
                string asmName = asm.Name + ".resources.dll";
                try
                {
                    byte[] asmBytes = asmDAO.GetI18NAssembly(asm, i18n);
                    if (asmBytes != null)
                    {
                        File.WriteAllBytes(Path.Combine(appFolder, i18n, asmName), asmBytes);
                        Logger.Info(String.Format(Messages.FileUpdated, asmName, asm.Version));
                    }
                    else
                    {
                        Logger.Warn(String.Format(Messages.FileMissing, asmName, asm.Version));
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(String.Format(Messages.FileError, asmName, asm.Version), e);
                }
            }
        }

        private bool IsDifferent(AssemblyInformation asm, string appFolder, string fullPath)
        {
            return !File.Exists(fullPath) || !CheckSum(asm.MD5, fullPath);
        }

        private string MD5Sum(byte[] fileByte)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(fileByte);
                var sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
                var md5sum = sb.ToString();
                Logger.Debug(DebugString.Format(Messages.MD5Sum, md5sum));
                return md5sum;
            }
        }

        private bool CheckSum(string asmHash, string filename)
        {
            Logger.Debug(DebugString.Format(Messages.CheckSum, asmHash, filename));
            byte[] fileByte = File.ReadAllBytes(filename);
            return MD5Sum(fileByte) == asmHash;
        }

    }
}
