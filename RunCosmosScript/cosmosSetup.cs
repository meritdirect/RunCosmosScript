using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace RunCosmosScript
{
    class cosmosSetup
    {
        public static cosmosSetupSection _Config = ConfigurationManager.GetSection("cosmosSetup") as cosmosSetupSection;
        public static FileElementCollection GetFiles()
        {
            return _Config.cosmos;
        }
        public static FileElement GetFiles(string sKey)
        {            

            var sfileElement = _Config.cosmos
        .Cast<FileElement>()
        .FirstOrDefault(_element => _element.name == sKey);
            
         return sfileElement;
            
        
        }

    }
    public class cosmosSetupSection : ConfigurationSection
    {
        [ConfigurationProperty("cosmos")]
        public FileElementCollection cosmos
        {
            get { return (FileElementCollection)this["cosmos"]; }
        }
    }

    [ConfigurationCollection(typeof(FileElement))]
    public class FileElementCollection : ConfigurationElementCollection
    {
        public FileElement this[int index]
        {
            get { return (FileElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new FileElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FileElement)element).name;
        }
    }
    public class FileElement : ConfigurationElement
    {
        public FileElement() { }

        [ConfigurationProperty("name", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("VC", DefaultValue = "", IsRequired = true)]
        public string VC
        {
            get { return (string)this["VC"]; }
            set { this["VC"] = value; }
        }

        [ConfigurationProperty("script", DefaultValue = "", IsRequired = true)]
        public string script
        {
            get { return (string)this["script"]; }
            set { this["script"] = value; }
        }

        [ConfigurationProperty("filePrefix", DefaultValue = "", IsRequired = true)]
        public string filePrefix
        {
            get { return (string)this["filePrefix"]; }
            set { this["filePrefix"] = value; }
        }

        [ConfigurationProperty("streamPath", DefaultValue = "", IsRequired = true)]
        public string streamPath
        {
            get { return (string)this["streamPath"]; }
            set { this["streamPath"] = value; }
        }

        [ConfigurationProperty("downloadDirectory", DefaultValue = "", IsRequired = true)]
        public string downloadDirectory
        {
            get { return (string)this["downloadDirectory"]; }
            set { this["downloadDirectory"] = value; }
        }

        [ConfigurationProperty("ftpScript", DefaultValue = "", IsRequired = true)]
        public string ftpScript
        {
            get { return (string)this["ftpScript"]; }
            set { this["ftpScript"] = value; }
        }

        [ConfigurationProperty("excludeHeader", DefaultValue = "", IsRequired = true)]
        public string excludeHeader
        {
            get { return (string)this["excludeHeader"]; }
            set { this["excludeHeader"] = value; }
        }

        [ConfigurationProperty("renameFile", DefaultValue = "", IsRequired = true)]
        public string renameFile
        {
            get { return (string)this["renameFile"]; }
            set { this["renameFile"] = value; }
        }

        [ConfigurationProperty("updateFile", DefaultValue = "", IsRequired = true)]
        public string updateFile
        {
            get { return (string)this["updateFile"]; }
            set { this["updateFile"] = value; }
        }

    }
}
