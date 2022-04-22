using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestTaskApp.Model
{
    class JsonDataProvider<T> : IDataProvider<T>
    {
        
        public T Read(out bool result, string[] args = null) 
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            List<string> errors = new List<string>();

            if (openFileDialog.ShowDialog() == true)
            {
                var fileContext = File.ReadAllText(openFileDialog.FileName);
                
                var jsonSerializerSettings = new JsonSerializerSettings
                {
                    Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
                    {
                        errors.Add(errorArgs.ErrorContext.Error.Message);
                        errorArgs.ErrorContext.Handled = true;
                    }
                };
                T data = JsonConvert.DeserializeObject<T>(fileContext, jsonSerializerSettings);
                if (!errors.Any())
                {
                    result = true;
                    return data;
                }
                
            }
            result = false;
            return default;

        }

        public void Write(T data, string[] args = null)
        {
            List<string> errors = new List<string>();
            var saveFileDialog = new SaveFileDialog();
            
            saveFileDialog.FileName = $"{nameof(data)}JsonData";
            saveFileDialog.Filter = "JSON files (*.json)|*.json";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == true)
            {
                var jsonSerializerSettings = new JsonSerializerSettings
                {
                    Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
                    {
                        errors.Add(errorArgs.ErrorContext.Error.Message);
                        errorArgs.ErrorContext.Handled = true;
                    }
                };

                string jsonData = JsonConvert.SerializeObject(data);
                File.WriteAllText(saveFileDialog.FileName, jsonData);

                if (errors.Any())
                {
                    throw new FileFormatException(String.Join("/r/n", errors));
                }
            }
        }    
    }
}
