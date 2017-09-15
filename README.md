# ProcessMemory
Little shitty library i made that can do the following: Read, Write, Query memory regions and AOB Scan Processes 64Bit and 32Bit.

Example for those who need it i guess

```csharp
ProcessStream stream = new ProcessStream(Process.GetProcessesByName("NAME_OF_PROCESS").First());
//Each byte is separated by a space, and each ?? indicates an wildcard
HexPattern p = new HexPattern("?? ?? 0x32 ?? 0x43");
long address = stream.PatternScan(p);
```
