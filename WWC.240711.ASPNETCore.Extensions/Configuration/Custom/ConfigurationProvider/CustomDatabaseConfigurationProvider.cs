using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Extensions.Configuration.Custom.DB;
using WWC._240711.ASPNETCore.Extensions.Configuration.Custom.DB.DTO;
using WWC._240711.ASPNETCore.Extensions.Configuration.Custom.DB.Entity;

namespace WWC._240711.ASPNETCore.Extensions.Configuration.Custom
{
    public class CustomDatabaseConfigurationProvider : IConfigurationProvider
    {
        private readonly ConfigDbContext _configDbContext;
        private readonly IConfigurationBuilder _builder;
        private readonly Dictionary<string, string> configurations = new Dictionary<string, string>();
        private List<ConfigurationInfoDTO> TreeData = new List<ConfigurationInfoDTO>();

        public CustomDatabaseConfigurationProvider(/*IConfigurationBuilder builder*/)
        {
            //_builder = builder;
            _configDbContext = new ConfigDbContext();
        }

        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string? parentPath)
        {
            throw new NotImplementedException();
        }

        public IChangeToken GetReloadToken()
        {
            throw new NotImplementedException("暂不支持数据刷新");
        }

        public async void Load()
        {
            var configurationData = _configDbContext.ConfigurationInfos.ToList();
            TreeData = BuildTree(configurationData);
            DataBuildDictionary(TreeData);

        }
        private void DataBuildDictionary(List<ConfigurationInfoDTO> Data)
        {
            foreach (var item in Data)
            {
                //if (item.ConfigurationInfo == null)
                //{
                //    configurations.Add(item.Key, item.Value);
                //}
                //else if (item.ConfigurationInfo as ConfigurationInfoDTO == null)
                //{
                //    configurations.Add(item.Key, item.Value);
                //}
                //else if (item.ConfigurationInfo as List<ConfigurationInfoDTO> != null)
                //{
                //    if (((List<ConfigurationInfoDTO>)item.ConfigurationInfo).Count <= 0)
                //    {
                //        configurations.Add(item.Key, item.Value);
                //    }
                //}
                //else
                //{
                if ((item.ConfigurationInfo as List<ConfigurationInfoDTO>) != null)
                {
                    if (((List<ConfigurationInfoDTO>)item.ConfigurationInfo).Count > 0)
                    {
                        DataBuildDictionary((List<ConfigurationInfoDTO>)item.ConfigurationInfo);
                    }
                }
                if (item.ConfigurationInfo as ConfigurationInfoDTO != null)
                {
                    DataBuildObjectDictionary((ConfigurationInfoDTO)item.ConfigurationInfo);
                }
                //}
                configurations.Add(item.Key, item.Value);
            }
        }

        private void DataBuildObjectDictionary(ConfigurationInfoDTO Data)
        {
            //if (Data.ConfigurationInfo == null)
            //{
            //    configurations.Add(Data.Key, Data.Value);
            //}
            //else if (Data.ConfigurationInfo as ConfigurationInfoDTO == null)
            //{
            //    configurations.Add(Data.Key, Data.Value);
            //}
            //else if (Data.ConfigurationInfo as List<ConfigurationInfoDTO> != null)
            //{
            //    if (((List<ConfigurationInfoDTO>)Data.ConfigurationInfo).Count <= 0)
            //    {
            //        configurations.Add(Data.Key, Data.Value);
            //    }
            //}
            //else
            //{
            if (Data.ConfigurationInfo as List<ConfigurationInfoDTO> != null)
            {
                if (((List<ConfigurationInfoDTO>)Data.ConfigurationInfo).Count > 0)
                {
                    DataBuildDictionary((List<ConfigurationInfoDTO>)Data.ConfigurationInfo);
                }
            }
            if (Data.ConfigurationInfo as ConfigurationInfoDTO != null)
            {
                DataBuildObjectDictionary((ConfigurationInfoDTO)Data.ConfigurationInfo);
            }
            //}

            configurations.Add(Data.Key, Data.Value);
        }


