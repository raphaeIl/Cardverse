using System;

public class FunctionEvaluator {

	private Func<float, float> function; // representing a math function f(x) = ...

	public FunctionEvaluator(Func<float, float> function) {
		this.function = function;
    }

	public float Evaluate(float x) {
		return function(x);
    }

	public float InverseEvaluate(float y) { // I have no idea how to inverse a function in c# so im doing it this way (extremely inefficient)
		for (int i = 0; i <= float.MaxValue; i++) {
			if (function(i) > y) {
				int roundedX = i - 1;
				return roundedX /* + (y - Evaluate(roundedX)) / (Evaluate(roundedX + 1) - Evaluate(roundedX)) */;
			}
		}
		return float.NaN;
	}
}
