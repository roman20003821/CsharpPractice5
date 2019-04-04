using TaskListView = CSharpPractice5.View.TaskListView;
using ThreadInfoView = CSharpPractice5.View.ThreadInfoView;
using ModulesView = CSharpPractice5.View.ModuleListView;
using System;

namespace CSharpPractice5.Tools.Navigation
{
    internal class InitializationNavigationModel : BaseNavigationModel
    {
        public InitializationNavigationModel(IContentOwner contentOwner) : base(contentOwner)
        {}

        protected override void InitializeView(ViewType viewType)
        {
            switch (viewType)
            {
                case ViewType.TaskManager:
                    ViewsDictionary.Add(viewType, new TaskListView());
                    break;
                case ViewType.ThreadInfo:
                    ViewsDictionary.Add(viewType, new ThreadInfoView());
                    break;
                case ViewType.Modules:
                    ViewsDictionary.Add(viewType, new ModulesView());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(viewType), viewType, null);
            }
        }
    }
}
