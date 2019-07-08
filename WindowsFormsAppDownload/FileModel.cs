using System;

namespace WindowsFormsAppDownload
{
    public class FileModel
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsActive { get; set; }
        public static FileModel FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');
            FileModel dailyValues = new FileModel();


            dailyValues.Start = Convert.ToDateTime(values[0]);
            dailyValues.End = Convert.ToDateTime(values[1]);
            dailyValues.IsActive = Convert.ToBoolean(values[2]);

            return dailyValues;
        }
    }
}
