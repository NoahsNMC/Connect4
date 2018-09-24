using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace CodingActivity_TicTacToe_ConsoleGame
{
    public class JsonServices
    {
        public const string dataFilePathJson = "Data\\";
        public const string defaultFileName = "game.json";
        string _dataFilePath;

        public JsonServices(string dataFilePath)
        {
            _dataFilePath = dataFilePath;
        }

        static public void WriteJsonFile(object _object, string fileName = defaultFileName)
        {
            StreamWriter sWriter;

            try
            {
                if (!Directory.Exists(dataFilePathJson))
                    Directory.CreateDirectory(dataFilePathJson);

                using (sWriter = new StreamWriter(dataFilePathJson + fileName))
                    sWriter.Write(JsonConvert.SerializeObject(_object, Formatting.Indented));
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine("You need more permissions to run the application. Try running it as an admin.");
                Console.WriteLine(ex);
                throw;
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine("You need more permissions to run the application. Try running it as an admin.");
                Console.WriteLine(ex);
                throw;
            }
            catch (PathTooLongException ex)
            {
                Console.WriteLine("The path is too long.");
                Console.WriteLine(ex);
                throw;
            }
        }

        static public object ReadJsonFile(string fileName = defaultFileName)
        {
            object _output;

            try
            {
                using (StreamReader sReader = new StreamReader(dataFilePathJson + fileName))
                    if(fileName == "game.json")
                        _output = JsonConvert.DeserializeObject<Gameboard>(sReader.ReadToEnd());
                    else
                        _output = JsonConvert.DeserializeObject<List<Scoreboard>>(sReader.ReadToEnd());
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("No scores found!");
                Console.WriteLine(ex);
                throw;
            }

            return _output;
        }
    }
}
