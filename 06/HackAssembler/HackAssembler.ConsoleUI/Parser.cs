using System.IO;

namespace HackAssembler
{
    public class Parser
    {
        public void UpdateSymbolTable(SymbolTable symbolTable, StreamReader fileReader)
        {
            var currentLine = string.Empty;
            for (var instructionNum = 0; currentLine != null;)
            {
                currentLine = fileReader.ReadLine();
                if (string.IsNullOrEmpty(currentLine)) continue;
                currentLine = CommentFilter(currentLine);

                HandleLabels(currentLine, symbolTable, instructionNum);
                HandleVariables(symbolTable, currentLine, instructionNum);
                if (currentLine.Contains('@') || currentLine.Contains(';') || currentLine.Contains('=') )
                {
                    instructionNum++;
                } 
            }
        }

        private void HandleVariables(SymbolTable symbolTable, string currentLine, int instructionNum)
        {
            var variablePresent = currentLine.Contains('@');
            if (variablePresent)
            {
                var variable = currentLine
                               .Trim()
                               .Substring(1);
                var isForwardReference = !symbolTable.Contains(variable);
                if (isForwardReference)
                {
                    symbolTable.Add(variable, instructionNum);
                }
            }
        }

        private void HandleLabels(string currentLine, SymbolTable symbolTable, int instructionNum)
        {
            var openBracketLoc = currentLine.IndexOf('(');
            var closeBracketLoc = currentLine.IndexOf(')');
            var labelPresent = openBracketLoc != -1 &&
                               closeBracketLoc != -1;

            if (labelPresent && currentLine.Length > 0)
            {
                var labelToAdd = currentLine.Substring(openBracketLoc + 1, closeBracketLoc - 1);
                if (!symbolTable.Contains(labelToAdd))
                {
                    symbolTable.Add(labelToAdd, instructionNum);
                }
                else symbolTable.Replace(labelToAdd, instructionNum);
            }
        }

        private string CommentFilter(string currentLine)
        {
            var commentLocation = currentLine.IndexOf("//");
            if (commentLocation > -1)
            {
                return currentLine.Substring(0, commentLocation);
            }
            return currentLine;
        }
    }
}
