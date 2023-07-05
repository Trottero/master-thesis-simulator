using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Simulator.Configuration.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Simulator.Configuration.Systems
{
    public partial class JsonReaderSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<SimulationFrameworkConfigurationComponent>();
            RequireForUpdate<BoidsConfigurationComponent>();
            RequireForUpdate<EnergyConfigurationComponent>();
            RequireForUpdate<ReproductionConfigurationComponent>();
            RequireForUpdate<SchoolConfigurationComponent>();
            RequireForUpdate<FoodSourcesConfigurationComponent>();
        }
        
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            
            var actions = new List<Action<GlobalConfigurationComponent>>
            {
                a => SetConfigurationInComponent(a.SimulationFrameworkConfiguration),
                a => SetConfigurationInComponent(a.BoidsConfiguration),
                a => SetConfigurationInComponent(a.EnergyConfiguration),
                a => SetConfigurationInComponent(a.ReproductionConfiguration),
                a => SetConfigurationInComponent(a.SchoolConfiguration),
                a => SetConfigurationInComponent(a.FoodSourcesConfiguration)
            };
            
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            var configurationEntity = ecb.CreateEntity();
            
            // Check the arguments if a JSON file is specified
            var args = Environment.GetCommandLineArgs();
            if (!args.Contains("-config"))
            {
                ecb.AddComponent(configurationEntity, new GlobalConfigurationComponent
                {
                    SimulationFrameworkConfiguration = SystemAPI.GetSingleton<SimulationFrameworkConfigurationComponent>(),
                    BoidsConfiguration = SystemAPI.GetSingleton<BoidsConfigurationComponent>(),
                    EnergyConfiguration = SystemAPI.GetSingleton<EnergyConfigurationComponent>(),
                    ReproductionConfiguration = SystemAPI.GetSingleton<ReproductionConfigurationComponent>(),
                    SchoolConfiguration = SystemAPI.GetSingleton<SchoolConfigurationComponent>(),
                    FoodSourcesConfiguration = SystemAPI.GetSingleton<FoodSourcesConfigurationComponent>()
                });
                ecb.Playback(EntityManager);
                ecb.Dispose();
                return;
            }

            // Get index
            var index = args.ToList().IndexOf("-config");
            if (index == -1)
            {
                throw new Exception("Invalid command line arguments -config was passed but no file was specified.");
            }
            // Get the path
            var pathArg = args[index + 1];
            var path = UriUtility.Combine(Directory.GetCurrentDirectory(), pathArg);
            // Read the JSON file
            if (!File.Exists(path))
            {
                throw new Exception("Invalid command line arguments -config was passed but the file does not exist.");
            }
                
            var json = File.ReadAllText(path);
            var configurationObject = JsonUtility.FromJson<GlobalConfigurationComponent>(json);
            foreach (var action in actions)
            {
                action(configurationObject);
            }
            
            ecb.AddComponent(configurationEntity, configurationObject);
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        private void SetConfigurationInComponent<T>(T configuration) where T: unmanaged, IComponentData
        {
            var singleton = SystemAPI.GetSingletonRW<T>();
            singleton.ValueRW = configuration;
        }

        protected override void OnUpdate()
        {
            
        }
    }
    
    public static class UriUtility
    {
        public static string Combine(string basePath, string relativePath)
        {
            var baseUri = new Uri(ExpandUri(basePath), UriKind.Absolute);
            var relativeUri = new Uri(relativePath, UriKind.RelativeOrAbsolute);

            var result = baseUri.IsAbsoluteUri ? baseUri.GetLeftPart(UriPartial.Query) : baseUri.ToString();
            result += result.EndsWith("/") ? "" : "/";
            result += relativeUri.IsAbsoluteUri ? relativeUri.PathAndQuery : relativeUri.ToString();

            return ExpandUri(result);
        }
        private static string ExpandUri(string path)
        {
            var uri = new Uri(path, UriKind.RelativeOrAbsolute);

            if (uri.IsAbsoluteUri)
            {
                if (uri.IsFile) return uri.LocalPath;
                return uri.AbsoluteUri;
            }

            if (File.Exists(path) || Directory.Exists(path)) return Path.GetFullPath(path);

            return path;
        }

    }
}
