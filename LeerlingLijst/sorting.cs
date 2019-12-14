using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace LeerlingLijst
{
    class Sorting
    {
        public static Dictionary<string, string> CompareKlassen = new Dictionary<string, string>()
        {
            {"lo", "Lichaamelijk opvoeding"},
            {"fatl", "Frans"},
            {"wisa", "Wiskunde A"},
            {"entl", "Engels"},
            {"ak", "Aarderijkskunde"},
            {"netl", "Nederlands"},
            {"ges", "Geschiedenis"},
            {"econ", "Economie"},
            {"gds", "Godsdienst"},
            {"bsm", "Beweging sport management (bsm)"},
            {"ckv", "Cultureel kunstzinnige vorming (ckv)"},
            {"nlt", "Natuur leven technologie (nlt)"},
            {"biol", "Biologie"},
            {"wisb", "Wiskunde B"},
            {"nat", "Natuurkunde"},
            {"schk", "Scheikunde"},
            {"chtl", "Chinees"},
            {"dutl", "Duits"},
            {"gtc", "Griekse taal en cultuur"},
            {"kubv", "Kunst bevo"},
            {"ltc", "Latijnse taal cultuur"},
            {"kua", "Kunst algemeen"},
            {"beco", "Bedrijfseconomie"},
            {"entlt", "Engels tweetalig"},
            {"mo", "Management organisatie"},
            {"maw", "Maatschappij wetenschappen"},
            {"wisd", "Wiskunde D"},
            {"maat", "Maatschappijleer"},
            {"begl_ne", "beglijding nederlands"},
            {"kumu", "Kunst muziek"},
            {"begl_en", "Beglijding engels"},
            {"begl_wi", "Beglijding wiskunde"},
            {"wi", "wiskunde"},
            {"du", "Duits"},
            {"gd", "godsdienst"},
            {"na", "Natuurkunde"},
            {"en", "Engels"},
            {"mu", "Muziek"},
            {"ne", "Nederlands"},
            {"gs", "Geschiedenis"},
            {"ml", "Mentorles"},
            {"sk", "Scheikunde"},
            {"ec", "Economie"},
            {"fa", "Frans"},
            {"bv", "Bevo"},
            {"ch", "Chinees"},
            {"gr", "Griekse taal en cultuur"},
            {"la", "Latijnse taal cultuur"},
            {"ent", "Engels"},
            {"sc", "Science"},
            {"bi", "Biologie"},
            {"tkdv", "Talentklas Davinci"},
            {"tkkc", "Talentklas kunst cultuur"},
            {"tksp", "Talentklas sport"},
            {"tkwe", "Talentklas wereld"},
            {"dr", "Dr"}
        };
        public static async Task<SortedDictionary<string, SortedDictionary<string, List<string>>>> GroupBySubject(List<UData> users, Api api, long start, long end, ProgressBar progressBar, Label label)
        {
            SortedDictionary<string, SortedDictionary<string, List<string>>> klassen = new SortedDictionary<string, SortedDictionary<string, List<string>>>();
            progressBar.Maximum = users.Count;
            progressBar.Value = 0;

            List<SData> days;
            string name;
            foreach (UData i in users)
            {
                if (i.prefix == "")
                {
                    name = $"{i.firstName} {i.lastName}";
                }
                else
                {
                    name = $"{i.firstName} {i.prefix} {i.lastName}";
                }
                progressBar.Value += 1;
                label.Text = name;
                Console.WriteLine($"{i.firstName} {i.prefix} {i.lastName} - {i.code}");
                ScheduleResponse schedule = await api.GetSchedule(start, end, user: i.code);
                days = schedule.response.data;
                foreach (SData day in days)
                {
                    foreach (string subject in day.subjects)
                    {
                        if (!klassen.ContainsKey(subject))
                        {
                            klassen[subject] = new SortedDictionary<string, List<string>>();
                        }
                        foreach (string group in day.groups)
                        {
                            if (!klassen[subject].ContainsKey(group))
                            {
                                klassen[subject][group] = new List<string>();
                            }
                            
                            if (!klassen[subject][group].Contains(name))
                            {
                                klassen[subject][group].Add(name);
                            }
                        }
                    }
                }
            }
            return klassen;
        }

        public static async Task SortVakkenToFile(Dictionary<string, string> klassen, SortedDictionary<string,SortedDictionary<string, List<string>>> toSort, String folder)
        {
            String fileName;

            foreach (string i in toSort.Keys)
            {
                SortedDictionary<string, List<string>> groups;
                if (klassen.ContainsKey(i))
                {
                    fileName = $"{folder}/{klassen[i]}.json";
                }
                else
                {
                    fileName = $"{folder}/{i}.json";
                }
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
                using (StreamWriter sw = new StreamWriter(stream))
                {
                    groups = toSort[i];
                    await sw.WriteAsync(JsonConvert.SerializeObject(groups, Formatting.Indented));
                }
            }
        }
        public static async Task SortLeerlingenToFile(Dictionary<string, string> klassen, SortedDictionary<string, SortedDictionary<string, List<string>>> compare, List<UData> names, string folder, ProgressBar progressBar, Label label)
        {
            progressBar.Maximum = names.Count;
            progressBar.Value = 0;
            SortedDictionary<string, SortedDictionary<string, List<string>>> currentDict;
            String currentName;
            foreach (UData name in names)
            {
                progressBar.Value += 1;
                if (name.prefix == "")
                {
                    currentName = $"{name.firstName} {name.lastName}";
                }
                else
                {
                    currentName = $"{name.firstName} {name.prefix} {name.lastName}";
                }
                label.Text = currentName;
                label.Update();
                Console.WriteLine(currentName);
                currentDict = new SortedDictionary<string, SortedDictionary<string, List<string>>>();
                foreach (KeyValuePair<string, SortedDictionary<string, List<string>>> keyValue0 in compare)
                {
                    if (keyValue0.Value.Values.Any(lst => lst.Contains(currentName)))
                    {
                        foreach (KeyValuePair<string, List<string>> keyValue1 in keyValue0.Value)
                        {
                            if (keyValue1.Value.Contains(currentName))
                            {
                                currentDict[keyValue0.Key] = new SortedDictionary<string, List<string>>();
                                currentDict[keyValue0.Key][keyValue1.Key] = keyValue1.Value;
                            }
                        }
                    }
                }
                await SortVakkenToFile(klassen, currentDict, $"{folder}/{currentName}");
            }
        }

    }
}
