using System.Collections.Generic;
using System.IO;

namespace HackAssembler
{
    public class Parser
    {
        private int variableAddress = 16;

        public IEnumerable<string> Parse(SymbolTable symbolTable, StreamReader fileReader)
        {
            var filteredCommands = FilterCode(fileReader);
            AddLabels(filteredCommands, symbolTable);

            var i = 0;
            foreach (var command in filteredCommands)
            {
                if (command.StartsWith("@"))
                {
                    var instructionA = command[1..];
                    var validInstruction = int.TryParse(instructionA, out int _);

                    if (!symbolTable.Contains(instructionA) && validInstruction)
                    {
                        symbolTable.Add(instructionA, variableAddress);
                        variableAddress++;
                    }
                }
                i++;
            }
            filteredCommands.RemoveAll(command => command.StartsWith('('));
            return filteredCommands;
        }

        private void AddLabels(IList<string> commands, SymbolTable symbolTable)
        {
            var removedLabels = 0;
            var i = 0;
            foreach (var command in commands)
            {
                if (command.StartsWith('(') && command.EndsWith(')'))
                {
                    var label = command.Substring(1, command.IndexOf(')'));
                    symbolTable.Add(label, i - removedLabels);
                    removedLabels++;
                }
                i++;
            }
        }

        private List<string> FilterCode(StreamReader streamReader)
        {
            var commandsAndLabels = new List<string>();
            var currentLine = "";

            while ((currentLine = streamReader.ReadLine()) != null)
            {
                currentLine = currentLine.
                              Replace(" ", "")
                              .Replace("\\t", "");
                var commentIndex = currentLine.IndexOf("//");
                if (commentIndex > -1)
                {
                    currentLine = currentLine.Substring(0, commentIndex);
                }
                if (currentLine != string.Empty)
                {
                    commandsAndLabels.Add(currentLine);
                }
            }
            return commandsAndLabels;
        }
    }
}
