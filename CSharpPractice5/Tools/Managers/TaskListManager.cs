using CSharpPractice5.Model;

namespace CSharpPractice5.Tools.Managers
{
    class TaskListManager
    {
        private static ITaskListOwner _taskListOwner;

        internal static void Initialize(ITaskListOwner owner)
        {
            _taskListOwner = owner;
        }

        internal static Task GetSelected()
        {
            return _taskListOwner.Selected;
        }
    }
}
