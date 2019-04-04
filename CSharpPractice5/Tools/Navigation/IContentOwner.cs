using System.Windows.Controls;

namespace CSharpPractice5.Tools.Navigation
{
    internal interface IContentOwner
    {
        ContentControl ContentControl { get; }
    }
}
