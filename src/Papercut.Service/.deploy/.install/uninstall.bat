@ECHO OFF
SET "rootPath=%~dp0.."

CALL "%rootPath%\Papercut.Service.install-config.bat"


ECHO @ stop
CALL "%rootPath%\Papercut.Service.exe"^
 stop^
 --sudo^
 -servicename:%env_servicename%


ECHO @ uninstall
CALL "%rootPath%\Papercut.Service.exe"^
 uninstall^
 --delayed^
 --sudo^
 -servicename:%env_servicename%^
