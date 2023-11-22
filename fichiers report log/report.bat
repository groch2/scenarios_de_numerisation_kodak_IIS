@echo off
rem Make sure changes to environment are not global
setlocal

rem Get a list of all the files in the installation folder
dir ..\ /S > file_list.txt

rem Check / define JAVA_HOME
if not "%JAVA_HOME%" == "" goto okJava
pushd ..\java
set "JAVA_HOME=%cd%"
popd
:okJava

rem Make sure ANT_HOME points to our own Ant
pushd ..\apache-ant
set "ANT_HOME=%cd%"
popd

rem Unset any CLASSPATH variable
set CLASSPATH=

rem Invoke Ant
call ..\apache-ant\bin\ant.bat -f report.xml -q %*

set EL=%ERRORLEVEL%

@exit /b %EL%
