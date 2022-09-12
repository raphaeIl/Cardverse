using System;
using UnityEngine;

public class Level {

	private float BaseEXP = 0;
	
	public static readonly int[] AscensionLevels = { 30, 50, 60, 75, 100 };

	public event Action OnExperienceChanged;
    public event Action<int, int> OnLevelChanged; // <lastLevel, currentLevel>

	public float CurrentLevel { get { return InverseEvaluate(_totalEXP); } }
	public int RoundedCurrentLevel { get { return Mathf.FloorToInt(CurrentLevel); }}

	public float CurrentXP { get { return _totalEXP - Evaluate(RoundedCurrentLevel); } } // how much xp do you already have on the current level
	public float XPToNextLevel { get { return Evaluate(RoundedCurrentLevel + 1) - Evaluate(RoundedCurrentLevel); } } // total amount of xp required to reach to next level

	public float TotalEXP { get { return _totalEXP; } }
	private float _totalEXP = 0;

	public int MaxLevel { get { return AscensionLevels[_ascensionLevel]; } }
	private int _ascensionLevel;

	private FunctionEvaluator scalingFunction; // the function used for scaling the level, f(x), x being the level, y being the total xp required for that level, f(level) = exp

	public Level(Func<float, float> scalingFunction, float baseEXP) {
		this.scalingFunction = new FunctionEvaluator(scalingFunction);
		this.BaseEXP = baseEXP;
	}

	public Level(Func<float, float> scalingFunction) : this(scalingFunction, 0) { }

	public void AddEXP(int amount) {
		int lastLevel = RoundedCurrentLevel;

		_totalEXP += amount;

		if (_totalEXP >= Evaluate(MaxLevel)) {
			_totalEXP = Evaluate(MaxLevel);
		}

		if (OnExperienceChanged != null)
			OnExperienceChanged();

		if (CurrentLevel > lastLevel) {
			if (OnLevelChanged != null)
				OnLevelChanged(lastLevel, RoundedCurrentLevel);

		}
    }

	public void Ascend() {
		if (CurrentLevel == MaxLevel)
			_ascensionLevel++;
    }

	public float Evaluate(float level) { // level -> total exp
		return BaseEXP + scalingFunction.Evaluate(level);
	}

	public float InverseEvaluate(float exp) { // total exp -> level
		return scalingFunction.InverseEvaluate(exp - BaseEXP) == float.NaN ? MaxLevel : scalingFunction.InverseEvaluate(exp - BaseEXP);
    }

    public override string ToString() {
		return $"Level Stats: [Level {CurrentLevel} with a total xp of {TotalEXP}, progression to next level: {CurrentXP}/{XPToNextLevel}]";
    }
}
