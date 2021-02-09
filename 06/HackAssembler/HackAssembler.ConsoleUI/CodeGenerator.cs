using System;
using System.Collections.Generic;
using System.Text;

namespace HackAssembler
{
    public class CodeGenerator
    {
        private readonly SymbolTable _symbolTable;
        private readonly Dictionary<string, string> _destTable;
        private readonly Dictionary<string, string> _compTable;
        private readonly Dictionary<string, string> _jumpTable;
        private int _variableBaseAddress = 16;

        public CodeGenerator(SymbolTable table)
        {
            _symbolTable = table;
            _destTable = new Dictionary<string, string>();
            _compTable = new Dictionary<string, string>();
            _jumpTable = new Dictionary<string, string>();
            PrepareDestTable();
            PrepareCompTable();
            PrepareJumpTable();
        }

        #region Table Preparation

        private void PrepareDestTable()
        {
            _destTable["0"] = "000";
            _destTable["M"] = "001";
            _destTable["D"] = "010";
            _destTable["MD"] = "011";
            _destTable["A"] = "100";
            _destTable["AM"] = "101";
            _destTable["AD"] = "110";
            _destTable["AMD"] = "111";
        }

        private void PrepareCompTable()
        {
            _compTable["0"] = "101010";
            _compTable["1"] = "111111";
            _compTable["-1"] = "111010";
            _compTable["D"] = "001100";
            _compTable["A"] = "110000";
            _compTable["M"] = "110000";
            _compTable["!D"] = "001111";
            _compTable["!A"] = "110001";
            _compTable["!M"] = "110001";
            _compTable["-D"] = "001111";
            _compTable["-A"] = "110011";
            _compTable["-M"] = "110011";
            _compTable["D+1"] = "011111";
            _compTable["A+1"] = "110111";
            _compTable["M+1"] = "110111";
            _compTable["D-1"] = "001110";
            _compTable["A-1"] = "110010";
            _compTable["M-1"] = "110010";
            _compTable["D+A"] = "000010";
            _compTable["D+M"] = "000010";
            _compTable["D-A"] = "010011";
            _compTable["D-M"] = "010011";
            _compTable["A-D"] = "000111";
            _compTable["M-D"] = "000111";
            _compTable["D&A"] = "000000";
            _compTable["D&M"] = "000000";
            _compTable["D|A"] = "010101";
            _compTable["D|M"] = "010101";
        }

        private void PrepareJumpTable()
        {
            _jumpTable["0"] = "000";
            _jumpTable["JGT"] = "001";
            _jumpTable["JEQ"] = "010";
            _jumpTable["JGE"] = "011";
            _jumpTable["JLT"] = "100";
            _jumpTable["JNE"] = "101";
            _jumpTable["JLE"] = "110";
            _jumpTable["JMP"] = "111";
        }

        #endregion

        public string GenerateInstruction(string currentLine)
        {
            var instructionToOutput = string.Empty;
            var isAInstruction = currentLine.Contains('@');
            if (isAInstruction) 
                instructionToOutput = GenerateAInstruction(currentLine);
            else 
                instructionToOutput = GenerateCInstruction(currentLine);
            return instructionToOutput;
        }

        private string GenerateAInstruction(string command)
        {
            var aInstruction = string.Empty;
            var content = command[1..];
            var validAddress = int.TryParse(content, out int address);

            if (_symbolTable.Contains(content) && !validAddress)
            {
                address = _symbolTable.GetValue(content);
            }

            var binaryAddress = Convert.ToString(address, 2);
            var placeholderBits = CreatePlaceHolderBits(binaryAddress);
            return placeholderBits + binaryAddress;
        }

        private string GenerateCInstruction(string currentLine)
        {
            var instruction = "111";
            var aBit = '0';

            var equalIndex = currentLine.IndexOf("=");
            var semicolonIndex = currentLine.IndexOf(";");
            var compFirstIndex = equalIndex + 1;
            var compLastIndex = semicolonIndex > -1 ? semicolonIndex : currentLine.Length;

            var dest = "0";
            var jump = "0";
            var length = compLastIndex - compFirstIndex;
            if (length == 0) length = 1;
            var comp = currentLine.Substring(compFirstIndex, length);

            if (equalIndex > -1) dest = currentLine.Substring(0, equalIndex);
            if (semicolonIndex > -1) jump = currentLine.Substring(semicolonIndex + 1);

            var destBits = _destTable[dest];
            var compBits = _compTable[comp];
            var jumpBits = _jumpTable[jump];

            return instruction + aBit + compBits + destBits + jumpBits;
        }

        private IEnumerable<string> AssignBits(string[] splitInstruction, string originalLine)
        {
            var jumpBits = "000";
            var compBits = "000000";
            var destBits = "000";
            var a = '0';

            if (splitInstruction[1].Contains('J'))
            {
                jumpBits = _jumpTable[splitInstruction[1]];
            }
            else if (splitInstruction.Length > 2)
            {
                jumpBits = _jumpTable[splitInstruction[2]];
                if (splitInstruction[1].Contains('M')) a = '1';
            }
            else
            {
                compBits = _compTable[splitInstruction[1]];
                if (splitInstruction[1].Contains('M')) a = '1';
            }

            if (originalLine.Contains('=')) destBits = _destTable[splitInstruction[0]];
            else if (originalLine.Contains(';')) compBits = _compTable[splitInstruction[0]];

            yield return a.ToString();
            yield return destBits;
            yield return compBits;
            yield return jumpBits;
        }

        private string CreatePlaceHolderBits(string instruction)
        {
            var placeholderBits = new StringBuilder();
            while (instruction.Length + placeholderBits.Length < 16)
            {
                placeholderBits.Append('0');
            }
            return placeholderBits.ToString();
        }
    }
}
