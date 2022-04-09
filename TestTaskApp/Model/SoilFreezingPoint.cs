using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TestTaskApp.Model
{
    public enum ТипГрунта : uint
    {
        [EnumMember(Value = "Пески")]
        Пески,
        [EnumMember(Value = "Супеси и пылеватые пески")]
        СупесиПылеватыеПески,
        [EnumMember(Value = "Суглинок")]
        Суглинок,
        [EnumMember(Value = "Глины")]
        Глины,
        [EnumMember(Value = "Торф слаборазложившийся")]
        ТорфСлаборазложившийся,
        [EnumMember(Value = "Торф среднеразложившийся")]
        ТорфСреднеразложившийся,
    }
    public enum СтепеньЗасоленности : uint
    {
        [EnumMember(Value = "Незасоленный")]
        Незасоленный,
        [EnumMember(Value = "Засоленный (морского типа)")]
        ЗасоленныйМорскогоТипа,
        [EnumMember(Value = "Засоленный (континентального типа)")]
        ЗасоленныйКонтинентальногоТипа,
    }
    
    class SoilFreezingPoint : ICalculatedData<decimal>
    {

        private decimal salinityLevel;
        private decimal icily;
        private decimal soilMoisture;
        private decimal frozenSoilMoisture;
        private ТипГрунта soilType;
        private СтепеньЗасоленности soilSalinity;
        private decimal? soilFreezingPointTemperatur;

        [JsonProperty("Степень засоленности")]
        public decimal SalinityLevel { get => salinityLevel; set => salinityLevel = value; }

        [JsonProperty("Льдистость")]
        public decimal Icily { get => icily; set => icily = value; }

        [JsonProperty("Суммарная влажность мерзлого грунта")]
        public decimal SoilMoisture { get => soilMoisture; set => soilMoisture = value; }
        
        [JsonProperty("Влажность мерзлого грунта, расположенного между ледяными включениями")]
        public decimal FrozenSoilMoisture { get => frozenSoilMoisture; set => frozenSoilMoisture = value; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ТипГрунта SoilType { get => soilType; set => soilType = value; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public СтепеньЗасоленности SoilSalinity { get => soilSalinity; set => soilSalinity = value; }

        [JsonProperty("Температура начала замерзания грунта")]
        public decimal? SoilFreezingPointTemperature { get => soilFreezingPointTemperatur; }
       
        public decimal GetCalculatedDataValue()
        {
            var A = ValueFromSoilType(soilType);
            var B = ValueFromSoilSalinity(soilSalinity);
            var W = icily < 0.4m ? soilMoisture : frozenSoilMoisture;
            var Cps = salinityLevel / (salinityLevel + 100 * W);
            var Tbs = A - B * (53 * Cps + 40 * Cps * Cps);

            soilFreezingPointTemperatur = Tbs;
            return Tbs;
        }

        private decimal ValueFromSoilType(ТипГрунта типГрунта)
        {
            switch (типГрунта)
            {
                case ТипГрунта.Пески:
                    return -0.1m;
                case ТипГрунта.СупесиПылеватыеПески:
                    return -0.15m;
                case ТипГрунта.Суглинок:
                    return -0.20m;
                case ТипГрунта.Глины:
                    return -0.25m;
                case ТипГрунта.ТорфСлаборазложившийся:
                    return GetWeakDecadePeakTemperature();
                case ТипГрунта.ТорфСреднеразложившийся:
                    return getNormalDecadePeakTemperature();
                default:
                    throw new ArgumentException(message: "Invalid enum value", paramName: nameof(типГрунта));
            }
        }
        private decimal ValueFromSoilSalinity (СтепеньЗасоленности cтепеньЗасоленности)
        {
            switch (cтепеньЗасоленности)
            {
                case СтепеньЗасоленности.Незасоленный:
                    return 0m;
                case СтепеньЗасоленности.ЗасоленныйМорскогоТипа:
                    return 1m;
                case СтепеньЗасоленности.ЗасоленныйКонтинентальногоТипа:
                    return 0.85m;
                default:
                    throw new ArgumentException(message: "Invalid enum value", paramName: nameof(cтепеньЗасоленности));
            }
        }

        private decimal GetWeakDecadePeakTemperature()
        {
            decimal[] weakPeakTemperature = new[] { -0.14m, -0.16m, -0.25m, -0.35m };
            decimal[] weakPeakHumidity    = new[] { 7.30m, 5.90m, 3.27m, 1.64m };

            int indexOfClosest = FindClosestValueInArray(this.soilMoisture, weakPeakHumidity);
            return weakPeakTemperature[indexOfClosest];
        }

        private decimal getNormalDecadePeakTemperature()
        {
            decimal[] normalPeakTemperature = new[] { -0.13m, -0.20m };
            decimal[] normalPeakHumidity = new[] { 3.50m, 0.90m };

            int indexOfClosest = FindClosestValueInArray(this.soilMoisture, normalPeakHumidity);
            return normalPeakTemperature[indexOfClosest];
        }

        private static int FindClosestValueInArray(decimal valueToSearch, IEnumerable<decimal> array)
        {
            int indexOfClosest = 0;
            decimal closestDistance = decimal.MaxValue;
            foreach (var (index, value) in array.Select((value, i) => (i, value)))
            {
                if (Math.Abs(value - valueToSearch) < closestDistance)
                {
                    closestDistance = Math.Abs(value - valueToSearch);
                    indexOfClosest = index;
                }
            }
            return indexOfClosest;
        }
    }
}
