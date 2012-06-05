echo off
net stop "NagiosNetClient"
%windir%\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u /LogToConsole=true NagiosNetClient.exe
echo on
