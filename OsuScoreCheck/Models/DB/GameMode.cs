using ReactiveUI;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OsuScoreCheck.Models.DB
{
    public class GameMode : ReactiveObject
    {
        private int _id;
        private string _name;
        private bool _isChecked;

        public int Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        [NotMapped]
        public bool IsChecked
        {
            get => _isChecked;
            set => this.RaiseAndSetIfChanged(ref _isChecked, value);
        }

        public override bool Equals(object obj)
        {
            if (obj is GameMode other)
            {
                return Id == other.Id && Name == other.Name;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }
    }
}
