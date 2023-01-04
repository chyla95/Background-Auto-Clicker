﻿using System.Collections.ObjectModel;
using System.Collections.Specialized;
using AC.Model.Models;
using AC.ViewModel.ViewModels;

namespace AC.ViewModel.Utilities
{
    public class SynchronizableCollection<TViewModel, TModel> : ObservableCollection<TViewModel>
        where TViewModel : ModelWrapper<TModel>
        where TModel : ModelBase
    {
        private readonly ObservableCollection<TModel> _modelCollection;
        private bool _isSyncActive = true;

        /// <summary>
        /// Creates a new instance of SyncCollection
        /// </summary>
        /// <param name="modelCollection">The list of Models to sync to.</param>
        public SynchronizableCollection(ObservableCollection<TModel> modelCollection)
        {
            _modelCollection = modelCollection;

            // create ViewModels for all Model items in the modelCollection
            foreach (TModel model in _modelCollection) Add(CreateViewModel(model));

            CollectionChanged += SyncroniseModelCollections;
            _modelCollection.CollectionChanged += _modelCollection_CollectionChanged;
        }

        private void _modelCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (!_isSyncActive) return;
            _isSyncActive = false;

            Items.Clear();
            foreach (TModel item in _modelCollection)
            {
                Items.Add(CreateViewModel(item));
            }
            OnCollectionChanged(e);

            _isSyncActive = true;
        }

        private void SyncroniseModelCollections(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (!_isSyncActive) return;
            _isSyncActive = false;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object newItem in e.NewItems)
                    {
                        TViewModel viewModel = (TViewModel)newItem;
                        TModel model = viewModel.Model;
                        _modelCollection.Add(model);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object _ in e.OldItems)
                    {
                        TModel model = _modelCollection.ElementAt(e.OldStartingIndex);
                        _modelCollection.Remove(model);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    _modelCollection.Clear();
                    if (e.NewItems == null) break;
                    foreach (object newItem in e.NewItems)
                    {
                        TViewModel viewModel = CreateViewModel((TModel)newItem);
                        TModel model = viewModel.Model;
                        _modelCollection.Add(model);
                    }
                    break;

                default:
                    throw new NotImplementedException($"Unsuported Action {e.Action}!");
                    // https://stackoverflow.com/a/2177659/20798039
                    // https://stackoverflow.com/a/15831128/20798039
            }

            _isSyncActive = true;
        }

        private static TViewModel CreateViewModel(TModel model)
        {
            TViewModel? vm = (TViewModel)Activator.CreateInstance(typeof(TViewModel), new object[] { model });
            return vm!;
        }

        public TViewModel SingleByModel(TModel model)
        {
            return Items.Single(vm => vm.Model == model);
        }
        public TViewModel? SingleOrDefaultByModel(TModel model)
        {
            return Items.SingleOrDefault(vm => vm.Model == model);
        }
    }
}
