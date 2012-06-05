echo off
%windir%\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /LogToConsole=true NagiosNetClient.exe
net start "NagiosNetClient"
echo on
