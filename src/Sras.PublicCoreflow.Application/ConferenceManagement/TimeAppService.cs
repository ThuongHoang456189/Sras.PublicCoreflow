using Scriban.Parsing;
using Sras.PublicCoreflow.BlobContainer;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.Content;
using Volo.Abp.Guids;
using Volo.Abp.Users;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class TimeAppService : PublicCoreflowAppService, ITimeAppService
    {
        private readonly IGuidGenerator _guidGenerator;

        public TimeAppService(IGuidGenerator guidGenerator)
        {
            _guidGenerator = guidGenerator;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime(ref SYSTEMTIME st);

        public string ChangeSystemTime(string newDate)
        {
                // Set the desired new system time
                SYSTEMTIME st = new SYSTEMTIME
                {
                    wYear = 2009,
                    wMonth = 1,
                    wDay = 1,
                    wHour = 0,
                    wMinute = 0,
                    wSecond = 0
                };

                // Call the SetSystemTime function to change the system time
                bool success = SetSystemTime(ref st);

                if (success)
                {
                    return  "System time changed successfully.";
                }
                else
                {
                    return "Failed to change the system time.";
                }
        }

    }
}
