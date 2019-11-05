using UnityEngine;
using System.Collections.Generic;

public class PoliModifiedValue
{
	private readonly INumericValueState<float> _modifiedValue;
	private readonly IInterpolationFunction _interpolation;

	private readonly List<InterpolatedOverTimeModifier> _overTimeModifiers = new List<InterpolatedOverTimeModifier>();

	public IValueStateObserver<float> ModifiedValue => _modifiedValue;

	float _lastTime = 0;

	public PoliModifiedValue( INumericValueState<float> value, IInterpolationFunction interpolation = null )
	{
		_modifiedValue = value;
		_interpolation = interpolation ?? LinearInterpolation.Instance;
	}

	public void Tick( float deltaTime )
	{
		for( int i = _overTimeModifiers.Count - 1; i >= 0; i-- )
		{
			var mod = _overTimeModifiers[i];
			mod.Tick( deltaTime, _modifiedValue );
			if( mod.IsOver )
			{
				_overTimeModifiers.RemoveAt( i );
				ObjectPool<InterpolatedOverTimeModifier>.Return( mod );
			}
		}
		_lastTime = Time.time;
	}

	public void Clear()
	{
		for( int i = _overTimeModifiers.Count - 1; i >= 0; i-- )
		{
			var mod = _overTimeModifiers[i];
			_overTimeModifiers.RemoveAt( i );
			ObjectPool<InterpolatedOverTimeModifier>.Return( mod );
		}
	}

	public void SetInitialValue( float value ) { _modifiedValue.Setter( value ); }

	public void SetValue( float value, float time = 0 )
	{
		float penddingModifications = 0;
		for( int i = _overTimeModifiers.Count - 1; i >= 0; i-- ) penddingModifications += _overTimeModifiers[i].LastingValue;
		var mod = value - ( _modifiedValue.Value + penddingModifications );
		Add( mod, time );
	}

	public void Add( float value, float time = 0 )
	{
		if( Mathf.Approximately( value, 0 ) ) return;
		if( Mathf.Approximately( time, 0 ) || time < 0 ) _modifiedValue.Increment( value );
		else { _overTimeModifiers.Add( ObjectPool<InterpolatedOverTimeModifier>.Request().Reseted( value, time, _interpolation, _lastTime - Time.time ) ); }
	}
}
