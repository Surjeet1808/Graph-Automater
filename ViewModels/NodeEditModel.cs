using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GraphSimulator.ViewModels
{
    // Lightweight staging model for node property editing
    public class NodeEditModel : ObservableObject
    {
        private string name = "";
        public string Name { get => name; set => SetProperty(ref name, value); }

        private string type = "Process";
        public string Type { get => type; set => SetProperty(ref type, value); }

        private string color = "#007ACC";
        public string Color { get => color; set => SetProperty(ref color, value); }

        private string jsonData = "{}";
        public string JsonData { get => jsonData; set => SetProperty(ref jsonData, value); }

        private double width = 120;
        public double Width { get => width; set => SetProperty(ref width, value); }

        private double height = 80;
        public double Height { get => height; set => SetProperty(ref height, value); }
    }
}
