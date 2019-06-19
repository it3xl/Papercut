## My edition of [Papercut](https://github.com/ChangemakerStudios/Papercut) here is everything my development teams need to forget about Microsoft Exchange on your development and test environments.
* See all sent messages as .eml files stored dynamically in sub-folders.
* Open them in Microsoft Outlook as any saved earlier email messages.

### This is a modified version of Papercut project
![Papercut Logo](https://raw.githubusercontent.com/ChangemakerStudios/Papercut/develop/graphics/PapercutLogo.png)
The Simple SMTP Desktop Email Receiver

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

**Deployment**

* Copy all content of the `\src\Papercut.Service\.deploy\` folder to desired installation location.
* Change parameters in the `Papercut.Service.json` and in `Papercut.Service.install-config.bat` files if you wish.

**Running under Local System account**

* In Windows Explorer find the `.install\install.localsystem.bat` file and from its Context Menu select `Run as administrator`.

**Running under Network Service account**

* Open file `Papercut.Service.json` and find all path parameters (like MessagePath and LogPath).<br/>
You should give the NETWORK SERVICE local account "Full control" access rights on all this paths.
* In Windows Explorer find the `.install\install.networkservice.bat` file and from its Context Menu select `Run as administrator`.

**Updating configuration parameters**

If you will change any configurating parameters then simply repeat the **Running** operations described above.<br/>
Or restart the service named like "Papercut..." from the Windows Services console.

**Uninstalling**

Use `.install\uninstall.bat` to uninstall (unregister) the service.

**Other options**

See another possible installation options
* by entering: `Papercut.Service.exe help`
* Or see [Topshelf CLI](https://topshelf.readthedocs.io/en/latest/overview/commandline.html)

### How to Run Papercut.Service as a Console Application

In Windows Explorer find the `Papercut.Service.exe` file  
`\src\Papercut.Service\.deploy\Papercut.Service.exe`  
and from the Context Menu select "Run as administrator".
