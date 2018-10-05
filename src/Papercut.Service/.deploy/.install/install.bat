@ECHO OFF
SET "managerPath=%~dp0."
SET "rootPath=%~dp0.."

CALL "%rootPath%\Papercut.Service.install-config.bat"


CALL "%managerPath%\uninstall.bat"


ECHO @ install
CALL "%rootPath%\Papercut.Service.exe"^
 install^
 --delayed^
 --sudo^
 --localsystem^
 -servicename:%env_servicename%^


ECHO @ start
CALL "%rootPath%\Papercut.Service.exe"  start  -servicename:%env_servicename%  --sudo
