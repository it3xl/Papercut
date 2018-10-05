### How to Install Papercut.Service as a Windows Service

* Copy all content of the `\src\Papercut.Service\.deploy\` folder to desired installation location.
* Change parameters in the `Papercut.Service.json` and in `Papercut.Service.install-config.bat` files if you wish.
* In Windows Explorer find the `.install\install.bat` file and from the Context Menu select `Run as administrator`.

If you will change the parameters later you should update the servise by calling `.install\install.bat` through the "Run as administrator".  
Or just restart this sevice named "Papercut..." from the Windows Services console.

Use `.install\uninstall.bat` to uninstall (unregister) the service.

See another possible installation options
* by entering: `Papercut.Service.exe help`
* Or see [Topshelf CLI](https://topshelf.readthedocs.io/en/latest/overview/commandline.html)

### How to Run Papercut.Service as a Console Application

In Windows Explorer find the `Papercut.Service.exe` file  
`\src\Papercut.Service\.deploy\Papercut.Service.exe`  
and from the Context Menu select "Run as administrator".

### Last Version

See [it3xl/Papercut](https://github.com/it3xl/Papercut)



