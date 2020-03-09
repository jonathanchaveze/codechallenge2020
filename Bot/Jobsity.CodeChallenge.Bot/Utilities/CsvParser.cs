using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.IO;

namespace Jobsity.CodeChallenge.Bot.Utilities
{
    public class CsvParser
    {
        public List<string[]> Parse(byte[] buffer)
        {
            var result = new List<string[]>();

            using (TextFieldParser parser = new TextFieldParser(new MemoryStream(buffer)))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    result.Add(fields);
                }
            }

            return result;
        }
    }
}