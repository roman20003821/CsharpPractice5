using CSharpPractice5.Model;

namespace CSharpPractice5.Tools
{
    internal interface ITaskListOwner
    {
      Task Selected { get; set; }
    }
}
