using System;

namespace HWdTech.DS.Internals
{
    public interface IThreadPool
    {
        void StartTask(Action<object> task, object arg);
    }
}
