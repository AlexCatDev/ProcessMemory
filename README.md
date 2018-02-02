# ProcessMemory
Read, Write, Query Process Memory Regions And AOB Scan Processes 64Bit & 32Bit

Example:

```csharp
ProcessStream stream = new ProcessStream(Process.GetProcessesByName("NAME_OF_PROCESS").First());
//Each byte is separated by a space, and each ?? indicates an wildcard
HexPattern p = new HexPattern("A1 ?? ?? ?? ?? FF 08");
long address = stream.PatternScan(p);
```
