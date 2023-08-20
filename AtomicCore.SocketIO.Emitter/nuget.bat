echo off

::指定起始文件夹
set pro_dir=%~dp0
set release=bin\Release
cd %~dp0%release%
pause

:: 参数 /R 表示需要遍历子文件夹,去掉表示不遍历子文件夹
:: %%f 是一个变量,类似于迭代器,但是这个变量只能由一个字母组成,前面带上%%
:: 括号中是通配符,可以指定后缀名,*.*表示所有文件
for %%f in (*.nupkg) do ( 
	cmd start /k "dotnet nuget push %%f --api-key %Atomic-Master-2.0-PROD% --source https://api.nuget.org/v3/index.json"
)
pause