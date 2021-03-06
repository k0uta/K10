using UnityEngine;

[System.Serializable]
public class UIntState : INumericValueState<uint>, ISerializationCallbackReceiver
{
	[SerializeField] uint _value;
	[System.NonSerialized] EventSlot<uint> _onChange = new EventSlot<uint>();

	public uint Value { get { return _value; } set { Setter( value ); } }
	public uint Get() { return _value; }

	public void Setter( uint value )
	{
		if( _value == value ) return;
		_value = value;
		_onChange.Trigger( value );
	}

	public void Increment( uint value = 1 )
	{
		if( value == 0 ) return;
		Setter( _value + value );
	}

	public IEventRegister<uint> OnChange { get { return _onChange; } }

	public UIntState( uint initialValue = default( uint ) ) { _value = initialValue; Init(); }
	void Init()
	{
		if( _onChange == null ) _onChange = new EventSlot<uint>();
	}

	void ISerializationCallbackReceiver.OnBeforeSerialize() { }
	void ISerializationCallbackReceiver.OnAfterDeserialize() { Init(); }


	public override string ToString() { return string.Format( "IS({1})", typeof( uint ).ToString(), _value ); }
}