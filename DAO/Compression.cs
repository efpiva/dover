/*
 *  Dover Framework - OpenSource Development framework for SAP Business One
 *  Copyright (C) 2015  Eduardo Piva
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
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Dover.Framework.DAO
{
    internal static class Compression
    {
        internal static byte[] Uncompress(byte[] p)
        {
            MemoryStream output = new MemoryStream();
            MemoryStream ms = new MemoryStream(p);
            using (ZipInputStream zis = new ZipInputStream(ms))
            {
                int size = 2048;
                byte[] data = new byte[size];

                if (zis.GetNextEntry() != null)
                {
                    while (true)
                    {
                        size = zis.Read(data, 0, size);
                        if (size > 0)
                        {
                            output.Write(data, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return output.ToArray();
        }

        internal static byte[] Compress(byte[] asmBytes)
        {
            MemoryStream ms = new MemoryStream();
            using (ZipOutputStream zos = new ZipOutputStream(ms))
            {
                ZipEntry ze = new ZipEntry("file");
                ze.Size = asmBytes.Length;
                zos.PutNextEntry(ze);
                zos.Write(asmBytes, 0, asmBytes.Length);
                zos.CloseEntry();
            }

            return ms.ToArray();
        }
    }
}
