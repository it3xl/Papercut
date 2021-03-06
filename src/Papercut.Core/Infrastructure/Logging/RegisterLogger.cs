﻿// Papercut
// 
// Copyright © 2008 - 2012 Ken Robertson
// Copyright © 2013 - 2017 Jaben Cargman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License. 


namespace Papercut.Core.Infrastructure.Logging
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using Autofac;

    using AutofacSerilogIntegration;

    using Domain.Settings;

    using Papercut.Common.Domain;
    using Papercut.Core.Domain.Application;

    using Serilog;
    using Serilog.Debugging;

    public class RegisterLogger
    {
        public void Register(ContainerBuilder builder)
        {
            builder.Register(
                    c =>
                    {
                        var settingStore = c.Resolve<ISettingStore>();

                        var appMeta = c.Resolve<IAppMeta>();

                        var logPath = settingStore.GetOrSet<string>("LogPath", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"), "Path to store log files.");
                        var port = settingStore.GetOrSet("Port", 25, "SMTP Server listening Port. Default is 25.");

                        string logFilePath = Path.Combine(
                            //AppDomain.CurrentDomain.BaseDirectory,
                            //"Logs",
                            logPath,
                            $"{appMeta.AppName}.port{port}.log");

                        var logConfiguration =
                            new LoggerConfiguration()
#if DEBUG
                                .MinimumLevel.Verbose()
#else
                                .MinimumLevel.Information()
#endif
                                .Enrich.With<EnvironmentEnricher>()
                                .Enrich.FromLogContext()
                                .Enrich.WithProperty("AppName", appMeta.AppName)
                                .Enrich.WithProperty("AppVersion", appMeta.AppVersion)
                                .WriteTo.LiterateConsole()
                                .WriteTo.RollingFile(logFilePath);

                        // publish event so additional sinks, enriches, etc can be added before logger creation is finalized.
                        try
                        {
                            c.Resolve<IMessageBus>().Publish(new ConfigureLoggerEvent(logConfiguration));
                            // it3xl.com: You can check here in logConfiguration._logEventSinks what logging sinks are using.
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Failure Publishing ConfigurationLoggerEvent: " + ex.ToString());
                        }

                        // support self-logging
                        SelfLog.Enable(s =>
                        {
                            // You can comment out the following line to disables error spamming when the Papercut service started without the client.
                            // It is useful when Serilog.Sinks.Seq.SeqSink is implicitly connected by Papercut.Module.Seq.
                            Console.Error.WriteLine(s);
                        });

                        return logConfiguration;
                    })
                .AsSelf()
                .SingleInstance();

            builder.Register(
                    c =>
                    {
                        Log.Logger = c.Resolve<LoggerConfiguration>().CreateLogger();
                        return Log.Logger;
                    })
                .AutoActivate()
                .SingleInstance();

            builder.RegisterLogger();
        }
    }
}