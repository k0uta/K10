using UnityEngine;

[System.Serializable]
public class IntState : INumericValueState<int>
{
    [SerializeField] int _value;
    [System.NonSerialized] EventSlot<int> _onChange = new EventSlot<int>();

    public int Value { get { return _value; } set { Setter( value ); } }
    public int Get() { return _value; }

    public void Setter( int value )
    {
        if( _value == value ) return;
        _value = value;
        _onChange.Trigger( value );
	}

	public void Increment( int value = 1 )
	{
		if( value == 0 ) return;
		Setter( _value + value );
	}

    public IEventRegister<int> OnChange { get { return _onChange; } }

    public IntState( int initialValue = default( int ) ) { _value = initialValue; }

    public override string ToString() { return string.Format( "IS({1})", typeof( int ).ToString(), _value ); }
}