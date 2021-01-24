using System;
namespace NetLib{
	public class PacketTypeData<H> : IComparable where H : Delegate {
		public string Name;
		public H Handler = null;

		public PacketTypeData(string name){
			Name = name;
		}

		public PacketTypeData(string name, H handler){
			Name = name;
			Handler = handler;
		}

		public int CompareTo(object obj){
			if(obj == null) return 1;
			PacketTypeData<H> d;
			if((d = obj as PacketTypeData<H>) != null){
				return d.Name.CompareTo(Name);
			}
			throw new ArgumentException("Object is not of type PacketTypeData");
		}
	}
}
