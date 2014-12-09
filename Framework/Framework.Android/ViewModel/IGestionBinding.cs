using System.Windows.MVVM;

namespace Framework.ViewModel
{
    public interface IGestionBinding
    {
        IViewModel DataContext { get; set; }
    }
}