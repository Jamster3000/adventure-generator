namespace ConanTableCraft
{
    public class UpdateStatusService
    {
        private bool _updatesMissing;

        public bool UpdatesMissing
        {
            get => _updatesMissing;
            set
            {
                if (_updatesMissing != value)
                {
                    _updatesMissing = value;
                    OnChange?.Invoke();
                }
            }
        }

        public event Action? OnChange;
    }
}
