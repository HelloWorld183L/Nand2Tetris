using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace HackAssembler
{
    class Program
    {
        private static StreamReader _fileReader;
        private static SymbolTable _symbolTable;
        private const string sampleProgramsLoc = "../../../SampleAssemblyPrograms/";
        private const string translatedProgramsLoc = "";
        private const int variableBaseAddress = 16;

        // Args: arg[0] = file.asm import location, arg[1] = file.hack export location
        static void Main(string[] args)
        {
            _symbolTable = new SymbolTable();
            _fileReader = new StreamReader(sampleProgramsLoc + args[0]);
            Console.WriteLine("Entering the first pass phase...");
            FirstPass();

            Console.WriteLine("Entering the second pass phase...");
            _fileReader = new StreamReader(sampleProgramsLoc + args[0]);
            var hackFileContents = SecondPass();
            _fileReader.Close();

            Console.WriteLine("Writing contents to " + args[1]);
            var fileWriter = new StreamWriter(translatedProgramsLoc + args[1]);
            fileWriter.Write(hackFileContents);
            fileWriter.Close();
            Console.WriteLine("Assembly program has been translated!");
        }

        private static bool ValidateArgs(string[] args)
        {
            var argsNotEmpty = !(string.IsNullOrEmpty(args[0]) && string.IsNullOrEmpty(args[1]));
            var argsValid = argsNotEmpty && File.Exists(args[0]) 
                            && args[0].Contains(".asm");

            return argsValid;
        }

        private static void FirstPass()
        {
            var parser = new Parser();
            parser.UpdateSymbolTable(_symbolTable, _fileReader);
        }

        private static StringBuilder SecondPass()
        {
            var hackFileContents = new StringBuilder();
            var translator = new Translator(_symbolTable);
            for (var currentLine = _fileReader.ReadLine(); currentLine != null; )
            {
                var commentLocation = currentLine.IndexOf("//");
                if (commentLocation > -1) currentLine = currentLine.Substring(0, commentLocation);
                if (currentLine.Length == 0)
                {
                    currentLine = _fileReader.ReadLine();
                    continue;
                }
                currentLine = currentLine.Trim();

                var instructionToOutput = translator.TranslateIntoInstruction(currentLine);
                hackFileContents.AppendLine(instructionToOutput);
                currentLine = _fileReader.ReadLine();
            }
            return hackFileContents;
        }
    }
}
