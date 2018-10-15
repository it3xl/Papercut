## This [Papercut](https://github.com/ChangemakerStudios/Papercut) edition is everything your team need to forget about Microsoft Exchange on your development and test environments.
* See all sent messages as .eml files stored dynamically in sub-folders.
* Open them in Microsoft Outlook as any saved earlier email messages.

### This is a modified version of 
![Papercut Logo](https://raw.githubusercontent.com/ChangemakerStudios/Papercut/develop/graphics/PapercutLogo.png)
The Simple SMTP Desktop Email Receiver

[![Build status](https://ci.appveyor.com/api/projects/status/bs2asxoafdwbkcxa?svg=true)](https://ci.appveyor.com/project/Jaben/papercut)
[![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Jaben/Papercut?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)
[![Say Thanks!](https://img.shields.io/badge/Say%20Thanks-!-1EAEDB.svg)](https://saythanks.io/to/Jaben)

## Only service usage
This version of Papercut is used only as a Windows service or as many Windows services on the same machine.

## Features
* Saving of email massages to a configured folder. You can make this folder a shared folder for your team.
* Messages will be stored in sub-folders dynamically.
  see [MessageRepository.FileSystem.cs](https://github.com/it3xl/Papercut/blob/service-only-usage/src/Papercut.Message/MessageRepository.FileSystem.cs) > CreateFolder method to change the sub-folder logic.
* Message file name may have different values in a different order.
  see [MessageRepository.FileSystem.cs](https://github.com/it3xl/Papercut/blob/service-only-usage/src/Papercut.Message/MessageRepository.FileSystem.cs) > CreateUniqueFile method to change the file name generating logic.
* You can omit date-time stamp in the message file name (FileInfo.CreationTime will be used).
* Added configuring of a log files root folder.
* Changed log file name format to Papercut.Service.SMTPport-YYYYMMdd.log.

## How To Use

### How to Install Papercut.Service as a Windows Service

* Copy all content of the `\src\Papercut.Service\.deploy\` folder to desired installation location.
* Change parameters in the `Papercut.Service.json` and in `Papercut.Service.install-config.bat` files if you wish.
* In Windows Explorer find the `.install\install.bat` file and from the Context Menu select `Run as administrator`.

If you will change the parameters later will have to update the service by calling `.install\install.bat` through the "Run as administrator".  
Or just restart this service named "Papercut..." from the Windows Services console.

Use `.install\uninstall.bat` to uninstall (unregister) the service.

See another possible installation options
* by entering: `Papercut.Service.exe help`
* Or see [Topshelf CLI](https://topshelf.readthedocs.io/en/latest/overview/commandline.html)

### How to Run Papercut.Service as a Console Application

In Windows Explorer find the `Papercut.Service.exe` file  
`\src\Papercut.Service\.deploy\Papercut.Service.exe`  
and from the Context Menu select "Run as administrator".
