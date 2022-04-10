using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TestTaskApp.Model;

namespace TestTaskApp.ViewModel
{
    public class ComparisonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value?.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value?.Equals(true) == true ? parameter : Binding.DoNothing;
        }
    }

    public enum ТипГрунта : uint
    {
        Пески = 0,
        СупесиПылеватыеПески = 1,
        Суглинок = 2,
        Глины = 3,
        ТорфСлаборазложившийся = 4,
        ТорфСреднеразложившийся = 5,
    }
    public enum СтепеньЗасоленности : uint
    {
        Незасоленный = 0,
        ЗасоленныйМорскогоТипа = 1,
        ЗасоленныйКонтинентальногоТипа = 2
    }
   
    public sealed class MainVM : ViewModelBase, INotifyPropertyChanged
    {
        

        private SoilFreezingPoint soilFreezingPointModel;
        internal SoilFreezingPoint SoilFreezingPointModel
        {
            get
            {
                return soilFreezingPointModel;
            }
            set
            {
                soilFreezingPointModel = value;
                foreach (var propertyInfo in soilFreezingPointModel.GetType().GetProperties())
                {
                    OnPropertyChanged(propertyInfo.Name);
                }
            }
        }

        public ТипГрунта SoilType
        {
            get => (ТипГрунта)SoilFreezingPointModel.SoilType;
            set
            {
                SoilFreezingPointModel.SoilType = (Model.ТипГрунта)value;
                OnPropertyChanged();
            }
        }

        public СтепеньЗасоленности SoilSalinity
        {
            get => (СтепеньЗасоленности)SoilFreezingPointModel.SoilSalinity;
            set
            {
                SoilFreezingPointModel.SoilSalinity = (Model.СтепеньЗасоленности)value;
                OnPropertyChanged();
            }
        }

        public decimal SalinityLevel
        { 
            get => SoilFreezingPointModel.SalinityLevel; 
            set
            {
                SoilFreezingPointModel.SalinityLevel = value;
                OnPropertyChanged();
            }
        }
        public decimal Icily
        { 
            get => SoilFreezingPointModel.Icily; 
            set
            {
                SoilFreezingPointModel.Icily = value;
                OnPropertyChanged();
            }
        }

        public decimal SoilMoisture
        {
            get => SoilFreezingPointModel.SoilMoisture;
            set
            {
                SoilFreezingPointModel.SoilMoisture = value;
                OnPropertyChanged();
            }
        }

        public decimal FrozenSoilMoisture
        {
            get => SoilFreezingPointModel.FrozenSoilMoisture;
            set
            {
                SoilFreezingPointModel.FrozenSoilMoisture = value;
                OnPropertyChanged();
            }
        }

        public decimal? SoilFreezingPointTemperature
        {
            get => SoilFreezingPointModel.SoilFreezingPointTemperature;
        }

        public ICommand SaveJsonData { get; }
        public void SaveJsonDataClick(object parameter)
        {
            new JsonDataProvider<SoilFreezingPoint>().Write(soilFreezingPointModel);
        }

        public ICommand SavePivotTableJsonData { get; }
        public void SavePivotTableJsonDataClick(object parameter)
        {
            new JsonDataProvider<List<ICalculatedData<decimal>>>().Write(Collection.ToList());
        }

        public ICommand LoadJsonData { get; }
        public void LoadJsonDataClick(object parameter)
        {
            bool result;
            var loadedValue = new JsonDataProvider<SoilFreezingPoint>().Read(out result);
            if (result)
            {
                SoilFreezingPointModel = loadedValue;
            }
        }

        public ICommand ClearData { get; }
        public void ClearDataClick(object parameter)
        {
            SoilFreezingPointModel = new SoilFreezingPoint();
        }

        public ICommand SetDataFromHistory { get; }
        public void HistoryClick(object parameter)
        {
            if(parameter is SoilFreezingPoint)
            {
                SoilFreezingPointModel = (SoilFreezingPoint)parameter;
            }
            
        }

        public ICommand CalculateData { get; }
        public ObservableCollection<ICalculatedData<decimal>> Collection 
        { 
            get => collection;
            set => collection = value;
        }

        public void CalculateDataClick(object parameter)
        {
            try
            {
                SoilFreezingPointModel.GetCalculatedDataValue();
            }
            catch(DivideByZeroException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            
            OnPropertyChanged(nameof(SoilFreezingPointModel.SoilFreezingPointTemperature));
            Collection.Insert(0, SoilFreezingPointModel);
            SoilFreezingPointModel = SoilFreezingPointModel.Clone() as SoilFreezingPoint;
        }

        ObservableCollection<ICalculatedData<decimal>> collection;

        public MainVM()
        {
            soilFreezingPointModel = new SoilFreezingPoint()
            {
                Icily = 1m,
                SalinityLevel = 1m,
                SoilMoisture = 1m,
                FrozenSoilMoisture = 1m,

            };

            SaveJsonData = new MainVMCommand(SaveJsonDataClick);
            LoadJsonData = new MainVMCommand(LoadJsonDataClick);
            ClearData = new MainVMCommand(ClearDataClick);
            CalculateData = new MainVMCommand(CalculateDataClick);
            SetDataFromHistory = new MainVMCommand(HistoryClick);
            SavePivotTableJsonData = new MainVMCommand(SavePivotTableJsonDataClick);

            Collection = new ObservableCollection<ICalculatedData<decimal>>();
            collection.CollectionChanged += Collection_CollectionChanged;
            


        }

        private void Collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Collection");
        }
    }
}
