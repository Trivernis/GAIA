using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace GAIA
{
    namespace Files
    {
        //A File to save dictionaries
        public class DictionaryFile
        {
            private const string extension = ".dic";
            private FileInfo file;
            private Dictionary<string, string> dictA;


            public DictionaryFile(string name)
            {
                dictA = new Dictionary<string, string>();
                createDictionaryFile(name, "Data");
            }

            public DictionaryFile(string name, string directory)
            {
                dictA = new Dictionary<string, string>();
                createDictionaryFile(name, directory);
            }

            private void createDictionaryFile(string name, string directory)
            {
                //saving in standard directory 'Data'
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var fn = directory + "/" + name;

                //adding the extension .list if not existing
                if (!name.Contains(extension))
                {
                    fn += extension;
                }

                file = new FileInfo(fn);
                //reading the file if existent

                if (file.Exists)
                {
                    readFile();
                }
                else
                {
                    using (StreamWriter sw = file.CreateText())
                    {

                    }
                }
            }

            private void readFile()
            {
                foreach (var line in File.ReadAllLines(file.FullName))
                {
                    try
                    {
                        var l = line.Split('|');
                        dictA.Add(l[0], l[1]);
                    }
                    catch { }
                }
            }

            public void add(string key, string value)
            {
                dictA.Add(key, value);

                using (StreamWriter sw = file.AppendText())
                {
                    sw.WriteLine(key + "|" + value);
                }
            }

            public void remove(string key)
            {
                //removing the item by key 
                dictA.Remove(key);
                var lines = File.ReadAllLines(file.FullName);

                using (StreamWriter sw = file.CreateText())
                {
                    foreach(var line in lines)
                    {
                        if(!line.Split('|')[0].Equals(key))
                        {
                            sw.WriteLine(line);
                        }
                    }
                }
            }

            public void clear()
            {
                dictA.Clear();

                //Clearing the file
                using (StreamWriter sw = file.CreateText())
                {

                }
            }

            public Dictionary<string, string> getDictionary()
            {
                return dictA;
            }

            public void Delete()
            {
                file.Delete();
            }

            public void MoveTo(string path)
            {
                file.MoveTo(path);
            }

            public void CopyTo(string path)
            {
                file.CopyTo(path);
            }
        }

        //A File to save lists
        public class ListFile
        {
            //a file to store some lists in

            private FileInfo file;
            private List<string> listA;
            private const string extension = ".lst";

            public ListFile(string name)
            {
                listA = new List<string>();
                createListFile(name, "Data");
            }

            public ListFile(string name, string directory)
            {
                listA = new List<string>();
                createListFile(name, directory);
            }

            private void createListFile(string name, string directory)
            {
                //saving in standard directory 'Data'
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var fn = directory + "/" + name;

                //adding the extension .list if not existing
                if (!name.Contains(extension))
                {
                    fn += extension;
                }

                file = new FileInfo(fn);
                //reading the file if existent

                if (file.Exists)
                {
                    readFile();
                }
                else
                {
                    using (StreamWriter sw = file.CreateText())
                    {

                    }
                }
            }

            public void readFile()
            {
                //clearing the list to prevent doubled items
                listA.Clear();
                //reading the file line by line to list

                foreach (var line in File.ReadAllLines(file.FullName))
                {
                    listA.Add(line);
                }
            }

            public void add(string item)
            {
                //first appending item to list, then writing it to file
                listA.Add(item);

                using (StreamWriter sw = file.AppendText())
                {
                    sw.WriteLine(item);
                }
            }

            public void remove(int id)
            {
                listA.RemoveAt(id);
                var cId = 0;
                //getting the lines of the file
                var lines = File.ReadAllLines(file.FullName);

                //writing and skipping the line with the named id
                using (StreamWriter sw = file.AppendText())
                {
                    foreach (var line in lines)
                    {
                        if (cId != id)
                        {
                            sw.WriteLine(line);
                        }
                        cId++;
                    }
                }
            }

            public void clear()
            {
                //clearing list and overwriting file
                listA.Clear();

                using (StreamWriter sw = file.CreateText())
                {

                }
            }

            public List<string> getList()
            {
                return listA;
            }

            public void Delete()
            {
                file.Delete();
            }

            public void MoveTo(string destFileName)
            {
                file.MoveTo(destFileName);
            }

            public void CopyTo(string destFileName)
            {
                file.CopyTo(destFileName);
            }
        }

        //The standard Logfile
        public class LogFile
        {
            public FileInfo logfile;
            public bool withTime = true;
            public string timestring = "HH:mm:ss";

            string executable;

            public LogFile()
            {
                //choosing the name of the current executable as logfilename
                executable = System.AppDomain.CurrentDomain.FriendlyName;
                logfile = new FileInfo(executable.Replace(".exe", "") + ".log");
            }

            public LogFile(string path)
            {
                //logfile with chosen path
                logfile = new FileInfo(path);
            }

            private void writeLogStart()
            {
                //creating or overwriting the logfile and writing the head
                using (StreamWriter sw = logfile.CreateText())
                {
                    sw.WriteLine("Started logging at " + DateTime.Now.ToString(@"YYYY/MM/DD_HH:mm:ss") + " with executable " + executable);
                }
            }

            public void log(string data)
            {
                //the user decides whether he wants a the datetimestring or not
                using (StreamWriter sw = logfile.AppendText())
                {
                    if (withTime)
                    {
                        sw.WriteLine(DateTime.Now.ToString(timestring) + ": " + data);
                    }
                    else
                    {
                        sw.WriteLine(data);
                    }
                }
            }

            public void dump(string path)
            {
                //storing the file in the given path
                try
                {
                    logfile.MoveTo(path);
                }
                catch { log("{Gaia.Logging} [!!] Failed moving this file"); }
            }

            public void dump()
            {
                //storing the file in directory Logs
                if (!Directory.Exists("Logs"))
                {
                    Directory.CreateDirectory("Logs");
                }

                int count = 1;
                string filename = "Logs/" + DateTime.Now.ToString("YY-MM-DD");

                //counting in the filename while the file would overwrite an older one
                while (File.Exists(filename + ".log"))
                {
                    filename.Replace("_" + count, "");
                    count++;
                    filename += "_" + count;
                }

                try
                {
                    logfile.MoveTo(filename + ".log");
                }
                catch { log("{Gaia.Logging} [!!] Failed moving this file"); }
            }
        }

        public class XMLDictionaryFile
        {
            private XmlDocument document;
            private FileInfo file;
            const string extension = ".dix";

            public XMLDictionaryFile(string name)
            {
                createFile(name, "Data/");
            }

            public XMLDictionaryFile(string name, string directory)
            {
                createFile(name, directory);
            }

            private void createFile(string name, string directory)
            {
                document = new XmlDocument();

                if(!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (!name.Contains(".dix"))
                {
                    name += extension;
                }

                name = directory+"/" + name;

                file = new FileInfo(name);

                if (file.Exists)
                {
                    document.Load(file.FullName);
                }
                else
                {
                    XmlElement meta = document.CreateElement("meta");
                    meta.SetAttribute("creator", AppDomain.CurrentDomain.FriendlyName);
                    meta.SetAttribute("creationtime", DateTime.Now.ToString());
                }

                save();
            }

            private void save()
            {
                document.Save(file.FullName);
            }

            public void append(string key,string value)
            {
                XmlElement keyElement = document.CreateElement(key);
                keyElement.InnerText = value;
                save();
            }

            public void append(string key, XmlElement value)
            {
                document.CreateElement(key).InnerXml = value.OuterXml;
                save();
            }

            public void append(string key, string value, Dictionary<string,string> attributes)
            {
                var keyElement = document.CreateElement(key);
                keyElement.InnerText = value;

                foreach(var attribute in attributes)
                {
                    keyElement.SetAttribute(attribute.Key,attribute.Value);
                }
                save();
            }
        }

        public class IOAdditions
        {
            public IOAdditions()
            {

            }

            public bool copyDirectory(string sourcepath, string targetpath)
            {
                return copy(new DirectoryInfo(sourcepath), targetpath);
            }

            private bool copy(DirectoryInfo di, string target)
            {

                try
                {
                    target += "/"+di.Name;

                    foreach(var file in di.GetFiles())
                    {
                        file.CopyTo(target + "/" + file.Name);
                    }

                    foreach(var dir in di.GetDirectories())
                    {
                        copy(dir, target);
                    }
                    return true;
                }
                catch { return false; }
            }
        }
    }
}