        private List<ConfigurationInfoDTO> BuildTree(List<ConfigurationInfo> configurationInfos)
        {
            var lookup = configurationInfos.ToDictionary(c => c.Key);
            var result = new List<ConfigurationInfoDTO>();

            // 寻找所有根节点（ParentID 为空或不存在的节点）
            foreach (var config in configurationInfos)
            {
                if (string.IsNullOrEmpty(config.ParentID) || !lookup.ContainsKey(config.ParentID))
                {
                    var root = ToDTO(config, config.Key);
                    result.Add(root);
                    AddChildren(root, lookup, root.Key);
                }
            }

            return result;
        }

        private void AddChildren(ConfigurationInfoDTO node, Dictionary<string, ConfigurationInfo> lookup, string parentKey)
        {
            string readKey = node.Key.Split(":").LastOrDefault();
            var children = lookup.Values.Where(c => c.ParentID == readKey).ToList();
            if (children.Count == 1)
            {
                foreach (var child in children)
                {
                    var childKey = string.IsNullOrEmpty(parentKey) ? child.Key : $"{parentKey}:{child.Key}";
                    var childNode = ToDTO(child, childKey);
                    node.ConfigurationInfo = childNode;
                    AddChildren(childNode, lookup, childKey);
                }
            }
            else
            {
                List<ConfigurationInfoDTO> childList = new List<ConfigurationInfoDTO>();
                foreach (var child in children)
                {
                    var childKey = string.IsNullOrEmpty(parentKey) ? child.Key : $"{parentKey}:{child.Key}";
                    var childNode = ToDTO(child, childKey);
                    childList.Add(childNode);
                    AddChildren(childNode, lookup, childKey);
                }
                node.ConfigurationInfo = childList;
            }
        }

        private ConfigurationInfoDTO ToDTO(ConfigurationInfo config, string key)
        {
            return new ConfigurationInfoDTO
            {
                Key = key,
                Value = config.Value,
                ParentID = config.ParentID
            };
        }

        public async void Set(string key, string? value)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                throw new Exception("key 或者 value 不能为空");
            }

            if (_configDbContext.ConfigurationInfos.Any(p => p.Key == key))
            {
                throw new Exception($"不允许添加重复的键值 【{key}】");
            }
            await _configDbContext.ConfigurationInfos.AddAsync(new ConfigurationInfo()
            {
                ConfigKey = Guid.NewGuid(),
                Key = key,
                ParentID = null,
                Value = value
            });
            await _configDbContext.SaveChangesAsync();

        }

        public bool TryGet(string key, out string? value)
        {
            bool result = false;


            var keys = key.Split(':');
            var objectData = FindValueByKeyPath(TreeData, keys, 0);
            value = JsonConvert.SerializeObject(objectData);

            if (result == null)
            {
                value = null;
            }

            return result;
        }

        private object FindValueByKeyPath(List<ConfigurationInfoDTO> nodes, string[] keys, int index)
        {
            foreach (var node in nodes)
            {
                if (node.Key == keys[index])
                {
                    if (index == keys.Length - 1)
                    {
                        return new
                        {
                            Key = node.Key,
                            Value = node.Value
                        };
                    }
                    else
                    {
                        var child = new List<ConfigurationInfoDTO>();

                        if (node.ConfigurationInfo as List<ConfigurationInfoDTO> != null)
                        {
                            child = (List<ConfigurationInfoDTO>)node.ConfigurationInfo;
                        }
                        if (node.ConfigurationInfo as ConfigurationInfoDTO != null)
                        {
                            child.Add((ConfigurationInfoDTO)node.ConfigurationInfo);
                        }

                        var childResult = FindValueByKeyPath(child, keys, index + 1);
                        if (childResult is string)
                        {
                            return new
                            {
                                Key = node.Key,
                                Value = new List<object> { childResult }
                            };
                        }
                        else
                        {
                            return new
                            {
                                Key = node.Key,
                                Value = new List<object> { childResult }
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}
