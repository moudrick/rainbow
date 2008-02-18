using NAnt.Core;
using NAnt.Core.Attributes;

namespace Auxiliaries.NAntTasks
{
    [TaskName("property-ifnotexists")]
    public class LocalExtentionTask_Property_IfNotExists : NAnt.Core.Tasks.PropertyTask
    {
        protected override void ExecuteTask()
        {
            if (!Project.Properties.Contains(PropertyName))
            {
                base.ExecuteTask();
                Log(Level.Info, string.Format("{0}='{1}'", PropertyName, Value));
            }
        }
    }
}
