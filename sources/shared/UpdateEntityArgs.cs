using System;

namespace MyRepository
{
    [Serializable]
	public class UpdateEntityArgs 
	{
        public DateTime? TimeStamp { get; set; }
        public bool EntityCreated { get; set; }
        public bool CancelUpdate { get; set; }
        public object Entity { get; set; }

        public void CopyTo(UpdateEntityArgs args)
        {
            args.TimeStamp = TimeStamp;
            args.EntityCreated = EntityCreated;
            args.CancelUpdate = CancelUpdate;
            args.Entity = Entity;
        }
    }

    public class UpdateEntityArgs<T> : UpdateEntityArgs
	{
        public new T Entity { get; set; }
	}
}
