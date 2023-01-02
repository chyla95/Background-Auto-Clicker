﻿using AC.Model.Models.Application;

namespace AC.ViewModel.ViewModels.ApplicationViewModels
{
    public class ApplicationViewModel : ModelWrapper<Application>
    {
        public int Id
        {
            get
            {
                return Model.Id;
            }
        }
        public string Name
        {
            get
            {
                return Model.Name;
            }
        }
        public Window? Window
        {
            get
            {
                return Model.Window;
            }
        }

        public ApplicationViewModel(Application model) : base(model) { }
    }
}