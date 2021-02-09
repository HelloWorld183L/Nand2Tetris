using System;
using System.Collections.Generic;
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
        private const string translatedProgramsLoc = "../../../TranslatedAssemblyPrograms/";

        // Args: arg[0] = file name, arg[1] = file name after translation
        static void Main(string[] args)
        {
            _symbolTable = new SymbolTable();
            _fileReader = new StreamReader(sampleProgramsLoc + args[0]);

            var instructions = GenerateInstructions();
            var fileWriter = new StreamWriter(translatedProgramsLoc + args[1]);
            foreach (var instruction in instructions) fileWriter.WriteLine(instruction);
            
            fileWriter.Close();
            Console.WriteLine("Assembly program has been translated!");
        }

        private static List<string> GenerateInstructions()
        {
            var parser = new Parser();
            var generator = new CodeGenerator(_symbolTable);

            var commands = parser.Parse(_symbolTable, _fileReader);
            var instructions = new List<string>();
            foreach (var command in commands)
            {
                var instruction = generator.GenerateInstruction(command);
                instructions.Add(instruction);
            }

            return instructions;
        }
    }
}
