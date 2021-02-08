using System.Collections.Generic;
using System.Linq;

namespace HackAssembler
{
    public class SymbolTable
    {
		private readonly Dictionary<string, int> _table;

        public SymbolTable()
        {
			_table = new Dictionary<string, int>();

			for (int i = 0; i <= 15; i++)
			{
				_table["R" + i] = i;
			}
			_table["SCREEN"] = 16384;
			_table["KBD"] = 24576;
			_table["SP"] = 0;
			_table["LCL"] = 1;
			_table["ARG"] = 2;
			_table["THIS"] = 3;
			_table["THAT"] = 4;
		}

		public void Add(string symbol, int memoryAddress) => _table.Add(symbol, memoryAddress);
		public bool Contains(string symbol) => _table.ContainsKey(symbol);
		public string GetKey(string symbol) => _table.Keys.FirstOrDefault(x => x == symbol);
		public int GetValue(string symbol) => _table[symbol];
		public void Replace(string symbol, int newMemoryAddress)
        {
			_table.Remove(symbol);
            _table.Add(symbol, newMemoryAddress);
        }
    }
}
